using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatPreskok : RezultatSprava
    {
        private Nullable<short> redBroj2;
        public virtual Nullable<short> RedBroj2
        {
            get { return redBroj2; }
            set { redBroj2 = value; }
        }

        private Nullable<short> rank2;
        public virtual Nullable<short> Rank2
        {
            get { return rank2; }
            set { rank2 = value; }
        }

        private Nullable<float> d_2;
        public virtual Nullable<float> D_2
        {
            get { return d_2; }
            private set { d_2 = value; }
        }

        private Nullable<float> e_2;
        public virtual Nullable<float> E_2
        {
            get { return e_2; }
            private set { e_2 = value; }
        }

        private Nullable<float> penalty_2;
        public virtual Nullable<float> Penalty_2
        {
            get { return penalty_2; }
            private set { penalty_2 = value; }
        }

        private Nullable<float> total_2;
        public virtual Nullable<float> Total_2
        {
            get { return total_2; }
            private set { total_2 = value; }
        }

        private Nullable<float> totalObeOcene;
        public virtual Nullable<float> TotalObeOcene
        {
            get { return totalObeOcene; }
            private set { totalObeOcene = value; }
        }

        public override void setOcena(Ocena o)
        {
            base.setOcena(o);

            if (o != null && o.Ocena2 != null)
            {
                D_2 = o.Ocena2.D;
                E_2 = o.Ocena2.E;
                Penalty_2 = o.Ocena2.Penalty;
                Total_2 = o.Ocena2.Total;
                TotalObeOcene = o.TotalObeOcene;
            }
            else
            {
                D_2 = null;
                E_2 = null;
                Penalty_2 = null;
                Total_2 = null;
                TotalObeOcene = null;
            }
        }

        public override void clearOcena(Ocena o)
        {
            base.clearOcena(o);
            D_2 = null;
            E_2 = null;
            Penalty_2 = null;
            Total_2 = null;
            TotalObeOcene = null;
        }
    }
}
