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
                    where g.TakmicarskaKategorija.Takmicenje.Id = :takmicenjeId");
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
                    left join fetch g.TakmicarskaKategorija k
                    left join fetch g.KlubUcesnik
                    left join fetch g.DrzavaUcesnik
                    where k.Takmicenje.Id = :takmicenjeId
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
                    left join fetch g.TakmicarskaKategorija k
                    left join fetch g.KlubUcesnik
                    left join fetch g.DrzavaUcesnik
                    where k.Takmicenje.Id = :takmicenjeId";
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

        public IList<GimnasticarUcesnik> FindByKategorija(TakmicarskaKategorija kategorija)
        {
            try
            {
                string query = @"from GimnasticarUcesnik g
                    left join fetch g.KlubUcesnik
                    left join fetch g.DrzavaUcesnik
                    where g.TakmicarskaKategorija = :kategorija
                    order by g.Prezime asc, g.Ime asc";
                IQuery q = Session.CreateQuery(query);
                q.SetEntity("kategorija", kategorija);
                return q.List<GimnasticarUcesnik>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }
        
        public IList<GimnasticarUcesnik> FindGimnasticariUcesnici(string ime, string prezime, DrzavaUcesnik drzava,
            TakmicarskaKategorija kategorija, KlubUcesnik klub, int takmicenjeId)
        {
            string query = @"from GimnasticarUcesnik g
                    left join fetch g.TakmicarskaKategorija k
                    left join fetch g.KlubUcesnik
                    left join fetch g.DrzavaUcesnik
                    where k.Takmicenje.Id = :takmicenjeId";
            if (!String.IsNullOrEmpty(ime))
                query += " and lower(g.Ime) like :ime";
            if (!String.IsNullOrEmpty(prezime))
                query += " and lower(g.Prezime) like :prezime";
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
            if (kategorija != null)
                q.SetEntity("kategorija", kategorija);
            if (drzava != null)
                q.SetEntity("drzava", drzava);
            if (klub != null)
                q.SetEntity("klub", klub);
            return q.List<GimnasticarUcesnik>();
        }

        #endregion
    }
}