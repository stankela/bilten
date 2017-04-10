using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using Bilten.Test;
using Bilten.Domain;
using System.Diagnostics;
using System.IO;
using Bilten.Dao;
using Bilten.Exceptions;
using NHibernate.Context;
using Bilten.Data;
using Bilten.Misc;
using NHibernate;

namespace Bilten
{
    static class Program
    {
        // TODO4: (sa Memorijala 2016)
        // - Proveri OcenaForm (polje za E ocenu se ne ponasa ocekivano u sledece 3 situacije: kada se ostavi prazno,
        //   kada se ocena unese sa tackom umesto zareza, i kada se unese neki proizvoljan tekst)
        // - Dodati u propozicijama nacin razresavanja kada dva takmicara imaju istu konacnu ocenu
        //   (razresavanje za vezbe na spravi neka bude u propozicijama za takmicenje III itd.)
        // - Mislim da u programu postoji na nekoliko mesta obaveza da se kod menja ako se u neku
        //   klasu doda novo svojstvo (cini mi se da je prozor za propozicije jedno od tih mesta). Probaj da
        //   napises unit testove koji bi ovo proveravali sa refleksijom. Npr. test bi sadrzao listu svojstva koja
        //   su postojala u klasi u trenutku kada je test napisan, i onda bi refleksijom ocitao svojstva klase i
        //   proverio da li se liste poklapaju. Kada test pukne, odgovarajuci kod u programu treba apdejtovati, a test
        //   treba promeniti da u proveru ukljucuje i novododato svojstvo. Ima kod Petzolda u poglavlju 4 primer
        //   reflekcije (SysInfoReflection).
        // - Kada se otvaraju rezultati iz prozora za start liste, treba otvoriti onu kategoriju i onu spravu
        //   koja je trenutno aktivna.
        // - Kad oznacavas grupe za rotiranje u start listama, oznaci ih i u ostalim rotacijama a ne samo prvoj.
        // - Drag and drop za menjanje start lista
        // - Penalizacija za viseboj
        // - Ekipni poredak za Memorijal da se automatski izracuna.
        // - Ekipni poredak za Memorijal da se izracuna za sve ekipe, bez obzira na to da li ekipa ima gimnasticare
        //   u svim kategorijama.
        // - Stampanje gimnasticara iz Registra.
        // - Uvoz takmicenja.

        static int VERZIJA_PROGRAMA = 13;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Export schema from BiltenPodaci.sdf using the ExportSqlCe.exe tool.
            // For usage information, just run "ExportSqlCe.exe" without any arguments.            
            //string schemaFile = "BiltenPodaciScript.sql";
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = @"..\..\..\..\Bilten\Libs\ExportSqlCe.exe";
            //startInfo.Arguments = String.Format("\"Data Source=BiltenPodaci.sdf\" {0} schemaonly", schemaFile);
            //var process = Process.Start(startInfo);
            //process.WaitForExit();

            //Bilten.Util.DFS dfs = new Bilten.Util.DFS();
            //dfs.createGraphFromExportSqlCeStript(schemaFile);
            //dfs.doDFS();


            // NOTE: Prebacio sam ovde inicijalizaciju opcija jer se opcije koriste u nekim od update metoda.

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

            try
            {
                int verzijaBaze = SqlCeUtilities.getDatabaseVersionNumber();
                if (verzijaBaze != VERZIJA_PROGRAMA)
                {
                    if (verzijaBaze == 0)
                    {
                        string msg = "Bazu podataka je nemoguce konvertovati da radi sa trenutnom verzijom programa.";
                        MessageBox.Show(msg, "Bilten");
                        return;
                    }
                    if (verzijaBaze > VERZIJA_PROGRAMA)
                    {
                        string msg = "Greska u programu. Verzija baze je veca od verzije programa.";
                        MessageBox.Show(msg, "Bilten");
                        return;
                    }

                    bool converted = false;
                    if (verzijaBaze == 1 && VERZIJA_PROGRAMA > 1)
                    {
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version2.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(2);
                        verzijaBaze = 2;
                        converted = true;
                    }

                    // Precica
                    if (verzijaBaze == 2 && VERZIJA_PROGRAMA == 13)
                    {
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version4.sql", true);

                        SqlCeUtilities.dropReferentialConstraint("ekipe", "klubovi_ucesnici");
                        SqlCeUtilities.dropReferentialConstraint("ekipe", "drzave_ucesnici");
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version5.sql", true);

                        SqlCeUtilities.dropReferentialConstraint("gimnasticari_ucesnici", "takmicenja");
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version6.sql", true);

                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version8.sql", true);

                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version9.sql", true);

                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version10.sql", true);

                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version11.sql", true);

                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version12.sql", true);

                        new VersionUpdater().updateVersion3();
                        new VersionUpdater().updateVersion7();
                        new VersionUpdater().updateVersion13();

                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version13.sql", true);

                        SqlCeUtilities.updateDatabaseVersionNumber(13);
                        verzijaBaze = 13;
                        converted = true;
                    }

                    if (verzijaBaze == 2 && VERZIJA_PROGRAMA > 2)
                    {
                        // TODO4: Dodati prozor koji prikazuje da se baza apdejtuje, posto apdejt traje desetak sekundi.
                        new VersionUpdater().updateVersion3();
                        SqlCeUtilities.updateDatabaseVersionNumber(3);
                        verzijaBaze = 3;
                        converted = true;
                    }

                    if (verzijaBaze == 3 && VERZIJA_PROGRAMA > 3)
                    {
                        // TODO: Ove dve naredbe bi trebalo izvrsavati u okviru jedne transakcije. Isto i za ostale verzije.
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version4.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(4);
                        verzijaBaze = 4;
                        converted = true;
                    }

                    if (verzijaBaze == 4 && VERZIJA_PROGRAMA > 4)
                    {
                        SqlCeUtilities.dropReferentialConstraint("ekipe", "klubovi_ucesnici");
                        SqlCeUtilities.dropReferentialConstraint("ekipe", "drzave_ucesnici");
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version5.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(5);
                        verzijaBaze = 5;
                        converted = true;
                    }

                    if (verzijaBaze == 5 && VERZIJA_PROGRAMA > 5)
                    {
                        SqlCeUtilities.dropReferentialConstraint("gimnasticari_ucesnici", "takmicenja");
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version6.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(6);
                        verzijaBaze = 6;
                        converted = true;
                    }

                    if (verzijaBaze == 6 && VERZIJA_PROGRAMA > 6)
                    {
                        new VersionUpdater().updateVersion7();
                        SqlCeUtilities.updateDatabaseVersionNumber(7);
                        verzijaBaze = 7;
                        converted = true;
                    }

                    if (verzijaBaze == 7 && VERZIJA_PROGRAMA > 7)
                    {
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version8.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(8);
                        verzijaBaze = 8;
                        converted = true;
                    }

                    if (verzijaBaze == 8 && VERZIJA_PROGRAMA > 8)
                    {
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version9.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(9);
                        verzijaBaze = 9;
                        converted = true;
                    }

                    if (verzijaBaze == 9 && VERZIJA_PROGRAMA > 9)
                    {
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version10.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(10);
                        verzijaBaze = 10;
                        converted = true;
                    }

                    if (verzijaBaze == 10 && VERZIJA_PROGRAMA > 10)
                    {
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version11.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(11);
                        verzijaBaze = 11;
                        converted = true;
                    }

                    if (verzijaBaze == 11 && VERZIJA_PROGRAMA > 11)
                    {
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version12.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(12);
                        verzijaBaze = 12;
                        converted = true;
                    }

                    if (verzijaBaze == 12 && VERZIJA_PROGRAMA > 12)
                    {
                        new VersionUpdater().updateVersion13();
                        SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                            "Bilten.Update.DatabaseUpdate_version13.sql", true);
                        SqlCeUtilities.updateDatabaseVersionNumber(13);
                        verzijaBaze = 13;
                        converted = true;
                    }

                    if (converted)
                    {
                        string msg = String.Format("Baza podataka je konvertovana u verziju {0}.", verzijaBaze);
                        MessageBox.Show(msg, "Bilten");

                        if (File.Exists("NHibernateConfig"))
                        {
                            File.Delete("NHibernateConfig");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            Language.SetKeyboardLanguage(Language.acKeyboardLanguage.hklSerbianLatin);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("sr-Latn-CS");
            // ili
            // Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("sr-Cyrl-CS");
            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

      //      new RegistarInitializer().insert();
      //      new MilanoInitializer(Gimnastika.MSG).insert();
      //      new MilanoInitializer(Gimnastika.ZSG).insert();

            
            // Kreiranje prazne baze
            //new SqlCeUtilities().CreateDatabase(@"..\..\clanovi_podaci2.sdf", "sdv");
            
            //new DatabaseUpdater().updateDatabase();
       
            //MainForm mainForm = new MainForm();
            //Application.Run(mainForm);
            SingleInstanceApplication.Application.Run(args);
        }
    }
}