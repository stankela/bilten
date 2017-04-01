using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatEkipnoZbirViseKola : RezultatUkupnoZbirViseKola
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
                else
                    return String.Empty;
            }
        }

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Ekipa != null ? Ekipa.Id.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IDictionary<int, Ekipa> ekipeMap)
        {
            base.loadFromDump(reader, null);

            string ekipaId = reader.ReadLine();
            Ekipa = ekipaId != NULL ? ekipeMap[int.Parse(ekipaId)] : null;
        }
    }
}
