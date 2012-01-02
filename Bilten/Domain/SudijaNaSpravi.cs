using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class SudijaNaSpravi : DomainObject
    {
        private SudijskaUloga uloga;
        public virtual SudijskaUloga Uloga
        {
            get { return uloga; }
            set { uloga = value; }
        }

        private SudijaUcesnik sudija;
        public virtual SudijaUcesnik Sudija
        {
            get { return sudija; }
            set { sudija = value; }
        }

        public virtual string PrezimeIme
        {
            get
            {
                if (Sudija != null)
                    return Sudija.PrezimeIme;
                else
                    return String.Empty;
            }
        }

        public virtual string Drzava
        {
            get
            {
                if (Sudija != null && Sudija.Drzava != null)
                    return Sudija.Drzava.Kod;
                else
                    return String.Empty;
            }
        }

        public SudijaNaSpravi()
        {
        
        }

        public SudijaNaSpravi(SudijskaUloga uloga, SudijaUcesnik sudija)
        {
            this.uloga = uloga;
            this.sudija = sudija;
        }

    }
}
