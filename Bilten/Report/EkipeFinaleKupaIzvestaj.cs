using System;
using System.Drawing;
using Bilten.Domain;
using Bilten.Exceptions;
using System.Collections.Generic;
using Bilten.Data;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Bilten.Report
{
    public class EkipeFinaleKupaIzvestaj : Izvestaj
    {
        private EkipeFinaleKupaLista lista;

        public EkipeFinaleKupaIzvestaj(IList<RezultatEkipnoFinaleKupa> rezultati, Gimnastika gim,
            bool kvalColumn, DataGridView formGrid, string documentName, Takmicenje takmicenje)
            : base(takmicenje)
        {
            DocumentName = documentName;

            Font itemFont = new Font("Arial", 8);
            Font itemsHeaderFont = new Font("Arial", 8, FontStyle.Bold);

            Landscape = false;
            Margins = new Margins(75, 75, 75, 75);

            lista = new EkipeFinaleKupaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                gim, kvalColumn, formGrid);
        }

        protected override void doSetupContent(Graphics g)
        {
            lista.StartY = contentBounds.Y;
            lista.setupContent(g, contentBounds);
            lastPageNum = lista.LastPageNum;
        }

        public override void drawContent(Graphics g, int pageNum)
        {
            lista.drawContent(g, contentBounds, pageNum);
        }
    }

    public class EkipeFinaleKupaLista : ReportLista
    {
        private Brush totalBrush;
        private Brush totalAllBrush;

        private bool kvalColumn;
        private Gimnastika gimnastika;

        public EkipeFinaleKupaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatEkipnoFinaleKupa> rezultati,
            Gimnastika gim, bool kvalColumn, DataGridView formGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.kvalColumn = kvalColumn;
            this.gimnastika = gim;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, gim);
        }

        private void fetchItems(IList<RezultatEkipnoFinaleKupa> rezultati,
            Gimnastika gim)
        {
            items = getEkipnoReportItems(rezultati, gim);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getEkipnoReportItems(IList<RezultatEkipnoFinaleKupa> rezultati,
            Gimnastika gim)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatEkipnoFinaleKupa rez in rezultati)
            {
                if (gim == Gimnastika.MSG)
                {
                    result.Add(new object[] { rez.Rank, rez.Ekipa.Naziv, "I", "II",
                            rez.ParterPrvoKolo, rez.ParterDrugoKolo, rez.KonjPrvoKolo, rez.KonjDrugoKolo,
                            rez.KarikePrvoKolo, rez.KarikeDrugoKolo, rez.PreskokPrvoKolo, rez.PreskokDrugoKolo,
                            rez.RazbojPrvoKolo, rez.RazbojDrugoKolo, rez.VratiloPrvoKolo, rez.VratiloDrugoKolo,
                            rez.TotalPrvoKolo, rez.TotalDrugoKolo, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                }
                else
                {
                    result.Add(new object[] { rez.Rank, rez.Ekipa.Naziv, "I", "II",
                            rez.PreskokPrvoKolo, rez.PreskokDrugoKolo,
                            rez.DvovisinskiRazbojPrvoKolo, rez.DvovisinskiRazbojDrugoKolo,
                            rez.GredaPrvoKolo, rez.GredaDrugoKolo, rez.ParterPrvoKolo, rez.ParterDrugoKolo,
                            rez.TotalPrvoKolo, rez.TotalDrugoKolo, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                }
            }
            return result;
        }

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);

            itemHeight = itemFont.GetHeight(g) * 4.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 2.4f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds)
        {
            float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
            float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;

            float rankWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
            float ekipaWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float koloWidth = rankWidth / 2;

            float spravaWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float totalWidth = spravaWidth;
            float kvalWidth = rankWidth / 2;

            float xRank = contentBounds.X;
            float xEkipa = xRank + rankWidth;
            float xKolo = xEkipa + ekipaWidth;
            float xParter = xKolo + koloWidth;
            float xKonj = xParter + spravaWidth;
            float xKarike = xKonj + spravaWidth;
            float xPreskok = xKarike + spravaWidth;
            float xRazboj = xPreskok + spravaWidth;
            float xVratilo = xRazboj + spravaWidth;
            float xTotal = xVratilo + spravaWidth;
            if (gimnastika == Gimnastika.ZSG)
                xTotal = xRazboj;

            float xKval = xTotal + totalWidth;

            xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;

            float dWidth = (xKonj - xParter) / 3;

            float xParterE = xParter + dWidth;
            float xParterTot = xParterE + dWidth;
            float xKonjE = xKonj + dWidth;
            float xKonjTot = xKonjE + dWidth;
            float xKarikeE = xKarike + dWidth;
            float xKarikeTot = xKarikeE + dWidth;
            float xPreskokE = xPreskok + dWidth;
            float xPreskokTot = xPreskokE + dWidth;
            float xRazbojE = xRazboj + dWidth;
            float xRazbojTot = xRazbojE + dWidth;
            float xVratiloE = xVratilo + dWidth;
            float xVratiloTot = xVratiloE + dWidth;

            float delta = (contentBounds.Right - xRightEnd) / 2;  // moze da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;
            xRank += delta;
            xEkipa += delta;
            xKolo += delta;
            xParter += delta;
            xKonj += delta;
            xKarike += delta;
            xPreskok += delta;
            xRazboj += delta;
            xVratilo += delta;
            xTotal += delta;
            xKval += delta;
            xRightEnd += delta;

            xParterE += delta;
            xKonjE += delta;
            xKarikeE += delta;
            xPreskokE += delta;
            xRazbojE += delta;
            xVratiloE += delta;

            xParterTot += delta;
            xKonjTot += delta;
            xKarikeTot += delta;
            xPreskokTot += delta;
            xRazbojTot += delta;
            xVratiloTot += delta;

            float spravaDWidth = dWidth;
            float spravaEWidth = dWidth;
            float spravaTotWidth = xKonj - xParter - 2 * dWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;

            StringFormat ekipaFormat = new StringFormat(StringFormatFlags.NoWrap);
            ekipaFormat.Alignment = StringAlignment.Near;
            ekipaFormat.LineAlignment = StringAlignment.Center;

            StringFormat koloFormat = new StringFormat(StringFormatFlags.NoWrap);
            koloFormat.Alignment = StringAlignment.Center;
            koloFormat.LineAlignment = StringAlignment.Center;

            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat ekipaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat koloHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = "Rank";
            String ekipaTitle = "Ekipa";
            String koloTitle = ""; // TODO3: Neka bude uspravno (isto u i ostalim izvestajima sa vise kola).
            String totalTitle = "Total";
            String kvalTitle = String.Empty;

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            ReportColumn column = addColumn(xEkipa, ekipaWidth, ekipaFormat, ekipaTitle, ekipaHeaderFormat);
            column = addKoloColumn(column.getItemsIndexEnd(), 2, xKolo, koloWidth, null, koloFormat,
                koloTitle, koloHeaderFormat);

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            Sprava[] sprave = Sprave.getSprave(gimnastika);

            float[] x = { xParter, xKonj, xKarike, xPreskok, xRazboj, xVratilo };
            for (int i = 0; i < sprave.Length; i++)
            {
                column = addSpravaColumn(column.getItemsIndexEnd(), 2, x[i], spravaWidth, fmtTot,
                    spravaFormat, "", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprave[i]);
            }

            column = addTotalColumn(column.getItemsIndexEnd(), 3, xTotal, totalWidth, fmtTot, totalFormat,
                totalTitle, totalHeaderFormat);
            column.Brush = totalAllBrush;

            if (kvalColumn)
            {
                column = addColumn(xKval, kvalWidth, kvalFormat, kvalTitle);
                column.DrawHeaderRect = false;
                column.DrawItemRect = false;
            }
        }

        private EkipnoFinaleKupaSpravaReportColumn addKoloColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle,
            StringFormat headerFormat)
        {
            return addSpravaColumn(itemsIndex, itemsSpan, x, width, format, itemRectFormat, headerTitle, headerFormat);
        }

        private EkipnoFinaleKupaSpravaReportColumn addSpravaColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle,
            StringFormat headerFormat)
        {
            EkipnoFinaleKupaSpravaReportColumn result = new EkipnoFinaleKupaSpravaReportColumn(
                itemsIndex, itemsSpan, x, width, headerTitle);
            result.Format = format;
            result.ItemRectFormat = itemRectFormat;
            result.HeaderFormat = headerFormat;
            Columns.Add(result);
            return result;
        }

        private EkipnoFinaleKupaSpravaReportColumn addTotalColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle,
            StringFormat headerFormat)
        {
            return addSpravaColumn(itemsIndex, itemsSpan, x, width, format, itemRectFormat, headerTitle, headerFormat);
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
                        if (col.DrawHeaderRect)
                        {
                            g.DrawRectangle(pen, columnHeaderRect.X, columnHeaderRect.Y,
                                columnHeaderRect.Width, columnHeaderRect.Height);
                        }
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

    public class EkipnoFinaleKupaSpravaReportColumn : ReportColumn
    {
        public EkipnoFinaleKupaSpravaReportColumn(int itemsIndex, int itemsSpan, float x, float width, string headerTitle)
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

            RectangleF itemRect1 = new RectangleF(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 3);
            RectangleF itemRect2 = new RectangleF(itemRect.X, itemRect.Y + itemRect.Height / 3, itemRect.Width,
                itemRect.Height / 3);

            string item1 = this.getFormattedString(itemsRow, itemsIndex);
            string item2 = this.getFormattedString(itemsRow, itemsIndex + 1);
            g.DrawString(item1, itemFont, blackBrush, itemRect1, this.ItemRectFormat);
            g.DrawString(item2, itemFont, blackBrush, itemRect2, this.ItemRectFormat);

            if (itemsSpan == 3)
            {
                RectangleF itemRect3 = new RectangleF(itemRect.X, itemRect.Y + 2 * itemRect.Height / 3, itemRect.Width,
                    itemRect.Height / 3);
                string item3 = this.getFormattedString(itemsRow, itemsIndex + 2);
                g.DrawString(item3, itemFont, blackBrush, itemRect3, this.ItemRectFormat);
            }
        }
    }
}
