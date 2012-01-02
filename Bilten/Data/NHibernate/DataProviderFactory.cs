using Bilten.Data;

namespace Bilten.Data.NHibernate
{
    public class DataProviderFactory : IDataProviderFactory
	{
        public IDataContext GetDataContext()
        {
            return new NHibernateDataContext();
        }
    }
}
