using Bilten.Dao;
using Bilten.Dao.NHibernate;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Bilten.Misc
{
    public class Sesija
    {
        private static Sesija instance;
        public static Sesija Instance
        {
            get
            {
                if (instance == null)
                    instance = new Sesija();
                return instance;
            }
        }

        protected Sesija()
        {
            takmicenjeId = -1;
        }

        public void InitSession()
        {

        }

        public void EndSession()
        {

        }

        private int takmicenjeId;
        public int TakmicenjeId
        { 
            get { return takmicenjeId; }
        }

        public void onTakmicenjeChanged(int takmicenjeId)
        {
            this.takmicenjeId = takmicenjeId;
        }

        public ISession Session = null;
    }
}
