using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Bilten.UI;

namespace Bilten.Report
{
    class RasporedSudijaIzvestaj : Izvestaj
    {
        private bool svakaSpravaNaPosebnojStrani;
        private bool dveKolone;
        private RasporedSudijaLista vrhovniSudijaLista;
        
        public RasporedSudijaIzvestaj(SudijskiOdborNaSpravi odbor, string documentName, DataGridView formGrid,
            Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);

            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new RasporedSudijaLista(this, 1, 0f, itemFont, itemsHeaderFont, odbor, formGrid, resizeByGrid));
        }

        // Kada je broj sprava veci od 3, stampa se u dve kolone. Kada je broj sprava 1, stampa se svaka sprava na
        // posebnoj strani. Kada je 2 ili 3 stampa se u jednoj koloni, jedno ispod drugog dok ima mesta.
        // To znaci da ako zadamo 2 a na strani ima mesta za 3 sprave, bice stampane 3 sprave.
        // TODO5: Uradi ovako i za sve ostale liste sa manjim brojem elemenata (start liste, kvalifikanti)
        // TODO5: Neka u HeaderFooterForm budu opcije "Stampaj u jednoj koloni" i "Stampaj u dve kolone". Izbaci opcije
        //        za broj sprava po strani

        public RasporedSudijaIzvestaj(List<SudijskiOdborNaSpravi> odbori, string documentName, int brojSpravaPoStrani,
            DataGridView formGrid, SudijaUcesnik vrhovniSudija, Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);

            this.svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;
            this.dveKolone = brojSpravaPoStrani > 3;
            if (dveKolone)
                Margins = new Margins(50, 50, 100, 100);

            for (int i = 0; i < odbori.Count; i++)
            {
                RasporedSudijaLista lista = new RasporedSudijaLista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont,
                    odbori[i], formGrid, resizeByGrid);
                reportListe.Add(lista);
            }
            vrhovniSudijaLista = new RasporedSudijaLista(this, 1, 0f, itemFont, itemsHeaderFont, vrhovniSudija,
                formGrid, resizeByGrid);
        }

        protected override void doSetupContent(Graphics g)
        {
            if (vrhovniSudijaLista == null)
            {
                if (dveKolone)
                {
                    poredjajListeUDveKolone(g, contentBounds, reportListe);
                }
                else
                {
                    poredjajListeUJednuKolonu(g, contentBounds, reportListe, svakaSpravaNaPosebnojStrani);
                }
                return;
            }

            poredjajListeUJednuKolonu(g, contentBounds, new List<ReportLista>() { vrhovniSudijaLista },
                svakaSpravaNaPosebnojStrani);
            if (dveKolone)
            {
                poredjajListeUDveKolone(g, contentBounds, reportListe, vrhovniSudijaLista.LastPageNum,
                    vrhovniSudijaLista.EndY - contentBounds.Y);
            }
            else
            {
                poredjajListeUJednuKolonu(g, contentBounds, reportListe, svakaSpravaNaPosebnojStrani,
                    vrhovniSudijaLista.LastPageNum, vrhovniSudijaLista.EndY - contentBounds.Y);
            }            
        }

        public override void drawContent(Graphics g, int pageNum)
        {
            if (vrhovniSudijaLista != null)
            {
                vrhovniSudijaLista.drawContent(g, contentBounds, pageNum);
            }
            foreach (RasporedSudijaLista lista in reportListe)
            {
                lista.drawContent(g, contentBounds, pageNum);
            }
        }
    }

    public class RasporedSudijaLista : ReportLista
    {
        private Sprava sprava;
        private bool vrhovniSudija = false;
        private string ulogaVrhovniSudija;

        public RasporedSudijaLista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            SudijskiOdborNaSpravi odbor, DataGridView formGrid, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = odbor.Sprava;
            this.resizeByGrid = resizeByGrid;

            fetchItems(odbor);
        }

        public RasporedSudijaLista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            SudijaUcesnik vrhovniSudija, DataGridView formGrid, bool resizeByGrid)
                : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = Sprava.PraznaSprava1;
            this.vrhovniSudija = true;
            this.resizeByGrid = resizeByGrid;

            if (vrhovniSudija == null || vrhovniSudija.Pol == Pol.Muski)
                this.ulogaVrhovniSudija = Opcije.Instance.VrhovniSudijaMuskiString;
            else
                this.ulogaVrhovniSudija = Opcije.Instance.VrhovniSudijaZenskiString;
            string prezimeIme = String.Empty;
            string klubDrzava = String.Empty;
            if (vrhovniSudija != null)
            {
                prezimeIme = vrhovniSudija.PrezimeIme;
                klubDrzava = vrhovniSudija.KlubDrzava;
            }

            items = new List<object[]>();
            items.Add(new object[] { ulogaVrhovniSudija, prezimeIme, klubDrzava });

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        public int getFunkcijaColumnIndex()
        {
            return 0;
        }

        public int getImeColumnIndex()
        {
            return 1;
        }

        public int getKlubColumnIndex()
        {
            return 2;
        }

        private void fetchItems(SudijskiOdborNaSpravi odbor)
        {
            items = getSudijskiOdborReportItems(odbor);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getSudijskiOdborReportItems(SudijskiOdborNaSpravi odbor)
        {
            List<object[]> result = new List<object[]>();
            foreach (SudijaNaSpravi sudija in odbor.Raspored)
            {
                result.Add(new object[] { SudijskeUloge.toString(sudija.Uloga), sudija.PrezimeIme, sudija.KlubDrzava });
            }
            return result;
        }

        public override List<int> getAdjustableColumnIndexes()
        {
            List<int> result = new List<int>();
            result.Add(getFunkcijaColumnIndex());
            result.Add(getImeColumnIndex());
            result.Add(getKlubColumnIndex());
            return result;
        }

        protected override void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
         List<bool> rszByGrid /* output parameter */)
        {
            // First, create columns

            float funkcijaWidth;
            float imeWidth;
            float klubWidth;
            if (columnWidths.Count == 0)
            {
                // Prvi pass
                if (resizeByGrid)
                {
                    float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
                    float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;
                    funkcijaWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
                    imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                    klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
                }
                else
                {
                    // Resize by content
                    // Proizvoljna vrednost. Koristi se u prvom pozivu setupContent. U drugom pozivu setupContent, imeWidth,
                    // klubWidth i kategorijaWidth ce dobiti pravu vrednost, koja je dovoljna da i najduzi tekst stane bez
                    // odsecanja.
                    funkcijaWidth = 1f;
                    imeWidth = 1f;
                    klubWidth = 1f;
                }
            }
            else if (columnWidths.Count == 3)
            {
                // Drugi pass, sirine kolona su podesene
                funkcijaWidth = columnWidths[0];
                imeWidth = columnWidths[1];
                klubWidth = columnWidths[2];
            }
            else
            {
                throw new Exception("Trenutno, samo 3 kolone mogu da se podesavaju");
            }
            createColumns(g, contentBounds, funkcijaWidth, imeWidth, klubWidth);

            // Then, layout contents vertically

            itemHeight = itemFont.GetHeight(g) * 1.4f;
            if (vrhovniSudija)
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 1.4f;
            else
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float funkcijaWidth, float imeWidth,
            float klubWidth)
        {
            string imeTitle = Opcije.Instance.ImeString;
            imeWidth = getColumnWidth(g, imeWidth, imeTitle);

            string klubTitle = Opcije.Instance.KlubDrzavaString;
            klubWidth = getColumnWidth(g, klubWidth, klubTitle);

            string funkcijaTitle = Opcije.Instance.Funkcija;
            funkcijaWidth = getColumnWidth(g, funkcijaWidth, funkcijaTitle);

            float xFunkcija = contentBounds.X;
            float xIme = xFunkcija + funkcijaWidth;
            float xKlub = xIme + imeWidth;
            xRightEnd = xKlub + klubWidth;

            StringFormat funkcijaFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;

            StringFormat funkcijaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;

            Columns.Clear();

            ReportColumn column1 = addColumn(xFunkcija, funkcijaWidth, funkcijaFormat, funkcijaTitle, funkcijaHeaderFormat);
            if (!vrhovniSudija)
            {
                column1.Image = SlikeSprava.getImage(sprava);
                column1.Split = true;
                column1.Span = true;
            }

            ReportColumn column;
            column = addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            if (!vrhovniSudija)
            {
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;
            }

            column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            if (!vrhovniSudija)
            {
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
}
