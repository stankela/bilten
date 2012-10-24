using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Windows.Forms;

namespace Bilten.UI
{
    public class GridColumnsInitializer
    {
        public static void initGimnasticarUcesnik(DataGridViewUserControl dgw)
        {
            dgw.AddColumn("Ime", "Ime", 100);
            dgw.AddColumn("Prezime", "Prezime", 100);
            dgw.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dgw.AddColumn("Registarski broj", "RegistarskiBroj", 100);
            dgw.AddColumn("Takmicarski broj", "TakmicarskiBroj", 70);
            dgw.AddColumn("Klub", "KlubUcesnik", 130);
            dgw.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        public static void initGimnasticarUcesnik2(DataGridViewUserControl dgw)
        {
            dgw.AddColumn("Ime", "Ime", 100);
            dgw.AddColumn("Prezime", "Prezime", 100);
            dgw.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dgw.AddColumn("Klub", "KlubUcesnik", 130);
            dgw.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        public static void initRasporedSudija(DataGridViewUserControl dgw)
        {
            dgw.AddColumn("Funkcija", "Uloga", 100);
            dgw.AddColumn("Ime", "PrezimeIme", 150);
            dgw.AddColumn("Drzava", "Drzava", 100);
        }

        public static void initStartListaRotacija(DataGridViewUserControl dgw)
        {
            dgw.AddColumn("Broj", "TakmicarskiBroj", 50);
            dgw.AddColumn("Ime", "PrezimeIme", 150);
            dgw.AddColumn("Klub/Drzava", "KlubDrzava", 130);
            dgw.AddColumn("Kategorija", "Kategorija", 100);
        }

        public static void initTakmicenje(DataGridViewUserControl dgw)
        {
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
            
            int brojEOcena = takmicenje.BrojESudija;
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
    }
}
