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
        private List<RasporedSudijaLista> reportListe = new List<RasporedSudijaLista>();
        private float itemFontSize = 9;
        private bool svakaSpravaNaPosebnojStrani;
        private bool dveKolone;
        private RasporedSudijaLista vrhovniSudijaLista;
        
        public RasporedSudijaIzvestaj(SudijskiOdborNaSpravi odbor, string documentName, DataGridView formGrid)
        {
            DocumentName = documentName;

            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);

            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new RasporedSudijaLista(this, 1, 0f, itemFont, itemsHeaderFont, odbor, false, 1, formGrid));
        }

        // Kada je broj sprava veci od 3, stampa se u dve kolone. Kada je broj sprava 1, stampa se svaka sprava na
        // posebnoj strani. Kada je 2 ili 3 stampa se u jednoj koloni, jedno ispod drugog dok ima mesta.
        // To znaci da ako zadamo 2 a na strani ima mesta za 3 sprave, bice stampane 3 sprave.
        // TODO5: Uradi ovako i za sve ostale liste sa manjim brojem elemenata (start liste, kvalifikanti)

        public RasporedSudijaIzvestaj(List<SudijskiOdborNaSpravi> odbori, Gimnastika gim,
            string documentName, int brojSpravaPoStrani, SpravaGridGroupUserControl spravaGridGroupUserControl,
            SudijaUcesnik vrhovniSudija)
        {
            DocumentName = documentName;

            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);

            this.svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;
            this.dveKolone = brojSpravaPoStrani > 3;
            if (dveKolone)
                Margins = new Margins(50, 50, 100, 100);

            Sprava[] sprave = Sprave.getSprave(gim);
            int columnNumber = 1;
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                if (dveKolone)
                {
                    columnNumber = (i % 2 == 0) ? 1 : 2;
                }
                RasporedSudijaLista lista = new RasporedSudijaLista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont,
                    odbori[i], dveKolone, columnNumber,
                    spravaGridGroupUserControl[sprava].DataGridViewUserControl.DataGridView);
                reportListe.Add(lista);
            }
            vrhovniSudijaLista = new RasporedSudijaLista(this, 1, 0f, itemFont, itemsHeaderFont, vrhovniSudija,
                spravaGridGroupUserControl[Sprava.Parter].DataGridViewUserControl.DataGridView);
        }

        protected override void doSetupContent(Graphics g)
        {
            float afterListHeight = reportListe[0].getItemHeight(g);
            if (vrhovniSudijaLista != null)
            {
                vrhovniSudijaLista.FirstPageNum = 1;
                vrhovniSudijaLista.StartY = contentBounds.Y + 2 * afterListHeight;
                vrhovniSudijaLista.setupContent(g, contentBounds);
            }

            float startYPrvaStrana;
            float startYOstaleStrane;
            if (vrhovniSudijaLista != null)
            {
                // Svaka lista ima implicitno dodat prazan prostor nakon liste (koji je otprilike jednak afterListHeight).
                // Zato ne mnozim afterListHeight sa 2 u startYPrvaStrana.
                startYPrvaStrana = vrhovniSudijaLista.EndY + afterListHeight;
                startYOstaleStrane = contentBounds.Y + 2 * afterListHeight;
            }
            else
            {
                startYPrvaStrana = contentBounds.Y + 2 * afterListHeight;
                startYOstaleStrane = contentBounds.Y + 2 * afterListHeight;
            }
            
            if (dveKolone)
            {
                doSetupContentDveKolone(g, afterListHeight, startYPrvaStrana, startYOstaleStrane);
                return;
            }
        
            // Radim dvaput setupContent. Prvi put sluzi samo da odredim maximume kolona ime i klub u svim listama.
            float maxImeWidth = 0.0f;
            float maxKlubWidth = 0.0f;
            RasporedSudijaLista prevLista = null;
            for (int i = 0; i < 2; ++i)
            {
                prevLista = null;
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    RasporedSudijaLista lista = reportListe[j];
                    if (prevLista == null)
                    {
                        lista.FirstPageNum = 1;
                        lista.StartY = startYPrvaStrana;
                    }
                    else if (svakaSpravaNaPosebnojStrani || prebaciNaSledecuStranu)
                    {
                        lista.FirstPageNum = prevLista.LastPageNum + 1;
                        lista.StartY = startYOstaleStrane;
                        prebaciNaSledecuStranu = false;
                    }
                    else
                    {
                        // Nastavak na istoj strani
                        lista.FirstPageNum = prevLista.LastPageNum;
                        lista.StartY = prevLista.EndY + afterListHeight;
                    }

                    int firstPageNum = lista.FirstPageNum;
                    if (i == 0)
                    {
                        lista.setupContent(g, contentBounds);
                        if (lista.Columns[1].Width > maxImeWidth)
                            maxImeWidth = lista.Columns[1].Width;
                        if (lista.Columns[2].Width > maxKlubWidth)
                            maxKlubWidth = lista.Columns[2].Width;
                    }
                    else
                    {
                        lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth);
                    }

                    if (lista.LastPageNum == firstPageNum)
                    {
                        // Cela lista je stala na istu stranu
                        ++j;
                        prevLista = lista;
                    }
                    else
                    { 
                        // Lista nije stala na istu stranu pa je prebacujemo da pocinje na sledecoj strani. U ovom slucaju
                        // mozemo to da uradimo zato sto su liste kratke (krace od strane), ali u generalnom slucaju
                        // gde se izvestaj proteze na vise strana ne mozemo jer cemo uci u beskonacnu rekurziju.
                        prebaciNaSledecuStranu = true;
                    }
                }
            }
            lastPageNum = prevLista.LastPageNum;
        }

        private void doSetupContentDveKolone(Graphics g, float afterListHeight, float startYPrvaStrana,
            float startYOstaleStrane)
        {
            // Radim dvaput setupContent. Prvi put sluzi samo da odredim maximume kolona ime i klub u svim listama.
            float maxImeWidth = 0.0f;
            float maxKlubWidth = 0.0f;
            for (int i = 0; i < 2; ++i)
            {
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    RasporedSudijaLista lista = reportListe[j];
                    if (j == 0 || j == 1)
                    {
                        lista.FirstPageNum = 1;
                        lista.StartY = startYPrvaStrana;
                    }
                    else if (prebaciNaSledecuStranu)
                    {
                        lista.FirstPageNum = reportListe[j - 1].LastPageNum + 1;
                        lista.StartY = startYOstaleStrane;
                        prebaciNaSledecuStranu = false;
                    }
                    else if (j % 2 == 0)
                    {
                        lista.FirstPageNum = reportListe[j - 1].LastPageNum;
                        lista.StartY = Math.Max(reportListe[j - 1].EndY, reportListe[j - 2].EndY) + afterListHeight;
                    }
                    else if (j % 2 == 1)
                    {
                        // Kopiraj podatke iz liste sa leve strane
                        lista.FirstPageNum = reportListe[j - 1].FirstPageNum;
                        lista.StartY = reportListe[j - 1].StartY;
                    }

                    int firstPageNum = lista.FirstPageNum;
                    if (i == 0)
                    {
                        lista.setupContent(g, contentBounds);
                        if (lista.Columns[1].Width > maxImeWidth)
                            maxImeWidth = lista.Columns[1].Width;
                        if (lista.Columns[2].Width > maxKlubWidth)
                            maxKlubWidth = lista.Columns[2].Width;
                    }
                    else
                    {
                        lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth);
                    }

                    if (lista.LastPageNum == firstPageNum)
                    {
                        // Cela lista je stala na istu stranu
                        ++j;
                    }
                    else
                    {
                        prebaciNaSledecuStranu = true;
                        // Ako lista u desnoj koloni nije stala na istu stranu, moramo da se vratimo na listu u levoj
                        // koloni, i nju prebacimo na sledecu stranu.
                        if (j % 2 == 1)
                            --j;
                    }
                }
            }
            lastPageNum = reportListe[reportListe.Count - 1].LastPageNum;
        }

        public override void drawContent(Graphics g, int pageNum)
        {
            if (pageNum == 1 && vrhovniSudijaLista != null)
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
        private bool dveKolone;
        private int columnNumber;
        private bool vrhovniSudija = false;
        private string ulogaVrhovniSudija;

        public RasporedSudijaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, SudijskiOdborNaSpravi odbor,
            bool dveKolone, int columnNumber, DataGridView formGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = odbor.Sprava;
            this.dveKolone = dveKolone;
            this.columnNumber = columnNumber;

            fetchItems(odbor);
        }

        public RasporedSudijaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, SudijaUcesnik vrhovniSudija, DataGridView formGrid)
                : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = Sprava.PraznaSprava1;
            this.dveKolone = false;
            this.columnNumber = 1;
            this.vrhovniSudija = true;

            if (vrhovniSudija == null || vrhovniSudija.Pol == Pol.Muski)
                this.ulogaVrhovniSudija = "Vrhovni sudija";
            else
                this.ulogaVrhovniSudija = "Vrhovna sutkinja";
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

        public float getItemHeight(Graphics g)
        {
            return itemFont.GetHeight(g) * 1.4f;
        }

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);

            itemHeight = getItemHeight(g);
            if (vrhovniSudija)
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 1.4f;
            else
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        public void setupContent(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
        {
            createColumns(g, contentBounds, imeWidth, klubWidth);

            itemHeight = getItemHeight(g);
            if (vrhovniSudija)
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 1.4f;
            else
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

            float funkcijaWidth;
            if (vrhovniSudija)
            {
                funkcijaWidth = g.MeasureString(ulogaVrhovniSudija, itemFont).Width * 1.2f;
            }
            else
            {
                funkcijaWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
            }
            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;

            doCreateColumns(g, contentBounds, funkcijaWidth, imeWidth, klubWidth);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
        {
            float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
            float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;

            float funkcijaWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
            doCreateColumns(g, contentBounds, funkcijaWidth, imeWidth, klubWidth);
        }

        private void doCreateColumns(Graphics g, RectangleF contentBounds, float funkcijaWidth, float imeWidth, float klubWidth)
        {
            float xFunkcija = contentBounds.X + (columnNumber - 1) * contentBounds.Width / 2;
            float xIme = xFunkcija + funkcijaWidth;
            float xKlub = xIme + imeWidth;
            float xRightEnd = xKlub + klubWidth;

            float rightMargin;
            if (!dveKolone)
                rightMargin = contentBounds.Right;
            else
                rightMargin = contentBounds.Right - (2 - columnNumber) * contentBounds.Width / 2;

            // Kada imam dve kolone, razmak izmedju tabela podesavam da bude isti kao i razmak sa leve i desne strane.
            float delta;
            if (columnNumber == 1)
            {
                delta = contentBounds.X - (2 / 3.0f) * (contentBounds.X - (xRightEnd - rightMargin));  // moza da bude i negativno
                if (delta > contentBounds.X)
                    delta = contentBounds.X;
                xFunkcija -= delta;
                xIme -= delta;
                xKlub -= delta;
                xRightEnd -= delta;
            }
            else
            {
                delta = (1 / 3.0f) * (contentBounds.X - (xRightEnd - rightMargin));  // moza da bude i negativno
                if (delta < 0)
                    delta = 0.0f;
                xFunkcija += delta;
                xIme += delta;
                xKlub += delta;
                xRightEnd += delta;
            }

            StringFormat funkcijaFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.Alignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.Alignment = StringAlignment.Near;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat funkcijaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;

            String funkcijaTitle = "Funkcija";
            String imeTitle = "Ime";
            String klubTitle = "Klub/Drzava";

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
