using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface TakmicarskaKategorijaDAO : GenericDAO<TakmicarskaKategorija, int>
    {
        IList<TakmicarskaKategorija> FindByTakmicenje(int takmicenjeId);
        bool existsKategorijaNaziv(string naziv, int takmicenjeId);
        int GetCountForTakmicenje(int takmicenjeId);
    }
}
