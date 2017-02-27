using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface EkipaDAO : GenericDAO<Ekipa, int>
    {
        Ekipa FindEkipaById(int id);
        bool existsEkipaNaziv(int rezTakmicenjeId, string naziv);
        bool existsEkipaKod(int rezTakmicenjeId, string kod);
    }
}
