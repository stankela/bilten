using System.Data.SqlServerCe;
using System;
using System.IO;

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

    public static void ExecuteScript(string fileName, string password, string scriptFile)
    {
        string connectionString = getConnectionString(fileName, password);
        using (SqlCeConnection connection = new SqlCeConnection(connectionString))
        {
            string script = String.Empty;

            // Kada je script ugradjen u assembly.
            /*string resourceName = "Soko.create_all_objects.sqlce";
            System.Reflection.Assembly assem = this.GetType().Assembly;
            using (System.IO.Stream stream = assem.GetManifestResourceStream(resourceName))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    script = reader.ReadToEnd();
                }
            }*/

            using (System.IO.StreamReader reader = new System.IO.StreamReader(scriptFile))
            {
                script = reader.ReadToEnd();
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
}