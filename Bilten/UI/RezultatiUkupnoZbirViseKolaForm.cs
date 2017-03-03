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
        private bool[] takmicenjeOpened;
        private Takmicenje takmicenje;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiUkupnoZbirViseKolaForm(int takmicenjeId)
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

                    rezTakmicenja = takmicenje.getRezTakmicenjaViseboj(svaRezTakmicenja, DeoTakmicenjaKod.Takmicenje1, true);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje II ni za jednu kategoriju.");

                    initUI();
                    takmicenjeOpened = new bool[rezTakmicenja.Count];
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(Takmicenje takmicenje)
        {
            IList<RezultatskoTakmicenje> rezTakmicenjaPrvoKolo = null;
            IList<RezultatskoTakmicenje> rezTakmicenjaDrugoKolo = null;
            IList<RezultatskoTakmicenje> rezTakmicenjaTreceKolo = null;
            IList<RezultatskoTakmicenje> rezTakmicenjaCetvrtoKolo = null;

            RezultatskoTakmicenjeDAO rezTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

            if (takmicenje.PrvoKolo != null)
                rezTakmicenjaPrvoKolo = rezTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakUkupno(takmicenje.PrvoKolo.Id);
            if (takmicenje.DrugoKolo != null)
                rezTakmicenjaDrugoKolo = rezTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakUkupno(takmicenje.DrugoKolo.Id);
            if (takmicenje.TreceKolo != null)
                rezTakmicenjaTreceKolo = rezTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakUkupno(takmicenje.TreceKolo.Id);
            if (takmicenje.CetvrtoKolo != null)
                rezTakmicenjaCetvrtoKolo = rezTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakUkupno(takmicenje.CetvrtoKolo.Id);

            IList<RezultatskoTakmicenje> result = rezTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_Gimnasticari(takmicenje.Id);
            foreach (RezultatskoTakmicenje rt in result)
            {
                // potrebno u Poredak.create
                NHibernateUtil.Initialize(rt.Propozicije);
                takmicenje.createPoredakUkupnoZbirViseKola(rt, rezTakmicenjaPrvoKolo, rezTakmicenjaDrugoKolo,
                    rezTakmicenjaTreceKolo, rezTakmicenjaCetvrtoKolo);
            }
            return result;
        }

        private void initUI()
        {
            Text = "Rezultati viseboj";
            this.ClientSize = new Size(ClientSize.Width, 540);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";
            cmbTakmicenje.SelectedIndex = 0;
            cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);

            dataGridViewUserControl1.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
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
            cmdSelectedTakmicenjeChanged();
        }

        void cmdSelectedTakmicenjeChanged()
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    onSelectedTakmicenjeChanged();
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

        private void onSelectedTakmicenjeChanged()
        {
            if (dataGridViewUserControl1.DataGridView.Columns.Count == 0)
            {
                GridColumnsInitializer.initRezultatiUkupnoZbirViseKola(dataGridViewUserControl1,
                    takmicenje, kvalColumnVisible());
                GridColumnsInitializer.maximizeColumnsRezultatiUkupnoZbirViseKola(dataGridViewUserControl1,
                    rezTakmicenja);
            }
            else
            {
                // TODO
                // grid je vec inicijalizovan. podesi da velicine kolona budu nepromenjene.
                //GridColumnsInitializer.reinitRezultatiUkupnoKeepColumnWidths(dataGridViewUserControl1,
                  //  takmicenje, kvalColumnVisible());
            }
            
            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;

            setItems();
        }

        private void setItems()
        {
            dataGridViewUserControl1.setItems<RezultatUkupnoZbirViseKola>(
                ActiveTakmicenje.Takmicenje1.PoredakUkupnoZbirViseKola.getRezultati());
            dataGridViewUserControl1.clearSelection();
        }

        private bool kvalColumnVisible()
        {
            return ActiveTakmicenje.postojeKvalifikacijeViseboj(DeoTakmicenjaKod.Takmicenje1);
        }

        private void cmbTakmicenja_DropDownClosed(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
        }

        private void RezultatiUkupnoZbirViseKolaForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            cmdSelectedTakmicenjeChanged();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = ActiveTakmicenje.getNazivIzvestajaViseboj(DeoTakmicenjaKod.Takmicenje1, false, true);
            string documentName = nazivIzvestaja + " - " + ActiveTakmicenje.Kategorija.Naziv;

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje1,
                true, false, false, false, false, false, false);
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
                    Opcije.Instance.PrikaziDEOcene, kvalColumnVisible(), dataGridViewUserControl1.DataGridView,
                    documentName));
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}