using System;
using System.Drawing;
using Bilten.Domain;
using Bilten.Exceptions;
using System.Collections.Generic;
using Bilten.Data;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Bilten.Report
{
	public class UkupnoIzvestaj : Izvestaj
	{
		public UkupnoIzvestaj(IList<RezultatUkupnoExtended> rezultati, bool extended, bool kvalColumn, bool penalty,
            DataGridView formGrid, string documentName, bool stampanjeKvalifikanata, bool penalizacijaZaSprave,
            Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);

            Landscape = extended;
            if (extended)
                Margins = new Margins(40, 40, 50, 50);
            else
                Margins = new Margins(75, 75, 75, 75);

            reportListe.Add(new UkupnoLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati, takmicenje.Gimnastika,
                extended, kvalColumn, penalty, formGrid, stampanjeKvalifikanata, penalizacijaZaSprave,
                takmicenje.TakBrojevi, resizeByGrid));
		}

        // Ekipni izvestaj bez clanova ekipe
        public UkupnoIzvestaj(List<RezultatEkipno> rezultati, Gimnastika gim, bool kvalColumn, bool penalty,
            DataGridView formGrid, string documentName, Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);

            Landscape = false;
            Margins = new Margins(75, 75, 75, 75);

            reportListe.Add(new UkupnoLista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati, gim, kvalColumn, penalty,
                formGrid, resizeByGrid));
        }

        // Za stampanje viseboja po klubovima i kategorijama
        public UkupnoIzvestaj(IList<RezultatUkupno> rezultati, Gimnastika gim,
            DataGridView formGrid, string documentName, Takmicenje takmicenje, Font itemFont, bool resizeByGrid)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);

            Landscape = false;
            Margins = new Margins(75, 75, 75, 75);

            string prevKlubDrzava = String.Empty;
            List<RezultatUkupno> currList = new List<RezultatUkupno>();
            foreach (RezultatUkupno r in rezultati)
            {
                if (prevKlubDrzava == String.Empty || r.KlubDrzava == prevKlubDrzava)
                {
                    currList.Add(r);
                }
                else
                {
                    reportListe.Add(new UkupnoLista(this, 1, 0f, itemFont, itemsHeaderFont, currList,
                        gim, formGrid, takmicenje.TakBrojevi, resizeByGrid));
                    currList = new List<RezultatUkupno>();
                    currList.Add(r);
                }
                prevKlubDrzava = r.KlubDrzava;
            }
            reportListe.Add(new UkupnoLista(this, 1, 0f, itemFont, itemsHeaderFont, currList,
                gim, formGrid, takmicenje.TakBrojevi, resizeByGrid));
        }

        // TODO5: Promeni i ostale izvestaje kao sto je promenjen ovaj izvestaj
        //        - Izbaci Gimnastika iz Izvestaj konstruktora
        //        - Dodaj resizeByGrid u Izvestaj i Liste
        //        - Promeni izvestaj.doSetupContent da poziva poredjajListeUJednuKolonu
        //        - Izbaci metod drawContent ukoliko samo poziva drawContent za liste
        //        - Za izvestaje u dve kolone, iz liste izbrisi membere dveKolone i columnNumber i parametre konstruktora
        //        - Dodaj sve metode tipa lista.getImeColumnIndex()
        //        - Dodaj metod lista.getAdjustableColumnIndexes()
        //        - Dodaj metod lista.doSetupContent()
        //        - Samo jedan metod lista.createColumns() treba da postoji
        //        - Promeni lista.createColumns da uzima u obzir TOTAL_MAX_TEXT_UKUPNO itd
        //        - Naslove kolona direktno stavljaj u addColumn() pozive.
        //        - Izbaci deo "Center columns horizontally"

        // TODO5: Prebaci ovo u klasu Izvestaj. Kreiraj enum za tip izvestaja, i onda pozivaj ili poredjajListeUJednuKolonu
        // ili poredjajListeUDveKolone
        protected override void doSetupContent(Graphics g)
        {
            poredjajListeUJednuKolonu(g, contentBounds, reportListe, false);
        }
    }

	public class UkupnoLista : ReportLista
	{
        private Brush totalBrush;
        private Brush totalAllBrush;

        private bool extended;
        private bool kvalColumn;
        private bool penalty;
        private Gimnastika gimnastika;
        private bool penalizacijaZaSprave;
        private bool stampajBroj;
        private bool ekipniIzvestaj = false;
        private bool visebojPoKlubovimaIKategorijama = false;

		public UkupnoLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatUkupnoExtended> rezultati,
            Gimnastika gim, bool extended, bool kvalColumn, bool penalty, DataGridView formGrid, bool stampanjeKvalifikanata,
            bool penalizacijaZaSprave, bool stampajBroj, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
		{
            this.extended = extended;
            this.kvalColumn = kvalColumn;
            this.penalty = penalty;
            this.gimnastika = gim;
            this.penalizacijaZaSprave = penalizacijaZaSprave;
            this.stampajBroj = stampajBroj;
            this.resizeByGrid = resizeByGrid;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, gim, stampanjeKvalifikanata);
        }

        // Za stampanje ekipnih rezultata bez clanova ekipe
        public UkupnoLista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<RezultatEkipno> rezultati,
            Gimnastika gim, bool kvalColumn, bool penalty, DataGridView formGrid, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.extended = false;
            this.kvalColumn = kvalColumn;
            this.penalty = penalty;
            this.gimnastika = gim;
            this.penalizacijaZaSprave = false;
            this.stampajBroj = false;
            this.resizeByGrid = resizeByGrid;

            this.ekipniIzvestaj = true;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, gim);
        }

        // Za stampanje viseboja po klubovima i kategorijama (iz EkipeForm)
        public UkupnoLista(Izvestaj izvestaj, int pageNum, float y,
        Font itemFont, Font itemsHeaderFont, IList<RezultatUkupno> rezultati,
        Gimnastika gim, DataGridView formGrid, bool stampajBroj, bool resizeByGrid)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.extended = false;
            this.kvalColumn = false;
            this.penalty = false;
            this.gimnastika = gim;
            this.penalizacijaZaSprave = false;
            this.stampajBroj = stampajBroj;
            this.resizeByGrid = resizeByGrid;

            this.visebojPoKlubovimaIKategorijama = true;
            this.ShowHeaderForSecondListOnPage = false;

            totalBrush = Brushes.White;
            totalAllBrush = Brushes.White;

            fetchItems(rezultati, gim);
        }

        public int getImeColumnIndex()
        {
            if (ekipniIzvestaj)
                return 1;
            else
                return stampajBroj ? 2 : 1;
        }

        public int getKlubColumnIndex()
        {
            if (ekipniIzvestaj)
                return -1;  // Ekipa se prikazuje u ime koloni
            else
                return stampajBroj ? 3 : 2;
        }

        public int getKategorijaColumnIndex()
        {
            if (!visebojPoKlubovimaIKategorijama)
                return -1;
            else
                return stampajBroj ? 4 : 3;
        }
        
        private void fetchItems(IList<RezultatUkupnoExtended> rezultati,
            Gimnastika gim, bool stampanjeKvalifikanata)
		{
            items = getUkupnoReportItems(rezultati, gim, stampanjeKvalifikanata);
		
			groups = new List<ReportGrupa>();
			groups.Add(new ReportGrupa(0, items.Count));
		}

        private List<object[]> getUkupnoReportItems(IList<RezultatUkupnoExtended> rezultati,
            Gimnastika gim, bool stampanjeKvalifikanata)
        {
            List<object[]> result = new List<object[]>();
            int q = 0;
            int rezerva = 0;
            foreach (RezultatUkupnoExtended r in rezultati)
            {
                string redBroj = String.Empty;
                if (!stampanjeKvalifikanata)
                {
                    redBroj = r.Rank.ToString();
                }
                else
                {
                    if (r.KvalStatus == KvalifikacioniStatus.Q)
                    {
                        ++q;
                        redBroj = q.ToString();
                    }
                    else if (r.KvalStatus == KvalifikacioniStatus.R)
                    {
                        ++rezerva;
                        redBroj = "R" + rezerva.ToString();
                    }
                }

                // TODO5: Ne crtaj liniju izmedju "D E" i "Total B Pen" kolona

                List<object> items;
                if (gim == Gimnastika.MSG)
                {
                    items = new List<object> { redBroj, r.PrezimeIme, r.KlubDrzava,
                            r.Parter, r.Konj, r.Karike, r.Preskok, r.Razboj, r.Vratilo,
                            r.Total  };
                    if (penalty)
                    {
                        items.Insert(9, r.Penalty);
                    }
                    if (kvalColumn)
                    {
                        items.Add(KvalifikacioniStatusi.toString(r.KvalStatus));
                    }
                    if (extended)
                    {
                        items.Insert(3, r.ParterD);
                        items.Insert(4, r.ParterE);
                        items.Insert(6, r.KonjD);
                        items.Insert(7, r.KonjE);
                        items.Insert(9, r.KarikeD);
                        items.Insert(10, r.KarikeE);
                        items.Insert(12, r.PreskokD);
                        items.Insert(13, r.PreskokE);
                        items.Insert(15, r.RazbojD);
                        items.Insert(16, r.RazbojE);
                        items.Insert(18, r.VratiloD);
                        items.Insert(19, r.VratiloE);
                    }
                    if (penalizacijaZaSprave)
                    {
                        // extended je uvek true kada prikazujemo penalizaciju za sprave
                        // index se povecava za 5 zato sto imamo D, E, B, Pen, i Total za svaku spravu
                        string bon = r.ParterBon != null ? "B " + r.ParterBon.ToString() : "";
                        string pen = r.ParterPen != null ? "P " + r.ParterPen.ToString() : "";
                        items.Insert(6, bon);
                        items.Insert(7, pen);
                        bon = r.KonjBon != null ? "B " + r.KonjBon.ToString() : "";
                        pen = r.KonjPen != null ? "P " + r.KonjPen.ToString() : "";
                        items.Insert(11, bon);
                        items.Insert(12, pen);
                        bon = r.KarikeBon != null ? "B " + r.KarikeBon.ToString() : "";
                        pen = r.KarikePen != null ? "P " + r.KarikePen.ToString() : "";
                        items.Insert(16, bon);
                        items.Insert(17, pen);
                        bon = r.PreskokBon != null ? "B " + r.PreskokBon.ToString() : "";
                        pen = r.PreskokPen != null ? "P " + r.PreskokPen.ToString() : "";
                        items.Insert(21, bon);
                        items.Insert(22, pen);
                        bon = r.RazbojBon != null ? "B " + r.RazbojBon.ToString() : "";
                        pen = r.RazbojPen != null ? "P " + r.RazbojPen.ToString() : "";
                        items.Insert(26, bon);
                        items.Insert(27, pen);
                        bon = r.VratiloBon != null ? "B " + r.VratiloBon.ToString() : "";
                        pen = r.VratiloPen != null ? "P " + r.VratiloPen.ToString() : "";
                        items.Insert(31, bon);
                        items.Insert(32, pen);
                    }
                }
                else
                {
                    items = new List<object> { redBroj, r.PrezimeIme, r.KlubDrzava,
                            r.Preskok, r.DvovisinskiRazboj, r.Greda, r.Parter, 
                            r.Total };
                    if (penalty)
                    {
                        items.Insert(7, r.Penalty);
                    }
                    if (kvalColumn)
                    {
                        items.Add(KvalifikacioniStatusi.toString(r.KvalStatus));
                    }
                    if (extended)
                    {
                        items.Insert(3, r.PreskokD);
                        items.Insert(4, r.PreskokE);
                        items.Insert(6, r.DvovisinskiRazbojD);
                        items.Insert(7, r.DvovisinskiRazbojE);
                        items.Insert(9, r.GredaD);
                        items.Insert(10, r.GredaE);
                        items.Insert(12, r.ParterD);
                        items.Insert(13, r.ParterE);
                    }
                    if (penalizacijaZaSprave)
                    {
                        string bon = r.PreskokBon != null ? "B " + r.PreskokBon.ToString() : "";
                        string pen = r.PreskokPen != null ? "P " + r.PreskokPen.ToString() : "";
                        items.Insert(6, bon);
                        items.Insert(7, pen);
                        bon = r.DvovisinskiRazbojBon != null ? "B " + r.DvovisinskiRazbojBon.ToString() : "";
                        pen = r.DvovisinskiRazbojPen != null ? "P " + r.DvovisinskiRazbojPen.ToString() : "";
                        items.Insert(11, bon);
                        items.Insert(12, pen);
                        bon = r.GredaBon != null ? "B " + r.GredaBon.ToString() : "";
                        pen = r.GredaPen != null ? "P " + r.GredaPen.ToString() : "";
                        items.Insert(16, bon);
                        items.Insert(17, pen);
                        bon = r.ParterBon != null ? "B " + r.ParterBon.ToString() : "";
                        pen = r.ParterPen != null ? "P " + r.ParterPen.ToString() : "";
                        items.Insert(21, bon);
                        items.Insert(22, pen);
                    }
                }
                if (stampajBroj)
                {
                    string broj = (r.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? r.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
            }
            return result;
        }

        private void fetchItems(IList<RezultatEkipno> rezultati, Gimnastika gim)
        {
            items = getEkipnoReportItems(rezultati, gim);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getEkipnoReportItems(IList<RezultatEkipno> rezultati, Gimnastika gim)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatEkipno r in rezultati)
            {
                List<object> items;
                if (gim == Gimnastika.MSG)
                {
                    items = new List<object> { r.Rank, r.Ekipa.Naziv,
                            r.Parter, r.Konj, r.Karike, r.Preskok, r.Razboj, r.Vratilo, r.Total };
                    if (penalty)
                    {
                        items.Insert(8, r.Penalty);
                    }
                    if (kvalColumn)
                    {
                        items.Add(KvalifikacioniStatusi.toString(r.KvalStatus));
                    }
                }
                else
                {
                    items = new List<object> { r.Rank, r.Ekipa.Naziv,
                            r.Preskok, r.DvovisinskiRazboj, r.Greda, r.Parter, r.Total };
                    if (penalty)
                    {
                        items.Insert(6, r.Penalty);
                    }
                    if (kvalColumn)
                    {
                        items.Add(KvalifikacioniStatusi.toString(r.KvalStatus));
                    }
                }
                result.Add(items.ToArray());
            }
            return result;
        }

        private void fetchItems(IList<RezultatUkupno> rezultati, Gimnastika gim)
        {
            items = getUkupnoReportItems(rezultati, gim);

            groups = new List<ReportGrupa>();
            groups.Add(new ReportGrupa(0, items.Count));
        }

        private List<object[]> getUkupnoReportItems(IList<RezultatUkupno> rezultati, Gimnastika gim)
        {
            List<object[]> result = new List<object[]>();
            foreach (RezultatUkupno r in rezultati)
            {
                List<object> items;
                if (gim == Gimnastika.MSG)
                {
                    items = new List<object> { r.RedBrojIzvestaj, r.PrezimeIme, r.KlubDrzava, r.Kategorija,
                            r.Parter, r.Konj, r.Karike, r.Preskok, r.Razboj, r.Vratilo, r.Total };
                }
                else
                {
                    items = new List<object> { r.RedBrojIzvestaj, r.PrezimeIme, r.KlubDrzava, r.Kategorija,
                            r.Preskok, r.DvovisinskiRazboj, r.Greda, r.Parter, r.Total };
                }
                if (stampajBroj)
                {
                    string broj = (r.Gimnasticar.TakmicarskiBroj.HasValue)
                        ? r.Gimnasticar.TakmicarskiBroj.Value.ToString("D3") : string.Empty;
                    items.Insert(1, broj);
                }
                result.Add(items.ToArray());
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

        protected override void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
            List<bool> rszByGrid /* output parameter */)
        {
            // First, create columns

            float imeWidth;
            float klubWidth;
            float kategorijaWidth;
            if (columnWidths.Count == 0)
            {
                // Prvi pass
                if (resizeByGrid)
                {
                    float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
                    float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;
                    // TODO5: Indekse grid kolona treba menjati za sve gridove gde se dodaje kolona Broj
                    imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
                    klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;

                    // Kategorija je uvek resized by content
                    kategorijaWidth = 1f;
                    rszByGrid[2] = false;
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
                if (getKlubColumnIndex() == -1)
                {
                    klubWidth = 0f;
                }
                if (getKategorijaColumnIndex() == -1)
                {
                    kategorijaWidth = 0f;
                }
            }
            else if (columnWidths.Count == 3)
            {
                // Drugi pass, sirine kolona su podesene
                imeWidth = columnWidths[0];
                klubWidth = columnWidths[1];
                kategorijaWidth = columnWidths[2];
            }
            else
            {
                throw new Exception("Trenutno, samo 3 kolone mogu da se podesavaju");
            }
            createColumns(g, contentBounds, imeWidth, klubWidth, kategorijaWidth);

            // Then, layout contents vertically

            if (extended && penalizacijaZaSprave)
                itemHeight = itemFont.GetHeight(g) * 4.4f;
            else
                itemHeight = itemFont.GetHeight(g) * 1.4f;
            if (extended && penalizacijaZaSprave)
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 6.0f;
            else if (extended)
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            else
                itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 2.4f;
            // TODO5: Deluje da je itemsHeaderHeight reduntantno zato sto u svim izvestajima uvek ima istu vrednost kao i
            // groupHeaderHeight. Proveri da li je i u Soko programu tako.
            groupHeaderHeight = GroupHeaderVisible ? itemsHeaderHeight : 0f;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth,
            float kategorijaWidth)
        {
            if (extended && penalizacijaZaSprave)
            {
                doCreateColumnsExtendedPenalizacija(g, contentBounds, imeWidth, klubWidth);
            }
            else
            {
                doCreateColumns(g, contentBounds, imeWidth, klubWidth, kategorijaWidth);
            }
        }

        // TODO5: Svugde gde se koristi Izvestaj.convCmToInch(), ne skalira se sa promenom velicine fonta.
        
        private void doCreateColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth,
            float kategorijaWidth)
        {
            string rankTitle = Opcije.Instance.RankString;
            float rankWidth = getColumnWidth(g, RANK_MAX_TEXT, rankTitle);

            string brojTitle = Opcije.Instance.BrojString;
            float brojWidth = getColumnWidth(g, BROJ_MAX_TEXT, brojTitle);

            string imeTitle = (!ekipniIzvestaj) ? Opcije.Instance.ImeString : Opcije.Instance.EkipaString;
            imeWidth = getColumnWidth(g, imeWidth, imeTitle);

            string klubTitle = Opcije.Instance.KlubDrzavaString;
            klubWidth = getColumnWidth(g, klubWidth, klubTitle);

            string kategorijaTitle = Opcije.Instance.KategorijaString;
            kategorijaWidth = getColumnWidth(g, kategorijaWidth, kategorijaTitle);

            string penaltyTitle = Opcije.Instance.PenaltyString;
            float penaltyWidth = getColumnWidth(g, PENALTY_MAX_TEXT_UKUPNO, penaltyTitle);

            string totalTitle = Opcije.Instance.TotalString;
            float totalWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, totalTitle);

            string kvalTitle = "";
            float kvalWidth = getColumnWidth(g, QUAL_MAX_TEXT, kvalTitle);

            string spravaDTitle = Opcije.Instance.DString;
            float spravaDWidth = getColumnWidth(g, D_MAX_TEXT_UKUPNO, spravaDTitle);

            string spravaETitle = Opcije.Instance.EString;
            float spravaEWidth = getColumnWidth(g, E_MAX_TEXT_UKUPNO, spravaETitle);

            float spravaTotalWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, totalTitle);

            // Kolone za sprave nemaju tekst nego sliku
            float spravaWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, "");

            if (extended)
                spravaWidth = spravaDWidth + spravaEWidth + spravaTotalWidth;

            float xRank = contentBounds.X;
            float xBroj = 0f;
            float xIme;
            if (stampajBroj)
            {
                xBroj = xRank + rankWidth;
                xIme = xBroj + brojWidth;
            }
            else
            {
                xIme = xRank + rankWidth;
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

            float xParterE = xParter + spravaDWidth;
            float xParterTot = xParterE + spravaEWidth;
            float xKonjE = xKonj + spravaDWidth;
            float xKonjTot = xKonjE + spravaEWidth;
            float xKarikeE = xKarike + spravaDWidth;
            float xKarikeTot = xKarikeE + spravaEWidth;
            float xPreskokE = xPreskok + spravaDWidth;
            float xPreskokTot = xPreskokE + spravaEWidth;
            float xRazbojE = xRazboj + spravaDWidth;
            float xRazbojTot = xRazbojE + spravaEWidth;
            float xVratiloE = xVratilo + spravaDWidth;
            float xVratiloTot = xVratiloE + spravaEWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat kategorijaFormat = Izvestaj.nearCenterFormat;
            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat kategorijaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            if (stampajBroj)
            {
                addColumn(xBroj, brojWidth, brojFormat, brojTitle, brojHeaderFormat);
            }
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            if (getKlubColumnIndex() != -1)
            {
                addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);
            }
            if (getKategorijaColumnIndex() != -1)
            {
                addColumn(xKategorija, kategorijaWidth, kategorijaFormat, kategorijaTitle, kategorijaHeaderFormat);
            }

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            Sprava[] sprave = Sprave.getSprave(gimnastika);
            ReportColumn column;

            if (extended)
            {
                float[] x = { xParter, xParterE, xParterTot, xKonj, xKonjE, xKonjTot,
                    xKarike, xKarikeE, xKarikeTot, xPreskok, xPreskokE, xPreskokTot,
                    xRazboj, xRazbojE, xRazbojTot, xVratilo, xVratiloE, xVratiloTot };
                for (int i = 0; i < sprave.Length; i++)
                {
                    ReportColumn column1 = addColumn(x[3 * i], spravaDWidth, fmtD, spravaFormat, spravaDTitle,
                        spravaHeaderFormat);
                    column1.Image = SlikeSprava.getImage(sprave[i]);
                    column1.Split = true;
                    column1.Span = true;

                    column = addColumn(x[3 * i + 1], spravaEWidth, fmtE, spravaFormat, spravaETitle, spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprave[i]);
                    column.Split = true;

                    column = addColumn(x[3 * i + 2], spravaTotalWidth, fmtTot, spravaFormat, totalTitle, spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprave[i]);
                    column.Split = true;
                    column.Brush = totalBrush;

                    if (column1.Span)
                        column1.SpanEndColumn = column;
                }
            }
            else
            {
                float[] x = { xParter, xKonj, xKarike, xPreskok, xRazboj, xVratilo };
                for (int i = 0; i < sprave.Length; i++)
                {
                    column = addColumn(x[i], spravaWidth, fmtTot, spravaFormat, "", spravaHeaderFormat);
                    column.Image = SlikeSprava.getImage(sprave[i]);
                }
            }
            if (penalty)
            {
                addColumn(xPenalty, penaltyWidth, fmtPen, totalFormat, penaltyTitle, totalHeaderFormat);
            }
            column = addColumn(xTotal, totalWidth, fmtTot, totalFormat, totalTitle, totalHeaderFormat);
            column.Brush = totalAllBrush;
            if (kvalColumn)
            {
                column = addColumn(xKval, kvalWidth, kvalFormat, kvalTitle);
                column.DrawHeaderRect = false;
                column.DrawItemRect = false;
            }
        }

        private void doCreateColumnsExtendedPenalizacija(Graphics g, RectangleF contentBounds, float imeWidth,
            float klubWidth)
        {
            string rankTitle = Opcije.Instance.RankString;
            float rankWidth = getColumnWidth(g, RANK_MAX_TEXT, rankTitle);

            string brojTitle = Opcije.Instance.BrojString;
            float brojWidth = getColumnWidth(g, BROJ_MAX_TEXT, brojTitle);

            string imeTitle = Opcije.Instance.ImeString;
            imeWidth = getColumnWidth(g, imeWidth, imeTitle);

            string klubTitle = Opcije.Instance.KlubDrzavaString;
            klubWidth = getColumnWidth(g, klubWidth, klubTitle);

            string spravaDETitle = Opcije.Instance.DString + "\n" + Opcije.Instance.EString;
            float spravaDEWidth = getColumnWidth(g, E_MAX_TEXT_UKUPNO, spravaDETitle);

            string spravaTotalBonPenTitle = Opcije.Instance.TotalString + "\n" + Opcije.Instance.BonusString + "\n"
                + Opcije.Instance.PenaltyString;
            float spravaTotalBonPenWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, spravaTotalBonPenTitle);

            string penaltyTitle = Opcije.Instance.PenaltyString;
            float penaltyWidth = getColumnWidth(g, PENALTY_MAX_TEXT_UKUPNO, penaltyTitle);

            string totalTitle = Opcije.Instance.TotalString;
            float totalWidth = getColumnWidth(g, TOTAL_MAX_TEXT_UKUPNO, totalTitle);

            string kvalTitle = "";
            float kvalWidth = getColumnWidth(g, QUAL_MAX_TEXT, kvalTitle);

            float spravaWidth = spravaDEWidth + spravaTotalBonPenWidth;

            float xRank = contentBounds.X;
            float xBroj = 0f;
            float xIme;
            if (stampajBroj)
            {
                xBroj = xRank + rankWidth;
                xIme = xBroj + brojWidth;
            }
            else
            {
                xIme = xRank + rankWidth;
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

            float xPenalty = xTotal;
            if (penalty)
                xTotal += penaltyWidth;

            float xKval = xTotal + totalWidth;

            xRightEnd = xKval;
            if (kvalColumn)
                xRightEnd += kvalWidth;

            float xParterTot = xParter + spravaDEWidth;
            float xKonjTot = xKonj + spravaDEWidth;
            float xKarikeTot = xKarike + spravaDEWidth;
            float xPreskokTot = xPreskok + spravaDEWidth;
            float xRazbojTot = xRazboj + spravaDEWidth;
            float xVratiloTot = xVratilo + spravaDEWidth;

            StringFormat rankFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;
            StringFormat imeFormat = Izvestaj.nearCenterFormat;
            StringFormat klubFormat = Izvestaj.nearCenterFormat;
            StringFormat spravaFormat = Izvestaj.centerCenterFormat;
            StringFormat totalFormat = Izvestaj.centerCenterFormat;
            StringFormat kvalFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat spravaHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat totalHeaderFormat = Izvestaj.centerCenterFormat;

            Columns.Clear();

            addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
            if (stampajBroj)
            {
                addColumn(xBroj, brojWidth, brojFormat, brojTitle, brojHeaderFormat);
            }
            addColumn(xIme, imeWidth, imeFormat, imeTitle, imeHeaderFormat);
            ReportColumn column = addColumn(xKlub, klubWidth, klubFormat, klubTitle, klubHeaderFormat);

            string fmtD = "F" + Opcije.Instance.BrojDecimalaD;
            string fmtE = "F" + Opcije.Instance.BrojDecimalaE;
            string fmtPen = "F" + Opcije.Instance.BrojDecimalaPen;
            string fmtTot = "F" + Opcije.Instance.BrojDecimalaTotal;

            Sprava[] sprave = Sprave.getSprave(gimnastika);

            float[] x = { xParter, xParterTot, xKonj, xKonjTot, xKarike, xKarikeTot, xPreskok, xPreskokTot,
                          xRazboj, xRazbojTot, xVratilo, xVratiloTot };
            ReportColumn prevColumn = column;
            for (int i = 0; i < sprave.Length; i++)
            {
                ReportColumn column1 = addThreeRowColumn(prevColumn.getItemsIndexEnd(), 2, x[2 * i], spravaDEWidth,
                    new string[] { fmtD, fmtE }, spravaFormat, spravaDETitle, spravaHeaderFormat);

                column1.Image = SlikeSprava.getImage(sprave[i]);
                column1.Split = true;
                column1.Span = true;

                // Ne dodajem formate za bonus i penalizaciju zato sto su itemi vec formatirani (kada smo im dodali
                // prefikse B i P).
                column = addThreeRowColumn(column1.getItemsIndexEnd(), 3, x[2 * i + 1], spravaTotalBonPenWidth,
                    new string[] { fmtTot, "", "" }, spravaFormat, spravaTotalBonPenTitle, spravaHeaderFormat);
                column.Image = SlikeSprava.getImage(sprave[i]);
                column.Split = true;
                column.Brush = totalBrush;

                column1.SpanEndColumn = column;
                prevColumn = column;
            }
            column = prevColumn;
            // U sva tri sledeca poziva addColumn() potrebno je koristiti column.getItemsIndexEnd(), zato sto
            // ThreeRowColumn poremeti indekse (jer dodaje svaki put 2 ili 3 indeksa).
            if (penalty)
            {
                column = addColumn(column.getItemsIndexEnd(), xPenalty, penaltyWidth, fmtPen, totalFormat, "Pen.",
                    totalHeaderFormat);
            }
            column = addColumn(column.getItemsIndexEnd(), xTotal, totalWidth, fmtTot, totalFormat, totalTitle,
                totalHeaderFormat);
            column.Brush = totalAllBrush;
            if (kvalColumn)
            {
                column = addColumn(column.getItemsIndexEnd(), xKval, kvalWidth, kvalFormat, kvalTitle);
                column.DrawHeaderRect = false;
                column.DrawItemRect = false;
            }
        }

        private ThreeRowColumn addThreeRowColumn(int itemsIndex, int itemsSpan, float x, float width,
            string[] format, StringFormat itemRectFormat, string headerTitle, StringFormat headerFormat)
        {
            ThreeRowColumn result = new ThreeRowColumn(
                itemsIndex, itemsSpan, x, width, headerTitle);
            result.Format = format[0];
            result.Format2 = format[1];
            if (itemsSpan == 3)
            {
                result.Format3 = format[2];
            }
            result.ItemRectFormat = itemRectFormat;
            result.HeaderFormat = headerFormat;
            Columns.Add(result);
            result.Lista = this;
            return result;
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
                        float imageHeight;
                        if (extended && penalizacijaZaSprave)
                            imageHeight = (2f / 4) * columnHeaderRect.Height;
                        else
                            imageHeight = (2f / 3) * columnHeaderRect.Height;
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
                        if (col.DrawHeaderRect)
                        {
                            g.DrawRectangle(pen, columnHeaderRect.X, columnHeaderRect.Y,
                                columnHeaderRect.Width, columnHeaderRect.Height);
                        }
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

    // TODO5: Postoji nekoliko ReportColumn klasa koje mogu da prikazuju iteme u vise vrsta (2, 3 ili 5). Proveri da li
    // mozes da kreiras common klasu. Podji od ove klase jer sam jedino u njoj implementirao razlicite formate za
    // vrstu1, vrstu2 i vrstu3.
    public class ThreeRowColumn : ReportColumn
    {
        private string format2;
        public string Format2
        {
            set { format2 = value; }
        }

        private string format3;
        public string Format3
        {
            set { format3 = value; }
        }

        public ThreeRowColumn(int itemsIndex, int itemsSpan, float x, float width, string headerTitle)
            : base(itemsIndex, x, width, headerTitle)
        {
            this.itemsSpan = itemsSpan;
        }

        public override void draw(Graphics g, Pen pen, object[] itemsRow, Font itemFont, Brush blackBrush)
        {
            if (this.Brush != null)
            {
                g.FillRectangle(this.Brush, itemRect.X, itemRect.Y,
                    itemRect.Width, itemRect.Height);
            }
            if (this.DrawItemRect)
            {
                g.DrawRectangle(pen, itemRect.X, itemRect.Y,
                    itemRect.Width, itemRect.Height);
            }

            RectangleF itemRect1 = new RectangleF(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 3);
            RectangleF itemRect2 = new RectangleF(itemRect.X, itemRect.Y + itemRect.Height / 3, itemRect.Width,
                itemRect.Height / 3);

            string item1 = getFormattedString(itemsRow, itemsIndex);
            string item2 = getFormattedString(itemsRow, itemsIndex + 1, this.format2);
            g.DrawString(item1, itemFont, blackBrush, itemRect1, this.ItemRectFormat);
            g.DrawString(item2, itemFont, blackBrush, itemRect2, this.ItemRectFormat);

            if (itemsSpan == 3)
            {
                RectangleF itemRect3 = new RectangleF(itemRect.X, itemRect.Y + 2 * itemRect.Height / 3, itemRect.Width,
                    itemRect.Height / 3);
                string item3 = getFormattedString(itemsRow, itemsIndex + 2, this.format3);
                g.DrawString(item3, itemFont, blackBrush, itemRect3, this.ItemRectFormat);
            }
        }
    }
}
