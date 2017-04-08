using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.Dao
{
    public interface RezultatskoTakmicenjeDAO : GenericDAO<RezultatskoTakmicenje, int>
    {
        RezultatskoTakmicenje loadRezTakmicenje(int rezTakmicenjeId);
        IList<RezultatskoTakmicenje> FindRezTakmicenjaForGimnasticar(GimnasticarUcesnik g);
        IList<RezultatskoTakmicenje> FindByTakmicenje(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_Ekipe(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_Gimnasticari(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakUkupno(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakUkupno_KlubDrzava(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_KatDesc_Tak1_PoredakUkupno_KlubDrzava(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_KatDesc_Tak1_PoredakUkupnoZbirViseKola_KlubDrzava(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakEkipnoZbirViseKola(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakSprava(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_PoredakEkipno(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak1_Gimnasticari_PoredakEkipno(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak3_Poredak(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeKatFetch_Tak3_Poredak(int takmicenjeId, TakmicarskaKategorija kat);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetch_Tak4_Gimnasticari_Poredak(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTakmicenje2(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTakmicenje3(int takmicenjeId);
        IList<RezultatskoTakmicenje> FindByTakmicenjeFetchTak_2_3_4(int takmicenjeId);
        RezultatskoTakmicenje FindByIdFetch_Tak1_Gimnasticari_PoredakEkipno(int id);
        RezultatskoTakmicenje FindByIdFetchTakmicenje3(int rezTakmicenjeId);
        RezultatskoTakmicenje FindByKatDesc(TakmicarskaKategorija kat, RezultatskoTakmicenjeDescription desc);
        IList<RezultatskoTakmicenje> FindByKategorija(TakmicarskaKategorija kat);
        IList<RezultatskoTakmicenje> FindEkipnaTakmicenja(int takmicenjeId);
    }
}
