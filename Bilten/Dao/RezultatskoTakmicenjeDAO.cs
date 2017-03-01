using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface RezultatskoTakmicenjeDAO : GenericDAO<RezultatskoTakmicenje, int>
    {
        RezultatskoTakmicenje loadRezTakmicenje(int rezTakmicenjeId);
        IList<RezultatskoTakmicenje> FindRezTakmicenjaForGimnasticar(GimnasticarUcesnik g);
        IList<RezultatskoTakmicenje> FindByTakmicenje(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTakmicenje2(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTakmicenje3(int takmicenjeId);
        RezultatskoTakmicenje FindByIdFetchTakmicenje3(int rezTakmicenjeId);
        RezultatskoTakmicenje FindByKatDesc(TakmicarskaKategorija kat, RezultatskoTakmicenjeDescription desc);
    }
}
