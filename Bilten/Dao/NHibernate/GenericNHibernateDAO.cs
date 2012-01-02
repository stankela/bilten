using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Bilten.Exceptions;
using Bilten.Data;

namespace Bilten.Dao.NHibernate
{
    /**
	 * Implements the generic CRUD data access operations using Hibernate APIs.
	 * <p>
	 * To write a DAO, subclass and parameterize this class with your persistent class.
	 * Of course, assuming that you have a traditional 1:1 appraoch for Entity:DAO design.
	 * <p>
	 * You have to inject a current Hibernate <tt>Session</tt> to use a DAO. Otherwise, this
	 * generic implementation will use <tt>NHibernateHelper.GetCurrentSession()</tt> to obtain the
	 * current <tt>Session</tt>.
	 *
	 * @see NHibernateDAOFactory
	 */

    public abstract class GenericNHibernateDAO<T, ID> : GenericDAO<T, ID>
    {
        private ISession session;

        public ISession Session
        {
            get
            {
                if (session == null)
                    session = NHibernateHelper.GetCurrentSession();
                return session;
            }
            set { session = value; }
        }

        #region GenericDAO<T,ID> Members

        public T FindById(ID id)
        {
            try
            {
                return Session.Load<T>(id);
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        public T FindByIdAndLock(ID id)
        {
            try
            {
                return Session.Load<T>(id, LockMode.Upgrade);
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        public IList<T> FindAll()
        {
            return FindByCriteria();
        }

        public IList<T> FindByExample(T exampleInstance, params string[] excludeProperty)
        {
            try
            {
                ICriteria crit = Session.CreateCriteria(typeof(T));
                Example example = Example.Create(exampleInstance);
                foreach (string exclude in excludeProperty)
                    example.ExcludeProperty(exclude);
                crit.Add(example);
                return crit.List<T>();
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
    }

        public T MakePersistent(T entity)
        {
            try
            {
                Session.SaveOrUpdate(entity);
                return entity;
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        public void MakeTransient(T entity)
        {
            try
            {
                Session.Delete(entity);
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        public void Flush()
        {
            try
            {
                Session.Flush();
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        public void Clear()
        {
            try
            {
                Session.Clear();
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        #endregion

        /// <summary>
        /// Use this inside subclasses as a convenience method.
        /// </summary>
        protected IList<T> FindByCriteria(params ICriterion[] criterion)
        {
            try
            {
                ICriteria crit = Session.CreateCriteria(typeof(T));
                foreach (ICriterion c in criterion)
                    crit.Add(c);
                return crit.List<T>();
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }
    }
}