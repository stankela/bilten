using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class Takmicenje4FinaleKupaPage : PropertyPage
    {
        private Propozicije propozicije;
        private IList<Propozicije> dependentPropozicije;
        private bool dirty;

        public Takmicenje4FinaleKupaPage(Propozicije propozicije,
            IList<Propozicije> dependentPropozicije)
        {
            InitializeComponent();
            this.propozicije = propozicije;
            this.dependentPropozicije = dependentPropozicije;
        }

        public override string Text
        {
            get { return "Takmicenje IV"; }
        }

        private void ckbPostojiTak4_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            setEnabled();
        }

        private void setEnabled()
        {
            if (!ckbPostojiTak4.Checked)
            {
                rbtOdvojenoTak4.Enabled = false;
                rbtNaOsnovuPrvogIDrugogKola.Enabled = false;
                rbtPostojiTak4ZaSvakuKategoriju.Enabled = false;
                rbtJednoTak4ZaSveKategorije.Enabled = false;

                rbtFinalnaOcenaJeMax.Enabled = false;
                rbtFinalnaOcenaJeZbir.Enabled = false;
                rbtFinalnaOcenaJeProsek.Enabled = false;
                ckbNeRacunajProsek.Enabled = false;

                lblBrojEkipa.Enabled = false;
                txtBrojEkipa.Enabled = false;
                lblBrojRezultata.Enabled = false;
                txtBrojRezultata.Enabled = false;
            }
            else
            {
                rbtOdvojenoTak4.Enabled = true;
                rbtNaOsnovuPrvogIDrugogKola.Enabled = true;
                setEnabledOdvojenoTak4();
            }
        }

        private void rbtOdvojenoTak4_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (rbtOdvojenoTak4.Checked)
                setEnabledOdvojenoTak4();
        }

        private void rbtNaOsnovuTak1_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (rbtNaOsnovuPrvogIDrugogKola.Checked)
                setEnabledOdvojenoTak4();
        }

        private void setEnabledOdvojenoTak4()
        {
            bool postojiTak4 = rbtOdvojenoTak4.Enabled
                               && (rbtOdvojenoTak4.Checked || rbtNaOsnovuPrvogIDrugogKola.Checked);
            
            // Ovu opciju ima smisla podesavati samo u propozicijama za takmicenje description
            rbtPostojiTak4ZaSvakuKategoriju.Enabled = dependentPropozicije != null && postojiTak4;
            rbtJednoTak4ZaSveKategorije.Enabled = dependentPropozicije != null && postojiTak4;
            
            rbtFinalnaOcenaJeMax.Enabled = postojiTak4;
            rbtFinalnaOcenaJeZbir.Enabled = postojiTak4;
            rbtFinalnaOcenaJeProsek.Enabled = postojiTak4;
            ckbNeRacunajProsek.Enabled = postojiTak4 && rbtFinalnaOcenaJeProsek.Checked;

            bool odvojenoTak4 = rbtOdvojenoTak4.Enabled && rbtOdvojenoTak4.Checked;
            lblBrojEkipa.Enabled = odvojenoTak4;
            txtBrojEkipa.Enabled = odvojenoTak4;
            lblBrojRezultata.Enabled = odvojenoTak4;
            txtBrojRezultata.Enabled = odvojenoTak4;
        }

        private void txtBrojRezultata_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojEkipa_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void rbtPostojiTak4ZaSvakuKategoriju_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void rbtJednoTak4ZaSveKategorije_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void rbtFinalnaOcenaJeMax_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            ckbNeRacunajProsek.Enabled = rbtFinalnaOcenaJeProsek.Checked;
        }

        private void rbtFinalnaOcenaJeZbir_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            ckbNeRacunajProsek.Enabled = rbtFinalnaOcenaJeProsek.Checked;
        }

        private void rbtFinalnaOcenaJeProsek_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            ckbNeRacunajProsek.Enabled = rbtFinalnaOcenaJeProsek.Checked;
        }

        private void ckbNeRacunajProsek_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        public override void OnSetActive()
        {
            updateUIFromPropozicije(propozicije);
            dirty = false;
        }

        private void updateUIFromPropozicije(Propozicije propozicije)
        {
            disableHandlers();
            clearUI();

            ckbPostojiTak4.Checked = propozicije.PostojiTak4;
            if (propozicije.PostojiTak4)
            {
                rbtOdvojenoTak4.Checked = propozicije.OdvojenoTak4;
                rbtNaOsnovuPrvogIDrugogKola.Checked = !propozicije.OdvojenoTak4;

                rbtPostojiTak4ZaSvakuKategoriju.Checked = !propozicije.JednoTak4ZaSveKategorije;
                rbtJednoTak4ZaSveKategorije.Checked = propozicije.JednoTak4ZaSveKategorije;

                rbtFinalnaOcenaJeMax.Checked = propozicije.Tak4FinalnaOcenaJeMaxObaKola;
                rbtFinalnaOcenaJeZbir.Checked = propozicije.Tak4FinalnaOcenaJeZbirObaKola;
                rbtFinalnaOcenaJeProsek.Checked = propozicije.Tak4FinalnaOcenaJeProsekObaKola;
                ckbNeRacunajProsek.Checked = propozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola;

                if (propozicije.OdvojenoTak4)
                {
                    txtBrojEkipa.Text = propozicije.BrojEkipaUFinalu.ToString();
                    txtBrojRezultata.Text = propozicije.BrojRezultataKojiSeBodujuZaEkipu.ToString();
                }
            }

            enableHandlers();
            setEnabled();
        }

        private void disableHandlers()
        {
            ckbPostojiTak4.CheckedChanged -= ckbPostojiTak4_CheckedChanged;
            rbtOdvojenoTak4.CheckedChanged -= rbtOdvojenoTak4_CheckedChanged;
            rbtNaOsnovuPrvogIDrugogKola.CheckedChanged -= rbtNaOsnovuTak1_CheckedChanged;
            rbtPostojiTak4ZaSvakuKategoriju.CheckedChanged -= rbtPostojiTak4ZaSvakuKategoriju_CheckedChanged;
            rbtJednoTak4ZaSveKategorije.CheckedChanged -= rbtJednoTak4ZaSveKategorije_CheckedChanged;

            rbtFinalnaOcenaJeMax.CheckedChanged -= rbtFinalnaOcenaJeMax_CheckedChanged;
            rbtFinalnaOcenaJeZbir.CheckedChanged -= rbtFinalnaOcenaJeZbir_CheckedChanged;
            rbtFinalnaOcenaJeProsek.CheckedChanged -= rbtFinalnaOcenaJeProsek_CheckedChanged;
            ckbNeRacunajProsek.CheckedChanged -= ckbNeRacunajProsek_CheckedChanged;

        }

        private void enableHandlers()
        {
            ckbPostojiTak4.CheckedChanged += ckbPostojiTak4_CheckedChanged;
            rbtOdvojenoTak4.CheckedChanged += rbtOdvojenoTak4_CheckedChanged;
            rbtNaOsnovuPrvogIDrugogKola.CheckedChanged += rbtNaOsnovuTak1_CheckedChanged;
            rbtPostojiTak4ZaSvakuKategoriju.CheckedChanged += rbtPostojiTak4ZaSvakuKategoriju_CheckedChanged;
            rbtJednoTak4ZaSveKategorije.CheckedChanged += rbtJednoTak4ZaSveKategorije_CheckedChanged;

            rbtFinalnaOcenaJeMax.CheckedChanged += rbtFinalnaOcenaJeMax_CheckedChanged;
            rbtFinalnaOcenaJeZbir.CheckedChanged += rbtFinalnaOcenaJeZbir_CheckedChanged;
            rbtFinalnaOcenaJeProsek.CheckedChanged += rbtFinalnaOcenaJeProsek_CheckedChanged;
            ckbNeRacunajProsek.CheckedChanged += ckbNeRacunajProsek_CheckedChanged;
        }

        private void clearUI()
        {
            ckbPostojiTak4.Checked = false;
            rbtOdvojenoTak4.Checked = false;
            rbtNaOsnovuPrvogIDrugogKola.Checked = false;
            rbtPostojiTak4ZaSvakuKategoriju.Checked = false;
            rbtJednoTak4ZaSveKategorije.Checked = false;

            rbtFinalnaOcenaJeMax.Checked = false;
            rbtFinalnaOcenaJeZbir.Checked = false;
            rbtFinalnaOcenaJeProsek.Checked = false;
            ckbNeRacunajProsek.Checked = false;

            txtBrojEkipa.Text = String.Empty;
            txtBrojRezultata.Text = String.Empty;
        }

        public override void OnApply()
        {
            if (!dirty)
                return;

            Notification notification = new Notification();
            requiredFieldsAndFormatValidation(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            updatePropozicijeFromUI(propozicije);

            notification = new Notification();
            propozicije.validateTakmicenje4FinaleKupa(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            propozicije.copyTakmicenje4FinaleKupaTo(dependentPropozicije);
        }

        private void requiredFieldsAndFormatValidation(Notification notification)
        {
            byte dummyByte;
            if (!ckbPostojiTak4.Checked)
                return;
            if (!rbtOdvojenoTak4.Checked && !rbtNaOsnovuPrvogIDrugogKola.Checked)
            {
                notification.RegisterMessage(
                    "OdvojenoTak4", "Izaberite da li se finale takmicenja IV posebno odrzava, " +
                    "ili se racuna na osnovu rezultata 1. i 2. kola.");
            }
            if (!rbtPostojiTak4ZaSvakuKategoriju.Checked && !rbtJednoTak4ZaSveKategorije.Checked)
            {
                notification.RegisterMessage(
                    "JednoTak4ZaSveKategorije", "Izaberite da li za svaku kategoriju " +
                    "postoji takmicenje IV, ili postoji jedno takmicenje IV za sve kategorije.");
            }
            if (!rbtFinalnaOcenaJeMax.Checked && !rbtFinalnaOcenaJeZbir.Checked && !rbtFinalnaOcenaJeProsek.Checked)
            {
                notification.RegisterMessage(
                    "FinalnaOcena", "Izaberite kako se izracunava finalna ocena.");
            }

            if (txtBrojEkipa.Enabled)
            {
                if (txtBrojEkipa.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage(
                        "BrojEkipaUFinalu", "Broj ekipa u finalu je obavezan.");
                }
                else if (!byte.TryParse(txtBrojEkipa.Text, out dummyByte))
                {
                    notification.RegisterMessage(
                        "BrojEkipaUFinalu", "Neispravan format za broj ekipa u finalu.");
                }
            }

            if (txtBrojRezultata.Enabled)
            {
                if (txtBrojRezultata.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage(
                        "BrojRezultataKojiSeBodujuZaEkipu", "Unesite broj rezultata koji se vrednuju za ekipu.");
                }
                else if (!byte.TryParse(txtBrojRezultata.Text, out dummyByte))
                {
                    notification.RegisterMessage(
                        "BrojRezultataKojiSeBodujuZaEkipu", "Neispravan format za broj rezultata koji se vrednuju za ekipu.");
                }
            }
        }

        private void updatePropozicijeFromUI(Propozicije propozicije)
        {
            propozicije.PostojiTak4 = ckbPostojiTak4.Checked;
            if (propozicije.PostojiTak4)
            {
                propozicije.OdvojenoTak4 = rbtOdvojenoTak4.Checked;
                propozicije.JednoTak4ZaSveKategorije = rbtJednoTak4ZaSveKategorije.Checked;

                propozicije.Tak4FinalnaOcenaJeMaxObaKola = rbtFinalnaOcenaJeMax.Checked;
                propozicije.Tak4FinalnaOcenaJeZbirObaKola = rbtFinalnaOcenaJeZbir.Checked;
                propozicije.Tak4FinalnaOcenaJeProsekObaKola = rbtFinalnaOcenaJeProsek.Checked;
                propozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola = ckbNeRacunajProsek.Checked;

                if (propozicije.OdvojenoTak4)
                {
                    propozicije.BrojEkipaUFinalu = byte.Parse(txtBrojEkipa.Text);
                    propozicije.BrojRezultataKojiSeBodujuZaEkipu = byte.Parse(txtBrojRezultata.Text);
                }
            }
        }
    }
}

