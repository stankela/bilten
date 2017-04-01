using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class UcesnikFinala : DomainObject
    {
        private Nullable<short> _qualOrder;
        public virtual Nullable<short> QualOrder
        {
            get { return _qualOrder; }
            set { _qualOrder = value; }
        }

        private Nullable<float> _qualScore;
        public virtual Nullable<float> QualScore
        {
            get { return _qualScore; }
            protected set { _qualScore = value; }
        }

        private Nullable<short> _qualRank;
        public virtual Nullable<short> QualRank
        {
            get { return _qualRank; }
            set { _qualRank = value; }
        }

        private KvalifikacioniStatus _kvalStatus;
        public virtual KvalifikacioniStatus KvalStatus
        {
            get { return _kvalStatus; }
            set { _kvalStatus = value; }
        }

        protected UcesnikFinala()
        { 
        
        }

        protected UcesnikFinala(Nullable<short> qualOrder, Nullable<float> qualScore,
            Nullable<short> qualRank, KvalifikacioniStatus kvalStatus)
        {
            _qualOrder = qualOrder;
            _qualScore = qualScore;
            _qualRank = qualRank;
            _kvalStatus = kvalStatus;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(QualOrder != null ? QualOrder.Value.ToString() : NULL);
            strBuilder.AppendLine(QualScore != null ? QualScore.Value.ToString() : NULL);
            strBuilder.AppendLine(QualRank != null ? QualRank.Value.ToString() : NULL);
            strBuilder.AppendLine(KvalStatus.ToString());
        }

        public virtual void loadFromDump(StringReader reader)
        {
            string line = reader.ReadLine();
            QualOrder = line != NULL ? short.Parse(line) : (short?)null;

            line = reader.ReadLine();
            QualScore = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            QualRank = line != NULL ? short.Parse(line) : (short?)null;

            KvalStatus = (KvalifikacioniStatus)Enum.Parse(typeof(KvalifikacioniStatus), reader.ReadLine());
        }
    }
}
