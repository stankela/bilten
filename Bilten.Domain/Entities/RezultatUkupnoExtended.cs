using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatUkupnoExtended : RezultatUkupno
    {
        public RezultatUkupnoExtended(RezultatUkupno r)
        {
            RedBroj = r.RedBroj;
            Rank = r.Rank;
            KvalStatus = r.KvalStatus;
            Total = r.Total;

            Parter = r.Parter;
            Konj = r.Konj;
            Karike = r.Karike;
            Preskok = r.Preskok;
            Razboj = r.Razboj;
            Vratilo = r.Vratilo;
            Greda = r.Greda;
            DvovisinskiRazboj = r.DvovisinskiRazboj;

            Gimnasticar = r.Gimnasticar;
        }

        private Nullable<float> _parterD;
        public virtual Nullable<float> ParterD
        {
            get { return _parterD; }
            protected set { _parterD = value; }
        }

        private Nullable<float> _parterE;
        public virtual Nullable<float> ParterE
        {
            get { return _parterE; }
            protected set { _parterE = value; }
        }

        private Nullable<float> _parterPen;
        public virtual Nullable<float> ParterPen
        {
            get { return _parterPen; }
            protected set { _parterPen = value; }
        }

        private Nullable<float> _konjD;
        public virtual Nullable<float> KonjD
        {
            get { return _konjD; }
            protected set { _konjD = value; }
        }

        private Nullable<float> _konjE;
        public virtual Nullable<float> KonjE
        {
            get { return _konjE; }
            protected set { _konjE = value; }
        }

        private Nullable<float> _konjPen;
        public virtual Nullable<float> KonjPen
        {
            get { return _konjPen; }
            protected set { _konjPen = value; }
        }

        private Nullable<float> _karikeD;
        public virtual Nullable<float> KarikeD
        {
            get { return _karikeD; }
            protected set { _karikeD = value; }
        }

        private Nullable<float> _karikeE;
        public virtual Nullable<float> KarikeE
        {
            get { return _karikeE; }
            protected set { _karikeE = value; }
        }

        private Nullable<float> _karikePen;
        public virtual Nullable<float> KarikePen
        {
            get { return _karikePen; }
            protected set { _karikePen = value; }
        }

        private Nullable<float> _preskokD;
        public virtual Nullable<float> PreskokD
        {
            get { return _preskokD; }
            protected set { _preskokD = value; }
        }

        private Nullable<float> _preskokE;
        public virtual Nullable<float> PreskokE
        {
            get { return _preskokE; }
            protected set { _preskokE = value; }
        }

        private Nullable<float> _preskokPen;
        public virtual Nullable<float> PreskokPen
        {
            get { return _preskokPen; }
            protected set { _preskokPen = value; }
        }

        private Nullable<float> _razbojD;
        public virtual Nullable<float> RazbojD
        {
            get { return _razbojD; }
            protected set { _razbojD = value; }
        }

        private Nullable<float> _razbojE;
        public virtual Nullable<float> RazbojE
        {
            get { return _razbojE; }
            protected set { _razbojE = value; }
        }

        private Nullable<float> _razbojPen;
        public virtual Nullable<float> RazbojPen
        {
            get { return _razbojPen; }
            protected set { _razbojPen = value; }
        }

        private Nullable<float> _vratiloD;
        public virtual Nullable<float> VratiloD
        {
            get { return _vratiloD; }
            protected set { _vratiloD = value; }
        }

        private Nullable<float> _vratiloE;
        public virtual Nullable<float> VratiloE
        {
            get { return _vratiloE; }
            protected set { _vratiloE = value; }
        }

        private Nullable<float> _vratiloPen;
        public virtual Nullable<float> VratiloPen
        {
            get { return _vratiloPen; }
            protected set { _vratiloPen = value; }
        }

        private Nullable<float> _gredaD;
        public virtual Nullable<float> GredaD
        {
            get { return _gredaD; }
            protected set { _gredaD = value; }
        }

        private Nullable<float> _gredaE;
        public virtual Nullable<float> GredaE
        {
            get { return _gredaE; }
            protected set { _gredaE = value; }
        }

        private Nullable<float> _gredaPen;
        public virtual Nullable<float> GredaPen
        {
            get { return _gredaPen; }
            protected set { _gredaPen = value; }
        }

        private Nullable<float> _dvovisinskiRazbojD;
        public virtual Nullable<float> DvovisinskiRazbojD
        {
            get { return _dvovisinskiRazbojD; }
            protected set { _dvovisinskiRazbojD = value; }
        }

        private Nullable<float> _dvovisinskiRazbojE;
        public virtual Nullable<float> DvovisinskiRazbojE
        {
            get { return _dvovisinskiRazbojE; }
            protected set { _dvovisinskiRazbojE = value; }
        }

        private Nullable<float> _dvovisinskiRazbojPen;
        public virtual Nullable<float> DvovisinskiRazbojPen
        {
            get { return _dvovisinskiRazbojPen; }
            protected set { _dvovisinskiRazbojPen = value; }
        }

        public void setDOcena(Sprava sprava, Nullable<float> value)
        {
            switch (sprava)
            {
                case Sprava.Parter:
                    ParterD = value;
                    break;

                case Sprava.Konj:
                    KonjD = value;
                    break;

                case Sprava.Karike:
                    KarikeD = value;
                    break;

                case Sprava.Preskok:
                    PreskokD = value;
                    break;

                case Sprava.Razboj:
                    RazbojD = value;
                    break;

                case Sprava.Vratilo:
                    VratiloD = value;
                    break;

                case Sprava.DvovisinskiRazboj:
                    DvovisinskiRazbojD = value;
                    break;

                case Sprava.Greda:
                    GredaD = value;
                    break;

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za spravu.");
            }
        }

        public void setEOcena(Sprava sprava, Nullable<float> value)
        {
            switch (sprava)
            {
                case Sprava.Parter:
                    ParterE = value;
                    break;

                case Sprava.Konj:
                    KonjE = value;
                    break;

                case Sprava.Karike:
                    KarikeE = value;
                    break;

                case Sprava.Preskok:
                    PreskokE = value;
                    break;

                case Sprava.Razboj:
                    RazbojE = value;
                    break;

                case Sprava.Vratilo:
                    VratiloE = value;
                    break;

                case Sprava.DvovisinskiRazboj:
                    DvovisinskiRazbojE = value;
                    break;

                case Sprava.Greda:
                    GredaE = value;
                    break;

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za spravu.");
            }
        }

        public void setPenalizacija(Sprava sprava, Nullable<float> value)
        {
            switch (sprava)
            {
                case Sprava.Parter:
                    ParterPen = value;
                    break;

                case Sprava.Konj:
                    KonjPen = value;
                    break;

                case Sprava.Karike:
                    KarikePen = value;
                    break;

                case Sprava.Preskok:
                    PreskokPen = value;
                    break;

                case Sprava.Razboj:
                    RazbojPen = value;
                    break;

                case Sprava.Vratilo:
                    VratiloPen = value;
                    break;

                case Sprava.DvovisinskiRazboj:
                    DvovisinskiRazbojPen = value;
                    break;

                case Sprava.Greda:
                    GredaPen = value;
                    break;

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za spravu.");
            }
        }
    }
}
