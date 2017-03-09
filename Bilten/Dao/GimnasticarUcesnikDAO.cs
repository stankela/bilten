﻿using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface GimnasticarUcesnikDAO : GenericDAO<GimnasticarUcesnik, int>
    {
        IList<GimnasticarUcesnik> FindByTakmicenje(int takmicenjeId);
        IList<GimnasticarUcesnik> FindByTakmicenjeFetch_Kat_Klub_Drzava(int takmicenjeId);
        IList<GimnasticarUcesnik> FindByTakmicenjeKat(int takmicenjeId, TakmicarskaKategorija kategorija);
        IList<GimnasticarUcesnik> FindByTakmicenjeKatFetch_Klub_Drzava(Takmicenje tak, string kategorija);
        IList<GimnasticarUcesnik> FindGimnasticariUcesnici(string ime,
            string prezime, Nullable<int> godRodj, DrzavaUcesnik drzava, TakmicarskaKategorija kategorija,
            KlubUcesnik klub, int takmicenjeId);
        GimnasticarUcesnik FindByTakmicenjeTakBroj(Takmicenje takmicenje, int takBroj);
        bool existsGimnasticarTakBroj(int takBroj, Takmicenje takmicenje);
    }
}
