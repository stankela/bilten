using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using Bilten.Exceptions;
using System.ComponentModel;
using System.Diagnostics;

namespace Bilten.Domain
{
    public class RezultatskoTakmicenje : DomainObject
    {
        public virtual Pol Pol
        {
            get
            {
                if (Gimnastika == Gimnastika.MSG)
                    return Pol.Muski;
                else
                    return Pol.Zenski;
            }
        }

        private Takmicenje takmicenje;
        public virtual Takmicenje Takmicenje
        {
            get { return takmicenje; }
            set { takmicenje = value; }
        }

        private Gimnastika gimnastika;
        public virtual Gimnastika Gimnastika
        {
            get { return gimnastika; }
            set { gimnastika = value; }
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

        private byte redBroj;
        public virtual byte RedBroj
        {
            get { return redBroj; }
            set { redBroj = value; }
        }

        private Takmicenje1 _takmicenje1;
        public virtual Takmicenje1 Takmicenje1
        {
            get { return _takmicenje1; }
            private set { _takmicenje1 = value; }
        }

        private Takmicenje2 _takmicenje2;
        public virtual Takmicenje2 Takmicenje2
        {
            get { return _takmicenje2; }
            private set { _takmicenje2 = value; }
        }

        private Takmicenje3 _takmicenje3;
        public virtual Takmicenje3 Takmicenje3
        {
            get { return _takmicenje3; }
            private set { _takmicenje3 = value; }
        }

        private Takmicenje4 _takmicenje4;
        public virtual Takmicenje4 Takmicenje4
        {
            get { return _takmicenje4; }
            private set { _takmicenje4 = value; }
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
            if (propozicije.PostojiTak2)
                _takmicenje2 = new Takmicenje2();
            if (propozicije.PostojiTak3)
                _takmicenje3 = new Takmicenje3(takmicenje.Gimnastika);
            if (propozicije.PostojiTak4)
                _takmicenje4 = new Takmicenje4();
        }

        public virtual void updateTakmicenjaFromChangedPropozicije(
            out bool deletedTak2, out bool deletedTak3, out bool deletedTak4)
        {
            deletedTak2 = deletedTak3 = deletedTak4 = false;

            if (Propozicije.PostojiTak2 != (Takmicenje2 != null))
            {
                if (Takmicenje2 != null)
                {
                    // postojalo je takmicenje II, ali je u novim propozicijama
                    // PostojiTak2 postavljeno na false
                    Takmicenje2 = null;
                    deletedTak2 = true;
                }
                else
                {
                    // nije postojalo takmicenje II, a u novim propozicijama je
                    // PostojiTak2 postavljeno na true
                    Takmicenje2 = new Takmicenje2();
                }
            }
            if (Propozicije.PostojiTak3 != (Takmicenje3 != null))
            {
                if (Takmicenje3 != null)
                {
                    Takmicenje3 = null;
                    deletedTak3 = true;
                }
                else
                {
                    Takmicenje3 = new Takmicenje3(this.Gimnastika);
                }
            }
            if (Propozicije.PostojiTak4 != (Takmicenje4 != null))
            {
                if (Takmicenje4 != null)
                {
                    Takmicenje4 = null;
                    deletedTak4 = true;
                }
                else
                {
                    Takmicenje4 = new Takmicenje4();
                }
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

        // sledeca dva svojstva omogucuju da se sortiranjem kreira RedBroj
        public virtual byte KategorijaRedBroj
        {
            get { return Kategorija.RedBroj; }
        }

        public virtual byte TakmicenjeDescriptionRedBroj
        {
            get { return TakmicenjeDescription.RedBroj; }
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

        public virtual PoredakSprava getPoredakSprava(DeoTakmicenjaKod deoTakKod, Sprava sprava)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.getPoredakSprava(sprava);

            Debug.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje3);
            return Takmicenje3.getPoredak(sprava);
        }

        public virtual PoredakPreskok getPoredakPreskok(DeoTakmicenjaKod deoTakKod)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return Takmicenje1.PoredakPreskok;

            Debug.Assert(deoTakKod == DeoTakmicenjaKod.Takmicenje3);
            return Takmicenje3.PoredakPreskok;
        }
    }
}
