using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;
using System.Windows.Forms;

namespace Bilten.Report
{
    class SpravaIzvestaj : Izvestaj
    {
        private List<SpravaLista> liste = new List<SpravaLista>();
        private bool svakaSpravaNaPosebnojStrani;

        public SpravaIzvestaj(Sprava sprava, IList<RezultatSprava> rezultati,
            bool kvalColumn, string documentName, bool prikaziPenal, DataGridView formGrid, bool markFirstRows,
            int numRowsToMark, int brojEOcena, Takmicenje takmicenje, bool prikaziBonus) : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemFont = new Font("Arial", 8);
            Font itemsHeaderFont = new Font("Arial", 8, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            liste.Add(new SpravaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                kvalColumn, sprava, prikaziPenal, formGrid, markFirstRows, numRowsToMark, brojEOcena, takmicenje.TakBrojevi,
                prikaziBonus));
		}

        public SpravaIzvestaj(bool obaPreskoka, IList<RezultatPreskok> rezultati,
            bool kvalColumn, string documentName, bool prikaziPenal, DataGridView formGrid, bool markFirstRows,
            int numRowsToMark, int brojEOcena, Takmicenje takmicenje, bool prikaziBonus) : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemFont = new Font("Arial", 8);
            Font itemsHeaderFont = new Font("Arial", 8, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            liste.Add(new SpravaLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati,
                kvalColumn, obaPreskoka, prikaziPenal, formGrid, markFirstRows, numRowsToMark, brojEOcena,
                takmicenje.TakBrojevi, prikaziBonus));
        }

        public SpravaIzvestaj(List<List<RezultatSprava>> rezultatiSprave,
            List<RezultatPreskok> rezultatiPreskok, bool obaPreskoka, Gimnastika gim,
            bool kvalColumn, string documentName, int brojSpravaPoStrani, bool prikaziPenal,
            DataGridView formGrid, bool markFirstRows, int numRowsToMark, int brojEOcena, Takmicenje takmicenje,
            bool prikaziBonus) : base(takmicenje)
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
                    relY = 0.0f /*+ 0.03f*/;
                }
                if (sprava != Sprava.Preskok)
                {
                    int spravaIndex = i;
                    if (i > Sprave.indexOf(Sprava.Preskok, gim))
                        spravaIndex--;

                    SpravaLista lista = new SpravaLista(this, page, 0f, itemFont, itemsHeaderFont,
                        rezultatiSprave[spravaIndex], kvalColumn, sprava, prikaziPenal, formGrid, markFirstRows,
                        numRowsToMark, brojEOcena, takmicenje.TakBrojevi, prikaziBonus);
                    lista.RelY = relY;
                    liste.Add(lista);
                }
                else
                {
                    SpravaLista lista = new SpravaLista(this, page, 0f, itemFont, itemsHeaderFont,
                        rezultatiPreskok, kvalColumn, obaPreskoka, prikaziPenal, formGrid, markFirstRows,
                        numRowsToMark, brojEOcena, takmicenje.TakBrojevi, prikaziBonus);
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
        private Brush totalBrush;
        private Brush totalAllBrush;

        private bool kvalColumn;
        private Sprava sprava;
        private bool obaPreskoka;
        private bool prikaziPenal;
        private int brojEOcena;
        private bool stampajBroj;
        private bool prikaziBonus;

        public SpravaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatSprava> rezultati,
            bool kvalColumn, Sprava sprava, bool prikaziPenal, DataGridView formGrid, bool markFirstRows,
            int numRowsToMark, int brojEOcena, bool stampajBroj, bool prikaziBonus)
            : base(izvestaj, pageNum, y, itemFont,
            itemsHeaderFont, formGrid)
        {
            this.kvalColumn = kvalColumn;
            this.sprava = sprava;
            this.prikaziPenal = prikaziPenal;
            this.markFirstRows = markFirstRows;
            this.numRowsToMark = numRowsToMark;
            this.brojEOcena = brojEOcena;
            this.stampajBroj = stampajBroj;
            this.prikaziBonus = prikaziBonus;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati);
        }

        public SpravaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatPreskok> rezultati,
            bool kvalColumn, bool obaPreskoka, bool prikaziPenal, DataGridView formGrid, bool markFirstRows,
            int numRowsToMark, int brojEOcena, bool stampajBroj, bool prikaziBonus)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.kvalColumn = kvalColumn;
            this.sprava = Sprava.Preskok;
            this.obaPreskoka = obaPreskoka;
            this.prikaziPenal = prikaziPenal;
            this.markFirstRows = markFirstRows;
            this.numRowsToMark = numRowsToMark;
            this.brojEOcena = brojEOcena;
            this.stampajBroj = stampajBroj;
            this.prikaziBonus = prikaziBonus;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

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
            int rowNum = 0;
            foreach (RezultatSprava rez in rezultati)
            {
                ++rowNum;
                List<object> items = new List<object> {rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.D, rez.E, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus), rowNum.ToString() };
                if (brojEOcena > 0) items.Insert(4, rez.E1);
                if (brojEOcena > 1) items.Insert(5, rez.E2);
                if (brojEOcena > 2) items.Insert(6, rez.E3);
                if (brojEOcena > 3) items.Insert(7, rez.E4);
                if (brojEOcena > 4) items.Insert(8, rez.E5);
                if (brojEOcena > 5) items.Insert(9, rez.E6);
                if (prikaziBonus) items.Insert(5 + brojEOcena, rez.Bonus);
                if (prikaziPenal)
                {
                    if (prikaziBonus)
                        items.Insert(6 + brojEOcena, rez.Penalty);
                    else
                        items.Insert(5 + brojEOcena, rez.Penalty);
                }
                if (stampajBroj)
                {
                    string broj = (rez.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? rez.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
            }
            return result;
        }

        private List<object[]> getPreskokReportItems(IList<RezultatPreskok> rezultati)
        {
            List<object[]> result = new List<object[]>();
            int rowNum = 0;
            foreach (RezultatPreskok rez in rezultati)
            {
                ++rowNum;
                List<object> items;
                if (obaPreskoka)
                {
                    items = new List<object> {rez.Rank, rez.PrezimeIme, rez.KlubDrzava, "1", "2",
                            rez.D, rez.D_2, rez.E, rez.E_2, rez.Total, rez.Total_2, 
                            rez.TotalObeOcene, KvalifikacioniStatusi.toString(rez.KvalStatus), rowNum.ToString() };
                    if (brojEOcena > 0) { items.Insert(7, rez.E1); items.Insert(8, rez.E1_2); }
                    if (brojEOcena > 1) { items.Insert(9, rez.E2); items.Insert(10, rez.E2_2); }
                    if (brojEOcena > 2) { items.Insert(11, rez.E3); items.Insert(12, rez.E3_2); }
                    if (brojEOcena > 3) { items.Insert(13, rez.E4); items.Insert(14, rez.E4_2); }
                    if (brojEOcena > 4) { items.Insert(15, rez.E5); items.Insert(16, rez.E5_2); }
                    if (brojEOcena > 5) { items.Insert(17, rez.E6); items.Insert(18, rez.E6_2); }
                    if (prikaziBonus)
                    { items.Insert(9 + 2 * brojEOcena, rez.Bonus); items.Insert(10 + 2 * brojEOcena, rez.Bonus_2); }
                    if (prikaziPenal)
                    {
                        if (prikaziBonus)
                        {
                            items.Insert(11 + 2 * brojEOcena, rez.Penalty); items.Insert(12 + 2 * brojEOcena, rez.Penalty_2);
                        }
                        else
                        {
                            items.Insert(9 + 2 * brojEOcena, rez.Penalty); items.Insert(10 + 2 * brojEOcena, rez.Penalty_2);
                        }
                    }                    
                }
                else
                {
                    items = new List<object> { rez.Rank, rez.PrezimeIme, rez.KlubDrzava,
                            rez.D, rez.E, rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus), rowNum.ToString() };
                    if (brojEOcena > 0) items.Insert(4, rez.E1);
                    if (brojEOcena > 1) items.Insert(5, rez.E2);
                    if (brojEOcena > 2) items.Insert(6, rez.E3);
                    if (brojEOcena > 3) items.Insert(7, rez.E4);
                    if (brojEOcena > 4) items.Insert(8, rez.E5);
                    if (brojEOcena > 5) items.Insert(9, rez.E6);
                    if (prikaziBonus) items.Insert(5 + brojEOcena, rez.Bonus);
                    if (prikaziPenal)
                    {
                        if (prikaziBonus)
                            items.Insert(6 + brojEOcena, rez.Penalty);
                        else
                            items.Insert(5 + brojEOcena, rez.Penalty);
                    }
                }
                if (stampajBroj)
                {
                    string broj = (rez.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? rez.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
            }
            return result;
        }

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);

            if (obaPreskoka)
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
            float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
            float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;

            // TODO3: Ne bi trebalo pristupati kolonama po fixnom indexu (kao u sledecoj liniji) zato sto je moguce da se
            // index promeni (ako npr. dodam novu kolonu). Isto i u ostalim izvestajima.

            // TODO3: Trenutno se velicine svih kolona za ocene podesavaju prema velicini prve kolone (D). Promeni da se
            // svaka podesava odvojeno. (i u ostalim izvestajima)

            // skok i kval sam podesio kao polovinu Rank kolone.
            float rankWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
            float brojWidth = rankWidth;
            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float skokWidth = rankWidth / 2;
            // TODO5: Ovo je privremeno
            float ocenaWidth;
            if (brojEOcena > 0)
                ocenaWidth = g.MeasureString("00,000", itemFont).Width;
            else
                ocenaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
            float kvalWidth = rankWidth / 2;

            // TODO5: Smanji sirinu ocena kada ima vise E ocena, da sve moze da stane

            int brojOcena = 3;
            if (prikaziBonus)
                ++brojOcena;
            if (prikaziPenal)
                ++brojOcena;

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
            float xKlub = xIme + imeWidth;
            float xSkok = 0.0f;
            float xSprava;
            if (obaPreskoka)
            {
                xSkok = xKlub + klubWidth;
                xSprava = xSkok + skokWidth;
            }
            else
                xSprava = xKlub + klubWidth;

            float xKval = xSprava + (ocenaWidth * (brojOcena + brojEOcena));
            float xTotalObaPreskoka = 0.0f;  // Total oba preskoka
            if (obaPreskoka)
            {
                xTotalObaPreskoka = xKval;
                xKval = xTotalObaPreskoka + ocenaWidth;
            }

            float xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;

            float delta = (contentBounds.Right - xRightEnd) / 2;  // moza da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;

            xRank += delta;
            xBroj += delta;
            xIme += delta;
            xKlub += delta;
            if (obaPreskoka)
                xSkok += delta;
            xSprava += delta;
            xTotalObaPreskoka += delta;
            xKval += delta;
            xRightEnd += delta;

            float xE = xSprava + ocenaWidth;
            float xCurr = xE + (1 + brojEOcena) * ocenaWidth;
            float xBonus = 0f;
            if (prikaziBonus)
            {
                xBonus = xCurr;
                xCurr += ocenaWidth;
            }
            float xPen = 0f;
            if (prikaziPenal)
            {
                xPen = xCurr;
                xCurr += ocenaWidth;
            }
            float xTot = xCurr;

            float spravaDWidth = ocenaWidth;
            float spravaEWidth = ocenaWidth;
            float spravaTotWidth = ocenaWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;

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
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat skokHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = Opcije.Instance.RankString;
            String brojTitle = Opcije.Instance.BrojString;
            String imeTitle = Opcije.Instance.ImeString;
            String klubTitle = Opcije.Instance.KlubDrzavaString;
            String skokTitle = ""; // TODO3: Neka bude uspravno (isto i u izvestaju za sudijske formulare).
            String totalTitle = Opcije.Instance.TotalString;
            String kvalTitle = String.Empty;

            Columns.Clear();

            ReportColumn column = addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            column.IncludeInMarking = true;

            if (stampajBroj)
            {
                column = addColumn(xBroj, brojWidth, brojFormat, brojTitle, brojHeaderFormat);
                column.IncludeInMarking = true;
            }

            column = addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            column.IncludeInMarking = true;

            column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            column.IncludeInMarking = true;

            if (obaPreskoka)
            {
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSkok, skokWidth, null, skokFormat,
                  skokTitle, skokHeaderFormat);
            }

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtBon = "F" + Opcije.Instance.BrojDecimalaBon;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            ReportColumn column1;
            if (obaPreskoka)
                column1 = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xSprava, spravaDWidth, fmtD,
                  spravaFormat, "D", spravaHeaderFormat);
            else
                column1 = addColumn(xSprava, spravaDWidth, fmtD, spravaFormat, "D", spravaHeaderFormat);
            column1.Image = SlikeSprava.getImage(sprava);
            column1.Split = true;
            column1.Span = true;

            ReportColumn prevColumn = column1;
            for (int i = 0; i < brojEOcena; ++i)
            {
                string eOcenaHeader = "E" + (i+1).ToString();
                if (obaPreskoka)
                    column = addDvaPreskokaColumn(prevColumn.getItemsIndexEnd(), 2, xE, spravaEWidth, fmtE, spravaFormat,
                      eOcenaHeader, spravaHeaderFormat);
                else
                    column = addColumn(xE, spravaEWidth, fmtE, spravaFormat, eOcenaHeader, spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
                xE += ocenaWidth;
                prevColumn = column;
            }

            if (obaPreskoka)
                column = addDvaPreskokaColumn(prevColumn.getItemsIndexEnd(), 2, xE, spravaEWidth, fmtE, spravaFormat,
                  "E", spravaHeaderFormat);
            else
                column = addColumn(xE, spravaEWidth, fmtE, spravaFormat, "E", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (prikaziBonus)
            {
                if (obaPreskoka)
                    column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xBonus, ocenaWidth, fmtBon, spravaFormat,
                      "B", spravaHeaderFormat);
                else
                    column = addColumn(xBonus, ocenaWidth, fmtBon, spravaFormat, "B", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }
            
            if (prikaziPenal)
            {
                if (obaPreskoka)
                    column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xPen, ocenaWidth, fmtPen, spravaFormat,
                      "Pen.", spravaHeaderFormat);
                else
                    column = addColumn(xPen, ocenaWidth, fmtPen, spravaFormat, "Pen.", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            string title = Opcije.Instance.TotalString;
            if (obaPreskoka)
                column = addDvaPreskokaColumn(column.getItemsIndexEnd(), 2, xTot, spravaTotWidth, fmtTot, spravaFormat,
                    title, spravaHeaderFormat);
            else
                column = addColumn(xTot, spravaTotWidth, fmtTot, spravaFormat, title, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;
            column.Brush = totalBrush;

            if (column1.Span)
                column1.SpanEndColumn = column;

            if (obaPreskoka)
            {
                column = addColumn(column.getItemsIndexEnd(), xTotalObaPreskoka, ocenaWidth, fmtTot, totalFormat,
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

    public class DvaPreskokaReportColumn : ReportColumn
    {
        private bool drawPartItemRect;
        public bool DrawPartItemRect
        {
            get { return drawPartItemRect; }
            set { drawPartItemRect = value; }
        }

        public DvaPreskokaReportColumn(int itemsIndex, int itemsSpan, float x, float width, string headerTitle)
            : base(itemsIndex, x, width, headerTitle)
        {
            this.itemsSpan = itemsSpan;
        }

        public override void draw(Graphics g, Pen pen, object[] itemsRow, Font itemFont, Brush blackBrush)
        {
            string rowNum = this.getFormattedString(itemsRow, itemsRow.Length - 1);
            int currentRow = 0;
            if (!Int32.TryParse(rowNum, out currentRow))
                currentRow = 0;

            if (!Visible)
                return;

            if (this.Brush != null)
            {
                g.FillRectangle(this.Brush, itemRect.X, itemRect.Y,
                    itemRect.Width, itemRect.Height);
            }
            if (Lista != null && Lista.markFirstRows && this.IncludeInMarking
                && currentRow > 0 && currentRow <= Lista.numRowsToMark)
            {
                g.FillRectangle(this.Lista.markFirstRowsBrush, itemRect.X, itemRect.Y,
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

            if (this.DrawPartItemRect)
            {
                g.DrawRectangle(pen, itemRect1.X, itemRect1.Y,
                    itemRect1.Width, itemRect1.Height);
                g.DrawRectangle(pen, itemRect2.X, itemRect2.Y,
                    itemRect2.Width, itemRect2.Height);
            }

            string item1 = this.getFormattedString(itemsRow, itemsIndex);
            string item2 = this.getFormattedString(itemsRow, itemsIndex + 1);
            g.DrawString(item1, itemFont, blackBrush, itemRect1, this.ItemRectFormat);
            g.DrawString(item2, itemFont, blackBrush, itemRect2, this.ItemRectFormat);
        }
    }
}
