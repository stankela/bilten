using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class SablonRasporedaNastupaTakm1Item : DomainObject
    {
        public SablonRasporedaNastupaTakm1Item()
        {

        }

        public SablonRasporedaNastupaTakm1Item(Sprava sprava, byte redBroj,
            Ekipa ekipa, byte brojUcesnika)
        {
            this.sprava = sprava;
            this.redBroj = redBroj;
            this.ekipa = ekipa;
            this.brojUcesnika = brojUcesnika;
        }

        private byte redBroj;
        public virtual byte RedBroj
        {
            get { return redBroj; }
            set { redBroj = value; }
        }

        private byte brojUcesnika;
        public virtual byte BrojUcesnika
        {
            get { return brojUcesnika; }
            set { brojUcesnika = value; }
        }

        private Sprava sprava;
        public virtual Sprava Sprava
        {
            get { return sprava; }
            set { sprava = value; }
        }

        private Ekipa ekipa;
        public virtual Ekipa Ekipa
        {
            get { return ekipa; }
            set { ekipa = value; }
        }

        public virtual string KodBrojUcesnika
        {
            get
            {
                return string.Format("{0} ({1})", Ekipa.Kod, BrojUcesnika);
            }
        }

        public override void validate(Notification notification)
        {
            if (Sprava == Sprava.Undefined)
            {
                notification.RegisterMessage("Sprava", "Sprava je obavezna.");
            }

            if (Ekipa == null)
            {
                notification.RegisterMessage("Ekipa", "Ekipa je obavezna.");
            }

            if (BrojUcesnika <= 0)
            {
                notification.RegisterMessage(
                    "BrojUcesnika", "Broj ucesnika mora da bude veci od 0.");
            }
        }

        public override bool Equals(object other)
        {
            // NOTE: Uklonio sam RedBroj zato sto se RedBroj menja dok je item u setu
            // (kada se iz sablona izbaci item, RedBroj ostalih itema se prenumerise)

            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is SablonRasporedaNastupaTakm1Item)) return false;
            SablonRasporedaNastupaTakm1Item that = (SablonRasporedaNastupaTakm1Item)other;
            return this.Ekipa.Id == that.Ekipa.Id
                && this.Sprava == that.Sprava
   //             && this.RedBroj == that.RedBroj
                && this.BrojUcesnika == that.BrojUcesnika;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Ekipa.GetHashCode();
                result = 29 * result + Sprava.GetHashCode();
     //           result = 29 * result + RedBroj.GetHashCode();
                result = 29 * result + BrojUcesnika.GetHashCode();
                return result;
            }
        }
    }
}
