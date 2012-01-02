using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.IO;

namespace Bilten.Test
{
    public class StartListaParser
    {
        private List<object[]> nastupiNaSpravi;
        public List<object[]> NastupiNaSpravi
        {
            get { return nastupiNaSpravi; }
        }

        public StartListaParser()
        {

        }

        public void parse(string fileName)
        {
            nastupiNaSpravi = new List<object[]>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                string nazivSprave = String.Empty;
                int subdivision = 0;
                int rotation = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() == String.Empty)
                        continue;

                    if (line.StartsWith("SUBDIVISION"))
                    {
                        String sub = line.Remove(0, line.IndexOf(' ')).Trim();
                        subdivision = Int32.Parse(sub);
                        continue;
                    }

                    if (line.StartsWith("ROTATION"))
                    {
                        String rot = line.Remove(0, line.IndexOf(' ')).Trim();
                        rotation = Int32.Parse(rot);
                        continue;
                    }

                    if (!Char.IsDigit(line[0]))
                    {
                        nazivSprave = line.Trim();
                        continue;
                    }

                    int broj;
                    bool nastupaDvaPuta;

                    parseNastupNaSpravi(line, nazivSprave, out broj, out nastupaDvaPuta);
                    nastupiNaSpravi.Add(new object[] { nazivSprave, subdivision,
                        rotation, broj, nastupaDvaPuta });
                }
            }
        }

        private void parseNastupNaSpravi(string line, string sprava, out int broj,
            out bool nastupaDvaPuta)
        {
            nastupaDvaPuta = false;
            broj = Int32.Parse(line.Trim().Substring(0, 3));
            if (sprava.ToUpper() == "PRESKOK")
                nastupaDvaPuta = line.Trim().EndsWith("*");
        }
    }
}
