using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class EkipaDAOImpl : GenericNHibernateDAO<Ekipa, int>, EkipaDAO
    {
        #region EkipaDAO Members

        public Ekipa FindEkipaById(int id)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct e
                    from Ekipa e
                    left join fetch e.Gimnasticari g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where e.Id = :id");
                q.SetInt32("id", id);
                IList<Ekipa> result = q.List<Ekipa>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Ekipa> FindByGimnasticar(GimnasticarUcesnik g)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct e
                    from Ekipa e
                    join e.Gimnasticari g
                    where g = :g");
                q.SetEntity("g", g);
                return q.List<Ekipa>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsEkipaNaziv(int rezTakmicenjeId, string naziv)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from RezultatskoTakmicenje r
                    join r.Takmicenje1 t
                    join t.Ekipe e
                    where r.Id = :rezTakmicenjeId
                    and e.Naziv = :naziv");
                q.SetInt32("rezTakmicenjeId", rezTakmicenjeId);
                q.SetString("naziv", naziv);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsEkipaKod(int rezTakmicenjeId, string kod)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from RezultatskoTakmicenje r
                    join r.Takmicenje1 t
                    join t.Ekipe e
                    where r.Id = :rezTakmicenjeId
                    and e.Kod = :kod");
                q.SetInt32("rezTakmicenjeId", rezTakmicenjeId);
                q.SetString("kod", kod);
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