using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using System.Globalization;
using Bilten.Data;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class OcenaForm : EntityDetailForm
    {
        private GimnasticarUcesnik gimnasticar;
        private Sprava sprava;
        private DeoTakmicenjaKod deoTakKod;
        private Takmicenje takmicenje;
        private bool obeOcene;
        private Color selectionColor = Color.Aqua;
        private Color disabledColor = SystemColors.Control;
        private DrugaOcena deletedDrugaOcena;
        private bool izracunato;
        private bool textBoxHandlersDisabled;
        private Ocena original;

        public OcenaForm(Nullable<int> ocenaId, GimnasticarUcesnik g, 
            Sprava sprava, DeoTakmicenjaKod deoTakKod,
            int takmicenjeId)
        {
            InitializeComponent();
            this.gimnasticar = g;
            this.sprava = sprava;
            this.deoTakKod = deoTakKod;
            this.obeOcene = sprava == Sprava.Preskok;
            this.showWaitCursor = true;

            takmicenje = loadTakmicenje(takmicenjeId);
            initialize(ocenaId, true);

            izracunato = editMode && !ckbUnosOcene.Checked;
        }

        protected override DomainObject getEntityById(int id)
        {
            Ocena result = dataContext.GetById<Ocena>(id);

            original = (Ocena)result.shallowCopy();
            if (result.Ocena2 != null)
                original.Ocena2 = (DrugaOcena)result.Ocena2.shallowCopy();
            
            return result;
        }

        protected override DomainObject createNewEntity()
        {
            Ocena result = new Ocena();
            result.Sprava = sprava;
            result.DeoTakmicenjaKod = deoTakKod;
            result.Gimnasticar = gimnasticar;
            result.BrojEOcena = takmicenje.BrojEOcena;
            return result;
        }

        private Takmicenje loadTakmicenje(int takmicenjeId)
        {
            // TODO: Nisam mogao da smestim ucitavanje takmicenja u loadData zato sto
            // mi takmicenje treba u createNewEntity, a u EntityDetailForm je 
            // createNewEntity pre loadData. Ne mogu da u EntityDetailForm promenim
            // redosled - da smestim loadData pre createNewEntity - zato sto neznam 
            // da li cu time nesto pokvariti na drugim mestima.
            // Ne bi bilo lose da izbacim EntityDetailForm i da svaki novi form 
            // kreiram od pocetka, da bih izbegao ovakve zavisnosti
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                IDataContext dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                return dataContext.GetById<Takmicenje>(takmicenjeId);
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Unos ocene";

            pictureBoxSprava.Image = SlikeSprava.getImage(sprava);
            lblKategorijaTakmicenje.Text = gimnasticar.TakmicarskaKategorija.ToString() + " - "
                + DeoTakmicenjaKodovi.toString(deoTakKod);
            lblGimnasticar.Text = gimnasticar.PrezimeImeDrzava;

            ponistiOcena1();

            ckbUnosOcene.Checked = Opcije.Instance.UnosOcenaBezIzrZaCeloTak;
            updateUIRucniUnos();

            int brojEOcena = takmicenje.BrojEOcena;
            txtE1.Enabled = txtE1.Visible = lblE1.Visible = brojEOcena >= 1;
            txtE2.Enabled = txtE2.Visible = lblE2.Visible = brojEOcena >= 2;
            txtE3.Enabled = txtE3.Visible = lblE3.Visible = brojEOcena >= 3;
            txtE4.Enabled = txtE4.Visible = lblE4.Visible = brojEOcena >= 4;
            txtE5.Enabled = txtE5.Visible = lblE5.Visible = brojEOcena >= 5;
            txtE6.Enabled = txtE6.Visible = lblE6.Visible = brojEOcena >= 6;
            txtE.TabStop = brojEOcena == 0 || ckbUnosOcene.Checked;
            txtTotal.TabStop = ckbUnosOcene.Checked;

            ckbDrugaOcena.Enabled = ckbDrugaOcena.Visible = obeOcene;
            txtD_2.Enabled = txtD_2.Visible = obeOcene;
            txtE1_2.Enabled = txtE1_2.Visible = obeOcene;
            txtE2_2.Enabled = txtE2_2.Visible = obeOcene;
            txtE3_2.Enabled = txtE3_2.Visible = obeOcene;
            txtE4_2.Enabled = txtE4_2.Visible = obeOcene;
            txtE5_2.Enabled = txtE5_2.Visible = obeOcene;
            txtE6_2.Enabled = txtE6_2.Visible = obeOcene;
            txtE_2.Enabled = txtE_2.Visible = obeOcene;
            txtPenal_2.Enabled = txtPenal_2.Visible = obeOcene;
            txtTotal_2.Enabled = txtTotal_2.Visible = obeOcene;
            txtTotalObeOcene.Enabled = txtTotalObeOcene.Visible = obeOcene;
            txtE_2.TabStop = brojEOcena == 0 || ckbUnosOcene.Checked;
            txtTotal_2.TabStop = ckbUnosOcene.Checked;
            txtTotalObeOcene.TabStop = ckbUnosOcene.Checked;

            if (obeOcene)
            {
                ponistiOcena2();

                txtE1_2.Enabled = txtE1_2.Visible = brojEOcena >= 1;
                txtE2_2.Enabled = txtE2_2.Visible = brojEOcena >= 2;
                txtE3_2.Enabled = txtE3_2.Visible = brojEOcena >= 3;
                txtE4_2.Enabled = txtE4_2.Visible = brojEOcena >= 4;
                txtE5_2.Enabled = txtE5_2.Visible = brojEOcena >= 5;
                txtE6_2.Enabled = txtE6_2.Visible = brojEOcena >= 6;

                ckbDrugaOcena.Checked = false;
                setEnabledDrugaOcena();
            }
            else
            {
                int offset = 75;
                ckbUnosOcene.Location = 
                    new Point(ckbUnosOcene.Location.X, ckbUnosOcene.Location.Y - offset);
                btnIzracunaj.Location =
                    new Point(btnIzracunaj.Location.X, btnIzracunaj.Location.Y - offset);
                btnPonisti.Location =
                    new Point(btnPonisti.Location.X, btnPonisti.Location.Y - offset);
                btnOk.Location =
                    new Point(btnOk.Location.X, btnOk.Location.Y - offset);
                btnCancel.Location =
                    new Point(btnCancel.Location.X, btnCancel.Location.Y - offset);
                ClientSize = new Size(ClientSize.Width, ClientSize.Height - offset);
            }
        }

        private void ponistiOcena1()
        {
            disableTextBoxHandlers();

            txtD.Text = String.Empty;
            txtE1.Text = String.Empty;
            txtE2.Text = String.Empty;
            txtE3.Text = String.Empty;
            txtE4.Text = String.Empty;
            txtE5.Text = String.Empty;
            txtE6.Text = String.Empty;
            txtE.Text = String.Empty;
            txtPenal.Text = String.Empty;
            txtTotal.Text = String.Empty;

            enableTextBoxHandlers();
        }

        private void ponistiOcena2()
        {
            disableTextBoxHandlers();

            txtD_2.Text = String.Empty;
            txtE1_2.Text = String.Empty;
            txtE2_2.Text = String.Empty;
            txtE3_2.Text = String.Empty;
            txtE4_2.Text = String.Empty;
            txtE5_2.Text = String.Empty;
            txtE6_2.Text = String.Empty;
            txtE_2.Text = String.Empty;
            txtPenal_2.Text = String.Empty;
            txtTotal_2.Text = String.Empty;
            txtTotalObeOcene.Text = String.Empty;

            enableTextBoxHandlers();
        }

        private void setEnabledDrugaOcena()
        {
            bool drugaOcena = ckbDrugaOcena.Checked;
            txtD_2.Enabled = drugaOcena;
            txtE1_2.Enabled = drugaOcena;
            txtE2_2.Enabled = drugaOcena;
            txtE3_2.Enabled = drugaOcena;
            txtE4_2.Enabled = drugaOcena;
            txtE5_2.Enabled = drugaOcena;
            txtE6_2.Enabled = drugaOcena;
            txtE_2.Enabled = drugaOcena;
            txtPenal_2.Enabled = drugaOcena;
            txtTotal_2.Enabled = drugaOcena;
            txtTotalObeOcene.Enabled = drugaOcena;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            Ocena ocena = (Ocena)entity;

            string formatD = "F" + takmicenje.BrojDecimalaD;
            string formatE1 = "F" + takmicenje.BrojDecimalaE1;
            string formatE = "F" + takmicenje.BrojDecimalaE;
            string formatPen = "F" + takmicenje.BrojDecimalaPen;
            string formatTotal = "F" + takmicenje.BrojDecimalaTotal;

            txtD.Text = String.Empty;
            if (ocena.D != null)
                txtD.Text = ocena.D.Value.ToString(formatD);

            int brojEOcena = takmicenje.BrojEOcena;
            txtE1.Text = String.Empty;
            if (ocena.E1 != null && brojEOcena >= 1)
                txtE1.Text = ocena.E1.Value.ToString(formatE1);

            txtE2.Text = String.Empty;
            if (ocena.E2 != null && brojEOcena >= 2)
                txtE2.Text = ocena.E2.Value.ToString(formatE1);

            txtE3.Text = String.Empty;
            if (ocena.E3 != null && brojEOcena >= 3)
                txtE3.Text = ocena.E3.Value.ToString(formatE1);

            txtE4.Text = String.Empty;
            if (ocena.E4 != null && brojEOcena >= 4)
                txtE4.Text = ocena.E4.Value.ToString(formatE1);

            txtE5.Text = String.Empty;
            if (ocena.E5 != null && brojEOcena >= 5)
                txtE5.Text = ocena.E5.Value.ToString(formatE1);

            txtE6.Text = String.Empty;
            if (ocena.E6 != null && brojEOcena >= 6)
                txtE6.Text = ocena.E6.Value.ToString(formatE1);

            txtE.Text = String.Empty;
            if (ocena.E != null)
                txtE.Text = ocena.E.Value.ToString(formatE);

            txtPenal.Text = String.Empty;
            if (ocena.Penalty != null)
                txtPenal.Text = ocena.Penalty.Value.ToString(formatPen);

            txtTotal.Text = String.Empty;
            if (ocena.Total != null)
                txtTotal.Text = ocena.Total.Value.ToString(formatTotal);

            ckbUnosOcene.Checked = ocena.RucnoUnetaOcena;

            DrugaOcena ocena2 = ocena.Ocena2;
            ckbDrugaOcena.Checked = ocena2 != null;
            if (ckbDrugaOcena.Checked)
            {
                txtD_2.Text = String.Empty;
                if (ocena2.D != null)
                    txtD_2.Text = ocena2.D.Value.ToString(formatD);

                txtE1_2.Text = String.Empty;
                if (ocena2.E1 != null && brojEOcena >= 1)
                    txtE1_2.Text = ocena2.E1.Value.ToString(formatE1);

                txtE2_2.Text = String.Empty;
                if (ocena2.E2 != null && brojEOcena >= 2)
                    txtE2_2.Text = ocena2.E2.Value.ToString(formatE1);

                txtE3_2.Text = String.Empty;
                if (ocena2.E3 != null && brojEOcena >= 3)
                    txtE3_2.Text = ocena2.E3.Value.ToString(formatE1);

                txtE4_2.Text = String.Empty;
                if (ocena2.E4 != null && brojEOcena >= 4)
                    txtE4_2.Text = ocena2.E4.Value.ToString(formatE1);

                txtE5_2.Text = String.Empty;
                if (ocena2.E5 != null && brojEOcena >= 5)
                    txtE5_2.Text = ocena2.E5.Value.ToString(formatE1);

                txtE6_2.Text = String.Empty;
                if (ocena2.E6 != null && brojEOcena >= 6)
                    txtE6_2.Text = ocena2.E6.Value.ToString(formatE1);

                txtE_2.Text = String.Empty;
                if (ocena2.E != null)
                    txtE_2.Text = ocena2.E.Value.ToString(formatE);

                txtPenal_2.Text = String.Empty;
                if (ocena2.Penalty != null)
                    txtPenal_2.Text = ocena2.Penalty.Value.ToString(formatPen);

                txtTotal_2.Text = String.Empty;
                if (ocena2.Total != null)
                    txtTotal_2.Text = ocena2.Total.Value.ToString(formatTotal);

                txtTotalObeOcene.Text = String.Empty;
                if (ocena.TotalObeOcene != null)
                    txtTotalObeOcene.Text = ocena.TotalObeOcene.Value.ToString(formatTotal);
            }
            selectEOcene();
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            // provera da li je neko polje obavezno obavlja se u validate. Ovde se 
            // samo proverava format polja koja nisu prazna

            requiredFieldsAndFormatValidationZaIzracunavanje(notification, txtD, txtE1,
                txtE2, txtE3, txtE4, txtE5, txtE6, txtPenal, String.Empty);
            requiredFieldsAndFormatValidationRucnoUnetaOcena(notification, txtE, txtTotal, String.Empty);

            if (ckbDrugaOcena.Checked)
            {
                requiredFieldsAndFormatValidationZaIzracunavanje(notification, txtD_2,
                    txtE1_2, txtE2_2, txtE3_2, txtE4_2, txtE5_2, txtE6_2, txtPenal_2, "DrugaOcena.");
                requiredFieldsAndFormatValidationRucnoUnetaOcena(notification, txtE_2, txtTotal_2, "DrugaOcena.");
                
                if (txtTotalObeOcene.Text.Trim() != String.Empty)
                {
                    if (!isFloat(txtTotalObeOcene.Text))
                    {
                        notification.RegisterMessage(
                            "TotalObeOcene", "Neispravan format za konacnu ocenu.");
                    }
                    else if (!checkDecimalPlaces(txtTotalObeOcene.Text, takmicenje.BrojDecimalaTotal))
                    {
                        notification.RegisterMessage(
                            "TotalObeOcene", String.Format(
                            "Konacna ocena moze da sadrzi najvise {0} decimala.", takmicenje.BrojDecimalaTotal));
                    }
                }
            }
        }

        private bool isFloat(string s)
        {
            // NOTE: NumberStyles.Float sprecava situaciju da se umesto zareza unese
            // tacka (koja bi se tumacila kao celobrojni separator za grupe)
            NumberStyles numStyles = NumberStyles.Float & ~NumberStyles.AllowExponent;

            float dummy;
            return float.TryParse(s, numStyles, null, out dummy);
        }

        private bool checkDecimalPlaces(string s, int brojDecimala)
        {
            string decSeparator = 
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string[] parts = null;
            if (s.IndexOf(decSeparator) != -1)
                parts = s.Split(new string[1] { decSeparator }, StringSplitOptions.None);

            if (parts != null)
                return parts[1].Trim().Length <= brojDecimala;
            else
                return true;
        }

        private void requiredFieldsAndFormatValidationZaIzracunavanje(
            Notification notification, TextBox txtD, TextBox txtE1, TextBox txtE2,
            TextBox txtE3, TextBox txtE4, TextBox txtE5, TextBox txtE6, 
            TextBox txtPenal, string prefix)
        {
            if (txtD.Text.Trim() != String.Empty)
            {
                if (!isFloat(txtD.Text))
                {
                    notification.RegisterMessage(
                        prefix + "D", "Neispravan format za D ocenu.");
                }
                else if (!checkDecimalPlaces(txtD.Text, takmicenje.BrojDecimalaD))
                {
                    notification.RegisterMessage(
                        prefix + "D", String.Format(
                        "D ocena moze da sadrzi najvise {0} decimala.", takmicenje.BrojDecimalaD));
                }
            }

            TextBox[] txtEOcene = new TextBox[6] { txtE1, txtE2, txtE3, txtE4, txtE5, txtE6 };
            for (byte i = 1; i <= takmicenje.BrojEOcena; i++)
            {
                validateEOcenaFormat(notification, txtEOcene[i - 1], i, prefix);
            }

            if (txtPenal.Text.Trim() != String.Empty)
            {
                if (!isFloat(txtPenal.Text))
                {
                    notification.RegisterMessage(
                        prefix + "Penalty", "Neispravan format za penalizaciju.");
                }
                else if (!checkDecimalPlaces(txtPenal.Text, takmicenje.BrojDecimalaPen))
                {
                    notification.RegisterMessage(
                        prefix + "Penalty", String.Format(
                        "Penalizacija moze da sadrzi najvise {0} decimala.", takmicenje.BrojDecimalaPen));
                }
            }
        }

        private void validateEOcenaFormat(Notification notification, TextBox txtEOcena,
            byte broj, string prefix)
        {
            string propName = String.Format("E{0}", broj);

            if (txtEOcena.Text.Trim() != String.Empty)
            {
                if (!isFloat(txtEOcena.Text))
                {
                    notification.RegisterMessage(
                        prefix + propName, "Neispravan format za " + propName + " ocenu.");
                }
                else if (!checkDecimalPlaces(txtEOcena.Text, takmicenje.BrojDecimalaE1))
                {
                    notification.RegisterMessage(
                        prefix + propName, String.Format(
                        "{0} ocena moze da sadrzi najvise {1} decimala.", propName, takmicenje.BrojDecimalaE1));
                }
            }
        }

        private void requiredFieldsAndFormatValidationRucnoUnetaOcena(
            Notification notification, TextBox txtE, TextBox txtTotal, string prefix)
        {
            if (txtE.Text.Trim() != String.Empty)
            {
                // TODO: Kada se u toku izvrsavanja programa promeni kultura, trebalo 
                // bi promeniti Thread.CurrentThread.CurrentCulture

                if (!isFloat(txtE.Text))
                {
                    notification.RegisterMessage(
                        prefix + "E", "Neispravan format za E ocenu.");
                }
                else if (!checkDecimalPlaces(txtE.Text, takmicenje.BrojDecimalaE))
                {
                    notification.RegisterMessage(
                        prefix + "E", String.Format(
                        "E ocena moze da sadrzi najvise {0} decimala.", takmicenje.BrojDecimalaE));
                }
            }

            if (txtTotal.Text.Trim() != String.Empty)
            {
                if (!isFloat(txtTotal.Text))
                {
                    notification.RegisterMessage(
                        prefix + "Total", "Neispravan format za konacnu ocenu.");
                }
                else if (!checkDecimalPlaces(txtTotal.Text, takmicenje.BrojDecimalaTotal))
                {
                    notification.RegisterMessage(
                        prefix + "Total", String.Format(
                        "Konacna ocena moze da sadrzi najvise {0} decimala.", takmicenje.BrojDecimalaTotal));
                }
            }
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "D":
                    txtD.Focus();
                    break;

                case "E1":
                    txtE1.Focus();
                    break;

                case "E2":
                    txtE2.Focus();
                    break;

                case "E3":
                    txtE3.Focus();
                    break;

                case "E4":
                    txtE4.Focus();
                    break;

                case "E5":
                    txtE5.Focus();
                    break;

                case "E6":
                    txtE6.Focus();
                    break;

                case "E":
                    txtE.Focus();
                    break;

                case "Penalty":
                    txtPenal.Focus();
                    break;

                case "Total":
                    txtTotal.Focus();
                    break;

                case "DrugaOcena.D":
                    txtD_2.Focus();
                    break;

                case "DrugaOcena.E1":
                    txtE1_2.Focus();
                    break;

                case "DrugaOcena.E2":
                    txtE2_2.Focus();
                    break;

                case "DrugaOcena.E3":
                    txtE3_2.Focus();
                    break;

                case "DrugaOcena.E4":
                    txtE4_2.Focus();
                    break;

                case "DrugaOcena.E5":
                    txtE5_2.Focus();
                    break;

                case "DrugaOcena.E6":
                    txtE6_2.Focus();
                    break;

                case "DrugaOcena.E":
                    txtE_2.Focus();
                    break;

                case "DrugaOcena.Penalty":
                    txtPenal_2.Focus();
                    break;

                case "DrugaOcena.Total":
                    txtTotal_2.Focus();
                    break;

                case "TotalObeOcene":
                    txtTotalObeOcene.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            Ocena ocena = (Ocena)entity;
            updateOcenaFromUI_ZaIzracunavanje(ocena);

            if (txtE.Text.Trim() == String.Empty)
                ocena.E = null;
            else
                ocena.E = float.Parse(txtE.Text);

            if (txtTotal.Text.Trim() == String.Empty)
                ocena.Total = null;
            else
                ocena.Total = float.Parse(txtTotal.Text);
            
            DrugaOcena ocena2 = ocena.Ocena2;
            if (ocena2 != null)
            {
                if (txtE_2.Text.Trim() == String.Empty)
                    ocena2.E = null;
                else
                    ocena2.E = float.Parse(txtE_2.Text);

                if (txtTotal_2.Text.Trim() == String.Empty)
                    ocena2.Total = null;
                else
                    ocena2.Total = float.Parse(txtTotal_2.Text);
            }

            if (txtTotalObeOcene.Text.Trim() == String.Empty)
                ocena.TotalObeOcene = null;
            else
                ocena.TotalObeOcene = float.Parse(txtTotalObeOcene.Text);
        }

        private void updateOcenaFromUI_ZaIzracunavanje(Ocena ocena)
        {
            ocena.RucnoUnetaOcena = ckbUnosOcene.Checked;

            if (txtD.Text.Trim() == String.Empty)
                ocena.D = null;
            else
                ocena.D = float.Parse(txtD.Text);

            int brojEOcena = takmicenje.BrojEOcena;

            if (brojEOcena == 0)
            {
                if (txtE.Text.Trim() == String.Empty)
                    ocena.E = null;
                else
                    ocena.E = float.Parse(txtE.Text);
            }

            if (brojEOcena >= 1)
            {
                if (txtE1.Text.Trim() == String.Empty)
                    ocena.E1 = null;
                else
                    ocena.E1 = float.Parse(txtE1.Text);
            }

            if (brojEOcena >= 2)
            {
                if (txtE2.Text.Trim() == String.Empty)
                    ocena.E2 = null;
                else
                    ocena.E2 = float.Parse(txtE2.Text);
            }

            if (brojEOcena >= 3)
            {
                if (txtE3.Text.Trim() == String.Empty)
                    ocena.E3 = null;
                else
                    ocena.E3 = float.Parse(txtE3.Text);
            }

            if (brojEOcena >= 4)
            {
                if (txtE4.Text.Trim() == String.Empty)
                    ocena.E4 = null;
                else
                    ocena.E4 = float.Parse(txtE4.Text);
            }

            if (brojEOcena >= 5)
            {
                if (txtE5.Text.Trim() == String.Empty)
                    ocena.E5 = null;
                else
                    ocena.E5 = float.Parse(txtE5.Text);
            }

            if (brojEOcena >= 6)
            {
                if (txtE6.Text.Trim() == String.Empty)
                    ocena.E6 = null;
                else
                    ocena.E6 = float.Parse(txtE6.Text);
            }

            if (txtPenal.Text.Trim() == String.Empty)
                ocena.Penalty = null;
            else
                ocena.Penalty = float.Parse(txtPenal.Text);


            deletedDrugaOcena = null;
            if (!ckbDrugaOcena.Checked)
            {
                if (ocena.Ocena2 != null)
                    deletedDrugaOcena = ocena.Ocena2;
                ocena.Ocena2 = null;
            }
            else
            {
                if (ocena.Ocena2 == null)
                    ocena.Ocena2 = createDrugaOcena();

                DrugaOcena ocena2 = ocena.Ocena2;
                ocena2.RucnoUnetaOcena = ckbUnosOcene.Checked;

                if (txtD_2.Text.Trim() == String.Empty)
                    ocena2.D = null;
                else
                    ocena2.D = float.Parse(txtD_2.Text);

                if (brojEOcena == 0)
                {
                    if (txtE_2.Text.Trim() == String.Empty)
                        ocena2.E = null;
                    else
                        ocena2.E = float.Parse(txtE_2.Text);
                }
                if (brojEOcena >= 1)
                {
                    if (txtE1_2.Text.Trim() == String.Empty)
                        ocena2.E1 = null;
                    else
                        ocena2.E1 = float.Parse(txtE1_2.Text);
                }

                if (brojEOcena >= 2)
                {
                    if (txtE2_2.Text.Trim() == String.Empty)
                        ocena2.E2 = null;
                    else
                        ocena2.E2 = float.Parse(txtE2_2.Text);
                }

                if (brojEOcena >= 3)
                {
                    if (txtE3_2.Text.Trim() == String.Empty)
                        ocena2.E3 = null;
                    else
                        ocena2.E3 = float.Parse(txtE3_2.Text);
                }

                if (brojEOcena >= 4)
                {
                    if (txtE4_2.Text.Trim() == String.Empty)
                        ocena2.E4 = null;
                    else
                        ocena2.E4 = float.Parse(txtE4_2.Text);
                }

                if (brojEOcena >= 5)
                {
                    if (txtE5_2.Text.Trim() == String.Empty)
                        ocena2.E5 = null;
                    else
                        ocena2.E5 = float.Parse(txtE5_2.Text);
                }

                if (brojEOcena >= 6)
                {
                    if (txtE6_2.Text.Trim() == String.Empty)
                        ocena2.E6 = null;
                    else
                        ocena2.E6 = float.Parse(txtE6_2.Text);
                }

                if (txtPenal_2.Text.Trim() == String.Empty)
                    ocena2.Penalty = null;
                else
                    ocena2.Penalty = float.Parse(txtPenal_2.Text);
            }
        }

        private DrugaOcena createDrugaOcena()
        {
            DrugaOcena result = new DrugaOcena();
            result.BrojEOcena = takmicenje.BrojEOcena;
            return result;
        }

        private void ckbUnosOcene_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbUnosOcene.Checked)
            {
                string msg = "Izabrali ste da se ocena unosi bez izracunavanja. Da li zelite da ovaj izbor vazi " +
                    "za celo takmicenje ili samo za ovu ocenu?\n\nDa - celo takmicenje\nNe - samo ova ocena";
                Opcije.Instance.UnosOcenaBezIzrZaCeloTak = MessageDialogs.queryConfirmation(msg, this.Text);
            }
            else
            {
                Opcije.Instance.UnosOcenaBezIzrZaCeloTak = false;
            }
            
            updateUIRucniUnos();
            clearColors1();
            izracunato = false;
            updateAcceptButton();
            if (!ckbDrugaOcena.Checked)
                clearColors2(disabledColor);
            else
                clearColors2(SystemColors.Window);

            txtE.TabStop = takmicenje.BrojEOcena == 0 || ckbUnosOcene.Checked;
            txtTotal.TabStop = ckbUnosOcene.Checked;
            txtE_2.TabStop = takmicenje.BrojEOcena == 0 || ckbUnosOcene.Checked;
            txtTotal_2.TabStop = ckbUnosOcene.Checked;
            txtTotalObeOcene.TabStop = ckbUnosOcene.Checked;
        }

        private void updateUIRucniUnos()
        {
            bool rucniUnos = ckbUnosOcene.Checked;

            txtE.ReadOnly = !rucniUnos && takmicenje.BrojEOcena > 0;
            txtTotal.ReadOnly = !rucniUnos;
            txtE_2.ReadOnly = !rucniUnos && takmicenje.BrojEOcena > 0;
            txtTotal_2.ReadOnly = !rucniUnos;
            txtTotalObeOcene.ReadOnly = !rucniUnos;
            btnIzracunaj.Enabled = !rucniUnos;
        }

        protected override void validateEntity(DomainObject entity)
        {
            Ocena ocena = (Ocena)entity;
            if (ocena.Ocena2 != null)
                ocena.Ocena2.ValidationPrefix = "DrugaOcena.";
            base.validateEntity(entity);
        }

        private void btnIzracunaj_Click(object sender, EventArgs e)
        {
            try
            {
                Notification notification = new Notification();
                requiredFieldsAndFormatValidationZaIzracunavanje(notification, txtD, txtE1,
                    txtE2, txtE3, txtE4, txtE5, txtE6, txtPenal, String.Empty);
                if (ckbDrugaOcena.Checked)
                {
                    requiredFieldsAndFormatValidationZaIzracunavanje(notification, txtD_2,
                        txtE1_2, txtE2_2, txtE3_2, txtE4_2, txtE5_2, txtE6_2, txtPenal_2, "DrugaOcena.");
                }
                
                if (!notification.IsValid())
                    throw new BusinessException(notification);

                Ocena o = (Ocena)entity;
                updateOcenaFromUI_ZaIzracunavanje(o);

                // validate
                notification = new Notification();
                if (o.Ocena2 != null)
                    o.Ocena2.ValidationPrefix = "DrugaOcena.";
                o.validateZaIzracunavanje(notification);
                if (!notification.IsValid())
                    throw new BusinessException(notification);
                           
                // TODO: Razmisli sta treba da radis kada se u toku takmicenja
                // (kada su neke ocene vec unete) promene opcije za broj decimala

                o.izracunajOcenu(takmicenje.BrojDecimalaE,
                    takmicenje.BrojDecimalaPen, takmicenje.BrojDecimalaTotal);

                izracunato = true;
                updateAcceptButton();
                btnOk.Focus();
                selectEOcene();

                disableTextBoxHandlers();
                updateUIFromEntity(o);
                enableTextBoxHandlers();
            }
            catch (BusinessException ex)
            {
                if (ex.Notification != null)
                {
                    NotificationMessage msg = ex.Notification.FirstMessage;
                    MessageDialogs.showMessage(msg.Message, this.Text);
                    setFocus(msg.FieldName);
                }
                else if (!string.IsNullOrEmpty(ex.InvalidProperty))
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                    setFocus(ex.InvalidProperty);
                }
                else
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                }
            }
            catch (Exception ex)
            {
                MessageDialogs.showMessage(ex.Message, this.Text);
            }
            finally
            {

            }
        }

        private void selectEOcene()
        {
            if (ckbUnosOcene.Checked)
                return;

            Ocena o = (Ocena)entity;
            selectEOcene1(o.getMinEOcenaBroj(), o.getMaxEOcenaBroj());
            if (obeOcene && ckbDrugaOcena.Checked)
                selectEOcene2(o.Ocena2.getMinEOcenaBroj(), o.Ocena2.getMaxEOcenaBroj());
        }

        private void selectEOcene1(int minBroj, int maxBroj)
        {
            doSelectEOcene(new TextBox[] { txtE1, txtE2, txtE3, txtE4, txtE5, txtE6 },
                minBroj, maxBroj);
        }

        private void selectEOcene2(int minBroj, int maxBroj)
        {
            doSelectEOcene(new TextBox[] { txtE1_2, txtE2_2, txtE3_2, txtE4_2, txtE5_2, txtE6_2 },
                minBroj, maxBroj);
        }

        private void doSelectEOcene(TextBox[] txtBoxes, int minBroj, int maxBroj)
        {
            for (int i = 0; i < takmicenje.BrojEOcena; i++)
            {
                if (i != minBroj - 1 && i != maxBroj - 1)
                    txtBoxes[i].BackColor = selectionColor;
                else
                    txtBoxes[i].BackColor = SystemColors.Window;
            }
        }

        private void OcenaForm_Load(object sender, EventArgs e)
        {
            this.ckbUnosOcene.CheckedChanged += new System.EventHandler(this.ckbUnosOcene_CheckedChanged);
            addTextBoxHandlers();
        }

        private void addTextBoxHandlers()
        {
            txtD.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
            txtD_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);

            int brojEOcena = takmicenje.BrojEOcena;

            if (brojEOcena == 0)
            {
                txtE.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
                txtE_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);
            }

            if (brojEOcena >= 1)
            {
                txtE1.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
                txtE1_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);
            }
            if (brojEOcena >= 2)
            {
                txtE2.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
                txtE2_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);
            }
            if (brojEOcena >= 3)
            {
                txtE3.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
                txtE3_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);
            }
            if (brojEOcena >= 4)
            {
                txtE4.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
                txtE4_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);
            }
            if (brojEOcena >= 5)
            {
                txtE5.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
                txtE5_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);
            }
            if (brojEOcena >= 6)
            {
                txtE6.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
                txtE6_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);
            }

            txtPenal.TextChanged += new EventHandler(txtBoxOcena1_TextChanged);
            txtPenal_2.TextChanged += new EventHandler(txtBoxOcena2_TextChanged);
        }

        private void enableTextBoxHandlers()
        {
            textBoxHandlersDisabled = false;
        }

        private void disableTextBoxHandlers()
        {
            // NOTE, TODO: Kada treba da se onemoguce i kasnije ponovo omoguce 
            // handleri, ne koristi se uklanjanje i ponovo dodavanje handlera 
            // (naredbe txtD.TextChanged -= txtBoxOcena1_TextChanged i 
            // kasnije txtD.TextChanged += txtBoxOcena1_TextChanged) zato sto moze da
            // dodje do greske prilikom ugnjezdjivanja. Npr. neka imamo sledeci
            // niz naredbi

            // disableTextBoxHandlers();
            // .
            // .
            // disableTextBoxHandlers();
            // .
            // .
            // .
            // enableTextBoxHandlers();
            // .
            // .
            // enableTextBoxHandlers();

            // Tada ce dve disableTextBoxHandlers samo jedanput ukloniti handler (jer
            // postoji samo jedan handler), dok ce dve enableTextBoxHandlers dodati 
            // handler dva puta. Ako nakon toga imamo sledece naredbe

            // disableTextBoxHandlers();
            // .
            // .
            // .
            // enableTextBoxHandlers();

            // Tada naredba disableTextBoxHandlers nece onemoguciti handler, tj.
            // uklonice samo jedan handler dok ce drugi ostati.

            // Probaj da ovako uradis i na ostalim mestima na kojima se onemogucuju
            // i ponovo omogucuju handleri. Na pocetku hendlera treba da se 
            // ispituje promenljiva textBoxHandlersDisabled

            textBoxHandlersDisabled = true;
        }

        void txtBoxOcena1_TextChanged(object sender, EventArgs e)
        {
            if (textBoxHandlersDisabled)
                return;
            if (!ckbUnosOcene.Checked)
            {
                txtTotal.Text = String.Empty;
                txtTotalObeOcene.Text = String.Empty;

                TextBox txt = sender as TextBox;
                if (isTxtEOcena1(txt))
                {
                    txtE.Text = String.Empty;
                }
                clearColors1();
                izracunato = false;
                updateAcceptButton();
            }
        }

        private void clearColors1()
        {
            TextBox[] txtBoxes = { txtE1, txtE2, txtE3, txtE4, txtE5, txtE6 };
            foreach (TextBox txtBox in txtBoxes)
                txtBox.BackColor = SystemColors.Window;
        }

        void txtBoxOcena2_TextChanged(object sender, EventArgs e)
        {
            if (textBoxHandlersDisabled)
                return;
            if (!ckbUnosOcene.Checked)
            {
                txtTotal_2.Text = String.Empty;
                txtTotalObeOcene.Text = String.Empty;

                TextBox txt = sender as TextBox;
                if (isTxtEOcena2(txt))
                {
                    txtE_2.Text = String.Empty;
                }
                clearColors2(SystemColors.Window);
                izracunato = false;
                updateAcceptButton();
            }
        }

        private void clearColors2(Color color)
        {
            TextBox[] txtBoxes = { txtE1_2, txtE2_2, txtE3_2, txtE4_2, txtE5_2, txtE6_2 };
            foreach (TextBox txtBox in txtBoxes)
                txtBox.BackColor = color;
        }

        private bool isTxtEOcena1(TextBox txt)
        {
            return object.ReferenceEquals(txt, txtE1)
            || object.ReferenceEquals(txt, txtE2)
            || object.ReferenceEquals(txt, txtE3)
            || object.ReferenceEquals(txt, txtE4)
            || object.ReferenceEquals(txt, txtE5)
            || object.ReferenceEquals(txt, txtE6);
        }

        private bool isTxtEOcena2(TextBox txt)
        {
            return object.ReferenceEquals(txt, txtE1_2)
            || object.ReferenceEquals(txt, txtE2_2)
            || object.ReferenceEquals(txt, txtE3_2)
            || object.ReferenceEquals(txt, txtE4_2)
            || object.ReferenceEquals(txt, txtE5_2)
            || object.ReferenceEquals(txt, txtE6_2);
        }

        private void ckbDrugaOcena_CheckedChanged(object sender, EventArgs e)
        {
            setEnabledDrugaOcena();
            if (!ckbDrugaOcena.Checked)
            {
                ponistiOcena2();
                clearColors2(disabledColor);
            }
            else
            {
                clearColors2(SystemColors.Window);
                izracunato = false;
                updateAcceptButton();
            }
        }

        private void btnPonisti_Click(object sender, EventArgs e)
        {
            ponistiOcena1();
            ponistiOcena2();
            izracunato = false;
            updateAcceptButton();
            btnIzracunaj.Focus();  // mora i ova naredba, zato sto naredba this.AcceptButton = btnIzracunaj koja
                    // se izvrsava u updateAcceptButton nema efekta ako je btnPonisti_Click pozvan kao rezultat
            // klika misem (tada ce fokus ostati na btnPonisti). Ako je btnPonisti_Click pozvan kao rezultat
            // pritiska na Enter (pri cemu je btnPonisti bilo u fokusu), tada ce se fokus prebaciti na 
            // btnIzracunaj i bez ove naredbe.
        }

        protected override void handleOkClick()
        {
            if (!ckbUnosOcene.Checked && !izracunato)
            {
                MessageDialogs.showMessage("Najpre izracunajte ocenu.", this.Text);
                DialogResult = DialogResult.None;
            }
            else
            {
                base.handleOkClick();
            }
        }

        protected override void addEntity(DomainObject entity)
        {
            Ocena o = (Ocena)entity;
            dataContext.Add(o);

            IList<RezultatskoTakmicenje> rezTakmicenja = loadRezTakmicenja(o.Gimnasticar);
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    rezTak.Takmicenje1.ocenaAdded(o, rezTak);
                    dataContext.Save(rezTak.Takmicenje1);
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                {
                    if (rezTak.Propozicije.OdvojenoTak2)
                    {
                        rezTak.Takmicenje2.ocenaAdded(o, rezTak);
                        dataContext.Save(rezTak.Takmicenje2);
                    }
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                {
                    if (rezTak.Propozicije.OdvojenoTak3)
                    {
                        rezTak.Takmicenje3.ocenaAdded(o, rezTak);
                        dataContext.Save(rezTak.Takmicenje3);
                    }
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje4)
                {
                    if (rezTak.Propozicije.OdvojenoTak4)
                    {
                        rezTak.Takmicenje4.ocenaAdded(o, rezTak);
                        dataContext.Save(rezTak.Takmicenje4);
                    }
                }
            }

            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    foreach (GimnasticarUcesnik g in rezTak.Takmicenje1.Gimnasticari)
                        dataContext.Evict(g);
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                {
                    foreach (UcesnikTakmicenja2 u in rezTak.Takmicenje2.Ucesnici)
                    {
                        if (dataContext.Contains(u.Gimnasticar))
                            dataContext.Evict(u.Gimnasticar);
                        dataContext.Evict(u);
                    }
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                {
                    foreach (UcesnikTakmicenja3 u in rezTak.Takmicenje3.Ucesnici)
                    {
                        if (dataContext.Contains(u.Gimnasticar))
                            dataContext.Evict(u.Gimnasticar);
                        dataContext.Evict(u);
                    }
                }
            }
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(GimnasticarUcesnik g)
        {
            return dataContext.ExecuteNamedQuery<RezultatskoTakmicenje>(
                "FindRezTakmicenjaForGimnasticar",
                new string[] { "gimnasticar" },
                new object[] { g });
        }

        protected override void updateEntity(DomainObject entity)
        {
            Ocena o = (Ocena)entity;
            if (deletedDrugaOcena != null)
                dataContext.Delete(deletedDrugaOcena);
            dataContext.Save(o);

            IList<RezultatskoTakmicenje> rezTakmicenja = loadRezTakmicenja(o.Gimnasticar);
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    rezTak.Takmicenje1.ocenaEdited(o, original, rezTak);
                    dataContext.Save(rezTak.Takmicenje1);
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                {
                    if (rezTak.Propozicije.OdvojenoTak2)
                    {
                        rezTak.Takmicenje2.ocenaEdited(o, original, rezTak);
                        dataContext.Save(rezTak.Takmicenje2);
                    }
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                {
                    if (rezTak.Propozicije.OdvojenoTak3)
                    {
                        rezTak.Takmicenje3.ocenaEdited(o, rezTak);
                        dataContext.Save(rezTak.Takmicenje3);
                    }
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje4)
                {
                    if (rezTak.Propozicije.OdvojenoTak4)
                    {
                        rezTak.Takmicenje4.ocenaEdited(o, original, rezTak);
                        dataContext.Save(rezTak.Takmicenje4);
                    }
                }
            }

            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    foreach (GimnasticarUcesnik g in rezTak.Takmicenje1.Gimnasticari)
                    {
                        // kada se gimnasticar takmici u vise takmicenja, kada ga
                        // izbacim (Evict) iz prvog takmicenja, prijavljuje mi gresku
                        // kada pokusam da ga izbacim i iz ostalih takmicenja. Zato
                        // sam dodao ovu proveru
                        if (dataContext.Contains(g))
                            dataContext.Evict(g);
                    }
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                {
                    foreach (UcesnikTakmicenja2 u in rezTak.Takmicenje2.Ucesnici)
                    {
                        if (dataContext.Contains(u.Gimnasticar))
                            dataContext.Evict(u.Gimnasticar);
                        dataContext.Evict(u);
                    }
                }
                else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                {
                    foreach (UcesnikTakmicenja3 u in rezTak.Takmicenje3.Ucesnici)
                    {
                        if (dataContext.Contains(u.Gimnasticar))
                            dataContext.Evict(u.Gimnasticar);
                        dataContext.Evict(u);
                    }
                }
            }
        }

        private void OcenaForm_Shown(object sender, EventArgs e)
        {
            updateAcceptButton();
            if (!editMode)
                txtD.Focus();
            else
                btnCancel.Focus();
        }

        private void updateAcceptButton()
        {
            if (izracunato)
                this.AcceptButton = btnOk;
            else
                this.AcceptButton = btnIzracunaj;
        }
    }
}