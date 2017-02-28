using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface TakmicenjeDAO : GenericDAO<Takmicenje, int>
    {
        Takmicenje FindByIdFetch_Kat_Desc(int takmicenjeId);
        bool existsTakmicenje(string naziv, Gimnastika gim, DateTime datum);
    }
}
