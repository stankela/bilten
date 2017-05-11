using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface PropozicijeDAO : GenericDAO<Propozicije, int>
    {
        IList<Propozicije> FindAll();
    }
}