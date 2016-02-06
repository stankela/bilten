using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using Bilten.UI;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using System.Collections;

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
            new DataAccessProviderFactory().GetDataContext();

            // TODO: Can throw InfrastructureException, kako od loadOptions()
            // tako i od saveOptions(). Verovatno bi trebalo prekinuti program.
            Opcije opcije = loadOptions();
            if (opcije == null)
            {
                // NOTE: Ova naredba se izvrsava samo pri prvom izvrsavanju aplikacije
                opcije = new Opcije();
                saveOptions(opcije, true);
            }
            Opcije.Instance = opcije;

            // Then create the main form, the splash screen will automatically close
            this.MainForm = new MainForm();
        }

        // TODO4: Ovaj i sledeci metod bi trebali da budu u nekoj DAO klasi.

        public static void saveOptions(Opcije opcije, bool insert)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                if (insert)
                    dataContext.Add(opcije);
                else
                    dataContext.Save(opcije);
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        public static Opcije loadOptions()
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                // mora ovako zato sto metod dataContext.GetAll<Opcije> trazi da 
                // Opcije imaju public contructor, a to nije moguce jer su Opcije 
                // singleton
                string query = @"from Opcije";
                IList result = dataContext.ExecuteQuery(QueryLanguageType.HQL,
                    query, new string[] { }, new object[] { });
                if (result.Count > 0)
                    return (Opcije)result[0];
                else
                    return null;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

    }
}
