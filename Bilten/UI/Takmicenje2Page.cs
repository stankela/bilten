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
    public partial class Takmicenje2Page : PropertyPage
    {
        private Propozicije propozicije;
        private IList<Propozicije> dependentPropozicije;
        private bool dirty;

        public Takmicenje2Page(Propozicije propozicije, 
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
                rbtNaOsnovuTak1.Enabled = false;
                lblPreskokViseboj.Enabled = false;
                rbtPrviPreskok.Enabled = false;
                rbtBoljiPreskok.Enabled = false;
                lblBrojFinalista.Enabled = false;
                txtBrojFinalista.Enabled = false;
                lblBrojRezervi.Enabled = false;
                txtBrojRezervi.Enabled = false;
                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
                ckbNeogranicenBrojTak.Enabled = false;
                rbtKlub.Enabled = false;
                rbtDrzava.Enabled = false;
            }
            else
            {
                rbtOdvojenoTak2.Enabled = true;
                rbtNaOsnovuTak1.Enabled = true;
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
            if (rbtNaOsnovuTak1.Checked)
                setEnabledOdvojenoTak2();
        }

        private void rbtPrviPreskok_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void rbtBoljiPreskok_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void setEnabledOdvojenoTak2()
        {
            bool postojiTak2 = rbtOdvojenoTak2.Enabled && (rbtOdvojenoTak2.Checked || rbtNaOsnovuTak1.Checked);
            lblPreskokViseboj.Enabled = postojiTak2;
            rbtPrviPreskok.Enabled = postojiTak2;
            rbtBoljiPreskok.Enabled = postojiTak2;
            
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
                rbtNaOsnovuTak1.Checked = !propozicije.OdvojenoTak2;

                rbtPrviPreskok.Checked = !propozicije.ZaPreskokVisebojRacunajBoljuOcenu;
                rbtBoljiPreskok.Checked = propozicije.ZaPreskokVisebojRacunajBoljuOcenu;

                if (propozicije.OdvojenoTak2)
                {
                    txtBrojFinalista.Text = propozicije.BrojFinalistaTak2.ToString();
                    txtBrojRezervi.Text = propozicije.BrojRezerviTak2.ToString();
                    ckbNeogranicenBrojTak.Checked = propozicije.NeogranicenBrojTakmicaraIzKlubaTak2;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                    {
                        txtMaxTak.Text = propozicije.MaxBrojTakmicaraIzKlubaTak2.ToString();
                        rbtDrzava.Checked = propozicije.MaxBrojTakmicaraTak2VaziZaDrzavu;
                    }
                }
            }
            
            enableHandlers();
            setEnabled();
        }

        private void disableHandlers()
        {
            ckbPostojiTak2.CheckedChanged -= ckbPostojiTak2_CheckedChanged;
            rbtOdvojenoTak2.CheckedChanged -= rbtOdvojenoTak2_CheckedChanged;
            rbtNaOsnovuTak1.CheckedChanged -= rbtNaOsnovuTak1_CheckedChanged;
            rbtPrviPreskok.CheckedChanged -= rbtPrviPreskok_CheckedChanged;
            rbtBoljiPreskok.CheckedChanged -= rbtBoljiPreskok_CheckedChanged;
            ckbNeogranicenBrojTak.CheckedChanged -= ckbNeogranicenBrojTak_CheckedChanged;
        }

        private void enableHandlers()
        {
            ckbPostojiTak2.CheckedChanged += ckbPostojiTak2_CheckedChanged;
            rbtOdvojenoTak2.CheckedChanged += rbtOdvojenoTak2_CheckedChanged;
            rbtNaOsnovuTak1.CheckedChanged += rbtNaOsnovuTak1_CheckedChanged;
            rbtPrviPreskok.CheckedChanged += rbtPrviPreskok_CheckedChanged;
            rbtBoljiPreskok.CheckedChanged += rbtBoljiPreskok_CheckedChanged;
            ckbNeogranicenBrojTak.CheckedChanged += ckbNeogranicenBrojTak_CheckedChanged;
        }

        private void clearUI()
        {
            ckbPostojiTak2.Checked = false;
            rbtOdvojenoTak2.Checked = false;
            rbtNaOsnovuTak1.Checked = false;
            rbtPrviPreskok.Checked = true;
            rbtBoljiPreskok.Checked = false;
            ckbNeogranicenBrojTak.Checked = false;
            rbtKlub.Checked = true;
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
            propozicije.validateTakmicenje2(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            propozicije.copyTakmicenje2To(dependentPropozicije);
        }

        private void requiredFieldsAndFormatValidation(Notification notification)
        {
            byte dummyByte;
            if (!ckbPostojiTak2.Checked)
                return;
            if (!rbtOdvojenoTak2.Checked && !rbtNaOsnovuTak1.Checked)
            {
                notification.RegisterMessage(
                    "OdvojenoTak2", "Izaberite da li se takmicenje II posebno odrzava, " +
                    "ili se racuna na osnovu rezultata takmicenja I.");
            }
            if (!rbtPrviPreskok.Checked && ! rbtBoljiPreskok.Checked)
            {
                notification.RegisterMessage(
                    "ZaPreskokVisebojRacunajBoljuOcenu",
                    "Izaberite da li se za preskok viseboja racuna prvi preskok ili bolji preskok.");
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
        }

        private void updatePropozicijeFromUI(Propozicije propozicije)
        {
            propozicije.PostojiTak2 = ckbPostojiTak2.Checked;
            if (propozicije.PostojiTak2)
            {
                propozicije.OdvojenoTak2 = rbtOdvojenoTak2.Checked;
                propozicije.ZaPreskokVisebojRacunajBoljuOcenu = rbtBoljiPreskok.Checked;
                if (propozicije.OdvojenoTak2)
                {
                    propozicije.BrojFinalistaTak2 = byte.Parse(txtBrojFinalista.Text);
                    propozicije.BrojRezerviTak2 = byte.Parse(txtBrojRezervi.Text);
                    propozicije.NeogranicenBrojTakmicaraIzKlubaTak2 = ckbNeogranicenBrojTak.Checked;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                    {
                        propozicije.MaxBrojTakmicaraIzKlubaTak2 = byte.Parse(txtMaxTak.Text);
                        propozicije.MaxBrojTakmicaraTak2VaziZaDrzavu = rbtDrzava.Checked;
                    }
                }
            }
        }

        private void rbtKlub_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void rbtDrzava_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }
    }
}

