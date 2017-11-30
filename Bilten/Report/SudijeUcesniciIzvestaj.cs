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
	public class SudijeUcesniciIzvestaj : Izvestaj
	{
        private SudijeUcesniciLista lista;

        public SudijeUcesniciIzvestaj(IList<SudijaUcesnik> sudije, DataGridView formGrid,
            string documentName)
		{
            DocumentName = documentName;
            Font itemFont = new Font("Arial", 10);
            Font itemsHeaderFont = new Font("Arial", 10, FontStyle.Bold);

            lista = new SudijeUcesniciLista(this, 1, 0f, itemFont, itemsHeaderFont, sudije, formGrid);
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

	public class SudijeUcesniciLista : ReportLista
	{
        private Brush totalBrush;
        private Brush totalAllBrush;

        public SudijeUcesniciLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<SudijaUcesnik> sudije,
            DataGridView formGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
		{
            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(sudije);
        }

        private void fetchItems(IList<SudijaUcesnik> sudije)
		{
            items = getTakmicariReportItems(sudije);
		
			groups = new List<ReportGrupa>();
			groups.Add(new ReportGrupa(0, items.Count));
		}

        private List<object[]> getTakmicariReportItems(IList<SudijaUcesnik> gimnasticari)
        {
            List<object[]> result = new List<object[]>();
            for (int i = 0; i < gimnasticari.Count; ++i)
            {
                SudijaUcesnik g = gimnasticari[i];
                string klub = (g.KlubUcesnik != null) ? g.KlubUcesnik.Naziv : String.Empty;
                string drzava = (g.DrzavaUcesnik != null) ? g.DrzavaUcesnik.Kod : string.Empty;
                string pol = (g.Pol == Pol.Muski) ? "M" : "Z";
                result.Add(new object[] { (i+1).ToString(), g.Ime, g.Prezime, pol,
                    klub, drzava });
            }
            return result;
        }

		public void setupContent(Graphics g, RectangleF contentBounds)
		{
			createColumns(g, contentBounds);

			itemHeight = itemFont.GetHeight(g) * 1.4f;
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

            float rankWidthCm = 0.7f;
            float rankWidth = Izvestaj.convCmToInch(rankWidthCm);
            
            float imeWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
            float prezimeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            // float godinaWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float polWidthCm = 1.5f;
            float polWidth = Izvestaj.convCmToInch(polWidthCm);
            float klubWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
            float drzavaWidth = this.formGrid.Columns[4].Width * printWidth / gridWidth;


			float xRank = contentBounds.X;
            float xIme = xRank + rankWidth;
            float xPrezime = xIme + imeWidth;
            float xPol = xPrezime + prezimeWidth;
            float xKlub = xPol + polWidth;
            float xDrzava = xKlub + klubWidth;
            
            float xRightEnd = xDrzava + drzavaWidth;
            
            float delta = (contentBounds.Right - xRightEnd) / 2;  // moza da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;
            xRank += delta;
            xIme += delta;
            xPrezime += delta;
            xPol += delta;
            xKlub += delta;
            xDrzava += delta;
            xRightEnd += delta;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.Alignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat prezimeFormat = new StringFormat(StringFormatFlags.NoWrap);
            prezimeFormat.Alignment = StringAlignment.Near;
            prezimeFormat.LineAlignment = StringAlignment.Center;

            StringFormat polFormat = new StringFormat(StringFormatFlags.NoWrap);
            polFormat.Alignment = StringAlignment.Near;
            polFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.Alignment = StringAlignment.Near;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat drzavaFormat = new StringFormat(StringFormatFlags.NoWrap);
            drzavaFormat.Alignment = StringAlignment.Near;
            drzavaFormat.LineAlignment = StringAlignment.Center;

            StringFormat rankHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat prezimeHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat polHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat drzavaHeaderFormat = Izvestaj.nearCenterFormat;

            String rankTitle = "RB";
			String imeTitle = "Ime";
            String prezimeTitle = "Prezime";
            String polTitle = "Pol";
            String klubTitle = "Klub";
            String drzavaTitle = "Drzava";

            Columns.Clear();

            bool drawItemRect = false;
			ReportColumn column = addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xPrezime, prezimeWidth, prezimeFormat, prezimeTitle, prezimeHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xPol, polWidth, polFormat, polTitle, polHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xDrzava, drzavaWidth, drzavaFormat, drzavaTitle, drzavaHeaderFormat);
            column.DrawItemRect = drawItemRect;
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
