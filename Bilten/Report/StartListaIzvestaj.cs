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
    class StartListaIzvestaj : Izvestaj
    {
        private List<StartListaLista> reportListe = new List<StartListaLista>();
        private float itemFontSize = 9;
        private bool svakaSpravaNaPosebnojStrani;

        public StartListaIzvestaj(StartListaNaSpravi startLista, string documentName, bool stampajRedniBroj,
            bool stampajKlub, bool stampajKategoriju, DataGridView formGrid, Takmicenje takmicenje) : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;

            reportListe.Add(new StartListaLista(this, 1, 0f, itemFont, itemsHeaderFont, startLista, stampajRedniBroj, 
                stampajKlub, stampajKategoriju, false, 1, formGrid));
		}

        public StartListaIzvestaj(List<StartListaNaSpravi> startListe,
            string documentName, int brojSpravaPoStrani, bool stampajRedniBroj,
            bool stampajKlub, bool stampajKategoriju, SpravaGridGroupUserControl spravaGridGroupUserControl,
            Takmicenje takmicenje) : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemFont = new Font("Arial", itemFontSize);
            Font itemsHeaderFont = new Font("Arial", itemFontSize, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;
            // TODO4: Ova promenljiva u stvari oznacava da li stampamo u jednoj (false) ili dve (true) kolone.
            // Ceo ovaj izvestaj (tj. layout sprava na strani) treba bolje uraditi.
            bool sveSpraveNaJednojStrani = brojSpravaPoStrani > 3;

            if (sveSpraveNaJednojStrani)
                Margins = new Margins(50, 50, 100, 100);

            Sprava[] sprave = new Sprava[startListe.Count];
            for (int i = 0; i < startListe.Count; ++i)
            {
                sprave[i] = startListe[i].Sprava;
            }
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                int page;
                float relY;
                int columnNumber = 1;
                if (sveSpraveNaJednojStrani)
                {
                    page = (i / 6) + 1;
                    relY = ((i % 6) / 2) * (1 / 3f) + 0.05f;
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
                    startListe[i], stampajRedniBroj, stampajKlub, stampajKategoriju, sveSpraveNaJednojStrani, columnNumber,
                    spravaGridGroupUserControl[sprava].DataGridViewUserControl.DataGridView);
                lista.RelY = relY;
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            // Radim dvaput setupContent. Prvi put sluzi samo da odredim maximume kolona ime i klub u svim listama.
            lastPageNum = 0;
            float maxImeWidth = 0.0f;
            float maxKlubWidth = 0.0f;
            float maxKategorijaWidth = 0.0f;
            foreach (StartListaLista lista in reportListe)
            {
                if (svakaSpravaNaPosebnojStrani)
                    lista.FirstPageNum = lastPageNum + 1;
                lista.StartY = contentBounds.Y + lista.RelY * contentBounds.Height;
                lista.setupContent(g, contentBounds);
                if (lista.Columns[1].Width > maxImeWidth)
                    maxImeWidth = lista.Columns[1].Width;
                if (lista.Columns[2].Width > maxKlubWidth)
                    maxKlubWidth = lista.Columns[2].Width;
                if (lista.Columns[3].Width > maxKategorijaWidth)
                    maxKategorijaWidth = lista.Columns[3].Width;
                lastPageNum = lista.LastPageNum;
            }

            lastPageNum = 0;
            foreach (StartListaLista lista in reportListe)
            {
                if (svakaSpravaNaPosebnojStrani)
                    lista.FirstPageNum = lastPageNum + 1;
                lista.StartY = contentBounds.Y + lista.RelY * contentBounds.Height;
                lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth, maxKategorijaWidth);
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
        private Sprava sprava;
        private bool stampajRedniBroj;
        private bool stampajKlub;
        private bool stampajKategoriju;
        private bool sveSpraveNaJednojStrani;
        private int columnNumber;
        private bool praznaLista;

        public StartListaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, StartListaNaSpravi startLista, bool stampajRedniBroj,
            bool stampajKlub, bool stampajKategoriju, bool sveSpraveNaJednojStrani, int columnNumber, DataGridView formGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = startLista.Sprava;
            this.stampajRedniBroj = stampajRedniBroj;
            this.stampajKlub = stampajKlub;
            this.stampajKategoriju = stampajKategoriju;
            this.sveSpraveNaJednojStrani = sveSpraveNaJednojStrani;
            this.columnNumber = columnNumber;
            this.praznaLista = startLista.Nastupi.Count == 0;

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
                result.Add(new object[] { redBroj, nastup.PrezimeIme, nastup.KlubDrzava, nastup.Kategorija });
            }
            if (result.Count == 0)
                // hack kojim se obezbedjuje da se stampaju hederi i za liste koje su prazne
                result.Add(new object[] { "", "", "", "" });
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

        public void setupContent(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth, float kategorijaWidth)
        {
            createColumns(g, contentBounds, imeWidth, klubWidth, kategorijaWidth);

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

            float rankWidthCm = 0.7f;

            float rankWidth = Izvestaj.convCmToInch(rankWidthCm);
            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float kategorijaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;

            if (this.praznaLista)
            {
                // Kada je lista prazna, namerno biram male vrednosti da bih sprecio da velicina kolona prazne liste
                // u Form-u utice na konacnu velicinu kolona. Npr. ako imam dve prazne i dve pune liste, i ako podesim dve
                // pune liste tako da su kolone manje od kolona prazne liste, konacna velicina kolone ce ipak biti ona u 
                // praznoj listi. Ovim se to sprecava.
                rankWidth = Izvestaj.convCmToInch(rankWidthCm);
                imeWidth = Izvestaj.convCmToInch(rankWidthCm * 2);
                klubWidth = Izvestaj.convCmToInch(rankWidthCm * 2);
                kategorijaWidth = Izvestaj.convCmToInch(rankWidthCm * 2);
            }

            if (!this.stampajKlub)
                klubWidth = 0f;
            if (!this.stampajKategoriju)
                kategorijaWidth = 0f;

            doCreateColumns(g, contentBounds, rankWidth, imeWidth, klubWidth, kategorijaWidth);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth,
            float kategorijaWidth)
        {
            float rankWidthCm = 0.7f;
            float rankWidth = Izvestaj.convCmToInch(rankWidthCm);

            doCreateColumns(g, contentBounds, rankWidth, imeWidth, klubWidth, kategorijaWidth);
        }

        private void doCreateColumns(Graphics g, RectangleF contentBounds, float rankWidth, float imeWidth, float klubWidth,
            float kategorijaWidth)
        {
            float xRank = contentBounds.X + (columnNumber - 1) * contentBounds.Width / 2;
            float xIme = xRank + rankWidth;
            float xKlub = xIme + imeWidth;
            float xKategorija = xKlub + klubWidth;
            float xRightEnd = xKategorija + kategorijaWidth;

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
                xRank -= delta;
                xIme -= delta;
                xKlub -= delta;
                xKategorija -= delta;
                xRightEnd -= delta;
            }
            else
            {
                delta = (1 / 3.0f) * (contentBounds.X - (xRightEnd - rightMargin));  // moza da bude i negativno
                if (delta < 0)
                    delta = 0.0f;
                xRank += delta;
                xIme += delta;
                xKlub += delta;
                xKategorija += delta;
                xRightEnd += delta;
            }

            StringFormat rankFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.Alignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.Alignment = StringAlignment.Near;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat kategorijaFormat = new StringFormat(StringFormatFlags.NoWrap);
            kategorijaFormat.Alignment = StringAlignment.Near;
            kategorijaFormat.LineAlignment = StringAlignment.Center;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat kategorijaHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = Opcije.Instance.RedBrojString;
            String imeTitle = Opcije.Instance.ImeString;
            String klubTitle = Opcije.Instance.KlubDrzavaString;
            String kategorijaTitle = Opcije.Instance.KategorijaString;

            Columns.Clear();

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
            column.Visible = stampajKlub;

            column = addColumn(xKategorija, kategorijaWidth, kategorijaFormat, kategorijaTitle, kategorijaHeaderFormat);
            column.Image = SlikeSprava.getImage(sprava);
            column.Split = true;
            column.Visible = stampajKategoriju;

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
