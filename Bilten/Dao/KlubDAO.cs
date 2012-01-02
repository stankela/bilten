using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    /// <summary>
    /// Business DAO operations related to the <see cref="Domain.Klub"/> entity.
    /// </summary>
    public interface KlubDAO : GenericDAO<Klub, int>
    {
        IList<Klub> FindKlubsByNaziv(string naziv);
        IList<Klub> FindKlubsByKod(string kod);
    }
}
