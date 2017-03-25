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
                // The SqlCeConnection.Database property contains the file parth portion
                // of the database from the full connectionstring
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
    }

    public class ColumnAdder
    {
        private string table;
        private string column;

        private SqlCeCommand addColumnCommand;
        private SqlCeCommand updateColumnCommand;

        public ColumnAdder(string table, string column, SqlDbType type,
            Nullable<int> size, object columnValue)
        {
            this.table = table;
            this.column = column;

            string addColumnSql = "ALTER TABLE [" + table + "] ADD COLUMN [" + column + "] " +
                getTypeSqlString(type, size);
            string updateColumnSql = "UPDATE [" + table + "] SET [" + column + "] = @" +
                column;

            addColumnCommand = new SqlCeCommand(addColumnSql);
            updateColumnCommand = new SqlCeCommand(updateColumnSql);
            if (size != null)
                updateColumnCommand.Parameters.Add("@" + column, type, size.Value).Value = columnValue;
            else
                updateColumnCommand.Parameters.Add("@" + column, type).Value = columnValue;
        }

        private string getTypeSqlString(SqlDbType type, Nullable<int> size)
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

        public void addColumn()
        {
            if (!columnExists(table, column))
            {
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
        }

        private bool columnExists(string table, string column)
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
    }

    public class DatabaseUpdater
    {
        public void updateDatabase()
        {
            addColumn("Elementi", "NazivPoGimnasticaru", SqlDbType.NVarChar, 64, "");
            addColumn("Slike", "ProcenatRedukcije", SqlDbType.TinyInt, null, 100);
        }

        private void addColumn(string table, string column, SqlDbType type, Nullable<int> size, object columnValue)
        {
            ColumnAdder adder = new ColumnAdder(table, column, type, size, columnValue);
            adder.addColumn();
        }

        private static bool tableExists(string tableName)
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

        public static int getDatabaseVersionNumber()
        {
            string tableName = "verzija_baze";
            if (!DatabaseUpdater.tableExists(tableName))
            {
                return 0;
            }

            string sql = "SELECT broj_verzije FROM " + tableName +
                " WHERE verzija_id = 1";
            SqlCeCommand cmd = new SqlCeCommand(sql);

            string errMsg = "Greska prilikom citanja podataka iz baze.";
            SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, errMsg);
            int result = 0;
            if (rdr.Read())
                result = rdr.GetInt32(0);
            rdr.Close();
            return result;
        }
    }
}