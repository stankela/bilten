using Bilten.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatskoTakmicenjeDescription : DomainObject
    {
        private static readonly int NAZIV_MAX_LENGTH = 128;
        
        private string naziv;
        public virtual string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        private byte redBroj;
        public virtual byte RedBroj
        {
            get { return redBroj; }
            set { redBroj = value; }
        }

        private Propozicije propozicije;
        public virtual Propozicije Propozicije
        {
            get { return propozicije; }
            set { propozicije = value; }
        }

        public RezultatskoTakmicenjeDescription()
        { 
        
        }

        public override string ToString()
        {
            return Naziv;
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is RezultatskoTakmicenjeDescription)) return false;
            RezultatskoTakmicenjeDescription that = (RezultatskoTakmicenjeDescription)other;
            return this.Naziv.ToUpper() == that.Naziv.ToUpper();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Naziv.GetHashCode();
                return result;
            }
        }

        public override void validate(Notification notification)
        {
            // validate Naziv
            if (string.IsNullOrEmpty(Naziv))
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv takmicenja je obavezan.");
            }
            else if (Naziv.Length > NAZIV_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv takmicenja moze da sadrzi maksimalno "
                    + NAZIV_MAX_LENGTH + " znakova.");
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Naziv != null ? Naziv : NULL);
            strBuilder.AppendLine(RedBroj.ToString());
            if (Propozicije == null)
                strBuilder.AppendLine(NULL);
            else
                Propozicije.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader)
        {
            string naziv = reader.ReadLine();
            Naziv = naziv != NULL ? naziv : null;
            RedBroj = byte.Parse(reader.ReadLine());

            string id = reader.ReadLine();
            Propozicije p = null;
            if (id != NULL)
            {
                p = new Propozicije();
                p.loadFromDump(reader);
            }
            Propozicije = p;
        }
    }
}
