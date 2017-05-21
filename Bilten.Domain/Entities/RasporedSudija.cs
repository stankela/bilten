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

        private string naziv;
        public virtual string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        private Takmicenje takmicenje;
        public virtual Takmicenje Takmicenje
        {
            get { return takmicenje; }
            set { takmicenje = value; }
        }

        // TODO4: Fali Equals i GetHashCode za SudijskiOdborNaSpravi

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

            this.Naziv = RasporedNastupa.kreirajNaziv(kategorije);
            this.deoTakKod = deoTakKod;
            this.takmicenje = kategorije[0].Takmicenje;

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
            strBuilder.AppendLine(Naziv != null ? Naziv : NULL);
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);

            strBuilder.AppendLine(Odbori.Count.ToString());
            foreach (SudijskiOdborNaSpravi s in Odbori)
                s.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());

            string line = reader.ReadLine();
            Naziv = line != NULL ? line : null;

            line = reader.ReadLine();
            Takmicenje = line != NULL ? map.takmicenjeMap[int.Parse(line)] : null;
            
            int count = int.Parse(reader.ReadLine());
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
