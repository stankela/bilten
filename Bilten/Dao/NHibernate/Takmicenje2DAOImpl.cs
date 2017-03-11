using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class Takmicenje2DAOImpl : GenericNHibernateDAO<Takmicenje2, int>, Takmicenje2DAO
    {
        #region Takmicenje2DAO Members

        public bool isGimnasticarUcesnik(int gimnasticarId)
        {
            IQuery q = Session.CreateQuery(@"
                select count(*)
                from Takmicenje2 t
                join t.Ucesnici u
                where u.Gimnasticar.Id = :gimnasticarId");
            q.SetInt32("gimnasticarId", gimnasticarId);
            return (long)q.UniqueResult() > 0;
        }

        #endregion
    }
}