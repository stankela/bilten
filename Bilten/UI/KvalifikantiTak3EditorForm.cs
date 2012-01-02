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

namespace Bilten.UI
{
    public partial class KvalifikantiTak3EditorForm : Form
    {
        private RezultatskoTakmicenje rezTakmicenje;
        private IDataContext dataContext;
        private Takmicenje takmicenje;
        private Sprava sprava;
    
        public KvalifikantiTak3EditorForm(int takmicenjeId, int rezTakmicenjeId, Sprava sprava)
        {
            InitializeComponent();
            this.sprava = sprava;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                rezTakmicenje = loadRezTakmicenje(rezTakmicenjeId);
                if (rezTakmicenje == null)
                    throw new BusinessException("Greska u programu.");

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                NHibernateUtil.Initialize(takmicenje);

                initUI();
            }
            catch (BusinessException)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw;
            }
            catch (InfrastructureException)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw;
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

        private RezultatskoTakmicenje loadRezTakmicenje(int rezTakmicenjeId)
        {
            string query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje3 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Id = :rezTakmicenjeId";

            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "rezTakmicenjeId" },
                        new object[] { rezTakmicenjeId });
            foreach (RezultatskoTakmicenje tak in result)
            {
                NHibernateUtil.Initialize(tak.Propozicije);
                foreach (PoredakSprava p in tak.Takmicenje3.Poredak)
                    NHibernateUtil.Initialize(p.Rezultati);
                NHibernateUtil.Initialize(tak.Takmicenje3.PoredakPreskok.Rezultati);
            }
            if (result.Count > 0)
                return result[0];
            else
                return null;
        }

        private void initUI()
        {
            Text = "Kvalifikanti - "
                + DeoTakmicenjaKodovi.toString(DeoTakmicenjaKod.Takmicenje3) + " - " + rezTakmicenje.Naziv;

            spravaGridUserControl1.init(sprava);
            GridColumnsInitializer.initKvalifikantiTak3(
                spravaGridUserControl1.DataGridViewUserControl, takmicenje);

            refreshItems();
        }

        private void refreshItems()
        {
            spravaGridUserControl1.DataGridViewUserControl
                .setItems<UcesnikTakmicenja3>(rezTakmicenje.Takmicenje3.getUcesniciKvalifikanti(sprava));
            spravaGridUserControl1.DataGridViewUserControl
                .sort<UcesnikTakmicenja3>("QualOrder", ListSortDirection.Ascending);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            RezultatiSpravaForm form = null;
            try
            {
                form = new RezultatiSpravaForm(takmicenje.Id,
                    DeoTakmicenjaKod.Takmicenje1, true, rezTakmicenje, sprava);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, this.Text);
                return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            // moram da koristim Naziv zato sto nije implementiran Equals u klasi
            // RezultatskoTakmicenje
            if (form.SelectedTakmicenje.Naziv != rezTakmicenje.Naziv)
            {
                string msg = "Morate da izaberete kvalifikanta iz istog takmicenja " +
                    "kao ono koje je trenutno selektovano.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }
            if (form.SelectedSprava != sprava)
            {
                string msg = "Morate da izaberete kvalifikanta za istu spravu " +
                    "kao ona koja je trenutno selektovana.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }

            foreach (UcesnikTakmicenja3 u in rezTakmicenje.Takmicenje3.getUcesniciKvalifikanti(sprava))
            {
                if (u.Gimnasticar.Id == form.SelectedResult.Gimnasticar.Id)
                {
                    string msg = String.Format("Gimnasticar \"{0}\" je vec medju kvalifikantima.", u.Gimnasticar);
                    MessageDialogs.showMessage(msg, this.Text);
                    return;
                }
            }

            UcesnikTakmicenja3 newKvalifikant;
            if (sprava != Sprava.Preskok)
            {
                RezultatSprava selResult = form.SelectedResult;
                newKvalifikant = rezTakmicenje.Takmicenje3.addKvalifikant(selResult.Gimnasticar, sprava,
                        selResult.Total, selResult.Rank);
            }
            else
            {
                RezultatPreskok selResult = (RezultatPreskok)form.SelectedResult;
                bool obaPreskoka = rezTakmicenje.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                Nullable<float> qualScore = obaPreskoka ? selResult.TotalObeOcene : selResult.Total;
                Nullable<short> qualRank = obaPreskoka ? selResult.Rank2 : selResult.Rank;

                newKvalifikant = rezTakmicenje.Takmicenje3.addKvalifikant(selResult.Gimnasticar, sprava,
                        qualScore, qualRank);
            }

            refreshItems();
            spravaGridUserControl1.setSelectedItem<UcesnikTakmicenja3>(newKvalifikant);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            IList<UcesnikTakmicenja3> selItems = spravaGridUserControl1.DataGridViewUserControl
                .getSelectedItems<UcesnikTakmicenja3>();
            if (selItems == null || selItems.Count != 1)
                return;

            UcesnikTakmicenja3 selItem = selItems[0];
            string msg = String.Format("Da li zelite da izbrisete kvalifikanta \"{0}\"?", selItem.Gimnasticar);

            if (!MessageDialogs.queryConfirmation(msg, "Kvalifikanti - " + 
                    DeoTakmicenjaKodovi.toString(DeoTakmicenjaKod.Takmicenje3)))
                return;

            int selIndex = spravaGridUserControl1.DataGridViewUserControl.getSelectedItemIndex();
            rezTakmicenje.Takmicenje3.removeKvalifikant(selItem.Gimnasticar, sprava);
            refreshItems();
            spravaGridUserControl1.DataGridViewUserControl.setSelectedItemIndex(selIndex);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            UcesnikTakmicenja3 u =
                spravaGridUserControl1.getSelectedItem<UcesnikTakmicenja3>();
            if (u == null)
                return;

            if (rezTakmicenje.Takmicenje3.moveKvalifikantUp(u, sprava))
            {
                refreshItems();
                spravaGridUserControl1.setSelectedItem<UcesnikTakmicenja3>(u);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            UcesnikTakmicenja3 u =
                spravaGridUserControl1.getSelectedItem<UcesnikTakmicenja3>();
            if (u == null)
                return;

            if (rezTakmicenje.Takmicenje3.moveKvalifikantDown(u, sprava))
            {
                refreshItems();
                spravaGridUserControl1.setSelectedItem<UcesnikTakmicenja3>(u);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                RezultatskoTakmicenje origTakmicenje = loadRezTakmicenje(rezTakmicenje.Id);
                
                List<UcesnikTakmicenja3> orig = new List<UcesnikTakmicenja3>(origTakmicenje.Takmicenje3.getUcesniciKvalifikanti(sprava));
                List<UcesnikTakmicenja3> curr = new List<UcesnikTakmicenja3>(rezTakmicenje.Takmicenje3.getUcesniciKvalifikanti(sprava));
                List<UcesnikTakmicenja3> added = new List<UcesnikTakmicenja3>();
                List<UcesnikTakmicenja3> updated = new List<UcesnikTakmicenja3>();
                List<UcesnikTakmicenja3> deleted = new List<UcesnikTakmicenja3>();
                diff(curr, orig, added, updated, deleted);

                foreach (UcesnikTakmicenja3 u in updated)
                {
                    UcesnikTakmicenja3 origUcesnik = origTakmicenje.Takmicenje3.getUcesnikKvalifikant(u.Gimnasticar, sprava);
                    if (origUcesnik == null)
                        throw new Exception("Greska u programu.");
                    origUcesnik.QualOrder = u.QualOrder;
                }

                foreach (UcesnikTakmicenja3 u in added)
                {
                    origTakmicenje.Takmicenje3.addUcesnik(u);
                    Ocena o = loadOcena(u.Gimnasticar, DeoTakmicenjaKod.Takmicenje3, sprava);
                    if (sprava != Sprava.Preskok)
                        origTakmicenje.Takmicenje3.getPoredak(sprava).addGimnasticar(u.Gimnasticar, o, origTakmicenje);
                    else
                        origTakmicenje.Takmicenje3.PoredakPreskok.addGimnasticar(u.Gimnasticar, o, origTakmicenje);
                }

                foreach (UcesnikTakmicenja3 u in deleted)
                {
                    origTakmicenje.Takmicenje3.removeUcesnik(u);
                    if (sprava == Sprava.Preskok)
                        origTakmicenje.Takmicenje3.PoredakPreskok.deleteGimnasticar(u.Gimnasticar, origTakmicenje);
                    else
                        origTakmicenje.Takmicenje3.getPoredak(sprava).deleteGimnasticar(u.Gimnasticar, origTakmicenje);
                }

                dataContext.Save(origTakmicenje.Takmicenje3);
                dataContext.Commit();
            }
            catch (InfrastructureException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;

                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void diff(List<UcesnikTakmicenja3> current, List<UcesnikTakmicenja3> original, List<UcesnikTakmicenja3> added,
            List<UcesnikTakmicenja3> updated, List<UcesnikTakmicenja3> deleted)
        {
            foreach (UcesnikTakmicenja3 t in current)
            {
                if (!contains(original, t))
                    added.Add(t);
                else
                    updated.Add(t);
            }
            foreach (UcesnikTakmicenja3 t in original)
            {
                if (!contains(current, t))
                    deleted.Add(t);
            }
        }

        private bool contains(List<UcesnikTakmicenja3> list, UcesnikTakmicenja3 t)
        {
            foreach (UcesnikTakmicenja3 t2 in list)
            {
                if (t2.Gimnasticar.Id == t.Gimnasticar.Id)
                    return true;
            }
            return false;
        }

        private Ocena loadOcena(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod, Sprava spava)
        {
            IList<Ocena> ocene = loadOcene(g, deoTakKod);
            foreach (Ocena o in ocene)
            {
                if (o.Sprava == spava)
                    return o;
            }
            return null;
        }

        private IList<Ocena> loadOcene(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                return dataContext.ExecuteNamedQuery<Ocena>(
                    "FindOceneForGimnasticar",
                    new string[] { "gim", "deoTakKod" },
                    new object[] { g, deoTakKod });
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

    }
}
