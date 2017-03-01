using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface OcenaDAO : GenericDAO<Ocena, int>
    {
        bool existsOcene(int takmicenjeId);
    }
}
