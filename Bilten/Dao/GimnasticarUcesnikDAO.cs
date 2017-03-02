using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface GimnasticarUcesnikDAO : GenericDAO<GimnasticarUcesnik, int>
    {
        IList<GimnasticarUcesnik> FindByTakmicenje(int takmicenjeId);
        bool existsGimnasticarTakBroj(int takBroj, Takmicenje takmicenje);
    }
}
