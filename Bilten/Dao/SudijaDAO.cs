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
        IList<Sudija> FindSudije(string ime, string prezime, Pol? pol, Drzava drzava, Klub klub);
        bool existsSudija(Drzava drzava);
        bool existsSudija(string ime, string prezime);
    }
}
