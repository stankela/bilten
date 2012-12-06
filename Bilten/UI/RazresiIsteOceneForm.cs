using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;

namespace Bilten.UI
{
    public partial class RazresiIsteOceneForm : Form
    {
        List<RezultatSprava> istiRezultati;
        List<RezultatUkupno> istiRezultatiUkupno;
        Takmicenje takmicenje;
        bool obaPreskoka;

        List<int> poredak;
        public List<int> Poredak
        {
            get { return poredak; }
        }

        public RazresiIsteOceneForm(List<RezultatSprava> istiRezultati, Takmicenje takmicenje, bool obaPreskoka)
        {
            InitializeComponent();
            this.istiRezultati = istiRezultati;
            this.takmicenje = takmicenje;
            this.obaPreskoka = obaPreskoka;
            initUI();
            if (!obaPreskoka)
                dataGridViewUserControl1.setItems<RezultatSprava>(istiRezultati);
            else
            {
                List<RezultatPreskok> istiRezultatiObaPreskoka = new List<RezultatPreskok>();
                foreach (RezultatPreskok r in istiRezultati)
                    istiRezultatiObaPreskoka.Add(r);
                dataGridViewUserControl1.setItems<RezultatPreskok>(istiRezultatiObaPreskoka);
            }
        }

        public RazresiIsteOceneForm(List<RezultatUkupno> istiRezultatiUkupno, Takmicenje takmicenje)
        {
            InitializeComponent();
            this.istiRezultatiUkupno = istiRezultatiUkupno;
            this.takmicenje = takmicenje;
            initUI();
            dataGridViewUserControl1.setItems<RezultatUkupno>(istiRezultatiUkupno);
        }

        private void initUI()
        {
            Text = "Promeni poredak za iste ocene";
            if (obaPreskoka)
                this.ClientSize = new Size(ClientSize.Width + 150, ClientSize.Height);
            else if (istiRezultatiUkupno != null)
                this.ClientSize = new Size(ClientSize.Width + 75, ClientSize.Height);
            if (istiRezultatiUkupno != null)
            {
                GridColumnsInitializer.initRezultatiUkupno(dataGridViewUserControl1, takmicenje,
                    /*kvalColumnVisible*/false);
            }
            else
            {
                GridColumnsInitializer.initRezultatiSprava(
                   dataGridViewUserControl1, takmicenje, /*kvalColumnVisible*/false, obaPreskoka);
            }
        }

        private List<int> parsePoredak()
        {
            List<int> result = new List<int>();
            string[] parts = textBox1.Text.Split(new Char[] { ' ' });

            int dummyInt;
            foreach (string s in parts)
            {
                if (!int.TryParse(s, out dummyInt))
                    return null;
                result.Add(int.Parse(s));
            }
            return result;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            poredak = parsePoredak();
            if (poredak == null || !checkPoredak())
            {
                MessageDialogs.showMessage("Nepravilno unesen poredak", this.Text);
                this.DialogResult = DialogResult.None;
                return;
            }

        }

        private bool checkPoredak()
        {
            List<Rezultat> rezultati = new List<Rezultat>();
            if (istiRezultatiUkupno != null)
            {
                foreach (Rezultat r in istiRezultatiUkupno)
                    rezultati.Add(r);
            }
            else
            {
                foreach (Rezultat r in istiRezultati)
                    rezultati.Add(r);
            }

            List<int> sortedPoredak = new List<int>(poredak);
            sortedPoredak.Sort();

            int rank;
            if (!obaPreskoka)
                rank = rezultati[0].Rank.Value;
            else
                rank = (rezultati[0] as RezultatPreskok).Rank2.Value;
            int prevPoredak = rank;
            int count = 0;
            foreach (int i in sortedPoredak)
            {
                if (++count > rezultati.Count)
                    // Prvih istiRezultati.Count brojeva je u redu.
                    return true;

                if (i != rank && i != prevPoredak)
                    return false;

                prevPoredak = i;
                ++rank;
            }
            return count == rezultati.Count;
        }
    }
}