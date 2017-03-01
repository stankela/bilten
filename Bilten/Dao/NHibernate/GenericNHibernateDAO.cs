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
                    session = NHibernateHelper.Instance.GetCurrentSession();
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
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
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
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
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
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public void Add(T entity)
        {
            try
            {
                Session.Save(entity);
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public void Update(T entity)
        {
            try
            {
                Session.Update(entity);
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public void Delete(T entity)
        {
            try
            {
                Session.Delete(entity);
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion

        public void Evict(T entity)
        {
            try
            {
                Session.Evict(entity);
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool Contains(T entity)
        {
            try
            {
                return Session.Contains(entity);
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public void Attach(object item, bool update)
        {
            try
            {
                if (update)
                    Session.Update(item);
                else
                    Session.Lock(item, LockMode.None);
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
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
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
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
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }
    }
}