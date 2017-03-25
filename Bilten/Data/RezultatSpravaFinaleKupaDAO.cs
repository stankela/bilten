using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Data.SqlServerCe;
using System.Data;
using Bilten.Exceptions;
using Bilten.Dao;

namespace Bilten.Data
{
    // TODO: Prebaci u Bilten.Dao, slicno kao klasa KonacanPlasmanDAO koja takodje nije NHibernate specificna.
    class RezultatSpravaFinaleKupaDAO
    {
        private string insertSQL =
            "INSERT INTO rezultati_sprava_finale_kupa_update " +
            "(rez_takmicenje_id, gimnasticar_id, sprava, kval_status) " +
            "VALUES (@rez_takmicenje_id, @gimnasticar_id, @sprava, @kval_status)";

        private string findSQL =
            "SELECT * FROM rezultati_sprava_finale_kupa_update " +
            "WHERE rez_takmicenje_id = @rez_takmicenje_id " +
            "AND gimnasticar_id = @gimnasticar_id " +
            "AND sprava = @sprava";

        private string updateSQL =
            "UPDATE rezultati_sprava_finale_kupa_update " +
            "SET kval_status = @kval_status " +
            "WHERE rez_takmicenje_id = @rez_takmicenje_id " +
            "AND gimnasticar_id = @gimnasticar_id " +
            "AND sprava = @sprava";

        private string findByRezTakSQL =
            "SELECT * FROM rezultati_sprava_finale_kupa_update " +
            "WHERE rez_takmicenje_id = @rez_takmicenje_id";

        //private string sqlGetId = "SELECT @@IDENTITY";
        
        public void insert(GimnasticarUcesnik gimnasticar, RezultatskoTakmicenje rezTak,
            Sprava sprava, KvalifikacioniStatus newKvalStatus)
        {
            SqlCeConnection conn = new SqlCeConnection(ConfigurationParameters.ConnectionString);
            SqlCeCommand cmd = new SqlCeCommand(insertSQL, conn);

            cmd.Parameters.Add("@rez_takmicenje_id", SqlDbType.Int).Value = rezTak.Id;
            cmd.Parameters.Add("@gimnasticar_id", SqlDbType.Int).Value = gimnasticar.Id;
            cmd.Parameters.Add("@sprava", SqlDbType.TinyInt).Value = sprava;
            cmd.Parameters.Add("@kval_status", SqlDbType.TinyInt).Value = newKvalStatus;

            SqlCeTransaction tr = null;
            try
            {
                conn.Open();
                tr = conn.BeginTransaction();
                cmd.Transaction = tr;
                int recordsAffected = cmd.ExecuteNonQuery();
                if (recordsAffected != 1)
                {
                    throw new InfrastructureException(Strings.DatabaseAccessExceptionMessage);
                }

                /*SqlCeCommand cmd2 = new SqlCeCommand(sqlGetId, conn, tr);
                object id = cmd2.ExecuteScalar();
                if (!Convert.IsDBNull(id))
                    entity.Id = Convert.ToInt32(id);*/

                tr.Commit(); // TODO: this can throw Exception and InvalidOperationException
            }
            catch (SqlCeException ex)
            {
                // in Open()
                if (tr != null)
                    tr.Rollback(); // TODO: this can throw Exception and InvalidOperationException
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            catch (InvalidOperationException ex)
            {
                // in ExecuteNonQuery(), ExecureScalar()
                if (tr != null)
                    tr.Rollback();
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
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
        }

        // can throw InfrastructureException
        public bool postojiRezultatSpravaFinaleKupaUpdate(GimnasticarUcesnik gimnasticar, RezultatskoTakmicenje rezTak,
            Sprava sprava)
        {
            SqlCeCommand cmd = new SqlCeCommand(findSQL);
            cmd.Parameters.Add("@rez_takmicenje_id", SqlDbType.Int).Value = rezTak.Id;
            cmd.Parameters.Add("@gimnasticar_id", SqlDbType.Int).Value = gimnasticar.Id;
            cmd.Parameters.Add("@sprava", SqlDbType.TinyInt).Value = sprava;

            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);
            bool result = false;
            if (rdr.Read())
                result = true;
            rdr.Close();
            return result;
        }

        public void update(GimnasticarUcesnik gimnasticar, RezultatskoTakmicenje rezTak,
            Sprava sprava, KvalifikacioniStatus newKvalStatus)
        {
            SqlCeConnection conn = new SqlCeConnection(ConfigurationParameters.ConnectionString);
            SqlCeCommand cmd = new SqlCeCommand(updateSQL, conn);

            cmd.Parameters.Add("@rez_takmicenje_id", SqlDbType.Int).Value = rezTak.Id;
            cmd.Parameters.Add("@gimnasticar_id", SqlDbType.Int).Value = gimnasticar.Id;
            cmd.Parameters.Add("@sprava", SqlDbType.TinyInt).Value = sprava;
            cmd.Parameters.Add("@kval_status", SqlDbType.TinyInt).Value = newKvalStatus;

            SqlCeTransaction tr = null;
            try
            {
                conn.Open();
                tr = conn.BeginTransaction(); // zbog updateDependents, inace ne mora
                cmd.Transaction = tr;
                int recordsAffected = cmd.ExecuteNonQuery();
                if (recordsAffected != 1)
                {
                    // TODO: Ukoliko se radi optimistic offline lock, ova grana se
                    // ce se izvrsavati ako su podaci u bazi u medjuvremenu promenjeni,
                    // pa bi trebalo izbaciti izuzetak koji bi to signalizirao
                    // (npr. NHibernate ima izuzetak StaleObjectStateException).
                    // Generalno, i kod insert i kod update i kod delete bi
                    // slucajevi gde se komanda izvrsi bez problema ali se
                    // recordsAffected razlikuje od ocekivanog
                    // trebalo da se signaliziraju razlicitim tipom izuzetka

                    throw new InfrastructureException(Strings.DatabaseAccessExceptionMessage);
                }
                tr.Commit(); // TODO: this can throw Exception and InvalidOperationException
            }
            catch (SqlCeException ex)
            {
                // in Open()
                if (tr != null)
                    tr.Rollback(); // TODO: this can throw Exception and InvalidOperationException
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            catch (InvalidOperationException ex)
            {
                // in ExecuteNonQuery(), ExecureScalar()
                if (tr != null)
                    tr.Rollback();
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
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
        }

        // can throw InfrastructureException
        public List<RezultatSpravaFinaleKupaUpdate> findByRezTak(RezultatskoTakmicenje rezTak)
        {
            SqlCeCommand cmd = new SqlCeCommand(findByRezTakSQL);
            cmd.Parameters.Add("@rez_takmicenje_id", SqlDbType.Int).Value = rezTak.Id;
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

            List<RezultatSpravaFinaleKupaUpdate> result = new List<RezultatSpravaFinaleKupaUpdate>();
            while (rdr.Read())
            {
                RezultatSpravaFinaleKupaUpdate rezultat = new RezultatSpravaFinaleKupaUpdate();
                rezultat.RezultatskoTakmicenjeId = (int)rdr["rez_takmicenje_id"];
                rezultat.GimnasticarId = (int)rdr["gimnasticar_id"];
                rezultat.Sprava = (Sprava)(byte)rdr["sprava"];
                rezultat.KvalStatus = (KvalifikacioniStatus)(byte)rdr["kval_status"];
                result.Add(rezultat);
            }

            rdr.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader
            return result;
        }

    }
}
