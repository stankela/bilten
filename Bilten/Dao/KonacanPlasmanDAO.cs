﻿using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Data.SqlServerCe;
using System.Data;
using Bilten.Exceptions;
using Bilten.Dao;

namespace Bilten.Dao
{
    public class KonacanPlasman
    {
        public int RezultatskoTakmicenjeId { get; set; }
        public string NazivTakmicenja { get; set; }
        public string MestoTakmicenja { get; set; }
        public DateTime DatumTakmicenja { get; set; }
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
        private string findVisebojTak1SQL = @"
            select rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank,
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
            select rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
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

        private string findSpraveTak1SQL = @"
            select rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, p.sprava, r.rank,
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
            select rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank, r.rank2,
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
            select rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
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
            select rt.rez_takmicenje_id, d.naziv as naziv_tak, t.datum, t.mesto, tk.naziv as naziv_kat,
            g.prezime, g.ime, g.srednje_ime, g.dan_rodj, g.mesec_rodj, g.god_rodj, r.rank, r.rank2
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

        // TODO4: Uradi i ekipni plasman.

        private void loadCommonData(KonacanPlasman kp, SqlCeDataReader rdr)
        {
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
                    throw new Exception("Greska u programu.");
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

        private void loadPreskok(KonacanPlasman kp, SqlCeDataReader rdr)
        {
            Nullable<short> rank2 = Convert.IsDBNull(rdr["rank2"]) ? null : (Nullable<short>)rdr["rank2"];
            if (rank2 != null)
                kp.Preskok = rank2;
            else
                kp.Preskok = Convert.IsDBNull(rdr["rank"]) ? null : (Nullable<short>)rdr["rank"];
        }

        // can throw InfrastructureException
        public List<KonacanPlasman> findVisebojTak1(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findVisebojTak1SQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = Database.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                if ((bool)rdr["postoji_tak2"] && !(bool)rdr["odvojeno_tak2"])
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
            SqlCeDataReader rdr = Database.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

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

        public List<KonacanPlasman> findSpraveTak1(string ime, string prezime)
        {
            SqlCeCommand cmd = new SqlCeCommand(findSpraveTak1SQL);
            cmd.Parameters.Add("@ime", SqlDbType.NVarChar).Value = ime;
            cmd.Parameters.Add("@prezime", SqlDbType.NVarChar).Value = prezime;
            SqlCeDataReader rdr = Database.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                if ((bool)rdr["postoji_tak3"] && !(bool)rdr["odvojeno_tak3"])
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
            SqlCeDataReader rdr = Database.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                if ((bool)rdr["postoji_tak3"] && !(bool)rdr["odvojeno_tak3"])
                {
                    KonacanPlasman kp = new KonacanPlasman();
                    loadCommonData(kp, rdr);
                    loadPreskok(kp, rdr);
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
            SqlCeDataReader rdr = Database.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

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
            SqlCeDataReader rdr = Database.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

            List<KonacanPlasman> result = new List<KonacanPlasman>();
            while (rdr.Read())
            {
                KonacanPlasman kp = new KonacanPlasman();
                loadCommonData(kp, rdr);
                loadPreskok(kp, rdr);
                result.Add(kp);
            }

            rdr.Close();
            return result;
        }
    }
}