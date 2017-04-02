using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Exceptions;
using System.IO;

namespace Bilten.Domain
{
    public class SudijskiOdborNaSpravi : DomainObject
    {
        private Sprava sprava;
        public virtual Sprava Sprava
        {
            get { return sprava; }
            set { sprava = value; }
        }

        // NOTE: Stavio sam byte umesto SudijskaUloga zato sto inace daje gresku
        private IDictionary<byte, SudijaUcesnik> sudije =
            new Dictionary<byte, SudijaUcesnik>();
        public virtual IDictionary<byte, SudijaUcesnik> Sudije
        {
            get { return sudije; }
            protected set { sudije = value; }
        }

        public virtual void addSudija(SudijaUcesnik sudija, SudijskaUloga uloga)
        {
            if (!hasFunction(uloga))
                throw new BusinessException(String.Format(
                    "Funkcija {0} nije dozvoljena.", SudijskeUloge.toString(uloga)));
            if (getSudija(uloga) != null)
                throw new BusinessException(String.Format(
                    "Vec postoji sudija sa funkcijom {0}.", SudijskeUloge.toString(uloga)));
            /*if (sudijaExists(sudija))
                throw new BusinessException(
                    String.Format("Sudija {0} je vec clan sudijskog odbora.", sudija));*/

            doAddSudija(sudija, uloga);
        }

        private void doAddSudija(SudijaUcesnik sudija, SudijskaUloga uloga)
        {
            // NOTE: Bitno je da se koristi svojstvo Sudije a ne polje sudije
            // zbog lazy inicijalizacije
            Sudije.Add((byte)uloga, sudija);
            addToRaspored(sudija, uloga);
        }

        private void addToRaspored(SudijaUcesnik sudija, SudijskaUloga uloga)
        {
            // bitno je da se koristi svojstvo Raspored a ne polje raspored zato sto
            // se Raspored lazy inicijalizuje
            foreach (SudijaNaSpravi s in Raspored)
            {
                if (s.Uloga == uloga)
                {
                    s.Sudija = sudija;
                    return;
                }
            }
        }

        public virtual void setSupportedUloge(byte brojDSudija, bool has_d1_e1, bool has_d2_e2, byte brojESudija,
            byte brojMerVremena, byte brojLinSudija)
        {
            BrojDSudija = brojDSudija;
            HasD1_E1 = has_d1_e1;
            HasD2_E2 = has_d2_e2;
            BrojESudija = brojESudija;
            BrojMeracaVremena = brojMerVremena;
            BrojLinijskihSudija = brojLinSudija;

            List<SudijskaUloga> noveUloge = new List<SudijskaUloga>(
                SudijskeUloge.getUloge(brojDSudija, has_d1_e1, has_d2_e2, brojESudija, brojMerVremena, 
                brojLinSudija));

            // ukloni sudije sa ulogama koje ne postoje u novim ulogama

            byte[] uloge = new byte[Sudije.Count];
            Sudije.Keys.CopyTo(uloge, 0);
            foreach (byte u in uloge)
            {
                if (noveUloge.IndexOf((SudijskaUloga)u) < 0)
                    Sudije.Remove(u);
            }

            createSupportedUloge();
            createRaspored();
        }

        public virtual bool hasFunction(SudijskaUloga uloga)
        {
            return SupportedUloge.IndexOf(uloga) >= 0;
        }

        public virtual bool canAddSudija(SudijaUcesnik sudija)
        {
            return /*!sudijaExists(sudija) &&*/ !isComplete();
        }

        public virtual void addSudija(SudijaUcesnik sudija)
        {
            /*if (sudijaExists(sudija))
                throw new BusinessException(
                    String.Format("Sudija {0} je vec clan sudijskog odbora.", sudija));*/
            if (isComplete())
                throw new BusinessException("Sudijski odbor je vec kompletiran.");

            doAddSudija(sudija, getFirstEmptyUloga());
        }

        private SudijskaUloga getFirstEmptyUloga()
        {
            foreach (SudijaNaSpravi s in this.Raspored)
            {
                if (s.Sudija == null)
                    return s.Uloga;
            }
            return SudijskaUloga.Undefined;
        }

        private SudijaUcesnik getSudija(SudijskaUloga uloga)
        {
            if (Sudije.ContainsKey((byte)uloga))
                return Sudije[(byte)uloga];
            else
                return null;
        }

        private bool sudijaExists(SudijaUcesnik sudija)
        {
            foreach (SudijaUcesnik s in Sudije.Values)
            {
                if (s.Equals(sudija))
                    return true;
            }
            return false;
        }

        public virtual bool removeSudija(SudijskaUloga uloga)
        {
            if (Sudije.Remove((byte)uloga))
            {
                removeSudijaFromRaspored(uloga);
                return true;
            }
            return false;
        }

        private void removeSudijaFromRaspored(SudijskaUloga uloga)
        {
            foreach (SudijaNaSpravi s in this.Raspored)
            {
                if (s.Uloga == uloga)
                {
                    s.Sudija = null;
                    return;
                }
            }
        }

        private List<SudijaNaSpravi> raspored;
        public virtual IList<SudijaNaSpravi> Raspored
        { 
            get
            {
                if (raspored == null)
                    createRaspored();
                return raspored;
            }
        }

        private void createRaspored()
        {
            raspored = new List<SudijaNaSpravi>();

            foreach (SudijskaUloga uloga in SupportedUloge)
            {
                SudijaNaSpravi sudija = new SudijaNaSpravi();
                sudija.Uloga = uloga;
                if (Sudije.ContainsKey((byte)uloga))
                    sudija.Sudija = Sudije[(byte)uloga];
                raspored.Add(sudija);
            }

            raspored.Sort(
                delegate(SudijaNaSpravi s1, SudijaNaSpravi s2)
                {
                    return s1.Uloga.CompareTo(s2.Uloga);
                });
        }

        private byte brojDSudija = 2;
        public virtual byte BrojDSudija
        {
            get { return brojDSudija; }
            protected set { brojDSudija = value; }
        }

        private bool hasD1_E1 = false;
        public virtual bool HasD1_E1
        {
            get { return hasD1_E1; }
            protected set { hasD1_E1 = value; }
        }

        private bool hasD2_E2 = false;
        public virtual bool HasD2_E2
        {
            get { return hasD2_E2; }
            protected set { hasD2_E2 = value; }
        }

        private byte brojESudija = 6;
        public virtual byte BrojESudija
        {
            get { return brojESudija; }
            protected set { brojESudija = value; }
        }

        private byte brojLinijskihSudija = 0;
        public virtual byte BrojLinijskihSudija
        {
            get { return brojLinijskihSudija; }
            protected set { brojLinijskihSudija = value; }
        }

        private byte brojMeracaVremena = 0;
        public virtual byte BrojMeracaVremena
        {
            get { return brojMeracaVremena; }
            protected set { brojMeracaVremena = value; }
        }

        public SudijskiOdborNaSpravi()
        {

        }

        public SudijskiOdborNaSpravi(Sprava sprava)
        {
            this.sprava = sprava;
        }

        private List<SudijskaUloga> _supportedUloge;
        private List<SudijskaUloga> SupportedUloge
        {
            get
            {
                if (_supportedUloge == null)
                    createSupportedUloge();
                return _supportedUloge;
            }
        }

        private void createSupportedUloge()
        {
            List<SudijskaUloga> uloge = SudijskeUloge.getUloge(BrojDSudija, HasD1_E1, HasD2_E2, BrojESudija,
                BrojMeracaVremena, BrojLinijskihSudija);
            
            _supportedUloge = new List<SudijskaUloga>();
            foreach (SudijskaUloga u in uloge)
                _supportedUloge.Add(u);
        }

        public virtual bool isComplete()
        {
            return Sudije.Count == SupportedUloge.Count;
        }

        public virtual bool isEmpty()
        {
            return Sudije.Count == 0;
        }

        public virtual void clearSudije()
        {
            Sudije.Clear();
            clearRaspored();
        }

        private void clearRaspored()
        {
            foreach (SudijaNaSpravi s in Raspored)
                s.Sudija = null;
        }

        public virtual bool moveSudijaUp(int index)
        {
            if (index < 1 || index >= Raspored.Count || Raspored[index].Sudija == null)
                return false;

            swapSudije(Raspored[index - 1], Raspored[index]);
            return true;
        }

        private void swapSudije(SudijaNaSpravi prevSudija, SudijaNaSpravi sudija)
        {
            SudijaUcesnik tmp = prevSudija.Sudija;
            prevSudija.Sudija = sudija.Sudija;
            sudija.Sudija = tmp;

            recreateSudijeFromRaspored();
        }

        private void recreateSudijeFromRaspored()
        {
            Sudije.Clear();
            foreach (SudijaNaSpravi s in Raspored)
            {
                if (s.Sudija != null)
                    Sudije[(byte)s.Uloga] = s.Sudija;
            }
        }

        public virtual bool moveSudijaDown(int index)
        {
            if (index < 0 || index >= Raspored.Count - 1 || Raspored[index].Sudija == null)
                return false;

            swapSudije(Raspored[index], Raspored[index + 1]);
            return true;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Sprava.ToString());
            strBuilder.AppendLine(BrojDSudija.ToString());
            strBuilder.AppendLine(HasD1_E1.ToString());
            strBuilder.AppendLine(HasD2_E2.ToString());
            strBuilder.AppendLine(BrojESudija.ToString());

            strBuilder.AppendLine(Sudije.Count.ToString());
            IEnumerator<KeyValuePair<byte, SudijaUcesnik>> e = Sudije.GetEnumerator();
            while (e.MoveNext())
            {
                strBuilder.AppendLine(e.Current.Key.ToString());
                strBuilder.AppendLine(e.Current.Value.Id.ToString());
            }
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());
            BrojDSudija = byte.Parse(reader.ReadLine());
            HasD1_E1 = bool.Parse(reader.ReadLine());
            HasD2_E2 = bool.Parse(reader.ReadLine());
            BrojESudija = byte.Parse(reader.ReadLine());

            int count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                byte key = byte.Parse(reader.ReadLine());
                SudijaUcesnik s = map.sudijeMap[int.Parse(reader.ReadLine())];
                Sudije.Add(key, s);
            }
        }
    }
}
