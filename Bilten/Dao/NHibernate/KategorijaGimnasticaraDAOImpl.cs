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

        public bool existsKategorijaGimnasticara(string naziv, Gimnastika gimnastika)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from KategorijaGimnasticara k
                    where k.Naziv = :naziv
                    and k.Gimnastika = :gimnastika");
                q.SetString("naziv", naziv);
                q.SetByte("gimnastika", (byte)gimnastika);
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