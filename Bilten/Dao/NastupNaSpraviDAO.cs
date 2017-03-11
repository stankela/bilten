using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface NastupNaSpraviDAO : GenericDAO<NastupNaSpravi, int>
    {
        bool existsNastupForGimnasticar(int gimnasticarId);
    }
}
