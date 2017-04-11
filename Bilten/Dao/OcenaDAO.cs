using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface OcenaDAO : GenericDAO<Ocena, int>
    {
        IList<Ocena> FindByGimnasticar(GimnasticarUcesnik gim, DeoTakmicenjaKod deoTakKod);
        IList<Ocena> FindByTakmicenje(int takmicenjeId);
        IList<Ocena> FindOceneByDeoTakmicenja(int takmicenjeId, DeoTakmicenjaKod deoTakKod);
        IList<Ocena> FindByKatSpravaDeoTak(TakmicarskaKategorija kategorija, Sprava sprava,
            DeoTakmicenjaKod deoTakKod);
        Ocena FindByIdFetch(int id);
        Ocena FindOcena(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod, Sprava sprava);
        bool existsOcene(int takmicenjeId);
        bool existsOcenaForGimnasticar(int gimnasticarId);
    }
}
