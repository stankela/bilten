using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatUkupno : Rezultat
    {
        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

        private Nullable<float> _parter;
        public virtual Nullable<float> Parter
        {
            get { return _parter; }
            protected set { _parter = value; }
        }

        private Nullable<float> _konj;
        public virtual Nullable<float> Konj
        {
            get { return _konj; }
            protected set { _konj = value; }
        }

        private Nullable<float> _karike;
        public virtual Nullable<float> Karike
        {
            get { return _karike; }
            protected set { _karike = value; }
        }

        private Nullable<float> _preskok;
        public virtual Nullable<float> Preskok
        {
            get { return _preskok; }
            protected set { _preskok = value; }
        }

        private Nullable<float> _razboj;
        public virtual Nullable<float> Razboj
        {
            get { return _razboj; }
            protected set { _razboj = value; }
        }

        private Nullable<float> _vratilo;
        public virtual Nullable<float> Vratilo
        {
            get { return _vratilo; }
            protected set { _vratilo = value; }
        }

        private Nullable<float> _dvovisinskiRazboj;
        public virtual Nullable<float> DvovisinskiRazboj
        {
            get { return _dvovisinskiRazboj; }
            protected set { _dvovisinskiRazboj = value; }
        }

        private Nullable<float> _greda;
        public virtual Nullable<float> Greda
        {
            get { return _greda; }
            protected set { _greda = value; }
        }

        public RezultatUkupno()
        { 
        
        }

        public virtual Nullable<float> getOcena(Sprava sprava)
        {
            switch (sprava)
            {
                case Sprava.Parter:
                    return Parter;

                case Sprava.Konj:
                    return Konj;

                case Sprava.Karike:
                    return Karike;

                case Sprava.Preskok:
                    return Preskok;

                case Sprava.Razboj:
                    return Razboj;

                case Sprava.Vratilo:
                    return Vratilo;

                case Sprava.DvovisinskiRazboj:
                    return DvovisinskiRazboj;

                case Sprava.Greda:
                    return Greda;

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za spravu.");
            }
        }

        protected void setOcena(Sprava sprava, Nullable<float> value)
        {
            switch (sprava)
            {
                case Sprava.Parter:
                    Parter = value;
                    break;

                case Sprava.Konj:
                    Konj = value;
                    break;

                case Sprava.Karike:
                    Karike = value;
                    break;

                case Sprava.Preskok:
                    Preskok = value;
                    break;

                case Sprava.Razboj:
                    Razboj = value;
                    break;

                case Sprava.Vratilo:
                    Vratilo = value;
                    break;

                case Sprava.DvovisinskiRazboj:
                    DvovisinskiRazboj = value;
                    break;

                case Sprava.Greda:
                    Greda = value;
                    break;

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za spravu.");
            }
        }

        public virtual void addOcena(Ocena o, bool zaPreskokVisebojRacunajBoljuOcenu)
        {
            float? value;
            if (o.Sprava != Sprava.Preskok || !zaPreskokVisebojRacunajBoljuOcenu)
                value = o.Total;
            else
            {
                if (o.Ocena2 == null)
                    value = o.Total;
                else
                    value = Math.Max(o.Total.Value, o.Ocena2.Total.Value);
            }
            setOcena(o.Sprava, value);

            if (Total == null)
                Total = value;
            else
                Total = (float)((decimal)Total + (decimal)value);
        }

        public virtual void removeOcena(Ocena o)
        {
            float? value = getOcena(o.Sprava);
            if (value == null)
                return;
            setOcena(o.Sprava, null);

            if (isEmpty())
                Total = null;
            else
                Total = (float)((decimal)Total - (decimal)value);
        }

        private bool isEmpty()
        {
            // TODO4: Apdejtuj ovo ako dodajes penalizaciju za gimnasticara za viseboj.
            return Parter == null && Konj == null && Karike == null
            && Preskok == null && Razboj == null && Vratilo == null
            && Greda == null && DvovisinskiRazboj == null;
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

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);
            strBuilder.AppendLine(Parter != null ? Parter.Value.ToString() : NULL);
            strBuilder.AppendLine(Konj != null ? Konj.Value.ToString() : NULL);
            strBuilder.AppendLine(Karike != null ? Karike.Value.ToString() : NULL);
            strBuilder.AppendLine(Preskok != null ? Preskok.Value.ToString() : NULL);
            strBuilder.AppendLine(Razboj != null ? Razboj.Value.ToString() : NULL);
            strBuilder.AppendLine(Vratilo != null ? Vratilo.Value.ToString() : NULL);
            strBuilder.AppendLine(DvovisinskiRazboj != null ? DvovisinskiRazboj.Value.ToString() : NULL);
            strBuilder.AppendLine(Greda != null ? Greda.Value.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;
        
            line = reader.ReadLine();
            Parter = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Konj = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Karike = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Preskok = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Razboj = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Vratilo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            DvovisinskiRazboj = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Greda = line != NULL ? float.Parse(line) : (float?)null;
        }
    }
}
