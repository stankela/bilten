using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class RasporedNastupaDAOImpl : GenericNHibernateDAO<RasporedNastupa, int>, RasporedNastupaDAO
    {
        #region RasporedNastupaDAO Members

        public IList<RasporedNastupa> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select distinct r
                        from RasporedNastupa r
                        where r.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RasporedNastupa>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public RasporedNastupa FindByIdFetch(int rasporedId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select distinct r
                    from RasporedNastupa r
                    left join fetch r.StartListe s
                    left join fetch s.Nastupi n
                    left join fetch n.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    left join fetch g.TakmicarskaKategorija
                    where r.Id = :rasporedId");
                q.SetInt32("rasporedId", rasporedId);
                IList<RasporedNastupa> result = q.List<RasporedNastupa>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RasporedNastupa> FindByTakmicenjeDeoTak(int takmicenjeId, DeoTakmicenjaKod deoTak)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RasporedNastupa r
                    left join fetch r.StartListe s
                    left join fetch s.Nastupi n
                    left join fetch n.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    left join fetch g.TakmicarskaKategorija
                    where r.DeoTakmicenjaKod = :deoTak
                    and r.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetByte("deoTak", (byte)deoTak);
                return q.List<RasporedNastupa>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RasporedNastupa> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from RasporedNastupa r");
                return q.List<RasporedNastupa>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}