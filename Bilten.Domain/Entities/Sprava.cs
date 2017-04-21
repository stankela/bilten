using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Bilten.Domain
{
    public enum Sprava : byte
    {
        // NOTE: Vrednosti se ne smeju menjati jer se koriste za maske za sprave kod racunanja ekipnog poretka.
        Undefined = 0,
        Parter = 1,
        Konj = 2,
        Karike = 3,
        Preskok = 4,
        Razboj = 5,
        Vratilo = 6,
        DvovisinskiRazboj = 7,
        Greda = 8,
        Max = Greda
    }

    public static class Sprave
    {
        public static Sprava[] getSprave(Gimnastika gimnastika)
        {
            if (gimnastika == Gimnastika.MSG)
                return MuskeSprave;
            else if (gimnastika == Gimnastika.ZSG)
                return ZenskeSprave;
            else
                return null;
        }

        private static Sprava[] MuskeSprave
        {
            get
            {
                return new Sprava[] { Sprava.Parter, Sprava.Konj, Sprava.Karike,
                    Sprava.Preskok, Sprava.Razboj, Sprava.Vratilo };
            }
        }

        private static Sprava[] ZenskeSprave
        {
            get
            {
                return new Sprava[] { Sprava.Preskok, Sprava.DvovisinskiRazboj,
                    Sprava.Greda, Sprava.Parter };
            }
        }

        public static string[] getSpraveNazivi(Gimnastika gimnastika)
        {
            if (gimnastika == Gimnastika.MSG)
                return MuskeSpraveNazivi;
            else if (gimnastika == Gimnastika.ZSG)
                return ZenskeSpraveNazivi;
            else
                return null;
        }

        private static string[] MuskeSpraveNazivi
        {
            get
            {
                Sprava[] sprave = Sprave.MuskeSprave;
                string[] result = new string[sprave.Length];
                for (int i = 0; i < sprave.Length; i++)
                    result[i] = Sprave.toString(sprave[i]);
                return result;
            }
        }

        private static string[] ZenskeSpraveNazivi
        {
            get
            {
                Sprava[] sprave = Sprave.ZenskeSprave;
                string[] result = new string[sprave.Length];
                for (int i = 0; i < sprave.Length; i++)
                    result[i] = Sprave.toString(sprave[i]);
                return result;
            }
        }

        public static string toString(Sprava sprava)
        {
            switch (sprava)
            {
                case Sprava.Parter:
                    return "Parter";

                case Sprava.Konj:
                    return "Konj sa hvataljkama";

                case Sprava.Karike:
                    return "Karike";

                case Sprava.Preskok:
                    return "Preskok";

                case Sprava.Razboj:
                    return "Razboj";

                case Sprava.Vratilo:
                    return "Vratilo";

                case Sprava.DvovisinskiRazboj:
                    return "Dvovisinski razboj";

                case Sprava.Greda:
                    return "Greda";

                case Sprava.Undefined:
                    return "Nepoznata sprava";

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za spravu.");
            }
        }

        public static Sprava parse(string sprava)
        {
            Sprava result = Sprava.Undefined;
            Sprava[] sprave = (Sprava[])Enum.GetValues(typeof(Sprava));
            foreach (Sprava s in sprave)
            {
                if (toString(s).ToUpper() == sprava.ToUpper())
                {
                    result = s;
                    break;
                }
            }
            if (result == Sprava.Undefined && sprava.ToUpper() == "KONJ")
                result = Sprava.Konj;
            if (result == Sprava.Undefined && sprava.ToUpper() == "DVOVISINSKI RAZBOJ")
                result = Sprava.DvovisinskiRazboj;
            return result;
        }

        public static int indexOf(Sprava sprava, Gimnastika gim)
        {
            if (gim == Gimnastika.MSG)
            {
                switch (sprava)
                {
                    case Sprava.Parter:
                        return 0;

                    case Sprava.Konj:
                        return 1;

                    case Sprava.Karike:
                        return 2;

                    case Sprava.Preskok:
                        return 3;

                    case Sprava.Razboj:
                        return 4;

                    case Sprava.Vratilo:
                        return 5;

                    default:
                        throw new ArgumentException("Nedozvoljena vrednost za spravu.");
                }
            }
            else
            {
                switch (sprava)
                {
                    case Sprava.Preskok:
                        return 0;

                    case Sprava.DvovisinskiRazboj:
                        return 1;

                    case Sprava.Greda:
                        return 2;

                    case Sprava.Parter:
                        return 3;

                    default:
                        throw new ArgumentException("Nedozvoljena vrednost za spravu.");
                }
            }
        }

        public static bool hasAllSprave(IList<Sprava> sprave, Gimnastika gimnastika)
        {
            ISet<Sprava> spraveSet = new HashSet<Sprava>(sprave);
            foreach (Sprava s in getSprave(gimnastika))
            {
                if (!spraveSet.Contains(s))
                    return false;
            }
            return true;
        }
    }
}
