using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class SudijaDAOImpl : GenericNHibernateDAO<Sudija, int>, SudijaDAO
    {
        #region SudijaDAO Members

        public IList<Sudija> FindSudijeByDrzava(Drzava drzava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Sudija s where s.Drzava = :drzava");
                q.SetEntity("drzava", drzava);
                return q.List<Sudija>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Sudija> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Sudija s
                    left join fetch s.Drzava
                    left join fetch s.Klub
                    order by s.Prezime asc, s.Ime asc");
                return q.List<Sudija>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Sudija> FindSudije(string ime, string prezime, Pol? pol, Drzava drzava, Klub klub)
        {
            string query = @"
                from Sudija s
                left join fetch s.Klub
                left join fetch s.Drzava";
            string WHERE = " where ";
            if (!String.IsNullOrEmpty(ime))
            {
                query += WHERE + "lower(s.Ime) like :ime";
                WHERE = " and ";
            }
            if (!String.IsNullOrEmpty(prezime))
            {
                query += WHERE + "lower(s.Prezime) like :prezime";
                WHERE = " and ";
            }
            if (pol != null)
            {
                query += WHERE + "s.Pol = :pol";
                WHERE = " and ";
            }
            if (drzava != null)
            {
                query += WHERE + "s.Drzava = :drzava";
                WHERE = " and ";
            }
            if (klub != null)
            {
                query += WHERE + "s.Klub = :klub";
                WHERE = " and ";
            }
            query += " order by s.Prezime asc, s.Ime asc";

            IQuery q = Session.CreateQuery(query);
            if (!String.IsNullOrEmpty(ime))
                q.SetString("ime", ime.ToLower() + '%');
            if (!String.IsNullOrEmpty(prezime))
                q.SetString("prezime", prezime.ToLower() + '%');
            if (pol != null)
                q.SetByte("pol", (byte)pol.Value);
            if (drzava != null)
                q.SetEntity("drzava", drzava);
            if (klub != null)
                q.SetEntity("klub", klub);
            return q.List<Sudija>();
        }

        public bool existsSudija(Drzava drzava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Sudija s where s.Drzava = :drzava");
                q.SetEntity("drzava", drzava);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsSudija(string ime, string prezime)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Sudija s
                    where s.Ime like :ime
                    and s.Prezime like :prezime");
                q.SetString("ime", ime);
                q.SetString("prezime", prezime);
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