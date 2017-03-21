using System.Data.SqlServerCe;
using System;
using System.IO;
using System.Reflection;

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
}