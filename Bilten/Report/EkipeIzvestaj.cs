using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Bilten.Util;

namespace Bilten.Report
{
    class EkipeIzvestaj : Izvestaj
    {
        public EkipeIzvestaj(List<RezultatEkipno> rezultati, IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap,
            bool penalty, bool kvalColumn, DataGridView formGrid, string documentName, Takmicenje takmicenje,
            Font itemFont, bool resizeByGrid, bool prikaziKategoriju)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            Font nazivEkipeFont = new Font(itemFont.FontFamily.Name, itemFont.Size * 10f / 8f, FontStyle.Bold);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipno))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatEkipno>(propDesc,
                ListSortDirection.Ascending));

            foreach (RezultatEkipno rez in rezultati)
            {
                List<RezultatEkipno> rezList = new List<RezultatEkipno>() { rez };
                reportListe.Add(new EkipeLista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont, nazivEkipeFont,
                    rezList, ekipaRezultatiUkupnoMap, takmicenje.Gimnastika, kvalColumn, penalty, formGrid,
                    takmicenje.TakBrojevi, resizeByGrid, prikaziKategoriju));
            }
		}

        protected override void doSetupContent(Graphics g)
        {
            poredjajListeUJednuKolonu(g, contentBounds, reportListe, false);
        }
    }

    public class EkipeLista : ReportLista
    {
        private Brush totalBrush;
        private Brush totalAllBrush;
        private Font nazivEkipeFont;

        private bool kvalColumn;
        private bool penalty;
        private Gimnastika gimnastika;
        private String totalTitle;
        private String penaltyTitle;
        private bool stampajBroj;
        private float kvalWidth;
        private bool prikaziKategoriju;

        public EkipeLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, Font nazivEkipeFont,
            List<RezultatEkipno> rezultati, IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap,
            Gimnastika gim, bool kvalColumn, bool penalty, DataGridView formGrid, bool stampajBroj, bool resizeByGrid,
            bool prikaziKategoriju)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.nazivEkipeFont = nazivEkipeFont;
            this.kvalColumn = kvalColumn;
            this.penalty = penalty;
            this.gimnastika = gim;
            this.stampajBroj = stampajBroj;
            this.resizeByGrid = resizeByGrid;
            this.prikaziKategoriju = prikaziKategoriju;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, ekipaRezultatiUkupnoMap, gim);
        }

        public int getImeColumnIndex()
        {
            return stampajBroj ? 1 : 0;
        }

        public int getKlubColumnIndex()
        {
            return stampajBroj ? 2 : 1;
        }

        public int getKategorijaColumnIndex()
        {
            if (!prikaziKategoriju)
                return -1;
            if (getKlubColumnIndex() != -1)
                return stampajBroj ? 3 : 2;
            else
                return stampajBroj ? 2 : 1;
        }

        private void fetchItems(List<RezultatEkipno> rezultatiEkipno,
            IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap, Gimnastika gim)
        {
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipno))["RedBroj"];
            rezultatiEkipno.Sort(new SortComparer<RezultatEkipno>(propDesc,
                ListSortDirection.Ascending));

            items = getEkipeReportItems(rezultatiEkipno, ekipaRezultatiUkupnoMap, gim);

            groups = new List<ReportGrupa>();
            int start = 0;
            for (int i = 0; i < rezultatiEkipno.Count; i++)
            {
                RezultatEkipno rez = rezultatiEkipno[i];
                int count = rez.Ekipa.Gimnasticari.Count;

                List<object> data;
                if (gim == Gimnastika.MSG)
                {
                    data = new List<object>() { rez.Rank, rez.Ekipa.Naziv, rez.Parter,
                        rez.Konj, rez.Karike, rez.Preskok, rez.Razboj, rez.Vratilo, 
                        rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus), rez.Penalty };
                }
                else
                {
                    data = new List<object>() { rez.Rank, rez.Ekipa.Naziv, rez.Preskok,
                        rez.DvovisinskiRazboj, rez.Greda, rez.Parter, rez.Total,
                        KvalifikacioniStatusi.toString(rez.KvalStatus), rez.Penalty };
                }
                groups.Add(new ReportGrupa(data.ToArray(), start, count));
                start += count;
            }
        }

        private List<object[]> getEkipeReportItems(List<RezultatEkipno> rezultatiEkipe,
            IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap, Gimnastika gim)
        {
            IList<RezultatUkupno> rezUkupnoSorted = getRezultatiUkupnoSorted(rezultatiEkipe, ekipaRezultatiUkupnoMap);

            List<object[]> result = new List<object[]>();
            for (int i = 0; i < rezUkupnoSorted.Count; ++i)
            {
                RezultatUkupno rez = rezUkupnoSorted[i];
                RezultatEkipno rezEkipa = getRezEkipa(i, rezultatiEkipe);

                // TODO: Proveri da li postoje situacije kada za clana ekipe umesto naziva ekipe treba da stoji naziv
                // kluba ili drzave (koji se nalazi u RezultatUkupno.KlubDrzava).

                string ekipaNaziv = rezEkipa != null ? rezEkipa.Ekipa.Naziv : rez.KlubDrzava;

                List<object> items;
                if (gim == Gimnastika.MSG)
                {
                    items = new List<object>() { rez.PrezimeIme, ekipaNaziv,
                            rez.Parter, rez.Konj, rez.Karike, rez.Preskok, rez.Razboj, rez.Vratilo, rez.Total };
                    if (penalty)
                    {
                        // Penalizacija se prikazuje u futeru ekipe, ali postoji kolona koja je prazna za clanove ekipe.
                        items.Insert(8, "");
                    }
                    // Kval status se ispisuje u hederu ekipe, i ne postoji kolona.
                }
                else
                {
                    items = new List<object>() { rez.PrezimeIme, ekipaNaziv,
                            rez.Preskok, rez.DvovisinskiRazboj, rez.Greda, rez.Parter, rez.Total };
                    if (penalty)
                    {
                        items.Insert(6, "");
                    }
                }
                if (prikaziKategoriju)
                {
                    items.Insert(2, rez.Kategorija);
                }
                if (stampajBroj)
                {
                    string broj = (rez.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? rez.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(0, broj);
                }
                result.Add(items.ToArray());
            }
            return result;
        }

        // TODO5: Jezicke opcije bi trebale da budu svojstvo takmicenja (kao sto su logoi). Kada se menjaju opcije,
        // za svaku opciju treba da postoji combo box sa svim vrednostima za tu opciju u prethodnim takmicenjima.

        private RezultatEkipno getRezEkipa(int redBrojRezUkupno, List<RezultatEkipno> rezultati)
        {
            int current = 0;
            for (int i = 0; i < rezultati.Count; ++i)
            {
                RezultatEkipno e = rezultati[i];
                current += e.Ekipa.Gimnasticari.Count;
                if (redBrojRezUkupno < current)
                  return e;
            }
            return null;
        }

        private IList<RezultatUkupno> getRezultatiUkupnoSorted(
            List<RezultatEkipno> rezultatiEkipno, IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap)
        {
            List<RezultatUkupno> result = new List<RezultatUkupno>();
            foreach (RezultatEkipno r in rezultatiEkipno)
            {
                List<RezultatUkupno> rezultati = ekipaRezultatiUkupnoMap[r.Ekipa.Id];
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(RezultatUkupno))["Total"];
                rezultati.Sort(new SortComparer<RezultatUkupno>(propDesc, ListSortDirection.Descending));
                result.AddRange(rezultati);
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

        // TODO5: Bolje ime za ovaj metod je layoutContentsVertically()
        public override void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
            List<bool> rszByGrid)
        {
            // First, create columns

            float imeWidth;
            float klubWidth;
            float kategorijaWidth;
            if (columnWidths.Count == 0)
            {
                if (resizeByGrid)
                {
                    float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
                    float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;
                    imeWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
                    klubWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                    kategorijaWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
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
            }
            else if (columnWidths.Count == 3)
            {
                // Podesene sirine kolona
                imeWidth = columnWidths[0];
                klubWidth = columnWidths[1];
                kategorijaWidth = columnWidths[2];
            }
            else
            {
                throw new Exception("Trenutno, samo 0 ili 3 kolone mogu da se podesavaju");
            }
            createColumns(g, contentBounds, imeWidth, klubWidth, kategorijaWidth);

            // Then, layout contents vertically

            itemHeight = itemFont.GetHeight(g) * 1.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 4.8f;
            groupHeaderHeight = itemsHeaderHeight;
            groupFooterHeight = itemHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, groupFooterHeight, afterGroupHeight, 0f,
                contentBounds);
        }

        // TODO5: Postoji greska kada ne prebacuje na novu stranu ekipu koja nije stala na prethodnu stranu.
        // Desava se u gimnazijadi 2025 MSG, kada se podesi item font na 16 i landscape.

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth,
            float kategorijaWidth)
        {
            float brojWidth = getColumnWidth(g, BROJ_MAX_TEXT, Opcije.Instance.BrojString);
            float spravaWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, "");
            float penaltyWidth = getColumnWidth(g, PENALTY_MAX_TEXT_UKUPNO, Opcije.Instance.PenaltyString);
            float totalWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, Opcije.Instance.TotalString);

            String kvalTitle = String.Empty;
            kvalWidth = getColumnWidth(g, QUAL_MAX_TEXT, kvalTitle);

            float xBroj = 0f;
            float xIme;
            if (stampajBroj)
            {
                xBroj = contentBounds.X;
                xIme = xBroj + brojWidth;
            }
            else
            {
                xIme = contentBounds.X;
            }
            float xKlub = xIme + imeWidth;
            float xKategorija = 0f;
            float xParter;
            if (getKategorijaColumnIndex() != -1)
            {
                xKategorija = xKlub + klubWidth;
                xParter = xKategorija + kategorijaWidth;
            }
            else
            {
                xParter = xKlub + klubWidth;
            }
            float xKonj = xParter + spravaWidth;
            float xKarike = xKonj + spravaWidth;
            float xPreskok = xKarike + spravaWidth;
            float xRazboj = xPreskok + spravaWidth;
            float xVratilo = xRazboj + spravaWidth;
            float xTotal = xVratilo + spravaWidth;
            if (gimnastika == Gimnastika.ZSG)
                xTotal = xRazboj;

            float xPenalty = xTotal;
            if (penalty)
                xTotal += penaltyWidth;

            float xKval = xTotal + totalWidth;

            xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;

            StringFormat brojFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat kategorijaFormat = Izvestaj.nearCenterFormat;
            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat kategorijaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            penaltyTitle = Opcije.Instance.PenaltyString;
            totalTitle = Opcije.Instance.TotalString;

            Columns.Clear();

            if (stampajBroj)
            {
                addColumn(xBroj, brojWidth, brojFormat, Opcije.Instance.BrojString, brojHeaderFormat);
            }
            addColumn(xIme, imeWidth, imeFormat, Opcije.Instance.ImeString, imeHeaderFormat);
            addColumn(xKlub, klubWidth, klubFormat, Opcije.Instance.KlubDrzavaString, klubHeaderFormat);
            if (getKategorijaColumnIndex() != -1)
            {
                addColumn(xKategorija, kategorijaWidth, kategorijaFormat, Opcije.Instance.KategorijaString,
                    kategorijaHeaderFormat);
            }

            float[] x = { xParter, xKonj, xKarike, xPreskok, xRazboj, xVratilo };
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            Sprava[] sprave = Sprave.getSprave(gimnastika);
            ReportColumn column;
            for (int i = 0; i < sprave.Length; i++)
            {
                column = addColumn(x[i], spravaWidth, fmtTot, spravaFormat, "", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprave[i]);
                column.Sprava = sprave[i];
            }
            if (penalty)
            {
                // Prazna kolona. Penalizacija se prikazuje u futeru.
                addColumn(xPenalty, penaltyWidth, fmtPen, totalFormat, penaltyTitle, totalHeaderFormat);
            }
            addColumn(xTotal, totalWidth, fmtTot, totalFormat, totalTitle, totalHeaderFormat);
        }

        protected override void drawGroupHeader(Graphics g, int groupId, RectangleF groupHeaderRect)
        {
            float headerHeight = groupHeaderRect.Height / 2;
            RectangleF nazivEkipeRectangle = new RectangleF(groupHeaderRect.X,
                groupHeaderRect.Y, groupHeaderRect.Width, headerHeight);

            ReportGrupa gr = groups[groupId];
            Nullable<short> rank = (Nullable<short>)gr.Data[0];
            string naziv = (string)gr.Data[1];
            int totalIndex = gimnastika == Gimnastika.ZSG ? 6 : 8;
            Nullable<float> total = (Nullable<float>)gr.Data[totalIndex];
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;
            string text = rank.ToString() + 
                (rank != null ? ".  " : "") +
                naziv + "       " + 
                (total != null ? total.Value.ToString(fmtTot) : String.Empty);

            StringFormat nazivEkipeFormat = new StringFormat();
            nazivEkipeFormat.Alignment = StringAlignment.Near;
            nazivEkipeFormat.LineAlignment = StringAlignment.Far;
            
            g.DrawString(text, nazivEkipeFont, blackBrush, nazivEkipeRectangle,
                nazivEkipeFormat);

            foreach (ReportColumn col in Columns)
            {
                RectangleF columnHeaderRect = new RectangleF(
                    col.X, groupHeaderRect.Y + headerHeight, col.Width, headerHeight);

                g.DrawRectangle(pen, columnHeaderRect.X, columnHeaderRect.Y,
                    columnHeaderRect.Width, columnHeaderRect.Height);
                if (col.Image != null)
                {
                    Izvestaj.scaleImageIsotropically(g, col.Image, columnHeaderRect);
                }
                else
                {
                    g.DrawString(col.HeaderTitle, itemsHeaderFont, blackBrush,
                        columnHeaderRect, col.HeaderFormat);
                }
                if (kvalColumn && col.HeaderTitle == totalTitle)
                {
                    // Prikazi kval status desno od Total kolone
                    RectangleF kvalRect = new RectangleF(columnHeaderRect.Right, columnHeaderRect.Y,
                        kvalWidth, columnHeaderRect.Height);
                    g.DrawString((string)gr.Data[totalIndex + 1], itemFont, blackBrush, kvalRect,
                        Izvestaj.centerCenterFormat);
                }
            }
        }

        protected override void drawGroupFooter(Graphics g, int groupId, RectangleF groupFooterRect)
        {
            ReportGrupa gr = groups[groupId];
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            int totalIndex = gimnastika == Gimnastika.ZSG ? 6 : 8;
            foreach (ReportColumn col in Columns)
            {
                RectangleF columnFooterRect = new RectangleF(
                    col.X, groupFooterRect.Y, col.Width, groupFooterRect.Height);
                if (col.Sprava != Sprava.Undefined)
                {
                    g.FillRectangle(totalBrush, columnFooterRect.X, columnFooterRect.Y,
                        columnFooterRect.Width, columnFooterRect.Height);
                    g.DrawRectangle(pen, columnFooterRect.X, columnFooterRect.Y,
                        columnFooterRect.Width, columnFooterRect.Height);
                    string text = ReportColumn.getFormattedString(gr.Data, getSpravaGroupIndex(col.Sprava), fmtTot);
                    g.DrawString(text, itemFont, blackBrush,
                        columnFooterRect, Izvestaj.centerCenterFormat);
                }
                else if (col.HeaderTitle == penaltyTitle)
                {
                    g.FillRectangle(totalBrush, columnFooterRect.X, columnFooterRect.Y,
                    columnFooterRect.Width, columnFooterRect.Height);
                    g.DrawRectangle(pen, columnFooterRect.X, columnFooterRect.Y,
                        columnFooterRect.Width, columnFooterRect.Height);

                    string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
                    string penalty = ReportColumn.getFormattedString(gr.Data, totalIndex + 2, fmtPen);
                    if (penalty != String.Empty)
                    {
                        g.DrawString(penalty, itemFont, blackBrush,
                            columnFooterRect, Izvestaj.centerCenterFormat);
                    }
                }
                else if (col.HeaderTitle == totalTitle)
                {
                    g.FillRectangle(totalBrush, columnFooterRect.X, columnFooterRect.Y,
                        columnFooterRect.Width, columnFooterRect.Height);
                    g.DrawRectangle(pen, columnFooterRect.X, columnFooterRect.Y,
                        columnFooterRect.Width, columnFooterRect.Height);

                    string text = ReportColumn.getFormattedString(gr.Data, totalIndex, fmtTot);
                    g.DrawString(text, itemFont, blackBrush,
                        columnFooterRect, Izvestaj.centerCenterFormat);
                }
            }
        }

        private int getSpravaGroupIndex(Sprava sprava)
        {
            if (gimnastika == Gimnastika.MSG)
            {
                switch (sprava)
                { 
                    case Sprava.Parter:
                        return 2;

                    case Sprava.Konj:
                        return 3;

                    case Sprava.Karike:
                        return 4;

                    case Sprava.Preskok:
                        return 5;

                    case Sprava.Razboj:
                        return 6;

                    case Sprava.Vratilo:
                        return 7;
                }
            }
            else
            {
                switch (sprava)
                {
                    case Sprava.Preskok:
                        return 2;

                    case Sprava.DvovisinskiRazboj:
                        return 3;

                    case Sprava.Greda:
                        return 4;

                    case Sprava.Parter:
                        return 5;
                }
            }
            return 0;
        }
    }
}
