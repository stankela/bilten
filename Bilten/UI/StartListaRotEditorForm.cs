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
    public partial class StartListaRotEditorForm : Form
    {
        private StartListaNaSpravi startLista;
        private IDataContext dataContext;
        private int takmicenjeId;

        private RasporedNastupa raspored;
        public RasporedNastupa RasporedNastupa
        {
            get { return raspored; }
        }

        public StartListaRotEditorForm(int rasporedId, Sprava sprava,
            int grupa, int rotacija, int takmicenjeId)
        {
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;
            spravaGridUserControl1.init(sprava);
            GridColumnsInitializer.initStartListaRotacija(spravaGridUserControl1.DataGridViewUserControl);
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                raspored = loadRaspored(rasporedId);
                startLista = raspored.getStartLista(sprava, grupa, rotacija);
                foreach (NastupNaSpravi n in startLista.Nastupi)
                {
                    //  potrebno za slucaj kada se u start listi nalaze i gimnasticari iz kategorija razlicitih od kategorija
                    // za koje start lista vazi.
                    NHibernateUtil.Initialize(n.Gimnasticar.TakmicarskaKategorija);   
                }
                
                initUI();
                spravaGridUserControl1.setItems(startLista.Nastupi);

            //    dataContext.Commit();
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

        private RasporedNastupa loadRaspored(int rasporedId)
        {
            IList<RasporedNastupa> result = dataContext.
                ExecuteNamedQuery<RasporedNastupa>("FindRaspNastById",
                new string[] { "id" },
                new object[] { rasporedId });
            if (result.Count > 0)
                return result[0];
            else
                return null;
        }

        private void initUI()
        {
            Text = "Start lista - " +
                DeoTakmicenjaKodovi.toString(raspored.DeoTakmicenjaKod);
        }

        private void StartListaRotEditorForm_Load(object sender, EventArgs e)
        {
            spravaGridUserControl1.clearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = DialogResult.None;
            SelectGimnasticarUcesnikForm form = null;
            try
            {
                form = new SelectGimnasticarUcesnikForm(takmicenjeId, raspored.Pol, null);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                return;

            List<GimnasticarUcesnik> okGimnasticari = new List<GimnasticarUcesnik>();
            List<GimnasticarUcesnik> illegalGimnasticari = new List<GimnasticarUcesnik>();
            foreach (GimnasticarUcesnik g in form.SelectedEntities)
            {
                if (startLista.canAddGimnasticar(g))
                    okGimnasticari.Add(g);
                else
                    illegalGimnasticari.Add(g);
            }

            /*for (int i = okGimnasticari.Count - 1; i >= 0; i--)
            {
                GimnasticarUcesnik g = okGimnasticari[i];
                if (!raspored.Kategorije.Contains(g.TakmicarskaKategorija))
                {
                    okGimnasticari.RemoveAt(i);
                    illegalGimnasticari.Add(g);
                }
            }*/

            foreach (GimnasticarUcesnik g in okGimnasticari)
            {
                startLista.addGimnasticar(g, false);
            }

            if (okGimnasticari.Count > 0)
            {
                spravaGridUserControl1.setItems(startLista.Nastupi);
            }

            if (illegalGimnasticari.Count > 0)
            {
                string msg = "Sledeci gimnasticari nisu dodati, zato sto ili vec " +
                    "postoje na start listi, ili im kategorija nije odgovarajuca: \n\n";
                msg += StringUtil.getListString(illegalGimnasticari.ToArray());
                //       MessageDialogs.showMessage(msg, this.Text);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            NastupNaSpravi nastup =
                spravaGridUserControl1.getSelectedItem<NastupNaSpravi>();
            if (nastup == null)
                return;

            string msgFmt = "Da li zelite da izbrisete gimnasticara '{0}'?";
            if (!MessageDialogs.queryConfirmation(
                String.Format(msgFmt, nastup.Gimnasticar.PrezimeIme), this.Text))
                return;

            startLista.removeNastup(nastup);
            spravaGridUserControl1.setItems(startLista.Nastupi);
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (startLista.empty())
                return;

            string msg = "Da li zelite da izbrisete sve gimnasticare?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            startLista.clear();
            spravaGridUserControl1.setItems(startLista.Nastupi);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            NastupNaSpravi nastup =
                spravaGridUserControl1.getSelectedItem<NastupNaSpravi>();
            if (nastup == null)
                return;

            if (startLista.moveNastupUp(nastup))
            {
                spravaGridUserControl1.setItems(startLista.Nastupi);
                spravaGridUserControl1.setSelectedItem<NastupNaSpravi>(nastup);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            NastupNaSpravi nastup =
                spravaGridUserControl1.getSelectedItem<NastupNaSpravi>();
            if (nastup == null)
                return;

            if (startLista.moveNastupDown(nastup))
            {
                spravaGridUserControl1.setItems(startLista.Nastupi);
                spravaGridUserControl1.setSelectedItem<NastupNaSpravi>(nastup);
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
                dataContext.Save(startLista);
                dataContext.Commit();
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

    }
}