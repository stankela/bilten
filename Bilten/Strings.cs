using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten
{
    public static class Strings
    {
        public static readonly string DatabaseAccessExceptionMessage =
            "Greska prilikom pristupa bazi podataka.";

        public static string getFullDatabaseAccessExceptionMessage(Exception ex)
        {
            return String.Format(
                "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
        }
    }
}
