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

        // TODO: U 4.5 je u System.Collections.Generic uveden ISet, tako da sam morao eksplicitno da kvalifikujem
        // ISet sa Iesi.Collections.Generic.ISet posto NHibernate trenutno ne radi sa System.Collections.Generic.ISet.
        // Proveri da li se nesto promenilo, tj. da li je NHibernate poceo da podrzava System.Collections.Generic.ISet.

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

        private Iesi.Collections.Generic.ISet<StartListaNaSpravi> startListe = new HashedSet<StartListaNaSpravi>();
        public virtual Iesi.Collections.Generic.ISet<StartListaNaSpravi> StartListe
        {
            get { return startListe; }
            set { startListe = value; }
        }

        // 6 sprava + 6 pauza + zadnji bit koji se ne racuna. Jedinicom se oznacavaju samo pauze.
        private int pauzeMask = 0; // 0b0000000000000
        //private int pauzeMask = 4;   // 0b0000000000100 = Par, Pauza, Konj, Kar, Pre, Raz, Vra
        public virtual int PauzeMask
        {
            get { return pauzeMask; }
            set { pauzeMask = value; }
        }

        public static bool isPauzaSet(int pauzeMask, int pauzaIndex)
        {
            return ((1 << pauzaIndex) & pauzeMask) != 0;
        }

        public virtual bool isPauzaSet(int pauzaIndex)
        {
            return isPauzaSet(PauzeMask, pauzaIndex);
        }

        public virtual void setPauza(int pauzaIndex)
        {
            PauzeMask |= (1 << pauzaIndex);
        }

        public virtual void clearPauze()
        {
            PauzeMask = 0;
        }

        public virtual bool hasPauze()
        {
            return PauzeMask != 0;
        }

        public RasporedNastupa()
        { 
        
        }

        public RasporedNastupa(IList<TakmicarskaKategorija> kategorije, 
            DeoTakmicenjaKod deoTakKod, Gimnastika gimnastika, int pauzeMask)
        {
            init(kategorije, deoTakKod, gimnastika, pauzeMask);
        }

        public RasporedNastupa(TakmicarskaKategorija kategorija,
            DeoTakmicenjaKod deoTakKod, Gimnastika gimnastika, int pauzeMask)
        {
            IList<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>();
            kategorije.Add(kategorija);
            init(kategorije, deoTakKod, gimnastika, pauzeMask);
        }

        private void init(IList<TakmicarskaKategorija> kategorije, DeoTakmicenjaKod deoTakKod, Gimnastika gimnastika,
            int pauzeMask)
        {
            if (kategorije.Count == 0)
                throw new ArgumentException("Kategorije ne smeju da budu prazne.");

            this.Naziv = kreirajNaziv(kategorije);
            this.DeoTakmicenjaKod = deoTakKod;
            this.PauzeMask = pauzeMask;
            this.Takmicenje = kategorije[0].Takmicenje;

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
            Sprava[] sprave = Sprave.getSpraveIPauze(PauzeMask, gimnastika);
            if (canAddNewGrupa())
            {
                int brojRotacija = sprave.Length;
                int grupa = getBrojGrupa() + 1;

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

        public virtual StartListaNaSpravi getStartLista(GimnasticarUcesnik g, int grupa, int rot)
        {
            foreach (StartListaNaSpravi s in StartListe)
            {
                if (s.Grupa == grupa && s.Rotacija == rot && s.gimnasticarExists(g))
                    return s;
            }
            return null;
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

        private StartListaNaSpravi getStartListaPrethRot(StartListaNaSpravi s, List<List<Sprava>> aktivneSprave)
        {
            int i = aktivneSprave[s.Rotacija - 1].IndexOf(s.Sprava);
            List<Sprava> prethSprave = aktivneSprave[s.Rotacija - 2];
            Sprava prethSprava = (i == 0) ? prethSprave[prethSprave.Count - 1] : prethSprave[i - 1];
            StartListaNaSpravi result = getStartLista(prethSprava, s.Grupa, s.Rotacija - 1);
            return result;
        }

        public virtual void kreirajRotaciju(int grupa, int rot, List<List<Sprava>> aktivneSprave)
        {
            foreach (Sprava s in Sprave.getSpraveIPauze(PauzeMask, Takmicenje.Gimnastika))
            {
                StartListaNaSpravi startLista = getStartLista(s, grupa, rot);
                startLista.clear();

                if (aktivneSprave[rot - 1].IndexOf(s) == -1)
                    // Sprava nije aktivna u rotaciji.
                    continue;

                StartListaNaSpravi startListaPrethRot = getStartListaPrethRot(startLista, aktivneSprave);
                if (startListaPrethRot.Nastupi.Count == 0)
                    continue;
                if (Sprave.isPraznaSprava(startListaPrethRot.Sprava))
                {
                    // Ako je prethodna sprava pauza, samo prebacujem gimnasticare na novu spravu, bez obzira koji je
                    // nacin rotacije. Rotacija je izvrsena kada sam prebacivao sa sprave na pauzu.
                    foreach (NastupNaSpravi n in startListaPrethRot.Nastupi)
                    {
                        startLista.addNastup(new NastupNaSpravi(n.Gimnasticar, n.Ekipa));
                    }
                    continue;
                }

                // Nadji nacin rotacije (u start listi na prvoj rotaciji).
                StartListaNaSpravi current = startListaPrethRot;
                while (current.Rotacija != 1)
                    current = getStartListaPrethRot(current, aktivneSprave);
                NacinRotacije nacinRotacije = current.NacinRotacije;

                if (nacinRotacije == NacinRotacije.RotirajSve || nacinRotacije == NacinRotacije.NeRotirajNista)
                {
                    foreach (NastupNaSpravi n in startListaPrethRot.Nastupi)
                        startLista.addNastup(new NastupNaSpravi(n.Gimnasticar, n.Ekipa));

                    if (nacinRotacije == NacinRotacije.RotirajSve)
                    {
                        NastupNaSpravi n2 = startLista.Nastupi[0];
                        startLista.removeNastup(n2);
                        startLista.addNastup(n2);
                    }
                }
                else if (nacinRotacije == NacinRotacije.RotirajEkipeRotirajGimnasticare
                         || nacinRotacije == NacinRotacije.NeRotirajEkipeRotirajGimnasticare)
                {
                    // Najpre pronadji ekipe
                    List<List<NastupNaSpravi>> listaEkipa = new List<List<NastupNaSpravi>>();
                    int m = 0;
                    while (m < startListaPrethRot.Nastupi.Count)
                    {
                        NastupNaSpravi n = startListaPrethRot.Nastupi[m];
                        byte ekipaId = n.Ekipa;
                        if (ekipaId == 0)
                        {
                            List<NastupNaSpravi> pojedinac = new List<NastupNaSpravi>();
                            pojedinac.Add(new NastupNaSpravi(n.Gimnasticar, 0));
                            listaEkipa.Add(pojedinac);
                            ++m;
                            continue;
                        }

                        List<NastupNaSpravi> novaEkipa = new List<NastupNaSpravi>();
                        while (n.Ekipa == ekipaId)
                        {
                            novaEkipa.Add(new NastupNaSpravi(n.Gimnasticar, ekipaId));
                            if (++m < startListaPrethRot.Nastupi.Count)
                                n = startListaPrethRot.Nastupi[m];
                            else
                                break;
                        }
                        listaEkipa.Add(novaEkipa);
                    }

                    if (nacinRotacije == NacinRotacije.RotirajEkipeRotirajGimnasticare)
                    {
                        // Rotiraj ekipe
                        List<NastupNaSpravi> prvaEkipa = listaEkipa[0];
                        listaEkipa.RemoveAt(0);
                        listaEkipa.Add(prvaEkipa);
                    }

                    foreach (List<NastupNaSpravi> ekipa in listaEkipa)
                    {
                        // Rotiraj clanove ekipe
                        NastupNaSpravi nastup = ekipa[0];
                        ekipa.RemoveAt(0);
                        ekipa.Add(nastup);

                        foreach (NastupNaSpravi n in ekipa)
                            startLista.addNastup(new NastupNaSpravi(n.Gimnasticar, n.Ekipa));
                    }
                }
            }
        }

        public virtual void prebaciGimnasticare(IList<NastupNaSpravi> nastupi, StartListaNaSpravi from,
            StartListaNaSpravi to)
        {
            foreach (NastupNaSpravi n in nastupi)
            {
                if (from.removeNastup(n))
                {
                    // Kod prebacivanja ne proveravam da li je gimnasticar vec u nekoj start listi u istoj rotaciji zato
                    // sto je prilikom dodavanja gimnasticara osigurano da gimnasticar moze da bude u samo jednoj start
                    // listi u rotaciji).
                    to.addNastup(new NastupNaSpravi(n.Gimnasticar, 0));
                }
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());
            strBuilder.AppendLine(Naziv != null ? Naziv : NULL);
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);
            strBuilder.AppendLine(PauzeMask.ToString());

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

            PauzeMask = int.Parse(reader.ReadLine());
            
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
