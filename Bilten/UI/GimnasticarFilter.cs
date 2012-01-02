using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.UI
{
    public class GimnasticarFilter
    {
        private RegistarskiBroj regBroj;
        public RegistarskiBroj RegBroj
        {
            get { return regBroj; }
            set { regBroj = value; }
        }

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

        private Drzava drzava;
        public Drzava Drzava
        {
            get { return drzava; }
            set { drzava = value; }
        }

        private KategorijaGimnasticara kategorija;
        public KategorijaGimnasticara Kategorija
        {
            get { return kategorija; }
            set { kategorija = value; }
        }

        private Klub klub;
        public Klub Klub
        {
            get { return klub; }
            set { klub = value; }
        }

        public GimnasticarFilter()
        { 
        
        }
    }
}
