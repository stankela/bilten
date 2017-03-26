using Bilten.Dao;
using Bilten.Domain;
using Bilten.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class KonacanPlasmanForm : Form
    {
        private StatusBar statusBar;
        Gimnastika gimnastika;
        
        public KonacanPlasmanForm(List<KonacanPlasman> plasmani, Gimnastika gimnastika)
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(Screen.PrimaryScreen.WorkingArea.Width - 20, 540);
            this.dataGridViewUserControl1.DataGridView.MouseUp += DataGridView_MouseUp;
            this.Text = "Rezultati";
            this.gimnastika = gimnastika;

            statusBar = new StatusBar();
            statusBar.Parent = this;
            statusBar.ShowPanels = true;
            StatusBarPanel sbPanel1 = new StatusBarPanel();
            statusBar.Panels.Add(sbPanel1);
            
            initializeGridColumns(plasmani, gimnastika);

            dataGridViewUserControl1.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(
                DataGridViewUserControl_GridColumnHeaderMouseClick);

            dataGridViewUserControl1.setItems<KonacanPlasman>(plasmani);
            dataGridViewUserControl1.sort<KonacanPlasman>(
                new string[] { "DatumTakmicenja" },
                new ListSortDirection[] { ListSortDirection.Descending });
            updatePlasmanCount(plasmani.Count);
        }

        void DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView grid = dataGridViewUserControl1.DataGridView;
            if (e.Button == MouseButtons.Right && grid.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
            {
                contextMenuStrip1.Show(grid, new Point(e.X, e.Y));
            }
        }

        void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<KonacanPlasman>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns(List<KonacanPlasman> plasmani, Gimnastika gimnastika)
        {
            List<string> takmicenja = new List<string>();
            List<string> mesta = new List<string>();
            List<string> datumiTak = new List<string>();
            List<string> kategorije = new List<string>();
            List<string> imena = new List<string>();
            List<string> prezimena = new List<string>();
            List<string> datumiRodj = new List<string>();

            foreach (KonacanPlasman kp in plasmani)
            {
                takmicenja.Add(kp.NazivTakmicenja);
                mesta.Add(kp.MestoTakmicenja);
                datumiTak.Add(kp.DatumTakmicenja.ToString());
                kategorije.Add(kp.NazivKategorije);
                imena.Add(kp.Ime);
                prezimena.Add(kp.Prezime);
                if (kp.DatumRodjenja != null)
                    datumiRodj.Add(kp.DatumRodjenja.ToString());
            }

            DataGridView dgw = dataGridViewUserControl1.DataGridView;
            
            dataGridViewUserControl1.AddColumn("Takmicenje", "NazivTakmicenja",
                takmicenja.Count > 0 ? GridColumnsInitializer.getMaxWidth(takmicenja, dgw) : 250);
            dataGridViewUserControl1.AddColumn("Mesto", "MestoTakmicenja",
                mesta.Count > 0 ? GridColumnsInitializer.getMaxWidth(mesta, dgw) : 100);
            dataGridViewUserControl1.AddColumn("Datum takmicenja", "DatumTakmicenja",
                datumiTak.Count > 0 ? GridColumnsInitializer.getMaxWidth(datumiTak, dgw) : 80, "{0:d}");
            dataGridViewUserControl1.AddColumn("Kategorija", "NazivKategorije",
                kategorije.Count > 0 ? GridColumnsInitializer.getMaxWidth(kategorije, dgw) : 150);
            // TODO: Proveri zasto imena i prezimena nece da podesi pravilno.
            dataGridViewUserControl1.AddColumn("Ime", "Ime",
                /*imena.Count > 0 ? GridColumnsInitializer.getMaxWidth(imena, dgw) :*/ 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime",
                /*prezimena.Count > 0 ? GridColumnsInitializer.getMaxWidth(prezimena, dgw) :*/ 100);
            dataGridViewUserControl1.AddColumn("Datum rodjenja", "DatumRodjenja",
                datumiRodj.Count > 0 ? GridColumnsInitializer.getMaxWidth(datumiRodj, dgw) : 100, "{0:d}");

            dataGridViewUserControl1.AddColumn("Viseboj", "Viseboj", 50);

            if (gimnastika == Gimnastika.MSG)
            {
                dataGridViewUserControl1.AddColumn("Parter", "Parter", 50);
                dataGridViewUserControl1.AddColumn("Konj", "Konj", 50);
                dataGridViewUserControl1.AddColumn("Karike", "Karike", 50);
                dataGridViewUserControl1.AddColumn("Preskok", "Preskok", 50);
                dataGridViewUserControl1.AddColumn("Razboj", "Razboj", 50);
                dataGridViewUserControl1.AddColumn("Vratilo", "Vratilo", 50);
            }
            else
            {
                dataGridViewUserControl1.AddColumn("Preskok", "Preskok", 50);
                dataGridViewUserControl1.AddColumn("Dvo. razboj", "DvovisinskiRazboj", 50);
                dataGridViewUserControl1.AddColumn("Greda", "Greda", 50);
                dataGridViewUserControl1.AddColumn("Parter", "Parter", 50);
            }
            dataGridViewUserControl1.AddColumn("Ekipno", "Ekipno", 50);
        }

        private void btnZatvori_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void KonacanPlasmanForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.clearSelection();
        }

        private void updatePlasmanCount(int count)
        {
            statusBar.Panels[0].Text = count + " takmicenja";
        }

        private void btnRezultatiVisebojTak1_Click(object sender, EventArgs e)
        {

        }

        private void mnRezultatiVisebojTakmicenjeI_Click(object sender, EventArgs e)
        {

        }

        private void btnRezultatiSpraveTakI_Click(object sender, EventArgs e)
        {
            cmdRezultatiSpraveTakmicenje(DeoTakmicenjaKod.Takmicenje1, "Rezultati sprave takmicenje I");
        }

        private void mnRezultatiSpraveTakmicenjeI_Click(object sender, EventArgs e)
        {
            cmdRezultatiSpraveTakmicenje(DeoTakmicenjaKod.Takmicenje1, "Rezultati sprave takmicenje I");
        }

        private void mnRezultatiSpraveTakmicenjeIII_Click(object sender, EventArgs e)
        {
            cmdRezultatiSpraveTakmicenje(DeoTakmicenjaKod.Takmicenje3, "Rezultati sprave takmicenje III");
        }

        private void btnRezultatiSpraveTak3_Click(object sender, EventArgs e)
        {
            cmdRezultatiSpraveTakmicenje(DeoTakmicenjaKod.Takmicenje3, "Rezultati sprave takmicenje III");
        }

        private void cmdRezultatiSpraveTakmicenje(DeoTakmicenjaKod deoTakKod, string errorMsg)
        {
            KonacanPlasman selItem = dataGridViewUserControl1.getSelectedItem<KonacanPlasman>();
            if (selItem == null)
                return;

            try
            {
                Sprava sprava = gimnastika == Gimnastika.MSG ? Sprava.Parter : Sprava.Preskok;
                RezultatiSpravaForm form = new RezultatiSpravaForm(selItem.TakmicenjeId, deoTakKod,
                    selItem.RezultatskoTakmicenjeId, sprava, true, false);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDialogs.showMessage(ex.Message, errorMsg);
            }
        }
    }
}
