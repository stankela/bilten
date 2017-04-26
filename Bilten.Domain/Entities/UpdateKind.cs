using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Domain
{
    public enum UpdateKind
    {
        Undefined = 0,
        PoredakUkupnoTak1 = 1,
        PoredakSpravaTak1,
        PoredakPreskokTak1,
        PoredakPreskokTak3,
        PoredakEkipeTak1,
        PoredakUkupnoFinaleKupa,
        PoredakUkupnoZbirViseKola,
        PoredakSpravaFinaleKupa,
        PoredakEkipnoFinaleKupa,
        PoredakEkipnoZbirViseKola,
        RezTak
    }

    public class UpdateKindUtil
    {
        public static bool shouldUpdate(UpdateKind kind, int updateMask)
        {
            return (updateMask & (1 << (int)kind)) != 0;
        }

        public static void setUpdateKind(UpdateKind kind, ref int updateMask)
        {
            updateMask |= (1 << (int)kind);
        }
    }
}
