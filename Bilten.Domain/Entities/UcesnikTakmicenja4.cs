using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class UcesnikTakmicenja4 : UcesnikFinala
    {
        private Ekipa _ekipa;
        public virtual Ekipa Ekipa
        {
            get { return _ekipa; }
            set { _ekipa = value; }
        }

        public UcesnikTakmicenja4()
        { 
        
        }

        public UcesnikTakmicenja4(Ekipa ekipa, Nullable<short> qualOrder,
            Nullable<float> qualScore, Nullable<short> qualRank, KvalifikacioniStatus kvalStatus)
            : base(qualOrder, qualScore, qualRank, kvalStatus)
        {
            _ekipa = ekipa;
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is UcesnikTakmicenja4)) return false;

            UcesnikTakmicenja4 that = (UcesnikTakmicenja4)other;
            return this.Ekipa.Equals(that.Ekipa);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Ekipa.GetHashCode();
                return result;
            }
        }
        
        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Ekipa != null ? Ekipa.Id.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Ekipa = line != NULL ? map.ekipeMap[int.Parse(line)] : null;
        }
    }
}
