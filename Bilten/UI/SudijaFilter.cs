using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.UI
{
    public class SudijaFilter
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

        private Pol? pol;
        public Pol? Pol
        {
            get { return pol; }
            set { pol = value; }
        }

        private Drzava drzava;
        public Drzava Drzava
        {
            get { return drzava; }
            set { drzava = value; }
        }

        private Klub klub;
        public Klub Klub
        {
            get { return klub; }
            set { klub = value; }
        }

        public SudijaFilter()
        { 
        
        }

        public bool isEmpty()
        {
            return String.IsNullOrEmpty(ime) && String.IsNullOrEmpty(prezime) && pol == null && drzava == null
                && klub == null;
        }
    }
}
