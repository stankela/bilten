using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Bilten.Util;
using Bilten.Exceptions;
using System.IO;

namespace Bilten.Domain
{
    public class Propozicije : DomainObject
    {
        // TODO4: Razmisli da uklonis svojstva PostojiTak2 i PostojiTak3, tj. neka ona uvek budu true.

        private bool postojiTak2;
        public virtual bool PostojiTak2
        {
            get { return postojiTak2; }
            set { postojiTak2 = value; }
        }

        private bool _odvojenoTak2;
        public virtual bool OdvojenoTak2
        {
            get { return _odvojenoTak2; }
            set { _odvojenoTak2 = value; }
        }

        private bool zaPreskokVisebojRacunajBoljuOcenu;
        public virtual bool ZaPreskokVisebojRacunajBoljuOcenu
        {
            get { return zaPreskokVisebojRacunajBoljuOcenu; }
            set { zaPreskokVisebojRacunajBoljuOcenu = value; }
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

        private bool _odvojenoTak3;
        public virtual bool OdvojenoTak3
        {
            get { return _odvojenoTak3; }
            set { _odvojenoTak3 = value; }
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

        private bool tak1PreskokNaOsnovuObaPreskoka;
        public virtual bool Tak1PreskokNaOsnovuObaPreskoka
        {
            get { return tak1PreskokNaOsnovuObaPreskoka; }
            set { tak1PreskokNaOsnovuObaPreskoka = value; }
        }

        private bool tak3PreskokNaOsnovuObaPreskoka;
        public virtual bool Tak3PreskokNaOsnovuObaPreskoka
        {
            get { return tak3PreskokNaOsnovuObaPreskoka; }
            set { tak3PreskokNaOsnovuObaPreskoka = value; }
        }

        private bool postojiTak4;
        public virtual bool PostojiTak4
        {
            get { return postojiTak4; }
            set { postojiTak4 = value; }
        }

        private bool _odvojenoTak4;
        public virtual bool OdvojenoTak4
        {
            get { return _odvojenoTak4; }
            set { _odvojenoTak4 = value; }
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

        public virtual bool racunajObaPreskoka(DeoTakmicenjaKod deoTakKod)
        {
            if (!PostojiTak3)
                return false;

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Tak1PreskokNaOsnovuObaPreskoka;
            return Tak3PreskokNaOsnovuObaPreskoka;
        }

        // Finale kupa

        // Takmicenje 2

        private NacinRacunanjaOceneFinaleKupa nacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.Zbir;
        public virtual NacinRacunanjaOceneFinaleKupa NacinRacunanjaOceneFinaleKupaTak2
        {
            get { return nacinRacunanjaOceneFinaleKupaTak2; }
            set { nacinRacunanjaOceneFinaleKupaTak2 = value; }
        }

        // TODO4: Ukloni sledeca 4 svojstva nakon sto izvrsis apdejt na verziju 5, iz .hbm fajlova i iz baze. Isto
        // i za takmicenja 3 i 4.
        private bool tak2FinalnaOcenaJeZbirObaKola = true;
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

        private NacinRacunanjaOceneFinaleKupa nacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.Max;
        public virtual NacinRacunanjaOceneFinaleKupa NacinRacunanjaOceneFinaleKupaTak3
        {
            get { return nacinRacunanjaOceneFinaleKupaTak3; }
            set { nacinRacunanjaOceneFinaleKupaTak3 = value; }
        }

        private bool tak3FinalnaOcenaJeZbirObaKola;
        public virtual bool Tak3FinalnaOcenaJeZbirObaKola
        {
            get { return tak3FinalnaOcenaJeZbirObaKola; }
            set { tak3FinalnaOcenaJeZbirObaKola = value; }
        }

        private bool tak3FinalnaOcenaJeMaxObaKola = true;
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

        private NacinRacunanjaOceneFinaleKupa nacinRacunanjaOceneFinaleKupaTak4 = NacinRacunanjaOceneFinaleKupa.Zbir;
        public virtual NacinRacunanjaOceneFinaleKupa NacinRacunanjaOceneFinaleKupaTak4
        {
            get { return nacinRacunanjaOceneFinaleKupaTak4; }
            set { nacinRacunanjaOceneFinaleKupaTak4 = value; }
        }

        private bool tak4FinalnaOcenaJeZbirObaKola = true;
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

        public virtual bool odvojenoTak2()
        {
            return PostojiTak2 && OdvojenoTak2;
        }

        public virtual bool odvojenoTak3()
        {
            return PostojiTak3 && OdvojenoTak3;
        }

        public virtual bool odvojenoTak4()
        {
            return PostojiTak4 && OdvojenoTak4;
        }

        public virtual void validateTakmicenje2(Notification notification)
        {
            if (!odvojenoTak2())
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
            if (!odvojenoTak3())
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

            if (OdvojenoTak4 && BrojEkipaUFinalu < 1)
            {
                throw new BusinessException(
                    "BrojEkipaUFinalu", "Neispravna vrednost za broj ekipa u finalu.");
            }
        }

        public virtual void validateTakmicenje2FinaleKupa(Notification notification)
        {
            if (!odvojenoTak2())
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
            if (!odvojenoTak3())
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
            if (!odvojenoTak4())
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

        public virtual void copyTakmicenje2To(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak2 = this.PostojiTak2;
                p.OdvojenoTak2 = this.OdvojenoTak2;
                p.ZaPreskokVisebojRacunajBoljuOcenu = this.ZaPreskokVisebojRacunajBoljuOcenu;
                p.BrojFinalistaTak2 = this.BrojFinalistaTak2;
                p.BrojRezerviTak2 = this.BrojRezerviTak2;
                p.NeogranicenBrojTakmicaraIzKlubaTak2 = this.NeogranicenBrojTakmicaraIzKlubaTak2;
                p.MaxBrojTakmicaraIzKlubaTak2 = this.MaxBrojTakmicaraIzKlubaTak2;
            }
        }

        public virtual void copyTakmicenje3To(IList<Propozicije> propozicije)
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
                p.Tak1PreskokNaOsnovuObaPreskoka = this.Tak1PreskokNaOsnovuObaPreskoka;
                p.Tak3PreskokNaOsnovuObaPreskoka = this.Tak3PreskokNaOsnovuObaPreskoka;
            }
        }

        // TODO: I za ostale propozicije (takmicenje 2, takmicenje 3, ...) kreiraj dva metoda - jedan sto kao argument
        // dobija object i jedan sto dobija listu.
        public virtual void copyTakmicenje4To(Propozicije p)
        {
            p.PostojiTak4 = this.PostojiTak4;
            p.OdvojenoTak4 = this.OdvojenoTak4;
            p.JednoTak4ZaSveKategorije = this.JednoTak4ZaSveKategorije;
            p.BrojRezultataKojiSeBodujuZaEkipu = this.BrojRezultataKojiSeBodujuZaEkipu;
            p.BrojEkipaUFinalu = this.BrojEkipaUFinalu;
        }

        public virtual void copyTakmicenje4To(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;
            foreach (Propozicije p in propozicije)
                copyTakmicenje4To(p);
        }

        public virtual void copyTakmicenje2FinaleKupaTo(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                // TODO: Da li ovde treba kopirati ZaPreskokVisebojRacunajBoljuOcenu. I generalno, za obicno odvojeno
                // takmicenje 2 (tj. koje nije finale kupa), da li mi treba posebno svojstvo kako se racuna
                // preskok za odvojeno finale.
                p.PostojiTak2 = this.PostojiTak2;
                p.OdvojenoTak2 = this.OdvojenoTak2;
                p.NacinRacunanjaOceneFinaleKupaTak2 = this.NacinRacunanjaOceneFinaleKupaTak2;
                p.BrojFinalistaTak2 = this.BrojFinalistaTak2;
                p.BrojRezerviTak2 = this.BrojRezerviTak2;
                p.NeogranicenBrojTakmicaraIzKlubaTak2 = this.NeogranicenBrojTakmicaraIzKlubaTak2;
                p.MaxBrojTakmicaraIzKlubaTak2 = this.MaxBrojTakmicaraIzKlubaTak2;
            }
        }

        public virtual void copyTakmicenje3FinaleKupaTo(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak3 = this.PostojiTak3;
                p.OdvojenoTak3 = this.OdvojenoTak3;
                p.NacinRacunanjaOceneFinaleKupaTak3 = this.NacinRacunanjaOceneFinaleKupaTak3;
                p.BrojFinalistaTak3 = this.BrojFinalistaTak3;
                p.BrojRezerviTak3 = this.BrojRezerviTak3;
                p.NeogranicenBrojTakmicaraIzKlubaTak3 = this.NeogranicenBrojTakmicaraIzKlubaTak3;
                p.MaxBrojTakmicaraIzKlubaTak3 = this.MaxBrojTakmicaraIzKlubaTak3;
                p.MaxBrojTakmicaraTak3VaziZaDrzavu = this.MaxBrojTakmicaraTak3VaziZaDrzavu;
                p.Tak1PreskokNaOsnovuObaPreskoka = this.Tak1PreskokNaOsnovuObaPreskoka;
            }
        }

        // TODO3: Probaj da koristis refleksiju za ovo (ili da ona izvrsi kopiranje, ili samo da te obavesti da li je
        // u medjuvremenu u klasi Propozicije dodato neko novo svojstvo, i ako jeste da generise izuzetak. Mogao bi i 
        // da generisem jednostavan test suite koji bi proveravao ovo)
        // TODO3: Uvedi komentar TODO9 za ono sto mora uvek da se proverava kada se menja program (kao naprimer ovde sto
        // mora da se proverava da li sam u medjuvremenu dodao novo svojstvo u klasu Propozicije.)
        
        public virtual void copyTakmicenje4FinaleKupaTo(IList<Propozicije> propozicije)
        {
            if (propozicije == null)
                return;

            foreach (Propozicije p in propozicije)
            {
                p.PostojiTak4 = this.PostojiTak4;
                p.OdvojenoTak4 = this.OdvojenoTak4;
                p.JednoTak4ZaSveKategorije = this.JednoTak4ZaSveKategorije;
                p.NacinRacunanjaOceneFinaleKupaTak4 = this.NacinRacunanjaOceneFinaleKupaTak4;
                p.BrojEkipaUFinalu = this.BrojEkipaUFinalu;
                p.BrojRezultataKojiSeBodujuZaEkipu = this.BrojRezultataKojiSeBodujuZaEkipu;
            }
        }

        public virtual Propozicije clonePropozicije()
        {
            Propozicije result = new Propozicije();
            result.PostojiTak2 = this.PostojiTak2;
            result.OdvojenoTak2 = this.OdvojenoTak2;
            result.ZaPreskokVisebojRacunajBoljuOcenu = this.ZaPreskokVisebojRacunajBoljuOcenu;
            result.NeogranicenBrojTakmicaraIzKlubaTak2 = this.NeogranicenBrojTakmicaraIzKlubaTak2;
            result.MaxBrojTakmicaraIzKlubaTak2 = this.MaxBrojTakmicaraIzKlubaTak2;
            result.BrojFinalistaTak2 = this.BrojFinalistaTak2;
            result.BrojRezerviTak2 = this.BrojRezerviTak2;

            result.PostojiTak3 = this.PostojiTak3;
            result.OdvojenoTak3 = this.OdvojenoTak3;
            result.NeogranicenBrojTakmicaraIzKlubaTak3 = this.NeogranicenBrojTakmicaraIzKlubaTak3;
            result.MaxBrojTakmicaraIzKlubaTak3 = this.MaxBrojTakmicaraIzKlubaTak3;
            result.MaxBrojTakmicaraTak3VaziZaDrzavu = this.MaxBrojTakmicaraTak3VaziZaDrzavu;
            result.BrojFinalistaTak3 = this.BrojFinalistaTak3;
            result.BrojRezerviTak3 = this.BrojRezerviTak3;
            result.Tak1PreskokNaOsnovuObaPreskoka = this.Tak1PreskokNaOsnovuObaPreskoka;
            result.Tak3PreskokNaOsnovuObaPreskoka = this.Tak3PreskokNaOsnovuObaPreskoka;

            result.PostojiTak4 = this.PostojiTak4;
            result.OdvojenoTak4 = this.OdvojenoTak4;
            result.BrojRezultataKojiSeBodujuZaEkipu = this.BrojRezultataKojiSeBodujuZaEkipu;
            result.BrojEkipaUFinalu = this.BrojEkipaUFinalu;
            result.JednoTak4ZaSveKategorije = this.JednoTak4ZaSveKategorije;

            result.NacinRacunanjaOceneFinaleKupaTak2 = this.NacinRacunanjaOceneFinaleKupaTak2;
            result.NacinRacunanjaOceneFinaleKupaTak3 = this.NacinRacunanjaOceneFinaleKupaTak3;
            result.NacinRacunanjaOceneFinaleKupaTak4 = this.NacinRacunanjaOceneFinaleKupaTak4;
            return result;
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is Propozicije)) return false;
            Propozicije that = (Propozicije)other;

            return this.PostojiTak2 == that.PostojiTak2
            && this.OdvojenoTak2 == that.OdvojenoTak2
            && this.ZaPreskokVisebojRacunajBoljuOcenu == that.ZaPreskokVisebojRacunajBoljuOcenu
            && this.NeogranicenBrojTakmicaraIzKlubaTak2 == that.NeogranicenBrojTakmicaraIzKlubaTak2
            && this.MaxBrojTakmicaraIzKlubaTak2 == that.MaxBrojTakmicaraIzKlubaTak2
            && this.BrojFinalistaTak2 == that.BrojFinalistaTak2
            && this.BrojRezerviTak2 == that.BrojRezerviTak2

            && this.PostojiTak3 == that.PostojiTak3
            && this.OdvojenoTak3 == that.OdvojenoTak3
            && this.NeogranicenBrojTakmicaraIzKlubaTak3 == that.NeogranicenBrojTakmicaraIzKlubaTak3
            && this.MaxBrojTakmicaraIzKlubaTak3 == that.MaxBrojTakmicaraIzKlubaTak3
            && this.MaxBrojTakmicaraTak3VaziZaDrzavu == that.MaxBrojTakmicaraTak3VaziZaDrzavu
            && this.BrojFinalistaTak3 == that.BrojFinalistaTak3
            && this.BrojRezerviTak3 == that.BrojRezerviTak3
            && this.Tak1PreskokNaOsnovuObaPreskoka == that.Tak1PreskokNaOsnovuObaPreskoka
            && this.Tak3PreskokNaOsnovuObaPreskoka == that.Tak3PreskokNaOsnovuObaPreskoka

            && this.PostojiTak4 == that.PostojiTak4
            && this.OdvojenoTak4 == that.OdvojenoTak4
            && this.BrojRezultataKojiSeBodujuZaEkipu == that.BrojRezultataKojiSeBodujuZaEkipu
            && this.BrojEkipaUFinalu == that.BrojEkipaUFinalu
            && this.JednoTak4ZaSveKategorije == that.JednoTak4ZaSveKategorije

            && this.NacinRacunanjaOceneFinaleKupaTak2 == that.NacinRacunanjaOceneFinaleKupaTak2
            && this.NacinRacunanjaOceneFinaleKupaTak3 == that.NacinRacunanjaOceneFinaleKupaTak3
            && this.NacinRacunanjaOceneFinaleKupaTak4 == that.NacinRacunanjaOceneFinaleKupaTak4;
        }
        
        public override int GetHashCode()
        {
            throw new Exception("TODO: Da li je ovo potrebno?");
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            
            strBuilder.AppendLine(PostojiTak2.ToString());
            strBuilder.AppendLine(OdvojenoTak2.ToString());
            strBuilder.AppendLine(ZaPreskokVisebojRacunajBoljuOcenu.ToString());
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
            strBuilder.AppendLine(Tak1PreskokNaOsnovuObaPreskoka.ToString());
            strBuilder.AppendLine(Tak3PreskokNaOsnovuObaPreskoka.ToString());

            strBuilder.AppendLine(PostojiTak4.ToString());
            strBuilder.AppendLine(OdvojenoTak4.ToString());
            strBuilder.AppendLine(BrojRezultataKojiSeBodujuZaEkipu.ToString());
            strBuilder.AppendLine(BrojEkipaUFinalu.ToString());
            strBuilder.AppendLine(JednoTak4ZaSveKategorije.ToString());

            strBuilder.AppendLine(NacinRacunanjaOceneFinaleKupaTak2.ToString());
            strBuilder.AppendLine(NacinRacunanjaOceneFinaleKupaTak3.ToString());
            strBuilder.AppendLine(NacinRacunanjaOceneFinaleKupaTak4.ToString());
        }

        public virtual void loadFromDump(StringReader reader)
        {
            PostojiTak2 = bool.Parse(reader.ReadLine());
            OdvojenoTak2 = bool.Parse(reader.ReadLine());
            ZaPreskokVisebojRacunajBoljuOcenu = bool.Parse(reader.ReadLine());
            NeogranicenBrojTakmicaraIzKlubaTak2 = bool.Parse(reader.ReadLine());
            MaxBrojTakmicaraIzKlubaTak2 = byte.Parse(reader.ReadLine());
            BrojFinalistaTak2 = byte.Parse(reader.ReadLine());
            BrojRezerviTak2 = byte.Parse(reader.ReadLine());

            PostojiTak3 = bool.Parse(reader.ReadLine());
            OdvojenoTak3 = bool.Parse(reader.ReadLine());
            NeogranicenBrojTakmicaraIzKlubaTak3 = bool.Parse(reader.ReadLine());
            MaxBrojTakmicaraIzKlubaTak3 = byte.Parse(reader.ReadLine());
            MaxBrojTakmicaraTak3VaziZaDrzavu = bool.Parse(reader.ReadLine());
            BrojFinalistaTak3 = byte.Parse(reader.ReadLine());
            BrojRezerviTak3 = byte.Parse(reader.ReadLine());
            Tak1PreskokNaOsnovuObaPreskoka = bool.Parse(reader.ReadLine());
            Tak3PreskokNaOsnovuObaPreskoka = bool.Parse(reader.ReadLine());

            PostojiTak4 = bool.Parse(reader.ReadLine());
            OdvojenoTak4 = bool.Parse(reader.ReadLine());
            BrojRezultataKojiSeBodujuZaEkipu = byte.Parse(reader.ReadLine());
            BrojEkipaUFinalu = byte.Parse(reader.ReadLine());
            JednoTak4ZaSveKategorije = bool.Parse(reader.ReadLine());

            NacinRacunanjaOceneFinaleKupaTak2
                = (NacinRacunanjaOceneFinaleKupa)Enum.Parse(typeof(NacinRacunanjaOceneFinaleKupa), reader.ReadLine());
            NacinRacunanjaOceneFinaleKupaTak3
                = (NacinRacunanjaOceneFinaleKupa)Enum.Parse(typeof(NacinRacunanjaOceneFinaleKupa), reader.ReadLine());
            NacinRacunanjaOceneFinaleKupaTak4
                = (NacinRacunanjaOceneFinaleKupa)Enum.Parse(typeof(NacinRacunanjaOceneFinaleKupa), reader.ReadLine());
        }
    }
}
