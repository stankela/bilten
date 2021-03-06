using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class Rezultat : DomainObject
    {
        private short redBroj;
        public virtual short RedBroj
        {
            get { return redBroj; }
            set { redBroj = value; }
        }

        private Nullable<short> rank;
        public virtual Nullable<short> Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        private KvalifikacioniStatus kvalStatus;
        public virtual KvalifikacioniStatus KvalStatus
        {
            get { return kvalStatus; }
            set { kvalStatus = value; }
        }

        private Nullable<float> total;
        public virtual Nullable<float> Total
        {
            get { return total; }
            protected set { total = value; }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            
            strBuilder.AppendLine(RedBroj.ToString());
            strBuilder.AppendLine(Rank != null ? Rank.Value.ToString() : NULL);
            strBuilder.AppendLine(KvalStatus.ToString());
            strBuilder.AppendLine(Total != null ? Total.Value.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader)
        {
            RedBroj = byte.Parse(reader.ReadLine());

            string rank = reader.ReadLine();
            Rank = rank != NULL ? short.Parse(rank) : (short?)null;
            
            KvalStatus = (KvalifikacioniStatus)Enum.Parse(typeof(KvalifikacioniStatus), reader.ReadLine());
            
            string total = reader.ReadLine();
            Total = total != NULL ? float.Parse(total) : (float?)null;
        }
    }
}
