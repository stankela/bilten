﻿using Bilten;
using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.UI;
using Bilten.Util;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

public class VersionUpdater
{
    public static bool hasUpdates()
    {
        int verzijaBaze = SqlCeUtilities.getDatabaseVersionNumber();
        return Program.VERZIJA_PROGRAMA > verzijaBaze;
    }

    public void update()
    {
        int verzijaBaze = SqlCeUtilities.getDatabaseVersionNumber();
        if (verzijaBaze == Program.VERZIJA_PROGRAMA)
            return;

        if (verzijaBaze < 5)
            throw new Exception("Bazu podataka je nemoguce konvertovati da radi sa trenutnom verzijom programa.");
        if (verzijaBaze > Program.VERZIJA_PROGRAMA)
            throw new Exception("Greska u programu. Verzija baze je veca od verzije programa.");

        int staraVerzijaBaze = verzijaBaze;
        bool converted = false;

        // Ovo sam koristio kada sam apdejtovao stare biltene (pre 2015).
        if (verzijaBaze == 0)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version1.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(1);
            verzijaBaze = 1;
            converted = true;
        }

        if (verzijaBaze == 1 && Program.VERZIJA_PROGRAMA > 1)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version2.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(2);
            verzijaBaze = 2;
            converted = true;
        }

        if (verzijaBaze == 2 && Program.VERZIJA_PROGRAMA > 2)
        {
            // Prazan apdejt (tj. prebacen je u apdejt za sledecu verziju)
            SqlCeUtilities.updateDatabaseVersionNumber(3);
            verzijaBaze = 3;
            converted = true;
        }

        if (verzijaBaze == 3 && Program.VERZIJA_PROGRAMA > 3)
        {
            // TODO: Ove naredbe bi trebalo izvrsavati u okviru jedne transakcije. Isto i za ostale verzije.

            SqlCeUtilities.dropReferentialConstraint("ekipe", "klubovi_ucesnici");
            SqlCeUtilities.dropReferentialConstraint("ekipe", "drzave_ucesnici");
            SqlCeUtilities.dropReferentialConstraint("gimnasticari_ucesnici", "takmicenja");

            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version4.txt", true);

            updateLastModified();
            updateVersion3();
            updateVersion7();
            updateVersion13();

            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version4b.txt", true);

            SqlCeUtilities.updateDatabaseVersionNumber(4);
            verzijaBaze = 4;
            converted = true;
        }

        if (verzijaBaze == 4 && Program.VERZIJA_PROGRAMA > 4)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version5.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(5);
            addTakmicenjeToRasporedNastupa();
            addTakmicenjeToRasporedSudija();
            updateRegistarskiBrojToString();
            updateNacinRacunanjaOceneFinaleKupa();
            updateNacinRacunanjaPreskoka();
            updateTakmicenja234();
            updateTakmicenje_200();
            verzijaBaze = 5;
            converted = true;
        }

        if (verzijaBaze == 5 && Program.VERZIJA_PROGRAMA > 5)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version6.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(6);
            verzijaBaze = 6;
            converted = true;
        }

        if (verzijaBaze == 6 && Program.VERZIJA_PROGRAMA > 6)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version7.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(7);
            verzijaBaze = 7;
            converted = true;
        }

        if (verzijaBaze == 7 && Program.VERZIJA_PROGRAMA > 7)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version8.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(8);
            verzijaBaze = 8;
            converted = true;
        }

        if (verzijaBaze == 8 && Program.VERZIJA_PROGRAMA > 8)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version9.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(9);
            verzijaBaze = 9;
            converted = true;
        }

        if (verzijaBaze == 9 && Program.VERZIJA_PROGRAMA > 9)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version10.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(10);
            verzijaBaze = 10;
            converted = true;
        }

        if (verzijaBaze == 10 && Program.VERZIJA_PROGRAMA > 10)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version11.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(11);
            verzijaBaze = 11;
            converted = true;
        }

        if (verzijaBaze == 11 && Program.VERZIJA_PROGRAMA > 11)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version12.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(12);
            verzijaBaze = 12;
            converted = true;
        }

        if (verzijaBaze == 12 && Program.VERZIJA_PROGRAMA > 12)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version13.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(13);
            verzijaBaze = 13;
            converted = true;
        }

        if (verzijaBaze == 13 && Program.VERZIJA_PROGRAMA > 13)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version14.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(14);
            verzijaBaze = 14;
            converted = true;
        }

        if (verzijaBaze == 14 && Program.VERZIJA_PROGRAMA > 14)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version15.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(15);
            verzijaBaze = 15;
            converted = true;
        }

        if (verzijaBaze == 15 && Program.VERZIJA_PROGRAMA > 15)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version16.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(16);
            verzijaBaze = 16;
            converted = true;
        }

        if (verzijaBaze == 16 && Program.VERZIJA_PROGRAMA > 16)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version17.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(17);
            verzijaBaze = 17;
            converted = true;
        }

        if (verzijaBaze == 17 && Program.VERZIJA_PROGRAMA > 17)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version18.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(18);
            verzijaBaze = 18;
            converted = true;
        }

        if (verzijaBaze == 18 && Program.VERZIJA_PROGRAMA > 18)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version19.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(19);
            verzijaBaze = 19;
            converted = true;
        }

        if (verzijaBaze == 19 && Program.VERZIJA_PROGRAMA > 19)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version20.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(20);
            verzijaBaze = 20;
            converted = true;
        }

        if (verzijaBaze == 20 && Program.VERZIJA_PROGRAMA > 20)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version21.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(21);
            verzijaBaze = 21;
            converted = true;
        }

        if (verzijaBaze == 21 && Program.VERZIJA_PROGRAMA > 21)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version22.txt", true);
            SqlCeUtilities.updateDatabaseVersionNumber(22);
            verzijaBaze = 22;
            converted = true;
        }

        if (verzijaBaze == 22 && Program.VERZIJA_PROGRAMA > 22)
        {
            SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                "Bilten.Update.DatabaseUpdate_version23.txt", true);
            brisiRedundantneSpraveFinaleKupa();
            SqlCeUtilities.updateDatabaseVersionNumber(23);
            verzijaBaze = 23;
            converted = true;
        }

        // TODO5: Neka sledeci update bude da se otvaraju redom sva finala kupa. To ce ponovo kreirati sve rezultate,
        // i popuniti rezultate preskok.

        if (converted)
        {
            string msg = String.Format("Baza podataka je konvertovana iz verzije {0} u verziju {1}.", staraVerzijaBaze,
                verzijaBaze);
            MessageBox.Show(msg, "Bilten");

            if (File.Exists("NHibernateConfig"))
                File.Delete("NHibernateConfig");
        }
    }

    class RezultatSpravaFinaleKupaDTO
    {
 	    public int rezultat_id;
	    public short red_broj;
	    public Nullable<short> rank;
	    public byte kval_status;
	    public Nullable<float> d_prvo_kolo;
        public Nullable<float> e_prvo_kolo;
        public Nullable<float> bonus_prvo_kolo;
        public Nullable<float> pen_prvo_kolo;
        public Nullable<float> total_prvo_kolo;
        public Nullable<float> d_drugo_kolo;
        public Nullable<float> e_drugo_kolo;
        public Nullable<float> bonus_drugo_kolo;
        public Nullable<float> pen_drugo_kolo;
        public Nullable<float> total_drugo_kolo;
        public Nullable<float> total;
	    public int gimnasticar_id;
        public int poredak_id;
    };

    Nullable<float> getNullableFloat(object o)
    {
        if (o.GetType().Equals(typeof(DBNull)))
            return null;
        else
            return (float)o;
    }

    Nullable<short> getNullableShort(object o)
    {
        if (o.GetType().Equals(typeof(DBNull)))
            return null;
        else
            return (short)o;
    }

    object getParamValue(Nullable<float> o)
    {
        if (o.HasValue)
            return o.Value;
        else
            return DBNull.Value;
    }

    object getParamValue(Nullable<short> o)
    {
        if (o.HasValue)
            return o.Value;
        else
            return DBNull.Value;
    }

    public void brisiRedundantneSpraveFinaleKupa()
    {
        // 1. Pronadji sve poretke sprave finale kupa za svako takmicenje1

        string sql = @"
select t.takmicenje1_id, p.poredak_id
FROM poredak_sprava_finale_kupa p JOIN takmicenje1 t
ON t.takmicenje1_id = p.takmicenje1_id
WHERE p.sprava = @sprava;";

        Dictionary<int, List<int>> result1 = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> result2 = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> result3 = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> result4 = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> result5 = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> result6 = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> result7 = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> result8 = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> result = null;
        List<int> duplicates = new List<int>();

        string ConnectionString = ConfigurationParameters.ConnectionString;
        // can throw InfrastructureException
        for (int sprava = 1; sprava <= 8; ++sprava)
        {
            if (sprava == 1) result = result1;
            else if (sprava == 2) result = result2;
            else if (sprava == 3) result = result3;
            else if (sprava == 4) result = result4;
            else if (sprava == 5) result = result5;
            else if (sprava == 6) result = result6;
            else if (sprava == 7) result = result7;
            else if (sprava == 8) result = result8;

            SqlCeCommand cmd = new SqlCeCommand(sql);
            cmd.Parameters.Add("@sprava", SqlDbType.TinyInt).Value = sprava;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

            while (rdr.Read())
            {
                int val1 = (int)rdr[0];
                int val2 = (int)rdr[1];
                if (result.ContainsKey(val1))
                {
                    duplicates.Add(val1);
                    duplicates.Add(val2);
                }
                else
                {
                    result.Add(val1, new List<int>());
                }
                result[val1].Add(val2);
            }
            rdr.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader
        }

        // 2. Izbrisi sve poretke osim onog sa najvisim id, koji je poslednji kreiran i sadrzi tacan poredak
        
        SqlCeConnection conn = new SqlCeConnection(ConfigurationParameters.ConnectionString);
        string errMsg = "Neuspesna promena baze.";
        SqlCeTransaction tr = null;

        try
        {
            conn.Open();
            tr = conn.BeginTransaction();

            for (int sprava = 1; sprava <= 8; ++sprava)
            {
                if (sprava == 1) result = result1;
                else if (sprava == 2) result = result2;
                else if (sprava == 3) result = result3;
                else if (sprava == 4) result = result4;
                else if (sprava == 5) result = result5;
                else if (sprava == 6) result = result6;
                else if (sprava == 7) result = result7;
                else if (sprava == 8) result = result8;

                foreach (int takmicenje1 in result.Keys)
                {
                    List<int> poretci = result[takmicenje1];
                    if (poretci.Count == 1)
                    {
                        if (sprava == 4)
                        {
                            updatePoredakPreskokFinaleKupaZaTakmicenje1(takmicenje1, poretci[0], conn, tr);
                        }
                    }
                    else
                    {
                        int maxPoredak = -1;
                        foreach (int i in poretci)
                        {
                            if (i > maxPoredak)
                                maxPoredak = i;
                        }
                        if (maxPoredak == -1)
                            throw new Exception("Greska u programu, finale kupa nema nijedan poredak po spravama");
                        foreach (int i in poretci)
                        {
                            if (i != maxPoredak)
                                deletePoredakSpravaFinaleKupa(i, conn, tr);
                        }
                        if (sprava == 4)
                        {
                            updatePoredakPreskokFinaleKupaZaTakmicenje1(takmicenje1, maxPoredak, conn, tr);
                        }
                    }
                }
            }
            tr.Commit();
        }
        catch (SqlCeException e)
        {
            // in Open()
            if (tr != null)
                tr.Rollback(); // TODO: this can throw Exception and InvalidOperationException
            throw new InfrastructureException(errMsg, e);
        }
        catch (InvalidOperationException e)
        {
            // in ExecuteNonQuery(), ExecureScalar()
            if (tr != null)
                tr.Rollback();
            throw new InfrastructureException(errMsg, e);
        }
        // za svaki slucaj
        catch (Exception)
        {
            if (tr != null)
                tr.Rollback();
            throw;
        }
        finally
        {
            conn.Close();
        }

        // 3. Pronadji sve rezultate preskoka za finale kupa

        string sql2 = @"
select r.*
FROM rezultati_sprava_finale_kupa r JOIN poredak_sprava_finale_kupa p
ON r.poredak_id = p.poredak_id
WHERE p.sprava = 4;";

        // can throw InfrastructureException
        SqlCeCommand cmd2 = new SqlCeCommand(sql2);
        SqlCeDataReader rdr2 = SqlCeUtilities.executeReader(cmd2, Strings.DATABASE_ACCESS_ERROR_MSG, ConnectionString);

        List<RezultatSpravaFinaleKupaDTO> rezultatiSpravaFinaleKupa = new List<RezultatSpravaFinaleKupaDTO>();
        while (rdr2.Read())
        {
            RezultatSpravaFinaleKupaDTO rez = new RezultatSpravaFinaleKupaDTO();
            rez.rezultat_id = (int)rdr2["rezultat_id"];
            rez.red_broj = (short)rdr2["red_broj"];
            rez.rank = getNullableShort(rdr2["rank"]);
            rez.kval_status = (byte)rdr2["kval_status"];

            rez.d_prvo_kolo = getNullableFloat(rdr2["d_prvo_kolo"]);
            rez.e_prvo_kolo = getNullableFloat(rdr2["e_prvo_kolo"]);
            rez.bonus_prvo_kolo = getNullableFloat(rdr2["bonus_prvo_kolo"]);
            rez.pen_prvo_kolo = getNullableFloat(rdr2["pen_prvo_kolo"]);
            rez.total_prvo_kolo = getNullableFloat(rdr2["total_prvo_kolo"]);
            rez.d_drugo_kolo = getNullableFloat(rdr2["d_drugo_kolo"]);
            rez.e_drugo_kolo = getNullableFloat(rdr2["e_drugo_kolo"]);
            rez.bonus_drugo_kolo = getNullableFloat(rdr2["bonus_drugo_kolo"]);
            rez.pen_drugo_kolo = getNullableFloat(rdr2["pen_drugo_kolo"]);
            rez.total_drugo_kolo = getNullableFloat(rdr2["total_drugo_kolo"]);
            rez.total = getNullableFloat(rdr2["total"]);

            rez.gimnasticar_id = (int)rdr2["gimnasticar_id"];
            rez.poredak_id = (int)rdr2["poredak_id"];

            rezultatiSpravaFinaleKupa.Add(rez);
        }
        rdr2.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader

        conn = new SqlCeConnection(ConfigurationParameters.ConnectionString);
        tr = null;

        try
        {
            conn.Open();
            tr = conn.BeginTransaction();

            // 4. Prebaci sve rezultate sprave u novu tabelu za preskok

            string sql3 = @"
INSERT INTO rezultati_preskok_finale_kupa (red_broj, rank, kval_status, d_prvo_kolo, e_prvo_kolo, bonus_prvo_kolo,
pen_prvo_kolo, total_prvo_kolo, d_drugo_kolo, e_drugo_kolo, bonus_drugo_kolo, pen_drugo_kolo, total_drugo_kolo, total,
gimnasticar_id, poredak_id)
VALUES (@red_broj, @rank, @kval_status, @d_prvo_kolo, @e_prvo_kolo, @bonus_prvo_kolo, @pen_prvo_kolo, @total_prvo_kolo,
@d_drugo_kolo, @e_drugo_kolo, @bonus_drugo_kolo, @pen_drugo_kolo, @total_drugo_kolo, @total, @gimnasticar_id, @poredak_id)";

            foreach (RezultatSpravaFinaleKupaDTO rez in rezultatiSpravaFinaleKupa)
            {
                SqlCeCommand cmd3 = new SqlCeCommand(sql3);
                cmd3.Connection = conn;
                cmd3.Transaction = tr;
                cmd3.Parameters.Add("@red_broj", SqlDbType.SmallInt).Value = rez.red_broj;
                cmd3.Parameters.Add("@rank", SqlDbType.SmallInt).Value = getParamValue(rez.rank);
                cmd3.Parameters.Add("@kval_status", SqlDbType.TinyInt).Value = rez.kval_status;
                cmd3.Parameters.Add("@d_prvo_kolo", SqlDbType.Float).Value = getParamValue(rez.d_prvo_kolo);
                cmd3.Parameters.Add("@e_prvo_kolo", SqlDbType.Float).Value = getParamValue(rez.e_prvo_kolo);
                cmd3.Parameters.Add("@bonus_prvo_kolo", SqlDbType.Float).Value = getParamValue(rez.bonus_prvo_kolo);
                cmd3.Parameters.Add("@pen_prvo_kolo", SqlDbType.Float).Value = getParamValue(rez.pen_prvo_kolo);
                cmd3.Parameters.Add("@total_prvo_kolo", SqlDbType.Float).Value = getParamValue(rez.total_prvo_kolo);
                cmd3.Parameters.Add("@d_drugo_kolo", SqlDbType.Float).Value = getParamValue(rez.d_drugo_kolo);
                cmd3.Parameters.Add("@e_drugo_kolo", SqlDbType.Float).Value = getParamValue(rez.e_drugo_kolo);
                cmd3.Parameters.Add("@bonus_drugo_kolo", SqlDbType.Float).Value = getParamValue(rez.bonus_drugo_kolo);
                cmd3.Parameters.Add("@pen_drugo_kolo", SqlDbType.Float).Value = getParamValue(rez.pen_drugo_kolo);
                cmd3.Parameters.Add("@total_drugo_kolo", SqlDbType.Float).Value = getParamValue(rez.total_drugo_kolo);
                cmd3.Parameters.Add("@total", SqlDbType.Float).Value = getParamValue(rez.total);
                cmd3.Parameters.Add("@gimnasticar_id", SqlDbType.Int).Value = rez.gimnasticar_id;
                cmd3.Parameters.Add("@poredak_id", SqlDbType.Int).Value = rez.poredak_id;
                cmd3.ExecuteNonQuery();
            }

            // 5. Izbrisi sve rezultate sprave finale kupa za preskok iz stare tabele

            string sql4 = @"
DELETE FROM rezultati_sprava_finale_kupa
WHERE rezultat_id = @rezultat_id;";

            foreach (RezultatSpravaFinaleKupaDTO rez in rezultatiSpravaFinaleKupa)
            {
                SqlCeCommand cmd4 = new SqlCeCommand(sql4);
                cmd4.Connection = conn;
                cmd4.Transaction = tr;
                cmd4.Parameters.Add("@rezultat_id", SqlDbType.Int).Value = rez.rezultat_id;
                cmd4.ExecuteNonQuery();
            }

            // NOTE: 6. Postavljam LastModified na danasnji datum, da bih izbegao azuriranje poretka kada se otvori finale
            // kupa. Time izbegavam potencijalnu gresku ako prethodna kola ne sadrze sve ocene (npr ako je takmicenje
            // uvezeno sa drugog laptopa koji jedini ima sve ocene za prethodna kola). To istovremeno znaci da rezultati za
            // preskok nece biti azurirani sa kompletnim prvim i drugim kolom, tj sadrzavace samo total prvog i drugog kola
            // i konacni total za oba kola.

            DateTime date = DateTime.Now;
            string datum = date.Year + "-" + date.Month + "-" + date.Day;
            string sql5 =
"UPDATE takmicenja SET last_modified = '" + datum + "' " + 
"WHERE tip_takmicenja = 1;";

            SqlCeCommand cmd5 = new SqlCeCommand(sql5);
            cmd5.Connection = conn;
            cmd5.Transaction = tr;
            cmd5.ExecuteNonQuery();

            tr.Commit();
        }
        catch (SqlCeException e)
        {
            // in Open()
            if (tr != null)
                tr.Rollback(); // TODO: this can throw Exception and InvalidOperationException
            throw new InfrastructureException(errMsg, e);
        }
        catch (InvalidOperationException e)
        {
            // in ExecuteNonQuery(), ExecureScalar()
            if (tr != null)
                tr.Rollback();
            throw new InfrastructureException(errMsg, e);
        }
        // za svaki slucaj
        catch (Exception)
        {
            if (tr != null)
                tr.Rollback();
            throw;
        }
        finally
        {
            conn.Close();
        }

        // Ispravi reference na prethodna kola
        AdminForm.ispraviFinalaKupa();
        AdminForm.ispraviZbirViseKola();

        // TODO5: Kod izvozenja finala kupa i zbira prethodnih kola, izvezi i sva kola. Kod uvozenja finala kupa i zbira
        // prethodnih kola, prvo proveri da li su reference na prethodna kola ispravne, i ako nisu ispravi ih kao u metodima
        // AdminForm.ispraviFinalaKupa() i AdminForm.ispraviZbirViseKola(). Zatim proveri da li su ocene u izvezenim
        // prethodnim kolima identicne kao ucene u prethodnim kolima na laptopu gde se uvozi takmicenje. Ako nisu, treba
        // prikazati poruku koje se ocene razlikuju, i verovatno ponuditi izbor koje prethodno kolo da se koristi - da li
        // ono izvezeno, ili ono na laptopu gde se uvozi takmicenje.
    }

    private void deletePoredakSpravaFinaleKupa(int poredak_id, SqlCeConnection conn, SqlCeTransaction tr)
    {
        string sql1 = @"
DELETE FROM rezultati_sprava_finale_kupa
WHERE poredak_id = @poredak;";

        SqlCeCommand cmd1 = new SqlCeCommand(sql1);
        cmd1.Connection = conn;
        cmd1.Transaction = tr;
        cmd1.Parameters.Add("@poredak", SqlDbType.Int).Value = poredak_id;
        cmd1.ExecuteNonQuery();

        string sql2 = @"
DELETE FROM poredak_sprava_finale_kupa
WHERE poredak_id = @poredak;";

        SqlCeCommand cmd2 = new SqlCeCommand(sql2);
        cmd2.Connection = conn;
        cmd2.Transaction = tr;
        cmd2.Parameters.Add("@poredak", SqlDbType.Int).Value = poredak_id;
        cmd2.ExecuteNonQuery();
    }

    private void updatePoredakPreskokFinaleKupaZaTakmicenje1(int takmicenje1_id, int poredak_id, SqlCeConnection conn,
        SqlCeTransaction tr)
    {
        string sql1 = @"
UPDATE takmicenje1 SET poredak_preskok_finale_kupa_id = @poredak
WHERE takmicenje1_id = @takmicenje1;";

        SqlCeCommand cmd1 = new SqlCeCommand(sql1);
        cmd1.Connection = conn;
        cmd1.Transaction = tr;
        cmd1.Parameters.Add("@poredak", SqlDbType.Int).Value = poredak_id;
        cmd1.Parameters.Add("@takmicenje1", SqlDbType.Int).Value = takmicenje1_id;
        cmd1.ExecuteNonQuery();

        string sql2 = @"
UPDATE poredak_sprava_finale_kupa SET takmicenje1_id = null
WHERE poredak_id = @poredak;";

        SqlCeCommand cmd2 = new SqlCeCommand(sql2);
        cmd2.Connection = conn;
        cmd2.Transaction = tr;
        cmd2.Parameters.Add("@poredak", SqlDbType.Int).Value = poredak_id;
        cmd2.ExecuteNonQuery();
    }

    public void updateVersion3()
    {
        IList<int> takmicenjaId = getTakmicenjaId();
        for (int i = 0; i < takmicenjaId.Count; ++i)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    
                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    Takmicenje2DAO tak2DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO();
                    Takmicenje3DAO tak3DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO();

                    Takmicenje t = takmicenjeDAO.FindById(takmicenjaId[i]);
                    if (!t.ZavrsenoTak1)
                        continue;
                    IList<RezultatskoTakmicenje> rezTakmicenja = rezTakDAO.FindByTakmicenje(t.Id);
                    foreach (RezultatskoTakmicenje tak in rezTakmicenja)
                    {
                        NHibernateUtil.Initialize(tak.Takmicenje1.PoredakUkupno.Rezultati);
                    }
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        if (rt.Propozicije.PostojiTak2 && !rt.Propozicije.OdvojenoTak2)
                        {
                            rt.Takmicenje2.clearUcesnici();
                            rt.Takmicenje2.Poredak.Rezultati.Clear();
                            tak2DAO.Update(rt.Takmicenje2);
                        }
                        if (rt.Propozicije.PostojiTak3 && !rt.Propozicije.OdvojenoTak3)
                        {
                            rt.Takmicenje3.clearUcesnici();
                            foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                            {
                                p.Rezultati.Clear();
                            }
                            rt.Takmicenje3.PoredakPreskok.Rezultati.Clear();
                            tak3DAO.Update(rt.Takmicenje3);
                        }
                    }
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }
    }
    
    public void updateVersion7()
    {
        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);

                GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                GimnasticarUcesnik g = gimUcesnikDAO.FindById(14422);
                OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                IList<Ocena> ocene = ocenaDAO.FindByGimnasticar(g, DeoTakmicenjaKod.Takmicenje1);
                if (ocene.Count > 0)
                {
                    ocenaDAO.Delete(ocene[0]);
                    gimUcesnikDAO.Delete(g);
                    session.Transaction.Commit();
                }
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }
    }

    /*class RezultatSpravaFinaleKupaUpdate
    {
        public int GimnasticarId;
        public Sprava Sprava;
        public KvalifikacioniStatus KvalStatus;
    }*/

    public void updateVersion13()
    {
        // NOTE: updatePreskok vise nije moguce zvati zato sto se oslanjalo na RedBroj2 i Rank2 koji su izbrisani.
        //updatePreskok();
        //updatePoredakViseKola();
        updateZavrsenoTak1();
        updateKvalifikanti();
    }

    /*private void updatePoredakViseKola()
    {
        SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM rezultati_sprava_finale_kupa_update");
        SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DATABASE_ACCESS_ERROR_MSG);

        IDictionary<int, List<RezultatSpravaFinaleKupaUpdate>> rezultatiUpdate
            = new Dictionary<int, List<RezultatSpravaFinaleKupaUpdate>>();
        while (rdr.Read())
        {
            RezultatSpravaFinaleKupaUpdate rezultat = new RezultatSpravaFinaleKupaUpdate();
            rezultat.GimnasticarId = (int)rdr["gimnasticar_id"];
            rezultat.Sprava = (Sprava)(byte)rdr["sprava"];
            rezultat.KvalStatus = (KvalifikacioniStatus)(byte)rdr["kval_status"];
            
            int rezultatskoTakmicenjeId = (int)rdr["rez_takmicenje_id"];
            if (rezultatiUpdate.ContainsKey(rezultatskoTakmicenjeId))
                rezultatiUpdate[rezultatskoTakmicenjeId].Add(rezultat);
            else
            {
                List<RezultatSpravaFinaleKupaUpdate> rezultati = new List<RezultatSpravaFinaleKupaUpdate>();
                rezultati.Add(rezultat);
                rezultatiUpdate.Add(rezultatskoTakmicenjeId, rezultati);
            }
        }
        rdr.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader
               
        IList<int> takmicenjaId = getTakmicenjaId();
        for (int i = 0; i < takmicenjaId.Count; ++i)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();

                    Takmicenje takmicenje = takmicenjeDAO.FindById(takmicenjaId[i]);
                    if (!takmicenje.FinaleKupa && !takmicenje.ZbirViseKola)
                        continue;

                    List<Takmicenje> prethodnaKola = new List<Takmicenje>();
                    prethodnaKola.Add(takmicenje.PrvoKolo);
                    prethodnaKola.Add(takmicenje.DrugoKolo);
                    if (takmicenje.TreceKolo != null)
                        prethodnaKola.Add(takmicenje.TreceKolo);
                    if (takmicenje.CetvrtoKolo != null)
                        prethodnaKola.Add(takmicenje.CetvrtoKolo);

                    RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    IList<RezultatskoTakmicenje> svaRezTakmicenja = rezultatskoTakmicenjeDAO.FindByTakmicenje(takmicenje.Id);

                    List<IList<RezultatskoTakmicenje>> rezTakmicenjaPrethodnaKola = new List<IList<RezultatskoTakmicenje>>();
                    foreach (Takmicenje prethKolo in prethodnaKola)
                    {
                        rezTakmicenjaPrethodnaKola.Add(
                            rezultatskoTakmicenjeDAO.FindByTakmicenje(prethKolo.Id));
                    }

                    IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
                    IList<RezultatskoTakmicenje> preskociRezTakmicenja = new List<RezultatskoTakmicenje>();
                    foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                    {
                        if (rt.TakmicenjeDescription.RedBroj != 0)
                            continue;

                        // preskoci rez. takmicenja gde ne postoji poklapanje u svim prethodnim kolima
                        bool preskoci = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], 0, rt.Kategorija) == null
                            || Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], 0, rt.Kategorija) == null
                            || (takmicenje.TreceKolo != null && Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[2], 0, rt.Kategorija) == null)
                            || (takmicenje.CetvrtoKolo != null && Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[3], 0, rt.Kategorija) == null);

                        if (!preskoci || takmicenje.Id == 226)
                            rezTakmicenja.Add(rt);
                        else
                            preskociRezTakmicenja.Add(rt);
                    }

                    ISet<string> rezTakmicenjaSet = new HashSet<string>();

                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        RezultatskoTakmicenje rezTak1 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], 0, rt.Kategorija);
                        RezultatskoTakmicenje rezTak2;
                        if (rt.Id != 654)
                            rezTak2 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], 0, rt.Kategorija);
                        else
                        {
                            rezTak2 = new RezultatskoTakmicenje();
                            rezTak2.Propozicije = new Propozicije();
                            rezTak2.Propozicije.Tak3PreskokNaOsnovuObaPreskoka = false;
                            rezTak2.Takmicenje1 = new Takmicenje1(takmicenje.Gimnastika);
                        }

                        RezultatskoTakmicenje rezTak3 = null;
                        RezultatskoTakmicenje rezTak4 = null;
                        if (takmicenje.TreceKolo != null)
                            rezTak3 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[2], 0, rt.Kategorija);
                        if (takmicenje.CetvrtoKolo != null)
                            rezTak4 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[3], 0, rt.Kategorija);

                        if (takmicenje.FinaleKupa)
                        {
                            if (rt.Takmicenje1.PoredakUkupnoFinaleKupa == null)
                                rt.Takmicenje1.PoredakUkupnoFinaleKupa = new PoredakUkupnoFinaleKupa();
                            rt.Takmicenje1.PoredakUkupnoFinaleKupa.create(rt, rezTak1, rezTak2);

                            rt.Takmicenje1.initPoredakSpravaFinaleKupa(takmicenje.Gimnastika);
                            foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
                                rt.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rt, rezTak1, rezTak2);

                            // update kval status za sprave
                            if (rezultatiUpdate.ContainsKey(rt.Id))
                            {
                                rezTakmicenjaSet.Add(rt.ToString());
                                foreach (RezultatSpravaFinaleKupaUpdate rezultatUpdate in rezultatiUpdate[rt.Id])
                                {
                                    foreach (RezultatSpravaFinaleKupa rezultat in
                                        rt.Takmicenje1.getPoredakSpravaFinaleKupa(rezultatUpdate.Sprava).Rezultati)
                                    {
                                        if (rezultat.Gimnasticar.Id == rezultatUpdate.GimnasticarId)
                                            rezultat.KvalStatus = rezultatUpdate.KvalStatus;
                                    }
                                }
                            }

                            if (rt.Takmicenje1.PoredakEkipnoFinaleKupa == null)
                                rt.Takmicenje1.PoredakEkipnoFinaleKupa = new PoredakEkipnoFinaleKupa();
                            rt.Takmicenje1.PoredakEkipnoFinaleKupa.create(rt, rezTak1, rezTak2);
                        }
                        else if (takmicenje.ZbirViseKola)
                        {
                            if (rt.Takmicenje1.PoredakUkupnoZbirViseKola == null)
                                rt.Takmicenje1.PoredakUkupnoZbirViseKola = new PoredakUkupnoZbirViseKola();
                            rt.Takmicenje1.PoredakUkupnoZbirViseKola.create(rt, rezTak1, rezTak2, rezTak3, rezTak4);

                            if (rt.Takmicenje1.PoredakEkipnoZbirViseKola == null)
                                rt.Takmicenje1.PoredakEkipnoZbirViseKola = new PoredakEkipnoZbirViseKola();
                            rt.Takmicenje1.PoredakEkipnoZbirViseKola.create(rt, rezTak1, rezTak2, rezTak3, rezTak4);
                        }
                    }

                    foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                        foreach (RezultatskoTakmicenje rt in rezTakmicenjaPrethKolo)
                            rezultatskoTakmicenjeDAO.Evict(rt);

                    Takmicenje1DAO takmicenje1DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO();
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                        takmicenje1DAO.Update(rt.Takmicenje1);

                    EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
                    foreach (RezultatskoTakmicenje rt in preskociRezTakmicenja)
                    {
                        // brisi rezultatska takmicenja i ekipe
                        foreach (Ekipa e in rt.Takmicenje1.Ekipe)
                            ekipaDAO.Delete(e);
                        rezultatskoTakmicenjeDAO.Delete(rt);
                    }

                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }
    }*/

    private IList<int> getTakmicenjaId()
    {
        IList<int> result = new List<int>();
        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                foreach (Takmicenje t in DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindAll())
                    result.Add(t.Id);
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }
        return result;
    }

    /*private void updatePreskok()
    {
        IList<int> takmicenjaId = getTakmicenjaId();
        StreamWriter logStreamWriter = File.CreateText("log_update_preskok.txt");
        try
        {
            for (int i = 0; i < takmicenjaId.Count; ++i)
                updatePreskok(takmicenjaId[i], i, logStreamWriter);
        }
        finally
        {
            logStreamWriter.Close();
        }
    }

    private void updatePreskok(int takmicenjeId, int i, StreamWriter logStreamWriter)
    {
        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);

                OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

                Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                if (t.ZbirViseKola)
                    return;

                string takmicenjeHeader = i.ToString() + ". " + t.ToString();
                if (t.FinaleKupa)
                    takmicenjeHeader += " - FINALE KUPA";
                takmicenjeHeader += " (" + t.Id + ")";

                IList<Ocena> ocene1 = ocenaDAO.FindByDeoTakmicenja(t.Id, DeoTakmicenjaKod.Takmicenje1);
                IList<Ocena> ocene3 = ocenaDAO.FindByDeoTakmicenja(t.Id, DeoTakmicenjaKod.Takmicenje3);
                IList<RezultatskoTakmicenje> rezTakmicenja = rezTakDAO.FindByTakmicenje(t.Id);

                if (t.FinaleKupa)
                {
                    bool postojiOdvojenoTak3 = false;
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        if (rt.Propozicije.odvojenoTak3())
                            postojiOdvojenoTak3 = true;
                    }
                    if (!postojiOdvojenoTak3 && ocene1.Count > 0)
                        throw new InfrastructureException("Greska1");
                    if (!postojiOdvojenoTak3 || ocene1.Count == 0)
                        return;
                }

                bool takmicenjeHeaderAdded = false;
                bool deoTakmicenjaHeaderAdded = false;
                foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                {
                    bool obaPreskoka;
                    if (!rt.Propozicije.PostojiTak3)
                        obaPreskoka = false;
                    else if (!rt.Propozicije.odvojenoTak3() || t.FinaleKupa)
                        obaPreskoka = rt.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;
                    else
                        obaPreskoka = rt.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                    createPoredakPreskok(rt, DeoTakmicenjaKod.Takmicenje1, ocene1, obaPreskoka, logStreamWriter,
                        takmicenjeHeader, ref takmicenjeHeaderAdded, ref deoTakmicenjaHeaderAdded);
                }
                deoTakmicenjaHeaderAdded = false;
                foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                {
                    if (rt.odvojenoTak3() && t.ZavrsenoTak1)
                    {
                        bool obaPreskoka = rt.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;
                        createPoredakPreskok(rt, DeoTakmicenjaKod.Takmicenje3, ocene3, obaPreskoka, logStreamWriter,
                          takmicenjeHeader, ref takmicenjeHeaderAdded, ref deoTakmicenjaHeaderAdded);
                    }
                }
                foreach (Ocena o in ocene1)
                    ocenaDAO.Evict(o);
                foreach (Ocena o in ocene3)
                    ocenaDAO.Evict(o);
                session.Transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }
    }

    private void createPoredakPreskok(RezultatskoTakmicenje rt,
        DeoTakmicenjaKod deoTakmicenjaKod, IList<Ocena> ocene, bool obaPreskoka, StreamWriter logStreamWriter,
        string takmicenjeHeader, ref bool takmicenjeHeaderAdded, ref bool deoTakmicenjaHeaderAdded)
    {
        bool rezTakHeaderAdded = false;

        PoredakPreskok p = rt.getPoredakPreskok(deoTakmicenjaKod);

        IDictionary<int, short?> rezultatiMap = new Dictionary<int, short?>();
        bool hasDrugaOcena = false;

        foreach (RezultatPreskok r in p.Rezultati)
        {
            if (r.Rank2 != null)
                hasDrugaOcena = true;

            if (obaPreskoka)
                rezultatiMap.Add(r.Gimnasticar.Id, r.Rank2);
            else
                rezultatiMap.Add(r.Gimnasticar.Id, r.Rank);
        }
        if (!hasDrugaOcena && obaPreskoka)
        {
            rezultatiMap.Clear();
            foreach (RezultatPreskok r in p.Rezultati)
            {
                rezultatiMap.Add(r.Gimnasticar.Id, r.Rank);
            }
        }

        p.create(rt, ocene);

        List<RezultatPreskok> changedRezultati = new List<RezultatPreskok>();
        foreach (RezultatPreskok r in p.Rezultati)
        {
            if (rezultatiMap[r.Gimnasticar.Id] != r.Rank)
                changedRezultati.Add(r);
        }
        PropertyDescriptor propDesc =
            TypeDescriptor.GetProperties(typeof(RezultatPreskok))["Rank"];
        changedRezultati.Sort(new SortComparer<RezultatPreskok>(propDesc,
            ListSortDirection.Ascending));

        bool uvekUzimajNew = false;
        foreach (RezultatPreskok r in changedRezultati)
        {
            // ako je bar jedan novi rank null to znaci da raniji poredak nije bio dobro izracunat (greska je da
            // je iz nekog razloga postojao rezultat a nisu postojale ocene) i da treba uzimati novoizracunat rank
            if (r.Rank == null)
                uvekUzimajNew = true;
        }

        foreach (RezultatPreskok r in changedRezultati)
        {
            if (!takmicenjeHeaderAdded)
            {
                logStreamWriter.WriteLine("\n");
                logStreamWriter.WriteLine("\n");
                logStreamWriter.WriteLine(takmicenjeHeader);
                logStreamWriter.WriteLine("================================================");
                takmicenjeHeaderAdded = true;
            }
            if (!deoTakmicenjaHeaderAdded)
            {
                logStreamWriter.WriteLine("\n");
                if (deoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje1)
                    logStreamWriter.WriteLine("TAKMICENJE 1");
                else
                    logStreamWriter.WriteLine("TAKMICENJE 3");
                deoTakmicenjaHeaderAdded = true;
            }
            if (!rezTakHeaderAdded)
            {
                logStreamWriter.WriteLine("\n");
                logStreamWriter.WriteLine(rt.ToString());
                logStreamWriter.WriteLine("------------------------------------------------");
                rezTakHeaderAdded = true;
            }
            string gimnasticarStr = r.Gimnasticar.ToString();
            logStreamWriter.Write(gimnasticarStr);
            for (int j = 0; j < 30 - gimnasticarStr.Length; ++j)
                logStreamWriter.Write(" ");

            short? old = rezultatiMap[r.Gimnasticar.Id];
            short? _new = r.Rank;
            if (!uvekUzimajNew && old != null)
            {
                r.Rank = old;
            }
            logStreamWriter.WriteLine("old: " + old + "\tnew: " + _new + "\tfinal: " + r.Rank);
        }

        DAOFactoryFactory.DAOFactory.GetPoredakPreskokDAO().Update(p);
    }*/

    // Ponisti ZavrsenoTak1 na svim mestima gde je ZavrsenoTak1 a nije odvojenoTak3 ni za jedno rez. takmicenje.
    public void updateZavrsenoTak1()
    {
        IList<int> takmicenjaId = getTakmicenjaId();
        StreamWriter logStreamWriter = File.CreateText("log_update_ZavrsenoTak1.txt");

        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                int numUpdated = 0;
                for (int i = 0; i < takmicenjaId.Count; ++i)
                {
                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    Takmicenje t = takmicenjeDAO.FindById(takmicenjaId[i]);
                    if (!t.ZavrsenoTak1)
                    {
                        takmicenjeDAO.Evict(t);
                        continue;
                    }

                    string takmicenjeHeader = i.ToString() + ". " + t.ToString();
                    takmicenjeHeader += " (" + t.Id + ")";

                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    IList<RezultatskoTakmicenje> rezTakmicenja = rezTakDAO.FindByTakmicenje(t.Id);

                    bool postojiOdvojeno = false;
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        if (rt.odvojenoTak2() || rt.odvojenoTak3() || rt.odvojenoTak4())
                        {
                            postojiOdvojeno = true;
                            break;
                        }
                    }
                    if (postojiOdvojeno)
                        continue;

                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                    IList<Ocena> ocene2 = ocenaDAO.FindByDeoTakmicenja(t.Id, DeoTakmicenjaKod.Takmicenje2);
                    IList<Ocena> ocene3 = ocenaDAO.FindByDeoTakmicenja(t.Id, DeoTakmicenjaKod.Takmicenje3);
                    IList<Ocena> ocene4 = ocenaDAO.FindByDeoTakmicenja(t.Id, DeoTakmicenjaKod.Takmicenje4);

                    if (ocene2.Count > 0 || ocene3.Count > 0 || ocene4.Count > 0)
                    {
                        logStreamWriter.WriteLine("Preskacem " + takmicenjeHeader + ", postoje ocene");
                        continue;
                    }

                    ++numUpdated;
                    t.ZavrsenoTak1 = false;

                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        if (rt.odvojenoTak2())
                        {
                            rt.Takmicenje2.clearUcesnici();
                            rt.Takmicenje2.Poredak.Rezultati.Clear();
                        }
                        if (rt.odvojenoTak3())
                        {
                            rt.Takmicenje3.clearUcesnici();
                            foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                                p.Rezultati.Clear();
                            rt.Takmicenje3.PoredakPreskok.Rezultati.Clear();
                        }
                        if (rt.odvojenoTak4())
                        {
                            rt.Takmicenje4.clearUcesnici();
                            rt.Takmicenje4.Poredak.Rezultati.Clear();
                        }

                        if (rt.odvojenoTak2())
                            DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Update(rt.Takmicenje2);
                        if (rt.odvojenoTak3())
                            DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Update(rt.Takmicenje3);
                        if (rt.odvojenoTak4())
                            DAOFactoryFactory.DAOFactory.GetTakmicenje4DAO().Update(rt.Takmicenje4);
                    }
                    takmicenjeDAO.Update(t);
                    logStreamWriter.WriteLine(takmicenjeHeader);
                }
                if (numUpdated > 0)
                    session.Transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            logStreamWriter.Close();
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }
    }

    // Ponisti Q ako nema odvojenog takmicenja 2, 3 ili 4.
    public void updateKvalifikanti()
    {
        IList<int> takmicenjaId = getTakmicenjaId();
        for (int i = 0; i < takmicenjaId.Count; ++i)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjaId[i]);

                    IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                        .FindByTakmicenje(t.Id);

                    PoredakUkupnoDAO poredakUkupnoDAO = DAOFactoryFactory.DAOFactory.GetPoredakUkupnoDAO();
                    PoredakSpravaDAO poredakSpravaDAO = DAOFactoryFactory.DAOFactory.GetPoredakSpravaDAO();
                    PoredakPreskokDAO poredakPreskokDAO = DAOFactoryFactory.DAOFactory.GetPoredakPreskokDAO();
                    PoredakEkipnoDAO poredakEkipnoDAO = DAOFactoryFactory.DAOFactory.GetPoredakEkipnoDAO();

                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        if (!rt.odvojenoTak2())
                        {
                            foreach (RezultatUkupno r in rt.Takmicenje1.PoredakUkupno.Rezultati)
                                r.KvalStatus = KvalifikacioniStatus.None;
                            poredakUkupnoDAO.Update(rt.Takmicenje1.PoredakUkupno);
                        }
                        if (!rt.odvojenoTak3())
                        {
                            foreach (PoredakSprava p in rt.Takmicenje1.PoredakSprava)
                            {
                                foreach (RezultatSprava r in p.Rezultati)
                                    r.KvalStatus = KvalifikacioniStatus.None;
                                poredakSpravaDAO.Update(p);
                            }
                            foreach (RezultatPreskok r in rt.Takmicenje1.PoredakPreskok.Rezultati)
                                r.KvalStatus = KvalifikacioniStatus.None;
                            poredakPreskokDAO.Update(rt.Takmicenje1.PoredakPreskok);
                        }
                        if (!rt.odvojenoTak4())
                        {
                            foreach (RezultatEkipno r in rt.Takmicenje1.PoredakEkipno.Rezultati)
                                r.KvalStatus = KvalifikacioniStatus.None;
                            poredakEkipnoDAO.Update(rt.Takmicenje1.PoredakEkipno);
                        }
                    }
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }
    }

    private void updateLastModified()
    {
        IList<int> takmicenjaId = getTakmicenjaId();
        for (int i = 0; i < takmicenjaId.Count; ++i)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    Takmicenje t = takmicenjeDAO.FindById(takmicenjaId[i]);
                    if (t.PrvoKolo != null)
                    {
                        if (t.PrvoKolo.Datum > t.Datum)
                            throw new Exception("Prvo kolo je kasnije od takmicenja - " + t.ToString());
                        takmicenjeDAO.Evict(t.PrvoKolo);
                    }
                    if (t.DrugoKolo != null)
                    {
                        if (t.DrugoKolo.Datum > t.Datum)
                            throw new Exception("Drugo kolo je kasnije od takmicenja - " + t.ToString());
                        takmicenjeDAO.Evict(t.DrugoKolo);
                    }
                    if (t.TreceKolo != null)
                    {
                        if (t.TreceKolo.Datum > t.Datum)
                            throw new Exception("Trece kolo je kasnije od takmicenja - " + t.ToString());
                        takmicenjeDAO.Evict(t.TreceKolo);
                    }
                    if (t.CetvrtoKolo != null)
                    {
                        if (t.CetvrtoKolo.Datum > t.Datum)
                            throw new Exception("Cetvrto kolo je kasnije od takmicenja - " + t.ToString());
                        takmicenjeDAO.Evict(t.CetvrtoKolo);
                    }
                    t.LastModified = t.Datum;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }
    }

    private void addTakmicenjeToRasporedNastupa()
    {
        /*ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                RasporedNastupaDAO rasporedNastupaDAO = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO();
                IList<RasporedNastupa> rasporedi = rasporedNastupaDAO.FindAll();
                foreach (RasporedNastupa r in rasporedi)
                {
                    IList<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>(r.Kategorije);
                    r.Takmicenje = kategorije[0].Takmicenje;
                    r.Naziv = RasporedNastupa.kreirajNaziv(kategorije);
                    r.Kategorije.Clear();
                    rasporedNastupaDAO.Update(r);
                }
                session.Transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }*/
    }

    private void addTakmicenjeToRasporedSudija()
    {
        /*ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                RasporedSudijaDAO rasporedSudijaDAO = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO();
                IList<RasporedSudija> rasporedi = rasporedSudijaDAO.FindAll();
                foreach (RasporedSudija r in rasporedi)
                {
                    IList<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>(r.Kategorije);
                    r.Takmicenje = kategorije[0].Takmicenje;
                    r.Naziv = RasporedNastupa.kreirajNaziv(kategorije);
                    r.Kategorije.Clear();
                    rasporedSudijaDAO.Update(r);
                }
                session.Transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }*/
    }

    private void updateRegistarskiBrojToString()
    {
        /*ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                GimnasticarDAO gimnasticarDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO();
                IList<Gimnasticar> gimnasticari = gimnasticarDAO.FindAll();
                foreach (Gimnasticar g in gimnasticari)
                {
                    if (g.RegBroj == null)
                        g.RegistarskiBroj = String.Empty;
                    else
                        g.RegistarskiBroj = g.RegBroj.ToString();
                    gimnasticarDAO.Update(g);
                }
                session.Transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }*/
    }

    private void updateNacinRacunanjaOceneFinaleKupa()
    {
        /*ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                PropozicijeDAO propozicijeDAO = DAOFactoryFactory.DAOFactory.GetPropozicijeDAO();
                IList<Propozicije> propozicije = propozicijeDAO.FindAll();
                foreach (Propozicije p in propozicije)
                {
                    if (p.Tak2FinalnaOcenaJeZbirObaKola)
                        p.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.Zbir;
                    else if (p.Tak2FinalnaOcenaJeMaxObaKola)
                        p.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.Max;
                    else if (p.Tak2FinalnaOcenaJeProsekObaKola)
                    {
                        if (p.Tak2NeRacunajProsekAkoNemaOceneIzObaKola)
                            p.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene;
                        else
                            p.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.ProsekUvek;
                    }
                    else
                        p.NacinRacunanjaOceneFinaleKupaTak2 = NacinRacunanjaOceneFinaleKupa.Undefined;

                    if (p.Tak3FinalnaOcenaJeZbirObaKola)
                        p.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.Zbir;
                    else if (p.Tak3FinalnaOcenaJeMaxObaKola)
                        p.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.Max;
                    else if (p.Tak3FinalnaOcenaJeProsekObaKola)
                    {
                        if (p.Tak3NeRacunajProsekAkoNemaOceneIzObaKola)
                            p.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene;
                        else
                            p.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.ProsekUvek;
                    }
                    else
                        p.NacinRacunanjaOceneFinaleKupaTak3 = NacinRacunanjaOceneFinaleKupa.Undefined;

                    if (p.Tak4FinalnaOcenaJeZbirObaKola)
                        p.NacinRacunanjaOceneFinaleKupaTak4 = NacinRacunanjaOceneFinaleKupa.Zbir;
                    else if (p.Tak4FinalnaOcenaJeMaxObaKola)
                        p.NacinRacunanjaOceneFinaleKupaTak4 = NacinRacunanjaOceneFinaleKupa.Max;
                    else if (p.Tak4FinalnaOcenaJeProsekObaKola)
                    {
                        if (p.Tak4NeRacunajProsekAkoNemaOceneIzObaKola)
                            p.NacinRacunanjaOceneFinaleKupaTak4 = NacinRacunanjaOceneFinaleKupa.ProsekSamoAkoPostojeObeOcene;
                        else
                            p.NacinRacunanjaOceneFinaleKupaTak4 = NacinRacunanjaOceneFinaleKupa.ProsekUvek;
                    }
                    else
                        p.NacinRacunanjaOceneFinaleKupaTak4 = NacinRacunanjaOceneFinaleKupa.Undefined;

                    propozicijeDAO.Update(p);
                }
                session.Transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }*/
    }

    private void updateNacinRacunanjaPreskoka()
    {
        IList<int> takmicenjaId = getTakmicenjaId();
        for (int i = 0; i < takmicenjaId.Count; ++i)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

                    Takmicenje t = takmicenjeDAO.FindById(takmicenjaId[i]);
                    IList<RezultatskoTakmicenje> rezTakmicenja = rezTakDAO.FindByTakmicenje(t.Id);

                    foreach (RezultatskoTakmicenjeDescription d in t.TakmicenjeDescriptions)
                        updateNacinRacunanjaPreskoka(d.Propozicije, t);
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                        updateNacinRacunanjaPreskoka(rt.Propozicije, t);

                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }
    }

    private void updateNacinRacunanjaPreskoka(Propozicije p, Takmicenje t)
    {
        if (!p.PostojiTak3)
        {

        }
        else if (!p.OdvojenoTak3)
        {
            p.Tak1PreskokNaOsnovuObaPreskoka = p.Tak3PreskokNaOsnovuObaPreskoka;
            p.Tak3PreskokNaOsnovuObaPreskoka = false;
        }
        else
        {
            if (!t.FinaleKupa)
            {

            }
            else
            {
                p.Tak1PreskokNaOsnovuObaPreskoka = p.Tak3PreskokNaOsnovuObaPreskoka;
                p.Tak3PreskokNaOsnovuObaPreskoka = false;
            }
        }
    }

    // Uvek treba da bude Takmicenje2 != null. Isto i za takmicenja 3 i 4.
    public void updateTakmicenja234()
    {
        /*StreamWriter logStreamWriter = File.CreateText("log_takmicenja234.txt");
        IList<int> takmicenjaId = getTakmicenjaId();
        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                for (int i = 0; i < takmicenjaId.Count; ++i)
                {
                    Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjaId[i]);

                    string takmicenjeHeader = i.ToString() + ". " + t.ToString();
                    if (t.FinaleKupa)
                        takmicenjeHeader += " - FINALE KUPA";
                    else if (t.ZbirViseKola)
                        takmicenjeHeader += " - ZBIR VISE KOLA";
                    takmicenjeHeader += " (" + t.Id + ")";

                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    bool tak2 = false;
                    bool tak3 = false;
                    bool tak4 = false;
                    foreach (RezultatskoTakmicenje rt in rezTakDAO.FindByTakmicenje(t.Id))
                    {
                        if (rt.Takmicenje2 == null)
                        {
                            tak2 = true;
                            rt.Takmicenje2 = new Takmicenje2();
                            rezTakDAO.Update(rt);
                        }
                        if (rt.Takmicenje3 == null)
                        {
                            tak3 = true;
                            rt.Takmicenje3 = new Takmicenje3(t.Gimnastika);
                            rezTakDAO.Update(rt);
                        }
                        if (rt.Takmicenje4 == null)
                        {
                            tak4 = true;
                            rt.Takmicenje4 = new Takmicenje4();
                            rezTakDAO.Update(rt);
                        }
                    }
                    if (tak2 || tak3 || tak4)
                    {
                        string msg = String.Empty;
                        if (tak2)
                            msg += "   TAK2";
                        if (tak3)
                            msg += "   TAK3";
                        if (tak4)
                            msg += "   TAK4";
                        logStreamWriter.WriteLine(takmicenjeHeader + msg);
                    }
                }
                session.Transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            logStreamWriter.Close();
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }*/
    }

    // Na takmicenju "ZSG - I KOLO PGL SRBIJE ZSG, NOVI SAD, 18.5.2013" nije unesena nijedna ocena u takmicenju 3,
    // ali poredak postoji i to sa ocenama iz takmicenja 1. Poredak za preskok je ispravljen sa apdejtom koji je
    // ponovo racunao poredak preskok za sva takmicenja. Ovaj metod ispravlja poretke i za ostale sprave.
    private void updateTakmicenje_200()
    {
        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(200);
                if (!t.ToString().StartsWith("ZSG - I KOLO PGL SRBIJE "))
                    throw new Exception("Pogresno takmicenje");
                if (!t.ToString().EndsWith("NOVI SAD, 18.5.2013"))
                    throw new Exception("Pogresno takmicenje");

                RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                Takmicenje3DAO tak3DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO();
                foreach (RezultatskoTakmicenje rt in rezTakDAO.FindByTakmicenje(t.Id))
                {
                    if (rt.Takmicenje3 == null)
                        continue;
                    if (!rt.odvojenoTak3())
                        continue;
                    foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                    {
                        foreach (RezultatSprava r in p.Rezultati)
                            r.clearOcena();
                    }
                    tak3DAO.Update(rt.Takmicenje3);
                }
                session.Transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }
    }
}
