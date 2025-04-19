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
using System.IO;

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
            if (!File.Exists(ConfigurationParameters.DatabaseFile))
            {
                string programName = "Bilten";
                if (MessageDialogs.queryConfirmation("Ne postoji baza podataka '"
                    + ConfigurationParameters.DatabaseFile + "'. Da li zelite da kreirate novu praznu bazu "
                    + "podataka?", programName))
                {
                    SqlCeUtilities.CreateDatabase(ConfigurationParameters.DatabaseFile,
                        ConfigurationParameters.Password, false);
                    // Update broj verzije baze
                    SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, ConfigurationParameters.Password,
                        "Bilten.Update.DatabaseUpdate_version0.txt", true);
                    SqlCeUtilities.updateDatabaseVersionNumber(Program.VERZIJA_PROGRAMA);

                    MessageDialogs.showMessage("Kreirana nova prazna baza podataka.", programName);
                    if (File.Exists("NHibernateConfig"))
                        File.Delete("NHibernateConfig");
                }
            }

            if (VersionUpdater.hasUpdates())
            {
                WaitForm wf = new WaitForm();
                wf.Message = "Azuriram bazu podataka. Sacekajte ...";
                this.SplashScreen = wf;
            }
            else
                this.SplashScreen = new SplashScreenForm();
        }

        protected override void OnCreateMainForm()
        {
            // Do your initialization here

            string initError = String.Empty;
            try
            {
                Program.init();
            }
            catch (Exception ex)
            {
                initError = ex.Message;
            }

            // Then create the main form, the splash screen will automatically close
            this.MainForm = new MainForm(initError);
        }
    }
}
