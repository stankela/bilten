using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Bilten.UI;
using System.Threading;
using System.Globalization;
using Bilten.Test;
using Bilten.Domain;

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
            
            //MainForm mainForm = new MainForm();
            //Application.Run(mainForm);
            SingleInstanceApplication.Application.Run(args);
        }
    }
}