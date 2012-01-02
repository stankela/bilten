using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.IO;

namespace Bilten.Test
{
    public class RegistrovaniGimnasticariParser
    {
        private List<object[]> gimnasticari;
        public List<object[]> Gimnasticari
        {
            get { return gimnasticari; }
        }

        public RegistrovaniGimnasticariParser()
        { 
        
        }

        public void parse(string fileName)
        {
            gimnasticari = new List<object[]>();

            List<object[]> parsedLine = new List<object[]>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine().Trim();
                    if (line.Length == 0 || !Char.IsDigit(line[0]))
                        continue;

                    char pol;
                    string ime;
                    string prezime;
                    string datumRodj;
                    string klubMesto;
                    string regBroj;
                    string datumReg;

                    parseGimnasticar(line, out ime, out prezime, out pol,
                        out datumRodj, out klubMesto, out regBroj, out datumReg);

                    parsedLine.Add(new object[] { pol, ime, prezime, datumRodj,
                        klubMesto, regBroj, datumReg});

                }
            }

            Dictionary<string, Klub> kluboviMestaMap = new Dictionary<string, Klub>();
            Dictionary<string, Gimnasticar> gimnast = new Dictionary<string, Gimnasticar>();
            Dictionary<string, Gimnasticar> duplGimnast = new Dictionary<string, Gimnasticar>();
            foreach (object[] o in parsedLine)
            {
                char pol = (char)o[0];
                string ime = (string)o[1];
                string prezime = (string)o[2];
                string datumRodj = (string)o[3];
                string klubMesto = (string)o[4];
                string regBroj = (string)o[5];
                string datumReg = (string)o[6];

        
                string gim_key = pol + ime + prezime
                    + datumRodj + klubMesto + regBroj;
                if (!gimnast.ContainsKey(gim_key))
                {
                    gimnast.Add(gim_key, null);
                    gimnasticari.Add(new object[] { pol, ime, prezime, datumRodj,
                        klubMesto, regBroj, datumReg });

                }
                else
                    duplGimnast.Add(gim_key, null);
                
            }
        }

        private void parseGimnasticar(string line, out string ime, out string prezime,
            out char pol, out string datumRodj, out string klubMesto,
            out string regBroj, out string datumReg)
        {
            string[] parts = line.Split('\t');

            int i = 0;
            string brojPolIme = parts[i++].Trim();
            string[] parts2 = brojPolIme.Split(' ');
            string broj = parts2[0].Trim();
            if (parts2[1] != String.Empty)
                pol = parts2[1][0];
            else
                pol = ' ';
            ime = parts2[parts2.Length - 1].Trim();

            while (parts[i].Trim() == String.Empty)
                i++;
            prezime = parts[i++].Trim();

            while (parts[i].Trim() == String.Empty)
                i++;
            datumRodj = parts[i++].Trim();

            while (parts[i].Trim() == String.Empty)
                i++;
            klubMesto = parts[i++].Trim();

            while (parts[i].Trim() == String.Empty)
                i++;
            regBroj = parts[i++].Trim();

            while (i < parts.Length && parts[i].Trim() == String.Empty)
                i++;
            if (i < parts.Length)
                datumReg = parts[i].Trim();
            else
                datumReg = String.Empty;
        }

    }
}
