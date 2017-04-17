using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using Bilten.Exceptions;
using System.Collections;
using System.IO;

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

        public virtual bool addGimnasticar(GimnasticarUcesnik gimnasticar)
        {
            return Gimnasticari.Add(gimnasticar);
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

        public virtual bool addEkipa(Ekipa ekipa)
        {
            if (existsEkipaKod(ekipa.Kod))
                throw new BusinessException("Kod", "Ekipa sa datim kodom vec postoji.");
            return Ekipe.Add(ekipa);
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

        // TODO4: Proveri da li je potrebno da inicijalizujem poredak sa novim objektom (za standardne poretke to nisam
        // radio). Isto i za zbir vise kola.
        private PoredakUkupnoFinaleKupa _poredakUkupnoFinaleKupa = new PoredakUkupnoFinaleKupa();
        public virtual PoredakUkupnoFinaleKupa PoredakUkupnoFinaleKupa
        {
            get { return _poredakUkupnoFinaleKupa; }
            set { _poredakUkupnoFinaleKupa = value; }
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

        private PoredakUkupnoZbirViseKola _poredakUkupnoZbirViseKola = new PoredakUkupnoZbirViseKola();
        public virtual PoredakUkupnoZbirViseKola PoredakUkupnoZbirViseKola
        {
            get { return _poredakUkupnoZbirViseKola; }
            set { _poredakUkupnoZbirViseKola = value; }
        }

        private PoredakEkipnoZbirViseKola _poredakEkipnoZbirViseKola = new PoredakEkipnoZbirViseKola();
        public virtual PoredakEkipnoZbirViseKola PoredakEkipnoZbirViseKola
        {
            get { return _poredakEkipnoZbirViseKola; }
            set { _poredakEkipnoZbirViseKola = value; }
        }

        public Takmicenje1()
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

        public virtual void ocenaAdded(Ocena o, RezultatskoTakmicenje rezTak, List<Ocena> sveOceneTak1)
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
            if (rezTak.ImaEkipnoTakmicenje)
                PoredakEkipno.addOcena(o, rezTak, sveOceneTak1);
        }

        public virtual void ocenaDeleted(Ocena o, RezultatskoTakmicenje rezTak, List<Ocena> sveOceneTak1)
        {
            if (Gimnasticari.Contains(o.Gimnasticar))
            {
                PoredakUkupno.deleteOcena(o, rezTak);
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.deleteOcena(o, rezTak, true);
                else
                    getPoredakSprava(o.Sprava).deleteOcena(o, rezTak, true);
            }

            PoredakEkipno.deleteOcena(o, rezTak, sveOceneTak1);
        }

        public virtual void ocenaEdited(Ocena o, Ocena old, RezultatskoTakmicenje rezTak, List<Ocena> sveOceneTak1)
        {
            if (Gimnasticari.Contains(o.Gimnasticar))
            {
                PoredakUkupno.editOcena(o, old, rezTak);
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.editOcena(o, rezTak);
                else
                    getPoredakSprava(o.Sprava).editOcena(o, rezTak);
            }

            PoredakEkipno.editOcena(o, old, rezTak, sveOceneTak1);
        }

        public virtual void updateRezultatiOnGimnasticarAdded(GimnasticarUcesnik g, IList<Ocena> ocene,
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

        // Za finale kupa
        public virtual void updateRezultatiOnGimnasticarAdded(GimnasticarUcesnik g, IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2)
        {
            PoredakUkupnoFinaleKupa.addGimnasticar(g, rezTak, rezTak1, rezTak2);

            foreach (Sprava s in Sprave.getSprave(rezTak.Gimnastika))
                getPoredakSpravaFinaleKupa(s).addGimnasticar(g, rezTak, rezTak1, rezTak2);
            
            if (rezTak.odvojenoTak2())
                PoredakUkupno.addGimnasticar(g, ocene, rezTak);
            if (rezTak.odvojenoTak3())
            {
                foreach (Ocena o in ocene)
                {
                    if (o.Sprava == Sprava.Preskok)
                        PoredakPreskok.addGimnasticar(g, o, rezTak);
                    else
                        getPoredakSprava(o.Sprava).addGimnasticar(g, o, rezTak);
                }
            }
        }

        // Za zbir vise kola
        public virtual void updateRezultatiOnGimnasticarAdded(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak,
            RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2, RezultatskoTakmicenje rezTak3,
            RezultatskoTakmicenje rezTak4)
        {
            PoredakUkupnoZbirViseKola.addGimnasticar(g, rezTak, rezTak1, rezTak2, rezTak3, rezTak4);
        }

        public virtual void updateRezultatiOnGimnasticarDeleted(GimnasticarUcesnik g, IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak)
        {
            PoredakUkupno.deleteGimnasticar(g, rezTak);
            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.deleteGimnasticar(g, rezTak);
                else
                    getPoredakSprava(o.Sprava).deleteGimnasticar(g, rezTak);
            }

            if (PoredakUkupnoFinaleKupa != null)
                PoredakUkupnoFinaleKupa.deleteGimnasticar(g, rezTak);
            foreach (Sprava s in Sprave.getSprave(rezTak.Gimnastika))
                getPoredakSpravaFinaleKupa(s).deleteGimnasticar(g, rezTak);

            if (PoredakUkupnoZbirViseKola != null)
                PoredakUkupnoZbirViseKola.deleteGimnasticar(g, rezTak);
        }

        public virtual void updateRezultatiOnEkipaAdded(Ekipa e, IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak)
        {
            PoredakEkipno.addEkipa(e, ocene, rezTak);
        }

        // Za finale kupa
        public virtual void updateRezultatiOnEkipaAdded(Ekipa e, IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2)
        {
            PoredakEkipnoFinaleKupa.addEkipa(e, rezTak, rezTak1, rezTak2);

            if (rezTak.odvojenoTak4())
                PoredakEkipno.addEkipa(e, ocene, rezTak);
        }

        // Za zbir vise kola
        public virtual void updateRezultatiOnEkipaAdded(Ekipa e, RezultatskoTakmicenje rezTak,
            RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2, RezultatskoTakmicenje rezTak3,
            RezultatskoTakmicenje rezTak4)
        {
            PoredakEkipnoZbirViseKola.addEkipa(e, rezTak, rezTak1, rezTak2, rezTak3, rezTak4);
        }

        public virtual void updateRezultatiOnEkipaDeleted(Ekipa e, RezultatskoTakmicenje rezTak)
        {
            PoredakEkipno.deleteEkipa(e, rezTak);
            if (PoredakEkipnoFinaleKupa != null)
                PoredakEkipnoFinaleKupa.deleteEkipa(e, rezTak);
            if (PoredakEkipnoZbirViseKola != null)
                PoredakEkipnoZbirViseKola.deleteEkipa(e, rezTak);
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

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            
            // gimnasticari
            strBuilder.AppendLine(Gimnasticari.Count.ToString());
            foreach (GimnasticarUcesnik g in Gimnasticari)
                strBuilder.AppendLine(g.Id.ToString());

            // ekipe
            strBuilder.AppendLine(Ekipe.Count.ToString());
            foreach (Ekipa e in Ekipe)
                e.dump(strBuilder);
            
            if (PoredakUkupno == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakUkupno.dump(strBuilder);

            strBuilder.AppendLine(PoredakSprava.Count.ToString());
            foreach (PoredakSprava p in PoredakSprava)
                p.dump(strBuilder);

            if (PoredakPreskok == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakPreskok.dump(strBuilder);

            if (PoredakEkipno == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakEkipno.dump(strBuilder);

            if (PoredakUkupnoFinaleKupa == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakUkupnoFinaleKupa.dump(strBuilder);

            strBuilder.AppendLine(PoredakSpravaFinaleKupa.Count.ToString());
            foreach (PoredakSpravaFinaleKupa p in PoredakSpravaFinaleKupa)
                p.dump(strBuilder);

            if (PoredakEkipnoFinaleKupa == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakEkipnoFinaleKupa.dump(strBuilder);

            if (PoredakUkupnoZbirViseKola == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakUkupnoZbirViseKola.dump(strBuilder);

            if (PoredakEkipnoZbirViseKola == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakEkipnoZbirViseKola.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            int count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
                Gimnasticari.Add(map.gimnasticariMap[int.Parse(reader.ReadLine())]);

            string id;
            count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                id = reader.ReadLine();
                Ekipa e = new Ekipa();
                map.ekipeMap.Add(int.Parse(id), e);
                e.loadFromDump(reader, map);
                Ekipe.Add(e);
            }

            id = reader.ReadLine();
            PoredakUkupno poredakUkupno = null;
            if (id != NULL)
            {
                poredakUkupno = new PoredakUkupno();
                poredakUkupno.loadFromDump(reader, map);
            }
            PoredakUkupno = poredakUkupno;

            count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                reader.ReadLine();  // id
                PoredakSprava poredakSprava = new PoredakSprava();
                poredakSprava.loadFromDump(reader, map);
                PoredakSprava.Add(poredakSprava);
            }

            id = reader.ReadLine();
            PoredakPreskok poredakPreskok = null;
            if (id != NULL)
            {
                poredakPreskok = new PoredakPreskok();
                poredakPreskok.loadFromDump(reader, map);
            }
            PoredakPreskok = poredakPreskok;

            id = reader.ReadLine();
            PoredakEkipno poredakEkipno = null;
            if (id != NULL)
            {
                poredakEkipno = new PoredakEkipno();
                poredakEkipno.loadFromDump(reader, map);
            }
            PoredakEkipno = poredakEkipno;

            id = reader.ReadLine();
            PoredakUkupnoFinaleKupa poredakUkupnoFinaleKupa = null;
            if (id != NULL)
            {
                poredakUkupnoFinaleKupa = new PoredakUkupnoFinaleKupa();
                poredakUkupnoFinaleKupa.loadFromDump(reader, map);
            }
            PoredakUkupnoFinaleKupa = poredakUkupnoFinaleKupa;

            count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                reader.ReadLine();  // id
                PoredakSpravaFinaleKupa p = new PoredakSpravaFinaleKupa();
                p.loadFromDump(reader, map);
                PoredakSpravaFinaleKupa.Add(p);
            }

            id = reader.ReadLine();
            PoredakEkipnoFinaleKupa poredakEkipnoFinaleKupa = null;
            if (id != NULL)
            {
                poredakEkipnoFinaleKupa = new PoredakEkipnoFinaleKupa();
                poredakEkipnoFinaleKupa.loadFromDump(reader, map);
            }
            PoredakEkipnoFinaleKupa = poredakEkipnoFinaleKupa;

            id = reader.ReadLine();
            PoredakUkupnoZbirViseKola poredakUkupnoZbirViseKola = null;
            if (id != NULL)
            {
                poredakUkupnoZbirViseKola = new PoredakUkupnoZbirViseKola();
                poredakUkupnoZbirViseKola.loadFromDump(reader, map);
            }
            PoredakUkupnoZbirViseKola = poredakUkupnoZbirViseKola;

            id = reader.ReadLine();
            PoredakEkipnoZbirViseKola poredakEkipnoZbirViseKola = null;
            if (id != NULL)
            {
                poredakEkipnoZbirViseKola = new PoredakEkipnoZbirViseKola();
                poredakEkipnoZbirViseKola.loadFromDump(reader, map);
            }
            PoredakEkipnoZbirViseKola = poredakEkipnoZbirViseKola;
        }
    }
}
