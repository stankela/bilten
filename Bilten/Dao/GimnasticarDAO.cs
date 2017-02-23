using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface GimnasticarDAO : GenericDAO<Gimnasticar, int>
    {
        IList<Gimnasticar> FindGimnasticariByKlub(Klub klub);
        bool existsGimnasticar(Klub klub);
    }
}
