using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class NastupNaSpravi : DomainObject
    {
        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

        private byte ekipa;
        public virtual byte Ekipa
        {
            get { return ekipa; }
            set { ekipa = value; }
        }

        private string kategorija = String.Empty;

        public NastupNaSpravi()
        { 
        
        }

        public NastupNaSpravi(GimnasticarUcesnik gimnasticar, byte ekipa)
        {
            this.gimnasticar = gimnasticar;
            this.ekipa = ekipa;

            // Svojstvo Kategorija kesiram zato sto sam imao problema na start listama gde nije mogao da ga ocita iz
            // gimnasticara.
            if (gimnasticar != null)
            {
                try
                {
                    kategorija = gimnasticar.TakmicarskaKategorija.ToString();
                }
                catch (Exception)
                {
                    kategorija = String.Empty;
                }
            }
        }

        public virtual string TakmicarskiBroj
        {
            get
            {
                if (Gimnasticar != null && Gimnasticar.TakmicarskiBroj.HasValue)
                    return Gimnasticar.TakmicarskiBroj.ToString();
                else
                    return String.Empty;
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

        public virtual string Kategorija
        {
            get
            {
                if (kategorija != String.Empty)
                    return kategorija;

                if (Gimnasticar != null)
                {
                    try
                    {
                        kategorija = Gimnasticar.TakmicarskaKategorija.ToString();
                    }
                    catch (Exception)
                    {
                        kategorija = String.Empty;
                    }
                    return kategorija;
                }
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

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);
            strBuilder.AppendLine(Ekipa.ToString());
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;
            Ekipa = byte.Parse(reader.ReadLine());
        }
    }
}
