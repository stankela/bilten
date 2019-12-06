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

        private string rezerve;
        public virtual string Rezerve
        {
            get { return rezerve; }
            set { rezerve = value; }
        }
    }
}
