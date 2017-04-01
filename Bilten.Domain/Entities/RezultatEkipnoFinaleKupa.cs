using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatEkipnoFinaleKupa : RezultatUkupnoFinaleKupa
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

        public override void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader, map);

            string ekipaId = reader.ReadLine();
            Ekipa = ekipaId != NULL ? map.ekipeMap[int.Parse(ekipaId)] : null;
        }
    }
}
