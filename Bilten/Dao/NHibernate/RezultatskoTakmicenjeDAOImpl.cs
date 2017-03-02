using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class RezultatskoTakmicenjeDAOImpl : GenericNHibernateDAO<RezultatskoTakmicenje, int>, RezultatskoTakmicenjeDAO
    {
        #region RezultatskoTakmicenjeDAO Members

        public RezultatskoTakmicenje loadRezTakmicenje(int rezTakmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.Ekipe e
                    where r.Id = :id");
                q.SetInt32("id", rezTakmicenjeId);
                IList<RezultatskoTakmicenje> result = q.List<RezultatskoTakmicenje>();
                if (result.Count == 0)
                    return null;
                else
                    return result[0];
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RezultatskoTakmicenje> FindRezTakmicenjaForGimnasticar(GimnasticarUcesnik g)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    join r.Takmicenje1 t
                    join t.Gimnasticari g
                    where g = :gimnasticar
                    order by r.RedBroj");
                q.SetEntity("gimnasticar", g);
                return q.List<RezultatskoTakmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RezultatskoTakmicenje> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from RezultatskoTakmicenje r
                    where r.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RezultatskoTakmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTakmicenje2(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje2 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    order by r.RedBroj");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RezultatskoTakmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTakmicenje3(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje3 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    order by r.RedBroj");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RezultatskoTakmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTak_2_3_4(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Propozicije
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje2
                    left join fetch r.Takmicenje3
                    left join fetch r.Takmicenje4
                    where r.Takmicenje.Id = :takmicenjeId
                    order by r.RedBroj");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RezultatskoTakmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public RezultatskoTakmicenje FindByIdFetchTakmicenje3(int rezTakmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje3 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Id = :rezTakmicenjeId");
                q.SetInt32("rezTakmicenjeId", rezTakmicenjeId);                
                IList<RezultatskoTakmicenje> result = q.List<RezultatskoTakmicenje>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public RezultatskoTakmicenje FindByKatDesc(TakmicarskaKategorija kat, RezultatskoTakmicenjeDescription desc)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from RezultatskoTakmicenje r
                    where r.Kategorija = :kat
                    and r.TakmicenjeDescription = :desc");
                q.SetEntity("kat", kat);
                q.SetEntity("desc", desc);
                IList<RezultatskoTakmicenje> result = q.List<RezultatskoTakmicenje>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}