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
        public SudijeUcesniciIzvestaj(IList<SudijaUcesnik> sudije, DataGridView formGrid,
            string documentName, Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);

            reportListe.Add(new SudijeUcesniciLista(this, 1, 0f, itemFont, itemsHeaderFont, sudije, formGrid, resizeByGrid));
		}

        protected override void doSetupContent(Graphics g)
		{
            poredjajListeUJednuKolonu(g, contentBounds, reportListe, false);
        }
    }

	public class SudijeUcesniciLista : ReportLista
	{
        private Brush totalBrush;
        private Brush totalAllBrush;

        public SudijeUcesniciLista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            IList<SudijaUcesnik> sudije, DataGridView formGrid, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
		{
            this.resizeByGrid = resizeByGrid;
            
            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(sudije);
        }

        public int getImeColumnIndex()
        {
            return 1;
        }

        public int getPrezimeColumnIndex()
        {
            return 2;
        }

        public int getKlubColumnIndex()
        {
            return 3;
        }

        public int getDrzavaColumnIndex()
        {
            return 4;
        }

        private void fetchItems(IList<SudijaUcesnik> sudije)
		{
            items = getTakmicariReportItems(sudije);
		
			groups = new List<ReportGrupa>();
			groups.Add(new ReportGrupa(0, items.Count));
		}

        // TODO5: U registru za sudije treba da se unosi gimnastika (MSG, ZSG) umesto pol

        private List<object[]> getTakmicariReportItems(IList<SudijaUcesnik> gimnasticari)
        {
            List<object[]> result = new List<object[]>();
            for (int i = 0; i < gimnasticari.Count; ++i)
            {
                SudijaUcesnik g = gimnasticari[i];
                string klub = (g.KlubUcesnik != null) ? g.KlubUcesnik.Naziv : String.Empty;
                string drzava = (g.DrzavaUcesnik != null) ? g.DrzavaUcesnik.Kod : string.Empty;
                result.Add(new object[] { (i+1).ToString(), g.Ime, g.Prezime, klub, drzava });
            }
            return result;
        }

        public override List<int> getAdjustableColumnIndexes()
        {
            List<int> result = new List<int>();
            result.Add(getImeColumnIndex());
            result.Add(getPrezimeColumnIndex());
            result.Add(getKlubColumnIndex());
            result.Add(getDrzavaColumnIndex());
            return result;
        }

        // TODO5: Kolona za klub se na engleskom zove "National team" zato sto se ime deli sa klubom za gimnasticare
        // a klub je podesen kao "National team" (u pitanju je Gimnazijada 2025). Mozda nije lose da se razdvoje ove dve
        // opcije (klub za gimnasticare i klub za sudije), zato sto je za sudije ucesnike smisao klub kolone uvek Klub.

        protected override void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
            List<bool> rszByGrid)
        {
            // First, create columns

            float imeWidth;
            float prezimeWidth;
            float klubWidth;
            float drzavaWidth;
            if (columnWidths.Count == 0)
            {
                // Prvi pass
                if (resizeByGrid)
                {
                    float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
                    float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;
                    imeWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
                    prezimeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                    klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
                    drzavaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
                }
                else
                {
                    // Resize by content
                    // Proizvoljna vrednost. Koristi se u prvom pozivu setupContent. U drugom pozivu setupContent, imeWidth,
                    // klubWidth i kategorijaWidth ce dobiti pravu vrednost, koja je dovoljna da i najduzi tekst stane bez
                    // odsecanja.
                    imeWidth = 1f;
                    prezimeWidth = 1f;
                    klubWidth = 1f;
                    drzavaWidth = 1f;
                }
            }
            else if (columnWidths.Count == 4)
            {
                // Drugi pass, sirine kolona su podesene
                imeWidth = columnWidths[0];
                prezimeWidth = columnWidths[1];
                klubWidth = columnWidths[2];
                drzavaWidth = columnWidths[3];
            }
            else
            {
                throw new Exception("Trenutno, samo 4 kolona mogu da se podesavaju");
            }
            createColumns(g, contentBounds, imeWidth, prezimeWidth, klubWidth, drzavaWidth);

            // Then, layout contents vertically

            itemHeight = itemFont.GetHeight(g) * 1.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 2.4f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float prezimeWidth, float klubWidth,
            float drzavaWidth)
		{
            float redBrojWidth = getColumnWidth(g, REDNI_BROJ_MAX_TEXT, Opcije.Instance.RedBrojString);

            // U koloni drzava se prikazuje skraceni kod (koji je manji od hedera kolone koji je obicno "Drzava" ili
            // "Country code"), pa moram da prosirim kolonu
            String drzavaTitle = Opcije.Instance.DrzavaString;
            drzavaWidth = getColumnWidth(g, drzavaWidth, drzavaTitle);
            
            float xRedBroj = contentBounds.X;
            float xIme = xRedBroj + redBrojWidth;
            float xPrezime = xIme + imeWidth;
            float xKlub = xPrezime + prezimeWidth;
            float xDrzava = xKlub + klubWidth;
            
            xRightEnd = xDrzava + drzavaWidth;
            
            StringFormat redBrojFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat prezimeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat drzavaFormat = Izvestaj.nearCenterFormat;

            StringFormat redBrojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat prezimeHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat polHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.nearCenterFormat;
            StringFormat drzavaHeaderFormat = Izvestaj.nearCenterFormat;

            Columns.Clear();

            bool drawItemRect = false;
			ReportColumn column = addColumn(xRedBroj, redBrojWidth, redBrojFormat, Opcije.Instance.RedBrojString,
                redBrojHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xIme, imeWidth, imeFormat, Opcije.Instance.ImeString, imeHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xPrezime, prezimeWidth, prezimeFormat, Opcije.Instance.PrezimeString, prezimeHeaderFormat);
            column.DrawItemRect = drawItemRect;
            column = addColumn(xKlub, klubWidth, klubFormat, Opcije.Instance.KlubString, klubHeaderFormat);
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
