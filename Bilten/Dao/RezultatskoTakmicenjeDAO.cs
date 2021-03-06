﻿using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface RezultatskoTakmicenjeDAO : GenericDAO<RezultatskoTakmicenje, int>
    {
        RezultatskoTakmicenje FindByIdFetch_Ekipe(int rezTakmicenjeId);
        IList<RezultatskoTakmicenje> FindByGimnasticar(GimnasticarUcesnik g);
        IList<RezultatskoTakmicenje> FindByUcesnikTak3(GimnasticarUcesnik g);
        RezultatskoTakmicenje FindByEkipa(Ekipa e);
        IList<RezultatskoTakmicenje> FindByTakmicenje(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_Gimnasticari(int takmicenjeId);
        RezultatskoTakmicenje FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(int takmicenjeId, string nazivKategorije,
            byte redBrojDesc);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakUkupno_Gimnasticari(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_KatDesc_Tak1_PoredakUkupnoFinaleKupa_KlubDrzava(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_KatDesc_Tak1_PoredakUkupnoZbirViseKola_KlubDrzava(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakEkipnoFinaleKupa(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakEkipnoZbirViseKola(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakSprava(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakSpravaFinaleKupa(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_Gimnasticari_PoredakEkipno(int takmicenjeId);
        RezultatskoTakmicenje FindByTakmicenjeFetch_Tak1_PoredakEkipno_Ekipe(int takmicenjeId, string nazivKategorije,
            byte redBrojDesc);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak3_Poredak(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak4_Gimnasticari_Poredak(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTakmicenje2(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTakmicenje3(int takmicenjeId);
        RezultatskoTakmicenje FindByIdFetch_Tak1_Gimnasticari_PoredakEkipno(int id);
        RezultatskoTakmicenje FindByIdFetchTakmicenje3(int rezTakmicenjeId);
        IList<RezultatskoTakmicenje> FindByKategorija(TakmicarskaKategorija kat);
        IList<RezultatskoTakmicenje> FindByDescription(RezultatskoTakmicenjeDescription desc);
        RezultatskoTakmicenje FindByKatDesc(TakmicarskaKategorija kat, RezultatskoTakmicenjeDescription desc);
        IList<RezultatskoTakmicenje> FindEkipnaTakmicenja(int takmicenjeId);
        int FindMaxRedBroj(int takmicenjeId);
        IList<Pair<RezultatskoTakmicenje, RezultatUkupno>> FindRezultatiUkupnoForGimnasticar(int takmicenjeId,
            int gimnasticarId);
    }
}
