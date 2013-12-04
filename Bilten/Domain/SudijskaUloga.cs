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
        D1_E1,
        D2,
        D2_E2,
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
        public static List<SudijskaUloga> getUloge(byte brojDSudija, bool hasD1_E1, bool hasD2_E2, byte brojESudija, 
            byte brojMeracaVremena, byte brojLinijskihSudija)
        {
            List<SudijskaUloga> result = new List<SudijskaUloga>();

            bool added_d1_e1 = false;
            bool added_d2_e2 = false;
            if (brojDSudija > 0)
            {
                if (hasD1_E1)
                {
                    result.Add(SudijskaUloga.D1_E1);
                    added_d1_e1 = true;
                }
                else
                    result.Add(SudijskaUloga.D1);
            }
            if (brojDSudija > 1)
            {
                if (hasD2_E2)
                {
                    result.Add(SudijskaUloga.D2_E2);
                    added_d2_e2 = true;
                }
                else
                    result.Add(SudijskaUloga.D2);
            }

            if (brojESudija > 0)
            {
                if (!hasD1_E1)
                    result.Add(SudijskaUloga.E1);
                else if (!added_d1_e1)
                    result.Add(SudijskaUloga.D1_E1);
            }
            if (brojESudija > 1)
            {
                if (!hasD2_E2)
                    result.Add(SudijskaUloga.E2);
                else if (!added_d2_e2)
                    result.Add(SudijskaUloga.D2_E2);
            }
            if (brojESudija > 2)
                result.Add(SudijskaUloga.E3);
            if (brojESudija > 3)
                result.Add(SudijskaUloga.E4);
            if (brojESudija > 4)
                result.Add(SudijskaUloga.E5);
            if (brojESudija > 5)
                result.Add(SudijskaUloga.E6);

            SudijskaUloga[] merVremena = meraciVremena();
            for (int i = 0; i < brojMeracaVremena; i++)
                result.Add(merVremena[i]);

            SudijskaUloga[] linSudije = linijskeSudije();
            for (int i = 0; i < brojLinijskihSudija; i++)
                result.Add(linSudije[i]);

            return result;
        }

        public static SudijskaUloga[] getSveUloge()
        {
            return new SudijskaUloga[] { 
                SudijskaUloga.D1, 
                SudijskaUloga.D2, 
                SudijskaUloga.D1_E1, 
                SudijskaUloga.D2_E2,
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

                case SudijskaUloga.D1_E1:
                    return "D1-E1";

                case SudijskaUloga.D2_E2:
                    return "D2-E2";

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
