using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten
{
    public static class Strings
    {
        public static readonly string DATABASE_ACCESS_ERROR_MSG =
            "Greska prilikom pristupa bazi podataka.";

        public static string getFullDatabaseAccessExceptionMessage(Exception ex)
        {
            return String.Format(
                "{0} \n\n{1}", Strings.DATABASE_ACCESS_ERROR_MSG, ex.Message);
        }

        public static readonly string NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG
            = "Morate najpre da unesete kategorije i takmicenja.";

        public readonly static string GIMNASTICAR_JE_CLAN_DRUGE_EKIPE_ERROR_MSG
            = "Gimnasticar \'{0}\' je clan druge ekipe ({1}).";

    }
}
