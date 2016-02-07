using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using Bilten.Exceptions;
using System.Collections;

namespace Bilten.Domain
{
    public class Takmicenje1 : DomainObject
    {
        private Iesi.Collections.Generic.ISet<GimnasticarUcesnik> _gimnasticari = new HashedSet<GimnasticarUcesnik>();
        public virtual Iesi.Collections.Generic.ISet<GimnasticarUcesnik> Gimnasticari
        {
            get { return _gimnasticari; }
            protected set { _gimnasticari = value; }
        }

        public virtual void addGimnasticar(GimnasticarUcesnik gimnasticar)
        {
            Gimnasticari.Add(gimnasticar);
        }

        public virtual void removeGimnasticar(GimnasticarUcesnik gimnasticar)
        {
            Gimnasticari.Remove(gimnasticar);
        }

        // TODO: Za sve klasa koje se cuvaju u setovima trebalo bi implementirati
        // Equals i GetHashCode
        private Iesi.Collections.Generic.ISet<Ekipa> ekipe = new HashedSet<Ekipa>();
        public virtual Iesi.Collections.Generic.ISet<Ekipa> Ekipe
        {
            get { return ekipe; }
            protected set { ekipe = value; }
        }

        public virtual void addEkipa(Ekipa ekipa)
        {
            checkAdd(ekipa);
            Ekipe.Add(ekipa);
        }

        private void checkAdd(Ekipa ekipa)
        {
            if (existsEkipaNaziv(ekipa.Naziv))
                throw new BusinessException("Naziv", "Ekipa sa datim nazivom vec postoji.");
            if (existsEkipaKod(ekipa.Kod))
                throw new BusinessException("Kod", "Ekipa sa datim kodom vec postoji.");
        }

        private bool existsEkipaNaziv(string naziv)
        {
            foreach (Ekipa e in Ekipe)
            {
                if (e.Naziv.ToUpper() == naziv.ToUpper())
                    return true;
            }
            return false;
        }

        private bool existsEkipaKod(string kod)
        {
            foreach (Ekipa e in Ekipe)
            {
                if (e.Kod.ToUpper() == kod.ToUpper())
                    return true;
            }
            return false;
        }

        public virtual bool canAddEkipa(Ekipa ekipa)
        {
            try
            {
                checkAdd(ekipa);
                return true;
            }
            catch (BusinessException)
            {
                return false;
            }
        }

        public virtual void removeEkipa(Ekipa ekipa)
        {
            Ekipe.Remove(ekipa);
        }

        private PoredakUkupno _poredakUkupno;
        public virtual PoredakUkupno PoredakUkupno
        {
            get { return _poredakUkupno; }
            set { _poredakUkupno = value; }
        }

        private Iesi.Collections.Generic.ISet<PoredakSprava> _poredakSprava = new HashedSet<PoredakSprava>();
        public virtual Iesi.Collections.Generic.ISet<PoredakSprava> PoredakSprava
        {
            get { return _poredakSprava; }
            set { _poredakSprava = value; }
        }

        public virtual PoredakSprava getPoredakSprava(Sprava sprava)
        {
            foreach (PoredakSprava p in PoredakSprava)
            {
                if (p.Sprava == sprava)
                    return p;
            }
            return null;
        }

        private PoredakPreskok _poredakPreskok;
        public virtual PoredakPreskok PoredakPreskok
        {
            get { return _poredakPreskok; }
            set { _poredakPreskok = value; }
        }

        private PoredakEkipno _poredakEkipno;
        public virtual PoredakEkipno PoredakEkipno
        {
            get { return _poredakEkipno; }
            set { _poredakEkipno = value; }
        }

        private PoredakUkupnoFinaleKupa _poredakUkupnoFinaleKupa = new PoredakUkupnoFinaleKupa();
        public virtual PoredakUkupnoFinaleKupa PoredakUkupnoFinaleKupa
        {
            get { return _poredakUkupnoFinaleKupa; }
            set { _poredakUkupnoFinaleKupa = value; }
        }

        private PoredakUkupnoZbirViseKola _poredakUkupnoZbirViseKola = new PoredakUkupnoZbirViseKola();
        public virtual PoredakUkupnoZbirViseKola PoredakUkupnoZbirViseKola
        {
            get { return _poredakUkupnoZbirViseKola; }
            set { _poredakUkupnoZbirViseKola = value; }
        }

        private Iesi.Collections.Generic.ISet<PoredakSpravaFinaleKupa> _poredakSpravaFinaleKupa = new HashedSet<PoredakSpravaFinaleKupa>();
        public virtual Iesi.Collections.Generic.ISet<PoredakSpravaFinaleKupa> PoredakSpravaFinaleKupa
        {
            get { return _poredakSpravaFinaleKupa; }
            set { _poredakSpravaFinaleKupa = value; }
        }

        public virtual PoredakSpravaFinaleKupa getPoredakSpravaFinaleKupa(Sprava sprava)
        {
            foreach (PoredakSpravaFinaleKupa p in PoredakSpravaFinaleKupa)
            {
                if (p.Sprava == sprava)
                    return p;
            }
            return null;
        }

        private PoredakEkipnoFinaleKupa _poredakEkipnoFinaleKupa = new PoredakEkipnoFinaleKupa();
        public virtual PoredakEkipnoFinaleKupa PoredakEkipnoFinaleKupa
        {
            get { return _poredakEkipnoFinaleKupa; }
            set { _poredakEkipnoFinaleKupa = value; }
        }

        private PoredakEkipnoZbirViseKola _poredakEkipnoZbirViseKola = new PoredakEkipnoZbirViseKola();
        public virtual PoredakEkipnoZbirViseKola PoredakEkipnoZbirViseKola
        {
            get { return _poredakEkipnoZbirViseKola; }
            set { _poredakEkipnoZbirViseKola = value; }
        }

        protected Takmicenje1()
        { 
        
        }

        public Takmicenje1(Gimnastika gimnastika)
        {
            _poredakUkupno = new PoredakUkupno(DeoTakmicenjaKod.Takmicenje1);
            foreach (Sprava s in Sprave.getSprave(gimnastika))
            {
                if (s != Sprava.Preskok)
                    _poredakSprava.Add(new PoredakSprava(DeoTakmicenjaKod.Takmicenje1, s));
            }
            _poredakPreskok = new PoredakPreskok(DeoTakmicenjaKod.Takmicenje1);
            _poredakEkipno = new PoredakEkipno(DeoTakmicenjaKod.Takmicenje1);

        }

        public virtual void initPoredakSpravaFinaleKupa(Gimnastika gimnastika)
        {
            foreach (Sprava s in Sprave.getSprave(gimnastika))
            {
                PoredakSpravaFinaleKupa poredak = new PoredakSpravaFinaleKupa();
                poredak.Sprava = s;
                _poredakSpravaFinaleKupa.Add(poredak);
            }
        }

        public virtual void ocenaAdded(Ocena o, RezultatskoTakmicenje rezTak)
        {
            if (Gimnasticari.Contains(o.Gimnasticar))
            {
                PoredakUkupno.addOcena(o, rezTak);
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.addOcena(o, rezTak, true);
                else
                    getPoredakSprava(o.Sprava).addOcena(o, rezTak, true);
            }

            // Ova naredba treba da bude unutar if izraza ako clanovi ekipe
            // mogu da budu samo gimnasticari ucesnici istog rez. takmicenja
            PoredakEkipno.addOcena(o, rezTak);
        }

        public virtual void ocenaDeleted(Ocena o, RezultatskoTakmicenje rezTak)
        {
            if (Gimnasticari.Contains(o.Gimnasticar))
            {
                PoredakUkupno.deleteOcena(o, rezTak);
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.deleteOcena(o, rezTak, true);
                else
                    getPoredakSprava(o.Sprava).deleteOcena(o, rezTak, true);
            }

            PoredakEkipno.deleteOcena(o, rezTak);
        }

        public virtual void ocenaEdited(Ocena o, Ocena old, RezultatskoTakmicenje rezTak)
        {
            if (Gimnasticari.Contains(o.Gimnasticar))
            {
                PoredakUkupno.editOcena(o, old, rezTak);
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.editOcena(o, rezTak);
                else
                    getPoredakSprava(o.Sprava).editOcena(o, rezTak);
            }

            PoredakEkipno.editOcena(o, old, rezTak);
        }

        public virtual void gimnasticarAdded(GimnasticarUcesnik g, IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak)
        {
            PoredakUkupno.addGimnasticar(g, ocene, rezTak);
            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.addGimnasticar(g, o, rezTak);
                else
                    getPoredakSprava(o.Sprava).addGimnasticar(g, o, rezTak);
            }
        }

        public virtual void gimnasticarDeleted(GimnasticarUcesnik g, IList vezbaneSprave,
            RezultatskoTakmicenje rezTak)
        {
            PoredakUkupno.deleteGimnasticar(g, rezTak);
            foreach (Sprava s in vezbaneSprave)
            {
                if (s == Sprava.Preskok)
                    PoredakPreskok.deleteGimnasticar(g, rezTak);
                else
                    getPoredakSprava(s).deleteGimnasticar(g, rezTak);
            }
        }

        public virtual void ekipaAdded(Ekipa e, IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak)
        {
            PoredakEkipno.addEkipa(e, ocene, rezTak);
        }

        public virtual void ekipaDeleted(Ekipa e, RezultatskoTakmicenje rezTak)
        {
            PoredakEkipno.deleteEkipa(e, rezTak);
        }

        public virtual void gimnasticarAddedToEkipa(GimnasticarUcesnik g, Ekipa e, 
            IList<Ocena> ocene, RezultatskoTakmicenje rezTak)
        {
            if (Ekipe.Contains(e))
                PoredakEkipno.gimnasticarAddedToEkipa(g, e, ocene, rezTak);
        }

        public virtual void gimnasticarDeletedFromEkipa(GimnasticarUcesnik g, Ekipa e,
            IList<Ocena> ocene, RezultatskoTakmicenje rezTak)
        {
            if (Ekipe.Contains(e))
                PoredakEkipno.gimnasticarDeletedFromEkipa(g, e, ocene, rezTak);
        }

    }
}
