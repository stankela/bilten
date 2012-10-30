using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using Bilten.Exceptions;
using Bilten.Dao;
using System.Data;

namespace Bilten
{
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
            SqlCeDataReader rdr = Database.executeReader(cmd, errMsg);
            bool result = false;
            if (rdr.Read())
                result = true;
            rdr.Close();
            return result;
        }
    }
}
