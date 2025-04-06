using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public enum Gimnastika : byte
    {
        Undefined = 0, MSG, ZSG
    }

    public static class GimnastikaUtil
    {
        public static string getGimnastikaStr(Gimnastika gimnastika, string jezik)
        {
            if (jezik == "Srpski")
            {
                return gimnastika == Gimnastika.MSG ? "MSG" : "ZSG";
            }
            else
            {
                return gimnastika == Gimnastika.MSG ? "MAG" : "WAG";
            }
        }
    }

}
