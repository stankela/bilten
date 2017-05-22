using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class UcesnikTakmicenja2 : UcesnikFinala
    {
        private GimnasticarUcesnik _gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return _gimnasticar; }
            set { _gimnasticar = value; }
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

        public UcesnikTakmicenja2()
        { 
        
        }

        public UcesnikTakmicenja2(GimnasticarUcesnik gimnasticar, Nullable<short> qualOrder,
            Nullable<float> qualScore, Nullable<short> qualRank, KvalifikacioniStatus kvalStatus) 
            : base(qualOrder, qualScore, qualRank, kvalStatus)
        {
            _gimnasticar = gimnasticar;
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is UcesnikTakmicenja2)) return false;

            UcesnikTakmicenja2 that = (UcesnikTakmicenja2)other;
            return this.Gimnasticar.Equals(that.Gimnasticar);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Gimnasticar.GetHashCode();
                return result;
            }
        }

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;
        }
    }
}
