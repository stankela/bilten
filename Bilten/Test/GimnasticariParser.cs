using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.IO;

namespace Bilten.Test
{
    public class GimnasticariParser
    {
        private List<object[]> gimnasticari;
        public List<object[]> Gimnasticari
        {
            get { return gimnasticari; }
        }

        public GimnasticariParser()
        {

        }

        public void parse(string fileName)
        {
            gimnasticari = new List<object[]>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0)
                        continue;

                    int broj;
                    string prezime;
                    string ime;
                    string kod;
                    DateTime datumRodj;

                    parseGimnasticar(line, out broj, out prezime, out ime, out kod,
                        out datumRodj);
                    gimnasticari.Add(new object[] { broj, prezime, ime, kod, datumRodj });
                }
            }
        }

        private void parseGimnasticar(string line, out int broj, out string prezime,
            out string ime, out string kod, out DateTime datumRodj)
        {
            string[] parts = line.Split('\t');

            int i = 0;
            while (parts[i].Trim() == String.Empty)
                i++;
            broj = Int32.Parse(parts[i].Trim().Substring(0, 3));
            prezime = parts[i++].Trim().Substring(3).Trim();

            while (parts[i].Trim() == String.Empty)
                i++;
            ime = parts[i++].Trim();

            while (parts[i].Trim() == String.Empty)
                i++;
            kod = parts[i].Substring(0, 3).ToUpper();
            datumRodj = DateTime.Parse(parts[i].Substring(3));
        }
    }
}
