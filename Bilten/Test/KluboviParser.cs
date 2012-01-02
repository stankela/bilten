using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bilten.Test
{
    public class KluboviParser
    {
        private List<object[]> klubovi;
        public List<object[]> Klubovi
        {
            get { return klubovi; }
        }

        public KluboviParser()
        { 
        
        }

        public void parse(string fileName)
        {
            klubovi = new List<object[]>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0)
                        continue;

                    string naziv;
                    string mesto;
                    string kod;

                    parseKlub(line, out naziv, out mesto, out kod);
                    klubovi.Add(new object[] { naziv, mesto, kod });
                }
            }
        }

        private void parseKlub(string line, out string naziv, out string mesto,
            out string kod)
        {
            string[] parts = line.Split('\t');

            int i = 0;
            naziv = parts[i++].Trim();

            while (parts[i].Trim() == String.Empty)
                i++;
            mesto = parts[i++].Trim();

            while (parts[i].Trim() == String.Empty)
                i++;
            kod = parts[i++].Trim();
        }
    }
}
