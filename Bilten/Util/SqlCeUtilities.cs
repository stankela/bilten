using System.Data.SqlServerCe;
using System;

public class SqlCeUtilities
{
    public void CreateDatabase(string fileName, string password)
    {
        string connectionString;
        if (System.IO.File.Exists(fileName))
            return;
            //System.IO.File.Delete(fileName);
        
        // The DataSource must be surrounded with double quotes. The Password, on the other hand, must be surrounded 
        // with single quotes
        connectionString = string.Format(
          "DataSource=\"{0}\"; Password='{1}'", fileName, password);

        // we'll use the SqlServerCe connection object to get the database file path
        using (SqlCeConnection localConnection = new SqlCeConnection(connectionString))
        {
            // The SqlCeConnection.Database property contains the file parth portion
            // of the database from the full connectionstring
            if (!System.IO.File.Exists(localConnection.Database))
            {
                using (SqlCeEngine sqlCeEngine = new SqlCeEngine(connectionString))
                {
                    sqlCeEngine.CreateDatabase();
                    CreateInitialDatabaseObjects(connectionString);
                }
            }
        }
    }

    private void CreateInitialDatabaseObjects(string connectionString)
    {
        using (SqlCeConnection connection = new SqlCeConnection(connectionString))
        {
            // To simplify editing and testing TSQL statements,
            // the commands are placed in a managed resource of the dll.
            string script;            
            string resourceName = "Soko.create_all_objects.sqlce";
            System.Reflection.Assembly assem = this.GetType().Assembly;
            using (System.IO.Stream stream = assem.GetManifestResourceStream(resourceName))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
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