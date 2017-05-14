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
                lblBrojFinalista.Enabled = false;
                txtBrojFinalista.Enabled = false;
                lblBrojRezervi.Enabled = false;
                txtBrojRezervi.Enabled = false;
                ckbNeogranicenBrojTak.Enabled = false;
                lblMaxTak.Enabled = false;
                txtMaxTak.Enabled = false;
                rbtKlub.Enabled = false;
                rbtDrzava.Enabled = false;

                lblKvalPreskok.Enabled = false;
                rbtKvalPreskok1.Enabled = false;
                rbtKvalPreskok2.Enabled = false;
                lblPoredakPreskok.Enabled = false;
                rbtPoredakPreskok1.Enabled = false;
                rbtPoredakPreskok2.Enabled = false;

                lblIstaKonacnaOcena.Enabled = false;
                rbtGimnasticariDelePlasman.Enabled = false;
                rbtPrednostImaVecaEOcena.Enabled = false;
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
            bool postojiTak3 = rbtOdvojenoTak3.Enabled && (rbtOdvojenoTak3.Checked || rbtNaOsnovuTak1.Checked);
            lblKvalPreskok.Enabled = postojiTak3;
            rbtKvalPreskok1.Enabled = postojiTak3;
            rbtKvalPreskok2.Enabled = postojiTak3;
            lblIstaKonacnaOcena.Enabled = postojiTak3;
            rbtGimnasticariDelePlasman.Enabled = postojiTak3;
            rbtPrednostImaVecaEOcena.Enabled = postojiTak3;

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

        private void txtBrojFinalista_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojRezervi_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
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

        private void rbtGimnasticariDelePlasman_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void rbtPrednostImaVecaEOcena_CheckedChanged(object sender, EventArgs e)
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

            // Nek ovo bude uvek selektovano
            rbtGimnasticariDelePlasman.Checked = !propozicije.VecaEOcenaImaPrednost;
            rbtPrednostImaVecaEOcena.Checked = propozicije.VecaEOcenaImaPrednost;

            if (propozicije.PostojiTak3)
            {
                rbtOdvojenoTak3.Checked = propozicije.OdvojenoTak3;
                rbtNaOsnovuTak1.Checked = !propozicije.OdvojenoTak3;

                if (!propozicije.OdvojenoTak3)
                {
                    rbtKvalPreskok1.Checked = !propozicije.Tak1PreskokNaOsnovuObaPreskoka;
                    rbtKvalPreskok2.Checked = propozicije.Tak1PreskokNaOsnovuObaPreskoka;
                }
                else
                {
                    txtBrojFinalista.Text = propozicije.BrojFinalistaTak3.ToString();
                    txtBrojRezervi.Text = propozicije.BrojRezerviTak3.ToString();

                    ckbNeogranicenBrojTak.Checked = propozicije.NeogranicenBrojTakmicaraIzKlubaTak3;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                    {
                        txtMaxTak.Text = propozicije.MaxBrojTakmicaraIzKlubaTak3.ToString();
                        rbtDrzava.Checked = propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu;
                    }

                    rbtKvalPreskok1.Checked = !propozicije.Tak1PreskokNaOsnovuObaPreskoka;
                    rbtKvalPreskok2.Checked = propozicije.Tak1PreskokNaOsnovuObaPreskoka;

                    rbtPoredakPreskok1.Checked = !propozicije.Tak3PreskokNaOsnovuObaPreskoka;
                    rbtPoredakPreskok2.Checked = propozicije.Tak3PreskokNaOsnovuObaPreskoka;
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

            rbtGimnasticariDelePlasman.CheckedChanged -= rbtGimnasticariDelePlasman_CheckedChanged;
            rbtPrednostImaVecaEOcena.CheckedChanged -= rbtPrednostImaVecaEOcena_CheckedChanged;
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

            rbtGimnasticariDelePlasman.CheckedChanged += rbtGimnasticariDelePlasman_CheckedChanged;
            rbtPrednostImaVecaEOcena.CheckedChanged += rbtPrednostImaVecaEOcena_CheckedChanged;
        }

        private void clearUI()
        {
            ckbPostojiTak3.Checked = false;
            rbtOdvojenoTak3.Checked = false;
            rbtNaOsnovuTak1.Checked = false;
            txtBrojFinalista.Text = String.Empty;
            txtBrojRezervi.Text = String.Empty;
            ckbNeogranicenBrojTak.Checked = false;
            rbtKlub.Checked = true;
            txtMaxTak.Text = String.Empty;

            rbtKvalPreskok1.Checked = false;
            rbtKvalPreskok2.Checked = false;
            rbtPoredakPreskok1.Checked = false;
            rbtPoredakPreskok2.Checked = false;

            rbtGimnasticariDelePlasman.Checked = false;
            rbtPrednostImaVecaEOcena.Checked = false;
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
            propozicije.validateTakmicenje3(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            propozicije.copyTakmicenje3To(dependentPropozicije);
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

            if (rbtKvalPreskok1.Enabled && !rbtKvalPreskok1.Checked && !rbtKvalPreskok2.Checked)
            {
                notification.RegisterMessage(
                    "Tak1PreskokNaOsnovuObaPreskoka",
                    "Izaberite da li se preskok u takmicenju I " +
                    "racuna na osnovu prvog ili oba preskoka.");
            }

            if (rbtPoredakPreskok1.Enabled && !rbtPoredakPreskok1.Checked && !rbtPoredakPreskok2.Checked)
            {
                notification.RegisterMessage(
                    "Tak3PreskokNaOsnovuObaPreskoka",
                    "Izaberite da li se preskok u takmicenju III " +
                    "racuna na osnovu prvog ili oba preskoka.");
            }

            if (rbtGimnasticariDelePlasman.Enabled && !rbtGimnasticariDelePlasman.Checked
                && !rbtPrednostImaVecaEOcena.Checked)
            {
                notification.RegisterMessage(
                    "VecaEOcenaImaPrednost",
                    "Izaberite kako se racuna plasman kada gimnasticari imaju istu ocenu.");
            }
        }

        private void updatePropozicijeFromUI(Propozicije propozicije)
        {
            propozicije.PostojiTak3 = ckbPostojiTak3.Checked;
            if (!propozicije.PostojiTak3)
            {
                // NOTE: Cak i ako se izabere da ne postoji takmicenje 3, rezultati ce se prikazivati. Proizvoljno sam
                // izabrao da se tada preskok racuna na osnovu prvog preskoka
                propozicije.Tak1PreskokNaOsnovuObaPreskoka = false;
            }
            else
            {
                propozicije.OdvojenoTak3 = rbtOdvojenoTak3.Checked;
                propozicije.VecaEOcenaImaPrednost = rbtPrednostImaVecaEOcena.Checked;

                if (!propozicije.OdvojenoTak3)
                    propozicije.Tak1PreskokNaOsnovuObaPreskoka = rbtKvalPreskok2.Checked;
                else
                {
                    propozicije.BrojFinalistaTak3 = byte.Parse(txtBrojFinalista.Text);
                    propozicije.BrojRezerviTak3 = byte.Parse(txtBrojRezervi.Text);

                    propozicije.NeogranicenBrojTakmicaraIzKlubaTak3 = ckbNeogranicenBrojTak.Checked;
                    if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                    {
                        propozicije.MaxBrojTakmicaraIzKlubaTak3 = byte.Parse(txtMaxTak.Text);
                        propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu = rbtDrzava.Checked;
                    }
                    propozicije.Tak1PreskokNaOsnovuObaPreskoka = rbtKvalPreskok2.Checked;
                    propozicije.Tak3PreskokNaOsnovuObaPreskoka = rbtPoredakPreskok2.Checked;
                }
            }
        }
    }
}

