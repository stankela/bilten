using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class OcenaDAOImpl : GenericNHibernateDAO<Ocena, int>, OcenaDAO
    {
        #region OcenaDAO Members

        public IList<Ocena> FindByGimnasticar(GimnasticarUcesnik gim, DeoTakmicenjaKod deoTakKod)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select o
                    from Ocena o
                    left join fetch o.Ocena2
                    join o.Gimnasticar g
                    where g = :gim
                    and o.DeoTakmicenjaKod = :deoTakKod");
                q.SetEntity("gim", gim);
                q.SetByte("deoTakKod", (byte)deoTakKod);
                return q.List<Ocena>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Ocena> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Ocena o
                    where o.Gimnasticar.TakmicarskaKategorija.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<Ocena>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Ocena> FindByDeoTakmicenja(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select o
                    from Ocena o
                    left join fetch o.Ocena2
                    join o.Gimnasticar g
                    where g.TakmicarskaKategorija.Takmicenje.Id = :takmicenjeId
                    and o.DeoTakmicenjaKod = :deoTakKod");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetByte("deoTakKod", (byte)deoTakKod);
                return q.List<Ocena>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Ocena> FindByKatSpravaDeoTak(TakmicarskaKategorija kategorija, Sprava sprava,
            DeoTakmicenjaKod deoTakKod)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select o
                    from Ocena o
                    left join fetch o.Ocena2
                    join fetch o.Gimnasticar g
                    join fetch g.TakmicarskaKategorija kat
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where kat = :kategorija
                    and o.Sprava = :sprava
                    and o.DeoTakmicenjaKod = :deoTakKod");
                q.SetEntity("kategorija", kategorija);
                q.SetByte("sprava", (byte)sprava);
                q.SetByte("deoTakKod", (byte)deoTakKod);
                return q.List<Ocena>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public Ocena FindByIdFetch(int id)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select o
                    from Ocena o
                    left join fetch o.Ocena2
                    join fetch o.Gimnasticar g
                    join fetch g.TakmicarskaKategorija kat
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where o.Id = :id");
                q.SetInt32("id", id);
                IList<Ocena> result = q.List<Ocena>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public Ocena FindOcena(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod, Sprava sprava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select o
                    from Ocena o
                    join o.Gimnasticar g
                    where g.Id = :gimnasticarId
                    and o.DeoTakmicenjaKod = :deoTakKod
                    and o.Sprava = :sprava");
                q.SetInt32("gimnasticarId", g.Id);
                q.SetByte("deoTakKod", (byte)deoTakKod);
                q.SetByte("sprava", (byte)sprava);
                IList<Ocena> result = q.List<Ocena>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsOcene(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from Ocena o
                    where o.Gimnasticar.TakmicarskaKategorija.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
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