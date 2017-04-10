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

        IList<Takmicenje> FindAllOdvojenoFinale();
        IList<Takmicenje> FindViseKola();

        // Ovaj metod mi je trebao u MilanoInitalizer, inace nema neku upotrebnu vrednost.
        Takmicenje FindByMestoGimnastika(string mesto, Gimnastika gim);

        bool existsTakmicenje(string naziv, Gimnastika gim, DateTime datum);
    }
}
