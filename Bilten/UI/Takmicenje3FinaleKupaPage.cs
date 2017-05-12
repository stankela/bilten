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
    public partial class Takmicenje3FinaleKupaPage : PropertyPage
    {
        private Propozicije propozicije;
        private IList<Propozicije> dependentPropozicije;
        private bool dirty;

        public Takmicenje3FinaleKupaPage(Propozicije propozicije, 
            IList<Propozicije> dependentPropozicije)
        {
            InitializeComponent();
            this.propozicije = propozicije;
            this.dependentPropozicije = dependentPropozicije;
        }

        public override string Text
        {
            get { return "Takmicenje III"; }
        }

        private void ckbPostojiTak3_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            setEnabled();
        }

        private void setEnabled()
        {
            if (!ckbPostojiTak3.Checked)
            {
                rbtOdvojenoTak3.Enabled = false;
                rbtNaOsnovuPrvogIDrugogKola.Enabled = false;

                rbtFinalnaOcenaJeMax.Enabled = false;
                rbtFinalnaOcenaJeZbir.Enabled = false;
                rbtFinalnaOcenaJeProsek.Enabled = false;
                ckbNeRacunajProsek.Enabled = false;

                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
                rbtKlub.Enabled = false;
                rbtDrzava.Enabled = false;
                ckbNeogranicenBrojTak.Enabled = false;
                lblBrojFinalista.Enabled = false;
                txtBrojFinalista.Enabled = false;
                lblBrojRezervi.Enabled = false;
                txtBrojRezervi.Enabled = false;

                lblPoredakPreskok.Enabled = false;
                rbtPoredakPreskok1.Enabled = false;
                rbtPoredakPreskok2.Enabled = false;
            }
            else
            {
                rbtOdvojenoTak3.Enabled = true;
                rbtNaOsnovuPrvogIDrugogKola.Enabled = true;
                setEnabledOdvojenoTak3();
            }
        }

        private void rbtOdvojenoTak3_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (rbtOdvojenoTak3.Checked)
                setEnabledOdvojenoTak3();
        }

        private void rbtNaOsnovuTak1_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (rbtNaOsnovuPrvogIDrugogKola.Checked)
                setEnabledOdvojenoTak3();
        }

        private void setEnabledOdvojenoTak3()
        {
            bool postojiTak3 = rbtOdvojenoTak3.Enabled
                               && (rbtOdvojenoTak3.Checked || rbtNaOsnovuPrvogIDrugogKola.Checked);
            rbtFinalnaOcenaJeMax.Enabled = postojiTak3;
            rbtFinalnaOcenaJeZbir.Enabled = postojiTak3;
            rbtFinalnaOcenaJeProsek.Enabled = postojiTak3;
            ckbNeRacunajProsek.Enabled = postojiTak3 && rbtFinalnaOcenaJeProsek.Checked;

            bool odvojenoTak3 = rbtOdvojenoTak3.Enabled && rbtOdvojenoTak3.Checked;
            lblBrojFinalista.Enabled = odvojenoTak3;
            txtBrojFinalista.Enabled = odvojenoTak3;
            lblBrojRezervi.Enabled = odvojenoTak3;
            txtBrojRezervi.Enabled = odvojenoTak3;

            ckbNeogranicenBrojTak.Enabled = odvojenoTak3;
            setEnabledNeogranicenBrojTak();

            lblPoredakPreskok.Enabled = odvojenoTak3;
            rbtPoredakPreskok1.Enabled = odvojenoTak3;
            rbtPoredakPreskok2.Enabled = odvojenoTak3;
        }

        private void setEnabledNeogranicenBrojTak()
        {
            bool enabled = ckbNeogranicenBrojTak.Enabled && !ckbNeogranicenBrojTak.Checked;
            lblMaxTak.Enabled = enabled;
            txtMaxTak.Enabled = enabled;
            rbtKlub.Enabled = enabled;
            rbtDrzava.Enabled = enabled;
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

        private void rbtPoredakPreskok_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void rbtKlub_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void rbtDrzava_CheckedChanged(object sender, EventArgs e)
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

            ckbPostojiTak3.Checked = propozicije.PostojiTak3;
            if (propozicije.PostojiTak3)
            {
                rbtOdvojenoTak3.Checked = propozicije.OdvojenoTak3;
                rbtNaOsnovuPrvogIDrugogKola.Checked = !propozicije.OdvojenoTak3;

                rbtFinalnaOcenaJeMax.Checked
                    = propozicije.NacinRacunanjaOceneFinaleKupaTak3 ==  NacinRacunanjaOceneFinaleKupa.Max;
                rbtFinalnaOcenaJeZbir.Checked
                    = propozicije.NacinRacunanjaOceneFinaleKupaTak3 == NacinRacunanjaOceneFinaleKupa.Zbir;
                rbtFinalnaOcenaJeProsek.Checked
                    = propozicije.NacinRacunanjaOceneFinaleKupaTak3 == NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene
                    || propozicije.NacinRacunanjaOceneFinaleKupaTak3 == NacinRacunanjaOceneFinaleKupa.ProsekUvek;
                ckbNeRacunajProsek.Checked
                    = propozicije.NacinRacunanjaOceneFinaleKupaTak3 == NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene;
                
                if (propozicije.OdvojenoTak3)
                {
                    txtBrojFinalista.Text = propozicije.BrojFinalistaTak3.ToString();
                    txtBrojRezervi.Text = propozicije.BrojRezerviTak3.ToString();

                    ckbNeogranicenBrojTak.Checked = propozicije.NeogranicenBrojTakmicaraIzKlubaTak3;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                    {
                        txtMaxTak.Text = propozicije.MaxBrojTakmicaraIzKlubaTak3.ToString();
                        rbtDrzava.Checked = propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu;
                    }

                    rbtPoredakPreskok1.Checked = !propozicije.Tak1PreskokNaOsnovuObaPreskoka;
                    rbtPoredakPreskok2.Checked = propozicije.Tak1PreskokNaOsnovuObaPreskoka;
                }
            }
            
            enableHandlers();
            setEnabled();
        }

        private void disableHandlers()
        {
            ckbPostojiTak3.CheckedChanged -= ckbPostojiTak3_CheckedChanged;
            rbtOdvojenoTak3.CheckedChanged -= rbtOdvojenoTak3_CheckedChanged;
            rbtNaOsnovuPrvogIDrugogKola.CheckedChanged -= rbtNaOsnovuTak1_CheckedChanged;

            rbtFinalnaOcenaJeMax.CheckedChanged -= rbtFinalnaOcenaJeMax_CheckedChanged;
            rbtFinalnaOcenaJeZbir.CheckedChanged -= rbtFinalnaOcenaJeZbir_CheckedChanged;
            rbtFinalnaOcenaJeProsek.CheckedChanged -= rbtFinalnaOcenaJeProsek_CheckedChanged;
            ckbNeRacunajProsek.CheckedChanged -= ckbNeRacunajProsek_CheckedChanged;
    
            ckbNeogranicenBrojTak.CheckedChanged -= ckbNeogranicenBrojTak_CheckedChanged;

            rbtPoredakPreskok1.CheckedChanged -= rbtPoredakPreskok_CheckedChanged;
            rbtPoredakPreskok2.CheckedChanged -= rbtPoredakPreskok_CheckedChanged;
        }

        private void enableHandlers()
        {
            ckbPostojiTak3.CheckedChanged += ckbPostojiTak3_CheckedChanged;
            rbtOdvojenoTak3.CheckedChanged += rbtOdvojenoTak3_CheckedChanged;
            rbtNaOsnovuPrvogIDrugogKola.CheckedChanged += rbtNaOsnovuTak1_CheckedChanged;

            rbtFinalnaOcenaJeMax.CheckedChanged += rbtFinalnaOcenaJeMax_CheckedChanged;
            rbtFinalnaOcenaJeZbir.CheckedChanged += rbtFinalnaOcenaJeZbir_CheckedChanged;
            rbtFinalnaOcenaJeProsek.CheckedChanged += rbtFinalnaOcenaJeProsek_CheckedChanged;
            ckbNeRacunajProsek.CheckedChanged += ckbNeRacunajProsek_CheckedChanged;

            ckbNeogranicenBrojTak.CheckedChanged += ckbNeogranicenBrojTak_CheckedChanged;

            rbtPoredakPreskok1.CheckedChanged += rbtPoredakPreskok_CheckedChanged;
            rbtPoredakPreskok2.CheckedChanged += rbtPoredakPreskok_CheckedChanged;
        }

        private void clearUI()
        {
            ckbPostojiTak3.Checked = false;
            rbtOdvojenoTak3.Checked = false;
            rbtNaOsnovuPrvogIDrugogKola.Checked = false;

            rbtFinalnaOcenaJeMax.Checked = false;
            rbtFinalnaOcenaJeZbir.Checked = false;
            rbtFinalnaOcenaJeProsek.Checked = false;
            ckbNeRacunajProsek.Checked = false;

            txtBrojFinalista.Text = String.Empty;
            txtBrojRezervi.Text = String.Empty;
            ckbNeogranicenBrojTak.Checked = false;
            rbtKlub.Checked = true;
            txtMaxTak.Text = String.Empty;

            rbtPoredakPreskok1.Checked = false;
            rbtPoredakPreskok2.Checked = false;
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
            propozicije.validateTakmicenje3FinaleKupa(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            propozicije.copyTakmicenje3FinaleKupaTo(dependentPropozicije);
        }

        private void requiredFieldsAndFormatValidation(Notification notification)
        {
            byte dummyByte;
            if (!ckbPostojiTak3.Checked)
                return;
            if (!rbtOdvojenoTak3.Checked && !rbtNaOsnovuPrvogIDrugogKola.Checked)
            {
                notification.RegisterMessage(
                    "OdvojenoTak3", "Izaberite da li se finale takmicenja III posebno odrzava, " +
                    "ili se racuna na osnovu rezultata 1. i 2. kola.");
            }
            if (!rbtFinalnaOcenaJeMax.Checked && !rbtFinalnaOcenaJeZbir.Checked && !rbtFinalnaOcenaJeProsek.Checked)
            {
                notification.RegisterMessage(
                    "FinalnaOcena", "Izaberite kako se izracunava finalna ocena.");
            }

            if (txtBrojFinalista.Enabled)
            {
                if (txtBrojFinalista.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage(
                        "BrojFinalistaTak3", "Broj finalista je obavezan.");
                }
                else if (!byte.TryParse(txtBrojFinalista.Text, out dummyByte))
                {
                    notification.RegisterMessage(
                        "BrojFinalistaTak3", "Neispravan format za broj finalista.");
                }
            }

            if (txtBrojRezervi.Enabled)
            {
                if (txtBrojRezervi.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage(
                        "BrojRezerviTak3", "Broj rezervi je obavezan.");
                }
                else if (!byte.TryParse(txtBrojRezervi.Text, out dummyByte))
                {
                    notification.RegisterMessage(
                        "BrojRezerviTak3", "Neispravan format za broj rezervi.");
                }
            }

            if (txtMaxTak.Enabled)
            {
                if (txtMaxTak.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage(
                        "MaxBrojTakmicaraIzKlubaTak3", "Unestite maksimalan broj " +
                        "takmicara iz istog kluba/drzave.");
                }
                else if (!byte.TryParse(txtMaxTak.Text, out dummyByte))
                {
                    notification.RegisterMessage(
                        "MaxBrojTakmicaraIzKlubaTak3", "Neispravan format za maksimalan broj " +
                        "takmicara iz istog kluba/drzave.");
                }
            }
            if (rbtPoredakPreskok1.Enabled && !rbtPoredakPreskok1.Checked && !rbtPoredakPreskok2.Checked)
            {
                notification.RegisterMessage(
                    "Tak1PreskokNaOsnovuObaPreskoka",
                    "Izaberite da li se poredak za " +
                    "preskok racuna na osnovu prvog ili oba preskoka.");
            }

        }

        // TODO: Postoji dosta slicnosti u unosenju propozicija izmedju takmicenja 2, 3 i 4 (kako obicno takmicenje tako i
        // finale kupa). Probaj da nekako generalizujes. Npr. u requiredFieldsAndFormatValidation ima dosta ponavljanja.

        private void updatePropozicijeFromUI(Propozicije propozicije)
        {
            propozicije.PostojiTak3 = ckbPostojiTak3.Checked;
            if (propozicije.PostojiTak3)
            {
                propozicije.OdvojenoTak3 = rbtOdvojenoTak3.Checked;

                if (rbtFinalnaOcenaJeMax.Checked)
                    propozicije.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.Max;
                else if (rbtFinalnaOcenaJeZbir.Checked)
                    propozicije.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.Zbir;
                else if (rbtFinalnaOcenaJeProsek.Checked)
                {
                    if (ckbNeRacunajProsek.Checked)
                        propozicije.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene;
                    else
                        propozicije.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.ProsekUvek;
                }

                if (propozicije.OdvojenoTak3)
                {
                    propozicije.BrojFinalistaTak3 = byte.Parse(txtBrojFinalista.Text);
                    propozicije.BrojRezerviTak3 = byte.Parse(txtBrojRezervi.Text);

                    propozicije.NeogranicenBrojTakmicaraIzKlubaTak3 = ckbNeogranicenBrojTak.Checked;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                    {
                        propozicije.MaxBrojTakmicaraIzKlubaTak3 = byte.Parse(txtMaxTak.Text);
                        propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu = rbtDrzava.Checked;
                    }

                    propozicije.Tak1PreskokNaOsnovuObaPreskoka = rbtPoredakPreskok2.Checked;
                }
            }
        }
    }
}

