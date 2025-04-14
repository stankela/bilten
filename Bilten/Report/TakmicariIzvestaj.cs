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
        public TakmicariIzvestaj(IList<GimnasticarUcesnik> gimnasticari, DataGridView formGrid, string documentName,
            Takmicenje takmicenje, Font itemFont, bool resizeByGrid) : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);

            reportListe.Add(new TakmicariLista(this, 1, 0f, itemFont, itemsHeaderFont, gimnasticari, takmicenje.Gimnastika,
                formGrid, takmicenje.TakBrojevi, resizeByGrid));
		}

        protected override void doSetupContent(Graphics g)
		{
            poredjajListeUJednuKolonu(g, contentBounds, reportListe, false);
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
            Gimnastika gim, DataGridView formGrid, bool stampajBroj, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
		{
            this.gimnastika = gim;
            this.stampajBroj = stampajBroj;
            this.resizeByGrid = resizeByGrid;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(gimnasticari, gim);
        }

        public int getImeColumnIndex()
        {
            return stampajBroj ? 2 : 1;
        }

        public int getPrezimeColumnIndex()
        {
            return stampajBroj ? 3 : 2;
        }

        public int getDatumRodjenjaColumnIndex()
        {
            return stampajBroj ? 4 : 3;
        }

        public int getKlubColumnIndex()
        {
            return stampajBroj ? 5 : 4;
        }

        public int getDrzavaColumnIndex()
        {
            return stampajBroj ? 6 : 5;
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

        public override List<int> getAdjustableColumnIndexes()
        {
            List<int> result = new List<int>();
            result.Add(getImeColumnIndex());
            result.Add(getPrezimeColumnIndex());
            result.Add(getDatumRodjenjaColumnIndex());
            result.Add(getKlubColumnIndex());
            result.Add(getDrzavaColumnIndex());
            return result;
        }

        protected override void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
            List<bool> rszByGrid)
        {
            // First, create columns

            float imeWidth;
            float prezimeWidth;
            float datumRodjenjaWidth;
            float klubWidth;
            float drzavaWidth;
            if (columnWidths.Count == 0)
            {
                // Prvi pass
                if (resizeByGrid)
                {
                    // Indeksi kolona u gridu su dobri, cak i u slucaju kada se ne prikazuje Broj kao prva kolona, zato sto
                    // je kolona Broj kreirana ali je sakrivena
                    float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
                    float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;
                    imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                    prezimeWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
                    datumRodjenjaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
                    klubWidth = this.formGrid.Columns[4].Width * printWidth / gridWidth;
                    drzavaWidth = this.formGrid.Columns[5].Width * printWidth / gridWidth;
                }
                else
                {
                    // Resize by content
                    // Proizvoljna vrednost. Koristi se u prvom pozivu setupContent. U drugom pozivu setupContent, imeWidth,
                    // klubWidth i kategorijaWidth ce dobiti pravu vrednost, koja je dovoljna da i najduzi tekst stane bez
                    // odsecanja.
                    imeWidth = 1f;
                    prezimeWidth = 1f;
                    datumRodjenjaWidth = 1f;
                    klubWidth = 1f;
                    drzavaWidth = 1f;
                }
            }
            else if (columnWidths.Count == 5)
            {
                // Drugi pass, sirine kolona su podesene
                imeWidth = columnWidths[0];
                prezimeWidth = columnWidths[1];
                datumRodjenjaWidth = columnWidths[2];
                klubWidth = columnWidths[3];
                drzavaWidth = columnWidths[4];
            }
            else
            {
                throw new Exception("Trenutno, samo 5 kolona mogu da se podesavaju");
            }
            createColumns(g, contentBounds, imeWidth, prezimeWidth, datumRodjenjaWidth, klubWidth, drzavaWidth);

            // Then, layout contents vertically

            itemHeight = itemFont.GetHeight(g) * 1.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 2.4f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

		private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float prezimeWidth,
            float datumRodjenjaWidth, float klubWidth, float drzavaWidth)
		{
            string redBrojTitle = Opcije.Instance.RedBrojString;
            float redBrojWidth = getColumnWidth(g, REDNI_BROJ_MAX_TEXT, redBrojTitle);

            string brojTitle = Opcije.Instance.BrojString;
            float brojWidth = getColumnWidth(g, BROJ_MAX_TEXT, brojTitle);

            string imeTitle = Opcije.Instance.ImeString;
            imeWidth = getColumnWidth(g, imeWidth, imeTitle);

            string prezimeTitle = Opcije.Instance.PrezimeString;
            prezimeWidth = getColumnWidth(g, prezimeWidth, prezimeTitle);

            string datumRodjenjaTitle = Opcije.Instance.DatumRodjenjaString;
            datumRodjenjaWidth = getColumnWidth(g, datumRodjenjaWidth, datumRodjenjaTitle);     
            
            string klubTitle = Opcije.Instance.KlubString;
            klubWidth = getColumnWidth(g, klubWidth, klubTitle);

            string drzavaTitle = Opcije.Instance.DrzavaString;
            drzavaWidth = getColumnWidth(g, drzavaWidth, drzavaTitle);

			float xRedniBroj = contentBounds.X;
            float xBroj = 0f;
            float xIme;
            if (stampajBroj)
            {
                xBroj = xRedniBroj + redBrojWidth;
                xIme = xBroj + brojWidth;
            }
            else
            {
                xIme = xRedniBroj + redBrojWidth;
            }
            float xPrezime = xIme + imeWidth;
            float xDatumRodjenja = xPrezime + prezimeWidth;
            float xKlub = xDatumRodjenja + datumRodjenjaWidth;
            float xDrzava = xKlub + klubWidth;
            
            xRightEnd = xDrzava + drzavaWidth;
            
            StringFormat redBrojFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.nearCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat prezimeFormat = Izvestaj.nearCenterFormat;
            StringFormat datumRodjenjaFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat drzavaFormat = Izvestaj.nearCenterFormat;

            StringFormat redBrojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat prezimeHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat datumRodjenjaHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat drzavaHeaderFormat = Izvestaj.nearCenterFormat;

            Columns.Clear();

            bool drawItemRect = false;
            ReportColumn column = addColumn(xRedniBroj, redBrojWidth, redBrojFormat, redBrojTitle, redBrojHeaderFormat);
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
            column = addColumn(xDatumRodjenja, datumRodjenjaWidth, datumRodjenjaFormat, datumRodjenjaTitle,
                datumRodjenjaHeaderFormat);
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
