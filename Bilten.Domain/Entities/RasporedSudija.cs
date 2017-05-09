using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.IO;

namespace Bilten.Domain
{
    public class RasporedSudija : DomainObject
    {
        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            protected set { deoTakKod = value; }
        }

        // TODO4: Izbrisi kategorije

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
            DeoTakmicenjaKod deoTakKod, Gimnastika gimnastika)
        {
            if (kategorije.Count == 0)
                throw new ArgumentException("Kategorije ne smeju da budu prazne.");

            foreach (TakmicarskaKategorija kat in kategorije)
                addKategorija(kat);
            this.deoTakKod = deoTakKod;

            Sprava[] sprave = Sprave.getSprave(gimnastika);
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

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());

            strBuilder.AppendLine(Kategorije.Count.ToString());
            foreach (TakmicarskaKategorija k in Kategorije)
                strBuilder.AppendLine(k.Id.ToString());

            strBuilder.AppendLine(Odbori.Count.ToString());
            foreach (SudijskiOdborNaSpravi s in Odbori)
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
                SudijskiOdborNaSpravi s = new SudijskiOdborNaSpravi();
                s.loadFromDump(reader, map);
                Odbori.Add(s);
            }
        }
    }
}
