using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using Bilten.UI;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using System.Collections;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Misc;

namespace Bilten
{
    class SingleInstanceApplication : WindowsFormsApplicationBase
    {
        private static SingleInstanceApplication application;

        internal static SingleInstanceApplication Application
        {
            get
            {
                if (application == null)
                    application = new SingleInstanceApplication();
                return application;
            }
        }

        // Must call base constructor to ensure correct initial 
        // WindowsFormsApplicationBase configuration
        public SingleInstanceApplication()
        {
            // This ensures the underlying single-SDI framework is employed, 
            // and OnStartupNextInstance is fired
            this.IsSingleInstance = true;
        }

        protected override void OnCreateSplashScreen()
        {
            this.SplashScreen = new SplashScreenForm();
        }

        protected override void OnCreateMainForm()
        {
            // Do your initialization here

            // This creates singleton instance of NHibernateHelper and builds session factory
            NHibernateHelper nh = NHibernateHelper.Instance;

            Sesija.Instance.InitSession();

            // TODO: Can throw InfrastructureException. Verovatno bi trebalo prekinuti program.

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    OpcijeDAO opcijeDAO = DAOFactoryFactory.DAOFactory.GetOpcijeDAO();
                    Opcije opcije = opcijeDAO.FindOpcije();
                    if (opcije == null)
                    {
                        // NOTE: Ova naredba se izvrsava samo pri prvom izvrsavanju aplikacije
                        opcije = new Opcije();
                        opcijeDAO.Add(opcije);
                        session.Transaction.Commit();
                    }
                    Opcije.Instance = opcije;
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            // Then create the main form, the splash screen will automatically close
            this.MainForm = new MainForm();
        }
    }
}
