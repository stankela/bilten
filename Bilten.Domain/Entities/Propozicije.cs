using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Bilten.Util;
using Bilten.Exceptions;

namespace Bilten.Domain
{
    public class Propozicije : DomainObject
    {
        private bool postojiTak2;
        public virtual bool PostojiTak2
        {
            get { return postojiTak2; }
            set { postojiTak2 = value; }
        }

        private bool odvojenoTak2;
        public virtual bool OdvojenoTak2
        {
            get { return odvojenoTak2; }
            set { odvojenoTak2 = value; }
        }

        private bool neogranicenBrojTakmicaraIzKlubaTak2;
        public virtual bool NeogranicenBrojTakmicaraIzKlubaTak2
        {
            get { return neogranicenBrojTakmicaraIzKlubaTak2; }
            set { neogranicenBrojTakmicaraIzKlubaTak2 = value; }
        }

        private byte maxBrojTakmicaraIzKlubaTak2;
        public virtual byte MaxBrojTakmicaraIzKlubaTak2
        {
            get { return maxBrojTakmicaraIzKlubaTak2; }
            set { maxBrojTakmicaraIzKlubaTak2 = value; }
        }

        private byte brojFinalistaTak2;
        public virtual byte BrojFinalistaTak2
        {
            get { return brojFinalistaTak2; }
            set { brojFinalistaTak2 = value; }
        }

        private byte brojRezerviTak2;
        public virtual byte BrojRezerviTak2
        {
            get { return brojRezerviTak2; }
            set { brojRezerviTak2 = value; }
        }

        private bool postojiTak3;
        public virtual bool PostojiTak3
        {
            get { return postojiTak3; }
            set { postojiTak3 = value; }
        }

        private bool odvojenoTak3;
        public virtual bool OdvojenoTak3
        {
            get { return odvojenoTak3; }
            set { odvojenoTak3 = value; }
        }

        private bool neogranicenBrojTakmicaraIzKlubaTak3;
        public virtual bool NeogranicenBrojTakmicaraIzKlubaTak3
        {
            get { return neogranicenBrojTakmicaraIzKlubaTak3; }
            set { neogranicenBrojTakmicaraIzKlubaTak3 = value; }
        }

        private byte maxBrojTakmicaraIzKlubaTak3;
        public virtual byte MaxBrojTakmicaraIzKlubaTak3
        {
            get { return maxBrojTakmicaraIzKlubaTak3; }
            set { maxBrojTakmicaraIzKlubaTak3 = value; }
        }

        private bool maxBrojTakmicaraTak3VaziZaDrzavu;
        public virtual bool MaxBrojTakmicaraTak3VaziZaDrzavu
        {
            get { return maxBrojTakmicaraTak3VaziZaDrzavu; }
            set { maxBrojTakmicaraTak3VaziZaDrzavu = value; }
        }

        private byte brojFinalistaTak3;
        public virtual byte BrojFinalistaTak3
        {
            get { return brojFinalistaTak3; }
            set { brojFinalistaTak3 = value; }
        }

        private byte brojRezerviTak3;
        public virtual byte BrojRezerviTak3
        {
            get { return brojRezerviTak3; }
            set { brojRezerviTak3 = value; }
        }

        private bool kvalifikantiTak3PreskokNaOsnovuObaPreskoka;
        public virtual bool KvalifikantiTak3PreskokNaOsnovuObaPreskoka
        {
            get { return kvalifikantiTak3PreskokNaOsnovuObaPreskoka; }
            set { kvalifikantiTak3PreskokNaOsnovuObaPreskoka = value; }
        }

        private bool poredakTak3PreskokNaOsnovuObaPreskoka;
        public virtual bool PoredakTak3PreskokNaOsnovuObaPreskoka
        {
            get { return poredakTak3PreskokNaOsnovuObaPreskoka; }
            set { poredakTak3PreskokNaOsnovuObaPreskoka = value; }
        }

        private bool postojiTak4;
        public virtual bool PostojiTak4
        {
            get { return postojiTak4; }
            set { postojiTak4 = value; }
        }

        private bool odvojenoTak4;
        public virtual bool OdvojenoTak4
        {
            get { return odvojenoTak4; }
            set { odvojenoTak4 = value; }
        }

        private byte brojRezultataKojiSeBodujuZaEkipu;
        public virtual byte BrojRezultataKojiSeBodujuZaEkipu
        {
            get { return brojRezultataKojiSeBodujuZaEkipu; }
            set { brojRezultataKojiSeBodujuZaEkipu = value; }
        }

        private byte brojEkipaUFinalu;
        public virtual byte BrojEkipaUFinalu
        {
            get { return brojEkipaUFinalu; }
            set { brojEkipaUFinalu = value; }
        }

        private bool jednoTak4ZaSveKategorije;
        public virtual bool JednoTak4ZaSveKategorije
        {
            get { return jednoTak4ZaSveKategorije; }
            set { jednoTak4ZaSveKategorije = value; }
        }

        public virtual bool racunajObaPreskoka(DeoTakmicenjaKod deoTakKod, bool finaleKupa)
        {
            if (!PostojiTak3)
                return false;

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (!OdvojenoTak3)
                    return PoredakTak3PreskokNaOsnovuObaPreskoka;
                else
                {
                    if (!finaleKupa)
                        return KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                    else
                        // Kada u finalu kupa postoji posebno finale po spravama.
                        return PoredakTak3PreskokNaOsnovuObaPreskoka;
                }
            }

            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje3);
            return PoredakTak3PreskokNaOsnovuObaPreskoka;
        }

        // Finale kupa

        // Takmicenje 2

        private bool tak2FinalnaOcenaJeZbirObaKola;
        public virtual bool Tak2FinalnaOcenaJeZbirObaKola
        {
            get { return tak2FinalnaOcenaJeZbirObaKola; }
            set { tak2FinalnaOcenaJeZbirObaKola = value; }
        }

        private bool tak2FinalnaOcenaJeMaxObaKola;
        public virtual bool Tak2FinalnaOcenaJeMaxObaKola
        {
            get { return tak2FinalnaOcenaJeMaxObaKola; }
            set { tak2FinalnaOcenaJeMaxObaKola = value; }
        }

        private bool tak2FinalnaOcenaJeProsekObaKola;
        public virtual bool Tak2FinalnaOcenaJeProsekObaKola
        {
            get { return tak2FinalnaOcenaJeProsekObaKola; }
            set { tak2FinalnaOcenaJeProsekObaKola = value; }
        }

        private bool tak2NeRacunajProsekAkoNemaOceneIzObaKola = true;
        public virtual bool Tak2NeRacunajProsekAkoNemaOceneIzObaKola
        {
            get { return tak2NeRacunajProsekAkoNemaOceneIzObaKola; }
            set { tak2NeRacunajProsekAkoNemaOceneIzObaKola = value; }
        }

        // Takmicenje 3

        private bool tak3FinalnaOcenaJeZbirObaKola;
        public virtual bool Tak3FinalnaOcenaJeZbirObaKola
        {
            get { return tak3FinalnaOcenaJeZbirObaKola; }
            set { tak3FinalnaOcenaJeZbirObaKola = value; }
        }

        private bool tak3FinalnaOcenaJeMaxObaKola;
        public virtual bool Tak3FinalnaOcenaJeMaxObaKola
        {
            get { return tak3FinalnaOcenaJeMaxObaKola; }
            set { tak3FinalnaOcenaJeMaxObaKola = value; }
        }

        private bool tak3FinalnaOcenaJeProsekObaKola;
        public virtual bool Tak3FinalnaOcenaJeProsekObaKola
        {
            get { return tak3FinalnaOcenaJeProsekObaKola; }
            set { tak3FinalnaOcenaJeProsekObaKola = value; }
        }

        private bool tak3NeRacunajProsekAkoNemaOceneIzObaKola = true;
        public virtual bool Tak3NeRacunajProsekAkoNemaOceneIzObaKola
        {
            get { return tak3NeRacunajProsekAkoNemaOceneIzObaKola; }
            set { tak3NeRacunajProsekAkoNemaOceneIzObaKola = value; }
        }

        // Takmicenje 4

        private bool tak4FinalnaOcenaJeZbirObaKola;
        public virtual bool Tak4FinalnaOcenaJeZbirObaKola
        {
            get { return tak4FinalnaOcenaJeZbirObaKola; }
            set { tak4FinalnaOcenaJeZbirObaKola = value; }
        }

        private bool tak4FinalnaOcenaJeMaxObaKola;
        public virtual bool Tak4FinalnaOcenaJeMaxObaKola
        {
            get { return tak4FinalnaOcenaJeMaxObaKola; }
            set { tak4FinalnaOcenaJeMaxObaKola = value; }
        }

        private bool tak4FinalnaOcenaJeProsekObaKola;
        public virtual bool Tak4FinalnaOcenaJeProsekObaKola
        {
            get { return tak4FinalnaOcenaJeProsekObaKola; }
            set { tak4FinalnaOcenaJeProsekObaKola = value; }
        }

        private bool tak4NeRacunajProsekAkoNemaOceneIzObaKola = true;
        public virtual bool Tak4NeRacunajProsekAkoNemaOceneIzObaKola
        {
            get { return tak4NeRacunajProsekAkoNemaOceneIzObaKola; }
            set { tak4NeRacunajProsekAkoNemaOceneIzObaKola = value; }
        }

        public virtual void validateTakmicenje2(Notification notification)
        {
            if (!PostojiTak2 || !OdvojenoTak2)
                return;

            if (BrojFinalistaTak2 < 1)
            {
                throw new BusinessException(
                    "BrojFinalistaTak2", "Neispravna vrednost za broj finalista.");
            }

            if (BrojRezerviTak2 < 1)
            {
                throw new BusinessException(
                    "BrojRezerviTak2", "Neispravna vrednost za broj rezervi.");
            }

            if (!NeogranicenBrojTakmicaraIzKlubaTak2 && MaxBrojTakmicaraIzKlubaTak2 < 1)
            {
                throw new BusinessException("MaxBrojTakmicaraIzKlubaTak2",
                    "Neispravna vrednost za maksimalan broj " +
                    "takmicara iz istog kluba/drzave.");
            }
        }

        public virtual void validateTakmicenje3(Notification notification)
        {
            if (!PostojiTak3 || !OdvojenoTak3)
                return;

            if (BrojFinalistaTak3 < 1)
            {
                throw new BusinessException(
                    "BrojFinalistaTak3", "Neispravna vrednost za broj finalista.");
            }

            if (BrojRezerviTak3 < 1)
            {
                throw new BusinessException(
                    "BrojRezerviTak3", "Neispravna vrednost za broj rezervi.");
            }

            if (!NeogranicenBrojTakmicaraIzKlubaTak3 && MaxBrojTakmicaraIzKlubaTak3 < 1)
            {
                throw new BusinessException("MaxBrojTakmicaraIzKlubaTak3",
                    "Neispravna vrednost za maksimalan broj " +
                    "takmicara iz istog kluba/drzave.");
            }
        }

        public virtual void validateTakmicenje4(Notification notification)
        {
            if (!PostojiTak4)
                return;

            if (BrojRezultataKojiSeBodujuZaEkipu < 1)
            {
                throw new BusinessException(
                    "BrojRezultataKojiSeBodujuZaEkipu", "Neispravna vrednost za broj rezultata koji se vrednuju za ekipu.");
            }

            if (odvojenoTak4 && BrojEkipaUFinalu < 1)
            {
                throw new BusinessException(
                    "BrojEkipaUFinalu", "Neispravna vrednost za broj ekipa u finalu.");
            }
        }

        public virtual void validateTakmicenje2FinaleKupa(Notification notification)
        {
            if (!PostojiTak2 || !OdvojenoTak2)
                return;

            if (BrojFinalistaTak2 < 1)
            {
                throw new BusinessException(
                    "BrojFinalistaTak2", "Neispravna vrednost za broj finalista.");
            }

            if (BrojRezerviTak2 < 1)
            {
                throw new BusinessException(
                    "BrojRezerviTak2", "Neispravna vrednost za broj rezervi.");
            }

            if (!NeogranicenBrojTakmicaraIzKlubaTak2 && MaxBrojTakmicaraIzKlubaTak2 < 1)
            {
                throw new BusinessException("MaxBrojTakmicaraIzKlubaTak2",
                    "Neispravna vrednost za maksimalan broj " +
                    "takmicara iz istog kluba/drzave.");
            }
        }

        public virtual void validateTakmicenje3FinaleKupa(Notification notification)
        {
            if (!PostojiTak3 || !OdvojenoTak3)
                return;

            if (BrojFinalistaTak3 < 1)
            {
                throw new BusinessException(
                    "BrojFinalistaTak3", "Neispravna vrednost za broj finalista.");
            }

            if (BrojRezerviTak3 < 1)
            {
                throw new BusinessException(
                    "BrojRezerviTak3", "Neispravna vrednost za broj rezervi.");
            }

            if (!NeogranicenBrojTakmicaraIzKlubaTak3 && MaxBrojTakmicaraIzKlubaTak3 < 1)
            {
                throw new BusinessException("MaxBrojTakmicaraIzKlubaTak3",
                    "Neispravna vrednost za maksimalan broj " +
                    "takmicara iz istog kluba/drzave.");
            }
        }

        public virtual void validateTakmicenje4FinaleKupa(Notification notification)
        {
            if (!PostojiTak4 || !OdvojenoTak4)
                return;

            if (BrojEkipaUFinalu < 1)
            {
                throw new BusinessException(
                    "BrojEkipaUFinalu", "Neispravna vrednost za broj ekipa u finalu.");
            }

            if (BrojRezultataKojiSeBodujuZaEkipu < 1)
            {
                throw new BusinessException(
                    "BrojRezultataKojiSeBodujuZaEkipu", "Neispravna vrednost za broj rezultata koji se vrednuju za ekipu.");
            }
        }

        public virtual void updateTakmicenje2(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak2 = this.PostojiTak2;
                p.OdvojenoTak2 = this.OdvojenoTak2;
                p.BrojFinalistaTak2 = this.BrojFinalistaTak2;
                p.BrojRezerviTak2 = this.BrojRezerviTak2;
                p.NeogranicenBrojTakmicaraIzKlubaTak2 = this.NeogranicenBrojTakmicaraIzKlubaTak2;
                p.MaxBrojTakmicaraIzKlubaTak2 = this.MaxBrojTakmicaraIzKlubaTak2;
            }
        }

        public virtual void updateTakmicenje3(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak3 = this.PostojiTak3;
                p.OdvojenoTak3 = this.OdvojenoTak3;
                p.NeogranicenBrojTakmicaraIzKlubaTak3 = this.NeogranicenBrojTakmicaraIzKlubaTak3;
                p.MaxBrojTakmicaraIzKlubaTak3 = this.MaxBrojTakmicaraIzKlubaTak3;
                p.MaxBrojTakmicaraTak3VaziZaDrzavu = this.MaxBrojTakmicaraTak3VaziZaDrzavu;
                p.BrojFinalistaTak3 = this.BrojFinalistaTak3;
                p.BrojRezerviTak3 = this.BrojRezerviTak3;
                p.KvalifikantiTak3PreskokNaOsnovuObaPreskoka = this.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                p.PoredakTak3PreskokNaOsnovuObaPreskoka = this.PoredakTak3PreskokNaOsnovuObaPreskoka;
            }
        }

        public virtual void updateTakmicenje4(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak4 = this.PostojiTak4;
                p.OdvojenoTak4 = this.OdvojenoTak4;
                p.JednoTak4ZaSveKategorije = this.JednoTak4ZaSveKategorije;
                p.BrojRezultataKojiSeBodujuZaEkipu = this.BrojRezultataKojiSeBodujuZaEkipu;
                p.BrojEkipaUFinalu = this.BrojEkipaUFinalu;
            }
        }

        public virtual void updateTakmicenje2FinaleKupa(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak2 = this.PostojiTak2;
                p.OdvojenoTak2 = this.OdvojenoTak2;
                p.Tak2FinalnaOcenaJeMaxObaKola = this.Tak2FinalnaOcenaJeMaxObaKola;
                p.Tak2FinalnaOcenaJeZbirObaKola = this.Tak2FinalnaOcenaJeZbirObaKola;
                p.Tak2FinalnaOcenaJeProsekObaKola = this.Tak2FinalnaOcenaJeProsekObaKola;
                p.Tak2NeRacunajProsekAkoNemaOceneIzObaKola = this.Tak2NeRacunajProsekAkoNemaOceneIzObaKola;
                p.BrojFinalistaTak2 = this.BrojFinalistaTak2;
                p.BrojRezerviTak2 = this.BrojRezerviTak2;
                p.NeogranicenBrojTakmicaraIzKlubaTak2 = this.NeogranicenBrojTakmicaraIzKlubaTak2;
                p.MaxBrojTakmicaraIzKlubaTak2 = this.MaxBrojTakmicaraIzKlubaTak2;
            }
        }

        public virtual void updateTakmicenje3FinaleKupa(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak3 = this.PostojiTak3;
                p.OdvojenoTak3 = this.OdvojenoTak3;
                p.Tak3FinalnaOcenaJeMaxObaKola = this.Tak3FinalnaOcenaJeMaxObaKola;
                p.Tak3FinalnaOcenaJeZbirObaKola = this.Tak3FinalnaOcenaJeZbirObaKola;
                p.Tak3FinalnaOcenaJeProsekObaKola = this.Tak3FinalnaOcenaJeProsekObaKola;
                p.Tak3NeRacunajProsekAkoNemaOceneIzObaKola = this.Tak3NeRacunajProsekAkoNemaOceneIzObaKola;
                p.BrojFinalistaTak3 = this.BrojFinalistaTak3;
                p.BrojRezerviTak3 = this.BrojRezerviTak3;
                p.NeogranicenBrojTakmicaraIzKlubaTak3 = this.NeogranicenBrojTakmicaraIzKlubaTak3;
                p.MaxBrojTakmicaraIzKlubaTak3 = this.MaxBrojTakmicaraIzKlubaTak3;
                p.MaxBrojTakmicaraTak3VaziZaDrzavu = this.MaxBrojTakmicaraTak3VaziZaDrzavu;
                p.PoredakTak3PreskokNaOsnovuObaPreskoka = this.PoredakTak3PreskokNaOsnovuObaPreskoka;
            }
        }

        public virtual void updateTakmicenje4FinaleKupa(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak4 = this.PostojiTak4;
                p.OdvojenoTak4 = this.OdvojenoTak4;
                p.JednoTak4ZaSveKategorije = this.JednoTak4ZaSveKategorije;
                p.Tak4FinalnaOcenaJeMaxObaKola = this.Tak4FinalnaOcenaJeMaxObaKola;
                p.Tak4FinalnaOcenaJeZbirObaKola = this.Tak4FinalnaOcenaJeZbirObaKola;
                p.Tak4FinalnaOcenaJeProsekObaKola = this.Tak4FinalnaOcenaJeProsekObaKola;
                p.Tak4NeRacunajProsekAkoNemaOceneIzObaKola = this.Tak4NeRacunajProsekAkoNemaOceneIzObaKola;
                p.BrojEkipaUFinalu = this.BrojEkipaUFinalu;
                p.BrojRezultataKojiSeBodujuZaEkipu = this.BrojRezultataKojiSeBodujuZaEkipu;
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            
            strBuilder.AppendLine(PostojiTak2.ToString());
            strBuilder.AppendLine(OdvojenoTak2.ToString());
            strBuilder.AppendLine(NeogranicenBrojTakmicaraIzKlubaTak2.ToString());
            strBuilder.AppendLine(MaxBrojTakmicaraIzKlubaTak2.ToString());
            strBuilder.AppendLine(BrojFinalistaTak2.ToString());
            strBuilder.AppendLine(BrojRezerviTak2.ToString());

            strBuilder.AppendLine(PostojiTak3.ToString());
            strBuilder.AppendLine(OdvojenoTak3.ToString());
            strBuilder.AppendLine(NeogranicenBrojTakmicaraIzKlubaTak3.ToString());
            strBuilder.AppendLine(MaxBrojTakmicaraIzKlubaTak3.ToString());
            strBuilder.AppendLine(MaxBrojTakmicaraTak3VaziZaDrzavu.ToString());
            strBuilder.AppendLine(BrojFinalistaTak3.ToString());
            strBuilder.AppendLine(BrojRezerviTak3.ToString());
            strBuilder.AppendLine(KvalifikantiTak3PreskokNaOsnovuObaPreskoka.ToString());
            strBuilder.AppendLine(PoredakTak3PreskokNaOsnovuObaPreskoka.ToString());

            strBuilder.AppendLine(PostojiTak4.ToString());
            strBuilder.AppendLine(OdvojenoTak4.ToString());
            strBuilder.AppendLine(BrojRezultataKojiSeBodujuZaEkipu.ToString());
            strBuilder.AppendLine(BrojEkipaUFinalu.ToString());
            strBuilder.AppendLine(JednoTak4ZaSveKategorije.ToString());

            strBuilder.AppendLine(Tak2FinalnaOcenaJeZbirObaKola.ToString());
            strBuilder.AppendLine(Tak2FinalnaOcenaJeMaxObaKola.ToString());
            strBuilder.AppendLine(Tak2FinalnaOcenaJeProsekObaKola.ToString());
            strBuilder.AppendLine(Tak2NeRacunajProsekAkoNemaOceneIzObaKola.ToString());

            strBuilder.AppendLine(Tak3FinalnaOcenaJeZbirObaKola.ToString());
            strBuilder.AppendLine(Tak3FinalnaOcenaJeMaxObaKola.ToString());
            strBuilder.AppendLine(Tak3FinalnaOcenaJeProsekObaKola.ToString());
            strBuilder.AppendLine(Tak3NeRacunajProsekAkoNemaOceneIzObaKola.ToString());

            strBuilder.AppendLine(Tak4FinalnaOcenaJeZbirObaKola.ToString());
            strBuilder.AppendLine(Tak4FinalnaOcenaJeMaxObaKola.ToString());
            strBuilder.AppendLine(Tak4FinalnaOcenaJeProsekObaKola.ToString());
            strBuilder.AppendLine(Tak4NeRacunajProsekAkoNemaOceneIzObaKola.ToString());
        }
    }
}
