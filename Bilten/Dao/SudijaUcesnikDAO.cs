using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface SudijaUcesnikDAO : GenericDAO<SudijaUcesnik, int>
    {
        IList<SudijaUcesnik> FindForTakmicenje(int takmicenjeId);
        bool existsSudijaUcesnik(Sudija s, Takmicenje takmicenje);
    }
}
