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
	public class TakmicariIzvestaj : Izvestaj
	{
		private TakmicariLista lista;

        public TakmicariIzvestaj(IList<GimnasticarUcesnik> gimnasticari, Gimnastika gim, DataGridView formGrid,
            string documentName, Takmicenje takmicenje) : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemFont = new Font("Arial", 10);
            Font itemsHeaderFont = new Font("Arial", 10, FontStyle.Bold);

            lista = new TakmicariLista(this, 1, 0f, itemFont, itemsHeaderFont, gimnasticari, gim, formGrid,
                takmicenje.TakBrojevi);
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

	public class TakmicariLista : ReportLista
	{
        private Brush totalBrush;
        private Brush totalAllBrush;

        private Gimnastika gimnastika;
        private bool stampajBroj;

        public TakmicariLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<GimnasticarUcesnik> gimnasticari,
            Gimnastika gim, DataGridView formGrid, bool stampajBroj)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
		{
            this.gimnastika = gim;
            this.stampajBroj = stampajBroj;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(gimnasticari, gim);
        }

        private void fetchItems(IList<GimnasticarUcesnik> gimnasticari, Gimnastika gim)
		{
            items = getTakmicariReportItems(gimnasticari, gim);
		
			groups = new List<ReportGrupa>();
			groups.Add(new ReportGrupa(0, items.Count));
		}

        private List<object[]> getTakmicariReportItems(IList<GimnasticarUcesnik> gimnasticari, Gimnastika gim)
        {
            List<object[]> result = new List<object[]>();
            for (int i = 0; i < gimnasticari.Count; ++i)
            {
                GimnasticarUcesnik g = gimnasticari[i];
                string klub = (g.KlubUcesnik != null) ? g.KlubUcesnik.Naziv : String.Empty;
                string drzava = (g.DrzavaUcesnik != null) ? g.DrzavaUcesnik.Kod : string.Empty;
                string datumRodjenja = (g.DatumRodjenja != null) ? g.DatumRodjenja.ToString("d") : string.Empty;
                List<object> items = new List<object> { (i+1).ToString(), g.Ime, g.Prezime, datumRodjenja,
                    klub, drzava };
                if (stampajBroj)
                {
                    string broj = (g.TakmicarskiBroj.HasValue) ? g.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
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
            float brojWidth = 2 * rankWidth;

            float imeWidth;
            float prezimeWidth;
            float godinaWidth;
            float klubWidth;
            float drzavaWidth;
            if (!stampajBroj)
            {
                imeWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
                prezimeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                godinaWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
                klubWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
                drzavaWidth = this.formGrid.Columns[4].Width * printWidth / gridWidth;
            }
            else
            {
                imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                prezimeWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
                godinaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
                klubWidth = this.formGrid.Columns[4].Width * printWidth / gridWidth;
                drzavaWidth = this.formGrid.Columns[5].Width * printWidth / gridWidth;
            }

			float xRank = contentBounds.X;
            float xBroj = 0f;
            float xIme;
            if (stampajBroj)
            {
                xBroj = xRank + rankWidth;
                xIme = xBroj + brojWidth;
            }
            else
            {
                xIme = xRank + rankWidth;
            }
            float xPrezime = xIme + imeWidth;
            float xGodina = xPrezime + prezimeWidth;
            float xKlub = xGodina + godinaWidth;
            float xDrzava = xKlub + klubWidth;
            
            float xRightEnd = xDrzava + drzavaWidth;
            
            float delta = (contentBounds.Right - xRightEnd) / 2;  // moze da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;
            xRank += delta;
            xBroj += delta;
            xIme += delta;
            xPrezime += delta;
            xGodina += delta;
            xKlub += delta;
            xDrzava += delta;
            xRightEnd += delta;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.nearCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.Alignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat prezimeFormat = new StringFormat(StringFormatFlags.NoWrap);
            prezimeFormat.Alignment = StringAlignment.Near;
            prezimeFormat.LineAlignment = StringAlignment.Center;

            StringFormat godinaFormat = new StringFormat(StringFormatFlags.NoWrap);
            godinaFormat.Alignment = StringAlignment.Near;
            godinaFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.Alignment = StringAlignment.Near;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat drzavaFormat = new StringFormat(StringFormatFlags.NoWrap);
            drzavaFormat.Alignment = StringAlignment.Near;
            drzavaFormat.LineAlignment = StringAlignment.Center;

            StringFormat rankHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat prezimeHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat godinaHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat drzavaHeaderFormat = Izvestaj.nearCenterFormat;

            String rankTitle = Opcije.Instance.RedBrojString;
            String brojTitle = Opcije.Instance.BrojString;
            String imeTitle = Opcije.Instance.ImeString;
            String prezimeTitle = Opcije.Instance.PrezimeString;
            String godinaTitle = Opcije.Instance.DatumRodjenjaString;
            String klubTitle = Opcije.Instance.KlubString;
            String drzavaTitle = Opcije.Instance.DrzavaString;

            Columns.Clear();

            bool drawItemRect = false;
			ReportColumn column = addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            column.DrawItemRect = drawItemRect;
            if (stampajBroj)
            {
                column = addColumn(xBroj, brojWidth, brojFormat, brojTitle, brojHeaderFormat);
                column.DrawItemRect = drawItemRect;
            }
            column = addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xPrezime, prezimeWidth, prezimeFormat, prezimeTitle, prezimeHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xGodina, godinaWidth, godinaFormat, godinaTitle, godinaHeaderFormat);
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
