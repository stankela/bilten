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
    public partial class RezultatiSpravaFinaleKupaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private Takmicenje takmicenje;

        // kljuc je rezTakmicenja.IndexOf(takmicenje) * (Sprava.Max + 1) + sprava
        private ISet<int> rezultatiOpened;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        private Sprava ActiveSprava
        {
            get { return Sprave.parse(cmbSprava.SelectedItem.ToString()); }
            set { cmbSprava.SelectedItem = Sprave.toString(value); }
        }

        public RezultatiSpravaFinaleKupaForm(int takmicenjeId)
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

                    rezTakmicenja = takmicenje.getRezTakmicenjaSprava(svaRezTakmicenja, DeoTakmicenjaKod.Takmicenje1, true);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje III ni za jednu kategoriju.");

                    initUI();
                    rezultatiOpened = new HashSet<int>();
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

        // TODO4: Podesi na svim mestima sirinu kombo bokseva u kojima se prikazuju rezultatska takmicenja da
        // ceo naziv bude vidljiv.

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(Takmicenje takmicenje)
        {
            IList<RezultatskoTakmicenje> result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenjeFetch_Tak1_PoredakSpravaFinaleKupa(takmicenje.Id);
            foreach (RezultatskoTakmicenje rt in result)
            {
                foreach (PoredakSpravaFinaleKupa p in rt.Takmicenje1.PoredakSpravaFinaleKupa)
                    NHibernateUtil.Initialize(p.Rezultati);
            }
            return result;
        }

        private void initUI()
        {
            Text = "I i II Kolo - rezultati sprave";
            this.ClientSize = new Size(930, 540);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";
            cmbTakmicenje.SelectedIndex = 0;
            cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            List<string> sprave = new List<string>(Sprave.getSpraveNazivi(rezTakmicenja[0].Gimnastika));
            cmbSprava.Items.AddRange(sprave.ToArray());
            cmbSprava.SelectedIndex = 0;
            cmbSprava.SelectedIndexChanged += new EventHandler(cmbSprava_SelectedIndexChanged);
            
            spravaGridUserControl1.DataGridViewUserControl.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            spravaGridUserControl1.SpravaGridMouseUp +=
                new EventHandler<SpravaGridMouseUpEventArgs>(spravaGridUserControl1_SpravaGridMouseUp);
            spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = true;
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (ActiveSprava != Sprava.Preskok)
                dgwuc.onColumnHeaderMouseClick<RezultatSpravaFinaleKupa>(e.DataGridViewCellMouseEventArgs);
            else
                dgwuc.onColumnHeaderMouseClick<RezultatSpravaFinaleKupa>(e.DataGridViewCellMouseEventArgs);
        }

        void cmbTakmicenje_SelectedIndexChanged(object sender, EventArgs e)
        {
            onSelectedRezultatiChanged();
        }

        void cmbSprava_SelectedIndexChanged(object sender, EventArgs e)
        {
            onSelectedRezultatiChanged();
        }

        private void onSelectedRezultatiChanged()
        {
            initSpravaGridUserControl(ActiveSprava);

            int rezultatiKey = getRezultatiKey(ActiveTakmicenje, ActiveSprava);
            if (!rezultatiOpened.Contains(rezultatiKey))
                rezultatiOpened.Add(rezultatiKey);

            setItems();
        }

        private void setItems()
        {
            spravaGridUserControl1.DataGridViewUserControl.setItems<RezultatSpravaFinaleKupa>(
                ActiveTakmicenje.Takmicenje1.getPoredakSpravaFinaleKupa(ActiveSprava).getRezultati());
            spravaGridUserControl1.DataGridViewUserControl.clearSelection();
        }

        private void initSpravaGridUserControl(Sprava sprava)
        {
            // TODO: Kada se promeni sprava trebalo bi da kolone zadrze postojecu sirinu.
            spravaGridUserControl1.init(sprava);

            DataGridViewUserControl dgw = spravaGridUserControl1.DataGridViewUserControl;
            if (dgw.DataGridView.Columns.Count == 0)
            {
                GridColumnsInitializer.initRezultatiSpravaFinaleKupa(dgw, takmicenje, kvalColumnVisible());
                GridColumnsInitializer.maximizeColumnsRezultatiSpravaFinaleKupa(dgw, rezTakmicenja);
            }
            else
            {
                // TODO
                // grid je vec inicijalizovan. podesi da velicine kolona budu nepromenjene.
                //GridColumnsInitializer.reinitRezultatiSpravaFinaleKupaKeepColumnWidths(dgw,
                  //  takmicenje, kvalColumnVisible(), obaPreskoka);
            }
        }

        private bool kvalColumnVisible()
        {
            return ActiveTakmicenje.odvojenoTak3();
        }

        private int getRezultatiKey(RezultatskoTakmicenje tak, Sprava sprava)
        {
            int result = rezTakmicenja.IndexOf(tak) * ((int)Sprava.Max + 1) + (int)sprava;
            return result;
        }

        private void cmbTakmicenje_DropDownClosed(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
        }

        private void cmbSprava_DropDownClosed(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
        }

        private void RezultatiSpravaFinaleKupaForm_Shown(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
            onSelectedRezultatiChanged();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = ActiveTakmicenje.getNazivIzvestajaSprava(DeoTakmicenjaKod.Takmicenje1, true, true);

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje1,
                false, true, false, false, false, false, false);
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

                string documentName;
                if (form.StampajSveSprave)
                {
                    documentName = nazivIzvestaja + " - " + ActiveTakmicenje.Kategorija.Naziv;
                }
                else
                {
                    documentName = nazivIzvestaja + " - " + Sprave.toString(ActiveSprava) + " - "
                        + ActiveTakmicenje.Kategorija.Naziv;
                }
                if (form.StampajSveSprave)
                {
                    List<List<RezultatSpravaFinaleKupa>> rezultatiSprave = new List<List<RezultatSpravaFinaleKupa>>();

                    foreach (Sprava s in Sprave.getSprave(ActiveTakmicenje.Gimnastika))
                        rezultatiSprave.Add(ActiveTakmicenje.Takmicenje1.getPoredakSpravaFinaleKupa(s).getRezultati());
                    p.setIzvestaj(new SpravaFinaleKupaIzvestaj(rezultatiSprave, ActiveTakmicenje.Gimnastika,
                        kvalColumnVisible(), documentName, form.BrojSpravaPoStrani,
                        spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                }
                else
                {
                    List<RezultatSpravaFinaleKupa> rezultati =
                        ActiveTakmicenje.Takmicenje1.getPoredakSpravaFinaleKupa(ActiveSprava).getRezultati();

                    p.setIzvestaj(new SpravaFinaleKupaIzvestaj(ActiveSprava, rezultati,
                        kvalColumnVisible(), documentName, spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                }

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

        void spravaGridUserControl1_SpravaGridMouseUp(object sender, SpravaGridMouseUpEventArgs e)
        {
            DataGridView grid = spravaGridUserControl1.DataGridViewUserControl.DataGridView;
            int x = e.MouseEventArgs.X;
            int y = e.MouseEventArgs.Y;
            if (e.MouseEventArgs.Button == MouseButtons.Right && grid.HitTest(x, y).Type == DataGridViewHitTestType.Cell)
            {
                mnQ.Enabled = /*mnQ.Visible =*/ kvalColumnVisible();
                mnR.Enabled = /*mnR.Visible =*/ kvalColumnVisible();
                mnPrazno.Enabled = /*mnPrazno.Visible =*/ kvalColumnVisible();
                contextMenuStrip1.Show(grid, new Point(x, y));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnQ_Click(object sender, EventArgs e)
        {
            promeniKvalStatus(KvalifikacioniStatus.Q);
        }

        private void mnR_Click(object sender, EventArgs e)
        {
            promeniKvalStatus(KvalifikacioniStatus.R);
        }

        private void mnPrazno_Click(object sender, EventArgs e)
        {
            promeniKvalStatus(KvalifikacioniStatus.None);
        }

        private void promeniKvalStatus(KvalifikacioniStatus kvalStatus)
        {
            RezultatSpravaFinaleKupa rez
                = spravaGridUserControl1.DataGridViewUserControl.getSelectedItem<RezultatSpravaFinaleKupa>();
            int selCount = spravaGridUserControl1.DataGridViewUserControl.getSelectedItems<RezultatSpravaFinaleKupa>().Count;
            if (selCount != 1 || rez.KvalStatus == kvalStatus)
                return;

            string msg = String.Empty;
            if (kvalStatus != KvalifikacioniStatus.None)
            {
                string msgFmt = "Da li zelite da oznacite sa \"{1}\" gimnasticara \"{0}\"?";
                msg = String.Format(msgFmt, rez.Gimnasticar, kvalStatus);
            }
            else
            {
                string msgFmt = "Da li zelite da ponistite oznaku \"{1}\" za gimnasticara \"{0}\"?";
                msg = String.Format(msgFmt, rez.Gimnasticar, rez.KvalStatus);
            }
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    rez.KvalStatus = kvalStatus;
                    DAOFactoryFactory.DAOFactory.GetPoredakSpravaFinaleKupaDAO().Update(
                        ActiveTakmicenje.Takmicenje1.getPoredakSpravaFinaleKupa(ActiveSprava));

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            spravaGridUserControl1.DataGridViewUserControl.refreshItems();
            spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatSpravaFinaleKupa>(rez);
        }

        private void btnIzracunaj_Click(object sender, EventArgs e)
        {
            string msg;
            if (kvalColumnVisible())
                msg = "Da li zelite da izracunate poredak, kvalifikante i rezerve?";
            else
                msg = "Da li zelite da izracunate poredak?";
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
                    
                    PoredakSpravaFinaleKupa p = ActiveTakmicenje.Takmicenje1.getPoredakSpravaFinaleKupa(ActiveSprava);
                        p.create(ActiveTakmicenje, rezTak1, rezTak2);
                    
                    rezultatskoTakmicenjeDAO.Evict(rezTak1);
                    rezultatskoTakmicenjeDAO.Evict(rezTak2);
                    
                    DAOFactoryFactory.DAOFactory.GetPoredakSpravaFinaleKupaDAO().Update(p);

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
