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
        private List<EkipeLista> reportListe = new List<EkipeLista>();

        // TODO5: Svaka ekipa treba da je cela na istoj strani (trenutno nije tako)

        public EkipeIzvestaj(List<RezultatEkipno> rezultati, IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap,
            Gimnastika gim, bool kvalColumn, DataGridView formGrid, string documentName, Takmicenje takmicenje,
            Font itemFont)
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
                    rezList, ekipaRezultatiUkupnoMap, gim, kvalColumn, formGrid, takmicenje.TakBrojevi));
            }
		}

        protected override void doSetupContent(Graphics g)
		{
            float startYPrvaStrana = contentBounds.Y;
            float startYOstaleStrane = contentBounds.Y;

            EkipeLista prevLista = null;
            // Radim samo jedanput setupContent (limit za i je 1).
            // TODO5: Uradi dvaput setupContent, tako da sirine kolona za ime i klub budu odgovarajuce.
            for (int i = 0; i < 1; ++i)
            {
                prevLista = null;
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    EkipeLista lista = reportListe[j];
                    if (prevLista == null)
                    {
                        lista.FirstPageNum = 1;
                        lista.StartY = startYPrvaStrana;
                    }
                    else if (prebaciNaSledecuStranu)
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
                    }
                    else
                    {
                        //lista.setupContent(g, contentBounds, maxImeWidth, maxKlubWidth);
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

		public override void drawContent(Graphics g, int pageNum)
		{
            foreach (EkipeLista lista in reportListe)
            {
                lista.drawContent(g, contentBounds, pageNum);
            }
        }
    }

    public class EkipeLista : ReportLista
    {
        private Brush totalBrush;
        private Brush totalAllBrush;
        private Font nazivEkipeFont;

        private bool kvalColumn;
        private Gimnastika gimnastika;
        private String totalTitle;
        private bool stampajBroj;

        private float delta;

        public EkipeLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, Font nazivEkipeFont,
            List<RezultatEkipno> rezultati, IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap,
            Gimnastika gim, bool kvalColumn, DataGridView formGrid, bool stampajBroj)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.nazivEkipeFont = nazivEkipeFont;
            this.kvalColumn = kvalColumn;
            this.gimnastika = gim;
            this.stampajBroj = stampajBroj;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, ekipaRezultatiUkupnoMap, gim);
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

                object[] data;
                if (gim == Gimnastika.MSG)
                {
                    data = new object[] { rez.Rank, rez.Ekipa.Naziv, rez.Parter,
                        rez.Konj, rez.Karike, rez.Preskok, rez.Razboj, rez.Vratilo, 
                        rez.Total, KvalifikacioniStatusi.toString(rez.KvalStatus), rez.Penalty };
                }
                else
                {
                    data = new object[] { rez.Rank, rez.Ekipa.Naziv, rez.Preskok,
                        rez.DvovisinskiRazboj, rez.Greda, rez.Parter, rez.Total,
                        KvalifikacioniStatusi.toString(rez.KvalStatus), rez.Penalty };
                }
                groups.Add(new ReportGrupa(data, start, count));
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
                    items = new List<object>() { rez.PrezimeIme, /*rez.KlubDrzava*/ekipaNaziv,
                            rez.Parter, rez.Konj, rez.Karike, rez.Preskok, rez.Razboj, rez.Vratilo, rez.Total };
                }
                else
                {
                    items = new List<object>() { rez.PrezimeIme, /*rez.KlubDrzava*/ekipaNaziv,
                            rez.Preskok, rez.DvovisinskiRazboj, rez.Greda, rez.Parter, rez.Total };
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
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(RezultatUkupno))["PrezimeIme"];
                rezultati.Sort(new SortComparer<RezultatUkupno>(propDesc, ListSortDirection.Ascending));
                result.AddRange(rezultati);
            }
            return result;
        }

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);

            itemHeight = itemFont.GetHeight(g) * 1.4f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 4.8f;
            groupHeaderHeight = itemsHeaderHeight;
            groupFooterHeight = itemHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, groupFooterHeight, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds)
        {
            float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
            float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;

            float rankWidthCm = 0.7f;

            float brojWidth = 1.2f * Izvestaj.convCmToInch(rankWidthCm);  // TODO5: Zasto 2 ovde daje preveliku sirinu
                                                                          // a na ostalim mestima ne daje
            float imeWidth = this.formGrid.Columns[0].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float spravaWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float totalWidth = spravaWidth;
            float kvalWidth = spravaWidth / 3;

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
            float xParter = xKlub + klubWidth;
            float xKonj = xParter + spravaWidth;
            float xKarike = xKonj + spravaWidth;
            float xPreskok = xKarike + spravaWidth;
            float xRazboj = xPreskok + spravaWidth;
            float xVratilo = xRazboj + spravaWidth;
            float xTotal = xVratilo + spravaWidth;
            if (gimnastika == Gimnastika.ZSG)
                xTotal = xRazboj;

            float xKval = xTotal + totalWidth;

            float xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;

            delta = (contentBounds.Right - xRightEnd) / 2;  // moze da bude i negativno
            if (delta < -contentBounds.X)
                delta = -contentBounds.X;
            xBroj += delta;
            xIme += delta;
            xKlub += delta;
            xParter += delta;
            xKonj += delta;
            xKarike += delta;
            xPreskok += delta;
            xRazboj += delta;
            xVratilo += delta;
            xTotal += delta;
            xKval += delta;
            xRightEnd += delta;

            StringFormat brojFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.LineAlignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.LineAlignment = StringAlignment.Center;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            String brojTitle = Opcije.Instance.BrojString;
            String imeTitle = Opcije.Instance.ImeString;
            String klubTitle = Opcije.Instance.KlubDrzavaString;
            totalTitle = Opcije.Instance.TotalString;
            String kvalTitle = String.Empty;

            Columns.Clear();

            if (stampajBroj)
            {
                addColumn(xBroj, brojWidth, brojFormat, brojTitle, brojHeaderFormat);
            }
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);

            float[] x = { xParter, xKonj, xKarike, xPreskok, xRazboj, xVratilo };
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            Sprava[] sprave = Sprave.getSprave(gimnastika);
            ReportColumn column;
            for (int i = 0; i < sprave.Length; i++)
            {
                column = addColumn(x[i], spravaWidth, fmtTot, spravaFormat, "", spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprave[i]);
                column.Sprava = sprave[i];
            }

            column = addColumn(xTotal, totalWidth, fmtTot, totalFormat, totalTitle, totalHeaderFormat);

            if (kvalColumn)
            {
                column = addColumn(xKval, kvalWidth, kvalFormat, kvalTitle);
                column.DrawHeaderRect = false;
                column.DrawItemRect = false;
            }
        }

        protected override void drawGroupHeader(Graphics g, int groupId, RectangleF groupHeaderRect)
        {
            float headerHeight = groupHeaderRect.Height / 2;
            RectangleF nazivEkipeRectangle = new RectangleF(groupHeaderRect.X + delta,
                groupHeaderRect.Y, groupHeaderRect.Width, headerHeight);

            ReportGrupa gr = groups[groupId];
            Nullable<short> rank = (Nullable<short>)gr.Data[0];
            string naziv = (string)gr.Data[1];
            Nullable<float> total;
            if (gimnastika == Gimnastika.MSG)
                total = (Nullable<float>)gr.Data[8];
            else
                total = (Nullable<float>)gr.Data[6];
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
            }
        }

        protected override void drawGroupFooter(Graphics g, int groupId, RectangleF groupFooterRect)
        {
            ReportGrupa gr = groups[groupId];
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

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
                    string text = getFormattedString(gr.Data, getSpravaGroupIndex(col.Sprava), fmtTot);
                    g.DrawString(text, itemFont, blackBrush,
                        columnFooterRect, Izvestaj.centerCenterFormat);
                }
                else if (col.HeaderTitle == totalTitle)
                {
                    g.FillRectangle(totalBrush, columnFooterRect.X, columnFooterRect.Y,
                        columnFooterRect.Width, columnFooterRect.Height);
                    g.DrawRectangle(pen, columnFooterRect.X, columnFooterRect.Y,
                        columnFooterRect.Width, columnFooterRect.Height);
                    
                    int totalIndex = 8;
                    if (gimnastika == Gimnastika.ZSG)
                        totalIndex = 6;
                    string text = getFormattedString(gr.Data, totalIndex, fmtTot);
                    g.DrawString(text, itemFont, blackBrush,
                        columnFooterRect, Izvestaj.centerCenterFormat);

                    string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
                    string penalty = getFormattedString(gr.Data, totalIndex + 2, fmtPen);
                    if (penalty != String.Empty)
                    {
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Center;
                        RectangleF penaltyRect = columnFooterRect;
                        penaltyRect.Offset(columnFooterRect.Width, 0);
                        g.DrawString("Pen. " + penalty, itemFont, blackBrush,
                            penaltyRect, format);
                    }
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

        private string getFormattedString(object[] data, int index, string format)
        {
            object item = data[index];
            if (item == null)
                return String.Empty;
            else if (String.IsNullOrEmpty(format))
                return item.ToString();
            else
            {
                string fmt = "{0:" + format + "}";
                return String.Format(fmt, item);
            }
        }
    }
}
