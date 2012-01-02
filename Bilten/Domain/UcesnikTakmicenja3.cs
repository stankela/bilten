using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class UcesnikTakmicenja3 : UcesnikFinala
    {
        private Sprava _sprava;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            set { _sprava = value; }
        }
    
        private GimnasticarUcesnik _gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return _gimnasticar; }
            set { _gimnasticar = value; }
        }

        public virtual Nullable<int> TakmicarskiBroj
        {
            get
            {
                if (Gimnasticar != null && Gimnasticar.TakmicarskiBroj.HasValue)
                    return Gimnasticar.TakmicarskiBroj;
                else
                    return null;
            }
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

        protected UcesnikTakmicenja3()
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
    }
}
