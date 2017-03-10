using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface RasporedNastupaDAO : GenericDAO<RasporedNastupa, int>
    {
        IList<RasporedNastupa> FindByTakmicenje(int takmicenjeId);
        RasporedNastupa FindByIdFetch(int rasporedId);
        IList<RasporedNastupa> FindByTakmicenjeDeoTak(int takmicenjeId, DeoTakmicenjaKod deoTak);
    }
}