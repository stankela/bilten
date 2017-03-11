﻿using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class Takmicenje3DAOImpl : GenericNHibernateDAO<Takmicenje3, int>, Takmicenje3DAO
    {
        #region Takmicenje3DAO Members

        public bool isGimnasticarUcesnik(int gimnasticarId)
        {
            IQuery q = Session.CreateQuery(@"
                select count(*)
                from Takmicenje3 t
                join t.Ucesnici u
                where u.Gimnasticar.Id = :gimnasticarId");
            q.SetInt32("gimnasticarId", gimnasticarId);
            return (long)q.UniqueResult() > 0;
        }
        
        #endregion
    }
}