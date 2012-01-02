using System;
using System.Configuration;
using System.Reflection;

namespace Bilten.Data
{
	/// <summary>
	/// Summary description for DataAccessProviderFactory.
	/// </summary>
	public class DataAccessProviderFactory : IDataProviderFactory
	{
		private static Assembly activeProvider = null;
		private static IDataProviderFactory activeDataProviderFactory = null;

		static DataAccessProviderFactory()
		{
			string providerName = ConfigurationManager.AppSettings["DataProvider"];
            string providerFactoryName = ConfigurationManager.AppSettings["DataProviderFactory"];
			//activeProvider = Assembly.Load(providerName);
            activeProvider = Assembly.GetCallingAssembly();
            activeDataProviderFactory = (IDataProviderFactory) activeProvider.CreateInstance(providerFactoryName);
		}

        public IDataContext GetDataContext()
        {
            return activeDataProviderFactory.GetDataContext();
        }

    }
}
