using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Data;
using Iesi.Collections.Generic;
using Bilten.Util;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class TakmicarskeKategorijeForm : Form
    {
        private Takmicenje takmicenje;
        private List<TakmicarskaKategorija> originalKategorije;        
        private List<RezultatskoTakmicenjeDescription> originalTakmicenja;

        public TakmicarskeKategorijeForm(int takmicenjeId)
        {
            // TODO: Dodaj strelice umesto "Pomeri gore" i "Pomeri dole"

            // TODO3: Probaj da se prvo definisu takmicenja, pa zatim kategorije, i da
            // bude moguce da se za dato takmicenje menjaju kategorije, tj. da ne vazi
            // da sva takmicenja imaju sve kategorije.

            InitializeComponent();
            btnDeleteKategorija.Enabled = false;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindByIdFetch_Kat_Desc(takmicenjeId);

                    originalKategorije = new List<TakmicarskaKategorija>(takmicenje.Kategorije);
                    originalTakmicenja =
                        new List<RezultatskoTakmicenjeDescription>(takmicenje.TakmicenjeDescriptions);

                    initUI();
                }
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
            }
        }

        private void initUI()
        {
            Text = "Takmicarske kategorije";

            setKategorije(takmicenje.Kategorije);
            SelectedKategorija = null;

            setTakmicenja(takmicenje.TakmicenjeDescriptions);
            SelectedTakmicenje = null;
        }

        private void setKategorije(Iesi.Collections.Generic.ISet<TakmicarskaKategorija> kategorije)
        {
            List<TakmicarskaKategorija> katList = new List<TakmicarskaKategorija>(kategorije);

            // TODO: Kreiraj metod u klasi TakmicarskaKategorija koji vraca kategorije sortirane po rednom broju.
            // Pronadji sva mesta na kojima sortiram kategorije po rednom broju, i zameni ih pozivom novog metoda.
            // Uradi isto i za klasu RezultatskoTakmicenjeDescription, a i za druge ako postoje.

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            katList.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

            lstKategorije.DataSource = katList;
        }

        private TakmicarskaKategorija SelectedKategorija
        {
            get { return lstKategorije.SelectedItem as TakmicarskaKategorija; }
            set { lstKategorije.SelectedItem = value; }
        }

        private void setTakmicenja(Iesi.Collections.Generic.ISet<RezultatskoTakmicenjeDescription> takmicenja)
        {
            List<RezultatskoTakmicenjeDescription> takList = new List<RezultatskoTakmicenjeDescription>(takmicenja);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenjeDescription))["RedBroj"];
            takList.Sort(new SortComparer<RezultatskoTakmicenjeDescription>(
                propDesc, ListSortDirection.Ascending));

            lstTakmicenja.DataSource = takList;
            lstTakmicenja.DisplayMember = "Naziv";
        }

        private RezultatskoTakmicenjeDescription SelectedTakmicenje
        {
            get { return lstTakmicenja.SelectedItem as RezultatskoTakmicenjeDescription; }
            set { lstTakmicenja.SelectedItem = value; }
        }

        private void btnAddKategorija_Click(object sender, EventArgs e)
        {
            string msg = "Izaberite kategorije gimnasticara";
            IList<TakmicarskaKategorija> katList =
                new List<TakmicarskaKategorija>(takmicenje.Kategorije);
            DialogResult dlgResult = DialogResult.None;
            SelectKategorijaForm form = null;
            try
            {
                form = new SelectKategorijaForm(-1, takmicenje.Gimnastika, katList, msg);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelektovaneKategorije.Count == 0)
                return;
            
            foreach (TakmicarskaKategorija k in form.SelektovaneKategorije)
                takmicenje.addKategorija(k);
            setKategorije(takmicenje.Kategorije);
        }

        private void btnDeleteKategorija_Click(object sender, EventArgs e)
        {
            // TODO: Uradi brisanje kategorija
            if (SelectedKategorija == null)
                return;
            string msgFmt = "Da li zelite da izbrisete kategoriju '{0}'?";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, SelectedKategorija), this.Text))
                return;

            // TODO: Prikazi dijalog za potvrdu da ce biti izbrisane sve ocene,
            // rasporedi nastupa, rasporedi sudija, ekipe i gimnasticari za datu
            // kategoriju

            takmicenje.removeKategorija(SelectedKategorija);
            setKategorije(takmicenje.Kategorije);
        }

        private void btnMoveUpKategorija_Click(object sender, EventArgs e)
        {
            TakmicarskaKategorija k = SelectedKategorija;
            if (k == null)
                return;

            if (takmicenje.moveKategorijaUp(k))
            {
                setKategorije(takmicenje.Kategorije);
                SelectedKategorija = k;
            }
        }

        private void btnMoveDownKategorija_Click(object sender, EventArgs e)
        {
            TakmicarskaKategorija k = SelectedKategorija;
            if (k == null)
                return;

            if (takmicenje.moveKategorijaDown(k))
            {
                setKategorije(takmicenje.Kategorije);
                SelectedKategorija = k;
            }
        }

        private void btnAddTakmicenje_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatskoTakmicenjeDescriptionForm form =
                    new RezultatskoTakmicenjeDescriptionForm(takmicenje);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RezultatskoTakmicenjeDescription d = 
                        (RezultatskoTakmicenjeDescription)form.Entity;
                    takmicenje.addTakmicenjeDescription(d);
                    setTakmicenja(takmicenje.TakmicenjeDescriptions);
                    SelectedTakmicenje = d;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private void btnEditTakmicenje_Click(object sender, EventArgs e)
        {
            if (SelectedTakmicenje == null)
                return;
            if (SelectedTakmicenje.RedBroj == 0)
            {
                MessageDialogs.showMessage("Nije dozvoljeno menjati naziv prvog takmicenja.", this.Text);
                return;
            }

            try
            {
                RezultatskoTakmicenjeDescriptionForm form =
                    new RezultatskoTakmicenjeDescriptionForm(SelectedTakmicenje, takmicenje);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // refresh
                    setTakmicenja(takmicenje.TakmicenjeDescriptions);
                    SelectedTakmicenje = (RezultatskoTakmicenjeDescription)form.Entity;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private void btnDeleteTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescription takmicenjeDesc =
                SelectedTakmicenje;
            if (takmicenjeDesc == null)
                return;

            string msgFmt = "Da li zelite da izbrisete takmicenje '{0}'?";
            if (!MessageDialogs.queryConfirmation(
                String.Format(msgFmt, takmicenjeDesc.Naziv), this.Text))
                return;

            try
            {
                takmicenje.removeTakmicenjeDescription(takmicenjeDesc);
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, this.Text);
                return;
            }
            setTakmicenja(takmicenje.TakmicenjeDescriptions);
        }

        private void btnMoveUpTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescription takmicenjeDesc =
                SelectedTakmicenje;
            if (takmicenjeDesc == null)
                return;

            bool moved = false;
            try
            {
                moved = takmicenje.moveTakmicenjeDescriptionUp(takmicenjeDesc);
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, this.Text);
                return;
            }
            if (moved)
            {
                setTakmicenja(takmicenje.TakmicenjeDescriptions);
                SelectedTakmicenje = takmicenjeDesc;
            }
        }

        private void btnMoveDownTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescription takmicenjeDesc =
                SelectedTakmicenje;
            if (takmicenjeDesc == null)
                return;

            bool moved = false;
            try
            {
                moved = takmicenje.moveTakmicenjeDescriptionDown(takmicenjeDesc);
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, this.Text);
                return;
            }
            if (moved)
            {
                setTakmicenja(takmicenje.TakmicenjeDescriptions);
                SelectedTakmicenje = takmicenjeDesc;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    // TODO: Prvo proveri da li je nesto menjano

                    Notification notification = new Notification();
                    validate(notification);
                    if (!notification.IsValid())
                        throw new BusinessException(notification);

                    List<TakmicarskaKategorija> addedKat = new List<TakmicarskaKategorija>();
                    List<TakmicarskaKategorija> updatedKat = new List<TakmicarskaKategorija>();
                    List<TakmicarskaKategorija> deletedKat = new List<TakmicarskaKategorija>();
                    diff(new List<TakmicarskaKategorija>(takmicenje.Kategorije),
                        originalKategorije, addedKat, updatedKat, deletedKat);

                    List<RezultatskoTakmicenjeDescription> addedTak =
                        new List<RezultatskoTakmicenjeDescription>();
                    List<RezultatskoTakmicenjeDescription> updatedTak =
                        new List<RezultatskoTakmicenjeDescription>();
                    List<RezultatskoTakmicenjeDescription> deletedTak =
                        new List<RezultatskoTakmicenjeDescription>();
                    diff(new List<RezultatskoTakmicenjeDescription>(takmicenje.TakmicenjeDescriptions),
                        originalTakmicenja, addedTak, updatedTak, deletedTak);

                    foreach (TakmicarskaKategorija kat in deletedKat)
                    {
                        foreach (RezultatskoTakmicenjeDescription desc in updatedTak)
                        {
                            deleteRezultatskoTakmicenje(kat, desc);
                        }
                        foreach (RezultatskoTakmicenjeDescription desc in deletedTak)
                        {
                            deleteRezultatskoTakmicenje(kat, desc);
                        }
                    }
                    foreach (RezultatskoTakmicenjeDescription desc in deletedTak)
                    {
                        foreach (TakmicarskaKategorija kat in updatedKat)
                        {
                            deleteRezultatskoTakmicenje(kat, desc);
                        }
                    }

                    foreach (RezultatskoTakmicenjeDescription t in deletedTak)
                        DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO().Delete(t);
                    foreach (TakmicarskaKategorija k in deletedKat)
                        deleteKategorija(k);

                    foreach (RezultatskoTakmicenjeDescription d in addedTak)
                        d.Propozicije = new Propozicije();

                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);

                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    foreach (TakmicarskaKategorija k in addedKat)
                    {
                        foreach (RezultatskoTakmicenjeDescription d in addedTak)
                        {
                            rezTakDAO.Add(createRezultatskoTakmicenje(takmicenje, k, d));
                        }
                        foreach (RezultatskoTakmicenjeDescription d in updatedTak)
                        {
                            rezTakDAO.Add(createRezultatskoTakmicenje(takmicenje, k, d));
                        }
                    }
                    foreach (RezultatskoTakmicenjeDescription d in addedTak)
                    {
                        foreach (TakmicarskaKategorija k in updatedKat)
                        {
                            rezTakDAO.Add(createRezultatskoTakmicenje(takmicenje, k, d));
                        }
                    }

                    // kreiraj redni broj
                    PropertyDescriptor[] propDesc = {
                        TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenje))["TakmicenjeDescriptionRedBroj"],
                        TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenje))["KategorijaRedBroj"]
                    };
                    ListSortDirection[] direction = {
                        ListSortDirection.Ascending,
                        ListSortDirection.Ascending
                    };
                    List<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>(
                        DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().FindByTakmicenje(takmicenje.Id));
                    rezTakmicenja.Sort(new SortComparer<RezultatskoTakmicenje>(propDesc, direction));
                    for (int i = 0; i < rezTakmicenja.Count; i++)
                        rezTakmicenja[i].RedBroj = (byte)(i + 1);

                    session.Transaction.Commit();
                }
            }
            catch (BusinessException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                if (ex.Notification != null)
                {
                    NotificationMessage msg = ex.Notification.FirstMessage;
                    MessageDialogs.showMessage(msg.Message, this.Text);
                    setFocus(msg.FieldName);
                }
                else
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                }
                this.DialogResult = DialogResult.None;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                this.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void validate(Notification notification)
        {
            if (takmicenje.Kategorije.Count == 0)
            {
                notification.RegisterMessage("Kategorije",
                    "Kategorije su obavezne.");
            }
            if (takmicenje.TakmicenjeDescriptions.Count == 0)
            {
                notification.RegisterMessage("TakmicenjeDescriptions",
                    "Takmicenja su obavezna.");
            }
        }

        private void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "TakmicenjeDescriptions":
                    lstTakmicenja.Focus();
                    break;

                case "Kategorije":
                    lstKategorije.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        private RezultatskoTakmicenje createRezultatskoTakmicenje(
            Takmicenje takmicenje, TakmicarskaKategorija k,
            RezultatskoTakmicenjeDescription d)
        {
            RezultatskoTakmicenje result = new RezultatskoTakmicenje(takmicenje,
                k, d, new Propozicije());
            return result;
        }

        private void deleteKategorija(TakmicarskaKategorija k)
        {
            // TODO2: Treba obrisati sve ocene, rasporede nastupa, rasporede sudija, 
            // ekipe i gimnasticari za datu kategoriju
            
            DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().Delete(k);
        }

        private void deleteRezultatskoTakmicenje(TakmicarskaKategorija kat,
            RezultatskoTakmicenjeDescription desc)
        {
            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            rezTakDAO.Delete(rezTakDAO.FindByKatDesc(kat, desc));
        }

        private void diff<T>(IList<T> current, IList<T> original, IList<T> added,
            IList<T> updated, IList<T> deleted)
        {
            foreach (T t in current)
            {
                if (!containsRef(original, t))
                    added.Add(t);
                else
                    updated.Add(t);
            }
            foreach (T t in original)
            {
                if (!containsRef(current, t))
                    deleted.Add(t);
            }
        }

        private bool containsRef<T>(IList<T> list, T t)
        {
            foreach (T t2 in list)
            {
                if (object.ReferenceEquals(t2, t))
                    return true;
            }
            return false;
        }

        private void btnEditKategorija_Click(object sender, EventArgs e)
        {
            if (SelectedKategorija == null)
                return;
            try
            {
                TakmicarskaKategorijaForm form =
                    new TakmicarskaKategorijaForm(SelectedKategorija, takmicenje);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // refresh
                    setKategorije(takmicenje.Kategorije);
                    SelectedKategorija = (TakmicarskaKategorija)form.Entity;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }
    }
}