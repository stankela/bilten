﻿using System;
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
                IQuery q = Session.CreateQuery(@"from RezultatskoTakmicenjeDescription d
                    where d.Takmicenje.Id = :takmicenjeId
                    order by d.RedBroj asc");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RezultatskoTakmicenjeDescription>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}