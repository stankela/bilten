using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class DrugaOcena : OcenaBase
    {
        public DrugaOcena()
        {

        }

        public override void loadFromDump(StringReader reader)
        {
            base.loadFromDump(reader);
        }
    }
}
