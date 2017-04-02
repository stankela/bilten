using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public enum SudijskaUloga : byte
    {
        Undefined = 0,

        // NOTE: Ako se dodaju nove konstante, treba ih dodavati na kraj (iza E6) da se ne bi promenile vrednosti
        // postojecih konstanti, zato sto se vrednosti konstanti unose u bazu podataka, u kolonu uloga tabele
        // sudija_na_spravi
        // TODO: Mozda da promenim ovo, da baza podataka ne zavisi od vrednosti enuma.
        D1,
        D1_E1,
        D2,
        D2_E2,
        E1,
        E2,
        E3,
        E4,
        E5,
        E6



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
        public static List<SudijskaUloga> getUloge(byte brojDSudija, bool hasD1_E1, bool hasD2_E2, byte brojESudija)
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

        public static string toString(SudijskaUloga uloga)
        {
            switch (uloga)
            {
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

                case SudijskaUloga.Undefined:
                    return "Nepoznata sudijska funkcija";

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za sudijsku funkciju.");
            }
        }
    }
}
