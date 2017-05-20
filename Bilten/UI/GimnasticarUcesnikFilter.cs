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

        public bool isEmpty()
        {
            bool result = String.IsNullOrEmpty(ime) && String.IsNullOrEmpty(prezime)
                && drzava == null && kategorija == null && klub == null;
            return result;
        }
    }
}
