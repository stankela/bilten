using System;
using System.Collections.Generic;
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

        protected UcesnikTakmicenja4()
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
    }
}
