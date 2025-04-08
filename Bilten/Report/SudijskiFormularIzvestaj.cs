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
        private bool svakaSpravaNaPosebnojStrani;

        public SudijskiFormularIzvestaj(StartListaNaSpravi startLista, string documentName, int brojEOcena, 
            bool stampajRedniBroj, bool stampajKategoriju, bool stampajKlub, DataGridView formGrid, Takmicenje takmicenje,
            bool stampajBonus, Font itemFont)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            Landscape = true;
            Margins = new Margins(30, 30, 75, 75);

            reportListe.Add(new SudijskiFormularLista(this, 1, 0f, itemFont, itemsHeaderFont, startLista, brojEOcena,
                stampajRedniBroj, stampajKategoriju, stampajKlub, formGrid, takmicenje.TakBrojevi, stampajBonus));
		}

        public SudijskiFormularIzvestaj(List<StartListaNaSpravi> startListe, Gimnastika gim,
            string documentName, int brojEOcena, int brojSpravaPoStrani, bool stampajRedniBroj,
            bool stampajKategoriju, bool stampajKlub, SpravaGridGroupUserControl spravaGridGroupUserControl,
            Takmicenje takmicenje, bool stampajBonus, Font itemFont)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;

            Landscape = true;
            Margins = new Margins(30, 30, 75, 75);
            
            Sprava[] sprave = Sprave.getSprave(gim);
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                SudijskiFormularLista lista = new SudijskiFormularLista(this, 1, 0f, itemFont, itemsHeaderFont,
                    startListe[i], brojEOcena, stampajRedniBroj, stampajKategoriju, stampajKlub,
                    spravaGridGroupUserControl[sprava].DataGridViewUserControl.DataGridView, takmicenje.TakBrojevi,
                    stampajBonus);
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            float startYPrvaStrana = contentBounds.Y;
            float startYOstaleStrane = contentBounds.Y;

            // Radim dvaput setupContent. Prvi put sluzi samo da odredim maximume kolona ime i klub u svim listama.
            float maxImeWidth = 0.0f;
            float maxKlubWidth = 0.0f;
            float maxKategorijaWidth = 0.0f;
            SudijskiFormularLista prevLista = null;
            for (int i = 0; i < 2; ++i)
            {
                prevLista = null;
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    SudijskiFormularLista lista = reportListe[j];
                    if (prevLista == null)
                    {
                        lista.FirstPageNum = 1;
                        lista.StartY = startYPrvaStrana;
                    }
                    else if (svakaSpravaNaPosebnojStrani || prebaciNaSledecuStranu)
                    {
                        lista.FirstPageNum = prevLista.LastPageNum + 1;
                        lista.StartY = startYOstaleStrane;
                        prebaciNaSledecuStranu = false;
                    }
                    else
                    {
                        // Nastavak na istoj strani
                        lista.FirstPageNum = prevLista.LastPageNum;
                        // Svaka lista ima implicitno dodat prazan prostor nakon liste (koji je jednak velicini vrste),
                        // i EndY pokazuje nakon tog praznog prostoja.
                        lista.StartY = prevLista.EndY;
                    }

                    int firstPageNum = lista.FirstPageNum;
                    if (i == 0)
                    {
                        lista.setupContent(g, contentBounds);
                        if (lista.Columns[lista.getImeColumnIndex()].Width > maxImeWidth)
                            maxImeWidth = lista.Columns[lista.getImeColumnIndex()].Width;
                        if (lista.Columns[lista.getKlubColumnIndex()].Width > maxKlubWidth)
                            maxKlubWidth = lista.Columns[lista.getKlubColumnIndex()].Width;
                        if (lista.Columns[lista.getKategorijaColumnIndex()].Width > maxKategorijaWidth)
                            maxKategorijaWidth = lista.Columns[lista.getKategorijaColumnIndex()].Width;
                    }
                    else
                    {
                        lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth, maxKategorijaWidth);
                    }

                    if (lista.LastPageNum == firstPageNum)
                    {
                        // Cela lista je stala na istu stranu
                        ++j;
                        prevLista = lista;
                    }
                    else
                    {
                        // Lista nije stala na istu stranu
                        float prvaStranaListHeight = contentBounds.Bottom - lista.StartY;
                        float zadnjaStranaListHeight = lista.EndY - contentBounds.Top;
                        if (prvaStranaListHeight + zadnjaStranaListHeight >= contentBounds.Height)
                        {
                            // Lista ne moze cela da stane na stranu cak i da pocnemo sa vrha strane, pa mora da ostane
                            // izlomljena (prvi deo na jednoj strani, drugi deo na drugoj strani).
                            ++j;
                            prevLista = lista;
                        }
                        else
                        {
                            // Lista nije stala na istu stranu pa je prebacujemo da pocinje na sledecoj strani.
                            prebaciNaSledecuStranu = true;
                        }
                    }
                }
            }
            lastPageNum = prevLista.LastPageNum;
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
        private bool stampajBroj;
        private bool stampajBonus;

        public SudijskiFormularLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, StartListaNaSpravi startLista, int brojEOcena, bool stampajRedniBroj,
            bool stampajKategoriju, bool stampajKlub, DataGridView formGrid, bool stampajBroj, bool stampajBonus)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = startLista.Sprava;
            this.brojEOcena = brojEOcena;
            this.stampajRedniBroj = stampajRedniBroj;
            this.stampajKategoriju = stampajKategoriju;
            this.stampajKlub = stampajKlub;
            this.stampajBroj = stampajBroj;
            this.stampajBonus = stampajBonus;

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

        public void setupContent(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth, float kategorijaWidth)
        {
            createColumns(g, contentBounds, imeWidth, klubWidth, kategorijaWidth);

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

            float rankWidthCm = 0.75f;
            float rankWidth = Izvestaj.convCmToInch(rankWidthCm);

            float brojWidth = 1.3f * rankWidth;

            float skokWidthCm = 0.5f;
            float skokWidth = Izvestaj.convCmToInch(skokWidthCm);

            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;

            float klubWidth;
            if (this.stampajKlub)
                klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            else
                klubWidth = 0.0F;

            float kategorijaWidth;
            if (this.stampajKategoriju)
                kategorijaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
            else
                kategorijaWidth = 0.0F;

            doCreateColumns(g, contentBounds, rankWidth, brojWidth, skokWidth, imeWidth, klubWidth, kategorijaWidth);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth,
         float kategorijaWidth)
        {
            float rankWidthCm = 0.75f;
            float rankWidth = Izvestaj.convCmToInch(rankWidthCm);

            float brojWidth = 1.3f * rankWidth;

            float skokWidthCm = 0.5f;
            float skokWidth = Izvestaj.convCmToInch(skokWidthCm);

            doCreateColumns(g, contentBounds, rankWidth, brojWidth, skokWidth, imeWidth, klubWidth, kategorijaWidth);
        }

        private void doCreateColumns(Graphics g, RectangleF contentBounds, float rankWidth, float brojWidth,
            float skokWidth, float imeWidth, float klubWidth, float kategorijaWidth)
        {
            float ocenaWidthCm = 2.1f;
            float ocenaWidth;

            float xRank;
            float xBroj;
            float xIme;
            float xKategorija;
            float xKlub;
            float xSkok;
            float xSprava;
            float xTotal;
            float xRightEnd;
            do {
                ocenaWidthCm -= 0.1f;
                ocenaWidth = Izvestaj.convCmToInch(ocenaWidthCm);

                xRank = contentBounds.X;
                xBroj = 0f;
                if (stampajBroj)
                {
                    xBroj = xRank + rankWidth;
                    xIme = xBroj + brojWidth;
                }
                else
                {
                    xIme = xRank + rankWidth;
                }
                xKlub = xIme + imeWidth;
                xKategorija = xKlub + klubWidth;
                xSkok = 0.0f;
                if (sprava == Sprava.Preskok)
                {
                    xSkok = xKategorija + kategorijaWidth;
                    xSprava = xSkok + skokWidth;
                }
                else
                    xSprava = xKategorija + kategorijaWidth;
                int brojOcena = stampajBonus ? 5 : 4;
                xTotal = xSprava + ocenaWidth * (brojOcena + brojEOcena);

                xRightEnd = xSprava + ocenaWidth * (brojOcena + brojEOcena);
                if (sprava == Sprava.Preskok)
                    xRightEnd += ocenaWidth;
            } while (xRightEnd - xRank > contentBounds.Width);

            float delta = (contentBounds.Right - xRightEnd) / 2;  // moze da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;

            xRank += delta;
            xBroj += delta;
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
            float xBonus = 0f;
            float xCurr;
            if (stampajBonus)
            {
                xBonus = xE + ocenaWidth;
                xCurr = xBonus;
            }
            else
            {
                xCurr = xE;
            }
            float xPen = xCurr + ocenaWidth;
            float xTot = xPen + ocenaWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;

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
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat kategorijaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat skokHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = Opcije.Instance.RedBrojString;
            String brojTitle = Opcije.Instance.BrojString;
            String imeTitle = Opcije.Instance.ImeString;
            String klubTitle = Opcije.Instance.KlubDrzavaString;
            String kategorijaTitle = Opcije.Instance.KategorijaString;
            String skokTitle = "";
            String totalTitle = Opcije.Instance.TotalString;

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            if (stampajBroj)
            {
                addColumn(xBroj, brojWidth, brojFormat, brojTitle, brojHeaderFormat);
            }
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            column.Visible = stampajKlub;
            column = addColumn(xKategorija, kategorijaWidth, kategorijaFormat, kategorijaTitle, kategorijaHeaderFormat);
            column.Visible = stampajKategoriju;
            if (sprava == Sprava.Preskok)
            {
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSkok, skokWidth, null, skokFormat,
                  skokTitle, skokHeaderFormat, true);
            }

            // TODO5: Ovo nije potrebno jer se ne stampaju nikakve ocene vec samo prazan formular
            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtBon = "F" + Opcije.Instance.BrojDecimalaBon;
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

            if (stampajBonus)
            {
                if (sprava == Sprava.Preskok)
                    column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xBonus, ocenaWidth, fmtBon, spravaFormat,
                        "Bonus", spravaHeaderFormat, true);
                else
                    column = addColumn(xBonus, ocenaWidth, fmtBon, spravaFormat, "Bonus", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            if (sprava == Sprava.Preskok)
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xPen, ocenaWidth, fmtPen, spravaFormat,
                    "Pen.", spravaHeaderFormat, true);
            else
                column = addColumn(xPen, ocenaWidth, fmtPen, spravaFormat, "Pen.", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (sprava == Sprava.Preskok)
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xTot, ocenaWidth, fmtTot, spravaFormat, 
                    Opcije.Instance.TotalString, spravaHeaderFormat, true);
            else
                column = addColumn(xTot, ocenaWidth, fmtTot, spravaFormat, Opcije.Instance.TotalString, spravaHeaderFormat);
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

        public int getImeColumnIndex()
        {
            return stampajBroj ? 2 : 1;
        }

        public int getKlubColumnIndex()
        {
            return stampajBroj ? 3 : 2;
        }

        public int getKategorijaColumnIndex()
        {
            return stampajBroj ? 4 : 3;
        }
    }
}
