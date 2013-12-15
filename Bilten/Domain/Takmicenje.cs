using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.Diagnostics;

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

        private bool finaleKupa;
        public virtual bool FinaleKupa
        {
            get { return finaleKupa; }
            set { finaleKupa = value; }
        }

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

        private ISet<RezultatskoTakmicenjeDescription> takmicenjeDescriptions = 
            new HashedSet<RezultatskoTakmicenjeDescription>();
        public virtual ISet<RezultatskoTakmicenjeDescription> TakmicenjeDescriptions
        {
            get { return takmicenjeDescriptions; }
            private set { takmicenjeDescriptions = value; }
        }

        public virtual void addTakmicenjeDescription(RezultatskoTakmicenjeDescription desc)
        {
            if (!TakmicenjeDescriptions.Contains(desc))
            {
                TakmicenjeDescriptions.Add(desc);
                desc.RedBroj = (byte)(TakmicenjeDescriptions.Count - 1);
            }
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

        public virtual bool moveTakmicenjeDescriptionUp(
            RezultatskoTakmicenjeDescription desc)
        {
            //if (redBroj < 1 || redBroj > TakmicenjeDescriptions.Count - 1)
            //    return false;
            if (desc.RedBroj == 0)
                return false;

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
            //if (redBroj < 0 || redBroj >= TakmicenjeDescriptions.Count - 1)
            //    return false;
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

        private ISet<TakmicarskaKategorija> kategorije =
            new HashedSet<TakmicarskaKategorija>();
        public virtual ISet<TakmicarskaKategorija> Kategorije
        {
            get { return kategorije; }
            private set { kategorije = value; }
        }

        // NOTE: Metodi addKategorija i removeKategorija upravljaju dvosmernom 
        // asocijacijom izmedju Takmicenja i TakmicarskeKategorije
        public virtual void addKategorija(TakmicarskaKategorija kat)
        {
            if (!Kategorije.Contains(kat))
            {
                Kategorije.Add(kat);
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
            Debug.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje1 || deoTakKod == DeoTakmicenjaKod.Takmicenje2);
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
                }
                else
                {
                    if (rt.Propozicije.PostojiTak2 && rt.Propozicije.OdvojenoTak2)
                        result.Add(rt);
                }
            }
            return result;
        }

        public virtual RezultatskoTakmicenje getRezTakmicenje(IList<RezultatskoTakmicenje> rezTakmicenja,
            TakmicarskaKategorija kat)
        {
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.Kategorija.Equals(kat))
                    return rezTak;
            }
            return null;
        }

    }
}
