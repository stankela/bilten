using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface Takmicenje2DAO : GenericDAO<Takmicenje2, int>
    {
        bool isGimnasticarUcesnik(int gimnasticarId);
    }
}
