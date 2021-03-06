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

        private string vrhovniSudija;
        private string vrhovniSudijaCaption;
        private bool stampajVrhovnogSudiju;

        private ReportText vrhovniSudijaReportText;
        private ReportText vrhovniSudijaCaptionReportText;

        private Font vrhovniSudijaCaptionFont;
        private Font vrhovniSudijaFont;
        
        public RasporedSudijaIzvestaj(SudijskiOdborNaSpravi odbor, string documentName, DataGridView formGrid)
        {
            DocumentName = documentName;
            this.vrhovniSudija = String.Empty;
            this.vrhovniSudijaCaption = String.Empty;
            stampajVrhovnogSudiju = false;

            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);

            vrhovniSudijaCaptionFont = new Font("Arial", 11, FontStyle.Bold);
            vrhovniSudijaFont = new Font("Arial", 11);
            
            svakaSpravaNaPosebnojStrani = true;

            reportListe.Add(new RasporedSudijaLista(this, 1, 0f, itemFont, itemsHeaderFont, odbor, false, 1, formGrid));
        }

        public RasporedSudijaIzvestaj(List<SudijskiOdborNaSpravi> odbori, Gimnastika gim,
            string documentName, int brojSpravaPoStrani, SpravaGridGroupUserControl spravaGridGroupUserControl,
            SudijaUcesnik vrhovniSudija)
        {
            DocumentName = documentName;
            if (vrhovniSudija != null)
            {
                this.vrhovniSudija = vrhovniSudija.PrezimeIme;
                if (vrhovniSudija.Pol == Pol.Muski)
                    vrhovniSudijaCaption = "Vrhovni sudija: ";
                else
                    vrhovniSudijaCaption = "Vrhovna sutkinja: ";
            }
            else
            {
                this.vrhovniSudija = String.Empty;
                if (gim == Gimnastika.MSG)
                    vrhovniSudijaCaption = "Vrhovni sudija: ";
                else
                    vrhovniSudijaCaption = "Vrhovna sutkinja: ";
            }
            stampajVrhovnogSudiju = true;

            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);

            vrhovniSudijaCaptionFont = new Font("Arial", 11, FontStyle.Bold);
            vrhovniSudijaFont = new Font("Arial", 11);

            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;
            bool sveSpraveNaJednojStrani = brojSpravaPoStrani > 3;

            if (sveSpraveNaJednojStrani)
                Margins = new Margins(50, 50, 100, 100);

            Sprava[] sprave = Sprave.getSprave(gim);
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                int page;
                float relY;
                int columnNumber = 1;
                if (sveSpraveNaJednojStrani)
                {
                    page = 1;
                    relY = (i / 2) * (1 / 3f) + 0.05f;
                    columnNumber = (i % 2 == 0) ? 1 : 2;
                }
                else if (brojSpravaPoStrani == 2 || brojSpravaPoStrani == 3)
                {
                    page = (i / brojSpravaPoStrani) + 1;
                    relY = (i % brojSpravaPoStrani) / (brojSpravaPoStrani * 1f) + 0.03f;
                }
                else // brojSpravaPoStrani == 1
                {
                    page = i + 1;
                    relY = 0.0f + 0.03f;
                }
                RasporedSudijaLista lista = new RasporedSudijaLista(this, page, 0f, itemFont, itemsHeaderFont,
                    odbori[i], sveSpraveNaJednojStrani, columnNumber,
                    spravaGridGroupUserControl[sprava].DataGridViewUserControl.DataGridView);
                lista.RelY = relY;
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            float vrhovniSudijaHeight = Math.Max(vrhovniSudijaCaptionFont.GetHeight(g),
                vrhovniSudijaFont.GetHeight(g));
            if (stampajVrhovnogSudiju)
            {
                StringFormat format = new StringFormat();
                //format.Alignment = StringAlignment.Center;
                float vrhovniSudijaCaptionWidth = g.MeasureString(vrhovniSudijaCaption, vrhovniSudijaCaptionFont).Width;
                float vrhovniSudijaWidth = g.MeasureString(vrhovniSudija, vrhovniSudijaFont).Width;
                float vrhovniSudijaTotalWidth = vrhovniSudijaCaptionWidth + vrhovniSudijaWidth;

                RectangleF vrhovniSudijaRect = new RectangleF(
                    contentBounds.X + (contentBounds.Width - vrhovniSudijaTotalWidth) / 2,
                    contentBounds.Y + 2 * vrhovniSudijaHeight, vrhovniSudijaTotalWidth, vrhovniSudijaHeight);

                vrhovniSudijaCaptionReportText = new ReportText(
                    vrhovniSudijaCaption, vrhovniSudijaCaptionFont,
                    blackBrush, vrhovniSudijaRect, format);

                vrhovniSudijaRect.X += vrhovniSudijaCaptionWidth;
                vrhovniSudijaRect.Width -= vrhovniSudijaCaptionWidth;
                vrhovniSudijaReportText = new ReportText(
                    vrhovniSudija, vrhovniSudijaFont,
                    blackBrush, vrhovniSudijaRect, format);
            }
            
            // Radim dvaput setupContent. Prvi put sluzi samo da odredim maximume kolona ime i klub u svim listama.
            lastPageNum = 0;
            float maxImeWidth = 0.0f;
            float maxKlubWidth = 0.0f;
            foreach (RasporedSudijaLista lista in reportListe)
            {
                if (svakaSpravaNaPosebnojStrani)
                    lista.FirstPageNum = lastPageNum + 1;
                lista.StartY = contentBounds.Y + 3 * vrhovniSudijaHeight + lista.RelY * (contentBounds.Height - 3 * vrhovniSudijaHeight);
                lista.setupContent(g, contentBounds);
                if (lista.Columns[1].Width > maxImeWidth)
                    maxImeWidth = lista.Columns[1].Width;
                if (lista.Columns[2].Width > maxKlubWidth)
                    maxKlubWidth = lista.Columns[2].Width;
                lastPageNum = lista.LastPageNum;
            }

            lastPageNum = 0;
            foreach (RasporedSudijaLista lista in reportListe)
            {
                if (svakaSpravaNaPosebnojStrani)
                    lista.FirstPageNum = lastPageNum + 1;
                lista.StartY = contentBounds.Y + 3 * vrhovniSudijaHeight + lista.RelY * (contentBounds.Height - 3 * vrhovniSudijaHeight);
                lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth);
                lastPageNum = lista.LastPageNum;
            }
        }

        public override void drawContent(Graphics g, int pageNum)
        {
            if (pageNum == 1 && stampajVrhovnogSudiju)
            {
                vrhovniSudijaReportText.draw(g);
                vrhovniSudijaCaptionReportText.draw(g);
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
        bool sveSpraveNaJednojStrani;
        int columnNumber;

        public RasporedSudijaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, SudijskiOdborNaSpravi odbor,
            bool sveSpraveNaJednojStrani, int columnNumber, DataGridView formGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = odbor.Sprava;
            this.sveSpraveNaJednojStrani = sveSpraveNaJednojStrani;
            this.columnNumber = columnNumber;

            fetchItems(odbor);
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

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);

            itemHeight = itemFont.GetHeight(g) * 1.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        public void setupContent(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
        {
            createColumns(g, contentBounds, imeWidth, klubWidth);

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

            float funkcijaWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
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
            if (!sveSpraveNaJednojStrani)
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
            column1.Image = SlikeSprava.getImage(sprava);
            column1.Split = true;
            column1.Span = true;

            ReportColumn column;
            column = addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

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
