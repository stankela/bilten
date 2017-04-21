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

        public virtual string NazivEkipe
        {
            get
            {
                if (Ekipa != null)
                    return Ekipa.Naziv;
                return String.Empty;
            }
        }

        public virtual Nullable<float> Penalty
        {
            get
            {
                if (Ekipa != null)
                    return Ekipa.Penalty;
                return null;
            }
        }

        public virtual void addPenalty(float value)
        {
            if (Total == null)
                Total = -value;
            else
                Total = (float)((decimal)Total - (decimal)value);
        }

        public virtual void changePenalty(Nullable<float> newPenalty)
        {
            Nullable<float> oldPenalty = Ekipa.Penalty;
            if (oldPenalty != null)
                Total = (float)((decimal)Total + (decimal)oldPenalty);
            if (newPenalty != null)
            {
                if (Total == null)
                    Total = -newPenalty;
                else
                    Total = (float)((decimal)Total - (decimal)newPenalty);
            }
            Ekipa.Penalty = newPenalty;
        }

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Ekipa != null ? Ekipa.Id.ToString() : NULL);
        }

        public override void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader, map);

            string ekipaId = reader.ReadLine();
            Ekipa = ekipaId != NULL ? map.ekipeMap[int.Parse(ekipaId)] : null;
        }
    }
}
