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
        private bool svakaSpravaNaPosebnojStrani;

        public SpravaFinaleKupaIzvestaj(Sprava sprava, IList<RezultatSpravaFinaleKupa> rezultati, bool kvalColumn,
            string documentName, DataGridView formGrid, Takmicenje takmicenje, Font itemFont, bool prikaziPenal,
            bool prikaziBonus, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            reportListe.Add(new SpravaFinaleKupaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                kvalColumn, sprava, formGrid, prikaziPenal, prikaziBonus, resizeByGrid));
        }

        public SpravaFinaleKupaIzvestaj(IList<RezultatPreskokFinaleKupa> rezultati, bool kvalColumn, string documentName,
            DataGridView formGrid, Takmicenje takmicenje, Font itemFont, bool prikaziPenal, bool prikaziBonus,
            bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            reportListe.Add(new SpravaFinaleKupaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                kvalColumn, formGrid, prikaziPenal, prikaziBonus, resizeByGrid));
        }

        public SpravaFinaleKupaIzvestaj(List<List<RezultatSpravaFinaleKupa>> rezultatiSprave,
            List<RezultatPreskokFinaleKupa> rezultatiPreskok, bool kvalColumn, string documentName, int brojSpravaPoStrani,
            DataGridView formGrid, Takmicenje takmicenje, Font itemFont, bool prikaziPenal, bool prikaziBonus,
            bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                SpravaFinaleKupaLista lista;
                if (sprava != Sprava.Preskok)
                {
                    int spravaIndex = i;
                    if (i > Sprave.indexOf(Sprava.Preskok, takmicenje.Gimnastika))
                        spravaIndex--;

                    lista = new SpravaFinaleKupaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultatiSprave[spravaIndex],
                        kvalColumn, sprava, formGrid, prikaziPenal, prikaziBonus, resizeByGrid);
                }
                else
                {
                    lista = new SpravaFinaleKupaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultatiPreskok, kvalColumn,
                        formGrid, prikaziPenal, prikaziBonus, resizeByGrid);
                }
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            poredjajListeUJednuKolonu(g, contentBounds, reportListe, svakaSpravaNaPosebnojStrani);
        }
    }

    public class SpravaFinaleKupaLista : ReportLista
    {
        private Brush totalBrush;
        private Brush totalAllBrush;

        private bool kvalColumn;
        private Sprava sprava;
        private bool prikaziPenal;
        private bool prikaziBonus;

        public SpravaFinaleKupaLista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            IList<RezultatSpravaFinaleKupa> rezultati, bool kvalColumn, Sprava sprava, DataGridView formGrid,
            bool prikaziPenal, bool prikaziBonus, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.kvalColumn = kvalColumn;
            this.sprava = sprava;
            this.resizeByGrid = resizeByGrid;
            this.prikaziPenal = prikaziPenal;
            this.prikaziBonus = prikaziBonus;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati);
        }

        public SpravaFinaleKupaLista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            IList<RezultatPreskokFinaleKupa> rezultati, bool kvalColumn, DataGridView formGrid, bool prikaziPenal,
            bool prikaziBonus, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.kvalColumn = kvalColumn;
            this.sprava = Sprava.Preskok;
            this.resizeByGrid = resizeByGrid;
            this.prikaziPenal = prikaziPenal;
            this.prikaziBonus = prikaziBonus;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati);
        }

        public int getImeColumnIndex()
        {
            return 1;
        }

        public int getKlubColumnIndex()
        {
            return 2;
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
                List<object> items = new List<object>() { rez.Rank, rez.PrezimeIme, rez.KlubDrzava, "I", "II",
                            rez.D_PrvoKolo, rez.D_DrugoKolo, rez.E_PrvoKolo, rez.E_DrugoKolo, rez.TotalPrvoKolo,
                            rez.TotalDrugoKolo, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus) };
                if (prikaziPenal)
                {
                    items.Insert(9, rez.Pen_PrvoKolo);
                    items.Insert(10, rez.Pen_DrugoKolo);
                }
                if (prikaziBonus)
                {
                    items.Insert(9, rez.Bonus_PrvoKolo);
                    items.Insert(10, rez.Bonus_DrugoKolo);
                }
                result.Add(items.ToArray());
            }
            return result;
        }

        private void fetchItems(IList<RezultatPreskokFinaleKupa> rezultati)
        {
            items = getSpravaFinaleKupaReportItems(rezultati);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getSpravaFinaleKupaReportItems(IList<RezultatPreskokFinaleKupa> rezultati)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatPreskokFinaleKupa rez in rezultati)
            {
                List<object> items = new List<object>() { rez.Rank, rez.PrezimeIme, rez.KlubDrzava, "I", "II", 1, 2, 1, 2,
                            rez.D_PrvoKolo, rez.D_2_PrvoKolo, rez.D_DrugoKolo, rez.D_2_DrugoKolo,
                            rez.E_PrvoKolo, rez.E_2_PrvoKolo, rez.E_DrugoKolo, rez.E_2_DrugoKolo,
                            rez.TotalPrvoKolo, rez.Total_2_PrvoKolo, rez.TotalDrugoKolo, rez.Total_2_DrugoKolo,
                            rez.TotalObeOcene_PrvoKolo, rez.TotalObeOcene_DrugoKolo, rez.Total,
                            KvalifikacioniStatusi.toString(rez.KvalStatus) };
                if (prikaziPenal)
                {
                    items.Insert(17, rez.Pen_PrvoKolo);
                    items.Insert(18, rez.Pen_2_PrvoKolo);
                    items.Insert(19, rez.Pen_DrugoKolo);
                    items.Insert(20, rez.Pen_2_DrugoKolo);
                }
                if (prikaziBonus)
                {
                    items.Insert(17, rez.Bonus_PrvoKolo);
                    items.Insert(18, rez.Bonus_2_PrvoKolo);
                    items.Insert(19, rez.Bonus_DrugoKolo);
                    items.Insert(20, rez.Bonus_2_DrugoKolo);
                }
                result.Add(items.ToArray());
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
                    // Proizvoljna vrednost. Koristi se u prvom pozivu setupContent. U drugom pozivu setupContent, imeWidth
                    // i klubWidth ce dobiti pravu vrednost, koja je dovoljna da i najduzi tekst stane bez
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

            itemHeight = itemFont.GetHeight(g) * 2.9f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;
            if (sprava == Sprava.Preskok)
                itemHeight *= 2;

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

            string koloTitle = "";
            float koloWidth = getColumnWidth(g, KOLO_MAX_TEXT, koloTitle);

            string skokTitle = "";
            float skokWidth = getColumnWidth(g, SKOK_MAX_TEXT, skokTitle);

            float ocenaWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, "");

            string spravaDTitle = Opcije.Instance.DString;
            float spravaDWidth = getColumnWidth(g, ocenaWidth, spravaDTitle);

            string spravaETitle = Opcije.Instance.EString;
            float spravaEWidth = getColumnWidth(g, ocenaWidth, spravaETitle);

            string spravaBonusTitle = Opcije.Instance.BonusString;
            float spravaBonusWidth = getColumnWidth(g, ocenaWidth, spravaBonusTitle);

            string spravaPenTitle = Opcije.Instance.PenaltyString;
            float spravaPenWidth = getColumnWidth(g, ocenaWidth, spravaPenTitle);

            string spravaTotTitle = Opcije.Instance.TotalString;
            float spravaTotWidth = getColumnWidth(g, ocenaWidth, spravaTotTitle);

            string totalTitle = Opcije.Instance.TotalString;
            float totalWidth = getColumnWidth(g, ocenaWidth, totalTitle);

            string kvalTitle = "";
            float kvalWidth = getColumnWidth(g, QUAL_MAX_TEXT, kvalTitle);
            
            float xRank = contentBounds.X;
            float xIme = xRank + rankWidth;
            float xKlub = xIme + imeWidth;
            float xKolo = xKlub + klubWidth;
            float xCurr = xKolo + koloWidth;
            float xSkok = 0f;
            if (sprava == Sprava.Preskok)
            {
                xSkok = xCurr;
                xCurr += skokWidth;
            }
            float xSprava = xCurr;
            float xE = xSprava + spravaDWidth;
            xCurr = xE + spravaEWidth;
            float xBonus = 0f;
            if (prikaziBonus)
            {
                xBonus = xCurr;
                xCurr += spravaBonusWidth;
            }
            float xPen = 0f;
            if (prikaziPenal)
            {
                xPen = xCurr;
                xCurr += spravaPenWidth;
            }
            float xTot = xCurr;
            xCurr = xTot + spravaTotWidth;
            float xTotalObeOcene = 0f;
            if (sprava == Sprava.Preskok)
            {
                xTotalObeOcene = xCurr;
                xCurr += totalWidth;
            }
            float xTotal = xCurr;
            float xKval = xTotal + totalWidth;

            xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat koloFormat = Izvestaj.centerCenterFormat;
            StringFormat skokFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat koloHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat skokHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            column = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), 2, xKolo, koloWidth, null, koloFormat, koloTitle,
                koloHeaderFormat);
            if (sprava == Sprava.Preskok)
            {
                column = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), 4, xSkok, skokWidth, null, skokFormat,
                    skokTitle, skokHeaderFormat);
            }

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtBon = "F" + Opcije.Instance.BrojDecimalaBon;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            int itemsSpan = (sprava == Sprava.Preskok) ? 4 : 2;
            ReportColumn column1 = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), itemsSpan, xSprava, spravaDWidth,
                fmtD, spravaFormat, spravaDTitle, spravaHeaderFormat);
            column1.Image = SlikeSprava.getImage(sprava);
            column1.Split = true;
            column1.Span = true;

            column = addSpravaFinaleKupaColumn(column1.getItemsIndexEnd(), itemsSpan, xE, spravaEWidth, fmtE, spravaFormat,
                spravaETitle, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (prikaziBonus)
            {
                column = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), itemsSpan, xBonus, spravaBonusWidth, fmtBon,
                    spravaFormat, spravaBonusTitle, spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            if (prikaziPenal)
            {
                column = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), itemsSpan, xPen, spravaPenWidth, fmtPen,
                    spravaFormat, spravaPenTitle, spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            column = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), itemsSpan, xTot, spravaTotWidth, fmtTot,
                spravaFormat, spravaTotTitle, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;
            column.Brush = totalBrush;

            if (column1.Span)
                column1.SpanEndColumn = column;

            if (sprava == Sprava.Preskok)
            {
                column = addSpravaFinaleKupaColumn(column.getItemsIndexEnd(), 2, xTotalObeOcene, totalWidth, null,
                    totalFormat, totalTitle, totalHeaderFormat);
            }

            column = addColumn(column.getItemsIndexEnd(), xTotal, totalWidth, fmtTot, totalFormat, totalTitle,
                totalHeaderFormat);
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
                g.FillRectangle(this.Brush, itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height);
            }
            if (this.DrawItemRect)
            {
                g.DrawRectangle(pen, itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height);
            }

            if (itemsSpan == 2)
            {
                RectangleF itemRect1 = new RectangleF(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 2);
                RectangleF itemRect2 = new RectangleF(itemRect.X, itemRect.Y + itemRect.Height / 2, itemRect.Width,
                    itemRect.Height / 2);

                // Stampaj liniju koja razdvaja I i II kolo
                RectangleF itemRectPolovina = itemRect1;
                g.DrawRectangle(pen, itemRectPolovina.X, itemRectPolovina.Y, itemRectPolovina.Width,
                    itemRectPolovina.Height);

                if (this.DrawPartItemRect)
                {
                    g.DrawRectangle(pen, itemRect1.X, itemRect1.Y, itemRect1.Width, itemRect1.Height);
                    g.DrawRectangle(pen, itemRect2.X, itemRect2.Y, itemRect2.Width, itemRect2.Height);
                }

                string item1 = this.getFormattedString(itemsRow, itemsIndex);
                string item2 = this.getFormattedString(itemsRow, itemsIndex + 1);
                g.DrawString(item1, itemFont, blackBrush, itemRect1, this.ItemRectFormat);
                g.DrawString(item2, itemFont, blackBrush, itemRect2, this.ItemRectFormat);
            }
            else
            {
                RectangleF itemRect1 = new RectangleF(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 4);
                RectangleF itemRect2 = new RectangleF(itemRect.X, itemRect.Y + itemRect.Height / 4, itemRect.Width,
                    itemRect.Height / 4);
                RectangleF itemRect3 = new RectangleF(itemRect.X, itemRect.Y + itemRect.Height / 2, itemRect.Width,
                    itemRect.Height / 4);
                RectangleF itemRect4 = new RectangleF(itemRect.X, itemRect.Y + itemRect.Height * 3f / 4f, itemRect.Width,
                    itemRect.Height / 4);

                // Stampaj liniju koja razdvaja I i II kolo
                RectangleF itemRectPolovina = new RectangleF(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 2);
                g.DrawRectangle(pen, itemRectPolovina.X, itemRectPolovina.Y, itemRectPolovina.Width,
                    itemRectPolovina.Height);
                
                if (this.DrawPartItemRect)
                {
                    g.DrawRectangle(pen, itemRect1.X, itemRect1.Y, itemRect1.Width, itemRect1.Height);
                    g.DrawRectangle(pen, itemRect2.X, itemRect2.Y, itemRect2.Width, itemRect2.Height);
                    g.DrawRectangle(pen, itemRect3.X, itemRect3.Y, itemRect3.Width, itemRect3.Height);
                    g.DrawRectangle(pen, itemRect4.X, itemRect4.Y, itemRect4.Width, itemRect4.Height);
                }

                string item1 = this.getFormattedString(itemsRow, itemsIndex);
                string item2 = this.getFormattedString(itemsRow, itemsIndex + 1);
                string item3 = this.getFormattedString(itemsRow, itemsIndex + 2);
                string item4 = this.getFormattedString(itemsRow, itemsIndex + 3);
                g.DrawString(item1, itemFont, blackBrush, itemRect1, this.ItemRectFormat);
                g.DrawString(item2, itemFont, blackBrush, itemRect2, this.ItemRectFormat);
                g.DrawString(item3, itemFont, blackBrush, itemRect3, this.ItemRectFormat);
                g.DrawString(item4, itemFont, blackBrush, itemRect4, this.ItemRectFormat);
            }
        }
    }
}
