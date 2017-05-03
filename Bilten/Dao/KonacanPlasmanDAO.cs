using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Data.SqlServerCe;
using System.Data;

namespace Bilten.Dao
{
    public class KonacanPlasman
    {
        public int TakmicenjeId { get; set; }
        public int RezultatskoTakmicenjeId { get; set; }
        public string NazivTakmicenja { get; set; }
        public string MestoTakmicenja { get; set; }
        public DateTime DatumTakmicenja { get; set; }
        public TipTakmicenja TipTakmicenja { get; set; }
        public string NazivKategorije { get; set; }

        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string SrednjeIme { get; set; }
        public Nullable<byte> DanRodj { get; set; }
        public Nullable<byte> MesecRodj { get; set; }
        public Nullable<short> GodRodj { get; set; }

        public Nullable<short> Viseboj { get; set; }
        public Nullable<short> Parter { get; set; }
        public Nullable<short> Konj { get; set; }
        public Nullable<short> Karike { get; set; }
        public Nullable<short> Preskok { get; set; }
        public Nullable<short> Razboj { get; set; }
        public Nullable<short> Vratilo { get; set; }
        public Nullable<short> Greda { get; set; }
        public Nullable<short> DvovisinskiRazboj { get; set; }
        public Nullable<short> Ekipno { get; set; }

        public Datum DatumRodjenja
        {
            get
            {
                if (DanRodj == null && MesecRodj == null && GodRodj == null)
                    return null;
                if (DanRodj == null && MesecRodj == null && GodRodj != null)
                    return new Datum(GodRodj.Value);
                if (DanRodj != null && MesecRodj != null && GodRodj != null)
                    return new Datum(new DateTime(GodRodj.Value, MesecRodj.Value, DanRodj.Value));
                throw new Exception("Nelegalan datum.");
            }
        }
    }
    
    public class KonacanPlasmanDAO
    {
        public string ConnectionString;
        
        private string findVisebojTak1SQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            t.tip_takmicenja, g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank,
            pr.postoji_tak2, pr.odvojeno_tak2
            from gimnasticari_ucesnici g
            join rezultati_ukupno r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_ukupno p
                on p.poredak_id = r.poredak_id
            join takmicenje1 t1
                on t1.poredak_ukupno_id = p.poredak_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje1_id = t1.takmicenje1_id
            join propozicije pr
                on rt.propozicije_id = pr.propozicije_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d
                on rt.description_id = d.description_id
            join takmicenja t
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findVisebojTak2SQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank
            from gimnasticari_ucesnici g
            join rezultati_ukupno r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_ukupno p
                on p.poredak_id = r.poredak_id
            join takmicenje2 t2
                on t2.poredak_id = p.poredak_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje2_id = t2.takmicenje2_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d
                on rt.description_id = d.description_id
            join takmicenja t
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findVisebojFinaleKupaSQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank,
            pr.postoji_tak2, pr.odvojeno_tak2
            from gimnasticari_ucesnici g
            join rezultati_ukupno_finale_kupa r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_ukupno_finale_kupa p
                on p.poredak_id = r.poredak_id
            join takmicenje1 t1
                on t1.poredak_ukupno_finale_kupa_id = p.poredak_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje1_id = t1.takmicenje1_id
            join propozicije pr
                on rt.propozicije_id = pr.propozicije_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d
                on rt.description_id = d.description_id
            join takmicenja t
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findVisebojZbirViseKolaSQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank
            from gimnasticari_ucesnici g
            join rezultati_ukupno_zbir_vise_kola r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_ukupno_zbir_vise_kola p
                on p.poredak_id = r.poredak_id
            join takmicenje1 t1
                on t1.poredak_ukupno_zbir_vise_kola_id = p.poredak_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje1_id = t1.takmicenje1_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d
                on rt.description_id = d.description_id
            join takmicenja t
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findSpraveTak1SQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            t.tip_takmicenja, g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, p.sprava, r.rank,
            pr.postoji_tak3, pr.odvojeno_tak3
            from gimnasticari_ucesnici g
            join rezultati_sprava r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_sprava p
                on p.poredak_id = r.poredak_id
            join takmicenje1 t1
                on t1.takmicenje1_id = p.takmicenje1_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje1_id = t1.takmicenje1_id
            join propozicije pr
                on rt.propozicije_id = pr.propozicije_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d            
                on rt.description_id = d.description_id
            join takmicenja t        
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findPreskokTak1SQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            t.tip_takmicenja, g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank,
            pr.postoji_tak3, pr.odvojeno_tak3
            from gimnasticari_ucesnici g
            join rezultati_preskok r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_sprava p
                on p.poredak_id = r.poredak_id
            join takmicenje1 t1
                on t1.poredak_preskok_id = p.poredak_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje1_id = t1.takmicenje1_id
            join propozicije pr
                on rt.propozicije_id = pr.propozicije_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d
                on rt.description_id = d.description_id
            join takmicenja t            
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findSpraveTak3SQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, p.sprava, r.rank
            from gimnasticari_ucesnici g
            join rezultati_sprava r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_sprava p
                on p.poredak_id = r.poredak_id
            join takmicenje3 t3
                on t3.takmicenje3_id = p.takmicenje3_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje3_id = t3.takmicenje3_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d
                on rt.description_id = d.description_id
            join takmicenja t
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findPreskokTak3SQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank
            from gimnasticari_ucesnici g
            join rezultati_preskok r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_sprava p
                on p.poredak_id = r.poredak_id
            join takmicenje3 t3
                on t3.poredak_preskok_id = p.poredak_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje3_id = t3.takmicenje3_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d            
                on rt.description_id = d.description_id
            join takmicenja t
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findSpraveFinaleKupaSQL = @"
            select t.takmicenje_id, rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            t.tip_takmicenja, g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, p.sprava, r.rank,
            pr.postoji_tak3, pr.odvojeno_tak3
            from gimnasticari_ucesnici g
            join rezultati_sprava_finale_kupa r
                on r.gimnasticar_id = g.gimnasticar_id
            join poredak_sprava_finale_kupa p
                on p.poredak_id = r.poredak_id
            join takmicenje1 t1
                on t1.takmicenje1_id = p.takmicenje1_id
            join rezultatsko_takmicenje rt
                on rt.takmicenje1_id = t1.takmicenje1_id
            join propozicije pr
                on rt.propozicije_id = pr.propozicije_id
            join takmicarske_kategorije tk
                on rt.kategorija_id = tk.kategorija_id
            join rezultatsko_takmicenje_description d            
                on rt.description_id = d.description_id
            join takmicenja t        
                on t.takmicenje_id = rt.takmicenje_id
            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        // TODO4: Uradi i ekipni plasman.

        private void loadCommonData(KonacanPlasman kp, SqlCeDataReader rdr)
        {
            kp.TakmicenjeId = (int)rdr["takmicenje_id"];
            kp.RezultatskoTakmicenjeId = (int)rdr["rez_takmicenje_id"];
            kp.NazivTakmicenja = (string)rdr["naziv_tak"];
            kp.MestoTakmicenja = (string)rdr["mesto"];
            kp.DatumTakmicenja = (DateTime)rdr["datum"];
            kp.NazivKategorije = (string)rdr["naziv_kat"];

            kp.Ime = (string)rdr["ime"];
            kp.Prezime = (string)rdr["prezime"];
            kp.SrednjeIme = Convert.IsDBNull(rdr["srednje_ime"]) ? null : (string)rdr["srednje_ime"];
            kp.DanRodj = Convert.IsDBNull(rdr["dan_rodj"]) ? null : (Nullable<byte>)rdr["dan_rodj"];
            kp.MesecRodj = Convert.IsDBNull(rdr["mesec_rodj"]) ? null : (Nullable<byte>)rdr["mesec_rodj"];
            kp.GodRodj = Convert.IsDBNull(rdr["god_rodj"]) ? null : (Nullable<short>)rdr["god_rodj"];
        }

        private void loadSprava(KonacanPlasman kp, SqlCeDataReader rdr)
        {
            Sprava sprava = (Sprava)(byte)rdr["sprava"];
            Nullable<short> rank = Convert.IsDBNull(rdr["rank"]) ? null : (Nullable<short>)rdr["rank"];
            switch (sprava)
            {
                case Sprava.Parter:
                    kp.Parter = rank;
                    break;
                case Sprava.Konj:
                    kp.Konj = rank;
                    break;
                case Sprava.Karike:
                    kp.Karike = rank;
                    break;
                case Sprava.Preskok:
                    kp.Preskok = rank;
                    break;
                case Sprava.Razboj:
                    kp.Razboj = rank;
                    break;
                case Sprava.Vratilo:
                    kp.Vratilo = rank;
                    break;
                case Sprava.DvovisinskiRazboj:
                    kp.DvovisinskiRazboj = rank;
                    break;
                case Sprava.Greda:
                    kp.Greda = rank;
                    break;
            }
        }

        // can throw InfrastructureException
        public List<KonacanPlasman> findVisebojTak1(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findVisebojTak1SQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                TipTakmicenja tipTakmicenja = (TipTakmicenja)rdr["tip_takmicenja"];
                bool postojiTak2 = (bool)rdr["postoji_tak2"];
                bool odvojenoTak2 = (bool)rdr["odvojeno_tak2"];
                if (tipTakmicenja == TipTakmicenja.StandardnoTakmicenje && (!postojiTak2 || !odvojenoTak2)
                    || tipTakmicenja == TipTakmicenja.FinaleKupa && odvojenoTak2)
                {
                    KonacanPlasman kp = new KonacanPlasman();
                    loadCommonData(kp, rdr);
                    kp.Viseboj = Convert.IsDBNull(rdr["rank"]) ? null : (Nullable<short>)rdr["rank"];
                    result.Add(kp);
                }
            }

            rdr.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader
            return result;
        }

        public List<KonacanPlasman> findVisebojTak2(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findVisebojTak2SQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                KonacanPlasman kp = new KonacanPlasman();
                loadCommonData(kp, rdr);
                kp.Viseboj = Convert.IsDBNull(rdr["rank"]) ? null : (Nullable<short>)rdr["rank"];
                result.Add(kp);
            }

            rdr.Close();
            return result;
        }

        public List<KonacanPlasman> findVisebojFinaleKupa(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findVisebojFinaleKupaSQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                bool postojiTak2 = (bool)rdr["postoji_tak2"];
                bool odvojenoTak2 = (bool)rdr["odvojeno_tak2"];
                // TODO4: Proveri da li se kod finala kupa PostojiTak2 i PostojiTak3 ponasaju kao kod obicnog takmicenja,
                // tj. da li se poretci kreiraju i kada se stavi false. Ako da, izmeni ovu if naredbu (i kod spava isto).
                if (!postojiTak2 || !odvojenoTak2)
                {
                    KonacanPlasman kp = new KonacanPlasman();
                    loadCommonData(kp, rdr);
                    kp.Viseboj = Convert.IsDBNull(rdr["rank"]) ? null : (Nullable<short>)rdr["rank"];
                    result.Add(kp);
                }
            }

            rdr.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader
            return result;
        }

        public List<KonacanPlasman> findVisebojZbirViseKola(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findVisebojZbirViseKolaSQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                KonacanPlasman kp = new KonacanPlasman();
                loadCommonData(kp, rdr);
                kp.Viseboj = Convert.IsDBNull(rdr["rank"]) ? null : (Nullable<short>)rdr["rank"];
                result.Add(kp);
            }

            rdr.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader
            return result;
        }

        public List<KonacanPlasman> findSpraveTak1(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findSpraveTak1SQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                // NOTE: Cak i ako ne postoji takmicenje 3, poredak se ipak racuna i rezultate je moguce pregledati
                // u prozoru RezultatiSpraveForm. Tako da u ovom slucaju treba prikazati i te plasmane. Isto vazi i za
                // viseboj.
                TipTakmicenja tipTakmicenja = (TipTakmicenja)rdr["tip_takmicenja"];
                bool postojiTak3 = (bool)rdr["postoji_tak3"];
                bool odvojenoTak3 = (bool)rdr["odvojeno_tak3"];
                if (tipTakmicenja == TipTakmicenja.StandardnoTakmicenje && (!postojiTak3 || !odvojenoTak3)
                    || tipTakmicenja == TipTakmicenja.FinaleKupa && odvojenoTak3)
                {
                    KonacanPlasman kp = new KonacanPlasman();
                    loadCommonData(kp, rdr);
                    loadSprava(kp, rdr);
                    result.Add(kp);
                }
            }

            rdr.Close();
            return result;
        }

        public List<KonacanPlasman> findPreskokTak1(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findPreskokTak1SQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                TipTakmicenja tipTakmicenja = (TipTakmicenja)rdr["tip_takmicenja"];
                bool postojiTak3 = (bool)rdr["postoji_tak3"];
                bool odvojenoTak3 = (bool)rdr["odvojeno_tak3"];
                if (tipTakmicenja == TipTakmicenja.StandardnoTakmicenje && (!postojiTak3 || !odvojenoTak3)
                    || tipTakmicenja == TipTakmicenja.FinaleKupa && odvojenoTak3)
                {
                    KonacanPlasman kp = new KonacanPlasman();
                    loadCommonData(kp, rdr);
                    kp.Preskok = Convert.IsDBNull(rdr["rank"]) ? null : (Nullable<short>)rdr["rank"];
                    result.Add(kp);
                }
            }

            rdr.Close();
            return result;
        }

        public List<KonacanPlasman> findSpraveTak3(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findSpraveTak3SQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                KonacanPlasman kp = new KonacanPlasman();
                loadCommonData(kp, rdr);
                loadSprava(kp, rdr);
                result.Add(kp);
            }

            rdr.Close();
            return result;
        }

        public List<KonacanPlasman> findPreskokTak3(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findPreskokTak3SQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                KonacanPlasman kp = new KonacanPlasman();
                loadCommonData(kp, rdr);
                kp.Preskok = Convert.IsDBNull(rdr["rank"]) ? null : (Nullable<short>)rdr["rank"];
                result.Add(kp);
            }

            rdr.Close();
            return result;
        }

        public List<KonacanPlasman> findSpraveFinaleKupa(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findSpraveFinaleKupaSQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                bool postojiTak3 = (bool)rdr["postoji_tak3"];
                bool odvojenoTak3 = (bool)rdr["odvojeno_tak3"];
                if (!postojiTak3 || !odvojenoTak3)
                {
                    KonacanPlasman kp = new KonacanPlasman();
                    loadCommonData(kp, rdr);
                    loadSprava(kp, rdr);
                    result.Add(kp);
                }
            }

            rdr.Close();
            return result;
        }
    }
}