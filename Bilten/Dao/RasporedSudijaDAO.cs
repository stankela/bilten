using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface RasporedSudijaDAO : GenericDAO<RasporedSudija, int>
    {
        IList<RasporedSudija> FindByTakmicenje(int takmicenjeId);
        RasporedSudija FindByIdFetch(int id);
    }
}