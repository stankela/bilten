using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using Bilten.UI;
using Bilten.Data;
using Bilten.Domain;

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

            // TODO: Can throw InfrastructureException, kako od Opcije.Instance (
            // ucitavanje opcija iz baze) tako i od save(). Verovatno bi
            // trebalo prekinuti program
            if (!Opcije.Instance.Saved)
            {
                // NOTE: Ova naredba se izvrsava samo pri prvom izvrsavanju aplikacije
                Opcije.Instance.save();
            }
            
            // Then create the main form, the splash screen will automatically close
            this.MainForm = new MainForm();
        }
    }
}
