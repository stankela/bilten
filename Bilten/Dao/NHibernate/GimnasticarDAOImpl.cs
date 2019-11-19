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
                IQuery q = Session.CreateQuery(@"
                    from Gimnasticar g
                    left join fetch g.Kategorija
                    left join fetch g.Klub
                    left join fetch g.Drzava
                    order by g.Prezime asc, g.Ime asc");
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindByGimnastika(Gimnastika gim)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Gimnasticar g
                    left join fetch g.Kategorija
                    left join fetch g.Klub
                    left join fetch g.Drzava
                    where g.Gimnastika = :gim
                    order by g.Prezime asc, g.Ime asc");
                q.SetByte("gim", (byte)gim);
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
                IQuery q = Session.CreateQuery(@"
                    from Gimnasticar g
                    where g.Klub = :klub");
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
                IQuery q = Session.CreateQuery(@"
                    from Gimnasticar g
                    where g.Drzava = :drzava");
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
                IQuery q = Session.CreateQuery(@"
                    from Gimnasticar g
                    where g.Kategorija = :kategorija");
                q.SetEntity("kategorija", kategorija);
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindGimnasticariByRegBroj(string regBroj)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Gimnasticar g
                    left join fetch g.Kategorija
                    left join fetch g.Klub
                    left join fetch g.Drzava
                    where g.RegistarskiBroj like :regBroj");
                q.SetString("regBroj", regBroj);
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
            string query = @"
                from Gimnasticar g
                left join fetch g.Kategorija
                left join fetch g.Klub
                left join fetch g.Drzava";
            string WHERE = " where ";
            if (!String.IsNullOrEmpty(ime))
            {
                query += WHERE + "lower(g.Ime) like :ime";
                WHERE = " and ";
            }
            if (!String.IsNullOrEmpty(prezime))
            {
                query += WHERE + "lower(g.Prezime) like :prezime";
                WHERE = " and ";
            }
            if (godRodj != null)
            {
                query += WHERE + "g.DatumRodjenja.Godina = :godRodj";
                WHERE = " and ";
            }
            if (gimnastika != null)
            {
                query += WHERE + "g.Gimnastika = :gimnastika";
                WHERE = " and ";
            }
            if (drzava != null)
            {
                query += WHERE + "g.Drzava = :drzava";
                WHERE = " and ";
            }
            if (kategorija != null)
            {
                query += WHERE + "g.Kategorija = :kategorija";
                WHERE = " and ";
            }
            if (klub != null)
            {
                query += WHERE + "g.Klub = :klub";
                WHERE = " and ";
            }
            query += " order by g.Prezime asc, g.Ime asc";

            IQuery q = Session.CreateQuery(query);
            if (!String.IsNullOrEmpty(ime))
                q.SetString("ime", ime.ToLower() + '%');
            if (!String.IsNullOrEmpty(prezime))
                q.SetString("prezime", prezime.ToLower() + '%');
            if (godRodj != null)
                q.SetInt16("godRodj", (short)godRodj.Value);
            if (gimnastika != null)
                q.SetByte("gimnastika", (byte)gimnastika.Value);
            if (drzava != null)
                q.SetEntity("drzava", drzava);
            if (kategorija != null)
                q.SetEntity("kategorija", kategorija);
            if (klub != null)
                q.SetEntity("klub", klub);
            return q.List<Gimnasticar>();
        }

        public Gimnasticar FindGimnasticar(string ime, string prezime, Nullable<int> danRodj, Nullable<int> mesRodj,
            Nullable<int> godRodj, Nullable<Gimnastika> gimnastika)
        {
            string query = @"
                from Gimnasticar g
                left join fetch g.Kategorija
                left join fetch g.Klub
                left join fetch g.Drzava";
            string WHERE = " where ";
            if (!String.IsNullOrEmpty(ime))
            {
                query += WHERE + "lower(g.Ime) like :ime";
                WHERE = " and ";
            }
            if (!String.IsNullOrEmpty(prezime))
            {
                query += WHERE + "lower(g.Prezime) like :prezime";
                WHERE = " and ";
            }
            if (danRodj != null)
            {
                query += WHERE + "g.DatumRodjenja.Dan = :danRodj";
                WHERE = " and ";
            }
            if (mesRodj != null)
            {
                query += WHERE + "g.DatumRodjenja.Mesec = :mesRodj";
                WHERE = " and ";
            }
            if (godRodj != null)
            {
                query += WHERE + "g.DatumRodjenja.Godina = :godRodj";
                WHERE = " and ";
            }
            if (gimnastika != null)
            {
                query += WHERE + "g.Gimnastika = :gimnastika";
                WHERE = " and ";
            }
            query += " order by g.Prezime asc, g.Ime asc";

            IQuery q = Session.CreateQuery(query);
            if (!String.IsNullOrEmpty(ime))
                q.SetString("ime", ime.ToLower() + '%');
            if (!String.IsNullOrEmpty(prezime))
                q.SetString("prezime", prezime.ToLower() + '%');
            if (danRodj != null)
                q.SetByte("danRodj", (byte)danRodj.Value);
            if (mesRodj != null)
                q.SetByte("mesRodj", (byte)mesRodj.Value);
            if (godRodj != null)
                q.SetInt16("godRodj", (short)godRodj.Value);
            if (gimnastika != null)
                q.SetByte("gimnastika", (byte)gimnastika.Value);
            IList<Gimnasticar> result = q.List<Gimnasticar>();
            if (result != null && result.Count > 0)
                return result[0];
            else
                return null;
        }

        public bool existsGimnasticar(Klub klub)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from Gimnasticar g
                    where g.Klub = :klub");
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
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from Gimnasticar g
                    where g.Drzava = :drzava");
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
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from Gimnasticar g
                    where g.Kategorija = :kategorija");
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
                string query = @"
                    select count(*)
                    from Gimnasticar g
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

        public bool existsGimnasticarRegBroj(string regBroj)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from Gimnasticar g
                    where g.RegistarskiBroj like :regBroj");
                q.SetString("regBroj", regBroj);
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