using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Exceptions;
using Bilten.Domain;
using Bilten.Data.QueryModel;
using Bilten.Data;

namespace Bilten.UI
{
    public partial class GimnasticarUcesnikForm : EntityDetailForm
    {
        private Nullable<int> oldTakBroj;
        private TakmicarskaKategorija kategorija;
        private IList<RezultatskoTakmicenje> rezTakmicenja;

        // TODO: Dodaj mogucnost promene kategorije (samo bi se promenila kategorija dok bi sve drugo ostalo
        // isto - gimnasticar bi i dalje bio u istim takmicenjima, ekipama, imao bi iste ocene. Prvo proveri da
        // li je ovo uopste moguce, tj. da li nece da dovede do greske u nekom drugom delu programa).

        public GimnasticarUcesnikForm(Nullable<int> gimnasticarUcesnikId,
            TakmicarskaKategorija kategorija)
        {
            if (gimnasticarUcesnikId == null)
                throw new ArgumentException("GimnasticarUcesnikForm only works in edit mode.");
            InitializeComponent();
            this.kategorija = kategorija;
            initialize(gimnasticarUcesnikId, true);
        }

        protected override void loadData()
        {
            rezTakmicenja = loadRezTakmicenja((GimnasticarUcesnik)entity);
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(GimnasticarUcesnik g)
        {
            return dataContext.ExecuteNamedQuery<RezultatskoTakmicenje>(
                "FindRezTakmicenjaForGimnasticar",
                new string[] { "gimnasticar" },
                new object[] { g });
        }

        protected override void initUI()
        {
            base.initUI();
            Text = "Prijavljeni gimnasticar";

            txtIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            txtGimnastika.Text = String.Empty;
            txtDatumRodj.Text = String.Empty;
            txtRegBroj.Text = String.Empty;
            txtKlub.Text = String.Empty;
            txtDrzava.Text = String.Empty;

            txtTakKategorija.Text = kategorija.ToString();
            txtTakBroj.Text = String.Empty;

            lstTakmicenja.HorizontalScrollbar = true;
            lstTakmicenja.Items.Clear();

            rbtKlub.Checked = true;
        }

        protected override DomainObject getEntityById(int id)
        {
            return dataContext.GetById<GimnasticarUcesnik>(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            oldTakBroj = gimnasticar.TakmicarskiBroj;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            txtIme.Text = gimnasticar.Ime;
            txtPrezime.Text = gimnasticar.Prezime;
            txtGimnastika.Text = gimnasticar.Gimnastika.ToString();

            txtDatumRodj.Text = String.Empty;
            if (gimnasticar.DatumRodjenja != null)
                txtDatumRodj.Text = gimnasticar.DatumRodjenja.ToString("d");

            txtRegBroj.Text = String.Empty;
            if (gimnasticar.RegistarskiBroj != null)
                txtRegBroj.Text = gimnasticar.RegistarskiBroj.ToString();

            txtKlub.Text = String.Empty;
            if (gimnasticar.KlubUcesnik != null)
                txtKlub.Text = gimnasticar.KlubUcesnik.Naziv;

            txtDrzava.Text = String.Empty;
            if (gimnasticar.DrzavaUcesnik != null)
                txtDrzava.Text = gimnasticar.DrzavaUcesnik.Naziv;

            txtTakKategorija.Text = kategorija.ToString();

            txtTakBroj.Text = String.Empty;
            if (gimnasticar.TakmicarskiBroj != null)
                txtTakBroj.Text = gimnasticar.TakmicarskiBroj.ToString();

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                lstTakmicenja.Items.Add(rt.Naziv);

            rbtDrzava.Checked = gimnasticar.NastupaZaDrzavu;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            int dummyInt;
            if (txtTakBroj.Text.Trim() != String.Empty &&
            !int.TryParse(txtTakBroj.Text, out dummyInt))
            {
                notification.RegisterMessage(
                    "TakmicarskiBroj", "Neispravan format za takmicarski broj.");
            }
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "TakmicarskiBroj":
                    txtTakBroj.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            if (txtTakBroj.Text.Trim() == String.Empty)
                gimnasticar.TakmicarskiBroj = null;
            else
                gimnasticar.TakmicarskiBroj = int.Parse(txtTakBroj.Text);

            gimnasticar.NastupaZaDrzavu = rbtDrzava.Checked;
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            Notification notification = new Notification();

            bool takBrojChanged =
                (gimnasticar.TakmicarskiBroj != oldTakBroj) ? true : false;
            if (takBrojChanged && gimnasticar.TakmicarskiBroj != null
            && existsGimnasticarTakBroj(gimnasticar.TakmicarskiBroj.Value))
            {
                notification.RegisterMessage("TakmicarskiBroj",
                    "Gimnasticar sa datim takmicarskim brojem vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool existsGimnasticarTakBroj(int takBroj)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("TakmicarskiBroj", CriteriaOperator.Equal, takBroj));
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, kategorija.Takmicenje));
            q.Operator = QueryOperator.And;
            return dataContext.GetCount<GimnasticarUcesnik>(q) > 0;
        }

    }
}