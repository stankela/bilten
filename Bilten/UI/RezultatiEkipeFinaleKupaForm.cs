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
    public partial class RezultatiEkipeFinaleKupaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private bool[] takmicenjeOpened;
        private Takmicenje takmicenje;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiEkipeFinaleKupaForm(int takmicenjeId)
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
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            IList<RezultatskoTakmicenje> rezTakmicenjaPrvoKolo
                = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakEkipno(takmicenje.PrvoKolo.Id);
            IList<RezultatskoTakmicenje> rezTakmicenjaDrugoKolo
                = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakEkipno(takmicenje.DrugoKolo.Id);

            IList<RezultatskoTakmicenje> result = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_Ekipe(takmicenje.Id);

            foreach (RezultatskoTakmicenje tak in result)
            {
                // potrebno u Poredak.create
                NHibernateUtil.Initialize(tak.Propozicije);

                PoredakEkipno poredak1 =
                    Takmicenje.getRezTakmicenje(rezTakmicenjaPrvoKolo, tak.Kategorija).Takmicenje1.PoredakEkipno;
                PoredakEkipno poredak2 =
                    Takmicenje.getRezTakmicenje(rezTakmicenjaDrugoKolo, tak.Kategorija).Takmicenje1.PoredakEkipno;
                tak.Takmicenje1.PoredakEkipnoFinaleKupa.create(tak, poredak1, poredak2);
            }
            return result;
        }

        private void initUI()
        {
            Text = "I i II Kolo - rezultati ekipno";

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
                dgwuc.onColumnHeaderMouseClick<RezultatEkipnoFinaleKupa>(e.DataGridViewCellMouseEventArgs);
        }

        void cmbTakmicenje_SelectedIndexChanged(object sender, EventArgs e)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    if (onSelectedTakmicenjeChanged())
                        session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private bool kvalColumnVisible()
        {
            return ActiveTakmicenje.postojeKvalifikacijeEkipno(DeoTakmicenjaKod.Takmicenje1);
        }

        private bool onSelectedTakmicenjeChanged()
        {
            GridColumnsInitializer.initRezultatiEkipnoFinaleKupa(dataGridViewUserControl1,
                takmicenje, kvalColumnVisible());
            
            bool save = false;
            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
            {
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;
                //ActiveTakmicenje.Takmicenje1.PoredakEkipno.create(ActiveTakmicenje, ocene);
                //dataContext.Save(ActiveTakmicenje.Takmicenje1.PoredakEkipno);
                //save = true;
            }
            setItems();

            return save;
        }

        private void setItems()
        {
            dataGridViewUserControl1.setItems<RezultatEkipnoFinaleKupa>(
                ActiveTakmicenje.Takmicenje1.PoredakEkipnoFinaleKupa.getRezultati());
            dataGridViewUserControl1.clearSelection();
        }

        private void RezultatiEkipeFinaleKupaForm_Shown(object sender, EventArgs e)
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
            nazivIzvestaja = "I i II kolo - Rezultati ekipno";

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

                List<RezultatEkipnoFinaleKupa> rezultatiEkipno =
                    ActiveTakmicenje.Takmicenje1.PoredakEkipnoFinaleKupa.getRezultati();

                p.setIzvestaj(new EkipeFinaleKupaIzvestaj(rezultatiEkipno,
                    ActiveTakmicenje.Gimnastika, kvalColumnVisible(), dataGridViewUserControl1.DataGridView,
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

    }
}