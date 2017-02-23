using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class KategorijaGimnasticaraDAOImpl : GenericNHibernateDAO<KategorijaGimnasticara, int>, KategorijaGimnasticaraDAO
    {
        #region KategorijaGimnasticaraDAO Members

        public IList<KategorijaGimnasticara> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from KategorijaGimnasticara");
                return q.List<KategorijaGimnasticara>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }
        
        #endregion
    }
}