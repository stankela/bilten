using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface MestoDAO : GenericDAO<Mesto, int>
    {
        IList<Mesto> FindAll(bool sorted = false);
    }
}
