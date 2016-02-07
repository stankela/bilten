using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatUkupnoFinaleKupa : Rezultat
    {
        private Nullable<float> _parterPrvoKolo;
        public virtual Nullable<float> ParterPrvoKolo
        {
            get { return _parterPrvoKolo; }
            set { _parterPrvoKolo = value; }
        }

        private Nullable<float> _parterDrugoKolo;
        public virtual Nullable<float> ParterDrugoKolo
        {
            get { return _parterDrugoKolo; }
            set { _parterDrugoKolo = value; }
        }

        private Nullable<float> _konjPrvoKolo;
        public virtual Nullable<float> KonjPrvoKolo
        {
            get { return _konjPrvoKolo; }
            set { _konjPrvoKolo = value; }
        }

        private Nullable<float> _konjDrugoKolo;
        public virtual Nullable<float> KonjDrugoKolo
        {
            get { return _konjDrugoKolo; }
            set { _konjDrugoKolo = value; }
        }

        private Nullable<float> _karikePrvoKolo;
        public virtual Nullable<float> KarikePrvoKolo
        {
            get { return _karikePrvoKolo; }
            set { _karikePrvoKolo = value; }
        }

        private Nullable<float> _karikeDrugoKolo;
        public virtual Nullable<float> KarikeDrugoKolo
        {
            get { return _karikeDrugoKolo; }
            set { _karikeDrugoKolo = value; }
        }

        private Nullable<float> _preskokPrvoKolo;
        public virtual Nullable<float> PreskokPrvoKolo
        {
            get { return _preskokPrvoKolo; }
            set { _preskokPrvoKolo = value; }
        }

        private Nullable<float> _preskokDrugoKolo;
        public virtual Nullable<float> PreskokDrugoKolo
        {
            get { return _preskokDrugoKolo; }
            set { _preskokDrugoKolo = value; }
        }

        private Nullable<float> _razbojPrvoKolo;
        public virtual Nullable<float> RazbojPrvoKolo
        {
            get { return _razbojPrvoKolo; }
            set { _razbojPrvoKolo = value; }
        }

        private Nullable<float> _razbojDrugoKolo;
        public virtual Nullable<float> RazbojDrugoKolo
        {
            get { return _razbojDrugoKolo; }
            set { _razbojDrugoKolo = value; }
        }

        private Nullable<float> _vratiloPrvoKolo;
        public virtual Nullable<float> VratiloPrvoKolo
        {
            get { return _vratiloPrvoKolo; }
            set { _vratiloPrvoKolo = value; }
        }

        private Nullable<float> _vratiloDrugoKolo;
        public virtual Nullable<float> VratiloDrugoKolo
        {
            get { return _vratiloDrugoKolo; }
            set { _vratiloDrugoKolo = value; }
        }

        private Nullable<float> _dvovisinskiRazbojPrvoKolo;
        public virtual Nullable<float> DvovisinskiRazbojPrvoKolo
        {
            get { return _dvovisinskiRazbojPrvoKolo; }
            set { _dvovisinskiRazbojPrvoKolo = value; }
        }

        private Nullable<float> _dvovisinskiRazbojDrugoKolo;
        public virtual Nullable<float> DvovisinskiRazbojDrugoKolo
        {
            get { return _dvovisinskiRazbojDrugoKolo; }
            set { _dvovisinskiRazbojDrugoKolo = value; }
        }

        private Nullable<float> _gredaPrvoKolo;
        public virtual Nullable<float> GredaPrvoKolo
        {
            get { return _gredaPrvoKolo; }
            set { _gredaPrvoKolo = value; }
        }

        private Nullable<float> _gredaDrugoKolo;
        public virtual Nullable<float> GredaDrugoKolo
        {
            get { return _gredaDrugoKolo; }
            set { _gredaDrugoKolo = value; }
        }

        private Nullable<float> _totalPrvoKolo;
        public virtual Nullable<float> TotalPrvoKolo
        {
            get { return _totalPrvoKolo; }
            set { _totalPrvoKolo = value; }
        }

        private Nullable<float> _totalDrugoKolo;
        public virtual Nullable<float> TotalDrugoKolo
        {
            get { return _totalDrugoKolo; }
            set { _totalDrugoKolo = value; }
        }

        // moram ovako jer je set accessor za Total protected u klasi Rezultat
        public void setTotal(Nullable<float> value)
        {
            Total = value;
        }

        public RezultatUkupnoFinaleKupa()
        { 
        
        }

        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

        public virtual Nullable<int> TakmicarskiBroj
        {
            get
            {
                if (Gimnasticar != null && Gimnasticar.TakmicarskiBroj.HasValue)
                    return Gimnasticar.TakmicarskiBroj;
                else
                    return null;
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

    }
}
