using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface SudijaDAO : GenericDAO<Sudija, int>
    {
        bool existsSudija(Drzava drzava);
        IList<Sudija> FindSudijeByDrzava(Drzava drzava);
        IList<Sudija> FindAll();
    }
}
