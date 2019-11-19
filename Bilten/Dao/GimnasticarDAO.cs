using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface GimnasticarDAO : GenericDAO<Gimnasticar, int>
    {
        IList<Gimnasticar> FindAll();
        IList<Gimnasticar> FindByGimnastika(Gimnastika gim);
        IList<Gimnasticar> FindGimnasticariByKlub(Klub klub);
        IList<Gimnasticar> FindGimnasticariByDrzava(Drzava drzava);
        IList<Gimnasticar> FindGimnasticariByKategorija(KategorijaGimnasticara kategorija);
        IList<Gimnasticar> FindGimnasticariByRegBroj(string regBroj);
        IList<Gimnasticar> FindGimnasticari(string ime, string prezime,
            Nullable<int> godRodj, Nullable<Gimnastika> gimnastika, Drzava drzava,
            KategorijaGimnasticara kategorija, Klub klub);
        Gimnasticar FindGimnasticar(string ime, string prezime, Nullable<int> danRodj, Nullable<int> mesRodj,
            Nullable<int> godRodj, Nullable<Gimnastika> gimnastika);
        bool existsGimnasticar(Klub klub);
        bool existsGimnasticar(Drzava drzava);
        bool existsGimnasticar(KategorijaGimnasticara kategorija);
        bool existsGimnasticarImePrezimeSrednjeImeDatumRodjenja(string ime, string prezime, string srednjeIme,
            Datum datumRodj);
        bool existsGimnasticarRegBroj(string regBroj);
    }
}
