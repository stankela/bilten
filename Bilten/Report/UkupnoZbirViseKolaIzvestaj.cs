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
	public class UkupnoZbirViseKolaIzvestaj : Izvestaj
	{
        public UkupnoZbirViseKolaIzvestaj(IList<RezultatUkupnoZbirViseKola> rezultati, DataGridView formGrid,
            string documentName, int brojKola, Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);

            Margins = new Margins(75, 75, 75, 75);

            reportListe.Add(new UkupnoZbirViseKolaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                takmicenje.Gimnastika, formGrid, brojKola, resizeByGrid));
		}

        protected override void doSetupContent(Graphics g)
		{
            poredjajListeUJednuKolonu(g, contentBounds, reportListe, false);
        }
    }

	public class UkupnoZbirViseKolaLista : ReportLista
	{
        private Brush totalBrush;
        private Brush totalAllBrush;

        private Gimnastika gimnastika;
        private int brojKola;

        public UkupnoZbirViseKolaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatUkupnoZbirViseKola> rezultati,
            Gimnastika gim, DataGridView formGrid, int brojKola, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
		{
            this.gimnastika = gim;
            this.brojKola = brojKola;
            this.resizeByGrid = resizeByGrid;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, gim);
        }

        public int getImeColumnIndex()
        {
            return 1;
        }

        public int getKlubColumnIndex()
        {
            return 2;
        }

        private void fetchItems(IList<RezultatUkupnoZbirViseKola> rezultati, Gimnastika gim)
		{
            items = getUkupnoReportItems(rezultati, gim);
		
			groups = new List<ReportGrupa>();
			groups.Add(new ReportGrupa(0, items.Count));
		}

        private List<object[]> getUkupnoReportItems(IList<RezultatUkupnoZbirViseKola> rezultati, Gimnastika gim)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatUkupnoZbirViseKola rez in rezultati)
            {
                if (gim == Gimnastika.MSG)
                {
                    result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            "I", "II", "III", "IV",
                            rez.ParterPrvoKolo, rez.ParterDrugoKolo, rez.ParterTreceKolo, rez.ParterCetvrtoKolo,
                            rez.KonjPrvoKolo, rez.KonjDrugoKolo, rez.KonjTreceKolo, rez.KonjCetvrtoKolo,
                            rez.KarikePrvoKolo, rez.KarikeDrugoKolo, rez.KarikeTreceKolo, rez.KarikeCetvrtoKolo,
                            rez.PreskokPrvoKolo, rez.PreskokDrugoKolo, rez.PreskokTreceKolo, rez.PreskokCetvrtoKolo,
                            rez.RazbojPrvoKolo, rez.RazbojDrugoKolo, rez.RazbojTreceKolo, rez.RazbojCetvrtoKolo,
                            rez.VratiloPrvoKolo, rez.VratiloDrugoKolo, rez.VratiloTreceKolo, rez.VratiloCetvrtoKolo,
                            rez.TotalPrvoKolo, rez.TotalDrugoKolo, rez.TotalTreceKolo, rez.TotalCetvrtoKolo,
                            rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                }
                else
                {
                    result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            "I", "II", "III", "IV",
                            rez.PreskokPrvoKolo, rez.PreskokDrugoKolo, rez.PreskokTreceKolo, rez.PreskokCetvrtoKolo,
                            rez.DvovisinskiRazbojPrvoKolo, rez.DvovisinskiRazbojDrugoKolo, rez.DvovisinskiRazbojTreceKolo,
                            rez.DvovisinskiRazbojCetvrtoKolo, rez.GredaPrvoKolo, rez.GredaDrugoKolo, rez.GredaTreceKolo,
                            rez.GredaCetvrtoKolo, rez.ParterPrvoKolo, rez.ParterDrugoKolo, rez.ParterTreceKolo,
                            rez.ParterCetvrtoKolo, rez.TotalPrvoKolo, rez.TotalDrugoKolo, rez.TotalTreceKolo,
                            rez.TotalCetvrtoKolo, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                }
            }
            return result;
        }

        public override List<int> getAdjustableColumnIndexes()
        {
            List<int> result = new List<int>();
            result.Add(getImeColumnIndex());
            result.Add(getKlubColumnIndex());
            return result;
        }

        protected override void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
            List<bool> rszByGrid)
        {
            // First, create columns

            float imeWidth;
            float klubWidth;
            if (columnWidths.Count == 0)
            {
                // Prvi pass
                if (resizeByGrid)
                {
                    float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
                    float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;
                    imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                    klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
                }
                else
                {
                    // Resize by content
                    // Proizvoljna vrednost. Koristi se u prvom pozivu setupContent. U drugom pozivu setupContent, imeWidth,
                    // klubWidth i kategorijaWidth ce dobiti pravu vrednost, koja je dovoljna da i najduzi tekst stane bez
                    // odsecanja.
                    imeWidth = 1f;
                    klubWidth = 1f;
                }
            }
            else if (columnWidths.Count == 2)
            {
                // Drugi pass, sirine kolona su podesene
                imeWidth = columnWidths[0];
                klubWidth = columnWidths[1];
            }
            else
            {
                throw new Exception("Trenutno, samo 2 kolone mogu da se podesavaju");
            }
            createColumns(g, contentBounds, imeWidth, klubWidth);

            // Then, layout contents vertically

            itemHeight = itemFont.GetHeight(g) * (6.6f / 5 * (brojKola + 1));
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 2.4f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
		{
            string rankTitle = Opcije.Instance.RankString;
            float rankWidth = getColumnWidth(g, RANK_MAX_TEXT, rankTitle);

            string imeTitle = Opcije.Instance.ImeString;
            imeWidth = getColumnWidth(g, imeWidth, imeTitle);

            string klubTitle = Opcije.Instance.KlubDrzavaString;
            klubWidth = getColumnWidth(g, klubWidth, klubTitle);

            string koloTitle = ""; // TODO3: Neka bude uspravno (isto i u ostalim izvestajima sa vise kola).
            float koloWidth = getColumnWidth(g, KOLO_MAX_TEXT, koloTitle);

            // Kolone za sprave nemaju tekst nego sliku
            float spravaWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, "");

            string totalTitle = Opcije.Instance.TotalString;
            float totalWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO_FINALE_KUPA, totalTitle);

            string kvalTitle = "";
            float kvalWidth = getColumnWidth(g, QUAL_MAX_TEXT, kvalTitle);

			float xRank = contentBounds.X;
            float xIme = xRank + rankWidth;
            float xKlub = xIme + imeWidth;
            float xKolo = xKlub + klubWidth;
            float xParter = xKolo + koloWidth;
            float xKonj = xParter + spravaWidth;
            float xKarike = xKonj + spravaWidth;
            float xPreskok = xKarike + spravaWidth;
            float xRazboj = xPreskok + spravaWidth;
            float xVratilo = xRazboj + spravaWidth;
            float xTotal = xVratilo + spravaWidth;
            if (gimnastika == Gimnastika.ZSG)
                xTotal = xRazboj;
            xRightEnd = xTotal + totalWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat koloFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat koloHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            column = addKoloColumn(column.getItemsIndexEnd(), 4, xKolo, koloWidth, null, koloFormat,
                "", koloHeaderFormat);

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            Sprava[] sprave = Sprave.getSprave(gimnastika);
            float[] x = { xParter, xKonj, xKarike, xPreskok, xRazboj, xVratilo };
            for (int i = 0; i < sprave.Length; i++)
            {
                column = addSpravaColumn(column.getItemsIndexEnd(), 4, x[i], spravaWidth, fmtTot,
                    spravaFormat, "", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprave[i]);
            }
            column = addTotalColumn(column.getItemsIndexEnd(), 5, xTotal, totalWidth, fmtTot, totalFormat, totalTitle,
                totalHeaderFormat);
            column.Brush = totalAllBrush;
        }

        private UkupnoZbirViseKolaSpravaReportColumn addKoloColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle,
            StringFormat headerFormat)
        {
            return addSpravaColumn(itemsIndex, itemsSpan, x, width, format, itemRectFormat, headerTitle, headerFormat);
        }

        private UkupnoZbirViseKolaSpravaReportColumn addSpravaColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle, StringFormat headerFormat)
        {
            UkupnoZbirViseKolaSpravaReportColumn result = new UkupnoZbirViseKolaSpravaReportColumn(
                itemsIndex, itemsSpan, x, width, headerTitle, brojKola);
            result.Format = format;
            result.ItemRectFormat = itemRectFormat;
            result.HeaderFormat = headerFormat;
            Columns.Add(result);
            return result;
        }

        private UkupnoZbirViseKolaSpravaReportColumn addTotalColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle, StringFormat headerFormat)
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

    public class UkupnoZbirViseKolaSpravaReportColumn : ReportColumn
    {
        private int brojKola;

        public UkupnoZbirViseKolaSpravaReportColumn(int itemsIndex, int itemsSpan, float x, float width, string headerTitle,
            int brojKola)
            : base(itemsIndex, x, width, headerTitle)
		{
            this.itemsSpan = itemsSpan;
            this.brojKola = brojKola;
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

            float visina = itemRect.Height / (brojKola + 1);

            RectangleF itemRect1 = new RectangleF(itemRect.X, itemRect.Y, itemRect.Width, visina);
            string item1 = this.getFormattedString(itemsRow, itemsIndex);
            g.DrawString(item1, itemFont, blackBrush, itemRect1, this.ItemRectFormat);

            RectangleF itemRect2 = new RectangleF(itemRect.X, itemRect.Y + visina, itemRect.Width, visina);
            string item2 = this.getFormattedString(itemsRow, itemsIndex + 1);
            g.DrawString(item2, itemFont, blackBrush, itemRect2, this.ItemRectFormat);

            if (brojKola > 2)
            {
                RectangleF itemRect3 = new RectangleF(itemRect.X, itemRect.Y + 2 * visina, itemRect.Width, visina);
                string item3 = this.getFormattedString(itemsRow, itemsIndex + 2);
                g.DrawString(item3, itemFont, blackBrush, itemRect3, this.ItemRectFormat);
            }
            if (brojKola > 3)
            {
                RectangleF itemRect4 = new RectangleF(itemRect.X, itemRect.Y + 3 * visina, itemRect.Width, visina);
                string item4 = this.getFormattedString(itemsRow, itemsIndex + 3);
                g.DrawString(item4, itemFont, blackBrush, itemRect4, this.ItemRectFormat);
            }

            // TODO3: Dodaj zumiranje (i forma i print previewa)

            if (itemsSpan == 5)
            {
                RectangleF itemRect5 = new RectangleF(itemRect.X, itemRect.Y + brojKola * visina, itemRect.Width, visina);
                string item5 = this.getFormattedString(itemsRow, itemsIndex + 4);
                g.DrawString(item5, itemFont, blackBrush, itemRect5, this.ItemRectFormat);
            }
        }
    }
}
