using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using Bilten.Test;
using Bilten.Domain;
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
        // - Kad oznacavas grupe za rotiranje u start listama, oznaci ih i u ostalim rotacijama a ne samo prvoj.
        // - Drag and drop za menjanje start lista
        // - Ekipni poredak za Memorijal da se automatski izracuna.
        // - Ekipni poredak za Memorijal da se izracuna za sve ekipe, bez obzira na to da li ekipa ima gimnasticare
        //   u svim kategorijama.
        // - Stampanje gimnasticara iz Registra.

        // TODO: (iz beleski)
        // - Menjanje imena za gimnasticara ucesnika
        // - Prozor gde ce biti prikazani klubovi ucesnici
        // - Kada ima manje takmicara nego sto je u propozicijama dato kvalifikanata, svi treba da budu kvalifikovani
        //   (nema pravila o max. takmicara iz kluba).
        // - Kod rezultata ekipa, pored gimnasticara treba da stoji i kategorija (za slucaj kada se ekipa sastoji iz vise
        //   kategorija)
        // - Mogucnost promene kategorije istovremeno za vise gimnasticara iz registra.
        // - Sprave u karticama umesto u padajucem meniju.
        // - Za novo takmicenje uvesti datum od-do.
        // - Promeni futer u stampi da stampa datum od-do.
        // - Broj E ocena da bude poseban za svako takmicenje 1, 2, 3, 4.
        // - Kod izbora kako se racunaju kvalifikanti za finale preskoka, ne treba da postoji opcija "na osnovu 1. preskoka"
        //   zato sto se uvek racuna na osnovu oba preskoka (oba ili max)
        // - Kod racunanja ocene na osnovu oba preskoka treba da postoji izbor da li se ocena racuna na osnovu oba preskoka
        //   ili vece ocene iz oba preskoka.

        public static int VERZIJA_PROGRAMA = 5;

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

            // init() zovem u SingleInstanceApplication.OnCreateMainForm

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
                   
            //MainForm mainForm = new MainForm();
            //Application.Run(mainForm);
            SingleInstanceApplication.Application.Run(args);
        }

        public static void init()
        {
            Sesija.Instance.InitSession();

            // NOTE: Najpre inicijalizujem opcije jer se koriste u nekim od update metoda.

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

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                new VersionUpdater().update();
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }

            RazneProvere rp = new RazneProvere();
            // rp.proveriPrvaDvaKola();   // OK
            rp.proveriRezultateIOcene();
            rp.proveriTakmicenja234();
            // rp.dumpRezultati();
            // rp.takmicenjaBezOcene();
            // rp.proveriViseKola();  // Postoji nekoliko takmicenja gde ova provera ne prolazi, ali mislim da je bolje
            // nista me menjam. 

            // TODO4: Ako je Propozicije.PostojiTak4 false, mora i RezultatskoTakmicenje.ImaEkipnoTakmicenje da bude false.

            // TODO4: Proveri sva finala kupa, tako sto ces ponovo kreirati finale (
            // pozivom TakmicenjeService.kreirajNaOsnovuViseKola) i uporediti rezultate sa postojecim finalima (gde su
            // gimnasticari rucno dodavani). Razmisli da li treba menjati rezultate tamo gde se ne poklapaju (uradi to samo
            // u takmicenjima gde ne postoji odvojeno takmicenje 3.)


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

            // Kreiranje prazne baze
            //new SqlCeUtilities().CreateDatabase(@"..\..\clanovi_podaci2.sdf", "sdv");

            //new DatabaseUpdater().updateDatabase();
        }
    }
}