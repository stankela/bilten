using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface TakmicenjeDAO : GenericDAO<Takmicenje, int>
    {
        Takmicenje FindByIdFetch_Kat_Desc(int takmicenjeId);
        Takmicenje FindByIdFetch_Prop_Kat_Desc(int takmicenjeId);
        IList<Takmicenje> FindAll();
        IList<Takmicenje> FindByGimnastika(Gimnastika gim);
        Takmicenje FindByNazivGimnastikaDatum(string naziv, Gimnastika gim, DateTime datum);
        bool existsTakmicenje(string naziv, Gimnastika gim, DateTime datum);
    }
}
