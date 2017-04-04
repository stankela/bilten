using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface RezultatskoTakmicenjeDescriptionDAO : GenericDAO<RezultatskoTakmicenjeDescription, int>
    {
        IList<RezultatskoTakmicenjeDescription> FindByTakmicenje(int takmicenjeId);
    }
}
