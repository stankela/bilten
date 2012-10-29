using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;

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

                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
                ckbNeogranicenBrojTak.Enabled = false;
                lblBrojFinalista.Enabled = false;
                txtBrojFinalista.Enabled = false;
                lblBrojRezervi.Enabled = false;
                txtBrojRezervi.Enabled = false;
            }
            else
            {
                rbtOdvojenoTak2.Enabled = true;
                rbtNaOsnovuPrvogIDrugogKola.Enabled = true;

                rbtFinalnaOcenaJeMax.Enabled = true;
                rbtFinalnaOcenaJeZbir.Enabled = true;
                rbtFinalnaOcenaJeProsek.Enabled = true;
                ckbNeRacunajProsek.Enabled = rbtFinalnaOcenaJeProsek.Checked;

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
            if (!rbtOdvojenoTak2.Enabled && !rbtNaOsnovuPrvogIDrugogKola.Enabled)
                return;

            if (!rbtOdvojenoTak2.Checked && !rbtNaOsnovuPrvogIDrugogKola.Checked)
            {
                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
                ckbNeogranicenBrojTak.Enabled = false;
                lblBrojFinalista.Enabled = false;
                txtBrojFinalista.Enabled = false;
                lblBrojRezervi.Enabled = false;
                txtBrojRezervi.Enabled = false;
            }
            else
            {
                bool odvojenoTak2 = rbtOdvojenoTak2.Checked;

                ckbNeogranicenBrojTak.Enabled = odvojenoTak2;
                setEnabledNeogranicenBrojTak();

                lblBrojFinalista.Enabled = odvojenoTak2;
                txtBrojFinalista.Enabled = odvojenoTak2;
                lblBrojRezervi.Enabled = odvojenoTak2;
                txtBrojRezervi.Enabled = odvojenoTak2;
            }
        }

        private void setEnabledNeogranicenBrojTak()
        {
            if (!ckbNeogranicenBrojTak.Enabled)
            {
                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
            }
            else
            {
                bool neogranicenBrojTak = ckbNeogranicenBrojTak.Checked;
                lblMaxTak.Enabled = !neogranicenBrojTak;
                txtMaxTak.Enabled = !neogranicenBrojTak;
            }
        }

        private void ckbNeogranicenBrojTak_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
            setEnabledNeogranicenBrojTak();
        }

        public override void OnSetActive()
        {
            refreshUI();
            dirty = false;
        }

        private void refreshUI()
        {
            disableHandlers();
            clearUI();

            ckbPostojiTak2.Checked = propozicije.PostojiTak2;
            if (propozicije.PostojiTak2)
            {
                rbtOdvojenoTak2.Checked = propozicije.OdvojenoTak2;
                rbtNaOsnovuPrvogIDrugogKola.Checked = !propozicije.OdvojenoTak2;

                rbtFinalnaOcenaJeMax.Checked = propozicije.Tak2FinalnaOcenaJeMaxObaKola;
                rbtFinalnaOcenaJeZbir.Checked = propozicije.Tak2FinalnaOcenaJeZbirObaKola;
                rbtFinalnaOcenaJeProsek.Checked = propozicije.Tak2FinalnaOcenaJeProsekObaKola;
                ckbNeRacunajProsek.Checked = propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola;

                if (propozicije.OdvojenoTak2)
                {
                    ckbNeogranicenBrojTak.Checked = propozicije.NeogranicenBrojTakmicaraIzKlubaTak2;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                        txtMaxTak.Text = propozicije.MaxBrojTakmicaraIzKlubaTak2.ToString();

                    txtBrojFinalista.Text = propozicije.BrojFinalistaTak2.ToString();
                    txtBrojRezervi.Text = propozicije.BrojRezerviTak2.ToString();
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

            validate();
            updatePropozicije();
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
                    // TODO: Trebalo bi (i ovde i na drugim mestima) format proveravati
                    // pomocu regular expressions, jer moze da se desi npr. da se
                    // unese broj 300 i da se dobije poruka da je format neispravan
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

        private void validate()
        {
            if (txtMaxTak.Enabled)
            {
                byte maxTak = byte.Parse(txtMaxTak.Text);
                if (maxTak < 1)
                {
                    throw new BusinessException("MaxBrojTakmicaraIzKlubaTak2", 
                        "Neispravna vrednost za maksimalan broj " +
                        "takmicara iz istog kluba/drzave.");
                }
            }

            if (txtBrojFinalista.Enabled)
            {
                byte brojFinalista = byte.Parse(txtBrojFinalista.Text);
                if (brojFinalista < 1)
                {
                    throw new BusinessException(
                        "BrojFinalistaTak2", "Neispravna vrednost za broj finalista.");
                }
            }

            if (txtBrojRezervi.Enabled)
            {
                byte brojRezervi = byte.Parse(txtBrojRezervi.Text);
                if (brojRezervi < 1)
                {
                    throw new BusinessException(
                        "BrojRezerviTak2", "Neispravna vrednost za broj rezervi.");
                }
            }
        }

        private void updatePropozicije()
        {
            propozicije.PostojiTak2 = ckbPostojiTak2.Checked;
            if (propozicije.PostojiTak2)
            {
                propozicije.OdvojenoTak2 = rbtOdvojenoTak2.Checked;

                propozicije.Tak2FinalnaOcenaJeMaxObaKola = rbtFinalnaOcenaJeMax.Checked;
                propozicije.Tak2FinalnaOcenaJeZbirObaKola = rbtFinalnaOcenaJeZbir.Checked;
                propozicije.Tak2FinalnaOcenaJeProsekObaKola = rbtFinalnaOcenaJeProsek.Checked;
                propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola = ckbNeRacunajProsek.Checked;
                
                if (propozicije.OdvojenoTak2)
                {
                    propozicije.NeogranicenBrojTakmicaraIzKlubaTak2 = ckbNeogranicenBrojTak.Checked;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                        propozicije.MaxBrojTakmicaraIzKlubaTak2 = byte.Parse(txtMaxTak.Text);

                    propozicije.BrojFinalistaTak2 = byte.Parse(txtBrojFinalista.Text);
                    propozicije.BrojRezerviTak2 = byte.Parse(txtBrojRezervi.Text);
                }
            }

            if (dependentPropozicije != null)
            {
                foreach (Propozicije p in dependentPropozicije)
                {
                    p.PostojiTak2 = propozicije.PostojiTak2;
                    p.OdvojenoTak2 = propozicije.OdvojenoTak2;
                    p.Tak2FinalnaOcenaJeMaxObaKola = propozicije.Tak2FinalnaOcenaJeMaxObaKola;
                    p.Tak2FinalnaOcenaJeZbirObaKola = propozicije.Tak2FinalnaOcenaJeZbirObaKola;
                    p.Tak2FinalnaOcenaJeProsekObaKola = propozicije.Tak2FinalnaOcenaJeProsekObaKola;
                    p.Tak2NeRacunajProsekAkoNemaOceneIzObaKola = propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola;
                    p.NeogranicenBrojTakmicaraIzKlubaTak2 = propozicije.NeogranicenBrojTakmicaraIzKlubaTak2;
                    p.MaxBrojTakmicaraIzKlubaTak2 = propozicije.MaxBrojTakmicaraIzKlubaTak2;
                    p.BrojFinalistaTak2 = propozicije.BrojFinalistaTak2;
                    p.BrojRezerviTak2 = propozicije.BrojRezerviTak2;
                }
            }
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

        private void txtMaxRezervi_TextChanged(object sender, EventArgs e)
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
    }
}

