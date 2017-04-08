using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.Diagnostics;
using Bilten.Util;
using System.IO;
using Bilten.Exceptions;

namespace Bilten.Domain
{
    public class Takmicenje : DomainObject
    {
        private static readonly int NAZIV_MAX_LENGTH = 128;
        private static readonly int MESTO_MAX_LENGTH = 32;

        private string naziv;
        public virtual string Naziv
        {
            get { return naziv; }
            set {
                // Naziv takmicenja i naziv prvog descriptiona moraju da se poklapaju.
                naziv = value;
                if (TakmicenjeDescriptions.Count == 0)
                {
                    RezultatskoTakmicenjeDescription d = new RezultatskoTakmicenjeDescription();
                    d.Naziv = value;
                    d.Propozicije = new Propozicije();
                    addTakmicenjeDescription(d);
                }
                else
                {
                    RezultatskoTakmicenjeDescription d = getDescription(0);
                    d.Naziv = value;
                }
                // Prethodni kod se ne izvrsava prilikom ucitavanja takmicenja iz baze zato sto sam za Naziv stavio
                // access="nosetter.camelcase"
            }
        }

        private Gimnastika gimnastika;
        public virtual Gimnastika Gimnastika
        {
            get { return gimnastika; }
            set { gimnastika = value; }
        }

        private DateTime datum;
        public virtual DateTime Datum
        {
            get { return datum; }
            set { datum = value; }
        }

        private string mesto;
        public virtual string Mesto
        {
            get { return mesto; }
            set { mesto = value; }
        }

        private TipTakmicenja tipTakmicenja;
        public virtual TipTakmicenja TipTakmicenja
        {
            get { return tipTakmicenja; }
            set { tipTakmicenja = value; }
        }

        public virtual bool StandardnoTakmicenje
        {
            get { return TipTakmicenja == TipTakmicenja.StandardnoTakmicenje; }
            set { throw new Exception("Not supported"); }
        }

        public virtual bool FinaleKupa
        {
            get { return TipTakmicenja == TipTakmicenja.FinaleKupa; }
            set { throw new Exception("Not supported"); }
        }

        public virtual bool ZbirViseKola
        {
            get { return TipTakmicenja == TipTakmicenja.ZbirViseKola; }
            set { throw new Exception("Not supported"); }
        }

        // TODO3: Zameni PrvoKolo, DrugoKolo, TreceKolo i CetvrtoKolo sa kolekcijom PrethodnaKola

        private Takmicenje prvoKolo;
        public virtual Takmicenje PrvoKolo
        {
            get { return prvoKolo; }
            set { prvoKolo = value; }
        }

        private Takmicenje drugoKolo;
        public virtual Takmicenje DrugoKolo
        {
            get { return drugoKolo; }
            set { drugoKolo = value; }
        }

        private Takmicenje treceKolo;
        public virtual Takmicenje TreceKolo
        {
            get { return treceKolo; }
            set { treceKolo = value; }
        }

        private Takmicenje cetvrtoKolo;
        public virtual Takmicenje CetvrtoKolo
        {
            get { return cetvrtoKolo; }
            set { cetvrtoKolo = value; }
        }

        private byte brojEOcena = 6;
        public virtual byte BrojEOcena
        {
            get { return brojEOcena; }
            set { brojEOcena = value; }
        }

        private byte brojDecimalaD = 3;
        public virtual byte BrojDecimalaD
        {
            get { return brojDecimalaD; }
            set { brojDecimalaD = value; }
        }

        private byte brojDecimalaE1 = 2;
        public virtual byte BrojDecimalaE1
        {
            get { return brojDecimalaE1; }
            set { brojDecimalaE1 = value; }
        }

        private byte brojDecimalaE = 3;
        public virtual byte BrojDecimalaE
        {
            get { return brojDecimalaE; }
            set { brojDecimalaE = value; }
        }

        private byte brojDecimalaPen = 1;
        public virtual byte BrojDecimalaPen
        {
            get { return brojDecimalaPen; }
            set { brojDecimalaPen = value; }
        }

        private byte brojDecimalaTotal = 3;
        public virtual byte BrojDecimalaTotal
        {
            get { return brojDecimalaTotal; }
            set { brojDecimalaTotal = value; }
        }

        private bool zavrsenoTak1;
        public virtual bool ZavrsenoTak1
        {
            get { return zavrsenoTak1; }
            set { zavrsenoTak1 = value; }
        }

        private string zrebZaFinalePoSpravama;
        public virtual string ZrebZaFinalePoSpravama
        {
            get { return zrebZaFinalePoSpravama; }
            set { zrebZaFinalePoSpravama = value; }
        }

        private Iesi.Collections.Generic.ISet<RezultatskoTakmicenjeDescription> takmicenjeDescriptions = 
            new HashedSet<RezultatskoTakmicenjeDescription>();
        public virtual Iesi.Collections.Generic.ISet<RezultatskoTakmicenjeDescription> TakmicenjeDescriptions
        {
            get { return takmicenjeDescriptions; }
            protected set { takmicenjeDescriptions = value; }
        }

        public virtual void addTakmicenjeDescription(RezultatskoTakmicenjeDescription desc)
        {
            if (TakmicenjeDescriptions.Add(desc))
                desc.RedBroj = (byte)(TakmicenjeDescriptions.Count - 1);
        }

        public virtual void removeTakmicenjeDescription(RezultatskoTakmicenjeDescription desc)
        {
            // Nije dozvoljeno uklanjati prvi description zato sto je prvi description kreiran da ima isti naziv kao i
            // takmicenje.
            if (desc.RedBroj == 0)
                throw new BusinessException("Nije dozvoljeno brisati prvo takmicenje.");

            if (TakmicenjeDescriptions.Remove(desc))
            {
                foreach (RezultatskoTakmicenjeDescription d in TakmicenjeDescriptions)
                {
                    if (d.RedBroj > desc.RedBroj)
                        d.RedBroj--;
                }
            }
        }

        public virtual bool moveTakmicenjeDescriptionUp(
            RezultatskoTakmicenjeDescription desc)
        {
            if (desc.RedBroj == 0)
                return false;

            // Posto je description 0 nepromenljiv (tj. mora da bude istog naziva kao i takmicenje), nije moguce
            // description 1 pomeriti na njegovo mesto.
            if (desc.RedBroj == 1)
                throw new BusinessException("Nije dozvoljeno pomeriti prvo takmicenje.");

            foreach (RezultatskoTakmicenjeDescription d in TakmicenjeDescriptions)
            {
                if (d.RedBroj == desc.RedBroj - 1)
                {
                    d.RedBroj++;
                    break;
                }
            }
            desc.RedBroj--;
            return true;
        }

        public virtual bool moveTakmicenjeDescriptionDown(
            RezultatskoTakmicenjeDescription desc)
        {
            // Description 0 nije moguce pomeriti na dole.
            if (desc.RedBroj == 0)
                throw new BusinessException("Nije dozvoljeno pomeriti prvo takmicenje.");

            if (desc.RedBroj == TakmicenjeDescriptions.Count - 1)
                return false;

            foreach (RezultatskoTakmicenjeDescription d in TakmicenjeDescriptions)
            {
                if (d.RedBroj == desc.RedBroj + 1)
                {
                    d.RedBroj--;
                    break;
                }
            }
            desc.RedBroj++;
            return true;
        }

        private RezultatskoTakmicenjeDescription getDescription(int redBroj)
        {
            foreach (RezultatskoTakmicenjeDescription d in TakmicenjeDescriptions)
            {
                if (d.RedBroj == redBroj)
                    return d;
            }
            return null;
        }

        private Iesi.Collections.Generic.ISet<TakmicarskaKategorija> kategorije =
            new HashedSet<TakmicarskaKategorija>();
        public virtual Iesi.Collections.Generic.ISet<TakmicarskaKategorija> Kategorije
        {
            get { return kategorije; }
            protected set { kategorije = value; }
        }

        // NOTE: Metodi addKategorija i removeKategorija upravljaju dvosmernom 
        // asocijacijom izmedju Takmicenja i TakmicarskeKategorije
        public virtual void addKategorija(TakmicarskaKategorija kat)
        {
            if (Kategorije.Add(kat))
            {
                kat.setTakmicenjeInternal(this);
                kat.RedBroj = (byte)(Kategorije.Count - 1);
            }
        }

        public virtual void removeKategorija(TakmicarskaKategorija kat)
        {
            if (Kategorije.Remove(kat))
            {
                kat.setTakmicenjeInternal(null);
                foreach (TakmicarskaKategorija k in Kategorije)
                {
                    if (k.RedBroj > kat.RedBroj)
                        k.RedBroj--;
                }
            }
        }

        public virtual bool moveKategorijaUp(TakmicarskaKategorija kat)
        {
            //if (redBroj < 1 || redBroj > Kategorije.Count - 1)
            //    return false;
            if (kat.RedBroj == 0)
                return false;

            foreach (TakmicarskaKategorija k in Kategorije)
            {
                if (k.RedBroj == kat.RedBroj - 1)
                {
                    k.RedBroj++;
                    break;
                }
            }
            kat.RedBroj--;
            return true;
        }

        public virtual bool moveKategorijaDown(TakmicarskaKategorija kat)
        {
            //if (redBroj < 0 || redBroj >= Kategorije.Count - 1)
            //    return false;
            if (kat.RedBroj == Kategorije.Count - 1)
                return false;

            foreach (TakmicarskaKategorija k in Kategorije)
            {
                if (k.RedBroj == kat.RedBroj + 1)
                {
                    k.RedBroj--;
                    break;
                }
            }
            kat.RedBroj++;
            return true;
        }

        public virtual TakmicarskaKategorija getKategorija(int redBroj)
        {
            foreach (TakmicarskaKategorija k in Kategorije)
            {
                if (k.RedBroj == redBroj)
                    return k;
            }
            return null;
        }

        public Takmicenje()
        { 
        
        }

        public virtual string GimnastikaNaziv
        {
            get { return Gimnastika.ToString() + " - " + Naziv; }
        }

        public override string ToString()
        {
            return GimnastikaNaziv + ", " + Mesto + ", " + Datum.ToString("d");
        }

        public override void validate(Notification notification)
        {
            // validate Naziv
            if (string.IsNullOrEmpty(Naziv))
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv takmicenja je obavezan.");
            }
            else if (Naziv.Length > NAZIV_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv takmicenja moze da sadrzi maksimalno "
                    + NAZIV_MAX_LENGTH + " znakova.");
            }

            if (Gimnastika == Gimnastika.Undefined)
            {
                notification.RegisterMessage(
                    "Gimnastika", "Gimnastika je obavezna.");
            }

            // validate Mesto
            if (string.IsNullOrEmpty(Mesto))
            {
                notification.RegisterMessage(
                    "Mesto", "Mesto odrzavanja je obavezno.");
            }
            else if (Mesto.Length > MESTO_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Mesto", "Mesto odrzavanja moze da sadrzi maksimalno "
                    + MESTO_MAX_LENGTH + " znakova.");
            }
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) 
                return true;
            if (!(other is Takmicenje)) 
                return false;
            
            Takmicenje that = (Takmicenje)other;
            return this.Naziv.ToUpper() == that.Naziv.ToUpper()
                && this.Gimnastika == that.Gimnastika
                && this.Datum == that.Datum;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Naziv.GetHashCode();
                result = 29 * result + Gimnastika.GetHashCode();
                result = 29 * result + Datum.GetHashCode();
                return result;
            }
        }

        public virtual IList<RezultatskoTakmicenje> getRezTakmicenjaViseboj(IList<RezultatskoTakmicenje> svaRezTakmicenja,
            DeoTakmicenjaKod deoTakKod, bool sumaObaKola)
        {
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje1 || deoTakKod == DeoTakmicenjaKod.Takmicenje2);
            IList<RezultatskoTakmicenje> result = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
            {
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    if (rt.Propozicije.PostojiTak2)
                    {
                        if (!FinaleKupa || sumaObaKola || rt.Propozicije.OdvojenoTak2)
                            result.Add(rt);
                    }
                    else
                    {
                        // NOTE: Za takmicenje 1 se rezultati i poredak uvek racunaju za sprave i viseboj, i moguce ih je
                        // pregledati u prozoru za rezultate, cak i ako su PostojiTak2 ili PostojiTak3 false.
                        result.Add(rt);
                    }
                }
                else
                {
                    if (rt.Propozicije.PostojiTak2 && rt.Propozicije.OdvojenoTak2)
                        result.Add(rt);
                }
            }
            return result;
        }

        public virtual IList<RezultatskoTakmicenje> getRezTakmicenjaSprava(IList<RezultatskoTakmicenje> svaRezTakmicenja,
            DeoTakmicenjaKod deoTakKod, bool sumaObaKola)
        {
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje1 || deoTakKod == DeoTakmicenjaKod.Takmicenje3);
            IList<RezultatskoTakmicenje> result = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
            {
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    if (rt.Propozicije.PostojiTak3)
                    {
                        if (!FinaleKupa || sumaObaKola || rt.Propozicije.OdvojenoTak3)
                            result.Add(rt);
                    }
                    else
                    {
                        // NOTE: Za takmicenje 1 se rezultati i poredak uvek racunaju za sprave i viseboj, i moguce ih je
                        // pregledati u prozoru za rezultate, cak i ako su PostojiTak2 ili PostojiTak3 false.
                        // TODO4: Proveri da li treba prikazivati rezultate i za finale kupa i zbir vise kola. Isto i za
                        // viseboj u metodu getRezTakmicenjaViseboj.
                        result.Add(rt);
                    }
                }
                else
                {
                    if (rt.Propozicije.PostojiTak3 && rt.Propozicije.OdvojenoTak3)
                        result.Add(rt);
                }
            }
            return result;
        }

        public virtual IList<RezultatskoTakmicenje> getRezTakmicenjaEkipe(IList<RezultatskoTakmicenje> svaRezTakmicenja,
            DeoTakmicenjaKod deoTakKod, bool sumaObaKola)
        {
            Trace.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje1 || deoTakKod == DeoTakmicenjaKod.Takmicenje4);
            IList<RezultatskoTakmicenje> result = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
            {
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    if (rt.Propozicije.PostojiTak4 && rt.ImaEkipnoTakmicenje)
                    {
                        if (!FinaleKupa || sumaObaKola || rt.Propozicije.OdvojenoTak4)
                            result.Add(rt);
                    }
                }
                else
                {
                    if (rt.Propozicije.PostojiTak4 && rt.Propozicije.OdvojenoTak4 && rt.ImaEkipnoTakmicenje)
                        result.Add(rt);
                }
            }
            return result;
        }

        public static RezultatskoTakmicenje getRezTakmicenje(IList<RezultatskoTakmicenje> rezTakmicenja,
            TakmicarskaKategorija kat)
        {
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.Kategorija.Equals(kat))
                    return rezTak;
            }
            return null;
        }

        public virtual void createPoredakUkupnoFinaleKupa(RezultatskoTakmicenje rezTakFinaleKupa,
            IList<RezultatskoTakmicenje> rezTakmicenjaPrvoKolo,
            IList<RezultatskoTakmicenje> rezTakmicenjaDrugoKolo)
        {
            // TODO3: Ovo ce raditi samo ako su prvo i drugo kolo imali samo jedno takmicenje. (takodje i kod
            // poretka ekipa i sprava)
            PoredakUkupno poredak1 =
                getRezTakmicenje(rezTakmicenjaPrvoKolo, rezTakFinaleKupa.Kategorija).Takmicenje1.PoredakUkupno;
            PoredakUkupno poredak2 =
                getRezTakmicenje(rezTakmicenjaDrugoKolo, rezTakFinaleKupa.Kategorija).Takmicenje1.PoredakUkupno;
            rezTakFinaleKupa.Takmicenje1.PoredakUkupnoFinaleKupa.create(rezTakFinaleKupa, poredak1, poredak2);
        }

        public virtual void createPoredakSpravaFinaleKupa(RezultatskoTakmicenje rezTak,
            IList<RezultatskoTakmicenje> rezTakmicenjaPrvoKolo,
            IList<RezultatskoTakmicenje> rezTakmicenjaDrugoKolo,
            List<RezultatSpravaFinaleKupaUpdate> rezultatiUpdate)
        {
            RezultatskoTakmicenje rezTakPrvoKolo = getRezTakmicenje(rezTakmicenjaPrvoKolo, rezTak.Kategorija);
            RezultatskoTakmicenje rezTakDrugoKolo = getRezTakmicenje(rezTakmicenjaDrugoKolo, rezTak.Kategorija);

            rezTak.Takmicenje1.initPoredakSpravaFinaleKupa(this.Gimnastika);

            foreach (Sprava s in Sprave.getSprave(this.Gimnastika))
            {
                if (s != Sprava.Preskok)
                {
                    PoredakSprava poredakPrvoKolo = null;
                    PoredakSprava poredakDrugoKolo = null;
                    if (rezTakPrvoKolo != null)
                        poredakPrvoKolo = rezTakPrvoKolo.Takmicenje1.getPoredakSprava(s);
                    if (rezTakDrugoKolo != null)
                        poredakDrugoKolo = rezTakDrugoKolo.Takmicenje1.getPoredakSprava(s);
                    rezTak.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rezTak,
                        poredakPrvoKolo, poredakDrugoKolo, rezultatiUpdate);
                }
                else
                {
                    PoredakPreskok poredakPrvoKolo = null;
                    PoredakPreskok poredakDrugoKolo = null;
                    if (rezTakPrvoKolo != null)
                        poredakPrvoKolo = rezTakPrvoKolo.Takmicenje1.PoredakPreskok;
                    if (rezTakDrugoKolo != null)
                        poredakDrugoKolo = rezTakDrugoKolo.Takmicenje1.PoredakPreskok;

                    bool poredakNaOsnovuObaPreskokaPrvoKolo = false;
                    bool poredakNaOsnovuObaPreskokaDrugoKolo = false;
                    if (rezTakPrvoKolo != null)
                        poredakNaOsnovuObaPreskokaPrvoKolo =
                            rezTakPrvoKolo.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;
                    if (rezTakDrugoKolo != null)
                        poredakNaOsnovuObaPreskokaDrugoKolo =
                            rezTakDrugoKolo.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;

                    rezTak.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rezTak,
                        poredakPrvoKolo, poredakDrugoKolo,
                        poredakNaOsnovuObaPreskokaPrvoKolo, poredakNaOsnovuObaPreskokaDrugoKolo, rezultatiUpdate);
                }
            }
        }

        public virtual IList<RezultatUkupno> createRezultatiUkupnoZaSveEkipe(
            IList<RezultatskoTakmicenje> rezTakmicenja, IList<Ocena> ocene, DeoTakmicenjaKod deoTakKod)
        {
            List<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                IList<Ekipa> ekipe;
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                    ekipe = new List<Ekipa>(rt.Takmicenje1.Ekipe);
                else
                    ekipe = rt.Takmicenje4.getUcesnici();

                foreach (Ekipa e in ekipe)
                {
                    foreach (GimnasticarUcesnik g in e.Gimnasticari)
                    {
                        if (!gimnasticari.Contains(g))
                            gimnasticari.Add(g);
                    }
                }
            }

            IDictionary<int, RezultatUkupno> rezultatiMap = new Dictionary<int, RezultatUkupno>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatUkupno rezultat = new RezultatUkupno();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g.Id, rezultat);
            }

            foreach (Ocena o in ocene)
            {
                if (rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].addOcena(o);
            }

            List<RezultatUkupno> result = new List<RezultatUkupno>(rezultatiMap.Values);
            return result;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Naziv != null ? Naziv : NULL);
            strBuilder.AppendLine(Gimnastika.ToString());
            strBuilder.AppendLine(Datum.ToString());
            strBuilder.AppendLine(Mesto != null ? Mesto : NULL);
            strBuilder.AppendLine(TipTakmicenja.ToString());
            strBuilder.AppendLine(PrvoKolo != null ? PrvoKolo.Id.ToString() : NULL);
            strBuilder.AppendLine(DrugoKolo != null ? DrugoKolo.Id.ToString() : NULL);
            strBuilder.AppendLine(TreceKolo != null ? TreceKolo.Id.ToString() : NULL);
            strBuilder.AppendLine(CetvrtoKolo != null ? CetvrtoKolo.Id.ToString() : NULL);

            strBuilder.AppendLine(BrojEOcena.ToString());
            strBuilder.AppendLine(BrojDecimalaD.ToString());
            strBuilder.AppendLine(BrojDecimalaE1.ToString());
            strBuilder.AppendLine(BrojDecimalaE.ToString());
            strBuilder.AppendLine(BrojDecimalaPen.ToString());
            strBuilder.AppendLine(BrojDecimalaTotal.ToString());
            strBuilder.AppendLine(ZavrsenoTak1.ToString());
            strBuilder.AppendLine(ZrebZaFinalePoSpravama != null ? ZrebZaFinalePoSpravama : NULL);

            strBuilder.AppendLine(TakmicenjeDescriptions.Count.ToString());
            foreach (RezultatskoTakmicenjeDescription d in TakmicenjeDescriptions)
                d.dump(strBuilder);

            strBuilder.AppendLine(Kategorije.Count.ToString());
            foreach (TakmicarskaKategorija k in Kategorije)
                k.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map, out int prvoKoloId, out int drugoKoloId,
            out int treceKoloId, out int cetvrtoKoloId)
        {
            string naziv = reader.ReadLine();
            // NOTE: Koristim field naziv (a ne svojstvo Naziv) da bih zaobisao kod u set accessoru za Naziv.
            this.naziv = naziv != NULL ? naziv : null;            
            Gimnastika = (Gimnastika)Enum.Parse(typeof(Gimnastika), reader.ReadLine());
            Datum = DateTime.Parse(reader.ReadLine());
            string mesto = reader.ReadLine();
            Mesto = mesto != NULL ? mesto : null;

            TipTakmicenja = (TipTakmicenja)Enum.Parse(typeof(TipTakmicenja), reader.ReadLine());

            string prvoKoloIdStr = reader.ReadLine();
            string drugoKoloIdStr = reader.ReadLine();
            string treceKoloIdStr = reader.ReadLine();
            string cetvrtoKoloIdStr = reader.ReadLine();
            prvoKoloId = prvoKoloIdStr != NULL ? int.Parse(prvoKoloIdStr) : -1;
            drugoKoloId = drugoKoloIdStr != NULL ? int.Parse(drugoKoloIdStr) : -1;
            treceKoloId = treceKoloIdStr != NULL ? int.Parse(treceKoloIdStr) : -1;
            cetvrtoKoloId = cetvrtoKoloIdStr != NULL ? int.Parse(cetvrtoKoloIdStr) : -1;

            BrojEOcena = byte.Parse(reader.ReadLine());
            BrojDecimalaD = byte.Parse(reader.ReadLine());
            BrojDecimalaE1 = byte.Parse(reader.ReadLine());
            BrojDecimalaE = byte.Parse(reader.ReadLine());
            BrojDecimalaPen = byte.Parse(reader.ReadLine());
            BrojDecimalaTotal = byte.Parse(reader.ReadLine());
            ZavrsenoTak1 = bool.Parse(reader.ReadLine());

            string zreb = reader.ReadLine();
            ZrebZaFinalePoSpravama = zreb != NULL ? zreb : null;

            int brojTakmicenja = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojTakmicenja; ++i)
            {
                string id = reader.ReadLine();
                RezultatskoTakmicenjeDescription d = new RezultatskoTakmicenjeDescription();
                map.descriptionsMap.Add(int.Parse(id), d);
                d.loadFromDump(reader);
                TakmicenjeDescriptions.Add(d);
            }

            int brojKategorija = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojKategorija; ++i)
            {
                string id = reader.ReadLine();
                TakmicarskaKategorija k = new TakmicarskaKategorija();
                map.kategorijeMap.Add(int.Parse(id), k);
                k.loadFromDump(reader, map);
                Kategorije.Add(k);
            }
        }
    }
}
