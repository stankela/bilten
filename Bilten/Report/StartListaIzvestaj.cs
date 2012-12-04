using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;
using System.Drawing.Printing;

namespace Bilten.Report
{
    class StartListaIzvestaj : Izvestaj
    {
        private List<StartListaLista> reportListe = new List<StartListaLista>();
        private float itemFontSize = 9;
        private bool svakaSpravaNaPosebnojStrani;

        public StartListaIzvestaj(StartListaNaSpravi startLista, string documentName, bool stampajRedniBroj)
		{
            DocumentName = documentName;
            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            reportListe.Add(new StartListaLista(this, 1, 0f, itemFont, itemsHeaderFont, startLista, stampajRedniBroj, 
                false, 1));
		}

        public StartListaIzvestaj(List<StartListaNaSpravi> startListe, Gimnastika gim,
            string documentName, int brojSpravaPoStrani, bool stampajRedniBroj)
        {
            DocumentName = documentName;
            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);
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
                StartListaLista lista = new StartListaLista(this, page, 0f, itemFont, itemsHeaderFont,
                    startListe[i], stampajRedniBroj, sveSpraveNaJednojStrani, columnNumber);
                lista.RelY = relY;
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            lastPageNum = 0;
            foreach (StartListaLista lista in reportListe)
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
            foreach (StartListaLista lista in reportListe)
            {
                lista.drawContent(g, contentBounds, pageNum);
            }
        }
    }

    public class StartListaLista : ReportLista
    {
        private float rankWidthCm = 0.7f;
        private float imeWidthCm = 3.5f;
        private float klubWidthCm = 3.5f;

        private Sprava sprava;
        private bool stampajRedniBroj;
        bool sveSpraveNaJednojStrani;
        int columnNumber;

        public StartListaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, StartListaNaSpravi startLista, bool stampajRedniBroj,
            bool sveSpraveNaJednojStrani, int columnNumber)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, null)
        {
            this.sprava = startLista.Sprava;
            this.stampajRedniBroj = stampajRedniBroj;
            this.sveSpraveNaJednojStrani = sveSpraveNaJednojStrani;
            this.columnNumber = columnNumber;

            fetchItems(startLista);
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
                result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.KlubDrzava });
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

        private void createColumns(Graphics g, RectangleF contentBounds)
        {
            float xRank = contentBounds.X + (columnNumber - 1) * contentBounds.Width / 2;
            float xIme = xRank + Izvestaj.convCmToInch(rankWidthCm);
            float xKlub = xIme + Izvestaj.convCmToInch(imeWidthCm);
            float xRightEnd = xKlub + Izvestaj.convCmToInch(klubWidthCm);

            float rightMargin;
            if (!sveSpraveNaJednojStrani)
                rightMargin = contentBounds.Right;
            else
                rightMargin = contentBounds.Right - (2 - columnNumber) * contentBounds.Width / 2;

            if (xRightEnd < rightMargin)
            {
                float delta = (rightMargin - xRightEnd) / 2;
                xRank += delta;
                xIme += delta;
                xKlub += delta;
                xRightEnd += delta;
            }


            float rankWidth = xIme - xRank;
            float imeWidth = xKlub - xIme;
            float klubWidth = xRightEnd - xKlub;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.LineAlignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.LineAlignment = StringAlignment.Center;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = "RB";
            String imeTitle = "Ime";
            String klubTitle = "Klub";
            String kvalTitle = String.Empty;

            columns.Clear();

            ReportColumn column1 = addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
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
            foreach (ReportColumn col in columns)
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
