using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace Bilten.Domain
{
    public class RasporedSudija : DomainObject
    {
        public virtual Pol Pol
        {
            get
            {
                foreach (TakmicarskaKategorija k in Kategorije)
                {
                    return k.Pol;
                }
                return Pol.Undefined;
            }
        }

        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            protected set { deoTakKod = value; }
        }

        private Iesi.Collections.Generic.ISet<TakmicarskaKategorija> kategorije = new HashedSet<TakmicarskaKategorija>();
        public virtual Iesi.Collections.Generic.ISet<TakmicarskaKategorija> Kategorije
        {
            get { return kategorije; }
            protected set { kategorije = value; }
        }

        public virtual void addKategorija(TakmicarskaKategorija kat)
        {
            Kategorije.Add(kat);
        }

        public virtual void removeKategorija(TakmicarskaKategorija kat)
        {
            Kategorije.Remove(kat);
        }

        private Iesi.Collections.Generic.ISet<SudijskiOdborNaSpravi> odbori = new HashedSet<SudijskiOdborNaSpravi>();
        public virtual Iesi.Collections.Generic.ISet<SudijskiOdborNaSpravi> Odbori
        {
            get { return odbori; }
            set { odbori = value; }
        }

        public RasporedSudija()
        { 
        
        }

        public RasporedSudija(IList<TakmicarskaKategorija> kategorije, 
            DeoTakmicenjaKod deoTakKod, Takmicenje takmicenje)
        {
            if (kategorije.Count == 0)
                throw new ArgumentException("Kategorije ne smeju da budu prazne.");

            foreach (TakmicarskaKategorija kat in kategorije)
                addKategorija(kat);
            this.deoTakKod = deoTakKod;

            Sprava[] sprave = Sprave.getSprave(kategorije[0].Pol);
            foreach (Sprava s in sprave)
                odbori.Add(new SudijskiOdborNaSpravi(s));
        }

        public virtual SudijskiOdborNaSpravi getOdbor(Sprava sprava)
        {
            foreach (SudijskiOdborNaSpravi o in Odbori)
            {
                if (o.Sprava == sprava)
                    return o;
            }
            return null;
        }
    }
}
