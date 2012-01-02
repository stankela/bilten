using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatSprava : Rezultat
    {
        private Nullable<float> d;
        public virtual Nullable<float> D
        {
            get { return d; }
            protected set { d = value; }
        }

        private Nullable<float> e;
        public virtual Nullable<float> E
        {
            get { return e; }
            protected set { e = value; }
        }

        private Nullable<float> penalty;
        public virtual Nullable<float> Penalty
        {
            get { return penalty; }
            protected set { penalty = value; }
        }

        public virtual void setOcena(Ocena o)
        {
            if (o != null)
            {
                D = o.D;
                E = o.E;
                Penalty = o.Penalty;
                Total = o.Total;
            }
            else
            {
                D = null;
                E = null;
                Penalty = null;
                Total = null;
            }
        }

        public virtual void clearOcena(Ocena o)
        {
            D = null;
            E = null;
            Penalty = null;
            Total = null;
        }

        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
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

    }
}
