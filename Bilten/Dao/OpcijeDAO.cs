using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface OpcijeDAO : GenericDAO<Opcije, int>
    {
        Opcije FindOpcije();
    }
}