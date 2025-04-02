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
    public partial class RezultatiUkupnoZbirViseKolaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private Takmicenje takmicenje;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiUkupnoZbirViseKolaForm(int takmicenjeId, int startRezTakmicenjeId, bool forViewingOnly)
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
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    rezTakmicenja = takmicenje.getRezTakmicenjaViseboj(svaRezTakmicenja, DeoTakmicenjaKod.Takmicenje1, true);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje II ni za jednu kategoriju.");

                    RezultatskoTakmicenje startRezTakmicenje = null;
                    if (startRezTakmicenjeId != -1)
                    {
                        startRezTakmicenje = findRezTakmicenje(startRezTakmicenjeId, rezTakmicenja);
                        if (startRezTakmicenje == null)
                            throw new BusinessException("Ne postoje rezultati viseboj za dato takmicenje.");
                    }
                    initUI(startRezTakmicenje, forViewingOnly);
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
                .FindByTakmicenjeFetch_KatDesc_Tak1_PoredakUkupnoZbirViseKola_KlubDrzava(takmicenje.Id);
            foreach (RezultatskoTakmicenje rt in result)
            {
                NHibernateUtil.Initialize(rt.Propozicije);
                // NOTE: Moram ovako da inicijalizujem, zato sto ako probam
                // fetch u queriju, jako se sporo izvrsava (verovato
                // zato sto se dobavljaju dve kolekcije - Gimnasticari i 
                // Rezultati).
                NHibernateUtil.Initialize(rt.Takmicenje1.PoredakUkupnoZbirViseKola.Rezultati);
            }
            return result;
        }

        private RezultatskoTakmicenje findRezTakmicenje(int rezTakmicenjeId, IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rt.Id == rezTakmicenjeId)
                    return rt;
            }
            return null;
        }

        private void initUI(RezultatskoTakmicenje startRezTakmicenje, bool forViewingOnly)
        {
            Text = "Rezultati viseboj";
            this.ClientSize = new Size(ClientSize.Width, 540);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";
            cmbTakmicenje.SelectedIndex = 0;
            if (startRezTakmicenje != null)
                ActiveTakmicenje = startRezTakmicenje;
            cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);

            dataGridViewUserControl1.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
        
            if (forViewingOnly)
            {
                btnPrint.Enabled = btnPrint.Visible = false;
                btnIzracunaj.Enabled = btnIzracunaj.Visible = false;
            }
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<RezultatUkupnoZbirViseKola>(e.DataGridViewCellMouseEventArgs);
        }

        void cmbTakmicenje_SelectedIndexChanged(object sender, EventArgs e)
        {
            onSelectedTakmicenjeChanged();
        }

        private void onSelectedTakmicenjeChanged()
        {
            if (dataGridViewUserControl1.DataGridView.Columns.Count == 0)
            {
                GridColumnsInitializer.initRezultatiUkupnoZbirViseKola(dataGridViewUserControl1, takmicenje);
                GridColumnsInitializer.maximizeColumnsRezultatiUkupnoZbirViseKola(dataGridViewUserControl1, rezTakmicenja);
            }
            else
            {
                // TODO
                // grid je vec inicijalizovan. podesi da velicine kolona budu nepromenjene.
                //GridColumnsInitializer.reinitRezultatiUkupnoKeepColumnWidths(dataGridViewUserControl1,
                  //  takmicenje, kvalColumnVisible());
            }
            setItems();
        }

        private void setItems()
        {
            dataGridViewUserControl1.setItems<RezultatUkupnoZbirViseKola>(
                ActiveTakmicenje.Takmicenje1.PoredakUkupnoZbirViseKola.getRezultati());
            dataGridViewUserControl1.clearSelection();
        }

        private void cmbTakmicenja_DropDownClosed(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
        }

        private void RezultatiUkupnoZbirViseKolaForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            onSelectedTakmicenjeChanged();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = ActiveTakmicenje.getNazivIzvestajaViseboj(DeoTakmicenjaKod.Takmicenje1, false, true);
            string documentName = nazivIzvestaja + " - " + ActiveTakmicenje.Kategorija.Naziv;

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje1,
                true, false, false, false, false, false, false, false, false, false, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  " 
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = ActiveTakmicenje.Kategorija.Naziv;
                form.Header4Text = nazivIzvestaja;
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header3Text = ActiveTakmicenje.Kategorija.Naziv;
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

                List<RezultatUkupnoZbirViseKola> rezultati
                    = ActiveTakmicenje.Takmicenje1.PoredakUkupnoZbirViseKola.getRezultati();

                p.setIzvestaj(new UkupnoZbirViseKolaIzvestaj(rezultati, ActiveTakmicenje.Gimnastika,
                    Opcije.Instance.PrikaziDEOcene, dataGridViewUserControl1.DataGridView,
                    documentName, takmicenje.getBrojKola(), takmicenje));
                p.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
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

                    RezultatskoTakmicenje rezTak1 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(
                        takmicenje.PrvoKolo.Id, ActiveTakmicenje.Kategorija.Naziv, 0);
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

                    ActiveTakmicenje.Takmicenje1.PoredakUkupnoZbirViseKola.create(ActiveTakmicenje,
                        rezTak1, rezTak2, rezTak3, rezTak4);

                    rezultatskoTakmicenjeDAO.Evict(rezTak1);
                    rezultatskoTakmicenjeDAO.Evict(rezTak2);
                    if (rezTak3 != null)
                        rezultatskoTakmicenjeDAO.Evict(rezTak3);
                    if (rezTak4 != null)
                        rezultatskoTakmicenjeDAO.Evict(rezTak4);

                    DAOFactoryFactory.DAOFactory.GetPoredakUkupnoZbirViseKolaDAO()
                        .Update(ActiveTakmicenje.Takmicenje1.PoredakUkupnoZbirViseKola);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
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