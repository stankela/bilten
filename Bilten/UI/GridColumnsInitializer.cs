using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Windows.Forms;
using System.Drawing;

namespace Bilten.UI
{
    public class GridColumnsInitializer
    {
        public static void initGimnasticarUcesnik(DataGridViewUserControl dgw)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Ime", "Ime", 100);
            dgw.AddColumn("Prezime", "Prezime", 100);
            dgw.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dgw.AddColumn("Registarski broj", "RegistarskiBroj", 100);
            dgw.AddColumn("Takmicarski broj", "TakmicarskiBroj", 70);
            dgw.AddColumn("Klub", "KlubUcesnik", 150);
            dgw.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        public static void initGimnasticarUcesnik2(DataGridViewUserControl dgw)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Ime", "Ime", 100);
            dgw.AddColumn("Prezime", "Prezime", 100);
            dgw.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dgw.AddColumn("Klub", "KlubUcesnik", 150);
            dgw.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        private static IDictionary<int, IDictionary<string, int>> rasporedSudijaMap
            = new Dictionary<int, IDictionary<string, int>>();

        private const string RASPORED_SUDIJA_FUNKCIJA = "Funkcija";
        private const string RASPORED_SUDIJA_IME = "Ime";
        private const string RASPORED_SUDIJA_KLUB = "Klub/Drzava";

        public static void initRasporedSudija(int id, DataGridViewUserControl dgw)
        {
            IDictionary<string, int> columnMap;
            if (!rasporedSudijaMap.ContainsKey(id))
            {
                columnMap = new Dictionary<string, int>();
                columnMap.Add(RASPORED_SUDIJA_FUNKCIJA, 50);
                columnMap.Add(RASPORED_SUDIJA_IME, 150);
                columnMap.Add(RASPORED_SUDIJA_KLUB, 130);

                rasporedSudijaMap.Add(id, columnMap);
            }
            columnMap = rasporedSudijaMap[id];

            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn(RASPORED_SUDIJA_FUNKCIJA, "Uloga", columnMap[RASPORED_SUDIJA_FUNKCIJA]);
            dgw.AddColumn(RASPORED_SUDIJA_IME, "PrezimeIme", columnMap[RASPORED_SUDIJA_IME]);
            dgw.AddColumn(RASPORED_SUDIJA_KLUB, "KlubDrzava", columnMap[RASPORED_SUDIJA_KLUB]);
        }

        public static void updateRasporedSudija(int id, DataGridView dgw)
        {
            if (!rasporedSudijaMap.ContainsKey(id))
                return;
            IDictionary<string, int> columnMap = rasporedSudijaMap[id];
            columnMap[RASPORED_SUDIJA_FUNKCIJA] = dgw.Columns[RASPORED_SUDIJA_FUNKCIJA].Width;
            columnMap[RASPORED_SUDIJA_IME] = dgw.Columns[RASPORED_SUDIJA_IME].Width;
            columnMap[RASPORED_SUDIJA_KLUB] = dgw.Columns[RASPORED_SUDIJA_KLUB].Width;
        }

        private static IDictionary<int, IDictionary<string, int>> startListeMap
            = new Dictionary<int, IDictionary<string, int>>();

        private const string START_LISTA_BROJ = "Broj";
        private const string START_LISTA_IME = "Ime";
        private const string START_LISTA_KLUB = "Klub/Drzava";
        private const string START_LISTA_KATEGORIJA = "Kategorija";

        public static void initStartLista(int id, DataGridViewUserControl dgw)
        {
            IDictionary<string, int> columnMap;
            if (!startListeMap.ContainsKey(id))
            {
                columnMap = new Dictionary<string, int>();
                columnMap.Add(START_LISTA_BROJ, 50);
                columnMap.Add(START_LISTA_IME, 150);
                columnMap.Add(START_LISTA_KLUB, 130);
                columnMap.Add(START_LISTA_KATEGORIJA, 100);

                startListeMap.Add(id, columnMap);
            }
            columnMap = startListeMap[id];

            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn(START_LISTA_BROJ, "TakmicarskiBroj", columnMap[START_LISTA_BROJ]);
            dgw.AddColumn(START_LISTA_IME, "PrezimeIme", columnMap[START_LISTA_IME]);
            dgw.AddColumn(START_LISTA_KLUB, "KlubDrzava", columnMap[START_LISTA_KLUB]);
            dgw.AddColumn(START_LISTA_KATEGORIJA, "Kategorija", columnMap[START_LISTA_KATEGORIJA]);
        }

        public static void updateStartLista(int id, DataGridView dgw)
        {
            if (!startListeMap.ContainsKey(id))
                return;
            IDictionary<string, int> columnMap = startListeMap[id];
            columnMap[START_LISTA_BROJ] = dgw.Columns[START_LISTA_BROJ].Width;
            columnMap[START_LISTA_IME] = dgw.Columns[START_LISTA_IME].Width;
            columnMap[START_LISTA_KLUB] = dgw.Columns[START_LISTA_KLUB].Width;
            columnMap[START_LISTA_KATEGORIJA] = dgw.Columns[START_LISTA_KATEGORIJA].Width;
        }

        public static void initTakmicenje(DataGridViewUserControl dgw)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Naziv takmicenja", "Naziv", 300);
            dgw.AddColumn("Gimnastika", "Gimnastika", 70);
            dgw.AddColumn("Datum odrzavanja", "Datum", 100, "{0:d}");
            dgw.AddColumn("Mesto odrzavanja", "Mesto", 100);
        }

        public static void initOcene(DataGridViewUserControl dgw, 
            Takmicenje takmicenje, Sprava sprava)
        {
            int ocenaWidth = 50;

            dgw.DataGridView.Columns.Clear();

            DataGridViewColumn column;
            dgw.AddColumn("Broj", "TakmicarskiBroj", 50);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);
            
            int brojEOcena = takmicenje.BrojEOcena;
            if (sprava == Sprava.Preskok)
            {
                column = dgw.AddColumn("D", "D_2", ocenaWidth);
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                if (brojEOcena >= 1)
                {
                    column = dgw.AddColumn("E1", "E1_2", ocenaWidth);
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
                if (brojEOcena >= 2)
                {
                    column = dgw.AddColumn("E2", "E2_2", ocenaWidth);
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
                if (brojEOcena >= 3)
                {
                    column = dgw.AddColumn("E3", "E3_2", ocenaWidth);
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
                if (brojEOcena >= 4)
                {
                    column = dgw.AddColumn("E4", "E4_2", ocenaWidth);
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
                if (brojEOcena >= 5)
                {
                    column = dgw.AddColumn("E5", "E5_2", ocenaWidth);
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
                if (brojEOcena >= 6)
                {
                    column = dgw.AddColumn("E6", "E6_2", ocenaWidth);
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
                
                column = dgw.AddColumn("E", "E_2", ocenaWidth);
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                column = dgw.AddColumn("Pen.", "Penalty_2", ocenaWidth);
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                column = dgw.AddColumn("Konacna ocena", "Total_2", 60);
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                
                dgw.AddColumn("Oba preskoka", "TotalObeOcene", 60, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            }
            else
            {
                dgw.AddColumn("D", "D", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaD + "}");

                if (brojEOcena >= 1)
                    dgw.AddColumn("E1", "E1", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaE1 + "}");
                if (brojEOcena >= 2)
                    dgw.AddColumn("E2", "E2", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaE1 + "}");
                if (brojEOcena >= 3)
                    dgw.AddColumn("E3", "E3", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaE1 + "}");
                if (brojEOcena >= 4)
                    dgw.AddColumn("E4", "E4", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaE1 + "}");
                if (brojEOcena >= 5)
                    dgw.AddColumn("E5", "E5", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaE1 + "}");
                if (brojEOcena >= 6)
                    dgw.AddColumn("E6", "E6", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaE1 + "}");
   
                dgw.AddColumn("E", "E", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaE + "}");
                dgw.AddColumn("Pen.", "Penalty", ocenaWidth, "{0:F" + takmicenje.BrojDecimalaPen + "}");
                dgw.AddColumn("Konacna ocena", "Total", 60, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            }
        }

        public static void initRezultatiUkupno(DataGridViewUserControl dgw, 
            Takmicenje takmicenje, bool kvalColumn)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
            dgw.AddColumn("Broj", "TakmicarskiBroj", 50);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);

            foreach (Sprava sprava in sprave)
            {
                string columnName = Sprave.toString(sprava);
                string propName = columnName;
                int width = 50;
                if (sprava == Sprava.Konj)
                {
                    width = 70;
                    propName = "Konj";
                }
                else if (sprava == Sprava.DvovisinskiRazboj)
                {
                    width = 70;
                    propName = "DvovisinskiRazboj";
                }
                dgw.AddColumn(columnName, propName, width, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            }
            dgw.AddColumn("Ukupno", "Total", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            
            if (kvalColumn)
                dgw.AddColumn("", "KvalStatus", 30);
        }

        public static void initRezultatiSprava(DataGridViewUserControl dgw,
            Takmicenje takmicenje, bool kvalColumn, bool obaPreskoka)
        {
            dgw.DataGridView.Columns.Clear();
            
            if (obaPreskoka)
                dgw.AddColumn("Rank", "Rank2", 40);
            else
                dgw.AddColumn("Rank", "Rank", 40);

            dgw.AddColumn("Broj", "TakmicarskiBroj", 50);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            if (obaPreskoka)
            {
                dgw.AddColumn("D", "D", 50, "{0:F" + takmicenje.BrojDecimalaD + "}");
                dgw.AddColumn("E", "E", 50, "{0:F" + takmicenje.BrojDecimalaE + "}");
                dgw.AddColumn("Pen.", "Penalty", 50, "{0:F" + takmicenje.BrojDecimalaPen + "}");
                dgw.AddColumn("Total 1", "Total", 65, "{0:F" + takmicenje.BrojDecimalaTotal + "}");

                dgw.AddColumn("D", "D_2", 50, "{0:F" + takmicenje.BrojDecimalaD + "}");
                dgw.AddColumn("E", "E_2", 50, "{0:F" + takmicenje.BrojDecimalaE + "}");
                dgw.AddColumn("Pen.", "Penalty_2", 50, "{0:F" + takmicenje.BrojDecimalaPen + "}");
                dgw.AddColumn("Total 2", "Total_2", 65, "{0:F" + takmicenje.BrojDecimalaTotal + "}");

                dgw.AddColumn("Konacna ocena", "TotalObeOcene", 60, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            }
            else
            {
                dgw.AddColumn("D", "D", 50, "{0:F" + takmicenje.BrojDecimalaD + "}");
                dgw.AddColumn("E", "E", 50, "{0:F" + takmicenje.BrojDecimalaE + "}");
                dgw.AddColumn("Pen.", "Penalty", 50, "{0:F" + takmicenje.BrojDecimalaPen + "}");
                dgw.AddColumn("Konacna ocena", "Total", 60, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            }

            if (kvalColumn)
                dgw.AddColumn("", "KvalStatus", 30);
        }

        public static void initKvalifikantiTak2(DataGridViewUserControl dgw, Takmicenje takmicenje)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Red. broj", "QualOrder", 40);
            dgw.AddColumn("Broj", "TakmicarskiBroj", 50);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            dgw.AddColumn("Kval. ocena", "QualScore", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Kval. rank", "QualRank", 40);
        }

        public static void initKvalifikantiTak3(DataGridViewUserControl dgw, Takmicenje takmicenje)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Red. broj", "QualOrder", 40);
            dgw.AddColumn("Broj", "TakmicarskiBroj", 50);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            dgw.AddColumn("Kval. ocena", "QualScore", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Kval. rank", "QualRank", 40);
        }

        public static void initRezultatiEkipno(DataGridViewUserControl dgw, Takmicenje takmicenje, bool kvalColumn)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
            dgw.AddColumn("Ekipa", "NazivEkipe", 170);

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);

            foreach (Sprava sprava in sprave)
            {
                string columnName = Sprave.toString(sprava);
                string propName = columnName;
                int width = 50;
                if (sprava == Sprava.Konj)
                {
                    width = 70;
                    propName = "Konj";
                }
                else if (sprava == Sprava.DvovisinskiRazboj)
                {
                    width = 70;
                    propName = "DvovisinskiRazboj";
                }
                dgw.AddColumn(columnName, propName, width, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            }
            dgw.AddColumn("Ukupno", "Total", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");

            if (kvalColumn)
                dgw.AddColumn("", "KvalStatus", 30);
        }

        public static void initRezultatiUkupnoZaEkipe(DataGridViewUserControl dgw,
            Takmicenje takmicenje)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);

            foreach (Sprava sprava in sprave)
            {
                string columnName = Sprave.toString(sprava);
                string propName = columnName;
                int width = 50;
                if (sprava == Sprava.Konj)
                {
                    width = 70;
                    propName = "Konj";
                }
                else if (sprava == Sprava.DvovisinskiRazboj)
                {
                    width = 70;
                    propName = "DvovisinskiRazboj";
                }
                dgw.AddColumn(columnName, propName, width, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            }
            dgw.AddColumn("Ukupno", "Total", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
        }

        public static void initRezultatiUkupnoFinaleKupa(DataGridViewUserControl dgw,
            Takmicenje takmicenje, bool kvalColumn)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
            dgw.AddColumn("Broj", "TakmicarskiBroj", 50);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);
            dgw.AddColumn("I Kolo", "TotalPrvoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("II Kolo", "TotalDrugoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Ukupno", "Total", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            if (kvalColumn)
                dgw.AddColumn("", "KvalStatus", 30);
        }

        public static void initRezultatiSpravaFinaleKupa(DataGridViewUserControl dgw,
            Takmicenje takmicenje, bool kvalColumn)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
            dgw.AddColumn("Broj", "TakmicarskiBroj", 50);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            dgw.AddColumn("    D \n I kolo", "D_PrvoKolo", 50, "{0:F" + takmicenje.BrojDecimalaD + "}");
            dgw.AddColumn("    E \n I kolo", "E_PrvoKolo", 50, "{0:F" + takmicenje.BrojDecimalaE + "}");
            dgw.AddColumn("Total \nI kolo", "TotalPrvoKolo", 60, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("    D \n II kolo", "D_DrugoKolo", 50, "{0:F" + takmicenje.BrojDecimalaD + "}");
            dgw.AddColumn("    E \n II kolo", "E_DrugoKolo", 50, "{0:F" + takmicenje.BrojDecimalaE + "}");
            dgw.AddColumn("Total \nII kolo", "TotalDrugoKolo", 70, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Finalna ocena", "Total", 60, "{0:F" + takmicenje.BrojDecimalaTotal + "}");

            if (kvalColumn)
                dgw.AddColumn("", "KvalStatus", 30);
        }

        public static void initRezultatiEkipnoFinaleKupa(DataGridViewUserControl dgw, Takmicenje takmicenje, bool kvalColumn)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
            dgw.AddColumn("Ekipa", "NazivEkipe", 170);
            dgw.AddColumn("I Kolo", "TotalPrvoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("II Kolo", "TotalDrugoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Ukupno", "Total", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            if (kvalColumn)
                dgw.AddColumn("", "KvalStatus", 30);
        }


        public static int getMaxWidth(List<string> strings, DataGridView dataGridView)
        {
            Graphics g = dataGridView.CreateGraphics();
            float maxWidth = 0.0f;
            foreach (string s in strings)
            {
                float width = g.MeasureString(s, dataGridView.Font).Width;
                if (width > maxWidth)
                    maxWidth = width;
            }
            g.Dispose();
            return (int)Math.Ceiling(maxWidth);
        }
    }
}
