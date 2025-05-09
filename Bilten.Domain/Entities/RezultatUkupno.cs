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

        public virtual Nullable<float> Penalty
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.PenaltyViseboj;
                return null;
            }
        }

        public virtual Nullable<float> getSprava(Sprava sprava)
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

        protected void setSprava(Sprava sprava, Nullable<float> value)
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

        public virtual void addSprava(Ocena o, bool zaPreskokVisebojRacunajBoljuOcenu)
        {
            float? value;
            if (o.Sprava != Sprava.Preskok || !zaPreskokVisebojRacunajBoljuOcenu || o.Ocena2 == null)
                value = o.Total;
            else
                value = Math.Max(o.Total.Value, o.Ocena2.Total.Value);
            setSprava(o.Sprava, value);

            if (Total == null)
                Total = value;
            else
                Total = (float)((decimal)Total + (decimal)value);
        }

        public virtual void clearSprava(Sprava s)
        {
            float? value = getSprava(s);
            if (value == null)
                return;

            setSprava(s, null);
            if (isEmpty())
                Total = null;
            else
                Total = (float)((decimal)Total - (decimal)value);
        }

        public virtual void addPenalty(float value)
        {
            if (Total == null)
                Total = -value;
            else
                Total = (float)((decimal)Total - (decimal)value);
        }

        private bool isEmpty()
        {
            return Parter == null && Konj == null && Karike == null
            && Preskok == null && Razboj == null && Vratilo == null
            && Greda == null && DvovisinskiRazboj == null && Penalty == null;
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

        public virtual string Kategorija
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.TakmicarskaKategorija.Naziv;
                else
                    return String.Empty;
            }
        }

        public virtual string KategorijaRedBroj
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.TakmicarskaKategorija.RedBroj.ToString();
                else
                    return String.Empty;
            }
        }

        private short redBrojIzvestaj;
        public virtual short RedBrojIzvestaj
        {
            get { return redBrojIzvestaj; }
            set { redBrojIzvestaj = value; }
        }

        public virtual void promeniPenalizacijuZaViseboj(Nullable<float> newPenalty)
        {
            Nullable<float> oldPenalty = Gimnasticar.PenaltyViseboj;
            if (oldPenalty != null)
                Total = (float)((decimal)Total + (decimal)oldPenalty);
            if (newPenalty != null)
            {
                if (Total == null)
                    Total = -newPenalty;
                else
                    Total = (float)((decimal)Total - (decimal)newPenalty);
            }
            Gimnasticar.PenaltyViseboj = newPenalty;
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
