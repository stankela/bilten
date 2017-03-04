using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface OcenaDAO : GenericDAO<Ocena, int>
    {
        IList<Ocena> FindOceneForGimnasticar(GimnasticarUcesnik gim, DeoTakmicenjaKod deoTakKod);
        IList<Ocena> FindByTakmicenje(int takmicenjeId);
        IList<Ocena> FindOceneByDeoTakmicenja(int takmicenjeId, DeoTakmicenjaKod deoTakKod);
        bool existsOcene(int takmicenjeId);
    }
}
