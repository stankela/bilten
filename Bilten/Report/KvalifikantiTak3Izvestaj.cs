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
    class KvalifikantiTak3Izvestaj : Izvestaj
    {
        private bool svakaSpravaNaPosebnojStrani;
        private bool dveKolone;

        public KvalifikantiTak3Izvestaj(List<RezultatSprava> rezultati, Sprava sprava, string documentName,
            DataGridView formGrid, Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new KvalifikantiTak3Lista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati, sprava, formGrid,
                takmicenje.TakBrojevi, resizeByGrid));
		}

        public KvalifikantiTak3Izvestaj(List<RezultatPreskok> rezultati, bool obaPreskoka, string documentName,
            DataGridView formGrid, Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new KvalifikantiTak3Lista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati, obaPreskoka,
                formGrid, takmicenje.TakBrojevi, resizeByGrid));
        }

        public KvalifikantiTak3Izvestaj(List<List<RezultatSprava>> rezultatiSprave, List<RezultatPreskok> rezultatiPreskok,
            bool obaPreskoka, string documentName, int brojSpravaPoStrani, DataGridView formGrid, Takmicenje takmicenje,
            Font itemFont, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;
            this.dveKolone = brojSpravaPoStrani > 3;
            if (dveKolone)
            {
                Margins = new Margins(50, 50, 25, 25);
            }

            // TODO5: Neka pocetne vrednosti za margine u svim izvestajima budu 0 (ili neka mala vrednost). Svi izvestaji
            // su svakako centrirani po sredini stranice.

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                KvalifikantiTak3Lista lista;
                if (sprava != Sprava.Preskok)
                {
                    int spravaIndex = i;
                    if (i > Sprave.indexOf(Sprava.Preskok, takmicenje.Gimnastika))
                        spravaIndex--;
                    lista = new KvalifikantiTak3Lista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont,
                        rezultatiSprave[spravaIndex], sprava, formGrid, takmicenje.TakBrojevi,
                        resizeByGrid);
                }
                else
                {
                    lista = new KvalifikantiTak3Lista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont,
                        rezultatiPreskok, obaPreskoka, formGrid, takmicenje.TakBrojevi,
                        resizeByGrid);
                }
                reportListe.Add(lista);
            }
        }

        public KvalifikantiTak3Izvestaj(IList<UcesnikTakmicenja3> kvalifikanti, IList<UcesnikTakmicenja3> rezerve,
            Sprava sprava, string documentName, DataGridView formGrid, Takmicenje takmicenje, Font itemFont,
            bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new KvalifikantiTak3Lista(this, 1, 0f, itemFont, itemsHeaderFont, kvalifikanti, rezerve,
                sprava, formGrid, takmicenje.TakBrojevi, resizeByGrid));
        }

        public KvalifikantiTak3Izvestaj(List<IList<UcesnikTakmicenja3>> kvalifikanti,
            List<IList<UcesnikTakmicenja3>> rezerve, string documentName, int brojSpravaPoStrani, DataGridView formGrid,
            Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = brojSpravaPoStrani == 1;
            this.dveKolone = brojSpravaPoStrani > 3;
            if (dveKolone)
            {
                Margins = new Margins(50, 50, 25, 25);
            }

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                KvalifikantiTak3Lista lista = new KvalifikantiTak3Lista(this, 1/*FirstPageNum*/, 0f, itemFont,
                    itemsHeaderFont, kvalifikanti[i], rezerve[i], sprava, formGrid, takmicenje.TakBrojevi, resizeByGrid);
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

    public class KvalifikantiTak3Lista : ReportLista
    {
        private Sprava sprava;
        private bool praznaLista;
        private bool stampajBroj;

        public KvalifikantiTak3Lista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            List<RezultatSprava> rezultati, Sprava sprava, DataGridView formGrid,
            bool stampajBroj, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = sprava;
            this.praznaLista = rezultati.Count == 0;
            this.stampajBroj = stampajBroj;
            this.resizeByGrid = resizeByGrid;

            fetchItems(rezultati);
        }

        public KvalifikantiTak3Lista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            List<RezultatPreskok> rezultati, bool obaPreskoka, DataGridView formGrid,
            bool stampajBroj, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = Sprava.Preskok;
            this.praznaLista = rezultati.Count == 0;
            this.stampajBroj = stampajBroj;
            this.resizeByGrid = resizeByGrid;

            fetchItems(rezultati, obaPreskoka);
        }

        public KvalifikantiTak3Lista(Izvestaj izvestaj, int pageNum, float y, Font itemFont, Font itemsHeaderFont,
            IList<UcesnikTakmicenja3> kvalifikanti, IList<UcesnikTakmicenja3> rezerve, Sprava sprava, DataGridView formGrid,
            bool stampajBroj, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = sprava;
            this.praznaLista = kvalifikanti.Count == 0 && rezerve.Count == 0;
            this.stampajBroj = stampajBroj;
            this.resizeByGrid = resizeByGrid;

            fetchItems(kvalifikanti, rezerve);
        }

        public int getImeColumnIndex()
        {
            return stampajBroj ? 2 : 1;
        }

        public int getKlubColumnIndex()
        {
            return stampajBroj ? 3 : 2;
        }

        private void fetchItems(List<RezultatSprava> rezultati)
        {
            items = getStartListaReportItems(rezultati);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private void fetchItems(List<RezultatPreskok> rezultati, bool obaPreskoka)
        {
            items = getStartListaReportItems(rezultati, obaPreskoka);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private void fetchItems(IList<UcesnikTakmicenja3> kvalifikanti, IList<UcesnikTakmicenja3> rezerve)
        {
            items = getStartListaReportItems(kvalifikanti, rezerve);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getStartListaReportItems(List<RezultatSprava> rezultati)
        {
            List<object[]> result = new List<object[]>();
            int q = 0;
            int r = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                string redBroj = String.Empty;
                RezultatSprava rezultat = rezultati[i];
                if (rezultat.KvalStatus == KvalifikacioniStatus.Q)
                {
                    ++q;
                    redBroj = q.ToString();
                }
                else if (rezultat.KvalStatus == KvalifikacioniStatus.R)
                {
                    ++r;
                    if (r == 1)
                    {
                        if (stampajBroj)
                            result.Add(new object[] { "", "", Opcije.Instance.RezerveString, "", "" });
                        else
                            result.Add(new object[] { "", Opcije.Instance.RezerveString, "", "" });
                    }
                    redBroj = "R" + r.ToString();
                }
                List<object> items = new List<object>() { redBroj, rezultat.Gimnasticar.PrezimeIme,
                    rezultat.Gimnasticar.KlubDrzava, rezultat.Total};
                if (stampajBroj)
                {
                    string broj = (rezultat.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? rezultat.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
            }
            if (result.Count == 0)
            {
                // hack kojim se obezbedjuje da se stampaju hederi i za liste koje su prazne
                if (stampajBroj)
                    result.Add(new object[] { "", "", "", "", "" });
                else
                    result.Add(new object[] { "", "", "", "" });
            }
            return result;
        }

        private List<object[]> getStartListaReportItems(List<RezultatPreskok> rezultati, bool obaPreskoka)
        {
            List<object[]> result = new List<object[]>();
            int q = 0;
            int r = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                string redBroj = String.Empty;
                RezultatPreskok rezultat = rezultati[i];
                if (rezultat.KvalStatus == KvalifikacioniStatus.Q)
                {
                    ++q;
                    redBroj = q.ToString();
                }
                else if (rezultat.KvalStatus == KvalifikacioniStatus.R)
                {
                    ++r;
                    if (r == 1)
                    {
                        if (stampajBroj)
                            result.Add(new object[] { "", "", Opcije.Instance.RezerveString, "", "" });
                        else
                            result.Add(new object[] { "", Opcije.Instance.RezerveString, "", "" });
                    }
                    redBroj = "R" + r.ToString();
                }
                List<object> items = new List<object>() { redBroj, rezultat.Gimnasticar.PrezimeIme,
                    rezultat.Gimnasticar.KlubDrzava, obaPreskoka ? rezultat.TotalObeOcene : rezultat.Total };
                if (stampajBroj)
                {
                    string broj = (rezultat.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? rezultat.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
            }
            if (result.Count == 0)
            {
                // hack kojim se obezbedjuje da se stampaju hederi i za liste koje su prazne
                if (stampajBroj)
                    result.Add(new object[] { "", "", "", "", "" });
                else
                    result.Add(new object[] { "", "", "", "" });
            }
            return result;
        }

        private List<object[]> getStartListaReportItems(IList<UcesnikTakmicenja3> kvalifikanti,
            IList<UcesnikTakmicenja3> rezerve)
        {
            IList<UcesnikTakmicenja3> ucesnici = new List<UcesnikTakmicenja3>();
            foreach (UcesnikTakmicenja3 u in kvalifikanti)
                ucesnici.Add(u);
            foreach (UcesnikTakmicenja3 u in rezerve)
                ucesnici.Add(u);
            List<object[]> result = new List<object[]>();
            int r = 0;
            for (int i = 0; i < ucesnici.Count; i++)
            {
                UcesnikTakmicenja3 rezultat = ucesnici[i];
                string redBroj = rezultat.QualOrder.ToString();
                if (rezultat.KvalStatus == KvalifikacioniStatus.R)
                {
                    ++r;
                    if (r == 1)
                    {
                        if (stampajBroj)
                            result.Add(new object[] { "", "", Opcije.Instance.RezerveString, "", "" });
                        else
                            result.Add(new object[] { "", Opcije.Instance.RezerveString, "", "" });
                    }
                    redBroj = "R" + r.ToString();
                }
                List<object> items = new List<object>() { redBroj, rezultat.Gimnasticar.PrezimeIme,
                    rezultat.Gimnasticar.KlubDrzava, rezultat.QualScore};
                if (stampajBroj)
                {
                    string broj = (rezultat.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? rezultat.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
            }
            if (result.Count == 0)
            {
                // hack kojim se obezbedjuje da se stampaju hederi i za liste koje su prazne
                if (stampajBroj)
                    result.Add(new object[] { "", "", "", "", "" });
                else
                    result.Add(new object[] { "", "", "", "" });
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
            List<bool> rszByGrid /* output parameter */)
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
                    // Proizvoljna vrednost. Koristi se u prvom pozivu setupContent. U drugom pozivu setupContent, imeWidth,
                    // klubWidth i kategorijaWidth ce dobiti pravu vrednost, koja je dovoljna da i najduzi tekst stane bez
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

            itemHeight = itemFont.GetHeight(g) * 1.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
        {
            string redBrojTitle = Opcije.Instance.RedBrojString;
            float redBrojWidth = getColumnWidth(g, REDNI_BROJ_MAX_TEXT, redBrojTitle);

            string brojTitle = Opcije.Instance.BrojString;
            float brojWidth = getColumnWidth(g, BROJ_MAX_TEXT, brojTitle);

            string imeTitle = Opcije.Instance.ImeString;
            imeWidth = getColumnWidth(g, imeWidth, imeTitle);

            string klubTitle = Opcije.Instance.KlubDrzavaString;
            klubWidth = getColumnWidth(g, klubWidth, klubTitle);

            string ocenaTitle = Opcije.Instance.OcenaString;
            float ocenaWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, ocenaTitle);

            float xRedBroj = contentBounds.X;
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
            float xOcena = xKlub + klubWidth;
            xRightEnd = xOcena + ocenaWidth;

            StringFormat redBrojFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat ocenaFormat = Izvestaj.centerCenterFormat;

            StringFormat redBrojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat ocenaHeaderFormat = Izvestaj.centerCenterFormat;

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

            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            column = addColumn(xOcena, ocenaWidth, fmtTot, ocenaFormat, ocenaTitle, ocenaHeaderFormat);
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
