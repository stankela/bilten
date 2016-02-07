using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace Bilten.Domain
{
    public class RasporedNastupa : DomainObject
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

        // TODO4: U 4.5 je u System.Collections.Generic uveden ISet, tako da sam morao eksplicitno da kvalifikujem
        // ISet sa Iesi.Collections.Generic.ISet posto NHibernate trenutno ne radi sa System.Collections.Generic.ISet.
        // Proveri da li se nesto promenilo, tj. da li je NHibernate poceo da podrzava Iesi.Collections.Generic.ISet.

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

        private Iesi.Collections.Generic.ISet<StartListaNaSpravi> startListe = new HashedSet<StartListaNaSpravi>();
        public virtual Iesi.Collections.Generic.ISet<StartListaNaSpravi> StartListe
        {
            get { return startListe; }
            set { startListe = value; }
        }

        public RasporedNastupa()
        { 
        
        }

        public RasporedNastupa(IList<TakmicarskaKategorija> kategorije, 
            DeoTakmicenjaKod deoTakKod)
        {
            if (kategorije.Count == 0)
                throw new ArgumentException("Kategorije ne smeju da budu prazne.");

            foreach (TakmicarskaKategorija kat in kategorije)
                addKategorija(kat);
            this.deoTakKod = deoTakKod;

            addNewGrupa();
        }

        public virtual void addNewGrupa()
        {
            if (canAddNewGrupa())
            {
                int brojRotacija = 4;
                if (Pol == Pol.Muski)
                    brojRotacija = 6;
                int grupa = getBrojGrupa() + 1;

                Sprava[] sprave = Sprave.getSprave(Pol);

                for (int i = 1; i <= brojRotacija; i++)
                {
                    foreach (Sprava s in sprave)
                        StartListe.Add(new StartListaNaSpravi(s, (byte)grupa, (byte)i, NacinRotacije.NeRotirajNista));
                }
            }
        }

        public virtual int getBrojGrupa()
        { 
            int result = 0;
            foreach (StartListaNaSpravi s in StartListe)
            {
                if (s.Grupa > result)
                    result = s.Grupa;
            }
            return result;
        }

        public virtual bool canAddNewGrupa()
        {
            int brojGrupa = getBrojGrupa();
            if (brojGrupa == 0)
                return true;
            else
                // dozvoljeno je dodati novu grupu jedino ako grupa sa najvecim
                // indeksom nije prazna
                return !isEmptyGrupa(brojGrupa);
        }

        private bool isEmptyGrupa(int g)
        {
            foreach (StartListaNaSpravi s in StartListe)
            {
                if (s.Grupa == g && !s.empty())
                    return false;
            }
            return true;
        }

        public virtual StartListaNaSpravi getStartLista(Sprava sprava, int grupa, int rot)
        {
            foreach (StartListaNaSpravi s in StartListe)
            {
                if (s.Sprava == sprava && s.Grupa == grupa && s.Rotacija == rot)
                    return s;
            }
            return null;
        }

        public virtual IList<StartListaNaSpravi> getStartListe(int g, int r)
        {
            IList<StartListaNaSpravi> result = new List<StartListaNaSpravi>();
            foreach (StartListaNaSpravi s in StartListe)
            {
                if (s.Grupa == g && s.Rotacija == r)
                    result.Add(s);
            }
            return result;
        }
    }
}
