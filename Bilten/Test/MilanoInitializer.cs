using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Data;
using Bilten.Exceptions;
using Bilten.Domain;
using Bilten.Data.QueryModel;
using Iesi.Collections.Generic;

namespace Bilten.Test
{
    public class MilanoInitializer
    {
        private IDataContext dataContext;
        private Gimnastika gimnastika;

        public MilanoInitializer(Gimnastika gimnastika)
        {
            this.gimnastika = gimnastika;
        }

        public void insert()
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                insertTakmicenje();
                insertGimnasticariAndDrzaveUcesniciAndAddRezTakmicenjaUcesnici();
                insertSudijeUcesnici();

                insertRasporedSudija();
                insertStartListe();
                insertOcene();

                insertRezultatiUkupno(DeoTakmicenjaKod.Takmicenje1);
                insertRezultatiUkupno(DeoTakmicenjaKod.Takmicenje2);

                insertRezultatiSprava(DeoTakmicenjaKod.Takmicenje1);
                insertRezultatiSprava(DeoTakmicenjaKod.Takmicenje3);

                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private void insertTakmicenje()
        {
            Takmicenje takmicenje = new Takmicenje();
            takmicenje.Naziv = "3rd European Artistic Gymnastics Individual Championships";
            takmicenje.Datum = DateTime.Parse("02.04.2009");
            takmicenje.Mesto = "Milano";
            takmicenje.Gimnastika = gimnastika;
            takmicenje.BrojESudija = 6;
            if (gimnastika == Gimnastika.MSG)
            {
                takmicenje.BrojMeracaVremena = 1;
                takmicenje.BrojLinijskihSudija = 2;
            }
            else
            {
                takmicenje.BrojMeracaVremena = 2;
                takmicenje.BrojLinijskihSudija = 4;
            }
            takmicenje.BrojDecimalaD = 3;
            takmicenje.BrojDecimalaE1 = 2;
            takmicenje.BrojDecimalaE = 3;
            takmicenje.BrojDecimalaPen = 1;
            takmicenje.BrojDecimalaTotal = 3;

            takmicenje.ZavrsenoTak1 = true;

            string naziv = (gimnastika == Gimnastika.MSG) ? "Seniori" : "Seniorke";
            TakmicarskaKategorija takKategorija = 
                new TakmicarskaKategorija(naziv, gimnastika);

            RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
            desc.Naziv = "European Championships";
            desc.Propozicije = createPropozicije();

            takmicenje.addKategorija(takKategorija);
            takmicenje.addTakmicenjeDescription(desc);
            dataContext.Add(takmicenje);

            RezultatskoTakmicenje rezTak = new RezultatskoTakmicenje(takmicenje,
                takKategorija, desc, createPropozicije());
            dataContext.Add(rezTak);
        }

        private Propozicije createPropozicije()
        {
            Propozicije result = new Propozicije();

            result.MaxBrojTakmicaraIzKlubaTak1 = 0; // nema ogranicenja

            result.PostojiTak2 = true;
            result.OdvojenoTak2 = true;
            result.Tak2NaOsnovuTak1 = false;
            result.NeogranicenBrojTakmicaraIzKlubaTak2 = false;
            result.MaxBrojTakmicaraIzKlubaTak2 = 2;
            result.BrojFinalistaTak2 = 24;
            result.BrojRezerviTak2 = 4;

            result.PostojiTak3 = true;
            result.OdvojenoTak3 = true;
            result.Tak3NaOsnovuTak1 = false;
            result.NeogranicenBrojTakmicaraIzKlubaTak3 = false;
            result.MaxBrojTakmicaraIzKlubaTak3 = 2;
            result.MaxBrojTakmicaraTak3VaziZaDrzavu = true;
            result.BrojFinalistaTak3 = 8;
            result.BrojRezerviTak3 = 3;
            result.KvalifikantiTak3PreskokNaOsnovuObaPreskoka = true;
            result.PoredakTak3PreskokNaOsnovuObaPreskoka = true;

            result.PostojiTak4 = false;
            result.OdvojenoTak4 = false;
            result.Tak4NaOsnovuTak1 = false;
            result.BrojGimnasticaraUEkipi = 0;
            result.BrojRezultataKojiSeBodujuZaEkipu = 0;
            result.BrojEkipaUFinalu = 0;

            return result;
        }

        private void insertGimnasticariAndDrzaveUcesniciAndAddRezTakmicenjaUcesnici()
        {
            Takmicenje takmicenje = loadTakmicenje("Milano");
            TakmicarskaKategorija seniori = loadKategorija(takmicenje);

            Query q = new Query();
            q.Criteria.Add(new Criterion("Kategorija", CriteriaOperator.Equal, seniori));
            RezultatskoTakmicenje rezTak = dataContext.GetByCriteria<RezultatskoTakmicenje>(q)[0];

            string fileName;
            if (gimnastika == Gimnastika.MSG)
                fileName = @"..\..\test\Data\KvalifikantiMuskarci.txt";
            else
                fileName = @"..\..\test\Data\KvalifikantiZene.txt";

            List<Drzava> drzave = new List<Drzava>(dataContext.GetAll<Drzava>());
            List<DrzavaUcesnik> drzaveUcesnici = new List<DrzavaUcesnik>();

            GimnasticariParser parser = new GimnasticariParser();
            parser.parse(fileName);

            foreach (object[] o in parser.Gimnasticari)
            {
                int broj = (int)o[0];
                string prezime = (string)o[1];
                string ime = (string)o[2];
                string kod = (string)o[3];
                DateTime datumRodj = (DateTime)o[4];

                GimnasticarUcesnik gimnasticarUcesnik = new GimnasticarUcesnik();
                gimnasticarUcesnik.Ime = ime;
                gimnasticarUcesnik.Prezime = prezime;
                gimnasticarUcesnik.DatumRodjenja = new Datum(datumRodj);

                DrzavaUcesnik drzavaUcesnik = findDrzavaUcesnik(kod, drzaveUcesnici);
                if (drzavaUcesnik == null)
                {
                    Drzava drzava = findDrzava(kod, drzave);
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = drzava.Naziv;
                    drzavaUcesnik.Kod = drzava.Kod;
                    drzavaUcesnik.Takmicenje = takmicenje;
                    drzaveUcesnici.Add(drzavaUcesnik);

                    dataContext.Add(drzavaUcesnik);
                }
                gimnasticarUcesnik.DrzavaUcesnik = drzavaUcesnik;

                gimnasticarUcesnik.TakmicarskiBroj = broj;
                gimnasticarUcesnik.Takmicenje = takmicenje;
                gimnasticarUcesnik.Gimnastika = gimnastika;
                gimnasticarUcesnik.TakmicarskaKategorija = seniori;
                gimnasticarUcesnik.NastupaZaDrzavu = true;

                dataContext.Add(gimnasticarUcesnik);

                rezTak.Takmicenje1.addGimnasticar(gimnasticarUcesnik);
            }

            dataContext.Save(rezTak);
        }

        private Takmicenje loadTakmicenje(string naziv)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Mesto", CriteriaOperator.Equal, naziv));
            q.Criteria.Add(new Criterion("Gimnastika", CriteriaOperator.Equal, (byte)gimnastika));
            return dataContext.GetByCriteria<Takmicenje>(q)[0];
        }

        private TakmicarskaKategorija loadKategorija(Takmicenje takmicenje)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            return dataContext.GetByCriteria<TakmicarskaKategorija>(q)[0];
        }

        private Drzava findDrzava(string kod, IList<Drzava> drzave)
        {
            foreach (Drzava d in drzave)
            {
                if (d.Kod.ToUpper() == kod.ToUpper())
                    return d;
            }
            return null;
        }

        private DrzavaUcesnik findDrzavaUcesnik(string kod, IList<DrzavaUcesnik> drzave)
        {
            foreach (DrzavaUcesnik d in drzave)
            {
                if (d.Kod.ToUpper() == kod.ToUpper())
                    return d;
            }
            return null;
        }

        private void insertSudijeUcesnici()
        {
            Takmicenje takmicenje = loadTakmicenje("Milano");

            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            IList<DrzavaUcesnik> drzave = dataContext.GetByCriteria<DrzavaUcesnik>(q);

            ISet<SudijaUcesnik> sudije = new HashedSet<SudijaUcesnik>();

            string[] fileNames;
            if (gimnastika == Gimnastika.MSG)
                fileNames = new string[]
                {
                    @"..\..\test\Data\RasporedSudijaMuskarciKvalifikacije.txt",
                    @"..\..\test\Data\RasporedSudijaMuskarciViseboj.txt",
                };
            else
                fileNames = new string[]
                {
                    @"..\..\test\Data\RasporedSudijaZeneKvalifikacije.txt",
                    @"..\..\test\Data\RasporedSudijaZeneViseboj.txt"
                };

            for (int i = 0; i < fileNames.Length; i++)
            {
                SudijeParser parser = new SudijeParser();
                parser.parse(fileNames[i]);

                Pol pol = Pol.Muski;
                if (gimnastika == Gimnastika.ZSG)
                    pol = Pol.Zenski;
                foreach (object[] o in parser.SudijskeUloge)
                {
                    string ime = (string)o[0];
                    string prezime = (string)o[1];
                    string kod = (string)o[2];
                    SudijskaUloga uloga = (SudijskaUloga)o[4];
                    bool clanGlavSudOdbora =
                        uloga >= SudijskaUloga.PredsednikGlavnogSudijskogOdbora
                        && uloga <= SudijskaUloga.GredaKontrolor;

                    SudijaUcesnik sudija = new SudijaUcesnik();
                    sudija.Ime = ime;
                    sudija.Prezime = prezime;
                    sudija.Pol = pol;
                    if (clanGlavSudOdbora)
                        sudija.UlogaUGlavnomSudijskomOdboru = uloga;
                    else
                        sudija.UlogaUGlavnomSudijskomOdboru = SudijskaUloga.Undefined;
                    sudija.Drzava = findDrzavaUcesnik(kod, drzave);
                    if (sudija.Drzava == null)
                        throw new Exception("Greska prilikom dodavanja sudija ucesnika.");

                    sudija.Takmicenje = takmicenje;

                    if (sudije.Add(sudija))
                        dataContext.Add(sudija);
                }
            }
            /*foreach (DrzavaUcesnik d in drzave)
                dataContext.Evict(d);*/
        }

        private void insertRasporedSudija()
        {
            Takmicenje takmicenje = loadTakmicenje("Milano");
            TakmicarskaKategorija seniori = loadKategorija(takmicenje);

            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            IList<SudijaUcesnik> sudije_ucesnici = dataContext.GetByCriteria<SudijaUcesnik>(q);

            DeoTakmicenjaKod[] deoTakmicenja = { 
                DeoTakmicenjaKod.Takmicenje1, 
                DeoTakmicenjaKod.Takmicenje2 
            };
            string[] fileNames;
            if (gimnastika == Gimnastika.MSG)
                fileNames = new string[] 
                {
                    @"..\..\test\Data\RasporedSudijaMuskarciKvalifikacije.txt",
                    @"..\..\test\Data\RasporedSudijaMuskarciViseboj.txt"
                };
            else
                fileNames = new string[] 
                { 
                    @"..\..\test\Data\RasporedSudijaZeneKvalifikacije.txt",
                    @"..\..\test\Data\RasporedSudijaZeneViseboj.txt"
                };

            for (int i = 0; i < deoTakmicenja.Length; i++)
            {
                List<TakmicarskaKategorija> list = new List<TakmicarskaKategorija>();
                list.Add(seniori);
                RasporedSudija raspored = new RasporedSudija(list,
                    deoTakmicenja[i], takmicenje);

                SudijeParser parser = new SudijeParser();
                parser.parse(fileNames[i]);

                Sprava prevSprava = Sprava.Undefined;
                int brojMeracaVremena = 0;
                int brojLinijskihSudija = 0;
                SudijskiOdborNaSpravi odbor = null;

                foreach (object[] o in parser.SudijskeUloge)
                {
                    SudijskaUloga uloga = (SudijskaUloga)o[4];
                    bool clanGlavSudOdbora =
                        uloga >= SudijskaUloga.PredsednikGlavnogSudijskogOdbora
                        && uloga <= SudijskaUloga.GredaKontrolor;
                    if (clanGlavSudOdbora)
                        continue;

                    string ime = (string)o[0];
                    string prezime = (string)o[1];
                    SudijaUcesnik sudija = findSudijaUcesnik(ime, prezime, sudije_ucesnici);
                    if (sudija == null)
                        throw new Exception("Greska prilikom dodavanja rasporeda sudija.");

                    Sprava sprava = (Sprava)o[3];
                    if (sprava == prevSprava)
                    {
                        if (SudijskeUloge.isMeracVremena(uloga))
                            brojMeracaVremena++;
                        else if (SudijskeUloge.isLinijskiSudija(uloga))
                            brojLinijskihSudija++;
                    }
                    else
                    {
                        if (prevSprava != Sprava.Undefined)
                        {
                            // podesi broj meraca vremena i linijskih sudija za prethodni
                            // odbor
                            odbor = raspored.getOdbor(prevSprava);
                            odbor.setSupportedUloge(odbor.BrojDSudija, odbor.BrojESudija,
                                (byte)brojMeracaVremena, (byte)brojLinijskihSudija);
                        }
                        prevSprava = sprava;
                        brojMeracaVremena = 0;
                        brojLinijskihSudija = 0;
                    }

                    raspored.getOdbor(sprava).addSudija(sudija, uloga);
                }
                // podesi broj meraca vremena i linijskih sudija za poslednji odbor
                odbor = raspored.getOdbor(prevSprava);
                odbor.setSupportedUloge(odbor.BrojDSudija, odbor.BrojESudija,
                    (byte)brojMeracaVremena, (byte)brojLinijskihSudija);

                /*dataContext.Evict(takmicenje);
                foreach (SudijaUcesnik s in sudije_ucesnici)
                    dataContext.Evict(s);
                dataContext.Evict(seniori);
                */

                dataContext.Add(raspored);
            }
        }

        private SudijaUcesnik findSudijaUcesnik(string ime, string prezime, IList<SudijaUcesnik> sudije_ucesnici)
        {
            foreach (SudijaUcesnik s in sudije_ucesnici)
            {
                if (s.Ime == ime && s.Prezime == prezime)
                    return s;
            }
            return null;
        }

        private void insertStartListe()
        {
            Takmicenje takmicenje = loadTakmicenje("Milano");
            TakmicarskaKategorija seniori = loadKategorija(takmicenje);

            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            IList<GimnasticarUcesnik> gim_uces = dataContext.GetByCriteria<GimnasticarUcesnik>(q);
            Dictionary<int, GimnasticarUcesnik> gim_ucesnici = new Dictionary<int, GimnasticarUcesnik>();
            foreach (GimnasticarUcesnik g in gim_uces)
                gim_ucesnici.Add(g.TakmicarskiBroj.Value, g);

            DeoTakmicenjaKod[] deoTakmicenja = { 
                DeoTakmicenjaKod.Takmicenje1, 
                DeoTakmicenjaKod.Takmicenje2,
                DeoTakmicenjaKod.Takmicenje3
            };
            string[] fileNames;
            if (gimnastika == Gimnastika.MSG)
                fileNames = new string[] 
                { 
                    @"..\..\test\Data\StartListaKvalifikacijeMuskarci.txt",
                    @"..\..\test\Data\StartListaVisebojMuskarci.txt",
                    @"..\..\test\Data\StartListaSpraveMuskarci.txt"
                };
            else
                fileNames = new string[] 
                {
                    @"..\..\test\Data\StartListaKvalifikacijeZene.txt",
                    @"..\..\test\Data\StartListaVisebojZene.txt",
                    @"..\..\test\Data\StartListaSpraveZene.txt"
                };

            for (int i = 0; i < deoTakmicenja.Length; i++)
            {
                List<TakmicarskaKategorija> list = new List<TakmicarskaKategorija>();
                list.Add(seniori);
                RasporedNastupa raspored = new RasporedNastupa(list, deoTakmicenja[i]);

                StartListaParser parser = new StartListaParser();
                parser.parse(fileNames[i]);

                foreach (object[] o in parser.NastupiNaSpravi)
                {
                    Sprava sprava = Sprave.parse((string)o[0]);
                    int grupa = (int)o[1];
                    int rot = (int)o[2];
                    int broj = (int)o[3];
                    bool nastupaDvaPuta = (bool)o[4];

                    GimnasticarUcesnik gimnasticar = gim_ucesnici[broj];
                    if (gimnasticar == null)
                        throw new Exception("Greska prilikom dodavanja rasporeda nastupa.");
                    NastupNaSpravi nastup = new NastupNaSpravi(
                        nastupaDvaPuta, gimnasticar);

                    while (raspored.getStartLista(sprava, grupa, rot) == null)
                        raspored.addNewGrupa();
                    raspored.getStartLista(sprava, grupa, rot).addNastup(nastup);
                }

                /*     dataContext.Evict(takmicenje);
                     foreach (GimnasticarUcesnik g in gim_uces)
                         dataContext.Evict(g);
                     dataContext.Evict(seniori);
                     */

                dataContext.Add(raspored);
            }
        }

        private void insertOcene()
        {
            Takmicenje takmicenje = loadTakmicenje("Milano");
            TakmicarskaKategorija seniori = loadKategorija(takmicenje);

            Query q = new Query();
            q.Criteria.Add(
                new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            q.FetchModes.Add(new AssociationFetch(
                "DrzavaUcesnik", AssociationFetchMode.Eager));
            IList<GimnasticarUcesnik> gimnasticari = dataContext.GetByCriteria<GimnasticarUcesnik>(q);

            Dictionary<int, GimnasticarUcesnik> gimnasticariMap = new Dictionary<int, GimnasticarUcesnik>();
            foreach (GimnasticarUcesnik g in gimnasticari)
                gimnasticariMap.Add(g.TakmicarskiBroj.Value, g);

            DeoTakmicenjaKod[] deoTakmicenja = { 
                DeoTakmicenjaKod.Takmicenje1, 
                DeoTakmicenjaKod.Takmicenje2,
                DeoTakmicenjaKod.Takmicenje3
            };
            string[] fileNames;
            if (gimnastika == Gimnastika.MSG)
                fileNames = new string[]  
                { 
                    @"..\..\test\Data\OceneKvalifikacijeMuskarci.txt",
                    @"..\..\test\Data\OceneVisebojMuskarci.txt",
                    @"..\..\test\Data\OceneSpraveMuskarci.txt"
                };
            else
                fileNames = new string[] 
                {
                    @"..\..\test\Data\OceneKvalifikacijeZene.txt",
                    @"..\..\test\Data\OceneVisebojZene.txt",
                    @"..\..\test\Data\OceneSpraveZene.txt"
                };

            for (int i = 0; i < deoTakmicenja.Length; i++)
            {
                List<Ocena> ocene = new List<Ocena>();

                OceneParser parser = new OceneParser(gimnasticariMap);
                parser.parse(fileNames[i]);

                TakmicarskaKategorija kategorija = seniori;

                int rank;
                int broj = 0;
                Ocena ocena = null;
                foreach (object[] o in parser.Ocene)
                {
                    Sprava sprava = Sprave.parse((string)o[0]);

                    if ((string)o[1] != "")
                        rank = Convert.ToInt32(o[1]);

                    if ((string)o[2] != "")
                        broj = Convert.ToInt32(o[2]);

                    Nullable<byte> brojPreskoka = null;
                    if ((string)o[3] != "")
                        brojPreskoka = Convert.ToByte(o[3]);

                    Nullable<short> vreme = null;
                    if ((string)o[4] != "")
                        vreme = parseMinutSecond((string)o[4]);

                    float D = Convert.ToSingle(replaceDelimiter(o[5]));
                    float E1 = Convert.ToSingle(replaceDelimiter(o[6]));
                    float E2 = Convert.ToSingle(replaceDelimiter(o[7]));
                    float E3 = Convert.ToSingle(replaceDelimiter(o[8]));
                    float E4 = Convert.ToSingle(replaceDelimiter(o[9]));
                    float E5 = Convert.ToSingle(replaceDelimiter(o[10]));
                    float E6 = Convert.ToSingle(replaceDelimiter(o[11]));
                    float E = Convert.ToSingle(replaceDelimiter(o[12]));

                    Nullable<float> pen = null;
                    if ((string)o[13] != "")
                        pen = Convert.ToSingle(replaceDelimiter(o[13]));

                    Nullable<float> L = null;
                    if ((string)o[14] != "")
                    {
                        L = Convert.ToSingle(replaceDelimiter(o[14]));
                    }

                    Nullable<float> T = null;
                    if ((string)o[15] != "")
                        T = Convert.ToSingle(replaceDelimiter(o[15]));

                    Nullable<float> O = null;
                    if ((string)o[16] != "")
                        O = Convert.ToSingle(replaceDelimiter(o[16]));

                    Nullable<float> ocenaPreskok = null;
                    if ((string)o[17] != "")
                        ocenaPreskok = Convert.ToSingle(replaceDelimiter(o[17]));

                    Nullable<float> total = null;
                    if ((string)o[18] != "")
                        total = Convert.ToSingle(replaceDelimiter(o[18]));

                    if (sprava != Sprava.Preskok || brojPreskoka != 2)
                    {
                        ocena = new Ocena();
                        ocena.Sprava = sprava;
                        ocena.DeoTakmicenjaKod = deoTakmicenja[i];
                        ocena.VremeVezbe = vreme;
                        ocena.D = D;
                        ocena.E1 = E1;
                        ocena.E2 = E2;
                        ocena.E3 = E3;
                        ocena.E4 = E4;
                        ocena.E5 = E5;
                        ocena.E6 = E6;
                        ocena.E = E;
                        ocena.Penalty = pen;
                        ocena.LinePenalty = L;
                        ocena.TimePenalty = T;
                        ocena.OtherPenalty = O;
                        if (sprava != Sprava.Preskok || deoTakmicenja[i] == DeoTakmicenjaKod.Takmicenje2)
                        {
                            // za finale viseboja postoji samo jedna ocena preskoka
                            ocena.Total = total;
                        }
                        else
                            ocena.Total = ocenaPreskok;

                        ocena.BrojEOcena = 6;
                        ocena.RucnoUnetaOcena = false;

                        GimnasticarUcesnik gimnasticar = gimnasticariMap[broj];
                        if (gimnasticar == null)
                            throw new Exception("Greska prilikom dodavanja rasporeda sudija.");
                        ocena.Gimnasticar = gimnasticar;

                        ocene.Add(ocena);
                    }
                    else
                    {
                        DrugaOcena ocena2 = new DrugaOcena();
                        ocena2.D = D;
                        ocena2.E1 = E1;
                        ocena2.E2 = E2;
                        ocena2.E3 = E3;
                        ocena2.E4 = E4;
                        ocena2.E5 = E5;
                        ocena2.E6 = E6;
                        ocena2.E = E;
                        ocena2.Penalty = pen;
                        ocena2.LinePenalty = L;
                        ocena2.OtherPenalty = O;
                        ocena2.Total = ocenaPreskok;
                        ocena2.BrojEOcena = 6;
                        ocena2.RucnoUnetaOcena = false;

                        ocena.Ocena2 = ocena2;
                        ocena.TotalObeOcene = ocena.getTotalObeOcene(takmicenje.BrojDecimalaTotal);
                    }
                }

                /*      dataContext.Evict(takmicenje);
                      foreach (GimnasticarUcesnik g in gimnasticari)
                          dataContext.Evict(g);
                      dataContext.Evict(seniori);
                 */

                foreach (Ocena o in ocene)
                {
                    dataContext.Add(o);
                }
            }
        }

        private short parseMinutSecond(string s)
        {
            string[] parts = s.Split(':');
            return (short)(Int16.Parse(parts[0]) * 60 + Int16.Parse(parts[1]));
        }

        private string replaceDelimiter(object o)
        {
            string s = (string)o;
            return s.Replace('.', ',');
        }

        private void insertRezultatiUkupno(DeoTakmicenjaKod deoTakKod)
        {
            Takmicenje takmicenje = loadTakmicenje("Milano");
            TakmicarskaKategorija seniori = loadKategorija(takmicenje);

            Query q = new Query();
            q.Criteria.Add(new Criterion("Kategorija", CriteriaOperator.Equal, seniori));
            RezultatskoTakmicenje rezTak = dataContext.GetByCriteria<RezultatskoTakmicenje>(q)[0];

            IList<Ocena> ocene = dataContext.ExecuteNamedQuery<Ocena>(
                "FindOceneByDeoTakmicenja",
                    new string[] { "takId", "deoTakKod" },
                    new object[] { takmicenje.Id, deoTakKod });

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                rezTak.Takmicenje1.PoredakUkupno.create(rezTak, ocene);
                rezTak.Takmicenje2.createUcesnici(rezTak.Takmicenje1);
                if (gimnastika == Gimnastika.ZSG)
                {
                    q = new Query();
                    q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
                    q.Criteria.Add(new Criterion("TakmicarskiBroj", CriteriaOperator.Equal, 172));
                    GimnasticarUcesnik GORYUNOVAKristina = dataContext.GetByCriteria<GimnasticarUcesnik>(q)[0];

                    q = new Query();
                    q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
                    q.Criteria.Add(new Criterion("TakmicarskiBroj", CriteriaOperator.Equal, 170));
                    GimnasticarUcesnik AFANASEVAKsenia = dataContext.GetByCriteria<GimnasticarUcesnik>(q)[0];

                    UcesnikTakmicenja2 ucesnikGORYUNOVAKristina =
                        rezTak.Takmicenje2.getUcesnikKvalifikant(GORYUNOVAKristina);
                    
                    rezTak.Takmicenje2.removeUcesnik(ucesnikGORYUNOVAKristina);
                    rezTak.Takmicenje2.addUcesnik(
                        new UcesnikTakmicenja2(AFANASEVAKsenia, 19, 54.575f, 20,
                            KvalifikacioniStatus.Q));

                    // NOTE: Primetiti da moram eksplicitno da obrisem izbacenog
                    // ucesnika, bez obzira sto je cascade="all-delete-orphan"
                    // za asocijaciju UcesnikTakmicenja2. Za razliku od ovoga, u
                    // klasi MainForm u metodu zavisiTakmicenje1 (koji pri kreiranju
                    // novih ucesnika najpre obrise stare ucesnike) nije neophodno da
                    // se brisu stari ucesnici, tj. bice automatski izbrisani. 
                    // Ispitati zasto se desava razlicito ponasanje

                    dataContext.Delete(ucesnikGORYUNOVAKristina);
                }
            }
            else
            {
                rezTak.Takmicenje2.Poredak.create(rezTak, ocene);
            }

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                dataContext.Save(rezTak.Takmicenje1);
                dataContext.Save(rezTak.Takmicenje2);
            }
            else
                dataContext.Save(rezTak.Takmicenje2);

        }

        private void insertRezultatiSprava(DeoTakmicenjaKod deoTakKod)
        {
            Takmicenje takmicenje = loadTakmicenje("Milano");
            TakmicarskaKategorija seniori = loadKategorija(takmicenje);

            Query q = new Query();
            q.Criteria.Add(new Criterion("Kategorija", CriteriaOperator.Equal, seniori));
            RezultatskoTakmicenje rezTak = dataContext.GetByCriteria<RezultatskoTakmicenje>(q)[0];

            IList<Ocena> ocene = dataContext.ExecuteNamedQuery<Ocena>(
                "FindOceneByDeoTakmicenja",
                    new string[] { "takId", "deoTakKod" },
                    new object[] { takmicenje.Id, deoTakKod });

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                foreach (PoredakSprava p in rezTak.Takmicenje1.PoredakSprava)
                    p.create(rezTak, ocene);
                rezTak.Takmicenje1.PoredakPreskok.create(rezTak, ocene);
                rezTak.Takmicenje3.createUcesnici(rezTak.Takmicenje1, 
                    rezTak.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka);
                
                if (gimnastika == Gimnastika.MSG)
                {
                    q = new Query();
                    q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
                    q.Criteria.Add(new Criterion("TakmicarskiBroj", CriteriaOperator.Equal, 628));
                    GimnasticarUcesnik KHOROKHORDINSergei = dataContext.GetByCriteria<GimnasticarUcesnik>(q)[0];

                    q = new Query();
                    q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
                    q.Criteria.Add(new Criterion("TakmicarskiBroj", CriteriaOperator.Equal, 600));
                    GimnasticarUcesnik WAMMESJeffrey = dataContext.GetByCriteria<GimnasticarUcesnik>(q)[0];

                    UcesnikTakmicenja3 ucesnikKHOROKHORDINSergei =
                        rezTak.Takmicenje3.getUcesnikKvalifikant(KHOROKHORDINSergei, Sprava.Vratilo);

                    rezTak.Takmicenje3.removeUcesnik(ucesnikKHOROKHORDINSergei);
                    rezTak.Takmicenje3.addUcesnik(
                        new UcesnikTakmicenja3(WAMMESJeffrey, Sprava.Vratilo, 8, 14.600f, 8,
                            KvalifikacioniStatus.Q));

                    dataContext.Delete(ucesnikKHOROKHORDINSergei);
                }
            }
            else
            {
                foreach (PoredakSprava p in rezTak.Takmicenje3.Poredak)
                    p.create(rezTak, ocene);
                rezTak.Takmicenje3.PoredakPreskok.create(rezTak, ocene);
            }

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                dataContext.Save(rezTak.Takmicenje1);
                dataContext.Save(rezTak.Takmicenje3);
            }
            else
                dataContext.Save(rezTak.Takmicenje3);

        }

    }
}
