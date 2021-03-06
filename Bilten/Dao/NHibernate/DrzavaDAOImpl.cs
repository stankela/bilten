﻿using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class DrzavaDAOImpl : GenericNHibernateDAO<Drzava, int>, DrzavaDAO
    {
        #region DrzavaDAO Members

        public IList<Drzava> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Drzava d order by d.Naziv asc");
                return q.List<Drzava>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public Drzava FindByKod(string kod)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Drzava d where d.Kod like :kod");
                q.SetString("kod", kod);
                IList<Drzava> result = q.List<Drzava>();
                if (result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsDrzavaNaziv(string naziv)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Drzava d where d.Naziv like :naziv");
                q.SetString("naziv", naziv);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsDrzavaKod(string kod)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Drzava d where d.Kod like :kod");
                q.SetString("kod", kod);
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