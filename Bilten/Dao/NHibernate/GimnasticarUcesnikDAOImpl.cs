using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class GimnasticarUcesnikDAOImpl : GenericNHibernateDAO<GimnasticarUcesnik, int>, GimnasticarUcesnikDAO
    {
        #region GimnasticarUcesnikDAO Members

        public IList<GimnasticarUcesnik> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from GimnasticarUcesnik g
                    where g.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<GimnasticarUcesnik>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<GimnasticarUcesnik> FindByTakmicenjeFetch_Kat_Klub_Drzava(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct g
                    from GimnasticarUcesnik g
                    left join fetch g.TakmicarskaKategorija
                    left join fetch g.KlubUcesnik
                    left join fetch g.DrzavaUcesnik
                    where g.Takmicenje.Id = :takmicenjeId
                    order by g.Prezime asc, g.Ime asc");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<GimnasticarUcesnik>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<GimnasticarUcesnik> FindByTakmicenjeKat(int takmicenjeId, TakmicarskaKategorija kategorija)
        {
            try
            {
                string query = @"from GimnasticarUcesnik g
                    left join fetch g.Takmicenje
                    left join fetch g.TakmicarskaKategorija
                    left join fetch g.KlubUcesnik
                    left join fetch g.DrzavaUcesnik
                    where g.Takmicenje.Id = :takmicenjeId";
                if (kategorija != null)
                    query += " and g.TakmicarskaKategorija = :kategorija";
                query += " order by g.Prezime asc, g.Ime asc";

                IQuery q = Session.CreateQuery(query);
                q.SetInt32("takmicenjeId", takmicenjeId);
                if (kategorija != null)
                    q.SetEntity("kategorija", kategorija);
                return q.List<GimnasticarUcesnik>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<GimnasticarUcesnik> FindGimnasticariUcesnici(string ime,
            string prezime, Nullable<int> godRodj, DrzavaUcesnik drzava, TakmicarskaKategorija kategorija,
            KlubUcesnik klub, int takmicenjeId)
        {
            string query = @"from GimnasticarUcesnik g
                    left join fetch g.Takmicenje
                    left join fetch g.TakmicarskaKategorija
                    left join fetch g.KlubUcesnik
                    left join fetch g.DrzavaUcesnik
                    where g.Takmicenje.Id = :takmicenjeId";
            if (!String.IsNullOrEmpty(ime))
                query += " and lower(g.Ime) like :ime";
            if (!String.IsNullOrEmpty(prezime))
                query += " and lower(g.Prezime) like :prezime";
            if (godRodj != null)
                query += " and g.DatumRodjenja.Godina = :godRodj";
            if (kategorija != null)
                query += " and g.TakmicarskaKategorija = :kategorija";
            if (drzava != null)
                query += " and g.DrzavaUcesnik = :drzava";
            if (klub != null)
                query += " and g.KlubUcesnik = :klub";
            query += " order by g.Prezime asc, g.Ime asc";

            IQuery q = Session.CreateQuery(query);
            q.SetInt32("takmicenjeId", takmicenjeId);
            if (!String.IsNullOrEmpty(ime))
                q.SetString("ime", ime.ToLower() + '%');
            if (!String.IsNullOrEmpty(prezime))
                q.SetString("prezime", prezime.ToLower() + '%');
            if (godRodj != null)
                q.SetInt16("godRodj", (short)godRodj.Value);
            if (kategorija != null)
                q.SetEntity("kategorija", kategorija);
            if (drzava != null)
                q.SetEntity("drzava", drzava);
            if (klub != null)
                q.SetEntity("klub", klub);
            return q.List<GimnasticarUcesnik>();
        }

        public GimnasticarUcesnik FindByTakmicenjeTakBroj(Takmicenje takmicenje, int takBroj)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from GimnasticarUcesnik g
                    where g.TakmicarskiBroj = :takBroj
                    and g.Takmicenje = :takmicenje");
                q.SetInt32("takBroj", takBroj);
                q.SetEntity("takmicenje", takmicenje);
                IList<GimnasticarUcesnik> result = q.List<GimnasticarUcesnik>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }

        }

        public bool existsGimnasticarTakBroj(int takBroj, Takmicenje takmicenje)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from GimnasticarUcesnik g
                    where g.TakmicarskiBroj = :takBroj
                    and g.Takmicenje = :takmicenje");
                q.SetInt32("takBroj", takBroj);
                q.SetEntity("takmicenje", takmicenje);
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