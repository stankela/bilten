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

        public RezultatskoTakmicenje FindByIdFetch_Ekipe(int rezTakmicenjeId)
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_KatDesc(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_Gimnasticari(int takmicenjeId)
        {
            // NOTE: Kada umesto d stavim desc za TakmicenjeDescription dobijam
            // gresku. Verovatno je desc neka interna promenljiva koju NHibernate
            // koristi.
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.Gimnasticari g
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

        public RezultatskoTakmicenje FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(int takmicenjeId,
            string nazivKategorije, byte redBrojDesc)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.Gimnasticari g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    and kat.Naziv like :nazivKategorije
                    and d.RedBroj = :redBrojDesc
                    order by r.RedBroj");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetString("nazivKategorije", nazivKategorije);
                q.SetByte("redBrojDesc", redBrojDesc);
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakUkupno_Gimnasticari(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakUkupno
                    left join fetch t.Gimnasticari g
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

        public IList<RezultatskoTakmicenje>
            FindByTakmicenjeFetch_KatDesc_Tak1_PoredakUkupnoFinaleKupa_KlubDrzava(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakUkupnoFinaleKupa
                    left join fetch t.Gimnasticari g
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

        public IList<RezultatskoTakmicenje>
            FindByTakmicenjeFetch_KatDesc_Tak1_PoredakUkupnoZbirViseKola_KlubDrzava(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakUkupnoZbirViseKola
                    left join fetch t.Gimnasticari g
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakEkipnoFinaleKupa(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakEkipnoFinaleKupa
                    left join fetch t.Ekipe e
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakEkipnoZbirViseKola(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakEkipnoZbirViseKola
                    left join fetch t.Ekipe e
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakSprava(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakSprava
                    left join fetch t.PoredakPreskok
                    left join fetch t.Gimnasticari g
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakSpravaFinaleKupa(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakSpravaFinaleKupa
                    left join fetch t.Gimnasticari g
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_Gimnasticari_PoredakEkipno(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakEkipno
                    left join fetch t.Ekipe e
                    left join fetch e.Gimnasticari g
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

        public RezultatskoTakmicenje FindByTakmicenjeFetch_Tak1_PoredakEkipno_Ekipe(int takmicenjeId,
            string nazivKategorije, byte redBrojDesc)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakEkipno
                    left join fetch t.Ekipe e
                    where r.Takmicenje.Id = :takmicenjeId
                    and kat.Naziv like :nazivKategorije
                    and d.RedBroj = :redBrojDesc
                    order by r.RedBroj");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetString("nazivKategorije", nazivKategorije);
                q.SetByte("redBrojDesc", redBrojDesc);
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak3_Poredak(int takmicenjeId)
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
                    left join fetch t.PoredakPreskok
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

        public IList<RezultatskoTakmicenje> FindByTakmicenjeKatFetch_Tak3_Poredak(int takmicenjeId, TakmicarskaKategorija kat)
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
                    left join fetch t.PoredakPreskok
                    left join fetch t.Ucesnici u
                    left join fetch u.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    and r.Kategorija = :kat
                    order by r.RedBroj");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetEntity("kat", kat);
                return q.List<RezultatskoTakmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak4_Gimnasticari_Poredak(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje4 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Ekipa e
                    left join fetch e.Gimnasticari g
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

        public RezultatskoTakmicenje FindByIdFetch_Tak1_Gimnasticari_PoredakEkipno(int rezTakmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakEkipno
                    left join fetch t.Ekipe e
                    left join fetch e.Gimnasticari g
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

        public IList<RezultatskoTakmicenje> FindByKategorija(TakmicarskaKategorija kat)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from RezultatskoTakmicenje r
                    where r.Kategorija = :kat
                    order by r.RedBroj");
                q.SetEntity("kat", kat);
                return q.List<RezultatskoTakmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<RezultatskoTakmicenje> FindEkipnaTakmicenja(int takmicenjeId)
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
                    where r.Takmicenje.Id = :takmicenjeId
                    and r.ImaEkipnoTakmicenje = true
                    order by r.RedBroj");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RezultatskoTakmicenje>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }                                 
        }

        public int FindMaxRedBroj(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select max(r.RedBroj)
                    from RezultatskoTakmicenje r
                    where r.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return (byte)q.UniqueResult();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}