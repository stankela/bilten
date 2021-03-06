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
    public partial class Takmicenje2FinaleKupaPage : PropertyPage
    {
        private Propozicije propozicije;
        private IList<Propozicije> dependentPropozicije;
        private bool dirty;

        public Takmicenje2FinaleKupaPage(Propozicije propozicije, 
            IList<Propozicije> dependentPropozicije)
        {
            InitializeComponent();
            this.propozicije = propozicije;
            this.dependentPropozicije = dependentPropozicije;
        }

        public override string Text
        {
            get { return "Takmicenje II"; }
        }

        private void ckbPostojiTak2_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            setEnabled();
        }

        private void setEnabled()
        {
            if (!ckbPostojiTak2.Checked)
            {
                rbtOdvojenoTak2.Enabled = false;
                rbtNaOsnovuPrvogIDrugogKola.Enabled = false;

                rbtFinalnaOcenaJeMax.Enabled = false;
                rbtFinalnaOcenaJeZbir.Enabled = false;
                rbtFinalnaOcenaJeProsek.Enabled = false;
                ckbNeRacunajProsek.Enabled = false;

                lblBrojFinalista.Enabled = false;
                txtBrojFinalista.Enabled = false;
                lblBrojRezervi.Enabled = false;
                txtBrojRezervi.Enabled = false;

                ckbNeogranicenBrojTak.Enabled = false;
                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
            }
            else
            {
                rbtOdvojenoTak2.Enabled = true;
                rbtNaOsnovuPrvogIDrugogKola.Enabled = true;
                setEnabledOdvojenoTak2();
            }
        }

        private void rbtOdvojenoTak2_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (rbtOdvojenoTak2.Checked)
                setEnabledOdvojenoTak2();
        }

        private void rbtNaOsnovuTak1_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (rbtNaOsnovuPrvogIDrugogKola.Checked)
                setEnabledOdvojenoTak2();
        }

        private void setEnabledOdvojenoTak2()
        {
            bool postojiTak2 = rbtOdvojenoTak2.Enabled
                               && (rbtOdvojenoTak2.Checked || rbtNaOsnovuPrvogIDrugogKola.Checked);
            rbtFinalnaOcenaJeMax.Enabled = postojiTak2;
            rbtFinalnaOcenaJeZbir.Enabled = postojiTak2;
            rbtFinalnaOcenaJeProsek.Enabled = postojiTak2;
            ckbNeRacunajProsek.Enabled = postojiTak2 && rbtFinalnaOcenaJeProsek.Checked;

            bool odvojenoTak2 = rbtOdvojenoTak2.Enabled && rbtOdvojenoTak2.Checked;
            lblBrojFinalista.Enabled = odvojenoTak2;
            txtBrojFinalista.Enabled = odvojenoTak2;
            lblBrojRezervi.Enabled = odvojenoTak2;
            txtBrojRezervi.Enabled = odvojenoTak2;
            ckbNeogranicenBrojTak.Enabled = odvojenoTak2;
            setEnabledNeogranicenBrojTak();
        }

        private void setEnabledNeogranicenBrojTak()
        {
            bool enabled = ckbNeogranicenBrojTak.Enabled && !ckbNeogranicenBrojTak.Checked;
            lblMaxTak.Enabled = enabled;
            txtMaxTak.Enabled = enabled;
        }

        private void ckbNeogranicenBrojTak_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            setEnabledNeogranicenBrojTak();
        }

        private void txtMaxTak_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojFinalista_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojRezervi_TextChanged(object sender, EventArgs e)
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

            ckbPostojiTak2.Checked = propozicije.PostojiTak2;
            if (propozicije.PostojiTak2)
            {
                rbtOdvojenoTak2.Checked = propozicije.OdvojenoTak2;
                rbtNaOsnovuPrvogIDrugogKola.Checked = !propozicije.OdvojenoTak2;

                rbtFinalnaOcenaJeMax.Checked
                    = propozicije.NacinRacunanjaOceneFinaleKupaTak2 == NacinRacunanjaOceneFinaleKupa.Max;
                rbtFinalnaOcenaJeZbir.Checked
                    = propozicije.NacinRacunanjaOceneFinaleKupaTak2 == NacinRacunanjaOceneFinaleKupa.Zbir;
                rbtFinalnaOcenaJeProsek.Checked
                    = propozicije.NacinRacunanjaOceneFinaleKupaTak2 == NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene
                    || propozicije.NacinRacunanjaOceneFinaleKupaTak2 == NacinRacunanjaOceneFinaleKupa.ProsekUvek;
                ckbNeRacunajProsek.Checked
                    = propozicije.NacinRacunanjaOceneFinaleKupaTak2 == NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene;

                if (propozicije.OdvojenoTak2)
                {
                    txtBrojFinalista.Text = propozicije.BrojFinalistaTak2.ToString();
                    txtBrojRezervi.Text = propozicije.BrojRezerviTak2.ToString();
                    ckbNeogranicenBrojTak.Checked = propozicije.NeogranicenBrojTakmicaraIzKlubaTak2;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                        txtMaxTak.Text = propozicije.MaxBrojTakmicaraIzKlubaTak2.ToString();
                }
            }
            
            enableHandlers();
            setEnabled();
        }

        private void disableHandlers()
        {
            ckbPostojiTak2.CheckedChanged -= ckbPostojiTak2_CheckedChanged;
            rbtOdvojenoTak2.CheckedChanged -= rbtOdvojenoTak2_CheckedChanged;
            rbtNaOsnovuPrvogIDrugogKola.CheckedChanged -= rbtNaOsnovuTak1_CheckedChanged;
            
            rbtFinalnaOcenaJeMax.CheckedChanged -= rbtFinalnaOcenaJeMax_CheckedChanged;
            rbtFinalnaOcenaJeZbir.CheckedChanged -= rbtFinalnaOcenaJeZbir_CheckedChanged;
            rbtFinalnaOcenaJeProsek.CheckedChanged -= rbtFinalnaOcenaJeProsek_CheckedChanged;
            ckbNeRacunajProsek.CheckedChanged -= ckbNeRacunajProsek_CheckedChanged;

            ckbNeogranicenBrojTak.CheckedChanged -= ckbNeogranicenBrojTak_CheckedChanged;
        }

        private void enableHandlers()
        {
            ckbPostojiTak2.CheckedChanged += ckbPostojiTak2_CheckedChanged;
            rbtOdvojenoTak2.CheckedChanged += rbtOdvojenoTak2_CheckedChanged;
            rbtNaOsnovuPrvogIDrugogKola.CheckedChanged += rbtNaOsnovuTak1_CheckedChanged;

            rbtFinalnaOcenaJeMax.CheckedChanged += rbtFinalnaOcenaJeMax_CheckedChanged;
            rbtFinalnaOcenaJeZbir.CheckedChanged += rbtFinalnaOcenaJeZbir_CheckedChanged;
            rbtFinalnaOcenaJeProsek.CheckedChanged += rbtFinalnaOcenaJeProsek_CheckedChanged;
            ckbNeRacunajProsek.CheckedChanged += ckbNeRacunajProsek_CheckedChanged;

            ckbNeogranicenBrojTak.CheckedChanged += ckbNeogranicenBrojTak_CheckedChanged;
        }

        private void clearUI()
        {
            ckbPostojiTak2.Checked = false;
            rbtOdvojenoTak2.Checked = false;
            rbtNaOsnovuPrvogIDrugogKola.Checked = false;

            rbtFinalnaOcenaJeMax.Checked = false;
            rbtFinalnaOcenaJeZbir.Checked = false;
            rbtFinalnaOcenaJeProsek.Checked = false;
            ckbNeRacunajProsek.Checked = false;

            ckbNeogranicenBrojTak.Checked = false;
            txtMaxTak.Text = String.Empty;
            txtBrojFinalista.Text = String.Empty;
            txtBrojRezervi.Text = String.Empty;
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
            propozicije.validateTakmicenje2FinaleKupa(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            propozicije.copyTakmicenje2FinaleKupaTo(dependentPropozicije);
        }

        private void requiredFieldsAndFormatValidation(Notification notification)
        {
            byte dummyByte;
            if (!ckbPostojiTak2.Checked)
                return;
            if (!rbtOdvojenoTak2.Checked && !rbtNaOsnovuPrvogIDrugogKola.Checked)
            {
                notification.RegisterMessage(
                    "OdvojenoTak2", "Izaberite da li se finale takmicenja II posebno odrzava, " +
                    "ili se racuna na osnovu rezultata 1. i 2. kola.");
            }
            if (!rbtFinalnaOcenaJeMax.Checked && !rbtFinalnaOcenaJeZbir.Checked && !rbtFinalnaOcenaJeProsek.Checked)
            {
                notification.RegisterMessage(
                    "FinalnaOcena", "Izaberite kako se izracunava finalna ocena.");
            }
            if (txtMaxTak.Enabled)
            {
                if (txtMaxTak.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage(
                        "MaxBrojTakmicaraIzKlubaTak2", "Unestite maksimalan broj " +
                        "takmicara iz istog kluba/drzave.");
                }
                else if (!byte.TryParse(txtMaxTak.Text, out dummyByte))
                {
                    notification.RegisterMessage(
                        "MaxBrojTakmicaraIzKlubaTak2", "Neispravan format za maksimalan broj " +
                        "takmicara iz istog kluba/drzave.");
                }
            }

            if (txtBrojFinalista.Enabled)
            {
                if (txtBrojFinalista.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage(
                        "BrojFinalistaTak2", "Broj finalista je obavezan.");
                }
                else if (!byte.TryParse(txtBrojFinalista.Text, out dummyByte))
                {
                    notification.RegisterMessage(
                        "BrojFinalistaTak2", "Neispravan format za broj finalista.");
                }
            }

            if (txtBrojRezervi.Enabled)
            {
                if (txtBrojRezervi.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage(
                        "BrojRezerviTak2", "Broj rezervi je obavezan.");
                }
                else if (!byte.TryParse(txtBrojRezervi.Text, out dummyByte))
                {
                    notification.RegisterMessage(
                        "BrojRezerviTak2", "Neispravan format za broj rezervi.");
                }
            }

        }

        private void updatePropozicijeFromUI(Propozicije propozicije)
        {
            propozicije.PostojiTak2 = ckbPostojiTak2.Checked;
            if (propozicije.PostojiTak2)
            {
                propozicije.OdvojenoTak2 = rbtOdvojenoTak2.Checked;

                if (rbtFinalnaOcenaJeMax.Checked)
                    propozicije.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.Max;
                else if (rbtFinalnaOcenaJeZbir.Checked)
                    propozicije.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.Zbir;
                else if (rbtFinalnaOcenaJeProsek.Checked)
                {
                    if (ckbNeRacunajProsek.Checked)
                        propozicije.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene;
                    else
                        propozicije.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.ProsekUvek;
                }

                if (propozicije.OdvojenoTak2)
                {
                    propozicije.BrojFinalistaTak2 = byte.Parse(txtBrojFinalista.Text);
                    propozicije.BrojRezerviTak2 = byte.Parse(txtBrojRezervi.Text);
                    propozicije.NeogranicenBrojTakmicaraIzKlubaTak2 = ckbNeogranicenBrojTak.Checked;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                        propozicije.MaxBrojTakmicaraIzKlubaTak2 = byte.Parse(txtMaxTak.Text);
                }
            }
        }
    }
}

