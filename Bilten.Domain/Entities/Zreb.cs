using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Domain
{
    public static class Zreb
    {
        // Parsiraj zreb onako kako je unesen (sa razmacima izmedju brojeva)
        public static List<int> parseRawZreb(string ZrebZaFinalePoSpravama)
        {
            string zreb = String.Empty;
            if (ZrebZaFinalePoSpravama != null)
                zreb = ZrebZaFinalePoSpravama.Trim();
            if (zreb == String.Empty)
                return new List<int>();

            List<string> parts = new List<string>();
            char delimiter = ' ';
            int index = zreb.IndexOf(delimiter);
            while (index != -1)
            {
                parts.Add(zreb.Substring(0, index));
                zreb = zreb.Substring(index).Trim();
                index = zreb.IndexOf(delimiter);
            }
            parts.Add(zreb.Trim());

            List<int> result = new List<int>();
            int value;
            for (int i = 0; i < parts.Count; i++)
            {
                if (!int.TryParse(parts[i], out value))
                    return null;
                result.Add(value);
            }

            int[] occurences = new int[result.Count];
            for (int i = 0; i < result.Count; i++)
                occurences[i] = 0;
            for (int i = 0; i < result.Count; i++)
            {
                int number = result[i];
                if (number < 1 || number > result.Count)
                    return null;
                if (occurences[number - 1] == 0)
                    occurences[number - 1] = 1;
                else
                    return null;
            }
            return result;
        }

        private static List<int> parseKompresovanZreb(string zreb)
        {
            List<int> result = new List<int>();
            int i = 0;
            while (i < zreb.Length)
            {
                if (zreb[i] != '\'')
                {
                    result.Add(Int32.Parse(zreb[i].ToString()));
                    ++i;
                }
                else
                {
                    int end = zreb.IndexOf('\'', i + 1);
                    result.Add(Int32.Parse(zreb.Substring(i + 1, end - i - 1)));
                    i = end;
                }
            }
            return result;
        }

        public static List<List<int>> parseZreb(string ZrebZaFinalePoSpravama, Gimnastika gim)
        {
            List<List<int>> result = new List<List<int>>();
            if (ZrebZaFinalePoSpravama[0] != '#')
            {
                if (gim == Gimnastika.MSG)
                {
                    List<int> zreb = Zreb.parseRawZreb(ZrebZaFinalePoSpravama);
                    result.Add(zreb);
                    result.Add(zreb);
                    result.Add(zreb);
                    result.Add(zreb);
                    result.Add(zreb);
                    result.Add(zreb);
                }
                else
                {
                    List<int> zreb = Zreb.parseRawZreb(ZrebZaFinalePoSpravama);
                    result.Add(zreb);
                    result.Add(zreb);
                    result.Add(zreb);
                    result.Add(zreb);
                }
            }
            else
            {
                ZrebZaFinalePoSpravama = ZrebZaFinalePoSpravama.Substring(1);
                char delimiter = '-';
                int index = ZrebZaFinalePoSpravama.IndexOf(delimiter);
                while (index != -1)
                {
                    result.Add(Zreb.parseKompresovanZreb(ZrebZaFinalePoSpravama.Substring(0, index)));
                    ZrebZaFinalePoSpravama = ZrebZaFinalePoSpravama.Substring(index + 1);
                    index = ZrebZaFinalePoSpravama.IndexOf(delimiter);
                }
                result.Add(Zreb.parseKompresovanZreb(ZrebZaFinalePoSpravama));
            }
            return result;
        }

        private static string kompresuj(string zrebZaSpravu)
        {
            string result = String.Empty;
            List<int> zreb = Zreb.parseRawZreb(zrebZaSpravu);
            foreach (int i in zreb)
            {
                if (i > 9)
                    result += "'";
                result += i.ToString();
                if (i > 9)
                    result += "'";
            }
            return result;
        }

        public static string kompresuj(Gimnastika gimnastika, string zreb1, string zreb2, string zreb3, string zreb4,
                                       string zreb5, string zreb6)
        {
            string result = "#";
            result += Zreb.kompresuj(zreb1) + "-";
            result += Zreb.kompresuj(zreb2) + "-";
            result += Zreb.kompresuj(zreb3) + "-";
            if (gimnastika == Gimnastika.ZSG)
            {
                result += Zreb.kompresuj(zreb4);
            }
            else
            {
                result += Zreb.kompresuj(zreb4) + "-";
                result += Zreb.kompresuj(zreb5) + "-";
                result += Zreb.kompresuj(zreb6);
            }
            return result;
        }

        public static string createRawZreb(List<int> zreb)
        {
            string result = String.Empty;
            foreach (int i in zreb)
            {
                result += i.ToString() + " ";
            }
            return result.TrimEnd();
        }
    }
}
