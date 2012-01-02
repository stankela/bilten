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
    public class RegistarInitializer
    {
        private IDataContext dataContext;

        public RegistarInitializer()
        { 
        
        }

        public void insert()
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                insertKategorijeGimnasticara();
                insertKluboviIMesta();
                insertDrzave();
                insertGimnasticari();
                insertRegistrovaniGimnasticari();
                insertSudije();

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

        private void insertKategorijeGimnasticara()
        {
            string[] kategorije = new string[] 
                { "Seniori", "Seniorke", "Juniori", "Juniorke" };
            Gimnastika[] gimnastike = new Gimnastika[] 
                { Gimnastika.MSG, Gimnastika.ZSG, Gimnastika.MSG, Gimnastika.ZSG };
            
            for (int i = 0; i < kategorije.Length; i++)
            {
                KategorijaGimnasticara kat = new KategorijaGimnasticara();
                kat.Naziv = kategorije[i];
                kat.Gimnastika = gimnastike[i];
                dataContext.Add(kat);
            }
        }

        private void insertKluboviIMesta()
        {
            KluboviParser parser = new KluboviParser();
            parser.parse(@"..\..\test\Data\Klubovi.txt");

            IList<Mesto> mesta = new List<Mesto>();

            foreach (object[] o in parser.Klubovi)
            {
                string naziv = (string)o[0];
                string mesto = (string)o[1];
                string kod = (string)o[2];

                Klub k = new Klub();
                k.Naziv = naziv;
                k.Kod = kod;

                Mesto m = findMesto(mesto, mesta);
                if (m == null)
                {
                    m = new Mesto();
                    m.Naziv = mesto;
                    dataContext.Add(m);
                    mesta.Add(m);
                }
                k.Mesto = m;
                dataContext.Add(k);
            }
        }

        private Mesto findMesto(string mesto, IList<Mesto> mesta)
        {
            foreach (Mesto m in mesta)
            {
                if (m.Naziv.ToUpper() == mesto.ToUpper())
                    return m;
            }
            return null;
        }

        private void insertDrzave()
        {
            DrzaveParser parser = new DrzaveParser();
            parser.parse(@"..\..\test\Data\DrzaveSrb.txt");

            foreach (object[] o in parser.Drzave)
            {
                Drzava drzava = new Drzava();
                drzava.Kod = (string)o[0];
                drzava.Naziv = (string)o[1];

                dataContext.Add(drzava);
            }
        }

        private void insertGimnasticari()
        {
            IList<Drzava> drzave = dataContext.GetAll<Drzava>();

            Query q = new Query();
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, "Seniori"));
            KategorijaGimnasticara seniori = dataContext.GetByCriteria<KategorijaGimnasticara>(q)[0];

            string[] fileNames = new string[]
                {
                    @"..\..\test\Data\KvalifikantiMuskarci.txt",
                    @"..\..\test\Data\KvalifikantiZene.txt"
                };

            for (int i = 0; i < fileNames.Length; i++)
            {
                GimnasticariParser parser = new GimnasticariParser();
                parser.parse(fileNames[i]);

                foreach (object[] o in parser.Gimnasticari)
                {
                    int broj = (int)o[0];
                    string prezime = (string)o[1];
                    string ime = (string)o[2];
                    string kod = (string)o[3];
                    DateTime datumRodj = (DateTime)o[4];

                    Gimnasticar gimnasticar = new Gimnasticar();
                    gimnasticar.Ime = ime;
                    gimnasticar.Prezime = prezime;
                    gimnasticar.DatumRodjenja = new Datum(datumRodj);
                    gimnasticar.Drzava = findDrzava(kod, drzave);
                    if (i == 0)
                        gimnasticar.Gimnastika = Gimnastika.MSG;
                    else
                        gimnasticar.Gimnastika = Gimnastika.ZSG;
                    gimnasticar.Kategorija = seniori;

                    dataContext.Add(gimnasticar);
                }
            }
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

        private void insertSudije()
        {
            IList<Drzava> drzave = dataContext.GetAll<Drzava>();
            ISet<Sudija> sudije = new HashedSet<Sudija>();

            string[] fileNames = new string[]
                {
                    @"..\..\test\Data\RasporedSudijaMuskarciKvalifikacije.txt",
                    @"..\..\test\Data\RasporedSudijaMuskarciViseboj.txt",
                    @"..\..\test\Data\RasporedSudijaZeneKvalifikacije.txt",
                    @"..\..\test\Data\RasporedSudijaZeneViseboj.txt"
                };

            for (int i = 0; i < fileNames.Length; i++)
            {
                SudijeParser parser = new SudijeParser();
                parser.parse(fileNames[i]);

                Pol pol = Pol.Muski;
                if (i > 1)
                    pol = Pol.Zenski;
                foreach (object[] o in parser.SudijskeUloge)
                {
                    string ime = (string)o[0];
                    string prezime = (string)o[1];
                    string kod = (string)o[2];

                    Sudija sudija = new Sudija();
                    sudija.Ime = ime;
                    sudija.Prezime = prezime;
                    sudija.Pol = pol;
                    sudija.Drzava = findDrzava(kod, drzave);
                    sudije.Add(sudija);
                }
            }

            foreach (Sudija s in sudije)
                dataContext.Add(s);
        }

        private void insertRegistrovaniGimnasticari()
        {
            RegistrovaniGimnasticariParser parser = new RegistrovaniGimnasticariParser();
            string fileName = @"..\..\test\Data\RegistracijaTakmicara2009.txt";
            parser.parse(fileName);

            IList<Gimnasticar> gimnasticari = dataContext.GetAll<Gimnasticar>();
            IList<Klub> klubovi = dataContext.GetAll<Klub>();

            Query q = new Query();
            q.Criteria.Add(new Criterion("Kod", CriteriaOperator.Equal, "SRB"));
            Drzava srbija = dataContext.GetByCriteria<Drzava>(q)[0];

            foreach (object[] o in parser.Gimnasticari)
            {
                char pol = (char)o[0];
                string ime = (string)o[1];
                string prezime = (string)o[2];
                string datumRodj = (string)o[3];
                string klubMesto = (string)o[4];
                string regBroj = (string)o[5];
                string datumReg = (string)o[6];

                Gimnasticar gimnasticar = findGimnasticar(ime, prezime, gimnasticari);
                if (gimnasticar != null)
                {
                    if (datumRodj.Trim() == "NULL")
                        gimnasticar.DatumRodjenja = null;
                    else
                        gimnasticar.DatumRodjenja = Datum.Parse(datumRodj);
                    gimnasticar.RegistarskiBroj = RegistarskiBroj.Parse(regBroj);
                    if (datumReg.Trim() == String.Empty)
                        gimnasticar.DatumPoslednjeRegistracije = null;
                    else
                        gimnasticar.DatumPoslednjeRegistracije = Datum.Parse(datumReg);
                    gimnasticar.Klub = findKlub(klubMesto, klubovi);

                    dataContext.Save(gimnasticar);
                }
                else
                {
                    gimnasticar = new Gimnasticar();
                    if (pol == 'M')
                        gimnasticar.Gimnastika = Gimnastika.MSG;
                    else if (pol == 'Z')
                        gimnasticar.Gimnastika = Gimnastika.ZSG;
                    else if (pol == ' ')
                        gimnasticar.Gimnastika = Gimnastika.Undefined;
                    else
                        throw new FormatException("Invalid format in file " + fileName);

                    gimnasticar.Ime = ime.Trim();
                    gimnasticar.Prezime = prezime.Trim();
                    if (datumRodj.Trim() == "NULL")
                        gimnasticar.DatumRodjenja = null;
                    else
                        gimnasticar.DatumRodjenja = Datum.Parse(datumRodj);
                    gimnasticar.RegistarskiBroj = RegistarskiBroj.Parse(regBroj);
                    if (datumReg.Trim() == String.Empty)
                        gimnasticar.DatumPoslednjeRegistracije = null;
                    else
                        gimnasticar.DatumPoslednjeRegistracije = Datum.Parse(datumReg);

                    gimnasticar.Klub = findKlub(klubMesto, klubovi);
                    gimnasticar.Drzava = srbija;

                    dataContext.Add(gimnasticar);
                }
            }
        }

        private Gimnasticar findGimnasticar(string ime, string prezime, IList<Gimnasticar> gimnasticari)
        {
            foreach (Gimnasticar g in gimnasticari)
            {
                if (g.Ime.ToUpper() == ime.ToUpper() && g.Prezime.ToUpper() == prezime.ToUpper())
                    return g;
            }
            return null;
        }

        private Klub findKlub(string naziv, IList<Klub> klubovi)
        {
            foreach (Klub k in klubovi)
            {
                if (k.Naziv.StartsWith(naziv, StringComparison.CurrentCultureIgnoreCase)
                || naziv.StartsWith(k.Naziv, StringComparison.CurrentCultureIgnoreCase))
                    return k;
            }
            return null;
        }

        public void delete()
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<Gimnasticar> gimnasticari = dataContext.GetAll<Gimnasticar>();
                foreach (Gimnasticar g in gimnasticari)
                    dataContext.Delete(g);

                IList<KategorijaGimnasticara> kategorije = dataContext.GetAll<KategorijaGimnasticara>();
                foreach (KategorijaGimnasticara k in kategorije)
                    dataContext.Delete(k);

                IList<Klub> klubovi = dataContext.GetAll<Klub>();
                foreach (Klub k in klubovi)
                    dataContext.Delete(k);

                IList<Mesto> mesta = dataContext.GetAll<Mesto>();
                foreach (Mesto m in mesta)
                    dataContext.Delete(m);

                IList<Sudija> sudije = dataContext.GetAll<Sudija>();
                foreach (Sudija s in sudije)
                    dataContext.Delete(s);

                IList<Drzava> drzave = dataContext.GetAll<Drzava>();
                foreach (Drzava d in drzave)
                    dataContext.Delete(d);

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

    }
}
