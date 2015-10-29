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
using Bilten.Data.QueryModel;
using Iesi.Collections.Generic;

namespace Bilten.UI
{
    public partial class TakmicarskeKategorijeForm : Form
    {
        private Takmicenje takmicenje;
        private List<TakmicarskaKategorija> originalKategorije;        
        private List<RezultatskoTakmicenjeDescription> originalTakmicenja;
        private IDataContext dataContext;

        public TakmicarskeKategorijeForm(int takmicenjeId)
        {
            // TODO: Dodaj strelice umesto "Pomeri gore" i "Pomeri dole"

            // TODO3: Probaj da se prvo definisu takmicenja, pa zatim kategorije, i da
            // bude moguce da se za dato takmicenje menjaju kategorije, tj. da ne vazi
            // da sva takmicenja imaju sve kategorije.

            InitializeComponent();
            btnDeleteKategorija.Enabled = false;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicenje = loadTakmicenje(takmicenjeId);
                if (takmicenje.TakmicenjeDescriptions.Count == 0)
                {
                    // za novo takmicenje, automatski se dodaje takmicenje sa nazivom
                    // kao glavno takmicenje
                    RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
                    desc.Naziv = takmicenje.Naziv;
                    desc.Propozicije = new Propozicije();
                    takmicenje.addTakmicenjeDescription(desc);
                }

                originalKategorije = new List<TakmicarskaKategorija>(takmicenje.Kategorije);
                originalTakmicenja = 
                    new List<RezultatskoTakmicenjeDescription>(takmicenje.TakmicenjeDescriptions);

                initUI();

                //dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private Takmicenje loadTakmicenje(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Id", CriteriaOperator.Equal, takmicenjeId));
            q.FetchModes.Add(new AssociationFetch("Kategorije", AssociationFetchMode.Eager));
            q.FetchModes.Add(new AssociationFetch("TakmicenjeDescriptions", AssociationFetchMode.Eager));
            IList<Takmicenje> result = dataContext.GetByCriteria<Takmicenje>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private void initUI()
        {
            Text = "Takmicarske kategorije";

            setKategorije(takmicenje.Kategorije);
            SelectedKategorija = null;

            setTakmicenja(takmicenje.TakmicenjeDescriptions);
            SelectedTakmicenje = null;
        }

        private void setKategorije(ISet<TakmicarskaKategorija> kategorije)
        {
            List<TakmicarskaKategorija> katList = new List<TakmicarskaKategorija>(kategorije);

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

        private void setTakmicenja(ISet<RezultatskoTakmicenjeDescription> takmicenja)
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
                form = new SelectKategorijaForm(takmicenje.Id, katList, true, msg);
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

            takmicenje.removeTakmicenjeDescription(takmicenjeDesc);
            setTakmicenja(takmicenje.TakmicenjeDescriptions);
        }

        private void btnMoveUpTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescription takmicenjeDesc =
                SelectedTakmicenje;
            if (takmicenjeDesc == null)
                return;

            if (takmicenje.moveTakmicenjeDescriptionUp(takmicenjeDesc))
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

            if (takmicenje.moveTakmicenjeDescriptionDown(takmicenjeDesc))
            {
                setTakmicenja(takmicenje.TakmicenjeDescriptions);
                SelectedTakmicenje = takmicenjeDesc;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

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
                    dataContext.Delete(t);
                foreach (TakmicarskaKategorija k in deletedKat)
                    deleteKategorija(k);

                foreach (RezultatskoTakmicenjeDescription d in addedTak)
                    d.Propozicije = new Propozicije();

                dataContext.Save(takmicenje);

                foreach (TakmicarskaKategorija k in addedKat)
                {
                    foreach (RezultatskoTakmicenjeDescription d in addedTak)
                    {
                        dataContext.Add(createRezultatskoTakmicenje(takmicenje, k, d));
                    }
                    foreach (RezultatskoTakmicenjeDescription d in updatedTak)
                    {
                        dataContext.Add(createRezultatskoTakmicenje(takmicenje, k, d));
                    }
                }
                foreach (RezultatskoTakmicenjeDescription d in addedTak)
                {
                    foreach (TakmicarskaKategorija k in updatedKat)
                    {
                        dataContext.Add(createRezultatskoTakmicenje(takmicenje, k, d));
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
                List<RezultatskoTakmicenje> rezTakmicenja = getRezTakmicenja(takmicenje.Id);
                rezTakmicenja.Sort(new SortComparer<RezultatskoTakmicenje>(propDesc, direction));
                for (int i = 0; i < rezTakmicenja.Count; i++)
                    rezTakmicenja[i].RedBroj = (byte)(i + 1);

                dataContext.Commit();
            }
            catch (BusinessException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
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
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                this.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private List<RezultatskoTakmicenje> getRezTakmicenja(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            return new List<RezultatskoTakmicenje>(
                dataContext.GetByCriteria<RezultatskoTakmicenje>(q));
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
            
            dataContext.Delete(k);
        }

        private void deleteRezultatskoTakmicenje(TakmicarskaKategorija kat,
            RezultatskoTakmicenjeDescription desc)
        {
            RezultatskoTakmicenje rezTak = loadRezultatskoTakmicenje(kat, desc);
            dataContext.Delete(rezTak);
        }

        private RezultatskoTakmicenje loadRezultatskoTakmicenje(
            TakmicarskaKategorija kat, RezultatskoTakmicenjeDescription desc)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Kategorija", CriteriaOperator.Equal, kat));
            q.Criteria.Add(new Criterion("TakmicenjeDescription",
                CriteriaOperator.Equal, desc));
            IList<RezultatskoTakmicenje> result =
                dataContext.GetByCriteria<RezultatskoTakmicenje>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
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

    }
}