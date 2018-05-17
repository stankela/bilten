using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface JezikDAO : GenericDAO<Jezik, int>
    {
        IList<Jezik> FindAll();
    }
}
