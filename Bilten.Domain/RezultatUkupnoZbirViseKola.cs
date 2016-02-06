using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatUkupnoZbirViseKola : Rezultat
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

        private Nullable<float> _parterTreceKolo;
        public virtual Nullable<float> ParterTreceKolo
        {
            get { return _parterTreceKolo; }
            set { _parterTreceKolo = value; }
        }

        private Nullable<float> _parterCetvrtoKolo;
        public virtual Nullable<float> ParterCetvrtoKolo
        {
            get { return _parterCetvrtoKolo; }
            set { _parterCetvrtoKolo = value; }
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

        private Nullable<float> _konjTreceKolo;
        public virtual Nullable<float> KonjTreceKolo
        {
            get { return _konjTreceKolo; }
            set { _konjTreceKolo = value; }
        }

        private Nullable<float> _konjCetvrtoKolo;
        public virtual Nullable<float> KonjCetvrtoKolo
        {
            get { return _konjCetvrtoKolo; }
            set { _konjCetvrtoKolo = value; }
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

        private Nullable<float> _karikeTreceKolo;
        public virtual Nullable<float> KarikeTreceKolo
        {
            get { return _karikeTreceKolo; }
            set { _karikeTreceKolo = value; }
        }

        private Nullable<float> _karikeCetvrtoKolo;
        public virtual Nullable<float> KarikeCetvrtoKolo
        {
            get { return _karikeCetvrtoKolo; }
            set { _karikeCetvrtoKolo = value; }
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

        private Nullable<float> _preskokTreceKolo;
        public virtual Nullable<float> PreskokTreceKolo
        {
            get { return _preskokTreceKolo; }
            set { _preskokTreceKolo = value; }
        }

        private Nullable<float> _preskokCetvrtoKolo;
        public virtual Nullable<float> PreskokCetvrtoKolo
        {
            get { return _preskokCetvrtoKolo; }
            set { _preskokCetvrtoKolo = value; }
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

        private Nullable<float> _razbojTreceKolo;
        public virtual Nullable<float> RazbojTreceKolo
        {
            get { return _razbojTreceKolo; }
            set { _razbojTreceKolo = value; }
        }

        private Nullable<float> _razbojCetvrtoKolo;
        public virtual Nullable<float> RazbojCetvrtoKolo
        {
            get { return _razbojCetvrtoKolo; }
            set { _razbojCetvrtoKolo = value; }
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

        private Nullable<float> _vratiloTreceKolo;
        public virtual Nullable<float> VratiloTreceKolo
        {
            get { return _vratiloTreceKolo; }
            set { _vratiloTreceKolo = value; }
        }

        private Nullable<float> _vratiloCetvrtoKolo;
        public virtual Nullable<float> VratiloCetvrtoKolo
        {
            get { return _vratiloCetvrtoKolo; }
            set { _vratiloCetvrtoKolo = value; }
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

        private Nullable<float> _dvovisinskiRazbojTreceKolo;
        public virtual Nullable<float> DvovisinskiRazbojTreceKolo
        {
            get { return _dvovisinskiRazbojTreceKolo; }
            set { _dvovisinskiRazbojTreceKolo = value; }
        }

        private Nullable<float> _dvovisinskiRazbojCetvrtoKolo;
        public virtual Nullable<float> DvovisinskiRazbojCetvrtoKolo
        {
            get { return _dvovisinskiRazbojCetvrtoKolo; }
            set { _dvovisinskiRazbojCetvrtoKolo = value; }
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

        private Nullable<float> _gredaTreceKolo;
        public virtual Nullable<float> GredaTreceKolo
        {
            get { return _gredaTreceKolo; }
            set { _gredaTreceKolo = value; }
        }

        private Nullable<float> _gredaCetvrtoKolo;
        public virtual Nullable<float> GredaCetvrtoKolo
        {
            get { return _gredaCetvrtoKolo; }
            set { _gredaCetvrtoKolo = value; }
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

        private Nullable<float> _totalTreceKolo;
        public virtual Nullable<float> TotalTreceKolo
        {
            get { return _totalTreceKolo; }
            set { _totalTreceKolo = value; }
        }

        private Nullable<float> _totalCetvrtoKolo;
        public virtual Nullable<float> TotalCetvrtoKolo
        {
            get { return _totalCetvrtoKolo; }
            set { _totalCetvrtoKolo = value; }
        }

        // moram ovako jer je set accessor za Total protected u klasi Rezultat
        public void setTotal(Nullable<float> value)
        {
            Total = value;
        }

        public RezultatUkupnoZbirViseKola()
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
