using System;
using System.Collections.Generic;
using System.IO;
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
            protected set { d_2 = value; }
        }

        private Nullable<float> e_2;
        public virtual Nullable<float> E_2
        {
            get { return e_2; }
            protected set { e_2 = value; }
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

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(RedBroj2 != null ? RedBroj2.Value.ToString() : NULL);
            strBuilder.AppendLine(Rank2 != null ? Rank2.Value.ToString() : NULL);
            strBuilder.AppendLine(D_2 != null ? D_2.Value.ToString() : NULL);
            strBuilder.AppendLine(E_2 != null ? E_2.Value.ToString() : NULL);
            strBuilder.AppendLine(Penalty_2 != null ? Penalty_2.Value.ToString() : NULL);
            strBuilder.AppendLine(Total_2 != null ? Total_2.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalObeOcene != null ? TotalObeOcene.Value.ToString() : NULL);
        }

        public override void loadFromDump(StringReader reader, IDictionary<int, GimnasticarUcesnik> gimnasticariMap)
        {
            base.loadFromDump(reader, gimnasticariMap);

            string redBroj2 = reader.ReadLine();
            RedBroj2 = redBroj2 != NULL ? short.Parse(redBroj2) : (short?)null;

            string rank2 = reader.ReadLine();
            Rank2 = rank2 != NULL ? short.Parse(rank2) : (short?)null;

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
