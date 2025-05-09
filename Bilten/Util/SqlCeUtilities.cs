using System.Data.SqlServerCe;
using System;
using System.IO;
using System.Reflection;
using Bilten.Exceptions;
using System.Data;

namespace Bilten.Dao
{
    public class SqlCeUtilities
    {
        private static string getConnectionString(string fileName, string password)
        {
            string result;

            // The DataSource must be surrounded with double quotes. The Password, on the other hand,
            // must be surrounded  with single quotes
            if (password != String.Empty)
                result = string.Format(
                  "DataSource=\"{0}\"; Password='{1}'", fileName, password);
            else
                result = string.Format(
                  "DataSource=\"{0}\"", fileName);
            return result;
        }

        public static bool CreateDatabase(string fileName, string password, bool overwrite)
        {
            string connectionString = getConnectionString(fileName, password);

            // we'll use the SqlServerCe connection object to get the database file path
            using (SqlCeConnection localConnection = new SqlCeConnection(connectionString))
            {
                // The SqlCeConnection.Database property contains the file path portion
                // of the database from the full connection string
                if (File.Exists(localConnection.Database))
                {
                    if (overwrite)
                        File.Delete(localConnection.Database);
                    else
                        return false;
                }

                using (SqlCeEngine sqlCeEngine = new SqlCeEngine(connectionString))
                {
                    sqlCeEngine.CreateDatabase();
                }

                // Fajl CreateAllObjects.sqlce je takodje moguce dobiti koristeci tool exportsqlce (imam ga u VS2010).
                // Komanda je:
                // ExportSqlCE.exe "Data Source=C:\Users\sale\Documents\Visual Studio 2012\Projects\
                //    Bilten\Bilten\bin\Release\BiltenPodaci.sdf" bilten_podaci.sql
                // Primeri koriscenja se dobijaju kada se pokrene bez ikakvih opcija. Npr, moguce je generisati
                // samo semu, samo podatke, i semu i podatke, itd. GO delimiter u skipt fajlu je neophodan
                // (SqlCeUtilities.ExecuteScript razdvaja ceo skipt fajl na individualne komande ocekujuci GO kao delimiter) 
                SqlCeUtilities.ExecuteScript(ConfigurationParameters.DatabaseFile, "",
                    "Bilten.CreateAllObjects.sqlce", true);
            }
            return true;
        }

        public static void ExecuteScript(string fileName, string password, string scriptFile, bool embeddedResource)
        {
            string connectionString = getConnectionString(fileName, password);
            using (SqlCeConnection connection = new SqlCeConnection(connectionString))
            {
                string script = String.Empty;
                if (embeddedResource)
                {
                    Assembly assem = Assembly.GetExecutingAssembly();
                    using (Stream stream = assem.GetManifestResourceStream(scriptFile))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            script = reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    using (StreamReader reader = new StreamReader(scriptFile))
                    {
                        script = reader.ReadToEnd();
                    }
                }

                // Using the SQL Management Studio convention of using GO to identify individual commands
                // Create a list of commands to execute
                string[] commands = script.Split(
                    new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = connection;
                connection.Open();
                foreach (string command in commands)
                {
                    string command2 = command.Trim();
                    if (!String.IsNullOrEmpty(command2))
                    {
                        cmd.CommandText = command2;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static SqlCeDataReader executeReader(SqlCeCommand cmd, string readErrorMsg, string connectionString = null)
        {
            if (connectionString == null)
                connectionString = ConfigurationParameters.ConnectionString;
            SqlCeConnection conn = new SqlCeConnection(connectionString);
            cmd.Connection = conn;
            try
            {
                conn.Open();
                // CommandBehavior.CloseConnection znaci da kada se DataReader zatvori
                // zatvara se i konekcija
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlCeException e)
            {
                // in Open()
                conn.Close();
                throw new InfrastructureException(readErrorMsg, e);
            }
            catch (InvalidOperationException e)
            {
                // in ExecuteReader()
                conn.Close();
                throw new InfrastructureException(readErrorMsg, e);
            }
        }

        public static bool tableExists(string tableName)
        {
            string sql = @"SELECT * FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_NAME = @TableName";
            SqlCeCommand cmd = new SqlCeCommand(sql);
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;

            string errMsg = "Greska prilikom citanja podataka iz baze.";
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, errMsg);
            bool result = false;
            if (rdr.Read())
                result = true;
            rdr.Close();
            return result;
        }

        public static bool columnExists(string table, string column)
        {
            string sql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName";
            SqlCeCommand cmd = new SqlCeCommand(sql);
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = table;
            cmd.Parameters.Add("@ColumnName", SqlDbType.NVarChar).Value = column;

            string errMsg = "Greska prilikom citanja podataka iz baze.";
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, errMsg);
            bool result = false;
            if (rdr.Read())
                result = true;
            rdr.Close();
            return result;
        }

        public static int getDatabaseVersionNumber()
        {
            string tableName = "verzija_baze";
            if (!SqlCeUtilities.tableExists(tableName))
            {
                return -1;
            }

            string sql = "SELECT broj_verzije FROM " + tableName +
                " WHERE verzija_id = 1";
            SqlCeCommand cmd = new SqlCeCommand(sql);

            string errMsg = "Greska prilikom citanja podataka iz baze.";
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, errMsg);
            int result = -1;
            if (rdr.Read())
                result = rdr.GetInt32(0);
            rdr.Close();
            return result;
        }

        public static void updateDatabaseVersionNumber(int newVersion)
        {
            string tableName = "verzija_baze";
            string sql =
                String.Format("UPDATE {0} SET broj_verzije = {1} WHERE verzija_id = 1", tableName, newVersion);
            SqlCeCommand cmd = new SqlCeCommand(sql);

            SqlCeConnection conn = new SqlCeConnection(ConfigurationParameters.ConnectionString);
            string errMsg = "Neuspesna promena baze.";
            SqlCeTransaction tr = null;
            
            // TODO: Ovaj kod se ponavlja na vise mesta u fajlu.
            try
            {
                conn.Open();
                tr = conn.BeginTransaction();

                cmd.Connection = conn;
                cmd.Transaction = tr;
                int result = cmd.ExecuteNonQuery();

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
        }

        public static void addColumn(string table, string column, SqlDbType type,
            Nullable<int> size, object columnValue)
        {
            if (SqlCeUtilities.columnExists(table, column))
                return;

            string addColumnSql = "ALTER TABLE [" + table + "] ADD COLUMN [" + column + "] " +
                getSqlType(type, size);
            string updateColumnSql = "UPDATE [" + table + "] SET [" + column + "] = @" +
                column;

            SqlCeCommand addColumnCommand = new SqlCeCommand(addColumnSql);
            SqlCeCommand updateColumnCommand = new SqlCeCommand(updateColumnSql);
            if (size != null)
                updateColumnCommand.Parameters.Add("@" + column, type, size.Value).Value = columnValue;
            else
                updateColumnCommand.Parameters.Add("@" + column, type).Value = columnValue;

            SqlCeConnection conn = new SqlCeConnection(ConfigurationParameters.ConnectionString);
            string errMsg = "Neuspesna promena baze.";
            SqlCeTransaction tr = null;
            try
            {
                conn.Open();
                tr = conn.BeginTransaction();

                addColumnCommand.Connection = conn;
                addColumnCommand.Transaction = tr;
                addColumnCommand.ExecuteNonQuery();

                updateColumnCommand.Connection = conn;
                updateColumnCommand.Transaction = tr;
                updateColumnCommand.ExecuteNonQuery();

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
        }

        private static string getSqlType(SqlDbType type, Nullable<int> size)
        {
            switch (type)
            {
                case SqlDbType.NVarChar:
                    return "nvarchar(" + size.Value + ")";

                case SqlDbType.TinyInt:
                    return "tinyint";

                default:
                    throw new Exception();
            }
        }

        public static void dropReferentialConstraint(string table, string referencedTable)
        {
            string findConstraintSQL = @"
                SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                WHERE CONSTRAINT_TABLE_NAME = @Table AND UNIQUE_CONSTRAINT_TABLE_NAME = @ReferencedTable";
            SqlCeCommand cmd = new SqlCeCommand(findConstraintSQL);
            cmd.Parameters.Add("@Table", SqlDbType.NVarChar).Value = table;
            cmd.Parameters.Add("@ReferencedTable", SqlDbType.NVarChar).Value = referencedTable;

            string errMsg = "Greska prilikom citanja podataka iz baze.";
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, errMsg);

            if (!rdr.Read())
                throw new Exception("Constraint does not exist.");

            string constraintName = (string)rdr["CONSTRAINT_NAME"];

            // NOTE: Izgleda da dodavanje parametara (pomocu @parameterName) radi samo kada je parametar sa desne
            // strane znaka jednakosti (kao u findConstraintSQL). Zato ovde koristim spajanje stringova.
            string dropConstraintSQL = "ALTER TABLE " + table + " DROP CONSTRAINT " + constraintName;

            SqlCeCommand cmd2 = new SqlCeCommand(dropConstraintSQL);

            SqlCeConnection conn = new SqlCeConnection(ConfigurationParameters.ConnectionString);
            errMsg = "Neuspesna promena baze.";
            SqlCeTransaction tr = null;
            try
            {
                conn.Open();
                tr = conn.BeginTransaction();

                cmd2.Connection = conn;
                cmd2.Transaction = tr;
                int result = cmd2.ExecuteNonQuery();

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
        }

    }
}