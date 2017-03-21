using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Domain
{
    public class KonacanPlasman : DomainObject
    {
        private Nullable<short> viseboj;
        public virtual Nullable<short> Viseboj
        {
            get { return viseboj; }
            set { viseboj = value; }
        }

        private Nullable<short> parter;
        public virtual Nullable<short> Parter
        {
            get { return parter; }
            set { parter = value; }
        }

        private Nullable<short> konj;
        public virtual Nullable<short> Konj
        {
            get { return konj; }
            set { konj = value; }
        }

        private Nullable<short> karike;
        public virtual Nullable<short> Karike
        {
            get { return karike; }
            set { karike = value; }
        }

        private Nullable<short> preskok;
        public virtual Nullable<short> Preskok
        {
            get { return preskok; }
            set { preskok = value; }
        }

        private Nullable<short> razboj;
        public virtual Nullable<short> Razboj
        {
            get { return razboj; }
            set { razboj = value; }
        }

        private Nullable<short> vratilo;
        public virtual Nullable<short> Vratilo
        {
            get { return vratilo; }
            set { vratilo = value; }
        }

        private Nullable<short> greda;
        public virtual Nullable<short> Greda
        {
            get { return greda; }
            set { greda = value; }
        }

        private Nullable<short> dvovisinskiRazboj;
        public virtual Nullable<short> DvovisinskiRazboj
        {
            get { return dvovisinskiRazboj; }
            set { dvovisinskiRazboj = value; }
        }

        private Nullable<short> ekipno;
        public virtual Nullable<short> Ekipno
        {
            get { return ekipno; }
            set { ekipno = value; }
        }

        private RezultatskoTakmicenje rezTakmicenje;
        public virtual RezultatskoTakmicenje RezultatskoTakmicenje
        {
            get { return rezTakmicenje; }
            set { rezTakmicenje = value; }
        }
    }
}
