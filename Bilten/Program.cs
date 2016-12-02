using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Bilten.UI;
using System.Threading;
using System.Globalization;
using Bilten.Test;
using Bilten.Domain;
using System.Diagnostics;
using System.IO;

namespace Bilten
{
    static class Program
    {
        static int VERZIJA_PROGRAMA = 2;

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

            int verzijaBaze = DatabaseUpdater.getDatabaseVersionNumber();
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

                if (verzijaBaze == 1 && VERZIJA_PROGRAMA > 1)
                {
                    string databaseFile = "BiltenPodaci.sdf";
                    SqlCeUtilities.ExecuteScript(databaseFile, "", Path.GetFullPath(@"..\..\DatabaseUpdate_version2.sql"));

                    verzijaBaze = DatabaseUpdater.getDatabaseVersionNumber();
                    string msg = String.Format("Baza podataka je konvertovana u verziju {0}.", verzijaBaze);
                    MessageBox.Show(msg, "Bilten");
                }
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