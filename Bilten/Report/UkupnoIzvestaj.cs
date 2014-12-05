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
            bool extended, bool kvalColumn, DataGridView formGrid, string documentName)
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
                gim, extended, kvalColumn, formGrid);
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
        private Gimnastika gimnastika;

		public UkupnoLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatUkupnoExtended> rezultati,
            Gimnastika gim, bool extended, bool kvalColumn, DataGridView formGrid)
            : base(izvestaj, pageNum, y, itemFont,
            itemsHeaderFont, formGrid)
		{
            this.extended = extended;
            this.kvalColumn = kvalColumn;
            this.gimnastika = gim;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, gim, extended);
        }

        private void fetchItems(IList<RezultatUkupnoExtended> rezultati, 
            Gimnastika gim, bool extended)
		{
            items = getUkupnoReportItems(rezultati, gim, extended);
		
			groups = new List<ReportGrupa>();
			groups.Add(new ReportGrupa(0, items.Count));
		}

        private List<object[]> getUkupnoReportItems(IList<RezultatUkupnoExtended> rezultati,
            Gimnastika gim, bool extended)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatUkupnoExtended rez in rezultati)
            {
                if (gim == Gimnastika.MSG)
                {
                    if (extended)
                    {
                        result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.ParterD, rez.ParterE, rez.Parter, 
                            rez.KonjD, rez.KonjE, rez.Konj, 
                            rez.KarikeD, rez.KarikeE, rez.Karike, 
                            rez.PreskokD, rez.PreskokE, rez.Preskok, 
                            rez.RazbojD, rez.RazbojE, rez.Razboj, 
                            rez.VratiloD, rez.VratiloE, rez.Vratilo, 
                            rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                    }
                    else
                    {
                        result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.Parter, rez.Konj, rez.Karike, rez.Preskok, rez.Razboj, 
                            rez.Vratilo, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                    }
                }
                else
                {
                    if (extended)
                    {
                        result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.PreskokD, rez.PreskokE, rez.Preskok, 
                            rez.DvovisinskiRazbojD, rez.DvovisinskiRazbojE, rez.DvovisinskiRazboj, 
                            rez.GredaD, rez.GredaE, rez.Greda, 
                            rez.ParterD, rez.ParterE, rez.Parter, 
                            rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                    }
                    else
                    {
                        result.Add(new object[] { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.Preskok, rez.DvovisinskiRazboj, rez.Greda, rez.Parter, 
                            rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) });
                    }
                }
            }
            return result;
        }

		public void setupContent(Graphics g, RectangleF contentBounds)
		{
			createColumns(g, contentBounds);

			itemHeight = itemFont.GetHeight(g) * 1.4f;
            if (extended)
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
            float imeWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;

            float spravaWidth = this.formGrid.Columns[4].Width * printWidth / gridWidth;
            float totalWidth = spravaWidth;
            if (extended)
            {
                spravaWidth = spravaWidth * 2.3f;
            }
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

            float xKval = xTotal + totalWidth;
            
            float xRightEnd = xKval;
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
            float spravaTotWidth = xKonj - xParter - 2*dWidth;

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
}
