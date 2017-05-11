using Bilten.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class TakmicarskaKategorija : DomainObject
    {
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

        private Takmicenje takmicenje;
        public virtual Takmicenje Takmicenje
        {
            get { return takmicenje; }
            protected set { takmicenje = value; }
        }

        // NOTE: Metod setTakmicenjeInternal nije za upotrebu vec se koristi samo kao 
        // pomoc prilikom uspostavlja dvostruke asocijacije izmedju Takmicenja i 
        // TakmicarskeKategorije (set aksesor za Takmicenje sam proglasio privatnim da 
        // ga ne bi greskom koristio za uspostavljanje asocijacije). Ako je potrebno
        // da i Takmicenje ima metod za uspostavljanje asocijacije on bi izgledao
        // ovako
        //      public void setTakmicenje(Takmicenje t)
        //      {
        //          t.addKategorija(this);
        //      }
        public virtual void setTakmicenjeInternal(Takmicenje t)
        {
            Takmicenje = t;
        }

        public TakmicarskaKategorija()
        { 
        
        }

        public TakmicarskaKategorija(string naziv)
        {
            this.naziv = naziv;
        }

        public override void validate(Notification notification)
        {
            if (String.IsNullOrEmpty(Naziv))
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv kategorije je obavezan.");
            }
        }

        public override string ToString()
        {
            return Naziv;
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is TakmicarskaKategorija)) return false;
            TakmicarskaKategorija that = (TakmicarskaKategorija)other;
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

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Naziv != null ? Naziv : NULL);
            strBuilder.AppendLine(RedBroj.ToString());
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            string naziv = reader.ReadLine();
            Naziv = naziv != NULL ? naziv : null;
            RedBroj = byte.Parse(reader.ReadLine());

            string line = reader.ReadLine();
            Takmicenje = line != NULL ? map.takmicenjeMap[int.Parse(line)] : null;
        }
    }
}
