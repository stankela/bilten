namespace Bilten.Dao
{
    public abstract class DAOFactory
    {
        public abstract KlubDAO GetKlubDAO();
        public abstract GimnasticarDAO GetGimnasticarDAO();
        public abstract DrzavaDAO GetDrzavaDAO();
        public abstract SudijaDAO GetSudijaDAO();
        public abstract KategorijaGimnasticaraDAO GetKategorijaGimnasticaraDAO();
        public abstract MestoDAO GetMestoDAO();
        public abstract TakmicenjeDAO GetTakmicenjeDAO();
        public abstract SudijaUcesnikDAO GetSudijaUcesnikDAO();
        public abstract DrzavaUcesnikDAO GetDrzavaUcesnikDAO();
        public abstract KlubUcesnikDAO GetKlubUcesnikDAO();
        public abstract RezultatskoTakmicenjeDAO GetRezultatskoTakmicenjeDAO();
        public abstract EkipaDAO GetEkipaDAO();
        public abstract Takmicenje1DAO GetTakmicenje1DAO();
        public abstract Takmicenje2DAO GetTakmicenje2DAO();
        public abstract Takmicenje3DAO GetTakmicenje3DAO();
        public abstract Takmicenje4DAO GetTakmicenje4DAO();
        public abstract GimnasticarUcesnikDAO GetGimnasticarUcesnikDAO();
        public abstract OcenaDAO GetOcenaDAO();
        public abstract DrugaOcenaDAO GetDrugaOcenaDAO();
        public abstract UcesnikTakmicenja2DAO GetUcesnikTakmicenja2DAO();
        public abstract UcesnikTakmicenja3DAO GetUcesnikTakmicenja3DAO();
        public abstract RezultatskoTakmicenjeDescriptionDAO GetRezultatskoTakmicenjeDescriptionDAO();
        public abstract TakmicarskaKategorijaDAO GetTakmicarskaKategorijaDAO();
        public abstract OpcijeDAO GetOpcijeDAO();
        public abstract RasporedNastupaDAO GetRasporedNastupaDAO();
        public abstract RasporedSudijaDAO GetRasporedSudijaDAO();
        public abstract PropozicijeDAO GetPropozicijeDAO();
        public abstract SudijskiOdborNaSpraviDAO GetSudijskiOdborNaSpraviDAO();
        public abstract StartListaNaSpraviDAO GetStartListaNaSpraviDAO();
        public abstract PoredakEkipnoDAO GetPoredakEkipnoDAO();
        public abstract PoredakSpravaDAO GetPoredakSpravaDAO();
        public abstract PoredakPreskokDAO GetPoredakPreskokDAO();
        public abstract PoredakUkupnoDAO GetPoredakUkupnoDAO();
        public abstract PoredakUkupnoFinaleKupaDAO GetPoredakUkupnoFinaleKupaDAO();
        public abstract PoredakSpravaFinaleKupaDAO GetPoredakSpravaFinaleKupaDAO();
        public abstract PoredakPreskokFinaleKupaDAO GetPoredakPreskokFinaleKupaDAO();
        public abstract PoredakUkupnoZbirViseKolaDAO GetPoredakUkupnoZbirViseKolaDAO();
        public abstract PoredakEkipnoFinaleKupaDAO GetPoredakEkipnoFinaleKupaDAO();
        public abstract PoredakEkipnoZbirViseKolaDAO GetPoredakEkipnoZbirViseKolaDAO();
        public abstract JezikDAO GetJezikDAO();
    }
}