using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Domain
{
    public class Jezik : DomainObject
    {
        public static char shMalo = '\u0161';
        public static char chMalo = '\u010d';
        public static char shVeliko = '\u0160';

        private string naziv;
        public virtual string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        private bool default_;
        public virtual bool Default
        {
            get { return default_; }
            set { default_ = value; }
        }

        private string redBroj;
        public virtual string RedBroj
        {
            get { return redBroj; }
            set { redBroj = value; }
        }

        private string broj;
        public virtual string Broj
        {
            get { return broj; }
            set { broj = value; }
        }

        private string rank;
        public virtual string Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        private string ime;
        public virtual string Ime
        {
            get { return ime; }
            set { ime = value; }
        }

        private string prezime;
        public virtual string Prezime
        {
            get { return prezime; }
            set { prezime = value; }
        }

        private string datumRodjenja;
        public virtual string DatumRodjenja
        {
            get { return datumRodjenja; }
            set { datumRodjenja = value; }
        }

        private string klub;
        public virtual string Klub
        {
            get { return klub; }
            set { klub = value; }
        }

        private string drzava;
        public virtual string Drzava
        {
            get { return drzava; }
            set { drzava = value; }
        }

        private string klubDrzava;
        public virtual string KlubDrzava
        {
            get { return klubDrzava; }
            set { klubDrzava = value; }
        }

        private string kategorija;
        public virtual string Kategorija
        {
            get { return kategorija; }
            set { kategorija = value; }
        }

        private string ukupno;
        public virtual string Ukupno
        {
            get { return ukupno; }
            set { ukupno = value; }
        }

        private string ocena;
        public virtual string Ocena
        {
            get { return ocena; }
            set { ocena = value; }
        }

        private string ekipa;
        public virtual string Ekipa
        {
            get { return ekipa; }
            set { ekipa = value; }
        }

        private string rezerve;
        public virtual string Rezerve
        {
            get { return rezerve; }
            set { rezerve = value; }
        }

        private string strana;
        public virtual string Strana
        {
            get { return strana; }
            set { strana = value; }
        }
    }
}
