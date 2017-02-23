using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface GimnasticarDAO : GenericDAO<Gimnasticar, int>
    {
        IList<Gimnasticar> FindGimnasticariByKlub(Klub klub);
        IList<Gimnasticar> FindGimnasticariByDrzava(Drzava drzava);
        IList<Gimnasticar> FindGimnasticariByKategorija(KategorijaGimnasticara kategorija);
        bool existsGimnasticar(Klub klub);
        bool existsGimnasticar(Drzava drzava);
        bool existsGimnasticar(KategorijaGimnasticara kategorija);
    }
}
