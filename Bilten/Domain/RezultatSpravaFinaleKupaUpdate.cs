using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatSpravaFinaleKupaUpdate
    {
        private int rezTakmicenjeId;
        public virtual int RezultatskoTakmicenjeId
        {
            get { return rezTakmicenjeId; }
            set { rezTakmicenjeId = value; }
        }

        private int gimnasticarId;
        public virtual int GimnasticarId
        {
            get { return gimnasticarId; }
            set { gimnasticarId = value; }
        }

        private Sprava sprava;
        public virtual Sprava Sprava
        {
            get { return sprava; }
            set { sprava = value; }
        }

        private KvalifikacioniStatus kvalStatus;
        public virtual KvalifikacioniStatus KvalStatus
        {
            get { return kvalStatus; }
            set { kvalStatus = value; }
        }

    }
}
