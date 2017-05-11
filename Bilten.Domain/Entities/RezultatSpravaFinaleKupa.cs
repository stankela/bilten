using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatSpravaFinaleKupa : Rezultat
    {
        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

        private Nullable<float> d_PrvoKolo;
        public virtual Nullable<float> D_PrvoKolo
        {
            get { return d_PrvoKolo; }
            set { d_PrvoKolo = value; }
        }

        private Nullable<float> e_PrvoKolo;
        public virtual Nullable<float> E_PrvoKolo
        {
            get { return e_PrvoKolo; }
            set { e_PrvoKolo = value; }
        }

        private Nullable<float> _totalPrvoKolo;
        public virtual Nullable<float> TotalPrvoKolo
        {
            get { return _totalPrvoKolo; }
            set { _totalPrvoKolo = value; }
        }

        private Nullable<float> d_DrugoKolo;
        public virtual Nullable<float> D_DrugoKolo
        {
            get { return d_DrugoKolo; }
            set { d_DrugoKolo = value; }
        }

        private Nullable<float> e_DrugoKolo;
        public virtual Nullable<float> E_DrugoKolo
        {
            get { return e_DrugoKolo; }
            set { e_DrugoKolo = value; }
        }

        private Nullable<float> _totalDrugoKolo;
        public virtual Nullable<float> TotalDrugoKolo
        {
            get { return _totalDrugoKolo; }
            set { _totalDrugoKolo = value; }
        }

        public virtual void initPrvoKolo(RezultatSprava r)
        {
            D_PrvoKolo = r.D;
            E_PrvoKolo = r.E;
            TotalPrvoKolo = r.Total;
        }

        public virtual void initDrugoKolo(RezultatSprava r)
        {
            D_DrugoKolo = r.D;
            E_DrugoKolo = r.E;
            TotalDrugoKolo = r.Total;
        }

        public virtual void initPrvoKolo(RezultatPreskok r, bool naOsnovuObaPreskoka, bool postojeObaPreskoka)
        {
            if (!naOsnovuObaPreskoka || !postojeObaPreskoka)
                // Ovo takodje obradjuje situaciju kada je u propozicijama za prvo kolo stavljeno
                // da se preskok racuna na osnovu oba preskoka, ali ni za jednog gimnasticara ne
                // postoji ocena za oba preskoka. Ova situacija najverovatnije nastaje kada se u
                // prvom kolu kao prvi preskok unosila konacna ocena za oba preskoka.
                // U tom slucaju, za ocenu prvog kola treba uzeti prvu ocenu.
                initPrvoKolo(r);
            else
            {
                D_PrvoKolo = null;
                E_PrvoKolo = null;
                TotalPrvoKolo = r.TotalObeOcene;
            }
        }

        public virtual void initDrugoKolo(RezultatPreskok r, bool naOsnovuObaPreskoka, bool postojeObaPreskoka)
        {
            if (!naOsnovuObaPreskoka || !postojeObaPreskoka)
                initDrugoKolo(r);
            else
            {
                D_DrugoKolo = null;
                E_DrugoKolo = null;
                TotalDrugoKolo = r.TotalObeOcene;
            }
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
                // TODO3: Proveri da li treba podesavati broj decimala (isto i za ostale rezultate finala kupa i
                // zbira vise kola).
                total = (total1 + total2) / 2;
                if (nacin == NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene
                    && (TotalPrvoKolo == null || TotalDrugoKolo == null))
                {
                    total = total1 + total2;
                }
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
            strBuilder.AppendLine(D_PrvoKolo != null ? D_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(E_PrvoKolo != null ? E_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalPrvoKolo != null ? TotalPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(D_DrugoKolo != null ? D_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(E_DrugoKolo != null ? E_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalDrugoKolo != null ? TotalDrugoKolo.Value.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;

            line = reader.ReadLine();
            D_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            TotalPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            D_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            TotalDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;
        }
    }
}
