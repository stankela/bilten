using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public enum DeoTakmicenjaKod : byte
    {
        Undefined = 0,
        Takmicenje1,
        Takmicenje2,
        Takmicenje3,
        Takmicenje4
    }

    public static class DeoTakmicenjaKodovi
    {
        public static string toString(DeoTakmicenjaKod kod)
        {
            switch (kod)
            {
                case DeoTakmicenjaKod.Takmicenje1:
                    return "Takmicenje I";

                case DeoTakmicenjaKod.Takmicenje2:
                    return "Takmicenje II";

                case DeoTakmicenjaKod.Takmicenje3:
                    return "Takmicenje III";

                case DeoTakmicenjaKod.Takmicenje4:
                    return "Takmicenje IV";

                case DeoTakmicenjaKod.Undefined:
                    return "Nepoznat deo takmicenja";

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za deo takmicenja.");
            }
        }

        public static DeoTakmicenjaKod parse(string deoTakmicenjaKod)
        {
            DeoTakmicenjaKod result = DeoTakmicenjaKod.Undefined;
            DeoTakmicenjaKod[] kodovi =
                (DeoTakmicenjaKod[])Enum.GetValues(typeof(DeoTakmicenjaKod));
            foreach (DeoTakmicenjaKod k in kodovi)
            {
                if (toString(k).ToUpper() == deoTakmicenjaKod.ToUpper())
                {
                    result = k;
                    break;
                }
            }
            return result;
        }

    }
}
