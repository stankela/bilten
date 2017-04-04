using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.IO;

namespace Bilten.Domain
{
    public class RasporedNastupa : DomainObject
    {
        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            protected set { deoTakKod = value; }
        }

        // TODO4: U 4.5 je u System.Collections.Generic uveden ISet, tako da sam morao eksplicitno da kvalifikujem
        // ISet sa Iesi.Collections.Generic.ISet posto NHibernate trenutno ne radi sa System.Collections.Generic.ISet.
        // Proveri da li se nesto promenilo, tj. da li je NHibernate poceo da podrzava Iesi.Collections.Generic.ISet.

        // TODO4: Izbaci Kategorije iz rasporeda nastupa

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

        public virtual Gimnastika Gimnastika
        {
            get
            {
                foreach (TakmicarskaKategorija k in Kategorije)
                {
                    return k.Gimnastika;
                }
                return Gimnastika.Undefined;
            }
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
                if (Gimnastika == Gimnastika.MSG)
                    brojRotacija = 6;
                int grupa = getBrojGrupa() + 1;

                Sprava[] sprave = Sprave.getSprave(Gimnastika);

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

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());

            strBuilder.AppendLine(Kategorije.Count.ToString());
            foreach (TakmicarskaKategorija k in Kategorije)
                strBuilder.AppendLine(k.Id.ToString());

            strBuilder.AppendLine(StartListe.Count.ToString());
            foreach (StartListaNaSpravi s in StartListe)
                s.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());

            int count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
                Kategorije.Add(map.kategorijeMap[int.Parse(reader.ReadLine())]);

            count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                reader.ReadLine();  // id
                StartListaNaSpravi s = new StartListaNaSpravi();
                s.loadFromDump(reader, map);
                StartListe.Add(s);
            }
        }
    }
}
