using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Bilten.UI;
using System.Threading;
using System.Globalization;
using Bilten.Test;
using Bilten.Domain;
using System.Diagnostics;

namespace Bilten
{
    static class Program
    {
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
            //startInfo.FileName = @"C:\Users\sale\Documents\Visual Studio 2008\Projects\Bilten\Libs\ExportSqlCe.exe";
            //startInfo.Arguments = String.Format("\"Data Source=BiltenPodaci.sdf\" {0} schemaonly", schemaFile);
            //var process = Process.Start(startInfo);
            //process.WaitForExit();

            //Bilten.Util.DFS dfs = new Bilten.Util.DFS();
            //dfs.createGraphFromExportSqlCeStript(schemaFile);
            //dfs.doDFS();


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