using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.IO;
using System.ComponentModel;
using Bilten.Util;

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

        // TODO4: Proveri da li je moguce prikazati poruku kada se pokusa sa otvaranjem biltena a druga instanca je
        // istovremeno otvorena.

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

        // TODO4: Ukloni ovo, deo iz *.hdm fajla i tabelu raspored_nastupa_kategorija nakon sto izvrsis apdejt na verziju
        // 5. Isto i u klasi RasporedSudija (tamo je tabela raspored_sudija_kategorija)
        private Iesi.Collections.Generic.ISet<TakmicarskaKategorija> kategorije = new HashedSet<TakmicarskaKategorija>();
        public virtual Iesi.Collections.Generic.ISet<TakmicarskaKategorija> Kategorije
        {
            get { return kategorije; }
            protected set { kategorije = value; }
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
            DeoTakmicenjaKod deoTakKod, Gimnastika gimnastika)
        {
            init(kategorije, deoTakKod, gimnastika);
        }

        public RasporedNastupa(TakmicarskaKategorija kategorija,
            DeoTakmicenjaKod deoTakKod, Gimnastika gimnastika)
        {
            IList<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>();
            kategorije.Add(kategorija);
            init(kategorije, deoTakKod, gimnastika);
        }

        private void init(IList<TakmicarskaKategorija> kategorije, DeoTakmicenjaKod deoTakKod, Gimnastika gimnastika)
        {
            if (kategorije.Count == 0)
                throw new ArgumentException("Kategorije ne smeju da budu prazne.");

            this.Naziv = kreirajNaziv(kategorije);
            this.deoTakKod = deoTakKod;
            this.takmicenje = kategorije[0].Takmicenje;

            addNewGrupa(gimnastika);
        }

        public static string kreirajNaziv(IList<TakmicarskaKategorija> katList)
        {
            List<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>(katList);
            if (kategorije.Count == 0)
                return String.Empty;

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            kategorije.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

            string result = kategorije[0].ToString();
            for (int i = 1; i < kategorije.Count; i++)
                result = result + ", " + kategorije[i].ToString();
            return result;
        }

        public virtual void addNewGrupa(Gimnastika gimnastika)
        {
            if (canAddNewGrupa())
            {
                int brojRotacija = 4;
                if (gimnastika == Gimnastika.MSG)
                    brojRotacija = 6;
                int grupa = getBrojGrupa() + 1;

                Sprava[] sprave = Sprave.getSprave(gimnastika);

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
            strBuilder.AppendLine(Naziv != null ? Naziv : NULL);
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);

            strBuilder.AppendLine(StartListe.Count.ToString());
            foreach (StartListaNaSpravi s in StartListe)
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
                StartListaNaSpravi s = new StartListaNaSpravi();
                s.loadFromDump(reader, map);
                StartListe.Add(s);
            }
        }
    }
}
