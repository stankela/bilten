using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Bilten.Domain;

namespace Bilten
{
    class PersistentConfigurationBuilder
    {
        public Configuration GetConfiguration()
        {
            //log.Debug("Building the Configuration");

            string fileName = "NHibernateConfig";
            string path = @"..\..\..\Bilten.Domain\NHibernate\Mappings";
            IList<string> dependencies = 
                getMappingFiles(path);

            Configuration configuration;
            if (IsNewConfigurationRequired(fileName, dependencies))
            {
                //log.Debug("Configuration is either old or some of the dependencies have changed");
                configuration = createConfiguration();
                SaveConfiguration(fileName, configuration);
            }
            else
            {
                configuration = LoadConfiguration(fileName);
            }
            return configuration;
        }

        private IList<string> getMappingFiles(string dirPath)
        {
            IList<string> result = new List<string>();
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            if (dirInfo.Exists)
            {
                FileInfo[] files = dirInfo.GetFiles("*hbm.xml");
                foreach (FileInfo fi in files)
                {
                    result.Add(fi.FullName);
                }
            }
            return result;
        }

        private bool IsNewConfigurationRequired(string fileName, IList<string> dependencies)
        {
            if (!File.Exists(fileName))
                return true;
            FileInfo fi = new FileInfo(fileName);
            DateTime lastModified = fi.LastWriteTime;
            for (int i = 0; i < dependencies.Count; i++)
            {
                FileInfo dependency = new FileInfo(dependencies[i]);
                DateTime dependencyLastModified = dependency.LastWriteTime;
                if (dependencyLastModified > lastModified)
                    return true;
            }
            return false;
        }

        private Configuration createConfiguration()
        {
            Configuration cfg = new Configuration();
            cfg.Configure();
            cfg.AddAssembly(typeof(Klub).Assembly);
            return cfg;
        }

        private Configuration LoadConfiguration(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (Configuration)formatter.Deserialize(fileStream);
            }
        }

        private void SaveConfiguration(string fileName, Configuration configuration)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(
                fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fileStream, configuration);
            }
        }
    }
}
