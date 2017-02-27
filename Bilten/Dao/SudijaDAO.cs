using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface SudijaDAO : GenericDAO<Sudija, int>
    {
        IList<Sudija> FindSudijeByDrzava(Drzava drzava);
        IList<Sudija> FindAll();
        bool existsSudija(Drzava drzava);
        bool existsSudija(string ime, string prezime);
    }
}
