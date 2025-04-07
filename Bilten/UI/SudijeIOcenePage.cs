using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Data;
using System.Globalization;
using Bilten.Util;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Dao.NHibernate;

namespace Bilten.UI
{
    public partial class SudijeIOcenePage : Bilten.UI.PropertyPage
    {
        private bool dirty;
        private Takmicenje takmicenje;
        
        public SudijeIOcenePage(Takmicenje takmicenje)
        {
            InitializeComponent();
            this.takmicenje = takmicenje;
        }

        public override string Text
        {
            get { return "Sudije i ocene"; }
        }

        public override void OnSetActive()
        {
            refreshUI();
            dirty = false;
        }

        private void refreshUI()
        {
            clearUI();
            
            txtBrojEOcena.Text = takmicenje.BrojEOcena.ToString();
            txtBrojDecD.Text = takmicenje.BrojDecimalaD.ToString();
            txtBrojDecE1.Text = takmicenje.BrojDecimalaE1.ToString();
            txtBrojDecE.Text = takmicenje.BrojDecimalaE.ToString();
            txtBrojDecBon.Text = takmicenje.BrojDecimalaBon.ToString();
            txtBrojDecPen.Text = takmicenje.BrojDecimalaPen.ToString();
            txtBrojDecTotal.Text = takmicenje.BrojDecimalaTotal.ToString();
            ckbTakBroj.Checked = takmicenje.TakBrojevi;

            ckbOdbaciMinMaxEOcenu.Enabled = takmicenje.BrojEOcena > 0;
            if (ckbOdbaciMinMaxEOcenu.Enabled)
                ckbOdbaciMinMaxEOcenu.Checked = takmicenje.OdbaciMinMaxEOcenu;

            txtBrojEOcenaTak3.Text = takmicenje.BrojEOcenaTak3.ToString();
            ckbOdbaciMinMaxEOcenuTak3.Enabled = takmicenje.BrojEOcenaTak3 > 0;
            if (ckbOdbaciMinMaxEOcenuTak3.Enabled)
                ckbOdbaciMinMaxEOcenuTak3.Checked = takmicenje.OdbaciMinMaxEOcenuTak3;
        }

        private void clearUI()
        {
            txtBrojEOcena.Text = String.Empty;
            txtBrojDecD.Text = String.Empty;
            txtBrojDecE1.Text = String.Empty;
            txtBrojDecE.Text = String.Empty;
            txtBrojDecBon.Text = String.Empty;
            txtBrojDecPen.Text = String.Empty;
            txtBrojDecTotal.Text = String.Empty;
            ckbTakBroj.Checked = false;
            ckbOdbaciMinMaxEOcenu.Enabled = false;
            txtBrojEOcenaTak3.Text = String.Empty;
            ckbOdbaciMinMaxEOcenuTak3.Enabled = false;
        }

        private void txtBrojESudija_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojEOcena_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
            byte brojEOcena;
            ckbOdbaciMinMaxEOcenu.Enabled = txtBrojEOcena.Text.Trim() != String.Empty
                && byte.TryParse(txtBrojEOcena.Text, out brojEOcena) && brojEOcena > 0;
            ckbOdbaciMinMaxEOcenu.Checked = ckbOdbaciMinMaxEOcenu.Enabled;
        }

        private void txtBrojEOcenaTak3_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
            byte brojEOcenaTak3;
            ckbOdbaciMinMaxEOcenuTak3.Enabled = txtBrojEOcenaTak3.Text.Trim() != String.Empty
                && byte.TryParse(txtBrojEOcenaTak3.Text, out brojEOcenaTak3) && brojEOcenaTak3 > 0;
            ckbOdbaciMinMaxEOcenuTak3.Checked = ckbOdbaciMinMaxEOcenuTak3.Enabled;
        }

        private void txtBrojDecD_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojDecE1_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojDecE_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojDecBon_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojDecPen_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void txtBrojDecTotal_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        public override void OnApply()
        {
            // TODO4: Izuzeci koji ce bacaju unutar ovog metoda mogu biti uhvaceni kada je u PropozicijeForm
            // predjeno na neku drugu stranu. Treba ponovo prikazati ovu stranu i oznaciti svojstvo koje je problematicno.

            if (!dirty)
                return;

            Notification notification = new Notification();
            requiredFieldsAndFormatValidation(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            validate();
            checkBusinessRulesOnUpdate();
            update();
        }

        private void requiredFieldsAndFormatValidation(Notification notification)
        {
            byte dummyByte;
            if (txtBrojEOcena.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojEOcena", "Broj E ocena je obavezan.");
            }
            else if (!byte.TryParse(txtBrojEOcena.Text, out dummyByte))
            {
                notification.RegisterMessage(
                    "BrojEOcena", "Neispravan format za broj E ocena.");
            }

            if (txtBrojEOcenaTak3.Text.Trim() != String.Empty && !byte.TryParse(txtBrojEOcenaTak3.Text, out dummyByte))
            {
                notification.RegisterMessage(
                    "BrojEOcenaTak3", "Neispravan format za broj E ocena za takmicenje III.");
            }
       
            if (txtBrojDecD.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaD", "Broj decimala D ocene je obavezan.");
            }
            else if (!byte.TryParse(txtBrojDecD.Text, out dummyByte))
            {
                notification.RegisterMessage(
                    "BrojDecimalaD", "Nepravilan format za broj decimala D ocene.");
            }

            if (txtBrojDecE1.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaE1", "Broj decimala E1-E6 ocena je obavezan.");
            }
            else if (!byte.TryParse(txtBrojDecE1.Text, out dummyByte))
            {
                notification.RegisterMessage(
                    "BrojDecimalaE1", "Nepravilan format za broj decimala E1-E6 ocena.");
            }

            if (txtBrojDecE.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaE", "Broj decimala E ocene je obavezan.");
            }
            else if (!byte.TryParse(txtBrojDecE.Text, out dummyByte))
            {
                notification.RegisterMessage(
                    "BrojDecimalaE", "Nepravilan format za broj decimala E ocene.");
            }

            if (txtBrojDecBon.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaBon", "Broj decimala za bonus je obavezan.");
            }
            else if (!byte.TryParse(txtBrojDecBon.Text, out dummyByte))
            {
                notification.RegisterMessage(
                    "BrojDecimalaBon", "Nepravilan format za broj decimala za bonus.");
            }

            if (txtBrojDecPen.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaPen", "Broj decimala penalizacije je obavezan.");
            }
            else if (!byte.TryParse(txtBrojDecPen.Text, out dummyByte))
            {
                notification.RegisterMessage(
                    "BrojDecimalaPen", "Nepravilan format za broj decimala penalizacije.");
            }

            if (txtBrojDecTotal.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaTotal", "Broj decimala konacne ocene je obavezan.");
            }
            else if (!byte.TryParse(txtBrojDecTotal.Text, out dummyByte))
            {
                notification.RegisterMessage(
                    "BrojDecimalaTotal", "Nepravilan format za broj decimala konacne ocene.");
            }
        }

        private void validate()
        {
            byte brojEOcena = byte.Parse(txtBrojEOcena.Text);
            if (brojEOcena < 0 || brojEOcena > 6)
            {
                throw new BusinessException("BrojEOcena",
                    "Neispravna vrednost za broj E ocena.");
            }

            if (txtBrojEOcenaTak3.Text.Trim() != String.Empty)
            {
                brojEOcena = byte.Parse(txtBrojEOcenaTak3.Text);
                if (brojEOcena < 0 || brojEOcena > 6)
                {
                    throw new BusinessException("BrojEOcenaTak3",
                        "Neispravna vrednost za broj E ocena za takmicenje III.");
                }
            }

            byte brojDecD = byte.Parse(txtBrojDecD.Text);
            if (brojDecD <= 0 || brojDecD > 3)
            {
                throw new BusinessException("BrojDecimalaD", 
                    "Broj decimala D ocene mora da bude izmedju 1 i 3.");
            }

            byte brojDecE1 = byte.Parse(txtBrojDecE1.Text);
            if (brojDecE1 <= 0 || brojDecE1 > 3)
            {
                throw new BusinessException("BrojDecimalaE1", 
                    "Broj decimala E1-E6 ocena mora da bude izmedju 1 i 3.");
            }

            byte brojDecE = byte.Parse(txtBrojDecE.Text);
            if (brojDecE <= 0 || brojDecE > 3)
            {
                throw new BusinessException("BrojDecimalaE", 
                    "Broj decimala E ocene mora da bude izmedju 1 i 3.");
            }

            byte brojDecBon = byte.Parse(txtBrojDecBon.Text);
            if (brojDecBon <= 0 || brojDecBon > 3)
            {
                throw new BusinessException("BrojDecimalaBon",
                    "Broj decimala za bonus mora da bude izmedju 1 i 3.");
            }

            byte brojDecPen = byte.Parse(txtBrojDecPen.Text);
            if (brojDecPen <= 0 || brojDecPen > 3)
            {
                throw new BusinessException("BrojDecimalaPen", 
                    "Broj decimala penalizacije mora da bude izmedju 1 i 3.");
            }

            byte brojDecTotal = byte.Parse(txtBrojDecTotal.Text);
            if (brojDecTotal <= 0 || brojDecTotal > 3)
            {
                throw new BusinessException("BrojDecimalaTotal", 
                    "Broj decimala konacne ocene mora da bude izmedju 1 i 3.");
            }
        }

        private void checkBusinessRulesOnUpdate()
        {
            if (!postojeUneteOcene(takmicenje.Id)) // can throw, a hvata ga PropozicijeForm
                return;

            byte brojEOcena = byte.Parse(txtBrojEOcena.Text);
            byte brojDecD = byte.Parse(txtBrojDecD.Text);
            byte brojDecE1 = byte.Parse(txtBrojDecE1.Text);
            byte brojDecE = byte.Parse(txtBrojDecE.Text);
            byte brojDecBon = byte.Parse(txtBrojDecBon.Text);
            byte brojDecPen = byte.Parse(txtBrojDecPen.Text);
            byte brojDecTotal = byte.Parse(txtBrojDecTotal.Text);

            // TODO3: Obradi ovde i situaciju kada postoje rasporedi sudija.
            // Verovatno bi trebalo brisati ili dodavati e sudijske uloge 
            // u sudijskim odborima

            if (brojEOcena != takmicenje.BrojEOcena)
            {
                throw new BusinessException("BrojEOcena",
                    "Nije dozvoljeno menjati broj E ocena zato sto " +
                    "vec postoje unete ocene.");
            }
            else if (brojDecD < takmicenje.BrojDecimalaD
            || brojDecE1 < takmicenje.BrojDecimalaE1
            || brojDecE < takmicenje.BrojDecimalaE
            || brojDecBon < takmicenje.BrojDecimalaBon
            || brojDecPen < takmicenje.BrojDecimalaPen
            || brojDecTotal < takmicenje.BrojDecimalaTotal)
            {
                // TODO5: Ovo je nepotrebno, jer se broj decimala koristi samo za stampanje
                throw new BusinessException("BrojDecimalaD",
                    "Nije dozvoljeno smanjivati broj decimala zato sto " +
                    "vec postoje unete ocene sa vecim brojem decimala.");
            }

            if (txtBrojEOcenaTak3.Text.Trim() != String.Empty)
            {
                // byte brojEOcenaTak3 = byte.Parse(txtBrojEOcenaTak3.Text);
                // TODO5: Ako vec postoje unesene ocene za takmicenje 3, nije dozvoljeno menjati broj ocena za tak3
                // TODO5: Razmisli da li je potrebno apdejtovati sva takmicenja u kojima je broj e ocena bio veci od nula,
                //        tako da broj e ocena u takmicenju 3 bude isti. (Primetiti da je broj e ocena zapamcen i u
                //        takmicenju, i u oceni). 
            }
        }

        private bool postojeUneteOcene(int takmicenjeId)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                    ocenaDAO.Session = session;
                    return ocenaDAO.existsOcene(takmicenjeId);
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {

            }
        }

        private void update()
        {
            takmicenje.BrojEOcena = byte.Parse(txtBrojEOcena.Text);
            takmicenje.BrojDecimalaD = byte.Parse(txtBrojDecD.Text);
            takmicenje.BrojDecimalaE1 = byte.Parse(txtBrojDecE1.Text);
            takmicenje.BrojDecimalaE = byte.Parse(txtBrojDecE.Text);
            takmicenje.BrojDecimalaBon = byte.Parse(txtBrojDecBon.Text);
            takmicenje.BrojDecimalaPen = byte.Parse(txtBrojDecPen.Text);
            takmicenje.BrojDecimalaTotal = byte.Parse(txtBrojDecTotal.Text);
            takmicenje.TakBrojevi = ckbTakBroj.Checked;
            if (takmicenje.BrojEOcena > 0)
                takmicenje.OdbaciMinMaxEOcenu = ckbOdbaciMinMaxEOcenu.Checked;
 
            if (txtBrojEOcenaTak3.Text.Trim() != String.Empty)
            {
                takmicenje.BrojEOcenaTak3 = byte.Parse(txtBrojEOcenaTak3.Text);
                if (takmicenje.BrojEOcenaTak3 > 0)
                    takmicenje.OdbaciMinMaxEOcenuTak3 = ckbOdbaciMinMaxEOcenuTak3.Checked;
            }
        }

        private void ckbTakBroj_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void ckbOdbaciMinMaxEOcenu_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void ckbOdbaciMinMaxEOcenuTak3_CheckedChanged(object sender, EventArgs e)
        {
            dirty = true;
        }
    }
}

