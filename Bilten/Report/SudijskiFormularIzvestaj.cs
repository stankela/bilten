using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;
using System.Drawing.Printing;

namespace Bilten.Report
{
    class SudijskiFormularIzvestaj : Izvestaj
    {
        private List<SudijskiFormularLista> reportListe = new List<SudijskiFormularLista>();
        private float itemFontSize = 10;
        private bool svakaSpravaNaPosebnojStrani;

        public SudijskiFormularIzvestaj(StartListaNaSpravi startLista, string documentName, int brojEOcena, bool stampajRedniBroj)
		{
            DocumentName = documentName;
            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            Landscape = startLista.Sprava == Sprava.Preskok && brojEOcena > 0;
            Margins = new Margins(30, 30, 75, 75);

            SudijskiFormularLista lista = new SudijskiFormularLista(this, 1, 0f, itemFont, itemsHeaderFont, startLista,
                brojEOcena, stampajRedniBroj);
            lista.RelY = 0.0f + 0.03f;
            reportListe.Add(lista);
		}

        public SudijskiFormularIzvestaj(List<StartListaNaSpravi> startListe, Gimnastika gim,
            string documentName, int brojEOcena, int brojSpravaPoStrani, bool stampajRedniBroj)
        {
            DocumentName = documentName;
            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;

            Margins = new Margins(30, 30, 75, 75);
            
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
                    relY = 0.0f + 0.03f;
                }
                SudijskiFormularLista lista = new SudijskiFormularLista(this, page, 0f, itemFont, itemsHeaderFont,
                    startListe[i], brojEOcena, stampajRedniBroj);
                lista.RelY = relY;
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            lastPageNum = 0;
            foreach (SudijskiFormularLista lista in reportListe)
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
            foreach (SudijskiFormularLista lista in reportListe)
            {
                lista.drawContent(g, contentBounds, pageNum);
            }
        }
    }

    public class SudijskiFormularLista : ReportLista
    {
        private float rankWidthCm = 0.7f;
        private float imeWidthCm = 3.5f;
        private float klubWidthCm = 3.5f;
        private float ocenaWidthCm = 2.0f;

        private Sprava sprava;
        private int brojEOcena;
        private bool stampajRedniBroj;

        public SudijskiFormularLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, StartListaNaSpravi startLista, int brojEOcena, bool stampajRedniBroj)
            : base(izvestaj, pageNum, y, itemFont,
            itemsHeaderFont)
        {
            this.sprava = startLista.Sprava;
            this.brojEOcena = brojEOcena;
            this.stampajRedniBroj = stampajRedniBroj;

            if (sprava == Sprava.Preskok)
            {
                if (brojEOcena == 0)
                {
                    imeWidthCm = 3.0f;
                    ocenaWidthCm = 1.7f;
                }
                else
                {
                    imeWidthCm = 2.7f;
                    ocenaWidthCm = 1.4f;
                }
            }
            else
            {
                if (brojEOcena > 0)
                {
                    ocenaWidthCm = 1.9f;
                }
            }

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
                if (startLista.Sprava != Sprava.Preskok)
                {
                    if (brojEOcena == 0)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.KlubDrzava,
                            "", "", "", "",
                            "" });
                    else if (brojEOcena == 1)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 2)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 3)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 4)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 5)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 6)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "", "", "",
                            "" });
                }
                else
                {
                    if (brojEOcena == 0)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "",  "", "",
                            "" });
                    else if (brojEOcena == 1)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "", "",  "", "",
                            "" });
                    else if (brojEOcena == 2)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 3)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 4)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 5)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                            "" });
                    else if (brojEOcena == 6)
                        result.Add(new object[] { redBroj, nastup.PrezimeIme, /*nastup.KlubDrzava,*/
                            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                            "" });
                }
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
            float ocenaWidth = Izvestaj.convCmToInch(ocenaWidthCm);

            float xRank = contentBounds.X;
            float xIme = xRank + Izvestaj.convCmToInch(rankWidthCm);
            float xKlub = xIme + Izvestaj.convCmToInch(imeWidthCm);

            float xSprava;
            if (brojEOcena == 0 && sprava != Sprava.Preskok)
                xSprava = xKlub + Izvestaj.convCmToInch(klubWidthCm);
            else
                xSprava = xKlub;

            float xSprava2 = xSprava + ocenaWidth * (4 + brojEOcena);
            float xTotal = xSprava2 + ocenaWidth * (4 + brojEOcena);
            float xRightEnd;
            if (sprava != Sprava.Preskok)
                xRightEnd = xSprava + ocenaWidth * (4 + brojEOcena);
            else
                xRightEnd = xTotal + ocenaWidth;

            if (xRightEnd < contentBounds.Right)
            {
                float delta = (contentBounds.Right - xRightEnd) / 2;
                xRank += delta;
                xIme += delta;
                xKlub += delta;
                xSprava += delta;
                xSprava2 += delta;
                xTotal += delta;
                xRightEnd += delta;
            }

            float[] xEn = new float[6];
            for (int i = 0; i < brojEOcena; i++)
            {
                if (i == 0)
                    xEn[i] = xSprava + ocenaWidth;
                else
                    xEn[i] = xEn[i - 1] + ocenaWidth;
            }
            float xE;
            if (brojEOcena == 0)
                xE = xSprava + ocenaWidth;
            else
                xE = xEn[brojEOcena - 1] + ocenaWidth;
            float xPen = xE + ocenaWidth;
            float xTot = xPen + ocenaWidth;

            float[] xEn2 = new float[6];
            for (int i = 0; i < brojEOcena; i++)
            {
                if (i == 0)
                    xEn2[i] = xSprava2 + ocenaWidth;
                else
                    xEn2[i] = xEn2[i - 1] + ocenaWidth;
            }
            float xE2;
            if (brojEOcena == 0)
                xE2 = xSprava2 + ocenaWidth;
            else
                xE2 = xEn2[brojEOcena - 1] + ocenaWidth;
            float xPen2 = xE2 + ocenaWidth;
            float xTot2 = xPen2 + ocenaWidth;

            float rankWidth = xIme - xRank;
            float imeWidth = xKlub - xIme;
            float klubWidth = xSprava - xKlub;

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
            String totalTitle = "Total";
            String kvalTitle = String.Empty;

            columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            if (brojEOcena == 0 && sprava != Sprava.Preskok)
                addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            ReportColumn column1 = addColumn(xSprava, ocenaWidth, fmtD, spravaFormat, "D", spravaHeaderFormat);
            column1.Image = SlikeSprava.getImage(sprava);
            column1.Split = true;
            column1.Span = true;

            ReportColumn column;
            if (brojEOcena > 0)
            {
                for (int i = 0; i < brojEOcena; i++)
                {
                    column = addColumn(xEn[i], ocenaWidth, fmtE, spravaFormat, "E" + (i + 1).ToString(), spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprava);
                    column.Split = true;
                }
            }

            column = addColumn(xE, ocenaWidth, fmtE, spravaFormat, "E", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            column = addColumn(xPen, ocenaWidth, fmtPen, spravaFormat, "Pen.", spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            string title = "Total";
            if (sprava == Sprava.Preskok)
                title = "1. skok";
            column = addColumn(xTot, ocenaWidth, fmtTot, spravaFormat, title, spravaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;

            if (sprava == Sprava.Preskok)
            {
                column = addColumn(xSprava2, ocenaWidth, fmtD, spravaFormat, "D", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;

                if (brojEOcena > 0)
                {
                    for (int i = 0; i < brojEOcena; i++)
                    {
                        column = addColumn(xEn2[i], ocenaWidth, fmtE, spravaFormat, "E" + (i + 1).ToString(), spravaHeaderFormat);
                        column.Image = SlikeSprava.getImage(sprava);
                        column.Split = true;
                    }
                }

                column = addColumn(xE2, ocenaWidth, fmtE, spravaFormat, "E", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;

                column = addColumn(xPen2, ocenaWidth, fmtPen, spravaFormat, "Pen.", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;

                column = addColumn(xTot2, ocenaWidth, fmtTot, spravaFormat, "2. skok", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprava);
                column.Split = true;

                ReportColumn columnTot = addColumn(xTotal, ocenaWidth, fmtTot, totalFormat, totalTitle, totalHeaderFormat);
            }

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
