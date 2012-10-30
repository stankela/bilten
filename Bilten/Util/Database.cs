using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using Bilten.Exceptions;
using System.Data;

namespace Bilten.Dao
{
    public class Database
    {
        public static SqlCeDataReader executeReader(SqlCeCommand cmd, string readErrorMsg)
        {
            SqlCeConnection conn = new SqlCeConnection(ConfigurationParameters.ConnectionString);
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
}
