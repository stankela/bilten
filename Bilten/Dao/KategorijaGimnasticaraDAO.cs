using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface KategorijaGimnasticaraDAO : GenericDAO<KategorijaGimnasticara, int>
    {
        IList<KategorijaGimnasticara> FindAll();
        IList<KategorijaGimnasticara> FindByGimnastika(Gimnastika gimnastika);
        bool existsKategorijaGimnasticara(string naziv, Gimnastika gimnastika);
    }
}
