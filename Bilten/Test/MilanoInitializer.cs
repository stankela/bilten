using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Data;
using Bilten.Exceptions;
using Bilten.Domain;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.Test
{
    public class MilanoInitializer
    {
        private Gimnastika gimnastika;

        public MilanoInitializer(Gimnastika gimnastika)
        {
            this.gimnastika = gimnastika;
        }

        public void insert()
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    Takmicenje t = insertTakmicenje();
                    insertGimnasticariAndDrzaveUcesniciAndAddRezTakmicenjaUcesnici(t);
                    insertSudijeUcesnici(t);

                    insertRasporedSudija(t);
                    insertStartListe(t);
                    insertOcene(t);

                    insertRezultatiUkupno(DeoTakmicenjaKod.Takmicenje1, t);
                    insertRezultatiUkupno(DeoTakmicenjaKod.Takmicenje2, t);

                    insertRezultatiSprava(DeoTakmicenjaKod.Takmicenje1, t);
                    insertRezultatiSprava(DeoTakmicenjaKod.Takmicenje3, t);

                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private Takmicenje insertTakmicenje()
        {
            Takmicenje takmicenje = new Takmicenje();
            takmicenje.Naziv = "3rd European Artistic Gymnastics Individual Championships";
            takmicenje.Datum = DateTime.Parse("02.04.2009");
            takmicenje.Mesto = "Milano";
            takmicenje.Gimnastika = gimnastika;

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
            DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Add(takmicenje);

            RezultatskoTakmicenje rezTak = new RezultatskoTakmicenje(takmicenje,
                takKategorija, desc, createPropozicije());
            DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().Add(rezTak);
            return takmicenje;
        }

        private Propozicije createPropozicije()
        {
            Propozicije result = new Propozicije();

            result.PostojiTak2 = true;
            result.OdvojenoTak2 = true;
            result.NeogranicenBrojTakmicaraIzKlubaTak2 = false;
            result.MaxBrojTakmicaraIzKlubaTak2 = 2;
            result.BrojFinalistaTak2 = 24;
            result.BrojRezerviTak2 = 4;

            result.PostojiTak3 = true;
            result.OdvojenoTak3 = true;
            result.NeogranicenBrojTakmicaraIzKlubaTak3 = false;
            result.MaxBrojTakmicaraIzKlubaTak3 = 2;
            result.MaxBrojTakmicaraTak3VaziZaDrzavu = true;
            result.BrojFinalistaTak3 = 8;
            result.BrojRezerviTak3 = 3;
            result.KvalifikantiTak3PreskokNaOsnovuObaPreskoka = true;
            result.PoredakTak3PreskokNaOsnovuObaPreskoka = true;

            result.PostojiTak4 = false;
            result.OdvojenoTak4 = false;
            result.BrojRezultataKojiSeBodujuZaEkipu = 0;
            result.BrojEkipaUFinalu = 0;

            return result;
        }

        private void insertGimnasticariAndDrzaveUcesniciAndAddRezTakmicenjaUcesnici(Takmicenje takmicenje)
        {
            TakmicarskaKategorija seniori = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenje.Id)[0];

            RezultatskoTakmicenjeDAO rezTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            RezultatskoTakmicenje rezTak = rezTakmicenjeDAO.FindByKategorija(seniori)[0];

            string fileName;
            if (gimnastika == Gimnastika.MSG)
                fileName = @"..\..\test\Data\KvalifikantiMuskarci.txt";
            else
                fileName = @"..\..\test\Data\KvalifikantiZene.txt";

            List<Drzava> drzave = new List<Drzava>(DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindAll());
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

                    DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().Add(drzavaUcesnik);
                }
                gimnasticarUcesnik.DrzavaUcesnik = drzavaUcesnik;

                gimnasticarUcesnik.TakmicarskaKategorija = seniori;
                gimnasticarUcesnik.NastupaZaDrzavu = true;

                DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().Add(gimnasticarUcesnik);

                rezTak.Takmicenje1.addGimnasticar(gimnasticarUcesnik);
            }

            rezTakmicenjeDAO.Update(rezTak);
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

        private void insertSudijeUcesnici(Takmicenje takmicenje)
        {
            IList<DrzavaUcesnik> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(takmicenje.Id);

            ISet<SudijaUcesnik> sudije = new HashSet<SudijaUcesnik>();

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
                    /*bool clanGlavSudOdbora =
                        uloga >= SudijskaUloga.PredsednikGlavnogSudijskogOdbora
                        && uloga <= SudijskaUloga.GredaKontrolor;*/

                    SudijaUcesnik sudija = new SudijaUcesnik();
                    sudija.Ime = ime;
                    sudija.Prezime = prezime;
                    sudija.Pol = pol;
                    /*if (clanGlavSudOdbora)
                        sudija.UlogaUGlavnomSudijskomOdboru = uloga;
                    else
                        sudija.UlogaUGlavnomSudijskomOdboru = SudijskaUloga.Undefined;*/
                    sudija.DrzavaUcesnik = findDrzavaUcesnik(kod, drzave);
                    if (sudija.DrzavaUcesnik == null)
                        throw new Exception("Greska prilikom dodavanja sudija ucesnika.");

                    sudija.Takmicenje = takmicenje;

                    if (sudije.Add(sudija))
                        DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().Add(sudija);
                }
            }
            /*foreach (DrzavaUcesnik d in drzave)
                dataContext.Evict(d);*/
        }

        private void insertRasporedSudija(Takmicenje takmicenje)
        {
            TakmicarskaKategorija seniori = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                .FindByTakmicenje(takmicenje.Id)[0];
            IList<SudijaUcesnik> sudije_ucesnici = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().FindByTakmicenje(takmicenje.Id);

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
                SudijskiOdborNaSpravi odbor = null;

                foreach (object[] o in parser.SudijskeUloge)
                {
                    SudijskaUloga uloga = (SudijskaUloga)o[4];
                    bool clanGlavSudOdbora =
                        false/*uloga >= SudijskaUloga.PredsednikGlavnogSudijskogOdbora
                        && uloga <= SudijskaUloga.GredaKontrolor*/;
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
                        /*if (SudijskeUloge.isMeracVremena(uloga))
                            brojMeracaVremena++;
                        else if (SudijskeUloge.isLinijskiSudija(uloga))
                            brojLinijskihSudija++;*/
                    }
                    else
                    {
                        if (prevSprava != Sprava.Undefined)
                        {
                            // podesi broj meraca vremena i linijskih sudija za prethodni
                            // odbor
                            odbor = raspored.getOdbor(prevSprava);
                            odbor.setSupportedUloge(odbor.BrojDSudija, odbor.HasD1_E1, odbor.HasD2_E2, odbor.BrojESudija);
                        }
                        prevSprava = sprava;
                    }

                    raspored.getOdbor(sprava).addSudija(sudija, uloga);
                }
                // podesi broj meraca vremena i linijskih sudija za poslednji odbor
                odbor = raspored.getOdbor(prevSprava);
                odbor.setSupportedUloge(odbor.BrojDSudija, odbor.HasD1_E1, odbor.HasD2_E2, odbor.BrojESudija);

                /*dataContext.Evict(takmicenje);
                foreach (SudijaUcesnik s in sudije_ucesnici)
                    dataContext.Evict(s);
                dataContext.Evict(seniori);
                */

                DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO().Add(raspored);
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

        private void insertStartListe(Takmicenje takmicenje)
        {
            TakmicarskaKategorija seniori = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                .FindByTakmicenje(takmicenje.Id)[0];

            IList<GimnasticarUcesnik> gim_uces = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()
                .FindByTakmicenje(takmicenje.Id);
            Dictionary<int, GimnasticarUcesnik> gim_ucesnici = new Dictionary<int, GimnasticarUcesnik>();
            //foreach (GimnasticarUcesnik g in gim_uces)
              //  gim_ucesnici.Add(g.TakmicarskiBroj.Value, g);

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
                RasporedNastupa raspored = new RasporedNastupa(list, deoTakmicenja[i], gimnastika);

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
                    NastupNaSpravi nastup = new NastupNaSpravi(gimnasticar, 0);

                    while (raspored.getStartLista(sprava, grupa, rot) == null)
                        raspored.addNewGrupa(gimnastika);
                    raspored.getStartLista(sprava, grupa, rot).addNastup(nastup);
                }

                /*     dataContext.Evict(takmicenje);
                     foreach (GimnasticarUcesnik g in gim_uces)
                         dataContext.Evict(g);
                     dataContext.Evict(seniori);
                     */

                DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO().Add(raspored);
            }
        }

        private void insertOcene(Takmicenje takmicenje)
        {
            TakmicarskaKategorija seniori = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                .FindByTakmicenje(takmicenje.Id)[0];
            IList<GimnasticarUcesnik> gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()
                .FindByTakmicenje(takmicenje.Id);

            Dictionary<int, GimnasticarUcesnik> gimnasticariMap = new Dictionary<int, GimnasticarUcesnik>();
            //foreach (GimnasticarUcesnik g in gimnasticari)
              //  gimnasticariMap.Add(g.TakmicarskiBroj.Value, g);

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
                        //ocena.VremeVezbe = vreme;
                        ocena.D = D;
                        ocena.E1 = E1;
                        ocena.E2 = E2;
                        ocena.E3 = E3;
                        ocena.E4 = E4;
                        ocena.E5 = E5;
                        ocena.E6 = E6;
                        ocena.E = E;
                        ocena.Penalty = pen;
                        //ocena.LinePenalty = L;
                        //ocena.TimePenalty = T;
                        //ocena.OtherPenalty = O;
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
                        //ocena2.LinePenalty = L;
                        //ocena2.OtherPenalty = O;
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
                    DAOFactoryFactory.DAOFactory.GetOcenaDAO().Add(o);
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

        private void insertRezultatiUkupno(DeoTakmicenjaKod deoTakKod, Takmicenje takmicenje)
        {
            TakmicarskaKategorija seniori = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                .FindByTakmicenje(takmicenje.Id)[0];
            RezultatskoTakmicenje rezTak = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByKategorija(seniori)[0];
            IList<Ocena> ocene = DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindByDeoTakmicenja(takmicenje.Id, deoTakKod);

            GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                rezTak.Takmicenje1.PoredakUkupno.create(rezTak, ocene);
                rezTak.Takmicenje2.createUcesnici(rezTak.Takmicenje1);
                if (gimnastika == Gimnastika.ZSG)
                {
                    /*GimnasticarUcesnik GORYUNOVAKristina = gimUcesnikDAO.FindByTakmicenjeTakBroj(takmicenje, 172);
                    GimnasticarUcesnik AFANASEVAKsenia = gimUcesnikDAO.FindByTakmicenjeTakBroj(takmicenje, 170);

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

                    DAOFactoryFactory.DAOFactory.GetUcesnikTakmicenja2DAO().Delete(ucesnikGORYUNOVAKristina);*/
                }
            }
            else
            {
                rezTak.Takmicenje2.Poredak.create(rezTak, ocene);
            }

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTak.Takmicenje1);
                DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Update(rezTak.Takmicenje2);
            }
            else
                DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Update(rezTak.Takmicenje2);
        }

        private void insertRezultatiSprava(DeoTakmicenjaKod deoTakKod, Takmicenje takmicenje)
        {
            TakmicarskaKategorija seniori = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                .FindByTakmicenje(takmicenje.Id)[0];
            RezultatskoTakmicenje rezTak = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByKategorija(seniori)[0];
            IList<Ocena> ocene = DAOFactoryFactory.DAOFactory.GetOcenaDAO()
                .FindByDeoTakmicenja(takmicenje.Id, deoTakKod);

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                foreach (PoredakSprava p in rezTak.Takmicenje1.PoredakSprava)
                    p.create(rezTak, ocene);
                rezTak.Takmicenje1.PoredakPreskok.create(rezTak, ocene);
                rezTak.Takmicenje3.createUcesnici(rezTak.Takmicenje1, 
                    rezTak.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka);

                GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                if (gimnastika == Gimnastika.MSG)
                {
                    /*GimnasticarUcesnik KHOROKHORDINSergei = gimUcesnikDAO.FindByTakmicenjeTakBroj(takmicenje, 628);
                    GimnasticarUcesnik WAMMESJeffrey = gimUcesnikDAO.FindByTakmicenjeTakBroj(takmicenje, 600);

                    UcesnikTakmicenja3 ucesnikKHOROKHORDINSergei =
                        rezTak.Takmicenje3.getUcesnikKvalifikant(KHOROKHORDINSergei, Sprava.Vratilo);

                    rezTak.Takmicenje3.removeUcesnik(ucesnikKHOROKHORDINSergei);
                    rezTak.Takmicenje3.addUcesnik(
                        new UcesnikTakmicenja3(WAMMESJeffrey, Sprava.Vratilo, 8, 14.600f, 8,
                            KvalifikacioniStatus.Q));

                    DAOFactoryFactory.DAOFactory.GetUcesnikTakmicenja3DAO().Delete(ucesnikKHOROKHORDINSergei);*/
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
                DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTak.Takmicenje1);
                DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Update(rezTak.Takmicenje3);
            }
            else
                DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Update(rezTak.Takmicenje3);
        }
    }
}
