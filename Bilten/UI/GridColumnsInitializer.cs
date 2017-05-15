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

            dgw.AddColumn("Ime", "ImeSrednjeIme", 100);
            dgw.AddColumn("Prezime", "Prezime", 100);
            dgw.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dgw.AddColumn("Klub", "KlubUcesnik", 150);
            dgw.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        private static IDictionary<int, IDictionary<string, int>> rasporedSudijaMap
            = new Dictionary<int, IDictionary<string, int>>();

        private static string[] RASPORED_SUDIJA_COLUMNS = new string[] { 
            "Funkcija", "Ime", "Klub/Drzava" };
        private static string[] RASPORED_SUDIJA_PROPERTIES = new string[] { 
            "Uloga", "PrezimeIme", "KlubDrzava" };
        private static int[] RASPORED_SUDIJA_WIDTHS = new int[] {        
            50, 150, 130 };

        public static void initRasporedSudija(int id, DataGridViewUserControl dgw)
        {
            initGrid(id, dgw, rasporedSudijaMap, RASPORED_SUDIJA_COLUMNS, RASPORED_SUDIJA_PROPERTIES, 
                RASPORED_SUDIJA_WIDTHS);
        }

        private static void initGrid(int id, DataGridViewUserControl dgw, IDictionary<int, IDictionary<string, int>> map,
            string[] columnNames, string[] propertyNames, int[] widths)
        {
            IDictionary<string, int> columnMap;
            if (!map.ContainsKey(id))
            {
                columnMap = new Dictionary<string, int>();
                for (int i = 0; i < columnNames.Length; ++i)
                {
                    columnMap.Add(columnNames[i], widths[i]);
                }
                map.Add(id, columnMap);
            }
            columnMap = map[id];

            dgw.DataGridView.Columns.Clear();

            for (int i = 0; i < columnNames.Length; ++i)
            {
                dgw.AddColumn(columnNames[i], propertyNames[i], columnMap[columnNames[i]]);
            }
        }

        public static void rasporedSudijaColumnWidthChanged(int id, DataGridView dgw)
        {
            recordColumnWidths(id, dgw, rasporedSudijaMap, RASPORED_SUDIJA_COLUMNS);
        }

        private static void recordColumnWidths(int id, DataGridView dgw, IDictionary<int, IDictionary<string, int>> map,
            string[] columnNames)
        {
            if (!map.ContainsKey(id))
                return;

            IDictionary<string, int> columnMap = map[id];
            foreach (string name in columnNames)
            {
                if (columnMap.ContainsKey(name))
                    columnMap[name] = dgw.Columns[name].Width;
            }
        }

        private static IDictionary<int, IDictionary<string, int>> startListeMap
            = new Dictionary<int, IDictionary<string, int>>();

        private static string[] START_LISTA_COLUMNS = new string[] {
            "Broj", "Ime", "Klub/Drzava", "Kategorija" };
        private static string[] START_LISTA_PROPERTIES = new string[] {
             "TakmicarskiBroj", "PrezimeIme", "KlubDrzava", "Kategorija" };
        private static int[] START_LISTA_WIDTHS = new int[] {
             50, 150, 130, 100 };

        public static void initStartLista(int id, DataGridViewUserControl dgw)
        {
            initGrid(id, dgw, startListeMap, START_LISTA_COLUMNS, START_LISTA_PROPERTIES,
                START_LISTA_WIDTHS);
        }

        public static void startListaColumnWidthChanged(int id, DataGridView dgw)
        {
            recordColumnWidths(id, dgw, startListeMap, START_LISTA_COLUMNS);
        }

        // TODO3: Dodaj pamcenje sirine kolona i za ostale gridove (narocito one koji se stampaju)

        public static void initTakmicenje(DataGridViewUserControl dgw)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Naziv takmicenja", "Naziv", 300);
            dgw.AddColumn("Gimnastika", "Gimnastika", 70);
            dgw.AddColumn("Datum odrzavanja", "Datum", 100, "{0:d}");
            dgw.AddColumn("Mesto odrzavanja", "Mesto", 100);
            dgw.AddColumn("Tip takmicenja", "TipTakmicenja", 100);
        }

        public static void initOcene(DataGridViewUserControl dgw, 
            Takmicenje takmicenje, Sprava sprava)
        {
            int ocenaWidth = 50;

            dgw.DataGridView.Columns.Clear();

            DataGridViewColumn column;
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
            Takmicenje takmicenje, bool kvalColumn, bool penalty)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
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

            if (penalty)
                dgw.AddColumn("Pen.", "Penalty", 50, "{0:F" + takmicenje.BrojDecimalaPen + "}");
            
            if (kvalColumn)
                dgw.AddColumn("", "KvalStatus", 30);
        }


        // TODO: Indexi kolona bi trebali da budu konstante
        public static void maximizeColumnsRezultatiUkupno(DataGridViewUserControl dgw, 
            DeoTakmicenjaKod deoTakKod, IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            List<string> imena = new List<string>();
            List<string> klubovi = new List<string>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (RezultatUkupno r in rt.getPoredakUkupno(deoTakKod).getRezultati())
                {
                    imena.Add(r.Gimnasticar.PrezimeIme);
                    klubovi.Add(r.Gimnasticar.KlubDrzava);
                }
            }
            if (imena.Count > 0)
                dgw.DataGridView.Columns[1].Width = GridColumnsInitializer.getMaxWidth(imena, dgw.DataGridView);
            if (klubovi.Count > 0)
                dgw.DataGridView.Columns[2].Width = GridColumnsInitializer.getMaxWidth(klubovi, dgw.DataGridView);
        }

        public static void reinitRezultatiUkupnoKeepColumnWidths(DataGridViewUserControl dgw, Takmicenje takmicenje, 
            bool kvalColumn, bool penalty)
        {
            int oldImeWidth = dgw.DataGridView.Columns[1].Width;
            int oldKlubWidth = dgw.DataGridView.Columns[2].Width;

            GridColumnsInitializer.initRezultatiUkupno(dgw, takmicenje, kvalColumn, penalty);

            dgw.DataGridView.Columns[1].Width = oldImeWidth;
            dgw.DataGridView.Columns[2].Width = oldKlubWidth;
        }

        public static void initRezultatiSprava(DataGridViewUserControl dgw,
            Takmicenje takmicenje, bool kvalColumn, Sprava sprava, bool obaPreskoka)
        {
            dgw.DataGridView.Columns.Clear();
            
            dgw.AddColumn("Rank", "Rank", 40);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            Color preskokColor = Color.LightGreen;
            if (sprava == Sprava.Preskok)
            {
                dgw.AddColumn("D", "D", 50, "{0:F" + takmicenje.BrojDecimalaD + "}");
                dgw.AddColumn("E", "E", 50, "{0:F" + takmicenje.BrojDecimalaE + "}");
                dgw.AddColumn("Pen.", "Penalty", 50, "{0:F" + takmicenje.BrojDecimalaPen + "}");
                DataGridViewColumn col = dgw.AddColumn("Total 1", "Total", 65, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
                if (!obaPreskoka)
                    col.DefaultCellStyle.BackColor = preskokColor;

                dgw.AddColumn("D", "D_2", 50, "{0:F" + takmicenje.BrojDecimalaD + "}");
                dgw.AddColumn("E", "E_2", 50, "{0:F" + takmicenje.BrojDecimalaE + "}");
                dgw.AddColumn("Pen.", "Penalty_2", 50, "{0:F" + takmicenje.BrojDecimalaPen + "}");
                dgw.AddColumn("Total 2", "Total_2", 65, "{0:F" + takmicenje.BrojDecimalaTotal + "}");

                col = dgw.AddColumn("Konacna ocena", "TotalObeOcene", 60, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
                if (obaPreskoka)
                    col.DefaultCellStyle.BackColor = preskokColor;
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

        public static void maximizeColumnsRezultatiSprava(DataGridViewUserControl dgw, 
            DeoTakmicenjaKod deoTakKod, IList<RezultatskoTakmicenje> rezTakmicenja, bool finaleKupa)
        {
            List<string> imena = new List<string>();
            List<string> klubovi = new List<string>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (Sprava s in Sprave.getSprave(rt.Gimnastika))
                {
                    if (s != Sprava.Preskok)
                    {
                        foreach (RezultatSprava r in rt.getPoredakSprava(deoTakKod, s).getRezultati())
                        {
                            imena.Add(r.Gimnasticar.PrezimeIme);
                            klubovi.Add(r.Gimnasticar.KlubDrzava);
                        }
                    }
                    else
                    {
                        foreach (RezultatPreskok r in rt.getPoredakPreskok(deoTakKod).getRezultati())
                        {
                            imena.Add(r.Gimnasticar.PrezimeIme);
                            klubovi.Add(r.Gimnasticar.KlubDrzava);
                        }
                    }
                }
            }
            if (imena.Count > 0)
                dgw.DataGridView.Columns[1].Width = GridColumnsInitializer.getMaxWidth(imena, dgw.DataGridView);
            if (klubovi.Count > 0)
                dgw.DataGridView.Columns[2].Width = GridColumnsInitializer.getMaxWidth(klubovi, dgw.DataGridView);
        }

        public static void reinitRezultatiSpravaKeepColumnWidths(DataGridViewUserControl dataGridViewUserControl,
            Takmicenje takmicenje, bool kvalColumnVisible, Sprava sprava, bool obaPreskoka)
        {
            DataGridView dgw = dataGridViewUserControl.DataGridView;
            int oldImeWidth = dgw.Columns[1].Width;
            int oldKlubWidth = dgw.Columns[2].Width;
            GridColumnsInitializer.initRezultatiSprava(dataGridViewUserControl, takmicenje, kvalColumnVisible, sprava,
                obaPreskoka);
            dgw.Columns[1].Width = oldImeWidth;
            dgw.Columns[2].Width = oldKlubWidth;
        }
   
        public static void initKvalifikantiTak2(DataGridViewUserControl dgw, Takmicenje takmicenje)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Red. broj", "QualOrder", 40);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            dgw.AddColumn("Kval. ocena", "QualScore", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Kval. rank", "QualRank", 40);
        }

        public static void initKvalifikantiTak3(DataGridViewUserControl dgw, Takmicenje takmicenje)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Red. broj", "QualOrder", 40);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);

            dgw.AddColumn("Kval. ocena", "QualScore", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Kval. rank", "QualRank", 40);
        }

        public static void initRezultatiEkipno(DataGridViewUserControl dgw, Takmicenje takmicenje, bool kvalColumn,
            bool penalty)
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

            if (penalty)
                dgw.AddColumn("Pen.", "Penalty", 50, "{0:F" + takmicenje.BrojDecimalaPen + "}");

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
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);
            dgw.AddColumn("I Kolo", "TotalPrvoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("II Kolo", "TotalDrugoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Ukupno", "Total", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            if (kvalColumn)
                dgw.AddColumn("", "KvalStatus", 30);
        }

        public static void maximizeColumnsRezultatiUkupnoFinaleKupa(DataGridViewUserControl dgw,
            IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            List<string> imena = new List<string>();
            List<string> klubovi = new List<string>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (RezultatUkupnoFinaleKupa r in rt.Takmicenje1.PoredakUkupnoFinaleKupa.getRezultati())
                {
                    imena.Add(r.Gimnasticar.PrezimeIme);
                    klubovi.Add(r.Gimnasticar.KlubDrzava);
                }
            }
            if (imena.Count > 0)
                dgw.DataGridView.Columns[1].Width = GridColumnsInitializer.getMaxWidth(imena, dgw.DataGridView);
            if (klubovi.Count > 0)
                dgw.DataGridView.Columns[2].Width = GridColumnsInitializer.getMaxWidth(klubovi, dgw.DataGridView);
        }

        public static void initRezultatiUkupnoZbirViseKola(DataGridViewUserControl dgw,
            Takmicenje takmicenje)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
            dgw.AddColumn("Ime", "PrezimeIme", 170);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);
            dgw.AddColumn("I Kolo", "TotalPrvoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("II Kolo", "TotalDrugoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            if (takmicenje.TreceKolo != null)
                dgw.AddColumn("III Kolo", "TotalTreceKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            if (takmicenje.CetvrtoKolo != null)
                dgw.AddColumn("IV Kolo", "TotalCetvrtoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Ukupno", "Total", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
        }

        public static void maximizeColumnsRezultatiUkupnoZbirViseKola(DataGridViewUserControl dgw,
            IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            List<string> imena = new List<string>();
            List<string> klubovi = new List<string>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (RezultatUkupnoZbirViseKola r in rt.Takmicenje1.PoredakUkupnoZbirViseKola.getRezultati())
                {
                    imena.Add(r.Gimnasticar.PrezimeIme);
                    klubovi.Add(r.Gimnasticar.KlubDrzava);
                }
            }
            if (imena.Count > 0)
                dgw.DataGridView.Columns[1].Width = GridColumnsInitializer.getMaxWidth(imena, dgw.DataGridView);
            if (klubovi.Count > 0)
                dgw.DataGridView.Columns[2].Width = GridColumnsInitializer.getMaxWidth(klubovi, dgw.DataGridView);
        }

        public static void initRezultatiSpravaFinaleKupa(DataGridViewUserControl dgw,
            Takmicenje takmicenje, bool kvalColumn)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
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

        public static void maximizeColumnsRezultatiSpravaFinaleKupa(DataGridViewUserControl dgw,
            IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            List<string> imena = new List<string>();
            List<string> klubovi = new List<string>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (PoredakSpravaFinaleKupa p in rt.Takmicenje1.PoredakSpravaFinaleKupa)
                {
                    foreach (RezultatSpravaFinaleKupa r in p.getRezultati())
                    {
                        imena.Add(r.Gimnasticar.PrezimeIme);
                        klubovi.Add(r.Gimnasticar.KlubDrzava);
                    }
                }
            }
            if (imena.Count > 0)
                dgw.DataGridView.Columns[1].Width = GridColumnsInitializer.getMaxWidth(imena, dgw.DataGridView);
            if (klubovi.Count > 0)
                dgw.DataGridView.Columns[2].Width = GridColumnsInitializer.getMaxWidth(klubovi, dgw.DataGridView);
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

        public static void initRezultatiEkipnoZbirViseKola(DataGridViewUserControl dgw, Takmicenje takmicenje)
        {
            dgw.DataGridView.Columns.Clear();

            dgw.AddColumn("Rank", "Rank", 40);
            dgw.AddColumn("Ekipa", "NazivEkipe", 170);
            dgw.AddColumn("I Kolo", "TotalPrvoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("II Kolo", "TotalDrugoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            if (takmicenje.TreceKolo != null)
                dgw.AddColumn("III Kolo", "TotalTreceKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            if (takmicenje.CetvrtoKolo != null)
                dgw.AddColumn("IV Kolo", "TotalCetvrtoKolo", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
            dgw.AddColumn("Ukupno", "Total", 50, "{0:F" + takmicenje.BrojDecimalaTotal + "}");
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
