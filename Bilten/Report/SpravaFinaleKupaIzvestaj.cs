using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;
using System.Windows.Forms;

namespace Bilten.Report
{
    class SpravaFinaleKupaIzvestaj : Izvestaj
    {
        private List<SpravaFinaleKupaLista> reportListe = new List<SpravaFinaleKupaLista>();
        private bool svakaSpravaNaPosebnojStrani;

        public SpravaFinaleKupaIzvestaj(Sprava sprava, IList<RezultatSpravaFinaleKupa> rezultati,
            bool kvalColumn, string documentName, DataGridView formGrid, Takmicenje takmicenje, Font itemFont)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            reportListe.Add(new SpravaFinaleKupaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                kvalColumn, sprava, formGrid));
        }

        public SpravaFinaleKupaIzvestaj(List<List<RezultatSpravaFinaleKupa>> rezultatiSprave,
            Gimnastika gim, bool kvalColumn, string documentName, int brojSpravaPoStrani, DataGridView formGrid,
            Takmicenje takmicenje, Font itemFont)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;

            Sprava[] sprave = Sprave.getSprave(gim);
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                SpravaFinaleKupaLista lista = new SpravaFinaleKupaLista(this, 1, 0f, itemFont, itemsHeaderFont,
                    rezultatiSprave[i], kvalColumn, sprava, formGrid);
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
            SpravaFinaleKupaLista prevLista = null;
            for (int i = 0; i < 2; ++i)
            {
                prevLista = null;
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    SpravaFinaleKupaLista lista = reportListe[j];
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
                        if (lista.Columns[1].Width > maxImeWidth)
                            maxImeWidth = lista.Columns[1].Width;
                        if (lista.Columns[2].Width > maxKlubWidth)
                            maxKlubWidth = lista.Columns[2].Width;
                    }
                    else
                    {
                        lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth);
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
            foreach (SpravaFinaleKupaLista lista in reportListe)
            {
                lista.drawContent(g, contentBounds, pageNum);
            }
        }
    }

    public class SpravaFinaleKupaLista : ReportLista
    {
        private Brush totalBrush;
        private Brush totalAllBrush;

        private bool kvalColumn;
        private Sprava sprava;

        public SpravaFinaleKupaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatSpravaFinaleKupa> rezultati,
            bool kvalColumn, Sprava sprava, DataGridView formGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.kvalColumn = kvalColumn;
            this.sprava = sprava;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati);
        }

        private void fetchItems(IList<RezultatSpravaFinaleKupa> rezultati)
        {
            items = getSpravaFinaleKupaReportItems(rezultati);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getSpravaFinaleKupaReportItems(IList<RezultatSpravaFinaleKupa> rezultati)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatSpravaFinaleKupa rez in rezultati)
            {
                result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava, "I", "II",
                            rez.D_PrvoKolo, rez.D_DrugoKolo, rez.E_PrvoKolo, rez.E_DrugoKolo, rez.TotalPrvoKolo,
                            rez.TotalDrugoKolo, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
            }
            return result;
        }

        // TODO5: Izdvoj ovaj method i u ostalim izvestajima gde radim dvaput setupContent
        private void createLayout(Graphics g, RectangleF contentBounds)
        {
            itemHeight = itemFont.GetHeight(g) * 2.9f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);
            createLayout(g, contentBounds);
        }

        public void setupContent(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
        {
            createColumns(g, contentBounds, imeWidth, klubWidth);
            createLayout(g, contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds)
        {
            float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
            float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;

            float rankWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float ocenaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;

            doCreateColumns(g, contentBounds, rankWidth, imeWidth, klubWidth, ocenaWidth);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
        {
            float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
            float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;

            float rankWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
            float ocenaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;

            doCreateColumns(g, contentBounds, rankWidth, imeWidth, klubWidth, ocenaWidth);
        }

        private void doCreateColumns(Graphics g, RectangleF contentBounds, float rankWidth, float imeWidth, float klubWidth,
            float ocenaWidth)
        {
            // kolo i kval sam podesio kao polovinu Rank kolone.
            float koloWidth = rankWidth / 2;
            float kvalWidth = rankWidth / 2;
            
            int brojOcena = 3;

            float xRank = contentBounds.X;
            float xIme = xRank + rankWidth;
            float xKlub = xIme + imeWidth;
            float xKolo = 0.0f;
            float xSprava;
            xKolo = xKlub + klubWidth;
            xSprava = xKolo + koloWidth;

            float xTotal = xSprava + (ocenaWidth * brojOcena);
            float xKval = xTotal + ocenaWidth;

            xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;

            float delta = (contentBounds.Right - xRightEnd) / 2;  // moze da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;

            xRank += delta;
            xIme += delta;
            xKlub += delta;
            xKolo += delta;
            xSprava += delta;
            xTotal += delta;
            xKval += delta;
            xRightEnd += delta;

            float xE = xSprava + ocenaWidth;
            float xPen = xE + ocenaWidth;
            float xTot = xPen;

            float spravaDWidth = ocenaWidth;
            float spravaEWidth = ocenaWidth;
            float spravaTotWidth = ocenaWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.Alignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.Alignment = StringAlignment.Near;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat koloFormat = new StringFormat(StringFormatFlags.NoWrap);
            koloFormat.Alignment = StringAlignment.Center;
            koloFormat.LineAlignment = StringAlignment.Center;

            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat koloHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = "Rank";
            String imeTitle = "Ime";
            String klubTitle = "Klub";
            String koloTitle = "";
            String totalTitle = "Total";
            String kvalTitle = String.Empty;

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            column = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), 2, xKolo, koloWidth, null, koloFormat,
              koloTitle, koloHeaderFormat);

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            ReportColumn column1;
            column1 = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), 2, xSprava, spravaDWidth, fmtD,
              spravaFormat, "D", spravaHeaderFormat);
            column1.Image = SlikeSprava.getImage(sprava);
            column1.Split = true;
            column1.Span = true;

            column = addSpravaFinaleKupaColumn(column1.getItemsIndexEnd(), 2, xE, spravaEWidth, fmtE, spravaFormat,
              "E", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            string title = "Total";
            column = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), 2, xTot, spravaTotWidth, fmtTot, spravaFormat,
                title, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;
            column.Brush = totalBrush;

            if (column1.Span)
                column1.SpanEndColumn = column;

            column = addColumn(column.getItemsIndexEnd(), xTotal, ocenaWidth, fmtTot, totalFormat,
                totalTitle, totalHeaderFormat);
            column.Brush = totalAllBrush;

            if (kvalColumn)
            {
                column = addColumn(column.getItemsIndexEnd(), xKval, kvalWidth, kvalFormat, kvalTitle);
                column.DrawHeaderRect = false;
                column.DrawItemRect = false;
            }
        }

        private SpravaFinaleKupaReportColumn addSpravaFinaleKupaColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle,
            StringFormat headerFormat)
        {
            SpravaFinaleKupaReportColumn result = new SpravaFinaleKupaReportColumn(
                itemsIndex, itemsSpan, x, width, headerTitle);
            result.Format = format;
            result.ItemRectFormat = itemRectFormat;
            result.HeaderFormat = headerFormat;
            Columns.Add(result);
            return result;
        }

        protected override void drawGroupHeader(Graphics g, int groupId, RectangleF groupHeaderRect)
        {
            foreach (ReportColumn col in Columns)
            {
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
    }

    // TODO3: Ovu klasu bi trebalo merdzovati sa klasom  UkupnoFinaleKupaSpravaReportColumn (isto i za klasu
    // DvaPreskokaReportColumn)
    public class SpravaFinaleKupaReportColumn : ReportColumn
    {
        private bool drawPartItemRect;
        public bool DrawPartItemRect
        {
            get { return drawPartItemRect; }
            set { drawPartItemRect = value; }
        }

        public SpravaFinaleKupaReportColumn(int itemsIndex, int itemsSpan, float x, float width, string headerTitle)
            : base(itemsIndex, x, width, headerTitle)
        {
            this.itemsSpan = itemsSpan;
        }

        public override void draw(Graphics g, Pen pen, object[] itemsRow, Font itemFont, Brush blackBrush)
        {
            if (this.Brush != null)
            {
                g.FillRectangle(this.Brush, itemRect.X, itemRect.Y,
                    itemRect.Width, itemRect.Height);
            }
            if (this.DrawItemRect)
            {
                g.DrawRectangle(pen, itemRect.X, itemRect.Y,
                    itemRect.Width, itemRect.Height);
            }

            RectangleF itemRect1 = new RectangleF(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 2);
            RectangleF itemRect2 = new RectangleF(itemRect.X, itemRect.Y + itemRect.Height / 2, itemRect.Width,
                itemRect.Height / 2);

            if (this.DrawPartItemRect)
            {
                g.DrawRectangle(pen, itemRect1.X, itemRect1.Y,
                    itemRect1.Width, itemRect1.Height);
                g.DrawRectangle(pen, itemRect2.X, itemRect2.Y,
                    itemRect2.Width, itemRect2.Height);
            }

            string item1 = this.getFormattedString(itemsRow, itemsIndex);
            string item2 = this.getFormattedString(itemsRow, itemsIndex + 1);
            g.DrawString(item1, itemFont, blackBrush, itemRect1, this.ItemRectFormat);
            g.DrawString(item2, itemFont, blackBrush, itemRect2, this.ItemRectFormat);
        }
    }
}
