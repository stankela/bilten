using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    /// <summary>
    /// NHibernate-specific implementation of <see cref="KlubDAO"/>.
    /// </summary>
    public class KlubDAOImpl : GenericNHibernateDAO<Klub, int>, KlubDAO
    {
        #region KlubDAO Members

        public virtual IList<Klub> FindKlubsByNaziv(string naziv)
        {
            try
            {
                IQuery q = Session.GetNamedQuery("FindKlubsByNaziv");
                q.SetString("naziv", naziv);
                return q.List<Klub>();
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        public virtual IList<Klub> FindKlubsByKod(string kod)
        {
            try
            {
                IQuery q = Session.GetNamedQuery("FindKlubsByKod");
                q.SetString("kod", kod);
                return q.List<Klub>();
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        #endregion
    }
}