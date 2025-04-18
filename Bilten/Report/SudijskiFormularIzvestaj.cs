using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Bilten.UI;

namespace Bilten.Report
{
    class SudijskiFormularIzvestaj : Izvestaj
    {
        private bool svakaSpravaNaPosebnojStrani;

        public SudijskiFormularIzvestaj(StartListaNaSpravi startLista, string documentName, int brojEOcena, 
            bool stampajRedniBroj, bool stampajKategoriju, bool stampajKlub, DataGridView formGrid, Takmicenje takmicenje,
            bool stampajBonus, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            Landscape = true;
            Margins = new Margins(30, 30, 75, 75);

            reportListe.Add(new SudijskiFormularLista(this, 1, 0f, itemFont, itemsHeaderFont, startLista, brojEOcena,
                stampajRedniBroj, stampajKategoriju, stampajKlub, formGrid, takmicenje.TakBrojevi, stampajBonus,
                resizeByGrid));
		}

        public SudijskiFormularIzvestaj(List<StartListaNaSpravi> startListe, string documentName, int brojEOcena,
            int brojSpravaPoStrani, bool stampajRedniBroj, bool stampajKategoriju, bool stampajKlub,
            SpravaGridGroupUserControl spravaGridGroupUserControl, Takmicenje takmicenje, bool stampajBonus, Font itemFont,
            bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;

            Landscape = true;
            Margins = new Margins(30, 30, 75, 75);
            
            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                SudijskiFormularLista lista = new SudijskiFormularLista(this, 1, 0f, itemFont, itemsHeaderFont,
                    startListe[i], brojEOcena, stampajRedniBroj, stampajKategoriju, stampajKlub,
                    spravaGridGroupUserControl[sprava].DataGridViewUserControl.DataGridView, takmicenje.TakBrojevi,
                    stampajBonus, resizeByGrid);
                reportListe.Add(lista);
            }
        }

        // TODO5: Trebalo bi da za svaku listu posebno izracunavam sirinu za ime, klub i kategoriju. Time sprecavam
        // situaciju sa gimnazijade 2025 gde je bio jedan Brazilac sa jako velikim imenom, i to je uticalo i na start
        // liste u spravama gde su imena bila kraca.
        
        protected override void doSetupContent(Graphics g)
        {
            poredjajListeUJednuKolonu(g, contentBounds, reportListe, svakaSpravaNaPosebnojStrani);
        }
    }

    public class SudijskiFormularLista : ReportLista
    {
        private Sprava sprava;
        private int brojEOcena;
        private bool stampajRedniBroj;
        private bool stampajKategoriju;
        private bool stampajKlub;
        private bool stampajBroj;
        private bool stampajBonus;

        public SudijskiFormularLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, StartListaNaSpravi startLista, int brojEOcena, bool stampajRedniBroj,
            bool stampajKategoriju, bool stampajKlub, DataGridView formGrid, bool stampajBroj, bool stampajBonus,
            bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = startLista.Sprava;
            this.brojEOcena = brojEOcena;
            this.stampajRedniBroj = stampajRedniBroj;
            this.stampajKategoriju = stampajKategoriju;
            this.stampajKlub = stampajKlub;
            this.stampajBroj = stampajBroj;
            this.stampajBonus = stampajBonus;
            this.resizeByGrid = resizeByGrid;

            fetchItems(startLista);
        }

        public int getImeColumnIndex()
        {
            return stampajBroj ? 2 : 1;
        }

        public int getKlubColumnIndex()
        {
            if (!stampajKlub)
                return -1;
            return stampajBroj ? 3 : 2;
        }

        public int getKategorijaColumnIndex()
        {
            if (!stampajKategoriju)
                return -1;
            else if (!stampajKlub)
                return stampajBroj ? 3 : 2;
            else
                return stampajBroj ? 4 : 3;
        }

        private void fetchItems(StartListaNaSpravi startLista)
        {
            items = getStartListaReportItems(startLista);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getStartListaReportItems(StartListaNaSpravi startLista)
        {
            List<object[]> result = new List<object[]>();
            for (int i = 0; i < startLista.Nastupi.Count; i++)
            {
                string redBroj = stampajRedniBroj ? (i+1).ToString() : String.Empty;
                NastupNaSpravi nastup = startLista.Nastupi[i];
                List<object> items;
                if (startLista.Sprava != Sprava.Preskok)
                {
                    items = new List<object>() { redBroj, nastup.PrezimeIme, nastup.KlubDrzava,
                        nastup.Kategorija, "", "", "", ""};
                    if (stampajBonus) items.Add("");
                    if (brojEOcena > 0) items.Add("");
                    if (brojEOcena > 1) items.Add("");
                    if (brojEOcena > 2) items.Add("");
                    if (brojEOcena > 3) items.Add("");
                    if (brojEOcena > 4) items.Add("");
                    if (brojEOcena > 5) items.Add("");
                }
                else
                {
                    items = new List<object>() { redBroj, nastup.PrezimeIme, nastup.KlubDrzava,
                        nastup.Kategorija, "1", "2", "", "", "", "", "", "", "", "", ""};
                    if (stampajBonus) items.Add(""); items.Add("");
                    if (brojEOcena > 0) items.Add(""); items.Add("");
                    if (brojEOcena > 1) items.Add(""); items.Add("");
                    if (brojEOcena > 2) items.Add(""); items.Add("");
                    if (brojEOcena > 3) items.Add(""); items.Add("");
                    if (brojEOcena > 4) items.Add(""); items.Add("");
                    if (brojEOcena > 5) items.Add(""); items.Add("");
                }
                if (stampajBroj)
                {
                    string broj = (nastup.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? nastup.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
            }
            return result;
        }

        public override List<int> getAdjustableColumnIndexes()
        {
            List<int> result = new List<int>();
            result.Add(getImeColumnIndex());
            result.Add(getKlubColumnIndex());
            result.Add(getKategorijaColumnIndex());
            return result;
        }

        protected override void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
            List<bool> rszByGrid)
        {
            // First, create columns

            float imeWidth;
            float klubWidth;
            float kategorijaWidth;
            if (columnWidths.Count == 0)
            {
                // Prvi pass
                if (resizeByGrid)
                {
                    float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
                    float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;
                    imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                    klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
                    kategorijaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
                }
                else
                {
                    // Resize by content
                    // Proizvoljna vrednost. Koristi se u prvom pozivu setupContent. U drugom pozivu setupContent, imeWidth,
                    // klubWidth i kategorijaWidth ce dobiti pravu vrednost, koja je dovoljna da i najduzi tekst stane bez
                    // odsecanja.
                    imeWidth = 1f;
                    klubWidth = 1f;
                    kategorijaWidth = 1f;
                }
            }
            else if (columnWidths.Count == 3)
            {
                // Drugi pass, sirine kolona su podesene
                imeWidth = columnWidths[0];
                klubWidth = columnWidths[1];
                kategorijaWidth = columnWidths[2];
            }
            else
            {
                throw new Exception("Trenutno, samo 3 kolone mogu da se podesavaju");
            }
            createColumns(g, contentBounds, imeWidth, klubWidth, kategorijaWidth);

            // Then, layout contents vertically

            if (sprava == Sprava.Preskok)
                itemHeight = itemFont.GetHeight(g) * 4.0f;
            else
                itemHeight = itemFont.GetHeight(g) * 2.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth,
            float kategorijaWidth)
        {
            string redBrojTitle = Opcije.Instance.RedBrojString;
            float redBrojWidth = getColumnWidth(g, REDNI_BROJ_MAX_TEXT, redBrojTitle);

            string brojTitle = Opcije.Instance.BrojString;
            float brojWidth = getColumnWidth(g, BROJ_MAX_TEXT, brojTitle);

            string imeTitle = Opcije.Instance.ImeString;
            imeWidth = getColumnWidth(g, imeWidth, imeTitle);

            string klubTitle = Opcije.Instance.KlubDrzavaString;
            klubWidth = getColumnWidth(g, klubWidth, klubTitle);

            string kategorijaTitle = Opcije.Instance.KategorijaString;
            kategorijaWidth = getColumnWidth(g, kategorijaWidth, kategorijaTitle);

            string skokTitle = "";
            float skokWidth = getColumnWidth(g, SKOK_MAX_TEXT, skokTitle);

            string spravaDTitle = Opcije.Instance.DString;
            string spravaETitle = Opcije.Instance.EString;
            string spravaBonusTitle = Opcije.Instance.BonusString;
            string spravaPenTitle = Opcije.Instance.PenaltyString;
            string totalTitle = Opcije.Instance.TotalString;

            float ocenaWidthCm = 2.1f * (itemFont.Size / 10f);  // Izraz u zagradi omogucuje skaliranje ako promenim
                                                                // font size. Vrednost 2.1f odgovara fontu 10
            float ocenaWidth;

            float xRedBroj;
            float xBroj;
            float xIme;
            float xKategorija;
            float xKlub;
            float xSkok;
            float xSprava;
            float xTotal;
            float xCurr;
            do {
                // Smanjuj ocenaWidth dok sve ne stane na stranu
                ocenaWidthCm -= 0.1f;
                ocenaWidth = Izvestaj.convCmToInch(ocenaWidthCm);

                xRedBroj = contentBounds.X;
                xCurr = xRedBroj + redBrojWidth;
                xBroj = 0f;
                if (stampajBroj)
                {
                    xBroj = xCurr;
                    xCurr += brojWidth;
                }
                xIme = xCurr;
                xCurr += imeWidth;
                xKlub = 0f;
                if (stampajKlub)
                {
                    xKlub = xCurr;
                    xCurr += klubWidth;
                }
                xKategorija = 0f;
                if (stampajKategoriju)
                {
                    xKategorija = xCurr;
                    xCurr += kategorijaWidth;
                }
                xSkok = 0f;
                if (sprava == Sprava.Preskok)
                {
                    xSkok = xCurr;
                    xCurr += skokWidth;
                }
                xSprava = xCurr;
                int brojOcena = stampajBonus ? 5 : 4;
                xTotal = xSprava + ocenaWidth * (brojOcena + brojEOcena);

                xRightEnd = xSprava + ocenaWidth * (brojOcena + brojEOcena);
                if (sprava == Sprava.Preskok)
                    xRightEnd += ocenaWidth;
            } while (xRightEnd - xRedBroj > contentBounds.Width);

            float[] xEn = new float[6];
            for (int i = 0; i < brojEOcena; i++)
            {
                if (i == 0)
                    xEn[i] = xSprava + ocenaWidth;
                else
                    xEn[i] = xEn[i - 1] + ocenaWidth;
            }
            float xE;
            if (brojEOcena == 0)
                xE = xSprava + ocenaWidth;
            else
                xE = xEn[brojEOcena - 1] + ocenaWidth;
            xCurr = xE + ocenaWidth;
            float xBonus = 0f;
            if (stampajBonus)
            {
                xBonus = xCurr;
                xCurr += ocenaWidth;
            }
            float xPen = xCurr;
            float xTot = xPen + ocenaWidth;

            StringFormat redBrojFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat kategorijaFormat = Izvestaj.nearCenterFormat;
            StringFormat skokFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;

            StringFormat redBrojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat kategorijaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat skokHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            Columns.Clear();

            addColumn(xRedBroj, redBrojWidth, redBrojFormat, redBrojTitle, redBrojHeaderFormat);
            if (stampajBroj)
            {
                addColumn(xBroj, brojWidth, brojFormat, brojTitle, brojHeaderFormat);
            }
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKlub, klubWidth, klubFormat, klubTitle,
                klubHeaderFormat);
            column.Visible = stampajKlub;
            column = addColumn(xKategorija, kategorijaWidth, kategorijaFormat, kategorijaTitle, kategorijaHeaderFormat);
            column.Visible = stampajKategoriju;
            if (sprava == Sprava.Preskok)
            {
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSkok, skokWidth, null, skokFormat,
                  skokTitle, skokHeaderFormat, true);
            }

            ReportColumn column1;
            if (sprava == Sprava.Preskok)
                column1 = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSprava, ocenaWidth, "", spravaFormat,
                    spravaDTitle, spravaHeaderFormat, true);
            else
                column1 = addColumn(xSprava, ocenaWidth, "", spravaFormat, spravaDTitle, spravaHeaderFormat);
            column1.Image = SlikeSprava.getImage(sprava);
            column1.Split = true;
            column1.Span = true;

            if (brojEOcena > 0)
            {
                for (int i = 0; i < brojEOcena; i++)
                {
                    if (sprava == Sprava.Preskok)
                    {
                        ReportColumn prevColumn = column;
                        if (i == 0)
                            prevColumn = column1;
                        column = addDvaPreskokaColumn(prevColumn.getItemsIndexEnd(), 2, xEn[i], ocenaWidth, "",
                            spravaFormat, spravaETitle + (i + 1).ToString(), spravaHeaderFormat, true);
                    }
                    else
                        column = addColumn(xEn[i], ocenaWidth, "", spravaFormat, spravaETitle + (i + 1).ToString(),
                            spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprava);
                    column.Split = true;
                }
            }

            ReportColumn prevColumn2 = column;
            if (brojEOcena == 0)
                prevColumn2 = column1;
            if (sprava == Sprava.Preskok)
                column = addDvaPreskokaColumn(prevColumn2.getItemsIndexEnd(), 2, xE, ocenaWidth, "", spravaFormat,
                    spravaETitle, spravaHeaderFormat, true);
            else
                column = addColumn(xE, ocenaWidth, "", spravaFormat, spravaETitle, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (stampajBonus)
            {
                if (sprava == Sprava.Preskok)
                    column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xBonus, ocenaWidth, "", spravaFormat,
                        spravaBonusTitle, spravaHeaderFormat, true);
                else
                    column = addColumn(xBonus, ocenaWidth, "", spravaFormat, spravaBonusTitle, spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            if (sprava == Sprava.Preskok)
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xPen, ocenaWidth, "", spravaFormat,
                    spravaPenTitle, spravaHeaderFormat, true);
            else
                column = addColumn(xPen, ocenaWidth, "", spravaFormat, spravaPenTitle, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (sprava == Sprava.Preskok)
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xTot, ocenaWidth, "", spravaFormat,
                    totalTitle, spravaHeaderFormat, true);
            else
                column = addColumn(xTot, ocenaWidth, "", spravaFormat, totalTitle, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (column1.Span)
                column1.SpanEndColumn = column;

            if (sprava == Sprava.Preskok)
            {
                column = addColumn(column.getItemsIndexEnd(), xTotal, ocenaWidth, "", totalFormat, totalTitle,
                    totalHeaderFormat);
            }
        }

        protected override void drawGroupHeader(Graphics g, int groupId, RectangleF groupHeaderRect)
        {
            foreach (ReportColumn col in Columns)
            {
                if (!col.Visible)
                    continue;

                RectangleF columnHeaderRect = new RectangleF(
                    col.X, groupHeaderRect.Y, col.Width, groupHeaderRect.Height);

                if (col.Image != null)
                {
                    if (col.Split)
                    {
                        float imageHeight = (2f / 3) * columnHeaderRect.Height;
                        RectangleF textRect = new RectangleF(
                            columnHeaderRect.X, columnHeaderRect.Y + imageHeight, columnHeaderRect.Width,
                            columnHeaderRect.Height - imageHeight);
                        g.DrawRectangle(pen, textRect.X, textRect.Y,
                            textRect.Width, textRect.Height);
                        g.DrawString(col.HeaderTitle, itemsHeaderFont, blackBrush,
                            textRect, col.HeaderFormat);

                        if (col.Span)
                        {
                            RectangleF imageRect = new RectangleF(
                                col.X, groupHeaderRect.Y,
                                col.SpanEndColumn.X + col.SpanEndColumn.Width - col.X,
                                imageHeight);
                            g.DrawRectangle(pen, imageRect.X, imageRect.Y,
                                imageRect.Width, imageRect.Height);
                            Izvestaj.scaleImageIsotropically(g, col.Image, imageRect);
                        }
                    }
                    else
                    {
                        Izvestaj.scaleImageIsotropically(g, col.Image, columnHeaderRect);
                    }
                }
                else
                {
                    if (col.DrawHeaderRect)
                    {
                        g.DrawRectangle(pen, columnHeaderRect.X, columnHeaderRect.Y,
                            columnHeaderRect.Width, columnHeaderRect.Height);
                    }
                    g.DrawString(col.HeaderTitle, itemsHeaderFont, blackBrush,
                        columnHeaderRect, col.HeaderFormat);
                }
            }
        }

        private DvaPreskokaReportColumn addDvaPreskokaColumn(int itemsIndex, int itemsSpan, float x, float width,
          string format, StringFormat itemRectFormat, string headerTitle,
          StringFormat headerFormat, bool drawPartItemRect)
        {
            DvaPreskokaReportColumn result = new DvaPreskokaReportColumn(
                itemsIndex, itemsSpan, x, width, headerTitle);
            result.Format = format;
            result.ItemRectFormat = itemRectFormat;
            result.HeaderFormat = headerFormat;
            result.DrawPartItemRect = drawPartItemRect;
            Columns.Add(result);
            return result;
        }
    }
}
