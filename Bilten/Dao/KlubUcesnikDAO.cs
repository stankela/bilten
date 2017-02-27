using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface KlubUcesnikDAO : GenericDAO<KlubUcesnik, int>
    {
        KlubUcesnik FindKlubUcesnik(int takmicenjeId, string naziv);
        IList<KlubUcesnik> FindKluboviUcesnici(int takmicenjeId);
    }
}
