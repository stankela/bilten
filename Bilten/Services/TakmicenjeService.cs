using Bilten;
using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Util;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Services
{
    public class TakmicenjeService
    {
        public static void addTakmicenje(Takmicenje t, IList<KlubUcesnik> klubovi, IList<DrzavaUcesnik> drzave,
            IList<GimnasticarUcesnik> gimnasticari, IList<RezultatskoTakmicenje> rezTakmicenja,
            IList<SudijaUcesnik> sudije, IList<RasporedSudija> rasporediSudija, IList<RasporedNastupa> rasporediNastupa,
            IList<Ocena> ocene)
        {
            // dodaj takmicenje
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Add(t);

            // kategorije i descriptions se dodaju pomocu transitive persistance

            // dodaj klubove ucesnike
            KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
            foreach (KlubUcesnik k in klubovi)
                klubUcesnikDAO.Add(k);

            // dodaj drzave ucesnike
            DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
            foreach (DrzavaUcesnik d in drzave)
                drzavaUcesnikDAO.Add(d);

            // dodaj gimnasticare ucesnike
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            foreach (GimnasticarUcesnik g in gimnasticari)
                gimnasticarUcesnikDAO.Add(g);

            // dodaj rezultatska takmicenja i ekipe
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            foreach (RezultatskoTakmicenje r in rezTakmicenja)
            {
                foreach (Ekipa e in r.Takmicenje1.Ekipe)
                    ekipaDAO.Add(e);
                rezultatskoTakmicenjeDAO.Add(r);
            }

            // dodaj sudije ucesnike
            if (sudije != null)
            {
                SudijaUcesnikDAO sudijaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO();
                foreach (SudijaUcesnik s in sudije)
                    sudijaUcesnikDAO.Add(s);
            }

            // dodaj rasporede sudija
            if (rasporediSudija != null)
            {
                RasporedSudijaDAO rasporedSudijaDAO = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO();
                foreach (RasporedSudija r in rasporediSudija)
                    rasporedSudijaDAO.Add(r);
            }

            // dodaj rasporede nastupa
            if (rasporediNastupa != null)
            {
                RasporedNastupaDAO rasporedNastupaDAO = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO();
                foreach (RasporedNastupa r in rasporediNastupa)
                    rasporedNastupaDAO.Add(r);
            }

            // dodaj ocene
            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            foreach (Ocena o in ocene)
                ocenaDAO.Add(o);
        }

        public static void deleteTakmicenje(Takmicenje t, bool proveriFinala)
        {
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();

            if (proveriFinala)
            {
                // Proveri da li se neko takmicenje (finale kupa ili zbir vise kola) referise na ovo takmicenje.
                IList<Takmicenje> finala = takmicenjeDAO.FindFinala(t);
                if (finala.Count > 0)
                {
                    string msg = "Takmicenje \"" + t.ToString() + "\" je nemoguce izbrisati jer je ono jedno od kola za " +
                        "sledeca finala:\n\n";
                    foreach (Takmicenje f in finala)
                        msg += f.ToString() + "\n";
                    throw new BusinessException(msg);
                }
            }

            // brisi ocene
            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            foreach (Ocena o in ocenaDAO.FindByTakmicenje(t.Id))
                ocenaDAO.Delete(o);

            // brisi rasporede nastupa
            RasporedNastupaDAO rasporedNastupaDAO = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO();
            foreach (RasporedNastupa r in rasporedNastupaDAO.FindByTakmicenje(t.Id))
                rasporedNastupaDAO.Delete(r);

            // brisi rasporede sudija
            RasporedSudijaDAO rasporedSudijaDAO = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO();
            foreach (RasporedSudija r in rasporedSudijaDAO.FindByTakmicenje(t.Id))
                rasporedSudijaDAO.Delete(r);

            // brisi sudije ucesnike
            SudijaUcesnikDAO sudijaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO();
            foreach (SudijaUcesnik s in sudijaUcesnikDAO.FindByTakmicenje(t.Id))
                sudijaUcesnikDAO.Delete(s);

            // brisi rezultatska takmicenja i ekipe
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            foreach (RezultatskoTakmicenje r in rezultatskoTakmicenjeDAO.FindByTakmicenje(t.Id))
            {
                foreach (Ekipa e in r.Takmicenje1.Ekipe)
                    ekipaDAO.Delete(e);
                rezultatskoTakmicenjeDAO.Delete(r);
            }

            // brisi gimnasticare ucesnike
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            foreach (GimnasticarUcesnik g in gimnasticarUcesnikDAO.FindByTakmicenje(t.Id))
                gimnasticarUcesnikDAO.Delete(g);

            // brisi klubove ucesnike
            KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
            foreach (KlubUcesnik k in klubUcesnikDAO.FindByTakmicenje(t.Id))
                klubUcesnikDAO.Delete(k);

            // brisi drzave ucesnike
            DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
            foreach (DrzavaUcesnik d in drzavaUcesnikDAO.FindByTakmicenje(t.Id))
                drzavaUcesnikDAO.Delete(d);

            // brisi kategorije
            TakmicarskaKategorijaDAO takmicarskaKategorijaDAO = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO();
            foreach (TakmicarskaKategorija k in takmicarskaKategorijaDAO.FindByTakmicenje(t.Id))
                takmicarskaKategorijaDAO.Delete(k);

            // brisi descriptions
            RezultatskoTakmicenjeDescriptionDAO rezTakDescDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO();
            foreach (RezultatskoTakmicenjeDescription d in rezTakDescDAO.FindByTakmicenje(t.Id))
                rezTakDescDAO.Delete(d);

            // brisi takmicenje
            takmicenjeDAO.Delete(t);
        }

        public static void prebrisiTakmicenje(Takmicenje t, IList<KlubUcesnik> klubovi, IList<DrzavaUcesnik> drzave,
            IList<GimnasticarUcesnik> gimnasticari, IList<RezultatskoTakmicenje> rezTakmicenja,
            IList<SudijaUcesnik> sudije, IList<RasporedSudija> rasporediSudija, IList<RasporedNastupa> rasporediNastupa,
            IList<Ocena> ocene)
        {
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            Takmicenje t2 = takmicenjeDAO.FindByNazivGimnastikaDatum(t.Naziv, t.Gimnastika, t.Datum);
            if (t2 != null)
            {
                // Posto cemo izbrisati postojece takmicenje i uvesti ga sa novim ID-om, moramo da
                // promenimo sva finala koja se referisu na ovo takmicenje da koriste novi ID.
                IList<Takmicenje> finala = takmicenjeDAO.FindFinala(t2);
                if (finala.Count > 0)
                {
                    takmicenjeDAO.Add(t);
                    foreach (Takmicenje f in finala)
                    {
                        if (f.PrvoKolo != null && f.PrvoKolo.Id == t2.Id)
                            f.PrvoKolo = t;
                        if (f.DrugoKolo != null && f.DrugoKolo.Id == t2.Id)
                            f.DrugoKolo = t;
                        if (f.TreceKolo != null && f.TreceKolo.Id == t2.Id)
                            f.TreceKolo = t;
                        if (f.CetvrtoKolo != null && f.CetvrtoKolo.Id == t2.Id)
                            f.CetvrtoKolo = t;
                        takmicenjeDAO.Update(f);
                    }
                }
                deleteTakmicenje(t2, false);
            }
            addTakmicenje(t, klubovi, drzave, gimnasticari, rezTakmicenja, sudije, rasporediSudija, rasporediNastupa, ocene);
        }

        public static void createFromPrevTakmicenje(Takmicenje takmicenje, Takmicenje from,
            IList<RezultatskoTakmicenje> rezTakmicenjaFrom,
            IDictionary<int, List<GimnasticarUcesnik>> rezTakToGimnasticarUcesnikMap)
        {
            const int MAX = 1024;

            TakmicarskaKategorija[] kategorije = new TakmicarskaKategorija[MAX];
            for (int i = 0; i < MAX; ++i)
                kategorije[i] = null;
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                if (kategorije[rtFrom.Kategorija.RedBroj] == null)
                    kategorije[rtFrom.Kategorija.RedBroj] = new TakmicarskaKategorija(rtFrom.Kategorija.Naziv);
            }

            PropozicijeDAO propozicijeDAO = DAOFactoryFactory.DAOFactory.GetPropozicijeDAO();
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                propozicijeDAO.Attach(rtFrom.TakmicenjeDescription.Propozicije, false);
                propozicijeDAO.Attach(rtFrom.Propozicije, false);
            }

            RezultatskoTakmicenjeDescription[] descriptions = new RezultatskoTakmicenjeDescription[MAX];
            for (int i = 0; i < MAX; ++i)
                descriptions[i] = null;
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                if (descriptions[rtFrom.TakmicenjeDescription.RedBroj] == null)
                {
                    RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
                    desc.Naziv = rtFrom.TakmicenjeDescription.Naziv;
                    desc.Propozicije = new Propozicije();

                    // Apdejtujem jedino propozicije za takmicenje 4 zbog kombinovanog ekipnog takmicenja.
                    // Ostale propozicije su na inicijalnim vrednostima.
                    rtFrom.TakmicenjeDescription.Propozicije.copyTakmicenje4To(desc.Propozicije);                    
                    
                    descriptions[rtFrom.TakmicenjeDescription.RedBroj] = desc;
                }
            }

            takmicenje.Kategorije.Clear();
            for (int i = 0; i < MAX; ++i)
            {
                if (kategorije[i] != null)
                    takmicenje.addKategorija(kategorije[i]);
            }

            takmicenje.TakmicenjeDescriptions.Clear();
            bool first = true;
            for (int i = 0; i < MAX; ++i)
            {
                if (descriptions[i] != null)
                {
                    if (first)
                    {
                        // prvi description je uvek kao naziv takmicenja.
                        RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
                        desc.Naziv = takmicenje.Naziv;
                        desc.Propozicije = descriptions[i].Propozicije;  // klonirane propozicije
                        descriptions[i] = desc;
                        first = false;
                    }
                    takmicenje.addTakmicenjeDescription(descriptions[i]);
                }
            }

            IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                RezultatskoTakmicenje rt = new RezultatskoTakmicenje(takmicenje,
                    kategorije[rtFrom.Kategorija.RedBroj],
                    descriptions[rtFrom.TakmicenjeDescription.RedBroj],
                    new Propozicije());
                rtFrom.Propozicije.copyTakmicenje4To(rt.Propozicije);
                rezTakmicenja.Add(rt);
            }

            foreach (RezultatskoTakmicenjeDescription d in takmicenje.TakmicenjeDescriptions)
                RezultatskoTakmicenje.updateImaEkipnoTakmicenje(rezTakmicenja, d);

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Add(takmicenje);

            IDictionary<int, GimnasticarUcesnik> gimnasticariMap = new Dictionary<int, GimnasticarUcesnik>();
            for (int i = 0; i < rezTakmicenja.Count; ++i)
            {
                RezultatskoTakmicenje rt = rezTakmicenja[i];
                RezultatskoTakmicenje rtFrom = rezTakmicenjaFrom[i];
                foreach (GimnasticarUcesnik g in rezTakToGimnasticarUcesnikMap[rtFrom.Id])
                {
                    GimnasticarUcesnik g2;
                    if (!gimnasticariMap.ContainsKey(g.Id))
                    {
                        g2 = GimnasticarUcesnikService.createGimnasticarUcesnik(g,
                            kategorije[g.TakmicarskaKategorija.RedBroj]);
                        gimnasticariMap[g.Id] = g2;
                    }
                    else
                        g2 = gimnasticariMap[g.Id];
                    rt.Takmicenje1.addGimnasticar(g2);
                    rt.Takmicenje1.updateRezultatiOnGimnasticarAdded(g2, new List<Ocena>(), rt);
                }
            }

            Takmicenje1DAO takmicenje1DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO();
            for (int i = 0; i < rezTakmicenja.Count; ++i)
            {
                RezultatskoTakmicenje rt = rezTakmicenja[i];
                RezultatskoTakmicenje rtFrom = rezTakmicenjaFrom[i];
                takmicenje1DAO.Attach(rtFrom.Takmicenje1, false);
                foreach (Ekipa e in rtFrom.Takmicenje1.Ekipe)
                {
                    Ekipa ekipa = new Ekipa();
                    ekipa.Naziv = e.Naziv;
                    ekipa.Kod = e.Kod;

                    // Ne kopiram clanove ekipe zato sto dodavati clanove ekipe ima smisla jedino ako se znaju
                    // rezultati, a ovaj metod samo pravi pripremu takmicenja i nema nikakvih rezultata.

                    rt.Takmicenje1.addEkipa(ekipa);
                }
            }

            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                rezultatskoTakmicenjeDAO.Add(rt);
                foreach (Ekipa e in rt.Takmicenje1.Ekipe)
                    ekipaDAO.Add(e);
            }

            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            foreach (GimnasticarUcesnik g in gimnasticariMap.Values)
            {
                gimnasticarUcesnikDAO.Add(g);
            }
        }

        public static void kreirajNaOsnovuViseKola(Takmicenje takmicenje,
            IList<KeyValuePair<GimnasticarUcesnik, IList<Pair<int, TakmicarskaKategorija>>>> razlicitaKola)
        {
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Attach(takmicenje, false);

            List<Takmicenje> prethodnaKola = new List<Takmicenje>();
            prethodnaKola.Add(takmicenje.PrvoKolo);
            prethodnaKola.Add(takmicenje.DrugoKolo);
            if (takmicenje.TreceKolo != null)
                prethodnaKola.Add(takmicenje.TreceKolo);
            if (takmicenje.CetvrtoKolo != null)
                prethodnaKola.Add(takmicenje.CetvrtoKolo);

            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

            List<IList<RezultatskoTakmicenje>> rezTakmicenjaPrethodnaKola = new List<IList<RezultatskoTakmicenje>>();
            foreach (Takmicenje prethKolo in prethodnaKola)
                rezTakmicenjaPrethodnaKola.Add(rezTakDAO.FindByTakmicenje(prethKolo.Id));

            takmicenje.Kategorije.Clear();
            foreach (RezultatskoTakmicenje rt in rezTakmicenjaPrethodnaKola[0])
            {
                // Implementiran je najjednostavniji slucaj, gde se u svakom od prethodnih kola gleda samo prvo
                // takmicenje, i uzimaju se samo one kategorije gde postoji poklapanje. U principu, mogla bi se
                // implementirati i slozenija varijanta gde bi se, u slucaju da ne postoji poklapanje medju kategorijama,
                // otvorio prozor gde bi mogle da se uparuju kategorije, ali onda bi morao da nekako pamtim
                // koja su uparivanja izabrana (da bi ih primenio kod apdejtovanja kada se npr. ocena iz nekog od
                // prethodnih kola promeni).
                if (rt.TakmicenjeDescription.RedBroj != 0)
                    continue;

                bool ok = true;
                foreach (List<RezultatskoTakmicenje> rezTakList in rezTakmicenjaPrethodnaKola)
                {
                    if (Takmicenje.getRezTakmicenje(rezTakList, 0, rt.Kategorija) == null)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                    takmicenje.addKategorija(new TakmicarskaKategorija(rt.Kategorija.Naziv));
            }
            if (takmicenje.Kategorije.Count == 0)
                throw new BusinessException("Kategorije iz prethodnih kola se ne poklapaju");

            // prvi description je uvek kao naziv takmicenja.
            takmicenje.TakmicenjeDescriptions.Clear();            
            RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
            desc.Naziv = takmicenje.Naziv;
            desc.Propozicije = new Propozicije(true, takmicenje.FinaleKupa, true);
            takmicenje.addTakmicenjeDescription(desc);

            // Takmicenje dodajem ovako rano zato sto se takmicenje.Id koristi dole u createGimnasticarUcesnik
            takmicenjeDAO.Add(takmicenje);

            IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
            foreach (TakmicarskaKategorija k in takmicenje.Kategorije)
            {
                Propozicije p = new Propozicije(true, takmicenje.FinaleKupa, true);
                RezultatskoTakmicenje rt = new RezultatskoTakmicenje(takmicenje, k, desc, p);
                rt.ImaEkipnoTakmicenje = true;
                rt.KombinovanoEkipnoTak = false;
                rezTakmicenja.Add(rt);
            }

            GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();

            // Za svakog gimnasticara, zapamti u kojim kategorijama je ucestvovao u prethodnim kolima
            IDictionary<GimnasticarUcesnik, IList<Pair<int, TakmicarskaKategorija>>> mapaUcestvovanja
                = new Dictionary<GimnasticarUcesnik, IList<Pair<int, TakmicarskaKategorija>>>();

            IDictionary<TakmicarskaKategorija, RezultatskoTakmicenje> katToRezTakMap
                = new Dictionary<TakmicarskaKategorija, RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                katToRezTakMap.Add(rt.Kategorija, rt);
                for (int i = 0; i < rezTakmicenjaPrethodnaKola.Count; ++i)
                {
                    IList<RezultatskoTakmicenje> rezTakmicenjaPrethKolo = rezTakmicenjaPrethodnaKola[i];
                    RezultatskoTakmicenje rtFrom = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethKolo, 0, rt.Kategorija);

                    Pair<int, TakmicarskaKategorija> koloKatPair = new Pair<int, TakmicarskaKategorija>(i, rt.Kategorija);

                    foreach (GimnasticarUcesnik g in rtFrom.Takmicenje1.Gimnasticari)
                    {
                        if (!mapaUcestvovanja.ContainsKey(g))
                        {
                            // Koriscenje IDictionary obezbedjuje da je svaki gimnasticar dodat u samo jednu kategoriju.
                            GimnasticarUcesnik g2 = GimnasticarUcesnikService.createGimnasticarUcesnik(g, rt.Kategorija);
                            IList<Pair<int, TakmicarskaKategorija>> pairList = new List<Pair<int, TakmicarskaKategorija>>();
                            pairList.Add(koloKatPair);
                            mapaUcestvovanja.Add(g2, pairList);
                            gimUcesnikDAO.Add(g2);
                        }
                        else
                            mapaUcestvovanja[g].Add(koloKatPair);
                    }
                }
            }

            foreach (KeyValuePair<GimnasticarUcesnik, IList<Pair<int, TakmicarskaKategorija>>> entry in mapaUcestvovanja)
            {
                GimnasticarUcesnik g = entry.Key;
                TakmicarskaKategorija prevKat = null;
                bool ok = true;
                foreach (Pair<int, TakmicarskaKategorija> koloKatPair in entry.Value)
                {
                    TakmicarskaKategorija kat = koloKatPair.Second;
                    if (prevKat == null)
                        prevKat = kat;
                    else if (!kat.Equals(prevKat))
                        ok = false;

                    RezultatskoTakmicenje rt = katToRezTakMap[kat];
                    rt.Takmicenje1.addGimnasticar(g);
                }
                if (!ok)
                    razlicitaKola.Add(entry);
            }

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                {
                    RezultatskoTakmicenje rtFrom = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethKolo, 0, rt.Kategorija);
                    foreach (Ekipa e in rtFrom.Takmicenje1.Ekipe)
                    {
                        if (rt.Takmicenje1.Ekipe.Contains(e))
                            continue;

                        Ekipa ekipa = new Ekipa();
                        ekipa.Naziv = e.Naziv;
                        ekipa.Kod = e.Kod;
                        rt.Takmicenje1.addEkipa(ekipa);
                        ekipaDAO.Add(ekipa);
                    }
                }
            }

            // TODO: Prebaci u domenske klase sto vise koda iz ove funkcije.

            takmicenje.kreirajRezultateViseKola(rezTakmicenja, rezTakmicenjaPrethodnaKola);

            foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                foreach (RezultatskoTakmicenje rt in rezTakmicenjaPrethKolo)
                    rezTakDAO.Evict(rt);

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                rezTakDAO.Add(rt);
        }

        public static void updateViseKola(Takmicenje takmicenje)
        {
            // Ne apdejtujem kategorije i takmicenja. Dodajem nove gimnasticare i ekipe, i onda ponovo izracunavam
            // sve rezultate. Takodje, ne vodim racuna o tome da li je novi gimnsticar ucestvovao u razlicitim
            // kategorijama u prethodnim kolima (ako je i dodat neki nov gimnasticar, najverovatnije je dodat samo jedan,
            // i onda ce korisnik najverovatnije sam proveriti da li su kategorije u kojima je gimnasticar ucestvovao iste.)

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Attach(takmicenje, false);

            List<Takmicenje> prethodnaKola = new List<Takmicenje>();
            prethodnaKola.Add(takmicenje.PrvoKolo);
            prethodnaKola.Add(takmicenje.DrugoKolo);
            if (takmicenje.TreceKolo != null)
                prethodnaKola.Add(takmicenje.TreceKolo);
            if (takmicenje.CetvrtoKolo != null)
                prethodnaKola.Add(takmicenje.CetvrtoKolo);

            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();

            IList<RezultatskoTakmicenje> rezTakmicenja = rezTakDAO.FindByTakmicenje(takmicenje.Id);

            List<IList<RezultatskoTakmicenje>> rezTakmicenjaPrethodnaKola = new List<IList<RezultatskoTakmicenje>>();
            foreach (Takmicenje prethKolo in prethodnaKola)
                rezTakmicenjaPrethodnaKola.Add(rezTakDAO.FindByTakmicenje(prethKolo.Id));

            IDictionary<GimnasticarUcesnik, GimnasticarUcesnik> gimUcesniciMap
                = new Dictionary<GimnasticarUcesnik, GimnasticarUcesnik>();
            foreach (GimnasticarUcesnik g in gimUcesnikDAO.FindByTakmicenje(takmicenje.Id))
                gimUcesniciMap.Add(g, g);

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                {
                    RezultatskoTakmicenje rtFrom = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethKolo, 0, rt.Kategorija);
                    foreach (GimnasticarUcesnik g in rtFrom.Takmicenje1.Gimnasticari)
                    {
                        if (rt.Takmicenje1.Gimnasticari.Contains(g))
                            continue;

                        GimnasticarUcesnik g2;
                        if (!gimUcesniciMap.ContainsKey(g))
                        {
                            g2 = GimnasticarUcesnikService.createGimnasticarUcesnik(g, rt.Kategorija);
                            gimUcesniciMap.Add(g, g2);
                            gimUcesnikDAO.Add(g2);
                        }
                        else
                            g2 = gimUcesniciMap[g];
                        rt.Takmicenje1.addGimnasticar(g2);
                    }
                }
            }

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                {
                    RezultatskoTakmicenje rtFrom = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethKolo, 0, rt.Kategorija);
                    foreach (Ekipa e in rtFrom.Takmicenje1.Ekipe)
                    {
                        if (rt.Takmicenje1.Ekipe.Contains(e))
                            continue;

                        Ekipa ekipa = new Ekipa();
                        ekipa.Naziv = e.Naziv;
                        ekipa.Kod = e.Kod;
                        rt.Takmicenje1.addEkipa(ekipa);
                        ekipaDAO.Add(ekipa);
                    }
                }
            }

            // TODO: Prebaci u domenske klase sto vise koda iz ove funkcije.

            takmicenje.kreirajRezultateViseKola(rezTakmicenja, rezTakmicenjaPrethodnaKola);

            foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                foreach (RezultatskoTakmicenje rt in rezTakmicenjaPrethKolo)
                    rezTakDAO.Evict(rt);

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                rezTakDAO.Update(rt);
        }

        public static void addTakmicarskaKategorija(TakmicarskaKategorija kat, int takmicenjeId)
        {
            Notification notification = new Notification();
            kat.validate(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            if (DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().existsKategorijaNaziv(kat.Naziv, takmicenjeId))
            {
                notification.RegisterMessage("Naziv", "Kategorija sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            Takmicenje t = takmicenjeDAO.FindById(takmicenjeId);
            t.addKategorija(kat);
            takmicenjeDAO.Update(t);
        }
    }
}
