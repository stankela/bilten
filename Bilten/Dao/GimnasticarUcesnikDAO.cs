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
        IList<GimnasticarUcesnik> FindByKategorija(TakmicarskaKategorija kategorija);
        IList<GimnasticarUcesnik> FindGimnasticariUcesnici(string ime, string prezime, DrzavaUcesnik drzava,
            TakmicarskaKategorija kategorija, KlubUcesnik klub, int takmicenjeId);
        bool existsGimnasticarUcesnik(DrzavaUcesnik drzava);
        bool existsGimnasticarUcesnik(KlubUcesnik klub);
    }
}
