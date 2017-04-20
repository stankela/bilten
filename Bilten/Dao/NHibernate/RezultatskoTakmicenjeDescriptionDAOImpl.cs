using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class RezultatskoTakmicenjeDescriptionDAOImpl : GenericNHibernateDAO<RezultatskoTakmicenjeDescription, int>,
        RezultatskoTakmicenjeDescriptionDAO
    {
        #region RezultatskoTakmicenjeDescriptionDAO Members

        public IList<RezultatskoTakmicenjeDescription> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct d
                    from Takmicenje t
                    join t.TakmicenjeDescriptions d
                    where t.Id = :takmicenjeId
                    order by d.RedBroj asc");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RezultatskoTakmicenjeDescription>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsDescriptionNaziv(string naziv, int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select count(*)
                    from Takmicenje t
                    join t.TakmicenjeDescriptions d
                    where t.Id = :takmicenjeId
                    and d.Naziv like :naziv");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetString("naziv", naziv);
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