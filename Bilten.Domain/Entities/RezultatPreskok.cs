using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatPreskok : RezultatSprava
    {
        private Nullable<float> d_2;
        public virtual Nullable<float> D_2
        {
            get { return d_2; }
            protected set { d_2 = value; }
        }

        private Nullable<float> e_2;
        public virtual Nullable<float> E_2
        {
            get { return e_2; }
            protected set { e_2 = value; }
        }

        private Nullable<float> e1_2;
        public virtual Nullable<float> E1_2
        {
            get { return e1_2; }
            protected set { e1_2 = value; }
        }

        private Nullable<float> e2_2;
        public virtual Nullable<float> E2_2
        {
            get { return e2_2; }
            protected set { e2_2 = value; }
        }

        private Nullable<float> e3_2;
        public virtual Nullable<float> E3_2
        {
            get { return e3_2; }
            protected set { e3_2 = value; }
        }

        private Nullable<float> e4_2;
        public virtual Nullable<float> E4_2
        {
            get { return e4_2; }
            protected set { e4_2 = value; }
        }

        private Nullable<float> e5_2;
        public virtual Nullable<float> E5_2
        {
            get { return e5_2; }
            protected set { e5_2 = value; }
        }

        private Nullable<float> e6_2;
        public virtual Nullable<float> E6_2
        {
            get { return e6_2; }
            protected set { e6_2 = value; }
        }

        private Nullable<float> penalty_2;
        public virtual Nullable<float> Penalty_2
        {
            get { return penalty_2; }
            protected set { penalty_2 = value; }
        }

        private Nullable<float> total_2;
        public virtual Nullable<float> Total_2
        {
            get { return total_2; }
            protected set { total_2 = value; }
        }

        private Nullable<float> totalObeOcene;
        public virtual Nullable<float> TotalObeOcene
        {
            get { return totalObeOcene; }
            protected set { totalObeOcene = value; }
        }

        public virtual float EObeOcene
        {
            get
            {
                float result = 0f;
                if (E != null)
                    result += E.Value;
                if (E_2 != null)
                    result += E_2.Value;
                return result; 
            }
        }

        public override void setOcena(Ocena o)
        {
            base.setOcena(o);
            if (o != null && o.Ocena2 != null)
            {
                D_2 = o.Ocena2.D;
                E_2 = o.Ocena2.E;
                E1_2 = o.Ocena2.E1;
                E2_2 = o.Ocena2.E2;
                E3_2 = o.Ocena2.E3;
                E4_2 = o.Ocena2.E4;
                E5_2 = o.Ocena2.E5;
                E6_2 = o.Ocena2.E6;
                Penalty_2 = o.Ocena2.Penalty;
                Total_2 = o.Ocena2.Total;
                TotalObeOcene = o.TotalObeOcene;
            }
            else
            {
                D_2 = null;
                E_2 = null;
                E1_2 = null;
                E2_2 = null;
                E3_2 = null;
                E4_2 = null;
                E5_2 = null;
                E6_2 = null;
                Penalty_2 = null;
                Total_2 = null;
                TotalObeOcene = null;
            }
        }

        public override void clearOcena()
        {
            base.clearOcena();
            D_2 = null;
            E_2 = null;
            E1_2 = null;
            E2_2 = null;
            E3_2 = null;
            E4_2 = null;
            E5_2 = null;
            E6_2 = null;
            Penalty_2 = null;
            Total_2 = null;
            TotalObeOcene = null;
        }

        // TODO5: Da li ovde (i u loadFromDump) treba da se dodaju E1, E2, ...
        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(D_2 != null ? D_2.Value.ToString() : NULL);
            strBuilder.AppendLine(E_2 != null ? E_2.Value.ToString() : NULL);
            strBuilder.AppendLine(Penalty_2 != null ? Penalty_2.Value.ToString() : NULL);
            strBuilder.AppendLine(Total_2 != null ? Total_2.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalObeOcene != null ? TotalObeOcene.Value.ToString() : NULL);
        }

        public override void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader, map);

            string d_2 = reader.ReadLine();
            D_2 = d_2 != NULL ? float.Parse(d_2) : (float?)null;

            string e_2 = reader.ReadLine();
            E_2 = e_2 != NULL ? float.Parse(e_2) : (float?)null;

            string penalty_2 = reader.ReadLine();
            Penalty_2 = penalty_2 != NULL ? float.Parse(penalty_2) : (float?)null;

            string total_2 = reader.ReadLine();
            Total_2 = total_2 != NULL ? float.Parse(total_2) : (float?)null;

            string totalObeOcene = reader.ReadLine();
            TotalObeOcene = totalObeOcene != NULL ? float.Parse(totalObeOcene) : (float?)null;
        }
    }
}
