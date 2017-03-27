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
    public partial class Takmicenje4Page : PropertyPage
    {
        private Propozicije propozicije;
        private IList<Propozicije> dependentPropozicije;
        private bool dirty;

        public Takmicenje4Page(Propozicije propozicije,
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
                rbtNaOsnovuTak1.Enabled = false;
                rbtPostojiTak4ZaSvakuKategoriju.Enabled = false;
                rbtJednoTak4ZaSveKategorije.Enabled = false;
                lblBrojRezultata.Enabled = false;
                txtBrojRezultata.Enabled = false;
                lblBrojEkipa.Enabled = false;
                txtBrojEkipa.Enabled = false;
            }
            else
            {
                rbtOdvojenoTak4.Enabled = true;
                rbtNaOsnovuTak1.Enabled = true;
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
            if (rbtNaOsnovuTak1.Checked)
                setEnabledOdvojenoTak4();
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

        private void setEnabledOdvojenoTak4()
        {
            bool postojiTak4 = rbtOdvojenoTak4.Enabled && (rbtOdvojenoTak4.Checked || rbtNaOsnovuTak1.Checked);
            rbtPostojiTak4ZaSvakuKategoriju.Enabled = postojiTak4;
            rbtJednoTak4ZaSveKategorije.Enabled = postojiTak4;
            lblBrojRezultata.Enabled = postojiTak4;
            txtBrojRezultata.Enabled = postojiTak4;

            bool odvojenoTak4 = rbtOdvojenoTak4.Enabled && rbtOdvojenoTak4.Checked;
            lblBrojEkipa.Enabled = odvojenoTak4;
            txtBrojEkipa.Enabled = odvojenoTak4;            
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
                rbtNaOsnovuTak1.Checked = !propozicije.OdvojenoTak4;

                rbtPostojiTak4ZaSvakuKategoriju.Checked = !propozicije.JednoTak4ZaSveKategorije;
                rbtJednoTak4ZaSveKategorije.Checked = propozicije.JednoTak4ZaSveKategorije;

                txtBrojRezultata.Text = propozicije.BrojRezultataKojiSeBodujuZaEkipu.ToString();
                if (propozicije.OdvojenoTak4)
                    txtBrojEkipa.Text = propozicije.BrojEkipaUFinalu.ToString();
            }

            enableHandlers();
            setEnabled();
        }

        private void disableHandlers()
        {
            ckbPostojiTak4.CheckedChanged -= ckbPostojiTak4_CheckedChanged;
            rbtOdvojenoTak4.CheckedChanged -= rbtOdvojenoTak4_CheckedChanged;
            rbtNaOsnovuTak1.CheckedChanged -= rbtNaOsnovuTak1_CheckedChanged;
            rbtPostojiTak4ZaSvakuKategoriju.CheckedChanged -= rbtPostojiTak4ZaSvakuKategoriju_CheckedChanged;
            rbtJednoTak4ZaSveKategorije.CheckedChanged -= rbtJednoTak4ZaSveKategorije_CheckedChanged;
        }

        private void enableHandlers()
        {
            ckbPostojiTak4.CheckedChanged += ckbPostojiTak4_CheckedChanged;
            rbtOdvojenoTak4.CheckedChanged += rbtOdvojenoTak4_CheckedChanged;
            rbtNaOsnovuTak1.CheckedChanged += rbtNaOsnovuTak1_CheckedChanged;
            rbtPostojiTak4ZaSvakuKategoriju.CheckedChanged += rbtPostojiTak4ZaSvakuKategoriju_CheckedChanged;
            rbtJednoTak4ZaSveKategorije.CheckedChanged += rbtJednoTak4ZaSveKategorije_CheckedChanged;
        }

        private void clearUI()
        {
            ckbPostojiTak4.Checked = false;
            rbtOdvojenoTak4.Checked = false;
            rbtNaOsnovuTak1.Checked = false;
            rbtPostojiTak4ZaSvakuKategoriju.Checked = false;
            rbtJednoTak4ZaSveKategorije.Checked = false;
            txtBrojRezultata.Text = String.Empty;
            txtBrojEkipa.Text = String.Empty;
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
            propozicije.validateTakmicenje4(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            propozicije.updateTakmicenje4(dependentPropozicije);
        }

        private void requiredFieldsAndFormatValidation(Notification notification)
        {
            byte dummyByte;
            if (!ckbPostojiTak4.Checked)
                return;
            if (!rbtOdvojenoTak4.Checked && !rbtNaOsnovuTak1.Checked)
            {
                notification.RegisterMessage(
                    "OdvojenoTak4", "Izaberite da li se takmicenje IV posebno odrzava, " +
                    "ili se racuna na osnovu rezultata takmicenja I.");
            }
            if (!rbtPostojiTak4ZaSvakuKategoriju.Checked && !rbtJednoTak4ZaSveKategorije.Checked)
            {
                notification.RegisterMessage(
                    "JednoTak4ZaSveKategorije", "Izaberite da li za svaku kategoriju " +
                    "postoji takmicenje IV, ili postoji jedno takmicenje IV za sve kategorije.");
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
        }

        private void updatePropozicijeFromUI(Propozicije propozicije)
        {
            propozicije.PostojiTak4 = ckbPostojiTak4.Checked;
            if (propozicije.PostojiTak4)
            {
                propozicije.OdvojenoTak4 = rbtOdvojenoTak4.Checked;
                propozicije.JednoTak4ZaSveKategorije = rbtJednoTak4ZaSveKategorije.Checked;

                propozicije.BrojRezultataKojiSeBodujuZaEkipu = byte.Parse(txtBrojRezultata.Text);
                if (propozicije.OdvojenoTak4)
                    propozicije.BrojEkipaUFinalu = byte.Parse(txtBrojEkipa.Text);
            }
        }
    }
}

