using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using System.Data;
using Bilten.Exceptions;
using Bilten.Dao;

namespace Bilten
{
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
            SqlCeDataReader rdr = Database.executeReader(cmd, errMsg);
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
            SqlCeDataReader rdr = Database.executeReader(cmd, errMsg);
            int result = 0;
            if (rdr.Read())
                result = rdr.GetInt32(0);
            rdr.Close();
            return result;
        }
    }
}
