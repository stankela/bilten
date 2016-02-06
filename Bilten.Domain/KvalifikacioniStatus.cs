using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public enum KvalifikacioniStatus : byte
    {
        None = 0, Q, R
    }

    public static class KvalifikacioniStatusi
    {
        public static string toString(KvalifikacioniStatus status)
        {
            switch (status)
            {
                case KvalifikacioniStatus.None:
                    return String.Empty;

                case KvalifikacioniStatus.Q:
                    return "Q";

                case KvalifikacioniStatus.R:
                    return "R";

                default:
                    throw new ArgumentException("Nedozvoljena vrednost za kvalifikacioni status.");
            }
        }
    }

}
