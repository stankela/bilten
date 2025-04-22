using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Bilten.UI;
using Bilten.Exceptions;

namespace Bilten.Report
{
    class StartListaIzvestaj : Izvestaj
    {
        private bool svakaSpravaNaPosebnojStrani;
        private bool dveKolone;

        public StartListaIzvestaj(StartListaNaSpravi startLista, string documentName, bool stampajRedniBroj,
            bool stampajKlub, bool stampajKategoriju, DataGridView formGrid, Takmicenje takmicenje, Font itemFont,
            bool resizeByGrid)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new StartListaLista(this, 1, 0f, itemFont, itemsHeaderFont, startLista, stampajRedniBroj, 
                stampajKlub, stampajKategoriju, formGrid, takmicenje.TakBrojevi, resizeByGrid));
		}

        public StartListaIzvestaj(List<StartListaNaSpravi> startListe, string documentName, int brojSpravaPoStrani,
            bool stampajRedniBroj, bool stampajKlub, bool stampajKategoriju, DataGridView formGrid, Takmicenje takmicenje,
            Font itemFont, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;
            this.dveKolone = brojSpravaPoStrani > 3;
            if (dveKolone)
            {
                Margins = new Margins(50, 50, 100, 100);
            }

            for (int i = 0; i < startListe.Count; i++)
            {
                StartListaLista lista = new StartListaLista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont,
                    startListe[i], stampajRedniBroj, stampajKlub, stampajKategoriju, formGrid, takmicenje.TakBrojevi,
                    resizeByGrid);
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            if (dveKolone)
            {
                poredjajListeUDveKolone(g, contentBounds, reportListe);
            }
            else
            {
                poredjajListeUJednuKolonu(g, contentBounds, reportListe, svakaSpravaNaPosebnojStrani);
            }
        }
    }

    public class StartListaLista : ReportLista
    {
        private Sprava sprava;
        private bool stampajRedniBroj;
        private bool stampajKlub;
        private bool stampajKategoriju;
        private bool praznaLista;
        private bool stampajBroj;

        public StartListaLista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            StartListaNaSpravi startLista, bool stampajRedniBroj, bool stampajKlub, bool stampajKategoriju,
            DataGridView formGrid, bool stampajBroj, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = startLista.Sprava;
            this.stampajRedniBroj = stampajRedniBroj;
            this.stampajKlub = stampajKlub;
            this.stampajKategoriju = stampajKategoriju;
            this.praznaLista = startLista.Nastupi.Count == 0;
            this.stampajBroj = stampajBroj;
            this.resizeByGrid = resizeByGrid;

            fetchItems(startLista);
        }

        public int getImeColumnIndex()
        {
            return stampajBroj ? 2 : 1;
        }

        public int getKlubColumnIndex()
        {
            if (stampajKlub)
                return getImeColumnIndex() + 1;
            else
                return -1;
        }

        public int getKategorijaColumnIndex()
        {
            if (!stampajKategoriju)
                return -1;
            if (stampajKlub)
                return getKlubColumnIndex() + 1;
            else
                return getImeColumnIndex() + 1;
        }

        private void fetchItems(StartListaNaSpravi startLista)
        {
            items = getStartListaReportItems(startLista);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getStartListaReportItems(StartListaNaSpravi startLista)
        {
            List<object[]> result = new List<object[]>();
            for (int i = 0; i < startLista.Nastupi.Count; i++)
            {
                string redBroj = stampajRedniBroj ? (i+1).ToString() : String.Empty;
                NastupNaSpravi nastup = startLista.Nastupi[i];
                List<object> items = new List<object>() { redBroj, nastup.PrezimeIme };
                if (stampajBroj)
                {
                    string broj = (nastup.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? nastup.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                if (stampajKlub)
                {
                    items.Add(nastup.KlubDrzava);
                }
                if (stampajKategoriju)
                {
                    items.Add(nastup.Kategorija);
                }
                result.Add(items.ToArray());
            }
            if (result.Count == 0)
            {
                // hack kojim se obezbedjuje da se stampaju hederi i za liste koje su prazne
                List<object> items = new List<object>() { "", "" };
                if (stampajBroj)
                {
                    items.Add("");
                }
                if (stampajKlub)
                {
                    items.Add("");
                }
                if (stampajKategoriju)
                {
                    items.Add("");
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
            result.Add(getKategorijaColumnIndex());
            return result;
        }

        protected override void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
          List<bool> rszByGrid /* output parameter */)
        {
            // First, create columns

            float imeWidth;
            float klubWidth;
            float kategorijaWidth;
            if (columnWidths.Count == 0)
            {
                // Prvi pass
                if (resizeByGrid)
                {
                    float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
                    float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;
                    imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                    klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
                    kategorijaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
                }
                else
                {
                    // Resize by content
                    // Proizvoljna vrednost. Koristi se u prvom pozivu setupContent. U drugom pozivu setupContent, imeWidth,
                    // klubWidth i kategorijaWidth ce dobiti pravu vrednost, koja je dovoljna da i najduzi tekst stane bez
                    // odsecanja.
                    imeWidth = 1f;
                    klubWidth = 1f;
                    kategorijaWidth = 1f;
                }
                if (getKlubColumnIndex() == -1)
                {
                    klubWidth = 0f;
                }
                if (getKategorijaColumnIndex() == -1)
                {
                    kategorijaWidth = 0f;
                }
            }
            else if (columnWidths.Count == 3)
            {
                // Drugi pass, sirine kolona su podesene
                imeWidth = columnWidths[0];
                klubWidth = columnWidths[1];
                kategorijaWidth = columnWidths[2];
            }
            else
            {
                throw new Exception("Trenutno, samo 3 kolone mogu da se podesavaju");
            }
            createColumns(g, contentBounds, imeWidth, klubWidth, kategorijaWidth);

            // Then, layout contents vertically

            itemHeight = itemFont.GetHeight(g) * 1.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth,
            float kategorijaWidth)
        {
            string redBrojTitle = Opcije.Instance.RedBrojString;
            float redBrojWidth = getColumnWidth(g, REDNI_BROJ_MAX_TEXT, redBrojTitle);

            string brojTitle = Opcije.Instance.BrojString;
            float brojWidth = getColumnWidth(g, BROJ_MAX_TEXT, brojTitle);

            string imeTitle = Opcije.Instance.ImeString;
            imeWidth = getColumnWidth(g, imeWidth, imeTitle);

            string klubTitle = Opcije.Instance.KlubDrzavaString;
            klubWidth = getColumnWidth(g, klubWidth, klubTitle);

            string kategorijaTitle = Opcije.Instance.KategorijaString;
            kategorijaWidth = getColumnWidth(g, kategorijaWidth, kategorijaTitle);
            
            float xRedBroj = contentBounds.X;
            float xCurr = xRedBroj + redBrojWidth;
            float xBroj = 0f;
            if (stampajBroj)
            {
                xBroj = xCurr;
                xCurr += brojWidth;
            }
            float xIme = xCurr;
            xCurr += imeWidth;
            float xKlub = 0f;
            if (stampajKlub)
            {
                xKlub = xCurr;
                xCurr += klubWidth;
            }
            float xKategorija = 0f;
            if (stampajKategoriju)
            {
                xKategorija = xCurr;
                xCurr += kategorijaWidth;
            }
            xRightEnd = xCurr;

            StringFormat redBrojFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat kategorijaFormat = Izvestaj.nearCenterFormat;

            StringFormat redBrojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat kategorijaHeaderFormat = Izvestaj.centerCenterFormat;

            Columns.Clear();

            ReportColumn column1 = addColumn(xRedBroj, redBrojWidth, redBrojFormat, redBrojTitle, redBrojHeaderFormat);
            column1.Image = SlikeSprava.getImage(sprava);
            column1.Split = true;
            column1.Span = true;

            ReportColumn column;
            if (stampajBroj)
            {
                column = addColumn(xBroj, brojWidth, brojFormat, brojTitle, brojHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            column = addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (stampajKlub)
            {
                column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            if (stampajKategoriju)
            {
                column = addColumn(xKategorija, kategorijaWidth, kategorijaFormat, kategorijaTitle, kategorijaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            if (column1.Span)
                column1.SpanEndColumn = column;
        }

        protected override void drawGroupHeader(Graphics g, int groupId, RectangleF groupHeaderRect)
        {
            foreach (ReportColumn col in Columns)
            {
                if (!col.Visible)
                    continue;

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
                            // Mnozim sa 0.95 zato sto se desava da slike pauza (koje nemaju okvir oko
                            // sebe) prebrisu gornju liniju pravougaonika u koji su smestene.
                            Izvestaj.scaleImageIsotropically(g, col.Image, imageRect, 0.95f);
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
}
