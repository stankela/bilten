using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public enum SudijskaUloga : byte
    {
        Undefined = 0,
        PredsednikGlavnogSudijskogOdbora,
        ClanGlavnogSudijskogOdbora,
        ParterKontrolor,
        KonjKontrolor,
        KarikeKontrolor,
        PreskokKontrolor,
        RazbojKontrolor,
        VratiloKontrolor,
        DvovisinskiRazbojKontrolor,
        GredaKontrolor,
        D1,	
        D2,
        E1,
        E2,
        E3,
        E4,
        E5,
        E6,
        MeracVremena1,
        MeracVremena2,
        LinijskiSudija1,
        LinijskiSudija2,
        LinijskiSudija3,
        LinijskiSudija4,
        Max = LinijskiSudija4

        /*
        Undefined = 0,
        SuperiorJuryPresident,
        SuperiorJuryMember,
        FloorSupervisor,
        PommelHorseSupervisor,
        RingsSupervisor,
        VaultSupervisor,
        ParallelBarsSupervisor,
        HighBarSupervisor,
        UnevenBarsSupervisor,
        BeamSupervisor,
        D1,	
        D2,
        E1,
        E2,
        E3,
        E4,
        E5,
        E6,
        Timer1,
        Timer2,
        Line1,
        Line2,
        Line3,
        Line4
        */
    }

    public static class SudijskeUloge
    {
        public static SudijskaUloga[] getUloge(byte brojDSudija, byte brojESudija, 
            byte brojMeracaVremena, byte brojLinijskihSudija)
        {
            SudijskaUloga[] result = new SudijskaUloga[brojDSudija + brojESudija +
                brojMeracaVremena + brojLinijskihSudija];
       
            if (brojDSudija > 0)
                result[0] = SudijskaUloga.D1;
            if (brojDSudija > 1)
                result[1] = SudijskaUloga.D2;

            SudijskaUloga[] eSud = eSudije();
            for (int i = 0; i < brojESudija; i++)
                result[brojDSudija + i] = eSud[i];

            SudijskaUloga[] merVremena = meraciVremena();
            int merVremenaOffset = brojDSudija + brojESudija;
            for (int i = 0; i < brojMeracaVremena; i++)
                result[merVremenaOffset + i] = merVremena[i];

            SudijskaUloga[] linSudije = linijskeSudije();
            int linSudijeOffset = brojDSudija + brojESudija + brojMeracaVremena;
            for (int i = 0; i < brojLinijskihSudija; i++)
                result[linSudijeOffset + i] = linSudije[i];

            return result;
        }

        public static SudijskaUloga[] dSudije()
        {
            return new SudijskaUloga[]
            {
                SudijskaUloga.D1,
                SudijskaUloga.D2
            };
        }

        public static SudijskaUloga[] eSudije()
        {
            return new SudijskaUloga[]
            {
                SudijskaUloga.E1,
                SudijskaUloga.E2,
                SudijskaUloga.E3,
                SudijskaUloga.E4,
                SudijskaUloga.E5,
                SudijskaUloga.E6
            };
        }

        public static SudijskaUloga[] meraciVremena()
        {
            return new SudijskaUloga[]
            {
                SudijskaUloga.MeracVremena1,
                SudijskaUloga.MeracVremena2
            };
        }

        public static SudijskaUloga[] linijskeSudije()
        {
            return new SudijskaUloga[]
            {
                SudijskaUloga.LinijskiSudija1,
                SudijskaUloga.LinijskiSudija2,
                SudijskaUloga.LinijskiSudija3,
                SudijskaUloga.LinijskiSudija4
            };
        }

        public static SudijskaUloga[] ulogeNaSpravi()
        {
            return new SudijskaUloga[]
            {
                SudijskaUloga.D1,	
                SudijskaUloga.D2,
                SudijskaUloga.E1,
                SudijskaUloga.E2,
                SudijskaUloga.E3,
                SudijskaUloga.E4,
                SudijskaUloga.E5,
                SudijskaUloga.E6,
                SudijskaUloga.MeracVremena1,
                SudijskaUloga.MeracVremena2,
                SudijskaUloga.LinijskiSudija1,
                SudijskaUloga.LinijskiSudija2,
                SudijskaUloga.LinijskiSudija3,
                SudijskaUloga.LinijskiSudija4
            };
        }

        public static bool isUlogaNaSpravi(SudijskaUloga uloga)
        {
            foreach (SudijskaUloga u in ulogeNaSpravi())
            {
                if (u == uloga)
                    return true;
            }
            return false;
        }

        public static bool isDSudija(SudijskaUloga uloga)
        {
            foreach (SudijskaUloga u in dSudije())
            {
                if (u == uloga)
                    return true;
            }
            return false;
        }

        public static bool isESudija(SudijskaUloga uloga)
        {
            foreach (SudijskaUloga u in eSudije())
            {
                if (u == uloga)
                    return true;
            }
            return false;
        }

        public static bool isMeracVremena(SudijskaUloga uloga)
        {
            foreach (SudijskaUloga u in meraciVremena())
            {
                if (u == uloga)
                    return true;
            }
            return false;
        }

        public static bool isLinijskiSudija(SudijskaUloga uloga)
        {
            foreach (SudijskaUloga u in linijskeSudije())
            {
                if (u == uloga)
                    return true;
            }
            return false;
        }

        public static string toString(SudijskaUloga uloga)
        {
            switch (uloga)
            {
                case SudijskaUloga.PredsednikGlavnogSudijskogOdbora:
                    return "Predsednik glavnog sudijskog odbora";

                case SudijskaUloga.ClanGlavnogSudijskogOdbora:
                    return "Clan glavnog sudijskog odbora";

                case SudijskaUloga.ParterKontrolor:
                    return "Kontrolor na parteru";

                case SudijskaUloga.KonjKontrolor:
                    return "Kontrolor na konju sa hvataljkama";

                case SudijskaUloga.KarikeKontrolor:
                    return "Kontrolor na karikama";

                case SudijskaUloga.PreskokKontrolor:
                    return "Kontrolor na preskoku";

                case SudijskaUloga.RazbojKontrolor:
                    return "Kontrolor na razboju";

                case SudijskaUloga.VratiloKontrolor:
                    return "Kontrolor na vratilu";

                case SudijskaUloga.DvovisinskiRazbojKontrolor:
                    return "Kontrolor na dvovisinskom razboju";

                case SudijskaUloga.GredaKontrolor:
                    return "Kontrolor na gredi";

                case SudijskaUloga.D1:
                    return "D1";

                case SudijskaUloga.D2:
                    return "D2";

                case SudijskaUloga.E1:
                    return "E1";

                case SudijskaUloga.E2:
                    return "E2";

                case SudijskaUloga.E3:
                    return "E3";

                case SudijskaUloga.E4:
                    return "E4";

                case SudijskaUloga.E5:
                    return "E5";

                case SudijskaUloga.E6:
                    return "E6";

                case SudijskaUloga.MeracVremena1:
                    return "Merac vremena 1";

                case SudijskaUloga.MeracVremena2:
                    return "Merac vremena 2";

                case SudijskaUloga.LinijskiSudija1:
                    return "Linijski sudija 1";

                case SudijskaUloga.LinijskiSudija2:
                    return "Linijski sudija 2";

                case SudijskaUloga.LinijskiSudija3:
                    return "Linijski sudija 3";

                case SudijskaUloga.LinijskiSudija4:
                    return "Linijski sudija 4";

                case SudijskaUloga.Undefined:
                    return "Nepoznata sudijska funkcija";

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za sudijsku funkciju.");
            }
        }
    }
}
