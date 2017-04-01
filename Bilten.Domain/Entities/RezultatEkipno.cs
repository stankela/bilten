using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatEkipno : RezultatUkupno
    {
        private Ekipa _ekipa;
        public virtual Ekipa Ekipa
        {
            get { return _ekipa; }
            set { _ekipa = value; }
        }

        private Nullable<float> penalty;
        public virtual Nullable<float> Penalty
        {
            get { return penalty; }
            protected set { penalty = value; }
        }

        public virtual string NazivEkipe
        {
            get
            {
                if (Ekipa != null)
                    return Ekipa.Naziv;
                else
                    return String.Empty;
            }
        }

        public virtual void addPenalty(Nullable<float> value)
        {
            if (value == null || value == 0)
            {
                // remove penalty
                if (Penalty == null || Penalty == 0)
                    return;
                Total = (float)((decimal)Total + (decimal)Penalty);
                Penalty = null;
                return;
            }

            if (Penalty == null)
            {
                if (Total == null)
                {
                    Total = -value;
                }
                else
                {
                    Total = (float)((decimal)Total - (decimal)value);
                }
            }
            else
            {
                Total = (float)((decimal)Total + (decimal)Penalty - (decimal)value);
            }
            Penalty = value;
        }

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Ekipa != null ? Ekipa.Id.ToString() : NULL);
            strBuilder.AppendLine(Penalty != null ? Penalty.Value.ToString() : NULL);
        }

        public override void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader, map);

            string ekipaId = reader.ReadLine();
            Ekipa = ekipaId != NULL ? map.ekipeMap[int.Parse(ekipaId)] : null;

            string penalty = reader.ReadLine();
            Penalty = penalty != NULL ? float.Parse(penalty) : (float?)null;
        }
    }
}
