using System;
using System.Collections.Generic;
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
    }
}
