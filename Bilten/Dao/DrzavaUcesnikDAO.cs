using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface DrzavaUcesnikDAO : GenericDAO<DrzavaUcesnik, int>
    {
        DrzavaUcesnik FindDrzavaUcesnik(int takmicenjeId, string naziv);
        IList<DrzavaUcesnik> FindDrzaveUcesnici(int takmicenjeId);
    }
}
