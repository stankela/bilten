using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class GimnasticarDAOImpl : GenericNHibernateDAO<Gimnasticar, int>, GimnasticarDAO
    {
        #region GimnasticarDAO Members

        public IList<Gimnasticar> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g
                    left join fetch g.Kategorija
                    left join fetch g.Klub
                    left join fetch g.Drzava");
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindAllNoFetch()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g
                    order by g.Prezime asc, g.Ime asc");
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindGimnasticariByKlub(Klub klub)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g where g.Klub = :klub");
                q.SetEntity("klub", klub);
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindGimnasticariByDrzava(Drzava drzava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g where g.Drzava = :drzava");
                q.SetEntity("drzava", drzava);
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindGimnasticariByKategorija(KategorijaGimnasticara kategorija)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g where g.Kategorija = :kategorija");
                q.SetEntity("kategorija", kategorija);
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindGimnasticariByRegBroj(RegistarskiBroj regBroj)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g
                    left join fetch g.Kategorija
                    left join fetch g.Klub
                    left join fetch g.Drzava
                    where g.RegistarskiBroj.Broj = :broj
                    and g.RegistarskiBroj.GodinaRegistracije = :godina");
                q.SetInt32("broj", regBroj.Broj);
                q.SetInt16("godina", regBroj.GodinaRegistracije);
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindGimnasticari(string ime, string prezime,
            Nullable<int> godRodj, Nullable<Gimnastika> gimnastika, Drzava drzava,
            KategorijaGimnasticara kategorija, Klub klub)
        {
            string query = @"from Gimnasticar g
                    left join fetch g.Kategorija
                    left join fetch g.Klub
                    left join fetch g.Drzava";
            string WHERE = " where ";
            bool addIme = false;
            bool addPrezime = false;
            bool addGodRodj = false;
            bool addGimnastika = false;
            bool addDrzava = false;
            bool addKategorija = false;
            bool addKlub = false;
            if (!String.IsNullOrEmpty(ime))
            {
                query += WHERE + "lower(g.Ime) like :ime";
                WHERE = " and ";
                addIme = true;
            }
            if (!String.IsNullOrEmpty(prezime))
            {
                query += WHERE + "lower(g.Prezime) like :prezime";
                WHERE = " and ";
                addPrezime = true;
            }
            if (godRodj != null)
            {
                query += WHERE + "g.DatumRodjenja.Godina = :godRodj";
                WHERE = " and ";
                addGodRodj = true;
            }
            if (gimnastika != null)
            {
                query += WHERE + "g.Gimnastika = :gimnastika";
                WHERE = " and ";
                addGimnastika = true;
            }
            if (drzava != null)
            {
                query += WHERE + "g.Drzava = :drzava";
                WHERE = " and ";
                addDrzava = true;
            }
            if (kategorija != null)
            {
                query += WHERE + "g.Kategorija = :kategorija";
                WHERE = " and ";
                addKategorija = true;
            }
            if (klub != null)
            {
                query += WHERE + "g.Klub = :klub";
                WHERE = " and ";
                addKlub = true;
            }
            query += " order by g.Prezime asc, g.Ime asc";

            IQuery q = Session.CreateQuery(query);
            if (addIme)
                q.SetString("ime", ime.ToLower() + '%');
            if (addPrezime)
                q.SetString("prezime", prezime.ToLower() + '%');
            if (addGodRodj)
                q.SetInt16("godRodj", (short)godRodj.Value);
            if (addGimnastika)
                q.SetByte("gimnastika", (byte)gimnastika.Value);
            if (addDrzava)
                q.SetEntity("drzava", drzava);
            if (addKategorija)
                q.SetEntity("kategorija", kategorija);
            if (addKlub)
                q.SetEntity("klub", klub);
            return q.List<Gimnasticar>();
        }

        public bool existsGimnasticar(Klub klub)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Gimnasticar g where g.Klub = :klub");
                q.SetEntity("klub", klub);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsGimnasticar(Drzava drzava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Gimnasticar g where g.Drzava = :drzava");
                q.SetEntity("drzava", drzava);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsGimnasticar(KategorijaGimnasticara kategorija)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Gimnasticar g where g.Kategorija = :kategorija");
                q.SetEntity("kategorija", kategorija);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsGimnasticarImePrezimeSrednjeImeDatumRodjenja(string ime, string prezime, string srednjeIme,
            Datum datumRodj)
        {
            try
            {
                string query = @"select count(*) from Gimnasticar g
                    where g.Ime like :ime
                    and g.Prezime like :prezime";
                if (!string.IsNullOrEmpty(srednjeIme))
                    query += " and g.SrednjeIme like :srednjeIme";
                else
                    query += " and g.SrednjeIme is null";
                if (datumRodj != null && datumRodj.Dan != null)
                    query += " and g.DatumRodjenja.Dan = :dan";
                else
                    query += " and g.DatumRodjenja.Dan is null";
                if (datumRodj != null && datumRodj.Mesec != null)
                    query += " and g.DatumRodjenja.Mesec = :mesec";
                else
                    query += " and g.DatumRodjenja.Mesec is null";
                if (datumRodj != null && datumRodj.Godina != null)
                    query += " and g.DatumRodjenja.Godina = :godina";
                else
                    query += " and g.DatumRodjenja.Godina is null";
                                                           
                IQuery q = Session.CreateQuery(query);
                q.SetString("ime", ime);
                q.SetString("prezime", prezime);
                if (!string.IsNullOrEmpty(srednjeIme))
                    q.SetString("srednjeIme", srednjeIme);
                if (datumRodj != null && datumRodj.Dan != null)
                    q.SetByte("dan", datumRodj.Dan.Value);
                if (datumRodj != null && datumRodj.Mesec != null)
                    q.SetByte("mesec", datumRodj.Mesec.Value);
                if (datumRodj != null && datumRodj.Godina != null)
                    q.SetInt16("godina", datumRodj.Godina.Value);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsGimnasticarRegBroj(RegistarskiBroj regBroj)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Gimnasticar g
                    where g.RegistarskiBroj.Broj = :broj
                    and g.RegistarskiBroj.GodinaRegistracije = :godina");
                q.SetInt32("broj", regBroj.Broj);
                q.SetInt16("godina", regBroj.GodinaRegistracije);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}