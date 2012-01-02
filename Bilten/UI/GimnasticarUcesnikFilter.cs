using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.UI
{
    public class GimnasticarUcesnikFilter
    {
        private string ime;
        public string Ime
        {
            get { return ime; }
            set { ime = value; }
        }

        private string prezime;
        public string Prezime
        {
            get { return prezime; }
            set { prezime = value; }
        }

        private Nullable<int> godRodj;
        public Nullable<int> GodRodj
        {
            get { return godRodj; }
            set { godRodj = value; }
        }

        private Nullable<Gimnastika> gimnastika;
        public Nullable<Gimnastika> Gimnastika
        {
            get { return gimnastika; }
            set { gimnastika = value; }
        }

        private DrzavaUcesnik drzava;
        public DrzavaUcesnik Drzava
        {
            get { return drzava; }
            set { drzava = value; }
        }

        private TakmicarskaKategorija kategorija;
        public TakmicarskaKategorija Kategorija
        {
            get { return kategorija; }
            set { kategorija = value; }
        }

        private KlubUcesnik klub;
        public KlubUcesnik Klub
        {
            get { return klub; }
            set { klub = value; }
        }

        public GimnasticarUcesnikFilter()
        {

        }
    }
}
