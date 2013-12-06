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
        private List<SudijskiFormularLista> reportListe = new List<SudijskiFormularLista>();
        private float itemFontSize = 10;
        private bool svakaSpravaNaPosebnojStrani;

        public SudijskiFormularIzvestaj(StartListaNaSpravi startLista, string documentName, int brojEOcena, 
            bool stampajRedniBroj, bool stampajKategoriju, bool stampajKlub, DataGridView formGrid)
		{
            DocumentName = documentName;
            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            Landscape = true;
            Margins = new Margins(30, 30, 75, 75);

            SudijskiFormularLista lista = new SudijskiFormularLista(this, 1, 0f, itemFont, itemsHeaderFont, startLista,
                brojEOcena, stampajRedniBroj, stampajKategoriju, stampajKlub, formGrid);
            lista.RelY = 0.0f + 0.03f;
            reportListe.Add(lista);
		}

        public SudijskiFormularIzvestaj(List<StartListaNaSpravi> startListe, Gimnastika gim,
            string documentName, int brojEOcena, int brojSpravaPoStrani, bool stampajRedniBroj,
            bool stampajKategoriju, bool stampajKlub, SpravaGridGroupUserControl spravaGridGroupUserControl)
        {
            DocumentName = documentName;
            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;

            Landscape = true;
            Margins = new Margins(30, 30, 75, 75);
            
            Sprava[] sprave = Sprave.getSprave(gim);
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                int page;
                float relY;
                if (brojSpravaPoStrani != 1)
                {
                    page = (i / brojSpravaPoStrani) + 1;
                    relY = (i % brojSpravaPoStrani) / (brojSpravaPoStrani * 1f) + 0.03f;
                }
                else
                {
                    page = i + 1;
                    relY = 0.0f + 0.03f;
                }
                SudijskiFormularLista lista = new SudijskiFormularLista(this, page, 0f, itemFont, itemsHeaderFont,
                    startListe[i], brojEOcena, stampajRedniBroj, stampajKategoriju, stampajKlub,
                    spravaGridGroupUserControl[sprava].DataGridViewUserControl.DataGridView);
                lista.RelY = relY;
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            lastPageNum = 0;
            foreach (SudijskiFormularLista lista in reportListe)
            {
                if (svakaSpravaNaPosebnojStrani)
                    lista.FirstPageNum = lastPageNum + 1;
                lista.StartY = contentBounds.Y + lista.RelY * contentBounds.Height;
                lista.setupContent(g, contentBounds);
                lastPageNum = lista.LastPageNum;
            }
        }

        public override void drawContent(Graphics g, int pageNum)
        {
            foreach (SudijskiFormularLista lista in reportListe)
            {
                lista.drawContent(g, contentBounds, pageNum);
            }
        }
    }

    public class SudijskiFormularLista : ReportLista
    {
        private Sprava sprava;
        private int brojEOcena;
        private bool stampajRedniBroj;
        private bool stampajKategoriju;
        private bool stampajKlub;

        public SudijskiFormularLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, StartListaNaSpravi startLista, int brojEOcena, bool stampajRedniBroj,
            bool stampajKategoriju, bool stampajKlub, DataGridView formGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = startLista.Sprava;
            this.brojEOcena = brojEOcena;
            this.stampajRedniBroj = stampajRedniBroj;
            this.stampajKategoriju = stampajKategoriju;
            this.stampajKlub = stampajKlub;

            fetchItems(startLista);
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
                if (startLista.Sprava != Sprava.Preskok)
                {
                    if (brojEOcena == 0)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava,
                            "", "", "", "" });
                    else if (brojEOcena == 1)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava,
                            "", "", "", "", "" });
                    else if (brojEOcena == 2)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava,
                            "", "", "", "", "", "" });
                    else if (brojEOcena == 3)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava,
                            "", "", "", "", "", "", "" });
                    else if (brojEOcena == 4)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava,
                            "", "", "", "", "", "", "", "" });
                    else if (brojEOcena == 5)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava,
                            "", "", "", "", "", "", "", "", "" });
                    else if (brojEOcena == 6)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava,
                            "", "", "", "", "", "", "", "", "", "" });
                }
                else
                {
                    if (brojEOcena == 0)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava, "1", "2",
                            "", "", "", "", 
                            "", "", "", "", 
                            ""});
                    else if (brojEOcena == 1)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava, "1", "2",
                            "", "", "", "", "",
                            "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 2)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava, "1", "2",
                            "", "", "", "", "", "",
                            "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 3)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava, "1", "2",
                            "", "", "", "", "", "", "",
                            "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 4)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava, "1", "2",
                            "", "", "", "", "", "", "", "",
                            "", "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 5)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava, "1", "2",
                            "", "", "", "", "", "", "", "", "",
                            "", "", "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 6)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.Kategorija, nastup.KlubDrzava, "1", "2",
                            "", "", "", "", "", "", "", "", "", "",
                            "", "", "", "", "", "", "", "", "", "",
                            "" });
                }
            }
            return result;
        }

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);

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

        private void createColumns(Graphics g, RectangleF contentBounds)
        {
            float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
            float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;

            float rankWidthCm = 0.7f;
            float skokWidthCm = 0.5f;
            float ocenaWidthCm = 2.0f;
            if (brojEOcena > 0)
                ocenaWidthCm = 1.9f;

            float rankWidth = Izvestaj.convCmToInch(rankWidthCm);
            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;

            float kategorijaWidth;
            if (this.stampajKategoriju)
                kategorijaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
            else
                kategorijaWidth = 0.0F;

            float klubWidth;
            if (this.stampajKlub)
                klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            else
                klubWidth = 0.0F;

            float skokWidth = Izvestaj.convCmToInch(skokWidthCm);
            float ocenaWidth = Izvestaj.convCmToInch(ocenaWidthCm);

            float xRank = contentBounds.X;
            float xIme = xRank + Izvestaj.convCmToInch(rankWidthCm);
            float xKategorija = xIme + imeWidth;
            float xKlub = xKategorija + kategorijaWidth;
            float xSkok = 0.0f;
            float xSprava;
            if (sprava == Sprava.Preskok)
            {
                xSkok = xKlub + klubWidth;
                xSprava = xSkok + Izvestaj.convCmToInch(skokWidthCm);
            }
            else
                xSprava = xKlub + klubWidth;
            float xTotal = xSprava + ocenaWidth * (4 + brojEOcena);

            float xRightEnd = xSprava + ocenaWidth * (4 + brojEOcena);
            if (sprava == Sprava.Preskok)
                xRightEnd += ocenaWidth;

            float delta = (contentBounds.Right - xRightEnd) / 2;  // moza da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;

            xRank += delta;
            xIme += delta;
            xKlub += delta;
            xKategorija += delta;
            if (sprava == Sprava.Preskok)
                xSkok += delta;
            xSprava += delta;
            xTotal += delta;
            xRightEnd += delta;

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
            float xPen = xE + ocenaWidth;
            float xTot = xPen + ocenaWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.Alignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.Alignment = StringAlignment.Near;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat kategorijaFormat = new StringFormat(StringFormatFlags.NoWrap);
            kategorijaFormat.Alignment = StringAlignment.Near;
            kategorijaFormat.LineAlignment = StringAlignment.Center;

            StringFormat skokFormat = new StringFormat(StringFormatFlags.NoWrap);
            skokFormat.Alignment = StringAlignment.Center;
            skokFormat.LineAlignment = StringAlignment.Center;

            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat kategorijaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat skokHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = "RB";
            String imeTitle = "Ime";
            String klubTitle = "Klub";
            String kategorijaTitle = "Kategorija";
            String skokTitle = ""; // TODO3: Neka bude uspravno.
            String totalTitle = "Total";

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKategorija, kategorijaWidth, kategorijaFormat, kategorijaTitle, kategorijaHeaderFormat);
            column.Visible = stampajKategoriju;
            column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            column.Visible = stampajKlub;
            if (sprava == Sprava.Preskok)
            {
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSkok, skokWidth, null, skokFormat,
                  skokTitle, skokHeaderFormat, true);
            }

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            ReportColumn column1;
            if (sprava == Sprava.Preskok)
                column1 = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSprava, ocenaWidth, fmtD, spravaFormat,
                    "D", spravaHeaderFormat, true);
            else
                column1 = addColumn(xSprava, ocenaWidth, fmtD, spravaFormat, "D", spravaHeaderFormat);
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
                        column = addDvaPreskokaColumn(prevColumn.getItemsIndexEnd(), 2, xEn[i], ocenaWidth, fmtE, 
                            spravaFormat, "E" + (i + 1).ToString(), spravaHeaderFormat, true);
                    }
                    else
                        column = addColumn(xEn[i], ocenaWidth, fmtE, spravaFormat, "E" + (i + 1).ToString(),
                            spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprava);
                    column.Split = true;
                }
            }

            ReportColumn prevColumn2 = column;
            if (brojEOcena == 0)
                prevColumn2 = column1;
            if (sprava == Sprava.Preskok)
                column = addDvaPreskokaColumn(prevColumn2.getItemsIndexEnd(), 2, xE, ocenaWidth, fmtE, spravaFormat, 
                    "E", spravaHeaderFormat, true);
            else
                column = addColumn(xE, ocenaWidth, fmtE, spravaFormat, "E", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (sprava == Sprava.Preskok)
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xPen, ocenaWidth, fmtPen, spravaFormat,
                    "Pen.", spravaHeaderFormat, true);
            else
                column = addColumn(xPen, ocenaWidth, fmtPen, spravaFormat, "Pen.", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (sprava == Sprava.Preskok)
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xTot, ocenaWidth, fmtTot, spravaFormat, 
                    "Total", spravaHeaderFormat, true);
            else
                column = addColumn(xTot, ocenaWidth, fmtTot, spravaFormat, "Total", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (column1.Span)
                column1.SpanEndColumn = column;

            if (sprava == Sprava.Preskok)
            {
                column = addColumn(column.getItemsIndexEnd(), xTotal, ocenaWidth, fmtTot, totalFormat,
                    totalTitle, totalHeaderFormat);
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
