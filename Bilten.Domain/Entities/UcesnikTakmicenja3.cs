using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class UcesnikTakmicenja3 : UcesnikFinala
    {
        private GimnasticarUcesnik _gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return _gimnasticar; }
            set { _gimnasticar = value; }
        }

        private Sprava _sprava;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            set { _sprava = value; }
        }
    
        public virtual string PrezimeIme
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.PrezimeIme;
                else
                    return String.Empty;
            }
        }

        public virtual string KlubDrzava
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.KlubDrzava;
                else
                    return String.Empty;
            }
        }

        public UcesnikTakmicenja3()
        { 
        
        }
   
        public UcesnikTakmicenja3(GimnasticarUcesnik gimnasticar, Sprava sprava,
            Nullable<short> qualOrder, Nullable<float> qualScore,
            Nullable<short> qualRank, KvalifikacioniStatus kvalStatus) 
            : base(qualOrder, qualScore, qualRank, kvalStatus)
        {
            _gimnasticar = gimnasticar;
            _sprava = sprava;
        }

        // NOTE: Ovo je zakomentarisano jer je pravilo probleme kod promene kvalifikanata u takmicenju 3.
        // Trenutno, kada se doda nov kvalifikant u takmicenju 3 koji je bio rezerva, on i dalje ostaje rezerva
        // (tj. nalazi se i na listi za kvalifikante i na listi za rezerve). Dakle, u Takmicenje3.Ucesnici isti gimnasticar
        // moze da se pojavljuje dva puta.
        /*public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is UcesnikTakmicenja3)) return false;

            UcesnikTakmicenja3 that = (UcesnikTakmicenja3)other;
            return this.Gimnasticar.Equals(that.Gimnasticar) && this.Sprava == that.Sprava;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Gimnasticar.GetHashCode();
                result = 29 * result + Sprava.GetHashCode();
                return result;
            }
        }*/

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);
            strBuilder.AppendLine(Sprava.ToString());
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());
        }
    }
}
