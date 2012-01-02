using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Bilten.Domain;

namespace Bilten.Test
{
    public class OceneParser
    {
        private List<object[]> ocene;
        public List<object[]> Ocene
        {
            get { return ocene; }
        }

        private Dictionary<int, GimnasticarUcesnik> gimnasticariMap = new Dictionary<int,GimnasticarUcesnik>();

        public OceneParser(Dictionary<int, GimnasticarUcesnik> gimnasticariMap)
        {
            this.gimnasticariMap = gimnasticariMap;
        }

        public void parse(string fileName)
        {
            ocene = new List<object[]>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                string nazivSprave = String.Empty;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() == String.Empty)
                        continue;

                    if (!Char.IsDigit(line[0]) && !Char.IsWhiteSpace(line[0]))
                    {
                        nazivSprave = line.Trim();
                        continue;
                    }

                    string rank;                    
                    string broj;
                    string brojPreskoka;
                    string vreme;
                    string D;
                    string E1;
                    string E2;
                    string E3;
                    string E4;
                    string E5;
                    string E6;
                    string E;
                    string pen;
                    string L;
                    string T;
                    string O;
                    string preskok;
                    string total;

                    parseOcena(line, out rank, out broj, out brojPreskoka,
                        out vreme, out D, out E1, out E2, out E3, out E4, out E5, out E6, out E,
                        out pen, out L, out T, out O, out preskok, out total);
                    ocene.Add(new object[] { nazivSprave, rank, broj, brojPreskoka, vreme, D, E1, E2,
                        E3, E4, E5, E6, E, pen, L, T, O, preskok, total});
                }
            }
        }

        private void parseOcena(string line, out string rank, out string broj,
            out string brojPreskoka, out string vreme, out string D, out string E1, out string E2,
            out string E3, out string E4, out string E5, out string E6, out string E,
            out string pen, out string L, out string T, out string O,
            out string preskok, out string total)
        {
            rank = String.Empty;
            broj = String.Empty;
            int index;
            if (Char.IsDigit(line[0]))
            {
                index = line.IndexOf(' ');
                rank = line.Substring(0, index);
                line = line.Remove(0, index).Trim();

                index = line.IndexOf(' ');
                broj = line.Substring(0, index);
                line = line.Remove(0, index).Trim();

                // ukloni prezime, ime i kod
                string kod = gimnasticariMap[Int32.Parse(broj)].DrzavaUcesnik.Kod;
                index = line.IndexOf(kod.ToUpper());
                line = line.Remove(0, index + 3).Trim();
            }
            line = line.Trim();

            D = String.Empty;
            brojPreskoka = String.Empty;
            vreme = String.Empty;

            index = line.IndexOf(' ');
            string brojPreskokaVremeD = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            if (brojPreskokaVremeD.IndexOf(':') != -1)
                vreme = brojPreskokaVremeD;
            else if (brojPreskokaVremeD.IndexOf('.') != -1)
                D = brojPreskokaVremeD;
            else
                brojPreskoka = brojPreskokaVremeD;

            if (D == String.Empty)
            {
                index = line.IndexOf(' ');
                D = line.Substring(0, index);
                line = line.Remove(0, index).Trim();
            }

            index = line.IndexOf(' ');
            E1 = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            index = line.IndexOf(' ');
            E2 = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            index = line.IndexOf(' ');
            E3 = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            index = line.IndexOf(' ');
            E4 = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            index = line.IndexOf(' ');
            E5 = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            index = line.IndexOf(' ');
            E6 = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            index = line.IndexOf(' ');
            E = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            pen = String.Empty;
            L = String.Empty;
            T = String.Empty;
            O = String.Empty;
            if (line.IndexOf('(') != -1)
            {
                if (line[0] != '(')
                {
                    index = line.IndexOf(' ');
                    pen = line.Substring(0, index);
                    line = line.Remove(0, index).Trim();
                }
                line = line.Substring(1); // removes '('
                index = line.IndexOf(')');
                string LTO = line.Substring(0, index);
                line = line.Remove(0, index + 1).Trim(); // removes  LTO i ')'
                string[] parts = LTO.Split('/');
                L = parts[0];
                O = parts[parts.Length - 1];
                if (parts.Length == 3)
                    T = parts[1];
            }

            preskok = String.Empty;
            total = String.Empty;
            line = line.Trim();
            index = line.IndexOf(' ');
            if (index != -1)
            {
                preskok = line.Substring(0, index);
                total = line.Remove(0, index).Trim();
            }
            else if (brojPreskoka == String.Empty)
            {
                total = line;
            }
            else
            {
                preskok = line;
            }
        }

    }
}
