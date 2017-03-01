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
        RezultatskoTakmicenje FindByKatDesc(TakmicarskaKategorija kat, RezultatskoTakmicenjeDescription desc);
    }
}
