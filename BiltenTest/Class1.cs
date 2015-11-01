using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate.Cfg;
using Bilten.Domain;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using System.IO;
using System.Diagnostics;

namespace BiltenTest
{
    public class Account
    {
        private decimal balance;
        private decimal minimumBalance = 10m;

        public void Deposit(decimal amount)
        {
            balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            balance -= amount;
        }

        public void TransferFunds(Account destination, decimal amount)
        {
            if (balance - amount < minimumBalance)
                throw new InsufficientFundsException();

            destination.Deposit(amount);

            Withdraw(amount);
        }

        public decimal Balance
        {
            get { return balance; }
        }

        public decimal MinimumBalance
        {
            get { return minimumBalance; }
        }
    }

    public class InsufficientFundsException : ApplicationException
    {
    }

    [TestFixture]
    public class AccountTest
    {
        Account source;
        Account destination;

        [SetUp]
        public void Init()
        {
            source = new Account();
            source.Deposit(200m);

            destination = new Account();
            destination.Deposit(150m);
        }

        [Test]
        public void TransferFunds()
        {
            source.TransferFunds(destination, 100m);

            Assert.AreEqual(250m, destination.Balance);
            Assert.AreEqual(100m, source.Balance);
        }

        [Test]
        [ExpectedException(typeof(InsufficientFundsException))]
        public void TransferWithInsufficientFunds()
        {
            source.TransferFunds(destination, 300m);
        }

        [Test]
        [Ignore("Decide how to implement transaction management")]
        public void TransferWithInsufficientFundsAtomicity()
        {
            try
            {
                source.TransferFunds(destination, 300m);
            }
            catch (InsufficientFundsException)
            {
            }

            Assert.AreEqual(200m, source.Balance);
            Assert.AreEqual(150m, destination.Balance);
        }
    }


    public class InMemoryDatabaseTest : IDisposable
    {
        private static Configuration Configuration;
        private static ISessionFactory SessionFactory;
        protected ISession session;

        public InMemoryDatabaseTest(System.Reflection.Assembly assemblyContainingMapping)
        {
            if (Configuration == null)
            {
                Configuration = new Configuration()
                    .SetProperty(NHibernate.Cfg.Environment.ReleaseConnections, "on_close")
                    .SetProperty(NHibernate.Cfg.Environment.ConnectionProvider,
                        typeof(NHibernate.Connection.DriverConnectionProvider).AssemblyQualifiedName)
                    .SetProperty(NHibernate.Cfg.Environment.Dialect,
                        typeof(NHibernate.Dialect.MsSqlCeDialect).AssemblyQualifiedName)
                    .SetProperty(NHibernate.Cfg.Environment.ConnectionDriver,
                        typeof(NHibernate.Driver.SqlServerCeDriver).AssemblyQualifiedName)
                    .SetProperty(NHibernate.Cfg.Environment.ConnectionString, "Data Source=BiltenPodaci.sdf")
                    .AddAssembly(assemblyContainingMapping);

                SessionFactory = Configuration.BuildSessionFactory();
            }

            session = SessionFactory.OpenSession();

            new SchemaExport(Configuration).Execute(true, true, false, session.Connection, Console.Out);
        }

        public void Dispose()
        {
            session.Dispose();
        }
    }

    [TestFixture]
    public class GenerateSchema_Fixture
    {
        [Test]
        public void CanGenerateSchema()
        {
            var cfg = new Configuration();
            cfg.Configure();  // na osnovu hibernate.cfg.xml
            cfg.AddAssembly(typeof(Klub).Assembly);

            string databaseFile = "BiltenPodaciSchemaExport.sdf";
            SqlCeUtilities.CreateDatabase(databaseFile, "", false);

            // Ako treba (re)definisati neko svojstvo da bude razlicito od hibernate.cfg.xml
            string connectionString = String.Format("Data Source={0}", databaseFile);
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);

            new SchemaExport(cfg)
                 .SetOutputFile("bilten-schema-export.sql")
                 .Execute(false, true, false);
        }
    }

    [TestFixture]
    public class ProductRepository_Fixture
    {
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Export schema from BiltenPodaci.sdf using the ExportSqlCe.exe tool.
            string schemaFile = "BiltenPodaciScript.sql";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\..\Bilten\Libs\ExportSqlCe.exe";
            startInfo.Arguments =
                String.Format("\"Data Source=..\\..\\..\\Bilten\\bin\\Debug\\BiltenPodaci.sdf\" {0} schemaonly",
                schemaFile);
            var process = Process.Start(startInfo);
            process.WaitForExit();

            // Create database using the schema script, or (if database exists) delete data.
            string databaseFile = "BiltenPodaci.sdf";
            bool createdNew = SqlCeUtilities.CreateDatabase(databaseFile, "", false);
            if (createdNew)
                // Kreiraj semu
                SqlCeUtilities.ExecuteScript(databaseFile, "", Path.GetFullPath(schemaFile));
            else
                // Brisi podatke
                SqlCeUtilities.ExecuteScript(databaseFile, "", Path.GetFullPath(@"..\..\BiltenPodaciDelete.sql"));


            // Create configuration and build session factory.
            _configuration = new Configuration();
            _configuration.Configure();
            _configuration.AddAssembly(typeof(Klub).Assembly);
            _sessionFactory = _configuration.BuildSessionFactory();
        }

        // To keep our test methods side effect free we re-create our database schema before the execution of
        // each test method.
        [SetUp]
        public void SetupContext()
        {
            //new SchemaExport(_configuration).Execute(false, true, false);
        }

        //[ExpectedException(typeof(InsufficientFundsException))]
        [Test]
        public void CanAddNewTakmicenje()
        {
            /*var product = new Product { Name = "Apple", Category = "Fruits" };
            IProductRepository repository = new ProductRepository();
            repository.Add(product);*/
            Takmicenje takmicenje = new Takmicenje();
            takmicenje.Naziv = "Test takmicenje";
            takmicenje.Datum = DateTime.Parse("2014-12-20");
            takmicenje.Mesto = "Novi Sad";
            takmicenje.TipTakmicenja = TipTakmicenja.StandardnoTakmicenje;
            takmicenje.PrvoKolo = null;
            takmicenje.DrugoKolo = null;
            takmicenje.Gimnastika = Gimnastika.MSG;
        }
    }
}
