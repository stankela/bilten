using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public enum NacinRotacije : byte
    {
        NeRotirajNista = 0,
        RotirajEkipeRotirajGimnasticare,
        NeRotirajEkipeRotirajGimnasticare,
        RotirajSve
    }

    public static class NaciniRotacije
    {
        public static string toString(NacinRotacije nacin)
        {
            switch (nacin)
            { 
                case NacinRotacije.NeRotirajNista:
                    return "Ne rotiraj nista";

                case NacinRotacije.RotirajEkipeRotirajGimnasticare:
                    return "Rotiraj ekipe, rotiraj gimnasticare unutar ekipe";

                case NacinRotacije.NeRotirajEkipeRotirajGimnasticare:
                    return "Ne rotiraj ekipe, rotiraj gimnasticare unutar ekipe";

                case NacinRotacije.RotirajSve:
                    return "Rotiraj sve gimnasticare nezavisno od ekipe";

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za nacin rotacije.");
            }
        }
    }
}
