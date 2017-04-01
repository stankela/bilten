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
