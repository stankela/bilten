using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Exceptions;
using NHibernate;
using Bilten.Report;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class RezultatiEkipeZbirViseKolaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private bool[] takmicenjeOpened;
        private Takmicenje takmicenje;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiEkipeZbirViseKolaForm(int takmicenjeId)
        {
            InitializeComponent();

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);

                    IList<RezultatskoTakmicenje> svaRezTakmicenja = loadRezTakmicenja(takmicenje);
                    if (svaRezTakmicenja.Count == 0)
                        throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                    rezTakmicenja = takmicenje.getRezTakmicenjaEkipe(svaRezTakmicenja, DeoTakmicenjaKod.Takmicenje1, true);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje IV ni za jednu kategoriju.");

                    initUI();
                    takmicenjeOpened = new bool[rezTakmicenja.Count];
                    cmbTakmicenje.SelectedIndex = 0;

                    cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);

                    //onSelectedTakmicenjeChanged();
                }
            }
            catch (BusinessException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            catch (InfrastructureException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(Takmicenje takmicenje)
        {
            IList<RezultatskoTakmicenje> result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenjeFetch_Tak1_PoredakEkipnoZbirViseKola(takmicenje.Id);

            foreach (RezultatskoTakmicenje tak in result)
                NHibernateUtil.Initialize(tak.Takmicenje1.PoredakEkipnoZbirViseKola.Rezultati);
            return result;
        }

        private void initUI()
        {
            Text = "Rezultati ekipno";

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "NazivEkipnog";

            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<RezultatEkipnoZbirViseKola>(e.DataGridViewCellMouseEventArgs);
        }

        void cmbTakmicenje_SelectedIndexChanged(object sender, EventArgs e)
        {
            onSelectedTakmicenjeChanged();
        }

        private void onSelectedTakmicenjeChanged()
        {
            GridColumnsInitializer.initRezultatiEkipnoZbirViseKola(dataGridViewUserControl1, takmicenje);
            
            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;
            setItems();
        }

        private void setItems()
        {
            dataGridViewUserControl1.setItems<RezultatEkipnoZbirViseKola>(
                ActiveTakmicenje.Takmicenje1.PoredakEkipnoZbirViseKola.getRezultati());
            dataGridViewUserControl1.clearSelection();
        }

        private void RezultatiEkipeZbirViseKolaForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            cmbTakmicenje_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void cmbTakmicenje_DropDownClosed(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja;
            nazivIzvestaja = "Rezultati ekipno";

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje1,
                false, false, false, false, false, false, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = ActiveTakmicenje.Naziv;
                form.Header4Text = nazivIzvestaja;
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = ActiveTakmicenje.Naziv;
                form.Header4Text = nazivIzvestaja;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initHeaderFooterFromForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                PreviewDialog p = new PreviewDialog();

                List<RezultatEkipnoZbirViseKola> rezultatiEkipno =
                    ActiveTakmicenje.Takmicenje1.PoredakEkipnoZbirViseKola.getRezultati();

                p.setIzvestaj(new EkipeZbirViseKolaIzvestaj(rezultatiEkipno,
                    ActiveTakmicenje.Gimnastika, dataGridViewUserControl1.DataGridView,
                    nazivIzvestaja));
                p.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void btnZatvori_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnIzracunaj_Click(object sender, EventArgs e)
        {
            string msg = "Da li zelite da izracunate poredak?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

                    RezultatskoTakmicenje rezTak1 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari
                        (takmicenje.PrvoKolo.Id, ActiveTakmicenje.Kategorija.Naziv, 0);
                    RezultatskoTakmicenje rezTak2 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(
                        takmicenje.DrugoKolo.Id, ActiveTakmicenje.Kategorija.Naziv, 0);
                    RezultatskoTakmicenje rezTak3 = null;
                    if (takmicenje.TreceKolo != null)
                    {
                        rezTak3 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(
                            takmicenje.TreceKolo.Id, ActiveTakmicenje.Kategorija.Naziv, 0);
                    }
                    RezultatskoTakmicenje rezTak4 = null;
                    if (takmicenje.CetvrtoKolo != null)
                    {
                        rezTak4 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(
                            takmicenje.CetvrtoKolo.Id, ActiveTakmicenje.Kategorija.Naziv, 0);
                    }

                    ActiveTakmicenje.Takmicenje1.PoredakEkipnoZbirViseKola.create(ActiveTakmicenje,
                        rezTak1, rezTak2, rezTak3, rezTak4);

                    rezultatskoTakmicenjeDAO.Evict(rezTak1);
                    rezultatskoTakmicenjeDAO.Evict(rezTak2);
                    if (rezTak3 != null)
                        rezultatskoTakmicenjeDAO.Evict(rezTak3);
                    if (rezTak4 != null)
                        rezultatskoTakmicenjeDAO.Evict(rezTak4);

                    DAOFactoryFactory.DAOFactory.GetPoredakEkipnoZbirViseKolaDAO()
                        .Update(ActiveTakmicenje.Takmicenje1.PoredakEkipnoZbirViseKola);

                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            setItems();
        }

    }
}