using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class TakmicenjeDAOImpl : GenericNHibernateDAO<Takmicenje, int>, TakmicenjeDAO
    {
        #region TakmicenjeDAO Members

        public Takmicenje FindByIdFetch_Kat_Desc(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Takmicenje t
                    left join fetch t.Kategorije
                    left join fetch t.TakmicenjeDescriptions
                    where t.Id = :id");
                q.SetInt32("id", takmicenjeId);
                IList<Takmicenje> result = q.List<Takmicenje>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public Takmicenje FindByIdFetch_Prop_Kat_Desc(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Takmicenje t
                    left join fetch t.TakmicenjeDescriptions d
                    left join fetch d.Propozicije
                    left join fetch t.Kategorije
	                where t.Id = :id");
                q.SetInt32("id", takmicenjeId);
                IList<Takmicenje> result = q.List<Takmicenje>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Takmicenje> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Takmicenje t
                    order by t.Datum desc");
                return q.List<Takmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Takmicenje> FindByGimnastika(Gimnastika gim)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct t
                    from Takmicenje t
                    left join fetch t.Kategorije
                    where t.Gimnastika = :gim
                    order by t.Datum desc");
                q.SetByte("gim", (byte)gim);
                return q.List<Takmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public Takmicenje FindByNazivGimnastikaDatum(string naziv, Gimnastika gim, DateTime datum)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Takmicenje t
                    where t.Naziv like :naziv
                    and t.Gimnastika = :gim
                    and t.Datum = :datum");
                q.SetString("naziv", naziv);
                q.SetByte("gim", (byte)gim);
                q.SetDateTime("datum", datum);
                IList<Takmicenje> result = q.List<Takmicenje>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Takmicenje> FindFinala(Takmicenje t)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Takmicenje t
                    where t.PrvoKolo = :t
                    or t.DrugoKolo = :t
                    or t.TreceKolo = :t
                    or t.CetvrtoKolo = :t");
                q.SetEntity("t", t);
                return q.List<Takmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Takmicenje> FindByTipTakmicenja(TipTakmicenja tip)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Takmicenje t
                    where t.TipTakmicenja = :tip");
                q.SetByte("tip", (byte)tip);
                return q.List<Takmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsTakmicenje(string naziv, Gimnastika gim, DateTime datum)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from Takmicenje t
                    where t.Naziv like :naziv
                    and t.Gimnastika = :gim
                    and t.Datum = :datum");
                q.SetString("naziv", naziv);
                q.SetByte("gim", (byte)gim);
                q.SetDateTime("datum", datum);
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