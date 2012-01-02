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
    public partial class Takmicenje3Page : PropertyPage
    {
        private Propozicije propozicije;
        private IList<Propozicije> dependentPropozicije;
        private bool dirty;

        public Takmicenje3Page(Propozicije propozicije, 
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
                rbtNaOsnovuTak1.Enabled = false;
                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
                rbtKlub.Enabled = false;
                rbtDrzava.Enabled = false;
                ckbNeogranicenBrojTak.Enabled = false;
                lblBrojFinalista.Enabled = false;
                txtBrojFinalista.Enabled = false;
                lblBrojRezervi.Enabled = false;
                txtBrojRezervi.Enabled = false;

                lblKvalPreskok.Enabled = false;
                rbtKvalPreskok1.Enabled = false;
                rbtKvalPreskok2.Enabled = false;
                lblPoredakPreskok.Enabled = false;
                rbtPoredakPreskok1.Enabled = false;
                rbtPoredakPreskok2.Enabled = false;
            }
            else
            {
                rbtOdvojenoTak3.Enabled = true;
                rbtNaOsnovuTak1.Enabled = true;
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
            if (rbtNaOsnovuTak1.Checked)
                setEnabledOdvojenoTak3();
        }

        private void setEnabledOdvojenoTak3()
        {
            if (!rbtOdvojenoTak3.Enabled && !rbtNaOsnovuTak1.Enabled)
                return;

            if (!rbtOdvojenoTak3.Checked && !rbtNaOsnovuTak1.Checked)
            {
                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
                rbtKlub.Enabled = false;
                rbtDrzava.Enabled = false;
                ckbNeogranicenBrojTak.Enabled = false;
                lblBrojFinalista.Enabled = false;
                txtBrojFinalista.Enabled = false;
                lblBrojRezervi.Enabled = false;
                txtBrojRezervi.Enabled = false;
                
                lblKvalPreskok.Enabled = false;
                rbtKvalPreskok1.Enabled = false;
                rbtKvalPreskok2.Enabled = false;
                lblPoredakPreskok.Enabled = false;
                rbtPoredakPreskok1.Enabled = false;
                rbtPoredakPreskok2.Enabled = false;
            }
            else
            {
                ckbNeogranicenBrojTak.Enabled = true;
                setEnabledNeogranicenBrojTak();

                lblPoredakPreskok.Enabled = true;
                rbtPoredakPreskok1.Enabled = true;
                rbtPoredakPreskok2.Enabled = true;

                bool odvojenoTak3 = rbtOdvojenoTak3.Checked;
                lblBrojFinalista.Enabled = odvojenoTak3;
                txtBrojFinalista.Enabled = odvojenoTak3;
                lblBrojRezervi.Enabled = odvojenoTak3;
                txtBrojRezervi.Enabled = odvojenoTak3;

                lblKvalPreskok.Enabled = odvojenoTak3;
                rbtKvalPreskok1.Enabled = odvojenoTak3;
                rbtKvalPreskok2.Enabled = odvojenoTak3;
            }
        }

        private void setEnabledNeogranicenBrojTak()
        {
            if (!ckbNeogranicenBrojTak.Enabled)
                return;

            bool neogranicenBrojTak = ckbNeogranicenBrojTak.Checked;
            lblMaxTak.Enabled = !neogranicenBrojTak;
            txtMaxTak.Enabled = !neogranicenBrojTak;
            rbtKlub.Enabled = !neogranicenBrojTak;
            rbtDrzava.Enabled = !neogranicenBrojTak;
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

            ckbPostojiTak3.Checked = propozicije.PostojiTak3;
            if (propozicije.PostojiTak3)
            {
                rbtOdvojenoTak3.Checked = propozicije.OdvojenoTak3;
                rbtNaOsnovuTak1.Checked = propozicije.Tak3NaOsnovuTak1;

                ckbNeogranicenBrojTak.Checked = propozicije.NeogranicenBrojTakmicaraIzKlubaTak3;
                if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                {
                    txtMaxTak.Text = propozicije.MaxBrojTakmicaraIzKlubaTak3.ToString();
                    rbtDrzava.Checked = propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu;
                }

                rbtPoredakPreskok1.Checked = !propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;
                rbtPoredakPreskok2.Checked = propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;

                if (propozicije.OdvojenoTak3)
                {
                    txtBrojFinalista.Text = propozicije.BrojFinalistaTak3.ToString();
                    txtBrojRezervi.Text = propozicije.BrojRezerviTak3.ToString();

                    rbtKvalPreskok1.Checked = !propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                    rbtKvalPreskok2.Checked = propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                }
            }
            
            enableHandlers();
            setEnabled();
        }

        private void disableHandlers()
        {
            ckbPostojiTak3.CheckedChanged -= ckbPostojiTak3_CheckedChanged;
            rbtOdvojenoTak3.CheckedChanged -= rbtOdvojenoTak3_CheckedChanged;
            rbtNaOsnovuTak1.CheckedChanged -= rbtNaOsnovuTak1_CheckedChanged;
            ckbNeogranicenBrojTak.CheckedChanged -= ckbNeogranicenBrojTak_CheckedChanged;

            rbtKvalPreskok1.CheckedChanged -= rbtKvalPreskok_CheckedChanged;
            rbtKvalPreskok2.CheckedChanged -= rbtKvalPreskok_CheckedChanged;
            rbtPoredakPreskok1.CheckedChanged -= rbtPoredakPreskok_CheckedChanged;
            rbtPoredakPreskok2.CheckedChanged -= rbtPoredakPreskok_CheckedChanged;
        }

        private void enableHandlers()
        {
            ckbPostojiTak3.CheckedChanged += ckbPostojiTak3_CheckedChanged;
            rbtOdvojenoTak3.CheckedChanged += rbtOdvojenoTak3_CheckedChanged;
            rbtNaOsnovuTak1.CheckedChanged += rbtNaOsnovuTak1_CheckedChanged;
            ckbNeogranicenBrojTak.CheckedChanged += ckbNeogranicenBrojTak_CheckedChanged;

            rbtKvalPreskok1.CheckedChanged += rbtKvalPreskok_CheckedChanged;
            rbtKvalPreskok2.CheckedChanged += rbtKvalPreskok_CheckedChanged;
            rbtPoredakPreskok1.CheckedChanged += rbtPoredakPreskok_CheckedChanged;
            rbtPoredakPreskok2.CheckedChanged += rbtPoredakPreskok_CheckedChanged;
        }

        private void clearUI()
        {
            ckbPostojiTak3.Checked = false;
            rbtOdvojenoTak3.Checked = false;
            rbtNaOsnovuTak1.Checked = false;
            ckbNeogranicenBrojTak.Checked = false;
            rbtKlub.Checked = true;
            txtMaxTak.Text = String.Empty;
            txtBrojFinalista.Text = String.Empty;
            txtBrojRezervi.Text = String.Empty;

            rbtKvalPreskok1.Checked = false;
            rbtKvalPreskok2.Checked = false;
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

            validate();
            updatePropozicije();
        }

        private void requiredFieldsAndFormatValidation(Notification notification)
        {
            byte dummyByte;
            if (!ckbPostojiTak3.Checked)
                return;
            if (!rbtOdvojenoTak3.Checked && !rbtNaOsnovuTak1.Checked)
            {
                notification.RegisterMessage(
                    "OdvojenoTak3", "Izaberite da li se takmicenje III posebno odrzava, " +
                    "ili se racuna na osnovu rezultata takmicenja I.");
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
                    // TODO: Trebalo bi (i ovde i na drugim mestima) format proveravati
                    // pomocu regular expressions, jer moze da se desi npr. da se
                    // unese broj 300 i da se dobije poruka da je format neispravan
                    notification.RegisterMessage(
                        "MaxBrojTakmicaraIzKlubaTak3", "Neispravan format za maksimalan broj " +
                        "takmicara iz istog kluba/drzave.");
                }
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

            if (!rbtPoredakPreskok1.Checked && !rbtPoredakPreskok2.Checked)
            {
                notification.RegisterMessage(
                    "PoredakTak3PreskokNaOsnovuObaPreskoka",
                    "Izaberite da li se poredak za " +
                    "preskok racuna na osnovu prvog ili oba preskoka.");
            }

            if (rbtKvalPreskok1.Enabled)
            {
                if (!rbtKvalPreskok1.Checked && !rbtKvalPreskok2.Checked)
                {
                    notification.RegisterMessage(
                        "KvalifikantiTak3PreskokNaOsnovuObaPreskoka",
                        "Izaberite da li se kvalifikanti za finale " +
                        "preskoka racunaju na osnovu prvog ili oba preskoka.");
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
                    throw new BusinessException("MaxBrojTakmicaraIzKlubaTak3", 
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
                        "BrojFinalistaTak3", "Neispravna vrednost za broj finalista.");
                }
            }

            if (txtBrojRezervi.Enabled)
            {
                byte brojRezervi = byte.Parse(txtBrojRezervi.Text);
                if (brojRezervi < 1)
                {
                    throw new BusinessException(
                        "BrojRezerviTak3", "Neispravna vrednost za broj rezervi.");
                }
            }
        }

        private void updatePropozicije()
        {
            propozicije.PostojiTak3 = ckbPostojiTak3.Checked;
            if (propozicije.PostojiTak3)
            {
                propozicije.OdvojenoTak3 = rbtOdvojenoTak3.Checked;
                propozicije.Tak3NaOsnovuTak1 = rbtNaOsnovuTak1.Checked;

                propozicije.NeogranicenBrojTakmicaraIzKlubaTak3 = ckbNeogranicenBrojTak.Checked;
                if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                {
                    propozicije.MaxBrojTakmicaraIzKlubaTak3 = byte.Parse(txtMaxTak.Text);
                    propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu = rbtDrzava.Checked;
                }

                propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka =
                    rbtPoredakPreskok2.Checked;

                if (propozicije.OdvojenoTak3)
                {
                    propozicije.BrojFinalistaTak3 = byte.Parse(txtBrojFinalista.Text);
                    propozicije.BrojRezerviTak3 = byte.Parse(txtBrojRezervi.Text);

                    propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka =
                        rbtKvalPreskok2.Checked;
                }
            }

            if (dependentPropozicije != null)
            {
                foreach (Propozicije p in dependentPropozicije)
                {
                    p.PostojiTak3 = propozicije.PostojiTak3;
                    p.OdvojenoTak3 = propozicije.OdvojenoTak3;
                    p.Tak3NaOsnovuTak1 = propozicije.Tak3NaOsnovuTak1;
                    p.NeogranicenBrojTakmicaraIzKlubaTak3 = propozicije.NeogranicenBrojTakmicaraIzKlubaTak3;
                    p.MaxBrojTakmicaraIzKlubaTak3 = propozicije.MaxBrojTakmicaraIzKlubaTak3;
                    p.MaxBrojTakmicaraTak3VaziZaDrzavu = propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu;
                    p.BrojFinalistaTak3 = propozicije.BrojFinalistaTak3;
                    p.BrojRezerviTak3 = propozicije.BrojRezerviTak3;
                    p.KvalifikantiTak3PreskokNaOsnovuObaPreskoka =
                        propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                    p.PoredakTak3PreskokNaOsnovuObaPreskoka =
                        propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;
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

        private void rbtKvalPreskok_CheckedChanged(object sender, EventArgs e)
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

    }
}

