using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatUkupnoFinaleKupa : Rezultat
    {
        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

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

        public RezultatUkupnoFinaleKupa()
        { 
        
        }

        public virtual void initPrvoKolo(RezultatUkupno r)
        {
            ParterPrvoKolo = r.Parter;
            KonjPrvoKolo = r.Konj;
            KarikePrvoKolo = r.Karike;
            PreskokPrvoKolo = r.Preskok;
            RazbojPrvoKolo = r.Razboj;
            VratiloPrvoKolo = r.Vratilo;
            DvovisinskiRazbojPrvoKolo = r.DvovisinskiRazboj;
            GredaPrvoKolo = r.Greda;
            TotalPrvoKolo = r.Total;
        }

        public virtual void initDrugoKolo(RezultatUkupno r)
        {
            ParterDrugoKolo = r.Parter;
            KonjDrugoKolo = r.Konj;
            KarikeDrugoKolo = r.Karike;
            PreskokDrugoKolo = r.Preskok;
            RazbojDrugoKolo = r.Razboj;
            VratiloDrugoKolo = r.Vratilo;
            DvovisinskiRazbojDrugoKolo = r.DvovisinskiRazboj;
            GredaDrugoKolo = r.Greda;
            TotalDrugoKolo = r.Total;
        }

        public virtual void calculateTotal(NacinRacunanjaOceneFinaleKupa nacin)
        {
            if (TotalPrvoKolo == null && TotalDrugoKolo == null)
            {
                Total = null;
                return;
            }
            float total1 = TotalPrvoKolo == null ? 0 : TotalPrvoKolo.Value;
            float total2 = TotalDrugoKolo == null ? 0 : TotalDrugoKolo.Value;
            float total;

            if (nacin == NacinRacunanjaOceneFinaleKupa.Zbir)
                total = total1 + total2;
            else if (nacin == NacinRacunanjaOceneFinaleKupa.Max)
                total = total1 > total2 ? total1 : total2;
            else
            {
                total = (total1 + total2) / 2;
                if (nacin == NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene && (TotalPrvoKolo == null || TotalDrugoKolo == null))
                    total = total1 + total2;
            }
            Total = total;
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
            strBuilder.AppendLine(ParterPrvoKolo != null ? ParterPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(ParterDrugoKolo != null ? ParterDrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(KonjPrvoKolo != null ? KonjPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(KonjDrugoKolo != null ? KonjDrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(KarikePrvoKolo != null ? KarikePrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(KarikeDrugoKolo != null ? KarikeDrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(PreskokPrvoKolo != null ? PreskokPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(PreskokDrugoKolo != null ? PreskokDrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(RazbojPrvoKolo != null ? RazbojPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(RazbojDrugoKolo != null ? RazbojDrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(VratiloPrvoKolo != null ? VratiloPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(VratiloDrugoKolo != null ? VratiloDrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(DvovisinskiRazbojPrvoKolo != null ? DvovisinskiRazbojPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(DvovisinskiRazbojDrugoKolo != null ? DvovisinskiRazbojDrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(GredaPrvoKolo != null ? GredaPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(GredaDrugoKolo != null ? GredaDrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalPrvoKolo != null ? TotalPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalDrugoKolo != null ? TotalDrugoKolo.Value.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string gimnasticarId = reader.ReadLine();
            Gimnasticar = gimnasticarId != NULL ? map.gimnasticariMap[int.Parse(gimnasticarId)] : null;

            string line = reader.ReadLine();
            ParterPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            ParterDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            KonjPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            KonjDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            KarikePrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            KarikeDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            PreskokPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            PreskokDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            RazbojPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            RazbojDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            VratiloPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            VratiloDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            DvovisinskiRazbojPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            DvovisinskiRazbojDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            GredaPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            GredaDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            TotalPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            TotalDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;
        }
    }
}
