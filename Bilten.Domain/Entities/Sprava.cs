using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Bilten.Domain
{
    public enum Sprava : byte
    {
        Undefined = 0,
        Parter,
        Konj,
        Karike,
        Preskok,
        Razboj,
        Vratilo,
        DvovisinskiRazboj,
        Greda,
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
            List<Sprava> sprave = new List<Sprava>(MuskeSprave);
            if (gim == Gimnastika.ZSG)
                sprave = new List<Sprava>(ZenskeSprave);
            return sprave.IndexOf(sprava);
        }
    }
}
