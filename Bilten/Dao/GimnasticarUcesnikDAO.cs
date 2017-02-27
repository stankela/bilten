using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface GimnasticarUcesnikDAO : GenericDAO<GimnasticarUcesnik, int>
    {
        bool existsGimnasticarTakBroj(int takBroj, Takmicenje takmicenje);
    }
}
