using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.Diagnostics;
using Bilten.Util;
using System.IO;
using Bilten.Exceptions;
using System.Globalization;

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
            set { naziv = value; }
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

        private SudijaUcesnik vrhovniSudija;
        public virtual SudijaUcesnik VrhovniSudija
        {
            get { return vrhovniSudija; }
            set { vrhovniSudija = value; }
        }

        private byte brojEOcena = 0;
        public virtual byte BrojEOcena
        {
            get { return brojEOcena; }
            set { brojEOcena = value; }
        }

        private byte brojEOcenaTak3 = 0;
        public virtual byte BrojEOcenaTak3
        {
            get { return brojEOcenaTak3; }
            set { brojEOcenaTak3 = value; }
        }

        public virtual byte getBrojEOcena(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                return BrojEOcenaTak3;
            else
                return BrojEOcena;
        }

        private bool odbaciMinMaxEOcenu = true;
        public virtual bool OdbaciMinMaxEOcenu
        {
            get { return odbaciMinMaxEOcenu; }
            set { odbaciMinMaxEOcenu = value; }
        }

        private bool odbaciMinMaxEOcenuTak3 = true;
        public virtual bool OdbaciMinMaxEOcenuTak3
        {
            get { return odbaciMinMaxEOcenuTak3; }
            set { odbaciMinMaxEOcenuTak3 = value; }
        }

        public virtual bool getOdbaciMinMaxEOcenu(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                return OdbaciMinMaxEOcenuTak3;
            else
                return OdbaciMinMaxEOcenu;
        }

        private byte brojDecimalaD = 1;
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

        private byte brojDecimalaBon = 1;
        public virtual byte BrojDecimalaBon
        {
            get { return brojDecimalaBon; }
            set { brojDecimalaBon = value; }
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

        private DateTime lastModified;
        public virtual DateTime LastModified
        {
            get { return lastModified; }
            set { lastModified = value; }
        }

        private string logo1RelPath;
        public virtual string Logo1RelPath
        {
            get { return logo1RelPath; }
            set { logo1RelPath = value; }
        }

        private string logo2RelPath;
        public virtual string Logo2RelPath
        {
            get { return logo2RelPath; }
            set { logo2RelPath = value; }
        }

        private string logo3RelPath;
        public virtual string Logo3RelPath
        {
            get { return logo3RelPath; }
            set { logo3RelPath = value; }
        }

        private string logo4RelPath;
        public virtual string Logo4RelPath
        {
            get { return logo4RelPath; }
            set { logo4RelPath = value; }
        }

        private string logo5RelPath;
        public virtual string Logo5RelPath
        {
            get { return logo5RelPath; }
            set { logo5RelPath = value; }
        }

        private string logo6RelPath;
        public virtual string Logo6RelPath
        {
            get { return logo6RelPath; }
            set { logo6RelPath = value; }
        }

        private string logo7RelPath;
        public virtual string Logo7RelPath
        {
            get { return logo7RelPath; }
            set { logo7RelPath = value; }
        }

        private bool takBrojevi;
        public virtual bool TakBrojevi
        {
            get { return takBrojevi; }
            set { takBrojevi = value; }
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
            if (TakmicenjeDescriptions.Remove(desc))
            {
                foreach (RezultatskoTakmicenjeDescription d in TakmicenjeDescriptions)
                {
                    if (d.RedBroj > desc.RedBroj)
                        d.RedBroj--;
                }
            }
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
        public virtual bool addKategorija(TakmicarskaKategorija kat)
        {
            if (Kategorije.Add(kat))
            {
                kat.setTakmicenjeInternal(this);
                kat.RedBroj = (byte)(Kategorije.Count - 1);
                return true;
            }
            return false;
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

        public virtual int getBrojKola()
        {
            int result = 0;
            if (PrvoKolo != null)
                ++result;
            if (DrugoKolo != null)
                ++result;
            if (TreceKolo != null)
                ++result;
            if (CetvrtoKolo != null)
                ++result;
            return result;
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

            if (PrvoKolo != null && PrvoKolo.Gimnastika != Gimnastika
                || DrugoKolo != null && DrugoKolo.Gimnastika != Gimnastika
                || TreceKolo != null && TreceKolo.Gimnastika != Gimnastika
                || CetvrtoKolo != null && CetvrtoKolo.Gimnastika != Gimnastika)
            {
                notification.RegisterMessage(
                    "Gimnastika", "Gimnastika mora da bude ista kao u prethodnim kolima.");
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
                    // NOTE: Za takmicenje 1 se rezultati i poredak uvek racunaju i moguce ih je pregledati u prozoru za
                    // rezultate, cak i ako je PostojiTak2 false. Zato ne proveravam PostojiTak2.
                    if (StandardnoTakmicenje || ZbirViseKola || (FinaleKupa && (sumaObaKola || rt.odvojenoTak2())))
                        result.Add(rt);
                }
                else if (rt.odvojenoTak2())
                    result.Add(rt);
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
                    // NOTE: Za takmicenje 1 se rezultati i poredak uvek racunaju i moguce ih je pregledati u prozoru za
                    // rezultate, cak i ako je PostojiTak3 false. Zato ne proveravam PostojiTak3.
                    if (StandardnoTakmicenje || (FinaleKupa && (sumaObaKola || rt.odvojenoTak3())))
                        result.Add(rt);
                }
                else if (rt.odvojenoTak3())
                    result.Add(rt);
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
                if (!rt.ImaEkipnoTakmicenje)
                    continue;
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    if (rt.Propozicije.PostojiTak4)
                    {
                        if (StandardnoTakmicenje || ZbirViseKola || (FinaleKupa && (sumaObaKola || rt.odvojenoTak4())))
                            result.Add(rt);
                    }
                }
                else if (rt.odvojenoTak4())
                    result.Add(rt);
            }
            return result;
        }

        public static RezultatskoTakmicenje getRezTakmicenje(IList<RezultatskoTakmicenje> rezTakmicenja,
            int redBrojDesc, TakmicarskaKategorija kat)
        {
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.TakmicenjeDescription.RedBroj == redBrojDesc && rezTak.Kategorija.Equals(kat))
                    return rezTak;
            }
            return null;
        }

        public static IDictionary<int, IList<Pair<RezultatskoTakmicenje, RezultatUkupno>>> getGimRezUkupnoMap(
        IList<RezultatskoTakmicenje> rezTakmicenja, DeoTakmicenjaKod deoTakKod)
        {
            IDictionary<int, IList<Pair<RezultatskoTakmicenje, RezultatUkupno>>> result
                = new Dictionary<int, IList<Pair<RezultatskoTakmicenje, RezultatUkupno>>>();

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                // TODO: Ovo nece raditi za Takmicenje 4 jer ne postoji poredak ukupno za takmicenje 4. Mozda bi za
                // takmicenje 4 takodje trebalo kreirati i poredak ukupno, koji ne bi bio vidljiv u programu i ciji
                // bi se rezultati koristili za ekipni poredak.
                PoredakUkupno p = rt.getPoredakUkupno(deoTakKod);

                foreach (RezultatUkupno r in p.Rezultati)
                {
                    Pair<RezultatskoTakmicenje, RezultatUkupno> rez = new Pair<RezultatskoTakmicenje, RezultatUkupno>(rt, r);
                    if (result.ContainsKey(r.Gimnasticar.Id))
                        // TODO: Deluje da je ova grana nepotrebna jer svaki gimnasticar ima samo jedan RezultatUkupno,
                        // bez obzira na to u koliko takmicenja ucestvuje.
                        result[r.Gimnasticar.Id].Add(rez);
                    else
                    {
                        IList<Pair<RezultatskoTakmicenje, RezultatUkupno>> rezList
                            = new List<Pair<RezultatskoTakmicenje, RezultatUkupno>>();
                        rezList.Add(rez);
                        result.Add(r.Gimnasticar.Id, rezList);
                    }
                }
            }
            return result;
        }

        public static IDictionary<int, List<RezultatUkupno>> getEkipaRezultatiUkupnoMap(
            RezultatskoTakmicenje rt, IList<RezultatskoTakmicenje> svaRezTakmicenja, DeoTakmicenjaKod deoTakKod)
        {
            IList<RezultatskoTakmicenje> list = new List<RezultatskoTakmicenje>();
            list.Add(rt);
            return getEkipaRezultatiUkupnoMap(list, svaRezTakmicenja, deoTakKod);
        }

        public static IDictionary<int, List<RezultatUkupno>> getEkipaRezultatiUkupnoMap(
            IList<RezultatskoTakmicenje> rezTakmicenja, IList<RezultatskoTakmicenje> svaRezTakmicenja,
            DeoTakmicenjaKod deoTakKod)
        {
            IDictionary<int, List<RezultatUkupno>> result = new Dictionary<int, List<RezultatUkupno>>();

            // Obradi najpre cest specijalan slucaj gde postoje ekipe ali ne postoje clanovi ekipe

            bool postojeClanovi = false;
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                IList<Ekipa> ekipe;
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                    ekipe = new List<Ekipa>(rt.Takmicenje1.Ekipe);
                else
                    ekipe = rt.Takmicenje4.getUcesnici();

                foreach (Ekipa e in ekipe)
                {
                    if (e.Gimnasticari.Count == 0)
                        result.Add(e.Id, new List<RezultatUkupno>());
                    else
                    {
                        postojeClanovi = true;
                        break;
                    }
                }
                if (postojeClanovi)
                    break;
            }
            if (!postojeClanovi)
                return result;

            // Obradi generalan slucaj

            result.Clear();
            
            // Napravi mapu svih rezultata ukupno
            IDictionary<int, IList<Pair<RezultatskoTakmicenje, RezultatUkupno>>> gimRezUkupnoMap
                = getGimRezUkupnoMap(svaRezTakmicenja, deoTakKod);

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                IList<Ekipa> ekipe;
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                    ekipe = new List<Ekipa>(rt.Takmicenje1.Ekipe);
                else
                    ekipe = rt.Takmicenje4.getUcesnici();

                foreach (Ekipa e in ekipe)
                    result.Add(e.Id, findRezultatiUkupnoForEkipa(e, gimRezUkupnoMap, rt.Gimnastika));
            }       
            return result;
        }

        public static List<RezultatUkupno> findRezultatiUkupnoForEkipa(Ekipa e,
            IDictionary<int, IList<Pair<RezultatskoTakmicenje, RezultatUkupno>>> gimRezUkupnoMap,
            Gimnastika gimnastika)
        {
            List<RezultatUkupno> result = new List<RezultatUkupno>();
            foreach (GimnasticarUcesnik g in e.Gimnasticari)
            {
                if (!gimRezUkupnoMap.ContainsKey(g.Id))
                {
                    string msgFmt;
                    if (gimnastika == Gimnastika.MSG)
                        msgFmt = "Gimnasticar '{0}' ({1}) je clan ekipe '{2}' a nije dodat u 'Takmicari - takmicenja'.";
                    else
                        msgFmt = "Gimnasticarka '{0}' ({1}) je clanica ekipe '{2}' a nije dodata u 'Takmicari - takmicenja'.";
                    string msg = String.Format(msgFmt, g.ImeSrednjeImePrezime, g.TakmicarskaKategorija, e.Naziv);
                    throw new BusinessException(msg);
                }
                result.Add(getRezultatUkupnoForEkipniRezultat(g, gimRezUkupnoMap[g.Id]));
            }
            return result;
        }

        public static RezultatUkupno getRezultatUkupnoForEkipniRezultat(GimnasticarUcesnik g,
            IList<Pair<RezultatskoTakmicenje, RezultatUkupno>> rezultati)
        {
            RezultatUkupno result;
            if (rezultati.Count == 0)
            {
                result = new RezultatUkupno();
                result.Gimnasticar = g;
            }
            else if (rezultati.Count == 1)
                result = rezultati[0].Second;
            else
            {
                // TODO4: Gimnasticar je ucestvovao na vise rez. takmicenja. Ovde bi najpre trebalo
                // proveriti da li su propozicije za sva ta rez. takmicenja ista, sto se tice viseboja
                // (npr. treba proveriti da li se preskok racuna na osnovu prvog ili boljeg skoka).
                // Ako su propozicije iste, moze se uzeti bilo koji RezultatUkupno. Ako propozicije nisu
                // iste, treba uzeti rezultate (ako postoje) iz rez. takmicenja rt (ciju ekipu trenutno
                // gledamo), ili iz rez. takmicenja ciji description je isti kao i za rt. Ako ni to nije
                // moguce, treba uzeti bilo koje rezultate (ako postoje), uz eventualno obavestenje
                // korsniku da je gimnasticar clan ekipe a nije ucestvovao u tom takmicenje descriptionu.

                result = rezultati[0].Second;
            }
            return result;
        }

        public virtual void kreirajRezultateViseKola(IList<RezultatskoTakmicenje> rezTakmicenja,
            List<IList<RezultatskoTakmicenje>> rezTakmicenjaPrethodnaKola)
        {
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                RezultatskoTakmicenje rezTak1 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], 0, rt.Kategorija);
                RezultatskoTakmicenje rezTak2 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], 0, rt.Kategorija);
                RezultatskoTakmicenje rezTak3 = null;
                RezultatskoTakmicenje rezTak4 = null;
                if (TreceKolo != null)
                    rezTak3 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[2], 0, rt.Kategorija);
                if (CetvrtoKolo != null)
                    rezTak4 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[3], 0, rt.Kategorija);

                if (FinaleKupa)
                {
                    rt.Takmicenje1.PoredakUkupnoFinaleKupa.create(rt, rezTak1, rezTak2);

                    foreach (PoredakSpravaFinaleKupa p in rt.Takmicenje1.PoredakSpravaFinaleKupa)
                        p.create(rt, rezTak1, rezTak2);
                    rt.Takmicenje1.PoredakPreskokFinaleKupa.create(rt, rezTak1, rezTak2);

                    // TODO4: Obradi slucaj kombinovanog ekipnog takmicenja (na svim mestima gde se racuna).
                    rt.Takmicenje1.PoredakEkipnoFinaleKupa.create(rt, rezTak1, rezTak2);
                }
                else if (ZbirViseKola)
                {
                    rt.Takmicenje1.PoredakUkupnoZbirViseKola.create(rt, rezTak1, rezTak2, rezTak3, rezTak4);
                    rt.Takmicenje1.PoredakEkipnoZbirViseKola.create(rt, rezTak1, rezTak2, rezTak3, rezTak4);
                }
            }
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

            strBuilder.AppendLine(VrhovniSudija != null ? VrhovniSudija.Id.ToString() : NULL);

            strBuilder.AppendLine(BrojEOcena.ToString());
            strBuilder.AppendLine(BrojEOcenaTak3.ToString());
            strBuilder.AppendLine(OdbaciMinMaxEOcenu.ToString());
            strBuilder.AppendLine(OdbaciMinMaxEOcenuTak3.ToString());
            strBuilder.AppendLine(BrojDecimalaD.ToString());
            strBuilder.AppendLine(BrojDecimalaE1.ToString());
            strBuilder.AppendLine(BrojDecimalaE.ToString());
            strBuilder.AppendLine(BrojDecimalaBon.ToString());
            strBuilder.AppendLine(BrojDecimalaPen.ToString());
            strBuilder.AppendLine(BrojDecimalaTotal.ToString());
            strBuilder.AppendLine(ZavrsenoTak1.ToString());
            strBuilder.AppendLine(ZrebZaFinalePoSpravama != null ? ZrebZaFinalePoSpravama : NULL);
            strBuilder.AppendLine(Logo1RelPath != null ? Logo1RelPath : NULL);
            strBuilder.AppendLine(Logo2RelPath != null ? Logo2RelPath : NULL);
            strBuilder.AppendLine(Logo3RelPath != null ? Logo3RelPath : NULL);
            strBuilder.AppendLine(Logo4RelPath != null ? Logo4RelPath : NULL);
            strBuilder.AppendLine(Logo5RelPath != null ? Logo5RelPath : NULL);
            strBuilder.AppendLine(Logo6RelPath != null ? Logo6RelPath : NULL);
            strBuilder.AppendLine(Logo7RelPath != null ? Logo7RelPath : NULL);
            strBuilder.AppendLine(LastModified.ToString());
            strBuilder.AppendLine(TakBrojevi.ToString());

            strBuilder.AppendLine(TakmicenjeDescriptions.Count.ToString());
            foreach (RezultatskoTakmicenjeDescription d in TakmicenjeDescriptions)
                d.dump(strBuilder);

            strBuilder.AppendLine(Kategorije.Count.ToString());
            foreach (TakmicarskaKategorija k in Kategorije)
                k.dump(strBuilder);
        }

        // TODO4: Ovaj metod se koristi za parsiranje dampovanih datuma, i uveden je zato sto srpski format za datum
        // nema tacku iza godine, a hrvatski ima. Proveri da li treba da se poziva na jos nekim mestima (osim ovih gde se
        // trenutno poziva). Proveri i da li sve moze jednostavnije i brze da se uradi.
        public static DateTime ParsirajDatum(string dateString)
        {
            DateTime result = new DateTime();
            CultureInfo[] cultures = {
                CultureInfo.CreateSpecificCulture("sr-Cyrl-RS"), 
                CultureInfo.CreateSpecificCulture("sr-Cyrl-ME"), 
                CultureInfo.CreateSpecificCulture("sr-Cyrl-CS"), 
                CultureInfo.CreateSpecificCulture("sr-Cyrl-BA"), 

                CultureInfo.CreateSpecificCulture("sr-Latn-RS"), 
                CultureInfo.CreateSpecificCulture("sr-Latn-ME"), 
                CultureInfo.CreateSpecificCulture("sr-Latn-CS"), 
                CultureInfo.CreateSpecificCulture("sr-Latn-BA"), 

                CultureInfo.CreateSpecificCulture("hr-HR"),
                CultureInfo.CreateSpecificCulture("hr-BA"),                
                CultureInfo.CreateSpecificCulture("bs-Cyrl-BA"),
                CultureInfo.CreateSpecificCulture("bs-Latn-BA"),
                System.Threading.Thread.CurrentThread.CurrentCulture
            };
            //CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            bool ok = false;
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    result = DateTime.Parse(dateString, culture);
                    ok = true;
                    break;
                }
                catch (FormatException)
                {
                }
            }
            if (!ok)
            {
                throw new Exception("Pogresan format za datum.");
            }
            return result;
        }

        public virtual void loadFromDump(StringReader reader, IdMap map, out int prvoKoloId, out int drugoKoloId,
            out int treceKoloId, out int cetvrtoKoloId, out int vrhovniSudijaId)
        {
            string naziv = reader.ReadLine();
            Naziv = naziv != NULL ? naziv : null;            
            Gimnastika = (Gimnastika)Enum.Parse(typeof(Gimnastika), reader.ReadLine());
            Datum = ParsirajDatum(reader.ReadLine());
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

            string vrhovniSudijaIdStr = reader.ReadLine();
            vrhovniSudijaId = vrhovniSudijaIdStr != NULL ? int.Parse(vrhovniSudijaIdStr) : -1;

            BrojEOcena = byte.Parse(reader.ReadLine());
            BrojEOcenaTak3 = byte.Parse(reader.ReadLine());
            OdbaciMinMaxEOcenu = bool.Parse(reader.ReadLine());
            OdbaciMinMaxEOcenuTak3 = bool.Parse(reader.ReadLine());
            BrojDecimalaD = byte.Parse(reader.ReadLine());
            BrojDecimalaE1 = byte.Parse(reader.ReadLine());
            BrojDecimalaE = byte.Parse(reader.ReadLine());
            BrojDecimalaBon = byte.Parse(reader.ReadLine());
            BrojDecimalaPen = byte.Parse(reader.ReadLine());
            BrojDecimalaTotal = byte.Parse(reader.ReadLine());
            ZavrsenoTak1 = bool.Parse(reader.ReadLine());

            string zreb = reader.ReadLine();
            ZrebZaFinalePoSpravama = zreb != NULL ? zreb : null;

            // TODO5: Proveri sta se desava ako dumpujem empty string (dakle, koji nije null).
            string logo = reader.ReadLine();
            Logo1RelPath = logo != NULL ? logo : null;
            logo = reader.ReadLine();
            Logo2RelPath = logo != NULL ? logo : null;
            logo = reader.ReadLine();
            Logo3RelPath = logo != NULL ? logo : null;
            logo = reader.ReadLine();
            Logo4RelPath = logo != NULL ? logo : null;
            logo = reader.ReadLine();
            Logo5RelPath = logo != NULL ? logo : null;
            logo = reader.ReadLine();
            Logo6RelPath = logo != NULL ? logo : null;
            logo = reader.ReadLine();
            Logo7RelPath = logo != NULL ? logo : null;

            LastModified = ParsirajDatum(reader.ReadLine());
            TakBrojevi = bool.Parse(reader.ReadLine());

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
