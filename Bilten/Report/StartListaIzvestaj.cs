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
        private List<StartListaLista> reportListe = new List<StartListaLista>();
        private bool svakaSpravaNaPosebnojStrani;
        private bool dveKolone;

        public StartListaIzvestaj(StartListaNaSpravi startLista, string documentName, bool stampajRedniBroj,
            bool stampajKlub, bool stampajKategoriju, DataGridView formGrid, Takmicenje takmicenje, Font itemFont)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new StartListaLista(this, 1, 0f, itemFont, itemsHeaderFont, startLista, stampajRedniBroj, 
                stampajKlub, stampajKategoriju, false, 1, formGrid, takmicenje.TakBrojevi));
		}

        public StartListaIzvestaj(List<StartListaNaSpravi> startListe, Gimnastika gim,
            string documentName, int brojSpravaPoStrani, bool stampajRedniBroj,
            bool stampajKlub, bool stampajKategoriju, SpravaGridGroupUserControl spravaGridGroupUserControl,
            Takmicenje takmicenje, Font itemFont)
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

            Sprava[] sprave = Sprave.getSprave(gim);
            int columnNumber = 1;
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                if (dveKolone)
                {
                    columnNumber = (i % 2 == 0) ? 1 : 2;
                }

                StartListaLista lista = new StartListaLista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont,
                    startListe[i], stampajRedniBroj, stampajKlub, stampajKategoriju, dveKolone, columnNumber,
                    spravaGridGroupUserControl[sprava].DataGridViewUserControl.DataGridView, takmicenje.TakBrojevi);
                reportListe.Add(lista);
            }
        }

        protected override void doSetupContent(Graphics g)
        {
            float startYPrvaStrana = contentBounds.Y;
            float startYOstaleStrane = contentBounds.Y;

            if (dveKolone)
            {
                doSetupContentDveKolone(g, startYPrvaStrana, startYOstaleStrane);
                return;
            }
        
            // Radim dvaput setupContent. Prvi put sluzi samo da odredim maximume kolona ime i klub u svim listama.
            // TODO5: Maksimumi se odredjuju prema sirini kolona u gridovima za sprave. Odredjuj ih prema sirini teksta
            // koji se stampa (u svim izvestajima).
            float maxImeWidth = 0.0f;
            float maxKlubWidth = 0.0f;
            float maxKategorijaWidth = 0.0f;
            StartListaLista prevLista = null;
            for (int i = 0; i < 2; ++i)
            {
                prevLista = null;
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    StartListaLista lista = reportListe[j];
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
                        // Svaka lista ima implicitno dodat prazan prostor nakon liste (koji je jednak velicini vrste),
                        // i EndY pokazuje nakon tog praznog prostoja.
                        lista.StartY = prevLista.EndY;
                    }

                    int firstPageNum = lista.FirstPageNum;
                    if (i == 0)
                    {
                        lista.setupContent(g, contentBounds);
                        if (lista.Columns[lista.getImeColumnIndex()].Width > maxImeWidth)
                            maxImeWidth = lista.Columns[lista.getImeColumnIndex()].Width;
                        if (lista.Columns[lista.getKlubColumnIndex()].Width > maxKlubWidth)
                            maxKlubWidth = lista.Columns[lista.getKlubColumnIndex()].Width;
                        if (lista.Columns[lista.getKategorijaColumnIndex()].Width > maxKategorijaWidth)
                            maxKategorijaWidth = lista.Columns[lista.getKategorijaColumnIndex()].Width;
                    }
                    else
                    {
                        lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth, maxKategorijaWidth);
                    }

                    if (lista.LastPageNum == firstPageNum)
                    {
                        // Cela lista je stala na istu stranu
                        ++j;
                        prevLista = lista;
                    }
                    else
                    {
                        // Lista nije stala na istu stranu
                        float prvaStranaListHeight = contentBounds.Bottom - lista.StartY;
                        float zadnjaStranaListHeight = lista.EndY - contentBounds.Top;
                        if (prvaStranaListHeight + zadnjaStranaListHeight >= contentBounds.Height)
                        {
                            // Lista ne moze cela da stane na stranu cak i da pocnemo sa vrha strane, pa mora da ostane
                            // izlomljena (prvi deo na jednoj strani, drugi deo na drugoj strani).
                            ++j;
                            prevLista = lista;
                        }
                        else
                        {
                            // Lista nije stala na istu stranu pa je prebacujemo da pocinje na sledecoj strani.
                            prebaciNaSledecuStranu = true;
                        }
                    }
                }
            }
            lastPageNum = prevLista.LastPageNum;
        }

        private void doSetupContentDveKolone(Graphics g, float startYPrvaStrana, float startYOstaleStrane)
        {
            // Radim dvaput setupContent. Prvi put sluzi samo da odredim maximume kolona ime i klub u svim listama.
            float maxImeWidth = 0.0f;
            float maxKlubWidth = 0.0f;
            float maxKategorijaWidth = 0.0f;
            for (int i = 0; i < 2; ++i)
            {
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    StartListaLista lista = reportListe[j];
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
                        lista.StartY = Math.Max(reportListe[j - 1].EndY, reportListe[j - 2].EndY);
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
                        if (lista.Columns[lista.getImeColumnIndex()].Width > maxImeWidth)
                            maxImeWidth = lista.Columns[lista.getImeColumnIndex()].Width;
                        if (lista.Columns[lista.getKlubColumnIndex()].Width > maxKlubWidth)
                            maxKlubWidth = lista.Columns[lista.getKlubColumnIndex()].Width;
                        if (lista.Columns[lista.getKategorijaColumnIndex()].Width > maxKategorijaWidth)
                            maxKategorijaWidth = lista.Columns[lista.getKategorijaColumnIndex()].Width;
                    }
                    else
                    {
                        lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth, maxKategorijaWidth);
                    }

                    if (lista.LastPageNum == firstPageNum)
                    {
                        // Cela lista je stala na istu stranu
                        ++j;
                    }
                    else
                    {
                        // Lista nije stala na istu stranu
                        float prvaStranaListHeight = contentBounds.Bottom - lista.StartY;
                        float zadnjaStranaListHeight = lista.EndY - contentBounds.Top;
                        if (prvaStranaListHeight + zadnjaStranaListHeight >= contentBounds.Height)
                        {
                            // Lista ne moze cela da stane na stranu cak i da pocnemo sa vrha strane, pa mora da ostane
                            // izlomljena (prvi deo na jednoj strani, drugi deo na drugoj strani).

                            // TODO5: Hendluj izlomljenu listu za slucaj dve kolone (i na ostalim izvestajima)
                            throw new SmallPageSizeException();
                            //++j;
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
            }
            lastPageNum = reportListe[reportListe.Count - 1].LastPageNum;
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
        private int columnNumber;
        private bool praznaLista;
        private bool stampajBroj;
        private bool dveKolone;

        public StartListaLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, StartListaNaSpravi startLista, bool stampajRedniBroj,
            bool stampajKlub, bool stampajKategoriju, bool dveKolone, int columnNumber,
            DataGridView formGrid, bool stampajBroj)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = startLista.Sprava;
            this.stampajRedniBroj = stampajRedniBroj;
            this.stampajKlub = stampajKlub;
            this.stampajKategoriju = stampajKategoriju;
            this.dveKolone = dveKolone;
            this.columnNumber = columnNumber;
            this.praznaLista = startLista.Nastupi.Count == 0;
            this.stampajBroj = stampajBroj;

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
                List<object> items = new List<object>() { redBroj, nastup.PrezimeIme, nastup.KlubDrzava, nastup.Kategorija };
                if (stampajBroj)
                {
                    string broj = (nastup.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? nastup.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }     
                result.Add(items.ToArray());
            }
            if (result.Count == 0)
                // hack kojim se obezbedjuje da se stampaju hederi i za liste koje su prazne
                if (stampajBroj)
                    result.Add(new object[] { "", "", "", "", "" });
                else
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

            float redBrojWidthCm = 0.7f;

            float redBrojWidth = Izvestaj.convCmToInch(redBrojWidthCm);
            float brojWidth = 2 * redBrojWidth;
            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float kategorijaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;

            if (this.praznaLista)
            {
                // Kada je lista prazna, namerno biram male vrednosti da bih sprecio da velicina kolona prazne liste
                // u Form-u utice na konacnu velicinu kolona. Npr. ako imam dve prazne i dve pune liste, i ako podesim dve
                // pune liste tako da su kolone manje od kolona prazne liste, konacna velicina kolone ce ipak biti ona u 
                // praznoj listi. Ovim se to sprecava.

                // redBrojWidth je kao gore
                imeWidth =        redBrojWidth * 2;
                klubWidth =       redBrojWidth * 2;
                kategorijaWidth = redBrojWidth * 2;
            }

            if (!this.stampajKlub)
                klubWidth = 0f;
            if (!this.stampajKategoriju)
                kategorijaWidth = 0f;

            doCreateColumns(g, contentBounds, redBrojWidth, brojWidth, imeWidth, klubWidth, kategorijaWidth);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth,
            float kategorijaWidth)
        {
            float redBrojWidthCm = 0.7f;
            float redBrojWidth = Izvestaj.convCmToInch(redBrojWidthCm);
            float brojWidth = 2 * redBrojWidth;

            doCreateColumns(g, contentBounds, redBrojWidth, brojWidth, imeWidth, klubWidth, kategorijaWidth);
        }

        private void doCreateColumns(Graphics g, RectangleF contentBounds, float redBrojWidth, float brojWidth,
            float imeWidth, float klubWidth, float kategorijaWidth)
        {
            float xRedBroj = contentBounds.X + (columnNumber - 1) * contentBounds.Width / 2;
            float xBroj = 0f;
            float xIme;
            if (stampajBroj)
            {
                xBroj = xRedBroj + redBrojWidth;
                xIme = xBroj + brojWidth;
            }
            else
            {
                xIme = xRedBroj + redBrojWidth;
            }
            float xKlub = xIme + imeWidth;
            float xKategorija = xKlub + klubWidth;
            xRightEnd = xKategorija + kategorijaWidth;

            float rightMargin;
            if (!dveKolone)
                rightMargin = contentBounds.Right;
            else
                rightMargin = contentBounds.Right - (2 - columnNumber) * contentBounds.Width / 2;

            // Kada imam dve kolone, razmak izmedju tabela podesavam da bude isti kao i razmak sa leve i desne strane.
            float delta;
            if (columnNumber == 1)
            {
                delta = contentBounds.X - (2 / 3.0f) * (contentBounds.X - (xRightEnd - rightMargin));  // moze da bude i negativno
                if (delta > contentBounds.X)
                    delta = contentBounds.X;
                xRedBroj -= delta;
                xBroj -= delta;
                xIme -= delta;
                xKlub -= delta;
                xKategorija -= delta;
                xRightEnd -= delta;
            }
            else
            {
                delta = (1 / 3.0f) * (contentBounds.X - (xRightEnd - rightMargin));  // moze da bude i negativno
                if (delta < 0)
                    delta = 0.0f;
                xRedBroj += delta;
                xBroj += delta;
                xIme += delta;
                xKlub += delta;
                xKategorija += delta;
                xRightEnd += delta;
            }

            StringFormat redBrojFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.Alignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.Alignment = StringAlignment.Near;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat kategorijaFormat = new StringFormat(StringFormatFlags.NoWrap);
            kategorijaFormat.Alignment = StringAlignment.Near;
            kategorijaFormat.LineAlignment = StringAlignment.Center;

            StringFormat redBrojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat kategorijaHeaderFormat = Izvestaj.centerCenterFormat;

            String redBrojTitle = Opcije.Instance.RedBrojString;
            String brojTitle = Opcije.Instance.BrojString;
            String imeTitle = Opcije.Instance.ImeString;
            String klubTitle = Opcije.Instance.KlubDrzavaString;
            String kategorijaTitle = Opcije.Instance.KategorijaString;

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

        public int getImeColumnIndex()
        {
            return stampajBroj ? 2 : 1;
        }

        public int getKlubColumnIndex()
        {
            return stampajBroj ? 3 : 2;
        }

        public int getKategorijaColumnIndex()
        {
            return stampajBroj ? 4 : 3;
        }
    }
}
