using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Exceptions;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Bilten.Domain
{
    public class RezultatskoTakmicenje : DomainObject
    {
        private byte redBroj;
        public virtual byte RedBroj
        {
            get { return redBroj; }
            set { redBroj = value; }
        }

        private Gimnastika gimnastika;
        public virtual Gimnastika Gimnastika
        {
            get { return gimnastika; }
            set { gimnastika = value; }
        }

        // TODO: Ukloni ovaj hack, tj. sledeca dva svojstva. Kreiraj klasu 
        // EkipnoTakmicenje, koja ce biti zaduzena za Takmicenja 1 i 4 u ekipnoj 
        // konkurenciji). Ukloniti takodje i svojstvo JednoTak4ZaSveKategorije 
        // klase Propozicije.

        private bool imaEkipnoTakmicenje = true;
        public virtual bool ImaEkipnoTakmicenje
        {
            get { return imaEkipnoTakmicenje; }
            set { imaEkipnoTakmicenje = value; }
        }

        // kada postoji samo jedno ekipno takmicenje za vise kategorija, ovo svojstvo
        // ce biti true samo za jedno RezultatskoTakmicenje.
        private bool kombinovanoEkipnoTak;
        public virtual bool KombinovanoEkipnoTak
        {
            get { return kombinovanoEkipnoTak; }
            set { kombinovanoEkipnoTak = value; }
        }

        private Takmicenje takmicenje;
        public virtual Takmicenje Takmicenje
        {
            get { return takmicenje; }
            set { takmicenje = value; }
        }

        private TakmicarskaKategorija kategorija;
        public virtual TakmicarskaKategorija Kategorija
        {
            get { return kategorija; }
            set { kategorija = value; }
        }

        private RezultatskoTakmicenjeDescription description;
        public virtual RezultatskoTakmicenjeDescription TakmicenjeDescription
        {
            get { return description; }
            set { description = value; }
        }

        private Propozicije propozicije;
        public virtual Propozicije Propozicije
        {
            get { return propozicije; }
            set { propozicije = value; }
        }

        private Takmicenje1 _takmicenje1;
        public virtual Takmicenje1 Takmicenje1
        {
            get { return _takmicenje1; }
            protected set { _takmicenje1 = value; }
        }

        private Takmicenje2 _takmicenje2;
        public virtual Takmicenje2 Takmicenje2
        {
            get { return _takmicenje2; }
            protected set { _takmicenje2 = value; }
        }

        private Takmicenje3 _takmicenje3;
        public virtual Takmicenje3 Takmicenje3
        {
            get { return _takmicenje3; }
            protected set { _takmicenje3 = value; }
        }

        private Takmicenje4 _takmicenje4;
        public virtual Takmicenje4 Takmicenje4
        {
            get { return _takmicenje4; }
            protected set { _takmicenje4 = value; }
        }

        public RezultatskoTakmicenje()
        { 
            // NOTE, TODO: Ovaj konstruktor nije za public upotrebu. Trebao je da bude
            // protected ali tada ne bi mogao da koristim RezultatskoTakmicenje kao
            // genericki parametar. Za kreiranje je potrebno koristiti donji 
            // konstruktor. Proveri da li je tako svugde u programu. 
        }

        public RezultatskoTakmicenje(Takmicenje takmicenje, TakmicarskaKategorija
            kategorija, RezultatskoTakmicenjeDescription desc, Propozicije propozicije)
        {
            this.takmicenje = takmicenje;
            this.kategorija = kategorija;
            this.description = desc;
            this.propozicije = propozicije;
            this.gimnastika = takmicenje.Gimnastika;

            Takmicenje1 = new Takmicenje1(takmicenje);
            Takmicenje2 = new Takmicenje2();
            Takmicenje3 = new Takmicenje3(Gimnastika);
            Takmicenje4 = new Takmicenje4();
        }

        public virtual void createTakmicenje2(IList<Ocena> oceneTak2)
        {
            Takmicenje2.createUcesnici(Takmicenje1);

            // Ako ne postoje ocene, sledeci poziv samo sortira po prezimenu i na osnovu toga dodeljuje RedBroj
            Takmicenje2.Poredak.create(this, oceneTak2);
        }

        public virtual void createTakmicenje3(IList<Ocena> oceneTak3)
        {
            Takmicenje3.createUcesnici(Takmicenje1, Propozicije.Tak1PreskokNaOsnovuObaPreskoka);

            // Ako ne postoje ocene, sledeci poziv samo sortira po prezimenu i na osnovu toga dodeljuje RedBroj
            foreach (PoredakSprava p in Takmicenje3.Poredak)
                p.create(this, oceneTak3);
            Takmicenje3.PoredakPreskok.create(this, oceneTak3);
        }

        public virtual void createTakmicenje4(IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap)
        {
            Takmicenje4.createUcesnici(Takmicenje1);
            Takmicenje4.Poredak.create(this, ekipaRezultatiUkupnoMap);
        }

        public virtual string Naziv
        {
            get 
            {
                string result = String.Empty;
                if (Kategorija != null)
                    result += Kategorija.Naziv;
                if (TakmicenjeDescription != null)
                {
                    if (result != String.Empty)
                        result += " - ";
                    result += TakmicenjeDescription.Naziv;
                }
                return result; 
            }
        }

        public override string ToString()
        {
            return Naziv;
        }

        public virtual string NazivEkipnog
        {
            get
            {
                string result = String.Empty;

                if (ImaEkipnoTakmicenje)
                {
                    if (!KombinovanoEkipnoTak)
                        return Naziv;
                    else
                    {
                        if (TakmicenjeDescription != null)
                            return TakmicenjeDescription.Naziv;
                    }
                }

                return result;
            }
        }

        // TODO4: Izbaci nepotrebne stvari iz Equals i GetHashCode (Gimnastika i Takmicenje)
        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is RezultatskoTakmicenje)) return false;

            RezultatskoTakmicenje that = (RezultatskoTakmicenje)other;
            return (this.Gimnastika == that.Gimnastika
                && this.Takmicenje.Equals(that.Takmicenje)
                && this.Kategorija.Equals(that.Kategorija)
                && this.TakmicenjeDescription.Equals(that.TakmicenjeDescription));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Gimnastika.GetHashCode();
                result = 29 * result + Takmicenje.GetHashCode();
                result = 29 * result + Kategorija.GetHashCode();
                result = 29 * result + TakmicenjeDescription.GetHashCode();
                return result;
            }
        }
        
        public virtual PoredakUkupno getPoredakUkupno(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.PoredakUkupno;
            
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje2);
            return Takmicenje2.Poredak;
        }

        public virtual PoredakSprava getPoredakSprava(DeoTakmicenjaKod deoTakKod, Sprava sprava)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.getPoredakSprava(sprava);

            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje3);
            return Takmicenje3.getPoredak(sprava);
        }

        public virtual PoredakPreskok getPoredakPreskok(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.PoredakPreskok;

            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje3);
            return Takmicenje3.PoredakPreskok;
        }

        public virtual PoredakEkipno getPoredakEkipno(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.PoredakEkipno;

            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje4);
            return Takmicenje4.Poredak;
        }

        public virtual bool odvojenoTak2()
        {
            return Propozicije.odvojenoTak2();
        }

        public virtual bool odvojenoTak3()
        {
            return Propozicije.PostojiTak3 && Propozicije.OdvojenoTak3;
        }

        public virtual bool odvojenoTak4()
        {
            return Propozicije.PostojiTak4 && Propozicije.OdvojenoTak4;
        }

        public virtual string getNazivIzvestajaViseboj(DeoTakmicenjaKod deoTakKod, bool finaleKupa, bool sumaObaKola)
        {
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje1 || deoTakKod == DeoTakmicenjaKod.Takmicenje2);

            string result = String.Empty;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (finaleKupa)
                {
                    if (sumaObaKola)
                        result = "I i II kolo - Rezultati vi" + Jezik.shMalo + "eboj";
                    else
                        result = "Vi" + Jezik.shMalo + "eboj";
                }
                else
                {
                    if (Propozicije.OdvojenoTak2)
                        result = "Kvalifikacije za finale vi" + Jezik.shMalo + "eboja";
                    else
                        result = Opcije.Instance.Viseboj;
                }
            }
            else
            {
                result = "Finale vi" + Jezik.shMalo + "eboja";
            }
            return result;
        }

        public virtual string getNazivIzvestajaSprava(DeoTakmicenjaKod deoTakKod, bool finaleKupa, bool sumaObaKola)
        {
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje1 || deoTakKod == DeoTakmicenjaKod.Takmicenje3);

            string result = String.Empty;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (finaleKupa)
                {
                    if (sumaObaKola)
                        result = "I i II kolo - Rezultati po spravama";
                    else
                        result = "Finale po spravama";
                }
                else
                {
                    if (Propozicije.OdvojenoTak3)
                        result = Opcije.Instance.KvalSprave;
                    else
                        result = Opcije.Instance.FinaleSprave;
                }
            }
            else
            {
                result = Opcije.Instance.FinaleSprave;
            }
            return result;
        }

        public static void updateImaEkipnoTakmicenje(IList<RezultatskoTakmicenje> rezTakmicenja,
            RezultatskoTakmicenjeDescription desc)
        {
            List<RezultatskoTakmicenje> rezTakDesc = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rt.TakmicenjeDescription.Equals(desc))
                    rezTakDesc.Add(rt);
            }
            bool kombAdded = false;
            foreach (RezultatskoTakmicenje rt in rezTakDesc)
            {
                if (!rt.TakmicenjeDescription.Propozicije.JednoTak4ZaSveKategorije)
                {
                    rt.ImaEkipnoTakmicenje = true;
                    rt.KombinovanoEkipnoTak = false;
                }
                else
                {
                    if (!kombAdded)
                    {
                        rt.ImaEkipnoTakmicenje = true;
                        rt.KombinovanoEkipnoTak = true;
                        kombAdded = true;
                    }
                    else
                    {
                        rt.ImaEkipnoTakmicenje = false;
                        rt.KombinovanoEkipnoTak = false;
                    }
                }
            }
        }

        public virtual Ekipa findEkipa(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod)
        {
            if (!ImaEkipnoTakmicenje)
                return null;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                foreach (Ekipa e in Takmicenje1.Ekipe)
                {
                    if (e.Gimnasticari.Contains(g))
                        return e;
                }
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje4)
            {
                foreach (UcesnikTakmicenja4 u in Takmicenje4.Ucesnici)
                {
                    if (u.Ekipa.Gimnasticari.Contains(g))
                        return u.Ekipa;
                }
            }
            return null;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(RedBroj.ToString());
            strBuilder.AppendLine(Gimnastika.ToString());
            strBuilder.AppendLine(ImaEkipnoTakmicenje.ToString());
            strBuilder.AppendLine(KombinovanoEkipnoTak.ToString());
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);
            strBuilder.AppendLine(Kategorija != null ? Kategorija.Id.ToString() : NULL);
            strBuilder.AppendLine(TakmicenjeDescription != null ? TakmicenjeDescription.Id.ToString() : NULL);
            if (Propozicije == null)
                strBuilder.AppendLine(NULL);
            else
                Propozicije.dump(strBuilder);
            if (Takmicenje1 == null)
                strBuilder.AppendLine(NULL);
            else
                Takmicenje1.dump(strBuilder);
            if (Takmicenje2 == null)
                strBuilder.AppendLine(NULL);
            else
                Takmicenje2.dump(strBuilder);
            if (Takmicenje3 == null)
                strBuilder.AppendLine(NULL);
            else
                Takmicenje3.dump(strBuilder);
            if (Takmicenje4 == null)
                strBuilder.AppendLine(NULL);
            else
                Takmicenje4.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            RedBroj = byte.Parse(reader.ReadLine());
            Gimnastika = (Gimnastika)Enum.Parse(typeof(Gimnastika), reader.ReadLine());
            ImaEkipnoTakmicenje = bool.Parse(reader.ReadLine());
            KombinovanoEkipnoTak = bool.Parse(reader.ReadLine());

            Takmicenje = map.takmicenjeMap[int.Parse(reader.ReadLine())];
            Kategorija = map.kategorijeMap[int.Parse(reader.ReadLine())];
            TakmicenjeDescription = map.descriptionsMap[int.Parse(reader.ReadLine())];

            string id = reader.ReadLine();
            Propozicije p = null;
            if (id != NULL)
            {
                p = new Propozicije();
                p.loadFromDump(reader);
            }
            Propozicije = p;

            id = reader.ReadLine();
            Takmicenje1 t1 = null;
            if (id != NULL)
            {
                t1 = new Takmicenje1();
                t1.loadFromDump(reader, map);
            }
            Takmicenje1 = t1;

            id = reader.ReadLine();
            Takmicenje2 t2 = null;
            if (id != NULL)
            {
                t2 = new Takmicenje2();
                t2.loadFromDump(reader, map);
            }
            Takmicenje2 = t2;

            id = reader.ReadLine();
            Takmicenje3 t3 = null;
            if (id != NULL)
            {
                t3 = new Takmicenje3();
                t3.loadFromDump(reader, map);
            }
            Takmicenje3 = t3;

            id = reader.ReadLine();
            Takmicenje4 t4 = null;
            if (id != NULL)
            {
                t4 = new Takmicenje4();
                t4.loadFromDump(reader, map);
            }
            Takmicenje4 = t4;
        }

        public virtual void updateRezultatiOnChangedPropozicije(IDictionary<int, Domain.Propozicije> origPropozicijeMap,
            Takmicenje takmicenje, IList<RezultatskoTakmicenje> rezTakmicenja, IList<Ocena> oceneTak1)
        {
            Propozicije origPropozicije = origPropozicijeMap[Id];

            bool rankPoredakUkupnoTak1 = false;
            bool createPoredakUkupnoTak1 = false;
            bool rankPoredakSpravaTak1 = false;
            bool rankPoredakPreskokTak1 = false;
            bool rankPoredakSpravaTak3 = false;
            bool rankPoredakPreskokTak3 = false;
            bool rankPoredakEkipeTak1 = false;
            bool createPoredakEkipeTak1 = false;
            bool calculatePoredakUkupnoFinaleKupa = false;
            bool calculatePoredakUkupnoZbirViseKola = false;
            bool rankPoredakSpravaFinaleKupa = false;
            bool calculatePoredakSpravaFinaleKupa = false;
            bool calculatePoredakEkipnoFinaleKupa = false;
            bool calculatePoredakEkipnoZbirViseKola = false;
            bool updateRezTak = false;

            // TODO: Fali kod za odvojeno takmicenje 2 finale kupa

            if (Propozicije.PostojiTak2 != origPropozicije.PostojiTak2)
            {
                // ignorisi, posto Takmicenje 2 uvek postoji, da bi se videli rezultati
            }
            if (Propozicije.OdvojenoTak2 != origPropozicije.OdvojenoTak2)
            {
                // Rangiraj ponovo rezultate jer se kval. status promenio.
                rankPoredakUkupnoTak1 = true;
            }
            if (Propozicije.ZaPreskokVisebojRacunajBoljuOcenu != origPropozicije.ZaPreskokVisebojRacunajBoljuOcenu)
                createPoredakUkupnoTak1 = true;
            if (Propozicije.NeogranicenBrojTakmicaraIzKlubaTak2 != origPropozicije.NeogranicenBrojTakmicaraIzKlubaTak2
                || Propozicije.MaxBrojTakmicaraIzKlubaTak2 != origPropozicije.MaxBrojTakmicaraIzKlubaTak2
                || Propozicije.MaxBrojTakmicaraTak2VaziZaDrzavu != origPropozicije.MaxBrojTakmicaraTak2VaziZaDrzavu
                || Propozicije.BrojFinalistaTak2 != origPropozicije.BrojFinalistaTak2
                || Propozicije.BrojRezerviTak2 != origPropozicije.BrojRezerviTak2)
            {
                rankPoredakUkupnoTak1 = true;
            }

            if (Propozicije.PostojiTak3 != origPropozicije.PostojiTak3)
            {
                // ignorisi, posto Takmicenje 3 uvek postoji, da bi se videli rezultati
            }
            if (Propozicije.OdvojenoTak3 != origPropozicije.OdvojenoTak3)
            {
                // Rangiraj ponovo rezultate jer se kval. status promenio.
                if (takmicenje.StandardnoTakmicenje)
                {
                    rankPoredakSpravaTak1 = true;
                    rankPoredakPreskokTak1 = true;
                }
                else if (takmicenje.FinaleKupa)
                    rankPoredakSpravaFinaleKupa = true;
            }
            if (Propozicije.NeogranicenBrojTakmicaraIzKlubaTak3 != origPropozicije.NeogranicenBrojTakmicaraIzKlubaTak3
                || Propozicije.MaxBrojTakmicaraIzKlubaTak3 != origPropozicije.MaxBrojTakmicaraIzKlubaTak3
                || Propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu != origPropozicije.MaxBrojTakmicaraTak3VaziZaDrzavu
                || Propozicije.BrojFinalistaTak3 != origPropozicije.BrojFinalistaTak3
                || Propozicije.BrojRezerviTak3 != origPropozicije.BrojRezerviTak3)
            {
                if (takmicenje.StandardnoTakmicenje)
                {
                    rankPoredakSpravaTak1 = true;
                    rankPoredakPreskokTak1 = true;
                }
                else if (takmicenje.FinaleKupa)
                    rankPoredakSpravaFinaleKupa = true;
            }
            if (Propozicije.VecaEOcenaImaPrednost != origPropozicije.VecaEOcenaImaPrednost)
            {
                if (takmicenje.StandardnoTakmicenje)
                {
                    rankPoredakSpravaTak1 = true;
                    rankPoredakPreskokTak1 = true;
                    rankPoredakSpravaTak3 = true;
                    rankPoredakPreskokTak3 = true;
                }
                else if (takmicenje.FinaleKupa)
                {
                    rankPoredakSpravaTak1 = true;
                    rankPoredakPreskokTak1 = true;
                }
            }
            if (Propozicije.Tak1PreskokNaOsnovuObaPreskoka != origPropozicije.Tak1PreskokNaOsnovuObaPreskoka)
            {
                if (takmicenje.StandardnoTakmicenje || takmicenje.FinaleKupa)
                    rankPoredakPreskokTak1 = true;
            }
            if (Propozicije.Tak3PreskokNaOsnovuObaPreskoka != origPropozicije.Tak3PreskokNaOsnovuObaPreskoka)
            {
                if (takmicenje.StandardnoTakmicenje)
                    rankPoredakPreskokTak3 = true;
            }

            // TODO: Fali kod za odvojeno ekipno finale kupa

            if (Propozicije.PostojiTak4 != origPropozicije.PostojiTak4)
            {
                // TODO4: Ovaj deo (konkretno grana else if (Propozicije.PostojiTak4)) je pravio probleme kada se
                // kreira takmicenje gde postoji jedno ekipno takmicenje za sve kategorije (prava vrednost za
                // ImaEkipnoTakmicenje se podesi u updateImaEkipnoTakmicenje, a onda se ovde pogresno postavi na true
                // za sva rez.takmicenja). Proveri da li moze (i kada) da pravi probleme to sto sam ceo ovaj deo
                // zakomentarisao 
                /*if (takmicenje.ZavrsenoTak1)
                {
                    // ignorisi
                    // TODO: Da li treba vracati originalnu vrednost za PostojiTak4 (isto i za PostojiTak2 i PostojiTak3)
                }
                else if (Propozicije.PostojiTak4)
                {
                    // ignorisi, posto se PoredakEkipno za takmicenje 1 uvek kreira
                    ImaEkipnoTakmicenje = true;
                }
                else
                {
                    // TODO: Razmisli da li treba pitati korisnika za potvrdu, pa zatim izbrisati ekipe i poredak ekipno.
                    ImaEkipnoTakmicenje = false;
                }*/
                // KombinovanoEkipnoTak se podesava u updateImaEkipnoTakmicenje.
            }
            if (Propozicije.OdvojenoTak4 != origPropozicije.OdvojenoTak4)
            {
                // Rangiraj ponovo rezultate jer se kval. status promenio.
                rankPoredakEkipeTak1 = true;
            }
            if (Propozicije.JednoTak4ZaSveKategorije != origPropozicije.JednoTak4ZaSveKategorije)
            {
                // rt.ImaEkipnoTakmicenje i rt.KombinovanoEkipnoTak su promenjeni u updateImaEkipnoTakmicenje.
                updateRezTak = true;

                // PoredakEkipno i Ekipe ignorisem, ostavljam korisniku da to ispodesava.
            }
            if (Propozicije.BrojRezultataKojiSeBodujuZaEkipu != origPropozicije.BrojRezultataKojiSeBodujuZaEkipu)
            {
                if (ImaEkipnoTakmicenje)
                    createPoredakEkipeTak1 = true;
            }
            if (!createPoredakEkipeTak1 && ImaEkipnoTakmicenje)
            {
                // Proveri da li treba ponovo racunati poredak zbog promenjenog nacina racunanja viseboja.
                bool create = false;
                if (!KombinovanoEkipnoTak
                    && Propozicije.ZaPreskokVisebojRacunajBoljuOcenu != origPropozicije.ZaPreskokVisebojRacunajBoljuOcenu)
                {
                    create = true;
                }
                if (!create && KombinovanoEkipnoTak)
                {
                    // Proveri da li je nekom rez. takmicenju unutar istog descriptiona promenjen nacin racunanja
                    // viseboja.
                    foreach (RezultatskoTakmicenje rt2 in rezTakmicenja)
                    {
                        if (!rt2.TakmicenjeDescription.Equals(TakmicenjeDescription))
                            continue;
                        if (rt2.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu
                            != origPropozicijeMap[rt2.Id].ZaPreskokVisebojRacunajBoljuOcenu)
                        {
                            create = true;
                            break;
                        }
                    }
                }
                if (create)
                    createPoredakEkipeTak1 = true;
            }
            if (Propozicije.BrojEkipaUFinalu != origPropozicije.BrojEkipaUFinalu)
            {
                if (ImaEkipnoTakmicenje)
                    rankPoredakEkipeTak1 = true;
            }

            if (Propozicije.NacinRacunanjaOceneFinaleKupaTak2 != origPropozicije.NacinRacunanjaOceneFinaleKupaTak2)
            {
                if (takmicenje.FinaleKupa)
                    calculatePoredakUkupnoFinaleKupa = true;
                else if (takmicenje.ZbirViseKola)
                    calculatePoredakUkupnoZbirViseKola = true;
            }

            if (Propozicije.NacinRacunanjaOceneFinaleKupaTak3 != origPropozicije.NacinRacunanjaOceneFinaleKupaTak3)
            {
                if (takmicenje.FinaleKupa)
                    calculatePoredakSpravaFinaleKupa = true;
            }

            if (Propozicije.NacinRacunanjaOceneFinaleKupaTak4 != origPropozicije.NacinRacunanjaOceneFinaleKupaTak4)
            {
                if (takmicenje.FinaleKupa)
                    calculatePoredakEkipnoFinaleKupa = true;
                else if (takmicenje.ZbirViseKola)
                    calculatePoredakEkipnoZbirViseKola = true;
            }

            if (updateRezTak)
            {

            }
            if (createPoredakUkupnoTak1)
                Takmicenje1.PoredakUkupno.create(this, oceneTak1);
            else if (rankPoredakUkupnoTak1)
                Takmicenje1.PoredakUkupno.rankRezultati(Propozicije);
            if (rankPoredakSpravaTak1)
            {
                foreach (PoredakSprava ps in Takmicenje1.PoredakSprava)
                    ps.rankRezultati(Propozicije);
            }
            if (rankPoredakPreskokTak1)
                Takmicenje1.PoredakPreskok.rankRezultati(Propozicije);
            if (rankPoredakSpravaTak3)
            {
                foreach (PoredakSprava ps in Takmicenje3.Poredak)
                    ps.rankRezultati(Propozicije);
            }
            if (rankPoredakPreskokTak3)
                Takmicenje3.PoredakPreskok.rankRezultati(Propozicije);
            if (createPoredakEkipeTak1)
            {
                Takmicenje1.PoredakEkipno.create(this,
                    Takmicenje.getEkipaRezultatiUkupnoMap(this, rezTakmicenja, DeoTakmicenjaKod.Takmicenje1));
            }
            else if (rankPoredakEkipeTak1)
                Takmicenje1.PoredakEkipno.rankRezultati(Propozicije);
            if (calculatePoredakUkupnoFinaleKupa)
                Takmicenje1.PoredakUkupnoFinaleKupa.calculateTotal(Propozicije);
            if (calculatePoredakUkupnoZbirViseKola)
                Takmicenje1.PoredakUkupnoZbirViseKola.calculateTotal();
            if (calculatePoredakSpravaFinaleKupa)
            {
                foreach (PoredakSpravaFinaleKupa p in Takmicenje1.PoredakSpravaFinaleKupa)
                    p.calculateTotal(Propozicije);
            }
            else if (rankPoredakSpravaFinaleKupa)
            {
                foreach (PoredakSpravaFinaleKupa p in Takmicenje1.PoredakSpravaFinaleKupa)
                    p.rankRezultati(Propozicije);
            }
            if (calculatePoredakEkipnoFinaleKupa)
                Takmicenje1.PoredakEkipnoFinaleKupa.calculateTotal(Propozicije);
            if (calculatePoredakEkipnoZbirViseKola)
                Takmicenje1.PoredakEkipnoZbirViseKola.calculateTotal();
        }
    }
}
