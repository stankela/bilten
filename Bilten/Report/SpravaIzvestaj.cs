using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;

namespace Bilten.Report
{
    class SpravaIzvestaj : Izvestaj
    {
        private List<SpravaLista> liste = new List<SpravaLista>();
        private bool svakaSpravaNaPosebnojStrani;

        public SpravaIzvestaj(Sprava sprava, IList<RezultatSprava> rezultati, 
            bool kvalColumn, string documentName, bool prikaziPenal)
		{
            DocumentName = documentName;
            Font itemFont = new Font("Arial", 8);
            Font itemsHeaderFont = new Font("Arial", 8, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            liste.Add(new SpravaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                kvalColumn, sprava, prikaziPenal));
		}

        public SpravaIzvestaj(bool extended, IList<RezultatPreskok> rezultati,
            bool kvalColumn, string documentName, bool prikaziPenal)
        {
            DocumentName = documentName;
            Font itemFont = new Font("Arial", 8);
            Font itemsHeaderFont = new Font("Arial", 8, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            liste.Add(new SpravaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                kvalColumn, extended, prikaziPenal));
        }

        public SpravaIzvestaj(List<List<RezultatSprava>> rezultatiSprave,
            List<RezultatPreskok> rezultatiPreskok, bool obaPreskoka, Gimnastika gim,
            bool kvalColumn, string documentName, int brojSpravaPoStrani, bool prikaziPenal)
        {
            DocumentName = documentName;
            Font itemFont = new Font("Arial", 8);
            Font itemsHeaderFont = new Font("Arial", 8, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;

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
                if (sprava != Sprava.Preskok)
                {
                    int spravaIndex = i;
                    if (i > Sprave.indexOf(Sprava.Preskok, gim))
                        spravaIndex--;

                    SpravaLista lista = new SpravaLista(this, page, 0f, itemFont, itemsHeaderFont,
                        rezultatiSprave[spravaIndex], kvalColumn, sprava, prikaziPenal);
                    lista.RelY = relY;
                    liste.Add(lista);
                }
                else
                {
                    SpravaLista lista = new SpravaLista(this, page, 0f, itemFont, itemsHeaderFont,
                        rezultatiPreskok, kvalColumn, obaPreskoka, prikaziPenal);
                    lista.RelY = relY;
                    liste.Add(lista);
                }
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            lastPageNum = 0;
            foreach (SpravaLista lista in liste)
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
            foreach (SpravaLista lista in liste)
            {
                lista.drawContent(g, contentBounds, pageNum);
            }
        }
    }

    public class SpravaLista : ReportLista
    {
        private float rankWidthCm = 1f;
        private float imeWidthCm = 3.5f;
        private float klubWidthCm = 3.5f;
        private float skokWidthCm = 0.5f;
        private float ocenaWidthCm = 1.3f;
        private float kvalWidthCm = 0.5f;
        private Brush totalBrush;
        private Brush totalAllBrush;

        private bool kvalColumn;
        private Sprava sprava;
        private bool extended;
        private bool prikaziPenal;

        public SpravaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatSprava> rezultati,
            bool kvalColumn, Sprava sprava, bool prikaziPenal)
            : base(izvestaj, pageNum, y, itemFont,
            itemsHeaderFont)
        {
            this.kvalColumn = kvalColumn;
            this.sprava = sprava;
            this.prikaziPenal = prikaziPenal;

            totalBrush = Brushes.LightGray;
            totalAllBrush = Brushes.Silver;

            fetchItems(rezultati);
        }

        public SpravaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatPreskok> rezultati,
            bool kvalColumn, bool extended, bool prikaziPenal)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont)
        {
            this.kvalColumn = kvalColumn;
            this.sprava = Sprava.Preskok;
            this.extended = extended;
            this.prikaziPenal = prikaziPenal;

            totalBrush = Brushes.LightGray;
            totalAllBrush = Brushes.Silver;

            fetchItems(rezultati);
        }

        private void fetchItems(IList<RezultatSprava> rezultati)
        {
            items = getSpravaReportItems(rezultati);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private void fetchItems(IList<RezultatPreskok> rezultati)
        {
            items = getPreskokReportItems(rezultati);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getSpravaReportItems(IList<RezultatSprava> rezultati)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatSprava rez in rezultati)
            {
                if (prikaziPenal)
                    result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.D, rez.E, rez.Penalty, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                else
                    result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.D, rez.E, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
            }
            return result;
        }

        private List<object[]> getPreskokReportItems(IList<RezultatPreskok> rezultati)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatPreskok rez in rezultati)
            {
                if (extended)
                {
                    if (prikaziPenal)
                        result.Add(new object[] { rez.Rank2, rez.PrezimeIme, rez.KlubDrzava, "1", "2",
                            rez.D, rez.D_2, rez.E, rez.E_2, rez.Penalty, rez.Penalty_2, rez.Total, rez.Total_2,
                            rez.TotalObeOcene, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                    else
                        result.Add(new object[] { rez.Rank2, rez.PrezimeIme, rez.KlubDrzava, "1", "2",
                            rez.D, rez.D_2, rez.E, rez.E_2, rez.Total, rez.Total_2, 
                            rez.TotalObeOcene, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                }
                else
                {
                    if (prikaziPenal)
                        result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.D, rez.E, rez.Penalty, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                    else
                        result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.D, rez.E, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                }
            }
            return result;
        }

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);

            if (extended)
                itemHeight = itemFont.GetHeight(g) * 2.9f;
            else
                itemHeight = itemFont.GetHeight(g) * 1.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds)
        {
            float ocenaWidth = Izvestaj.convCmToInch(ocenaWidthCm);
            int brojOcena;
            if (prikaziPenal)
                brojOcena = 4;
            else
                brojOcena = 3;

            float xRank = contentBounds.X;
            float xIme = xRank + Izvestaj.convCmToInch(rankWidthCm);
            float xKlub = xIme + Izvestaj.convCmToInch(imeWidthCm);
            float xSkok = 0.0f;
            float xSprava;
            if (extended)
            {
                xSkok = xKlub + Izvestaj.convCmToInch(klubWidthCm);
                xSprava = xSkok + Izvestaj.convCmToInch(skokWidthCm);
            }
            else
                xSprava = xKlub + Izvestaj.convCmToInch(klubWidthCm);

            float xTotal = xSprava + (ocenaWidth * brojOcena);
            float xKval = xTotal + ocenaWidth;
            if (!extended)
                xKval = xSprava + (ocenaWidth * brojOcena);

            float kvalWidth = Izvestaj.convCmToInch(kvalWidthCm);
            float xRightEnd = xKval + kvalWidth;
            if (xRightEnd < contentBounds.Right)
            {
                float delta = (contentBounds.Right - xRightEnd) / 2;
                xRank += delta;
                xIme += delta;
                xKlub += delta;
                if (extended)
                    xSkok += delta;
                xSprava += delta;
                xTotal += delta;
                xKval += delta;
                xRightEnd += delta;
            }

            float xE = xSprava + ocenaWidth;
            float xPen = xE + ocenaWidth;
            float xTot;
            if (prikaziPenal)
                xTot = xPen + ocenaWidth;
            else
                xTot = xPen;

            float rankWidth = xIme - xRank;
            float imeWidth = xKlub - xIme;
            float klubWidth;
            if (extended)
                klubWidth = xSkok - xKlub;
            else
                klubWidth = xSprava - xKlub;
            float skokWidth = xSprava - xSkok;

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

            StringFormat skokFormat = new StringFormat(StringFormatFlags.NoWrap);
            skokFormat.Alignment = StringAlignment.Center;
            skokFormat.LineAlignment = StringAlignment.Center;

            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat skokHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = "Rank";
            String imeTitle = "Ime";
            String klubTitle = "Klub";
            String skokTitle = ""; // TODO3: Neka bude uspravno.
            String totalTitle = "Total";
            String kvalTitle = String.Empty;

            columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            if (extended)
            {
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSkok, skokWidth, null, skokFormat,
                  skokTitle, skokHeaderFormat);
            }

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            ReportColumn column1;
            if (extended)
                column1 = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSprava, spravaDWidth, fmtD,
                  spravaFormat, "D", spravaHeaderFormat);
            else
                column1 = addColumn(xSprava, spravaDWidth, fmtD, spravaFormat, "D", spravaHeaderFormat);
            column1.Image = SlikeSprava.getImage(sprava);
            column1.Split = true;
            column1.Span = true;

            if (extended)
                column = addDvaPreskokaColumn(column1.getItemsIndexEnd(), 2, xE, spravaEWidth, fmtE, spravaFormat,
                  "E", spravaHeaderFormat);
            else
                column = addColumn(xE, spravaEWidth, fmtE, spravaFormat, "E", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (prikaziPenal)
            {
                if (extended)
                    column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xPen, ocenaWidth, fmtPen, spravaFormat,
                      "Pen.", spravaHeaderFormat);
                else
                    column = addColumn(xPen, ocenaWidth, fmtPen, spravaFormat, "Pen.", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            string title = "Total";
            if (extended)
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xTot, spravaTotWidth, fmtTot, spravaFormat,
                    title, spravaHeaderFormat);
            else
                column = addColumn(xTot, spravaTotWidth, fmtTot, spravaFormat, title, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;
            column.Brush = totalBrush;

            if (column1.Span)
                column1.SpanEndColumn = column;

            if (extended)
            {
                column = addColumn(column.getItemsIndexEnd(), xTotal, ocenaWidth, fmtTot, totalFormat,
                    totalTitle, totalHeaderFormat);
                column.Brush = totalAllBrush;
            }

            if (kvalColumn)
            {
                column = addColumn(column.getItemsIndexEnd(), xKval, kvalWidth, kvalFormat, kvalTitle);
                column.DrawHeaderRect = false;
                column.DrawItemRect = false;
            }
        }

        private DvaPreskokaReportColumn addDvaPreskokaColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle,
            StringFormat headerFormat)
        {
            DvaPreskokaReportColumn result = new DvaPreskokaReportColumn(
                itemsIndex, itemsSpan, x, width, headerTitle);
            result.Format = format;
            result.ItemRectFormat = itemRectFormat;
            result.HeaderFormat = headerFormat;
            columns.Add(result);
            return result;
        }

        protected override void drawGroupHeader(Graphics g, int groupId, RectangleF groupHeaderRect)
        {
            foreach (ReportColumn col in columns)
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

    // TODO3: Ovu klasu bi trebalo merdzovati sa klasom  UkupnoFinaleKupaSpravaReportColumn
    public class DvaPreskokaReportColumn : ReportColumn
    {
        public DvaPreskokaReportColumn(int itemsIndex, int itemsSpan, float x, float width, string headerTitle)
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

            string item1 = this.getFormattedString(itemsRow, itemsIndex);
            string item2 = this.getFormattedString(itemsRow, itemsIndex + 1);
            g.DrawString(item1, itemFont, blackBrush, itemRect1, this.ItemRectFormat);
            g.DrawString(item2, itemFont, blackBrush, itemRect2, this.ItemRectFormat);
        }
    }
}
