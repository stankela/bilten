using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class NastupNaSpraviDAOImpl : GenericNHibernateDAO<NastupNaSpravi, int>, NastupNaSpraviDAO
    {
        #region NastupNaSpraviDAO Members

        public bool existsNastupForGimnasticar(int gimnasticarId)
        {
            IQuery q = Session.CreateQuery(@"
                select count(*)
                from NastupNaSpravi n
                where n.Gimnasticar.Id = :gimnasticarId");
            q.SetInt32("gimnasticarId", gimnasticarId);
            return (long)q.UniqueResult() > 0;
        }

        #endregion
    }
}