using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class TakmicenjeDAOImpl : GenericNHibernateDAO<Takmicenje, int>, TakmicenjeDAO
    {
        #region TakmicenjeDAO Members

        public bool existsTakmicenje(string naziv, Gimnastika gim, DateTime datum)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Takmicenje t
                    where t.Naziv like :naziv
                    and t.Gimnastika = :gim
                    and t.Datum = :datum");
                q.SetString("naziv", naziv);
                q.SetByte("gim", (byte)gim);
                q.SetDateTime("datum", datum);
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