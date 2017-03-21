using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Data.SqlServerCe;
using System.Data;
using Bilten.Exceptions;
using Bilten.Dao;

namespace Bilten.Dao
{
    public class KonacanPlasmanDAO
    {
        private string findVisebojTak1SQL = @"
            select t.naziv, t.datum, t.mesto, d.naziv, tk.naziv, g.prezime, g.ime, g.srednje_ime,
            g.dan_rodj, g.mesec_rodj, g.god_rodj,
            r.rank, r.parter, r.konj, r.karike, r.preskok, r.razboj, r.vratilo, r.total
            from
            takmicenja t
            join rezultatsko_takmicenje_description d
            join takmicarske_kategorije tk
            join propozicije pr
            join rezultatsko_takmicenje rt
            join takmicenje1 t1
            join poredak_ukupno p
            join rezultati_ukupno r
            join gimnasticari_ucesnici g
            on r.gimnasticar_id = g.gimnasticar_id
            on p.poredak_id = r.poredak_id
            on t1.poredak_ukupno_id = p.poredak_id
            on rt.takmicenje1_id = t1.takmicenje1_id
            on rt.propozicije_id = pr.propozicije_id
            on rt.kategorija_id = tk.kategorija_id
            on rt.description_id = d.description_id
            on t.takmicenje_id = rt.takmicenje_id

            where g.prezime like @prezime
            and g.ime like @ime
            and pr.postoji_tak2 = 1
            and pr.odvojeno_tak2 = 0
            order by t.datum asc";

        private string findVisebojTak2SQL = @"
            select t.naziv, t.datum, t.mesto, d.naziv, tk.naziv, g.prezime, g.ime, g.srednje_ime,
            g.dan_rodj, g.mesec_rodj, g.god_rodj,
            r.rank, r.parter, r.konj, r.karike, r.preskok, r.razboj, r.vratilo, r.total
            from
            takmicenja t
            join rezultatsko_takmicenje_description d
            join takmicarske_kategorije tk
            join rezultatsko_takmicenje rt
            join takmicenje2 t2
            join poredak_ukupno p
            join rezultati_ukupno r
            join gimnasticari_ucesnici g
            on r.gimnasticar_id = g.gimnasticar_id
            on p.poredak_id = r.poredak_id
            on t2.poredak_id = p.poredak_id
            on rt.takmicenje2_id = t2.takmicenje2_id
            on rt.kategorija_id = tk.kategorija_id
            on rt.description_id = d.description_id
            on t.takmicenje_id = rt.takmicenje_id

            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findSpraveTak1SQL = @"
            select t.naziv, t.datum, t.mesto, d.naziv, tk.naziv, g.prezime, g.ime, g.srednje_ime,
            g.dan_rodj, g.mesec_rodj, g.god_rodj, p.sprava,
            r.rank, r.d, r.e, r.penalty, r.total
            from
            takmicenja t
            join rezultatsko_takmicenje_description d
            join takmicarske_kategorije tk
            join propozicije pr
            join rezultatsko_takmicenje rt
            join takmicenje1 t1
            join poredak_sprava p
            join rezultati_sprava r
            join gimnasticari_ucesnici g
            on r.gimnasticar_id = g.gimnasticar_id
            on p.poredak_id = r.poredak_id
            on t1.takmicenje1_id = p.takmicenje1_id
            on rt.takmicenje1_id = t1.takmicenje1_id
            on rt.propozicije_id = pr.propozicije_id
            on rt.kategorija_id = tk.kategorija_id
            on rt.description_id = d.description_id
            on t.takmicenje_id = rt.takmicenje_id

            where g.prezime like @prezime
            and g.ime like @ime
            and pr.postoji_tak3 = 1
            and pr.odvojeno_tak3 = 0
            order by t.datum asc";

        private string findPreskokTak1SQL = @"
            select t.naziv, t.datum, t.mesto, d.naziv, tk.naziv, g.prezime, g.ime, g.srednje_ime,
            g.dan_rodj, g.mesec_rodj, g.god_rodj, p.sprava,
            r.rank, r.d, r.e, r.penalty, r.total, r.rank2, r.d_2, r.e_2, r.penalty_2, r.total_2,
            r.total_obe_ocene
            from
            takmicenja t
            join rezultatsko_takmicenje_description d
            join takmicarske_kategorije tk
            join propozicije pr
            join rezultatsko_takmicenje rt
            join takmicenje1 t1
            join poredak_sprava p
            join rezultati_preskok r
            join gimnasticari_ucesnici g
            on r.gimnasticar_id = g.gimnasticar_id
            on p.poredak_id = r.poredak_id
            on t1.poredak_preskok_id = p.poredak_id
            on rt.takmicenje1_id = t1.takmicenje1_id
            on rt.propozicije_id = pr.propozicije_id
            on rt.kategorija_id = tk.kategorija_id
            on rt.description_id = d.description_id
            on t.takmicenje_id = rt.takmicenje_id

            where g.prezime like @prezime
            and g.ime like @ime
            and pr.postoji_tak3 = 1
            and pr.odvojeno_tak3 = 0
            order by t.datum asc";

        private string findSpraveTak3SQL = @"
            select t.naziv, t.datum, t.mesto, d.naziv, tk.naziv, g.prezime, g.ime, g.srednje_ime,
            g.dan_rodj, g.mesec_rodj, g.god_rodj, p.sprava,
            r.rank, r.d, r.e, r.penalty, r.total
            from
            takmicenja t
            join rezultatsko_takmicenje_description d
            join takmicarske_kategorije tk
            join rezultatsko_takmicenje rt
            join takmicenje3 t3
            join poredak_sprava p
            join rezultati_sprava r
            join gimnasticari_ucesnici g
            on r.gimnasticar_id = g.gimnasticar_id
            on p.poredak_id = r.poredak_id
            on t3.takmicenje3_id = p.takmicenje3_id
            on rt.takmicenje3_id = t3.takmicenje3_id
            on rt.kategorija_id = tk.kategorija_id
            on rt.description_id = d.description_id
            on t.takmicenje_id = rt.takmicenje_id

            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

        private string findPreskokTak3SQL = @"
            select t.naziv, t.datum, t.mesto, d.naziv, tk.naziv, g.prezime, g.ime, g.srednje_ime,
            g.dan_rodj, g.mesec_rodj, g.god_rodj, p.sprava,
            r.rank, r.d, r.e, r.penalty, r.total, r.rank2, r.d_2, r.e_2, r.penalty_2, r.total_2,
            r.total_obe_ocene
            from
            takmicenja t
            join rezultatsko_takmicenje_description d
            join takmicarske_kategorije tk
            join rezultatsko_takmicenje rt
            join takmicenje3 t3
            join poredak_sprava p
            join rezultati_preskok r
            join gimnasticari_ucesnici g
            on r.gimnasticar_id = g.gimnasticar_id
            on p.poredak_id = r.poredak_id
            on t3.poredak_preskok_id = p.poredak_id
            on rt.takmicenje3_id = t3.takmicenje3_id
            on rt.kategorija_id = tk.kategorija_id
            on rt.description_id = d.description_id
            on t.takmicenje_id = rt.takmicenje_id

            where g.prezime like @prezime
            and g.ime like @ime
            order by t.datum asc";

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
                KonacanPlasman kp = new KonacanPlasman();
                /*kp.RezultatskoTakmicenjeId = (int)rdr["rez_takmicenje_id"];
                kp.GimnasticarId = (int)rdr["gimnasticar_id"];
                kp.Sprava = (Sprava)(byte)rdr["sprava"];
                kp.KvalStatus = (KvalifikacioniStatus)(byte)rdr["kval_status"];*/
                result.Add(kp);
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
                KonacanPlasman kp = new KonacanPlasman();
                result.Add(kp);
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
                KonacanPlasman kp = new KonacanPlasman();
                result.Add(kp);
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
                result.Add(kp);
            }

            rdr.Close();
            return result;
        }
    }
}