using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Exceptions;
using System.IO;

namespace Bilten.Test
{
    public class DrzaveParser
    {
        private List<object[]> drzave;
        public List<object[]> Drzave
        {
            get { return drzave; }
        }

        public DrzaveParser()
        { 
        
        }

        public void parse(string fileName)
        {
            drzave = new List<object[]>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0)
                        continue;
                    
                    string kod = line.Substring(0, 3).ToUpper();
                    line = line.Remove(0, 3).Trim();

                    string naziv = line;

                    drzave.Add(new object[] { kod, naziv });
                }
            }
        }
    }
}
