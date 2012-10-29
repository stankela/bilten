using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class Propozicije : DomainObject
    {
        private byte maxBrojTakmicaraIzKlubaTak1;
        public virtual byte MaxBrojTakmicaraIzKlubaTak1
        {
            get { return maxBrojTakmicaraIzKlubaTak1; }
            set { maxBrojTakmicaraIzKlubaTak1 = value; }
        }

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


    }
}
