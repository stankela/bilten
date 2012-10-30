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
    }
}
