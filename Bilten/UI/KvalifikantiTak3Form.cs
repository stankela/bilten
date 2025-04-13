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
using System.Collections;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Report;

namespace Bilten.UI
{
    public partial class KvalifikantiTak3Form : Form
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

        public KvalifikantiTak3Form(int takmicenjeId)
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
                    IList<RezultatskoTakmicenje> svaRezTakmicenja = loadRezTakmicenja(takmicenjeId);
                    if (svaRezTakmicenja.Count == 0)
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    rezTakmicenja = new List<RezultatskoTakmicenje>();
                    foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                    {
                        if (rt.odvojenoTak3())
                            rezTakmicenja.Add(rt);
                    }
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji odvojeno takmicenje III ni za jednu kategoriju.");

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);

                    initUI();
                    rezultatiOpened = new HashSet<int>();
                    cmbTakmicenje.SelectedIndex = 0;
                    cmbSprava.SelectedIndex = 0;

                    cmbTakmicenje.SelectedIndexChanged += new EventHandler(selectedRezultatiChanged);
                    cmbSprava.SelectedIndexChanged += new EventHandler(selectedRezultatiChanged);

                    //onSelectedRezultatiChanged();
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

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {
            IList<RezultatskoTakmicenje> result
                = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().FindByTakmicenjeFetchTakmicenje3(takmicenjeId);
            foreach (RezultatskoTakmicenje rt in result)
            {
                NHibernateUtil.Initialize(rt.Propozicije);
                NHibernateUtil.Initialize(rt.Takmicenje);
                if (rt.odvojenoTak3())
                {
                    foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                        NHibernateUtil.Initialize(p.Rezultati);
                    NHibernateUtil.Initialize(rt.Takmicenje3.PoredakPreskok.Rezultati);
                }
            }
            return result;
        }

        private void initUI()
        {
            Text = "Kvalifikanti - " 
                + DeoTakmicenjaKodovi.toString(DeoTakmicenjaKod.Takmicenje3);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            List<string> sprave = new List<string>(Sprave.getSpraveNazivi(takmicenje.Gimnastika));
            cmbSprava.Items.AddRange(sprave.ToArray());

            spravaGridUserControl1.DataGridViewUserControl.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = true;

            dataGridViewUserControl1.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            GridColumnsInitializer.initKvalifikantiTak3(dataGridViewUserControl1,
                takmicenje);
            dataGridViewUserControl1.DataGridView.MultiSelect = true;

            this.ClientSize = new Size(ClientSize.Width, 550);
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
           GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            dgwuc.onColumnHeaderMouseClick<UcesnikTakmicenja3>(e.DataGridViewCellMouseEventArgs);
        }

        void selectedRezultatiChanged(object sender, EventArgs e)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    onSelectedRezultatiChanged();
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
        }

        private void onSelectedRezultatiChanged()
        {
            initSpravaGridUserControl(ActiveSprava);

            int rezultatiKey = getRezultatiKey(ActiveTakmicenje, ActiveSprava);
            if (!rezultatiOpened.Contains(rezultatiKey))
            {
                rezultatiOpened.Add(rezultatiKey);
            }

            refreshKvalifikantiIRezerve();
            spravaGridUserControl1.DataGridViewUserControl.clearSelection();
            dataGridViewUserControl1.clearSelection();
        }

        private void refreshKvalifikantiIRezerve()
        {
            spravaGridUserControl1.DataGridViewUserControl
                .setItems<UcesnikTakmicenja3>(ActiveTakmicenje.Takmicenje3.getUcesniciKvalifikanti(ActiveSprava));
            dataGridViewUserControl1.setItems<UcesnikTakmicenja3>(
                ActiveTakmicenje.Takmicenje3.getUcesniciRezerve(ActiveSprava));
        }

        private void initSpravaGridUserControl(Sprava sprava)
        {
            spravaGridUserControl1.init(sprava);

            GridColumnsInitializer.initKvalifikantiTak3(
                spravaGridUserControl1.DataGridViewUserControl, takmicenje);
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

        private void KvalifikantiTak3Form_Shown(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
            selectedRezultatiChanged(null, EventArgs.Empty);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult;
            KvalifikantiTak3EditorForm form;
            try
            {
                form = new KvalifikantiTak3EditorForm(takmicenje.Id, ActiveTakmicenje.Id, ActiveSprava);
                dlgResult = form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK)
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    // ponovo ucitaj takmicenje
                    RezultatskoTakmicenje rt = loadRezTakmicenje(ActiveTakmicenje.Id);
                    int index;
                    for (index = 0; index < rezTakmicenja.Count; index++)
                    {
                        if (rezTakmicenja[index].Id == rt.Id)
                            break;
                    }
                    rezTakmicenja[index] = rt;
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            CurrencyManager currencyManager =
                (CurrencyManager)this.BindingContext[cmbTakmicenje.DataSource];
            currencyManager.Refresh();
            refreshKvalifikantiIRezerve();

            spravaGridUserControl1.DataGridViewUserControl.clearSelection();
            dataGridViewUserControl1.clearSelection();
        }

        private RezultatskoTakmicenje loadRezTakmicenje(int rezTakmicenjeId)
        {
            RezultatskoTakmicenje result
                = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().FindByIdFetchTakmicenje3(rezTakmicenjeId);

            if (result != null)
            {
                NHibernateUtil.Initialize(result.Propozicije);
                NHibernateUtil.Initialize(result.Takmicenje);
                foreach (PoredakSprava p in result.Takmicenje3.Poredak)
                    NHibernateUtil.Initialize(p.Rezultati);
                NHibernateUtil.Initialize(result.Takmicenje3.PoredakPreskok.Rezultati);
            }
            return result;
        }

        // TODO5: Fali dugme "Promeni rezerve"

        private void btnStampaj_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = Opcije.Instance.SpraveFinalisti;

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje3, false, true, false, false, false, false, false, false,
                                                         false, false, false);
            string gym = GimnastikaUtil.getGimnastikaStr(takmicenje.Gimnastika, Opcije.Instance.Jezik);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = ActiveTakmicenje.Kategorija.Naziv;
                form.FooterText = mestoDatum;
                if (takmicenje.Gimnastika == Gimnastika.ZSG)
                    form.BrojSpravaPoStrani = 4;
                else
                    form.BrojSpravaPoStrani = 6;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = ActiveTakmicenje.Kategorija.Naziv;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initOpcijeFromHeaderFooterForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                PreviewDialog p = new PreviewDialog();

                string documentName = gym + " - " + nazivIzvestaja + " - " + ActiveTakmicenje.Kategorija.Naziv;
                bool obaPresk = ActiveTakmicenje.Propozicije.Tak1PreskokNaOsnovuObaPreskoka;

                if (form.StampajSveSprave)
                {
                    List<IList<UcesnikTakmicenja3>> kvalifikanti = new List<IList<UcesnikTakmicenja3>>();
                    List<IList<UcesnikTakmicenja3>> rezerve = new List<IList<UcesnikTakmicenja3>>();
                    foreach (Sprava s in Sprave.getSprave(ActiveTakmicenje.Gimnastika))
                    {
                        kvalifikanti.Add(ActiveTakmicenje.Takmicenje3.getUcesniciKvalifikanti(s));
                        rezerve.Add(ActiveTakmicenje.Takmicenje3.getUcesniciRezerve(s));
                    }
                    p.setIzvestaj(new KvalifikantiTak3Izvestaj(kvalifikanti, rezerve,
                        takmicenje.Gimnastika, documentName, form.BrojSpravaPoStrani,
                        spravaGridUserControl1.DataGridViewUserControl.DataGridView, takmicenje,
                        new Font(form.TekstFont, form.TekstFontSize)));
                }
                else
                {
                    IList<UcesnikTakmicenja3> kvalifikanti = ActiveTakmicenje.Takmicenje3.getUcesniciKvalifikanti(ActiveSprava);
                    IList<UcesnikTakmicenja3> rezerve = ActiveTakmicenje.Takmicenje3.getUcesniciRezerve(ActiveSprava);
                    p.setIzvestaj(new KvalifikantiTak3Izvestaj(kvalifikanti, rezerve, ActiveSprava, documentName,
                        spravaGridUserControl1.DataGridViewUserControl.DataGridView, takmicenje,
                        new Font(form.TekstFont, form.TekstFontSize)));
                }
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
    }
}
