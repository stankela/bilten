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
	public class UkupnoIzvestaj : Izvestaj
	{
		private UkupnoLista lista;

		public UkupnoIzvestaj(IList<RezultatUkupnoExtended> rezultati, Gimnastika gim,
            bool extended, bool kvalColumn, bool penalty, DataGridView formGrid, string documentName,
            bool stampanjeKvalifikanata, bool penalizacijaZaSprave)
		{
            DocumentName = documentName;

            Font itemFont = new Font("Arial", 8);
            Font itemsHeaderFont = new Font("Arial", 8, FontStyle.Bold);

            Landscape = extended;
            if (extended)
                Margins = new Margins(40, 40, 50, 50);
            else
                Margins = new Margins(75, 75, 75, 75);

            lista = new UkupnoLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                gim, extended, kvalColumn, penalty, formGrid, stampanjeKvalifikanata, penalizacijaZaSprave);
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

	public class UkupnoLista : ReportLista
	{
        private Brush totalBrush;
        private Brush totalAllBrush;

        private bool extended;
        private bool kvalColumn;
        private bool penalty;
        private Gimnastika gimnastika;
        bool penalizacijaZaSprave;

		public UkupnoLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatUkupnoExtended> rezultati,
            Gimnastika gim, bool extended, bool kvalColumn, bool penalty, DataGridView formGrid, bool stampanjeKvalifikanata,
            bool penalizacijaZaSprave)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
		{
            this.extended = extended;
            this.kvalColumn = kvalColumn;
            this.penalty = penalty;
            this.gimnastika = gim;
            this.penalizacijaZaSprave = penalizacijaZaSprave;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, gim, stampanjeKvalifikanata);
        }

        private void fetchItems(IList<RezultatUkupnoExtended> rezultati,
            Gimnastika gim, bool stampanjeKvalifikanata)
		{
            items = getUkupnoReportItems(rezultati, gim, extended, stampanjeKvalifikanata);
		
			groups = new List<ReportGrupa>();
			groups.Add(new ReportGrupa(0, items.Count));
		}

        private List<object[]> getUkupnoReportItems(IList<RezultatUkupnoExtended> rezultati,
            Gimnastika gim, bool extended, bool stampanjeKvalifikanata)
        {
            List<object[]> result = new List<object[]>();
            int q = 0;
            int rezerva = 0;
            foreach (RezultatUkupnoExtended r in rezultati)
            {
                string redBroj = String.Empty;
                if (!stampanjeKvalifikanata)
                {
                    redBroj = r.Rank.ToString();
                }
                else
                {
                    if (r.KvalStatus == KvalifikacioniStatus.Q)
                    {
                        ++q;
                        redBroj = q.ToString();
                    }
                    else if (r.KvalStatus == KvalifikacioniStatus.R)
                    {
                        ++rezerva;
                        redBroj = "R" + rezerva.ToString();
                    }
                }

                if (gim == Gimnastika.MSG)
                {
                    List<object> items = new List<object> { redBroj, r.PrezimeIme, r.KlubDrzava,
                            r.Parter, r.Konj, r.Karike, r.Preskok, r.Razboj, r.Vratilo,
                            r.Total, KvalifikacioniStatusi.toString(r.KvalStatus), r.Penalty };
                    if (extended)
                    {
                        items.Insert(3, r.ParterD);
                        items.Insert(4, r.ParterE);
                        items.Insert(6, r.KonjD);
                        items.Insert(7, r.KonjE);
                        items.Insert(9, r.KarikeD);
                        items.Insert(10, r.KarikeE);
                        items.Insert(12, r.PreskokD);
                        items.Insert(13, r.PreskokE);
                        items.Insert(15, r.RazbojD);
                        items.Insert(16, r.RazbojE);
                        items.Insert(18, r.VratiloD);
                        items.Insert(19, r.VratiloE);
                    }
                    if (penalizacijaZaSprave)
                    {
                        // extended je uvek true kada prikazujemo penalizaciju za sprave
                        // index se povecava za 4 zato sto imamo D, E, Pen, i Total za svaku spravu
                        items.Insert(6, r.ParterPen);
                        items.Insert(10, r.KonjPen);
                        items.Insert(14, r.KarikePen);
                        items.Insert(18, r.PreskokPen);
                        items.Insert(22, r.RazbojPen);
                        items.Insert(26, r.VratiloPen);
                    }
                    result.Add(items.ToArray());
                }
                else
                {
                    List<object> items = new List<object> { redBroj, r.PrezimeIme, r.KlubDrzava,
                            r.Preskok, r.DvovisinskiRazboj, r.Greda, r.Parter, 
                            r.Total, KvalifikacioniStatusi.toString(r.KvalStatus), r.Penalty };
                    if (extended)
                    {
                        items.Insert(3, r.PreskokD);
                        items.Insert(4, r.PreskokE);
                        items.Insert(6, r.DvovisinskiRazbojD);
                        items.Insert(7, r.DvovisinskiRazbojE);
                        items.Insert(9, r.GredaD);
                        items.Insert(10, r.GredaE);
                        items.Insert(12, r.ParterD);
                        items.Insert(13, r.ParterE);
                    }
                    if (penalizacijaZaSprave)
                    {
                        items.Insert(6, r.PreskokPen);
                        items.Insert(10, r.DvovisinskiRazbojPen);
                        items.Insert(14, r.GredaPen);
                        items.Insert(18, r.ParterPen);
                    }
                    result.Add(items.ToArray());
                }
            }
            return result;
        }

		public void setupContent(Graphics g, RectangleF contentBounds)
		{
			createColumns(g, contentBounds);

            if (extended && penalizacijaZaSprave)
                itemHeight = itemFont.GetHeight(g) * 2.9f;
            else
                itemHeight = itemFont.GetHeight(g) * 1.4f;
            if (extended && penalizacijaZaSprave)
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 4.8f;
            else if (extended)
			    itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            else
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
            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;

            float spravaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;

            if (extended && penalizacijaZaSprave)
            {
                createColumnsExtendedPenalizacija(g, contentBounds, gridWidth, printWidth, rankWidth, imeWidth, klubWidth,
                    spravaWidth);
                return;
            }

            float penaltyWidth = spravaWidth * (2.0f / 3);
            float totalWidth = spravaWidth;
            if (extended)
                spravaWidth = spravaWidth * 2.0f;
            float kvalWidth = rankWidth / 2;

			float xRank = contentBounds.X;
            float xIme = xRank + rankWidth;
            float xKlub = xIme + imeWidth;
            float xParter = xKlub + klubWidth;
            float xKonj = xParter + spravaWidth;
            float xKarike = xKonj + spravaWidth;
            float xPreskok = xKarike + spravaWidth;
            float xRazboj = xPreskok + spravaWidth;
            float xVratilo = xRazboj + spravaWidth;
            float xTotal = xVratilo + spravaWidth;
            if (gimnastika == Gimnastika.ZSG)
                xTotal = xRazboj;

            float xPenalty = xTotal;
            if (penalty)
                xTotal += penaltyWidth;

            float xKval = xTotal + totalWidth;
            
            float xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;
            
            float spravaDWidth = ((xKonj - xParter) / 3) * 0.8f;
            float spravaEWidth = (xKonj - xParter - spravaDWidth) / 2;
            float spravaTotWidth = spravaEWidth;

            float xParterE = xParter + spravaDWidth;
            float xParterTot = xParterE + spravaEWidth;
            float xKonjE = xKonj + spravaDWidth;
            float xKonjTot = xKonjE + spravaEWidth;
            float xKarikeE = xKarike + spravaDWidth;
            float xKarikeTot = xKarikeE + spravaEWidth;
            float xPreskokE = xPreskok + spravaDWidth;
            float xPreskokTot = xPreskokE + spravaEWidth;
            float xRazbojE = xRazboj + spravaDWidth;
            float xRazbojTot = xRazbojE + spravaEWidth;
            float xVratiloE = xVratilo + spravaDWidth;
            float xVratiloTot = xVratiloE + spravaEWidth;

            float delta = (contentBounds.Right - xRightEnd) / 2;  // moza da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;
            xRank += delta;
            xIme += delta;
            xKlub += delta;
            xParter += delta;
            xKonj += delta;
            xKarike += delta;
            xPreskok += delta;
            xRazboj += delta;
            xVratilo += delta;
            xPenalty += delta;
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
            
            StringFormat rankFormat = Izvestaj.centerCenterFormat;

            // TODO3: Ispravi sledece greske gde je umesto Alignment stavljeno ponovljeno LineAlignment. Uradi to i u 
            // ostalim izvestajima.

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.LineAlignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.LineAlignment = StringAlignment.Center;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = Opcije.Instance.RankString;
			String imeTitle = Opcije.Instance.ImeString;
			String klubTitle = Opcije.Instance.KlubDrzavaString;
            String totalTitle = Opcije.Instance.TotalString;
            String kvalTitle = String.Empty;

            Columns.Clear();

			addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
			addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
			addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            Sprava[] sprave = Sprave.getSprave(gimnastika);
            ReportColumn column;

            if (extended)
            {
                float[] x = { xParter, xParterE, xParterTot, xKonj, xKonjE, xKonjTot,
                    xKarike, xKarikeE, xKarikeTot, xPreskok, xPreskokE, xPreskokTot,
                    xRazboj, xRazbojE, xRazbojTot, xVratilo, xVratiloE, xVratiloTot };
                for (int i = 0; i < sprave.Length; i++)
                {
                    ReportColumn column1 = addColumn(x[3 * i], spravaDWidth, fmtD, spravaFormat, "D", spravaHeaderFormat);
                    column1.Image = SlikeSprava.getImage(sprave[i]);
                    column1.Split = true;
                    column1.Span = true;

                    column = addColumn(x[3 * i + 1], spravaEWidth, fmtE, spravaFormat, "E", spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprave[i]);
                    column.Split = true;

                    column = addColumn(x[3 * i + 2], spravaTotWidth, fmtTot, spravaFormat, "Total", spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprave[i]);
                    column.Split = true;
                    column.Brush = totalBrush;

                    if (column1.Span)
                        column1.SpanEndColumn = column;
                }
            }
            else
            {
                float[] x = { xParter, xKonj, xKarike, xPreskok, xRazboj, xVratilo };
                for (int i = 0; i < sprave.Length; i++)
                {
                    column = addColumn(x[i], spravaWidth, fmtTot, spravaFormat, "", spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprave[i]);
                }
            }

            column = addColumn(xTotal, totalWidth, fmtTot, totalFormat, totalTitle, totalHeaderFormat);
            column.Brush = totalAllBrush;

            if (kvalColumn)
            {
                column = addColumn(xKval, kvalWidth, kvalFormat, kvalTitle);
                column.DrawHeaderRect = false;
                column.DrawItemRect = false;
            }
            if (penalty)
            {
                column = addColumn(xPenalty, penaltyWidth, fmtPen, totalFormat, "Pen.", totalHeaderFormat);
                if (!kvalColumn)
                {
                    // Posto se kvalifikacioni status uvek dodaje u report items, cak i ako ne postoji kolona za
                    // kval. status, moram da azuriram report item index za penalty ako nije dodata kolona za
                    // kvalifikacioni status.
                    column.itemsIndex += 1;
                }
            }
        }

        private void createColumnsExtendedPenalizacija(Graphics g, RectangleF contentBounds,
            float gridWidth, float printWidth, float rankWidth, float imeWidth, float klubWidth, float spravaWidth)
        {
            float penaltyWidth = spravaWidth * (2.0f / 3);
            float totalWidth = spravaWidth;
            float kvalWidth = rankWidth / 2;
            spravaWidth = spravaWidth * 2;

            float xRank = contentBounds.X;
            float xIme = xRank + rankWidth;
            float xKlub = xIme + imeWidth;
            float xParter = xKlub + klubWidth;
            float xKonj = xParter + spravaWidth;
            float xKarike = xKonj + spravaWidth;
            float xPreskok = xKarike + spravaWidth;
            float xRazboj = xPreskok + spravaWidth;
            float xVratilo = xRazboj + spravaWidth;
            float xTotal = xVratilo + spravaWidth;
            if (gimnastika == Gimnastika.ZSG)
                xTotal = xRazboj;

            float xPenalty = xTotal;
            if (penalty)
                xTotal += penaltyWidth;

            float xKval = xTotal + totalWidth;

            float xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;

            float spravaEWidth = (xKonj - xParter) / 2;
            float spravaTotWidth = spravaEWidth;

            float xParterTot = xParter + spravaEWidth;
            float xKonjTot = xKonj + spravaEWidth;
            float xKarikeTot = xKarike + spravaEWidth;
            float xPreskokTot = xPreskok + spravaEWidth;
            float xRazbojTot = xRazboj + spravaEWidth;
            float xVratiloTot = xVratilo + spravaEWidth;

            float delta = (contentBounds.Right - xRightEnd) / 2;  // moza da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;
            xRank += delta;
            xIme += delta;
            xKlub += delta;
            xParter += delta;
            xKonj += delta;
            xKarike += delta;
            xPreskok += delta;
            xRazboj += delta;
            xVratilo += delta;
            xPenalty += delta;
            xTotal += delta;
            xKval += delta;
            xRightEnd += delta;

            xParterTot += delta;
            xKonjTot += delta;
            xKarikeTot += delta;
            xPreskokTot += delta;
            xRazbojTot += delta;
            xVratiloTot += delta;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.LineAlignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.LineAlignment = StringAlignment.Center;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = Opcije.Instance.RankString;
            String imeTitle = Opcije.Instance.ImeString;
            String klubTitle = Opcije.Instance.KlubDrzavaString;
            String totalTitle = Opcije.Instance.TotalString;
            String kvalTitle = String.Empty;

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            Sprava[] sprave = Sprave.getSprave(gimnastika);

            float[] x = { xParter, xParterTot, xKonj, xKonjTot, xKarike, xKarikeTot, xPreskok, xPreskokTot,
                          xRazboj, xRazbojTot, xVratilo, xVratiloTot };
            ReportColumn prevColumn = column;
            for (int i = 0; i < sprave.Length; i++)
            {
                ReportColumn column1 = addDvaPreskokaColumn(prevColumn.getItemsIndexEnd(), 2, x[2 * i], spravaEWidth, fmtE,
                    spravaFormat, "D\nE", spravaHeaderFormat);

                column1.Image = SlikeSprava.getImage(sprave[i]);
                column1.Split = true;
                column1.Span = true;

                column = addDvaPreskokaColumn(column1.getItemsIndexEnd(), 2, x[2 * i + 1], spravaTotWidth, fmtTot,
                    spravaFormat, "Total\nPen", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprave[i]);
                column.Split = true;
                column.Brush = totalBrush;

                column1.SpanEndColumn = column;
                prevColumn = column;
            }

            column = addColumn(prevColumn.getItemsIndexEnd(), xTotal, totalWidth, fmtTot, totalFormat, totalTitle, totalHeaderFormat);
            column.Brush = totalAllBrush;

            if (kvalColumn)
            {
                column = addColumn(column.getItemsIndexEnd(), xKval, kvalWidth, kvalFormat, kvalTitle);
                column.DrawHeaderRect = false;
                column.DrawItemRect = false;
            }
            if (penalty)
            {
                column = addColumn(column.getItemsIndexEnd(), xPenalty, penaltyWidth, fmtPen, totalFormat, "Pen.",
                    totalHeaderFormat);
                if (!kvalColumn)
                {
                    // Posto se kvalifikacioni status uvek dodaje u report items, cak i ako ne postoji kolona za
                    // kval. status, moram da azuriram report item index za penalty ako nije dodata kolona za
                    // kvalifikacioni status.
                    column.itemsIndex += 1;
                }
            }
        }

        private DvaPreskokaReportColumn addDvaPreskokaColumn(int itemsIndex, int itemsSpan, float x, float width,
            string format, StringFormat itemRectFormat, string headerTitle, StringFormat headerFormat)
        {
            DvaPreskokaReportColumn result = new DvaPreskokaReportColumn(
                itemsIndex, itemsSpan, x, width, headerTitle);
            result.Format = format;
            result.ItemRectFormat = itemRectFormat;
            result.HeaderFormat = headerFormat;
            Columns.Add(result);
            result.Lista = this;
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
                        float imageHeight;
                        if (extended && penalizacijaZaSprave)
                            imageHeight = (2f / 4) * columnHeaderRect.Height;
                        else
                            imageHeight = (2f / 3) * columnHeaderRect.Height;
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
}
