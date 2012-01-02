using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data.QueryModel;
using Bilten.Exceptions;

namespace Bilten.UI
{
    public partial class SablonRasporedaNastupaTakm1ItemForm : EntityDetailForm
    {
        private List<Ekipa> ekipe;
        private SablonRasporedaNastupaTakm1 sablon;

        public SablonRasporedaNastupaTakm1ItemForm(SablonRasporedaNastupaTakm1 sablon)
        {
            InitializeComponent();
            this.sablon = sablon;

            // radi samo u add modu
            initialize(null, false);
        }

        protected override DomainObject createNewEntity()
        {
            SablonRasporedaNastupaTakm1Item result = new SablonRasporedaNastupaTakm1Item();
            return result;
        }

        protected override void loadData()
        {
            ekipe = new List<Ekipa>();
            foreach (TakmicarskaKategorija kat in sablon.RasporedNastupa.Kategorije)
            {
                ekipe.AddRange(loadEkipe(kat));
            }
        }

        private List<Ekipa> loadEkipe(TakmicarskaKategorija kategorija)
        {
            Query q = new Query();
            q.Criteria.Add(
                new Criterion("TakmicarskaKategorija", CriteriaOperator.Equal, kategorija));
            q.OrderClauses.Add(new OrderClause("Kod", OrderClause.OrderClauseCriteria.Ascending));
            return new List<Ekipa>(dataContext.GetByCriteria<Ekipa>(q));
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Sablon rasporeda nastupa";

            txtBrojUcesnika.Text = String.Empty;

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            setSprave(sablon.RasporedNastupa.Pol);
            cmbSprava.SelectedIndex = 0;

            cmbEkipa.DropDownStyle = ComboBoxStyle.DropDownList;
            setEkipe(ekipe);
            SelectedEkipa = null;
        }

        private void setSprave(Pol pol)
        {
            string[] sprave = Sprave.getSpraveNazivi(pol);
            cmbSprava.Items.AddRange(sprave);
        }

        private Sprava SelectedSprava
        {
            get { return Sprave.parse(cmbSprava.SelectedItem.ToString()); }
            set { cmbSprava.SelectedItem = Sprave.toString(value); }
        }

        private void setEkipe(List<Ekipa> ekipe)
        {
            cmbEkipa.DisplayMember = "Naziv";
            cmbEkipa.DataSource = ekipe;
        }

        private Ekipa SelectedEkipa
        {
            get { return cmbEkipa.SelectedItem as Ekipa; }
            set { cmbEkipa.SelectedItem = value; }
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            byte dummy;
            if (cmbSprava.SelectedIndex == -1)
            {
                notification.RegisterMessage(
                    "Sprava", "Sprava je obavezna.");
            }
            if (cmbEkipa.SelectedIndex == -1)
            {
                notification.RegisterMessage(
                    "Ekipa", "Ekipa je obavezna.");
            }
            if (txtBrojUcesnika.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojUcesnika", "Broj ucesnika je obavezan.");
            }
            else if (!byte.TryParse(txtBrojUcesnika.Text, out dummy))
            {
                notification.RegisterMessage(
                    "BrojUcesnika", "Neispravan format za broj ucesnika.");
            }
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Sprava":
                    cmbSprava.Focus();
                    break;

                case "Ekipa":
                    cmbEkipa.Focus();
                    break;

                case "BrojUcesnika":
                    txtBrojUcesnika.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            SablonRasporedaNastupaTakm1Item item = 
                (SablonRasporedaNastupaTakm1Item)entity;

            item.Sprava = SelectedSprava;
            item.Ekipa = SelectedEkipa;
            item.BrojUcesnika = byte.Parse(txtBrojUcesnika.Text);
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            SablonRasporedaNastupaTakm1Item item =
                (SablonRasporedaNastupaTakm1Item)entity;
            Notification notification = new Notification();

            if (sablon.existsEkipa(item.Ekipa))
            {
                notification.RegisterMessage("Ekipa",
                    "Data ekipa vec postoji u sablonu.");
                throw new BusinessException(notification);
            }

            // TODO: Trebalo bi proveriti ekipu i u sablonima za ostale rasporede
            // nastupa
        }

    }
}