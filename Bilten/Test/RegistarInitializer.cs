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
    public class RegistarInitializer
    {
        public RegistarInitializer()
        { 
        
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
                    insertKategorijeGimnasticara();
                    insertKluboviIMesta();
                    insertDrzave();
                    insertGimnasticari();
                    insertRegistrovaniGimnasticari();
                    insertSudije();

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
                DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().Add(kat);
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
                    DAOFactoryFactory.DAOFactory.GetMestoDAO().Add(m);
                    mesta.Add(m);
                }
                k.Mesto = m;
                DAOFactoryFactory.DAOFactory.GetKlubDAO().Add(k);
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
                DAOFactoryFactory.DAOFactory.GetDrzavaDAO().Add(drzava);
            }
        }

        private void insertGimnasticari()
        {
            IList<Drzava> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindAll();
            KategorijaGimnasticara seniori = DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().FindByNaziv("Seniori");

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

                    DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().Add(gimnasticar);
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
            IList<Drzava> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindAll();
            ISet<Sudija> sudije = new HashSet<Sudija>();

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
                DAOFactoryFactory.DAOFactory.GetSudijaDAO().Add(s);
        }

        private void insertRegistrovaniGimnasticari()
        {
            RegistrovaniGimnasticariParser parser = new RegistrovaniGimnasticariParser();
            string fileName = @"..\..\test\Data\RegistracijaTakmicara2009.txt";
            parser.parse(fileName);

            GimnasticarDAO gimnasticarDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO();
            IList<Gimnasticar> gimnasticari = gimnasticarDAO.FindAll();
            IList<Klub> klubovi = DAOFactoryFactory.DAOFactory.GetKlubDAO().FindAll();
            Drzava srbija = DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindByKod("SRB");

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
                    gimnasticar.RegistarskiBroj = regBroj;
                    if (datumReg.Trim() == String.Empty)
                        gimnasticar.DatumPoslednjeRegistracije = null;
                    else
                        gimnasticar.DatumPoslednjeRegistracije = Datum.Parse(datumReg);
                    gimnasticar.Klub = findKlub(klubMesto, klubovi);

                    gimnasticarDAO.Update(gimnasticar);
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
                    gimnasticar.RegistarskiBroj = regBroj;
                    if (datumReg.Trim() == String.Empty)
                        gimnasticar.DatumPoslednjeRegistracije = null;
                    else
                        gimnasticar.DatumPoslednjeRegistracije = Datum.Parse(datumReg);

                    gimnasticar.Klub = findKlub(klubMesto, klubovi);
                    gimnasticar.Drzava = srbija;

                    gimnasticarDAO.Add(gimnasticar);
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    GimnasticarDAO gimnasticarDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO();
                    IList<Gimnasticar> gimnasticari = gimnasticarDAO.FindAll();
                    foreach (Gimnasticar g in gimnasticari)
                        gimnasticarDAO.Delete(g);

                    KategorijaGimnasticaraDAO kategorijaGimnasticaraDAO = DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO();
                    IList<KategorijaGimnasticara> kategorije = kategorijaGimnasticaraDAO.FindAll();
                    foreach (KategorijaGimnasticara k in kategorije)
                        kategorijaGimnasticaraDAO.Delete(k);

                    KlubDAO klubDAO = DAOFactoryFactory.DAOFactory.GetKlubDAO();
                    IList<Klub> klubovi = klubDAO.FindAll();
                    foreach (Klub k in klubovi)
                        klubDAO.Delete(k);

                    MestoDAO mestoDAO = DAOFactoryFactory.DAOFactory.GetMestoDAO();
                    IList<Mesto> mesta = mestoDAO.FindAll();
                    foreach (Mesto m in mesta)
                        mestoDAO.Delete(m);

                    SudijaDAO sudijaDAO = DAOFactoryFactory.DAOFactory.GetSudijaDAO();
                    IList<Sudija> sudije = sudijaDAO.FindAll();
                    foreach (Sudija s in sudije)
                        sudijaDAO.Delete(s);

                    DrzavaDAO drzavaDAO = DAOFactoryFactory.DAOFactory.GetDrzavaDAO();
                    IList<Drzava> drzave = drzavaDAO.FindAll();
                    foreach (Drzava d in drzave)
                        drzavaDAO.Delete(d);

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
    }
}
