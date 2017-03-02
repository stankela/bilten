using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface SudijaUcesnikDAO : GenericDAO<SudijaUcesnik, int>
    {
        IList<SudijaUcesnik> FindByTakmicenje(int takmicenjeId);
        IList<SudijaUcesnik> FindByTakmicenjeFetchKlubDrzava(int takmicenjeId);
        bool existsSudijaUcesnik(Sudija s, Takmicenje takmicenje);
    }
}
