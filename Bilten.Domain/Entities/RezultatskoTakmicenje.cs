using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Exceptions;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Bilten.Domain
{
    public class RezultatskoTakmicenje : DomainObject
    {
        private byte redBroj;
        public virtual byte RedBroj
        {
            get { return redBroj; }
            set { redBroj = value; }
        }

        private Gimnastika gimnastika;
        public virtual Gimnastika Gimnastika
        {
            get { return gimnastika; }
            set { gimnastika = value; }
        }

        // TODO: Ukloni ovaj hack, tj. sledeca dva svojstva. Kreiraj klasu 
        // EkipnoTakmicenje, koja ce biti zaduzena za Takmicenja 1 i 4 u ekipnoj 
        // konkurenciji). Ukloniti takodje i svojstvo JednoTak4ZaSveKategorije 
        // klase Propozicije.

        private bool imaEkipnoTakmicenje = true;
        public virtual bool ImaEkipnoTakmicenje
        {
            get { return imaEkipnoTakmicenje; }
            set { imaEkipnoTakmicenje = value; }
        }

        // kada postoji samo jedno ekipno takmicenje za vise kategorija, ovo svojstvo
        // ce biti true samo za jedno RezultatskoTakmicenje.
        private bool kombinovanoEkipnoTak;
        public virtual bool KombinovanoEkipnoTak
        {
            get { return kombinovanoEkipnoTak; }
            set { kombinovanoEkipnoTak = value; }
        }

        private Takmicenje takmicenje;
        public virtual Takmicenje Takmicenje
        {
            get { return takmicenje; }
            set { takmicenje = value; }
        }

        private TakmicarskaKategorija kategorija;
        public virtual TakmicarskaKategorija Kategorija
        {
            get { return kategorija; }
            set { kategorija = value; }
        }

        private RezultatskoTakmicenjeDescription description;
        public virtual RezultatskoTakmicenjeDescription TakmicenjeDescription
        {
            get { return description; }
            set { description = value; }
        }

        private Propozicije propozicije;
        public virtual Propozicije Propozicije
        {
            get { return propozicije; }
            set { propozicije = value; }
        }

        private Takmicenje1 _takmicenje1;
        public virtual Takmicenje1 Takmicenje1
        {
            get { return _takmicenje1; }
            protected set { _takmicenje1 = value; }
        }

        private Takmicenje2 _takmicenje2;
        public virtual Takmicenje2 Takmicenje2
        {
            get { return _takmicenje2; }
            protected set { _takmicenje2 = value; }
        }

        private Takmicenje3 _takmicenje3;
        public virtual Takmicenje3 Takmicenje3
        {
            get { return _takmicenje3; }
            protected set { _takmicenje3 = value; }
        }

        private Takmicenje4 _takmicenje4;
        public virtual Takmicenje4 Takmicenje4
        {
            get { return _takmicenje4; }
            protected set { _takmicenje4 = value; }
        }

        public RezultatskoTakmicenje()
        { 
            // NOTE, TODO: Ovaj konstruktor nije za public upotrebu. Trebao je da bude
            // protected ali tada ne bi mogao da koristim RezultatskoTakmicenje kao
            // genericki parametar. Za kreiranje je potrebno koristiti donji 
            // konstruktor. Proveri da li je tako svugde u programu. 
        }

        public RezultatskoTakmicenje(Takmicenje takmicenje, TakmicarskaKategorija
            kategorija, RezultatskoTakmicenjeDescription desc, Propozicije propozicije)
        {
            this.takmicenje = takmicenje;
            this.kategorija = kategorija;
            this.description = desc;
            this.propozicije = propozicije;
            this.gimnastika = takmicenje.Gimnastika;

            _takmicenje1 = new Takmicenje1(takmicenje.Gimnastika);
            if (propozicije.PostojiTak2 && propozicije.OdvojenoTak2)
                _takmicenje2 = new Takmicenje2();
            if (propozicije.PostojiTak3 && propozicije.OdvojenoTak3)
                _takmicenje3 = new Takmicenje3(takmicenje.Gimnastika);
            if (propozicije.PostojiTak4 && propozicije.OdvojenoTak4)
                _takmicenje4 = new Takmicenje4();
        }

        public virtual void updateTakmicenjaFromChangedPropozicije()
        {
            if ((Propozicije.PostojiTak2 && Propozicije.OdvojenoTak2) != (Takmicenje2 != null))
            {
                if (Takmicenje2 != null)
                    // postojalo je odvojeno takmicenje II, ali je u novim propozicijama
                    // OdvojenoTak2 postavljeno na false
                    Takmicenje2 = null;
                else
                    // nije postojalo odvojeno takmicenje II, a u novim propozicijama je
                    // OdvojenoTak2 postavljeno na true
                    Takmicenje2 = new Takmicenje2();
            }
            if ((Propozicije.PostojiTak3 && Propozicije.OdvojenoTak3) != (Takmicenje3 != null))
            {
                if (Takmicenje3 != null)
                    Takmicenje3 = null;
                else
                    Takmicenje3 = new Takmicenje3(this.Gimnastika);
            }
            if ((Propozicije.PostojiTak4 && Propozicije.OdvojenoTak4) != (Takmicenje4 != null))
            {
                if (Takmicenje4 != null)
                    Takmicenje4 = null;
                else
                    Takmicenje4 = new Takmicenje4();
            }
        }

        public virtual string Naziv
        {
            get 
            {
                string result = String.Empty;
                if (Kategorija != null)
                    result += Kategorija.NazivPol;
                if (TakmicenjeDescription != null)
                {
                    if (result != String.Empty)
                        result += " - ";
                    result += TakmicenjeDescription.Naziv;
                }
                return result; 
            }
        }

        public override string ToString()
        {
            return Naziv;
        }

        // sledeca dva svojstva omogucuju da se sortiranjem kreira RedBroj
        public virtual byte KategorijaRedBroj
        {
            get { return Kategorija.RedBroj; }
        }

        public virtual byte TakmicenjeDescriptionRedBroj
        {
            get { return TakmicenjeDescription.RedBroj; }
        }

        public virtual string NazivEkipnog
        {
            get
            {
                string result = String.Empty;

                if (ImaEkipnoTakmicenje)
                {
                    if (!KombinovanoEkipnoTak)
                        return Naziv;
                    else
                    {
                        if (TakmicenjeDescription != null)
                            return TakmicenjeDescription.Naziv;
                    }
                }

                return result;
            }
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is RezultatskoTakmicenje)) return false;

            RezultatskoTakmicenje that = (RezultatskoTakmicenje)other;
            return (this.Gimnastika == that.Gimnastika
                && this.Takmicenje.Equals(that.Takmicenje)
                && this.Kategorija.Equals(that.Kategorija)
                && this.TakmicenjeDescription.Equals(that.TakmicenjeDescription));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Gimnastika.GetHashCode();
                result = 29 * result + Takmicenje.GetHashCode();
                result = 29 * result + Kategorija.GetHashCode();
                result = 29 * result + TakmicenjeDescription.GetHashCode();
                return result;
            }
        }
        
        public virtual PoredakUkupno getPoredakUkupno(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.PoredakUkupno;
            
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje2);
            return Takmicenje2.Poredak;
        }

        public virtual PoredakSprava getPoredakSprava(DeoTakmicenjaKod deoTakKod, Sprava sprava)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.getPoredakSprava(sprava);

            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje3);
            return Takmicenje3.getPoredak(sprava);
        }

        public virtual PoredakPreskok getPoredakPreskok(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.PoredakPreskok;

            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje3);
            return Takmicenje3.PoredakPreskok;
        }

        public virtual PoredakEkipno getPoredakEkipno(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.PoredakEkipno;

            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje4);
            return Takmicenje4.Poredak;
        }

        public virtual bool postojeKvalifikacijeViseboj(DeoTakmicenjaKod deoTakKod)
        {
            return deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && Propozicije.PostojiTak2 && Propozicije.OdvojenoTak2;
        }

        public virtual bool postojeKvalifikacijeSprava(DeoTakmicenjaKod deoTakKod)
        {
            return deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && Propozicije.PostojiTak3 && Propozicije.OdvojenoTak3;
        }

        public virtual bool postojeKvalifikacijeEkipno(DeoTakmicenjaKod deoTakKod)
        {
            return deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && Propozicije.PostojiTak4 && Propozicije.OdvojenoTak4;
        }

        public virtual string getNazivIzvestajaViseboj(DeoTakmicenjaKod deoTakKod, bool finaleKupa, bool sumaObaKola)
        {
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje1 || deoTakKod == DeoTakmicenjaKod.Takmicenje2);

            //char shVeliko = '\u0160';
            char shMalo = '\u0161';
            string result = String.Empty;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (finaleKupa)
                {
                    if (sumaObaKola)
                        result = "I i II kolo - Rezultati vi" + shMalo + "eboj";
                    else
                        result = "Vi" + shMalo + "eboj";
                }
                else
                {
                    if (Propozicije.OdvojenoTak2)
                        result = "Kvalifikacije za finale vi" + shMalo + "eboja";
                    else
                        result = "Vi" + shMalo + "eboj";
                }
            }
            else
            {
                result = "Finale vi" + shMalo + "eboja";
            }
            return result;
        }

        public virtual string getNazivIzvestajaSprava(DeoTakmicenjaKod deoTakKod, bool finaleKupa, bool sumaObaKola)
        {
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje1 || deoTakKod == DeoTakmicenjaKod.Takmicenje3);

            string result = String.Empty;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (finaleKupa)
                {
                    if (sumaObaKola)
                        result = "I i II kolo - Rezultati po spravama";
                    else
                        result = "Finale po spravama";
                }
                else
                {
                    if (Propozicije.OdvojenoTak3)
                        result = "Kvalifikacije za finale po spravama";
                    else
                        result = "Finale po spravama";
                }
            }
            else
            {
                result = "Finale po spravama";
            }
            return result;
        }

        public static void updateImaEkipnoTakmicenje(IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            IDictionary<int, List<RezultatskoTakmicenje>> rezTakMap = new Dictionary<int, List<RezultatskoTakmicenje>>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rezTakMap.ContainsKey(rt.TakmicenjeDescription.Id))
                {
                    rezTakMap[rt.TakmicenjeDescription.Id].Add(rt);
                }
                else
                {
                    List<RezultatskoTakmicenje> rezTakList = new List<RezultatskoTakmicenje>();
                    rezTakList.Add(rt);
                    rezTakMap.Add(rt.TakmicenjeDescription.Id, rezTakList);
                }
            }
            foreach (List<RezultatskoTakmicenje> rezTakList in rezTakMap.Values)
            {
                bool kombAdded = false;
                foreach (RezultatskoTakmicenje rt in rezTakList)
                {
                    if (!rt.TakmicenjeDescription.Propozicije.JednoTak4ZaSveKategorije)
                    {
                        rt.ImaEkipnoTakmicenje = true;
                        rt.KombinovanoEkipnoTak = false;
                    }
                    else
                    {
                        if (!kombAdded)
                        {
                            rt.ImaEkipnoTakmicenje = true;
                            rt.KombinovanoEkipnoTak = true;
                            kombAdded = true;
                        }
                        else
                        {
                            rt.ImaEkipnoTakmicenje = false;
                            rt.KombinovanoEkipnoTak = false;
                        }
                    }
                }
            }
        }

        public virtual bool ucestvujeEkipno(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod)
        {
            if (!ImaEkipnoTakmicenje)
                return false;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                foreach (Ekipa e in Takmicenje1.Ekipe)
                {
                    if (e.Gimnasticari.Contains(g))
                        return true;
                }
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje4)
            {
                foreach (UcesnikTakmicenja4 u in Takmicenje4.Ucesnici)
                {
                    if (u.Ekipa.Gimnasticari.Contains(g))
                        return true;
                }
            }
            return false;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(RedBroj.ToString());
            strBuilder.AppendLine(Gimnastika.ToString());
            strBuilder.AppendLine(ImaEkipnoTakmicenje.ToString());
            strBuilder.AppendLine(KombinovanoEkipnoTak.ToString());
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);
            strBuilder.AppendLine(Kategorija != null ? Kategorija.Id.ToString() : NULL);
            strBuilder.AppendLine(TakmicenjeDescription != null ? TakmicenjeDescription.Id.ToString() : NULL);
            if (Propozicije == null)
                strBuilder.AppendLine(NULL);
            else
                Propozicije.dump(strBuilder);
            if (Takmicenje1 == null)
                strBuilder.AppendLine(NULL);
            else
                Takmicenje1.dump(strBuilder);
            if (Takmicenje2 == null)
                strBuilder.AppendLine(NULL);
            else
                Takmicenje2.dump(strBuilder);
            if (Takmicenje3 == null)
                strBuilder.AppendLine(NULL);
            else
                Takmicenje3.dump(strBuilder);
            if (Takmicenje4 == null)
                strBuilder.AppendLine(NULL);
            else
                Takmicenje4.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            RedBroj = byte.Parse(reader.ReadLine());
            Gimnastika = (Gimnastika)Enum.Parse(typeof(Gimnastika), reader.ReadLine());
            ImaEkipnoTakmicenje = bool.Parse(reader.ReadLine());
            KombinovanoEkipnoTak = bool.Parse(reader.ReadLine());

            Takmicenje = map.takmicenjeMap[int.Parse(reader.ReadLine())];
            Kategorija = map.kategorijeMap[int.Parse(reader.ReadLine())];
            TakmicenjeDescription = map.descriptionsMap[int.Parse(reader.ReadLine())];

            string id = reader.ReadLine();
            Propozicije p = null;
            if (id != NULL)
            {
                p = new Propozicije();
                p.loadFromDump(reader);
            }
            Propozicije = p;

            id = reader.ReadLine();
            Takmicenje1 t1 = null;
            if (id != NULL)
            {
                t1 = new Takmicenje1();
                t1.loadFromDump(reader, map);
            }
            Takmicenje1 = t1;

            id = reader.ReadLine();
            Takmicenje2 t2 = null;
            if (id != NULL)
            {
                t2 = new Takmicenje2();
                t2.loadFromDump(reader, map);
            }
            Takmicenje2 = t2;

            id = reader.ReadLine();
            Takmicenje3 t3 = null;
            if (id != NULL)
            {
                t3 = new Takmicenje3();
                t3.loadFromDump(reader, map);
            }
            Takmicenje3 = t3;

            id = reader.ReadLine();
            Takmicenje4 t4 = null;
            if (id != NULL)
            {
                t4 = new Takmicenje4();
                t4.loadFromDump(reader, map);
            }
            Takmicenje4 = t4;
        }
    }
}
