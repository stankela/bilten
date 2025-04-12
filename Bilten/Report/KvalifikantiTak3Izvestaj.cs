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
        private List<KvalifikantiTak3Lista> reportListe = new List<KvalifikantiTak3Lista>();
        private bool svakaSpravaNaPosebnojStrani;
        private bool dveKolone;

        public KvalifikantiTak3Izvestaj(List<RezultatSprava> rezultati, Sprava sprava, string documentName,
            DataGridView formGrid, Takmicenje takmicenje, Font itemFont)
            : base(takmicenje)
		{
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new KvalifikantiTak3Lista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati, sprava,
                false, 1, formGrid, takmicenje.TakBrojevi));
		}

        public KvalifikantiTak3Izvestaj(List<RezultatPreskok> rezultati, bool obaPreskoka, string documentName,
            DataGridView formGrid, Takmicenje takmicenje, Font itemFont)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new KvalifikantiTak3Lista(this, 1, 0f, itemFont, itemsHeaderFont, rezultati, obaPreskoka,
                false, 1, formGrid, takmicenje.TakBrojevi));
        }

        public KvalifikantiTak3Izvestaj(List<List<RezultatSprava>> rezultatiSprave,
            List<RezultatPreskok> rezultatiPreskok, bool obaPreskoka, Gimnastika gim,
            string documentName, int brojSpravaPoStrani, DataGridView formGrid, Takmicenje takmicenje, Font itemFont)
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

            Sprava[] sprave = Sprave.getSprave(gim);
            int columnNumber = 1;
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                if (dveKolone)
                {
                    columnNumber = (i % 2 == 0) ? 1 : 2;
                }

                KvalifikantiTak3Lista lista;
                if (sprava != Sprava.Preskok)
                {
                    int spravaIndex = i;
                    if (i > Sprave.indexOf(Sprava.Preskok, gim))
                        spravaIndex--;
                    lista = new KvalifikantiTak3Lista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont,
                        rezultatiSprave[spravaIndex], sprava, dveKolone, columnNumber, formGrid, takmicenje.TakBrojevi);
                }
                else
                {
                    lista = new KvalifikantiTak3Lista(this, 1/*FirstPageNum*/, 0f, itemFont, itemsHeaderFont,
                        rezultatiPreskok, obaPreskoka, dveKolone, columnNumber, formGrid, takmicenje.TakBrojevi);
                }
                reportListe.Add(lista);
            }
        }

        public KvalifikantiTak3Izvestaj(IList<UcesnikTakmicenja3> kvalifikanti, IList<UcesnikTakmicenja3> rezerve,
        Sprava sprava, string documentName, DataGridView formGrid, Takmicenje takmicenje, Font itemFont)
            : base(takmicenje)
        {
            DocumentName = documentName;
            Font itemsHeaderFont = new Font(itemFont.FontFamily.Name, itemFont.Size, FontStyle.Bold);
            svakaSpravaNaPosebnojStrani = true;
            dveKolone = false;

            reportListe.Add(new KvalifikantiTak3Lista(this, 1, 0f, itemFont, itemsHeaderFont, kvalifikanti, rezerve,
                sprava, false, 1, formGrid, takmicenje.TakBrojevi));
        }

        public KvalifikantiTak3Izvestaj(List<IList<UcesnikTakmicenja3>> kvalifikanti,
            List<IList<UcesnikTakmicenja3>> rezerve, Gimnastika gim,
            string documentName, int brojSpravaPoStrani, DataGridView formGrid, Takmicenje takmicenje, Font itemFont)
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

            Sprava[] sprave = Sprave.getSprave(gim);
            int columnNumber = 1;
            for (int i = 0; i < sprave.Length; i++)
            {
                Sprava sprava = sprave[i];
                if (dveKolone)
                {
                    columnNumber = (i % 2 == 0) ? 1 : 2;
                }
                KvalifikantiTak3Lista lista = new KvalifikantiTak3Lista(this, 1/*FirstPageNum*/, 0f, itemFont,
                    itemsHeaderFont, kvalifikanti[i], rezerve[i], sprava, dveKolone, columnNumber, formGrid,
                    takmicenje.TakBrojevi);
                reportListe.Add(lista);
            }
        }

        // TODO5: Metodi doSetupContent i doSetupContentDveKolone su skoro identicni u nekoliko izvestaja (
        // RasporedSudijaIzvestaj, SpravaIzvestaj, StartListaIzvestaj, KvalifikantiTak3Izvestaj). Probaj da generalizujes.

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
            float maxImeWidth = 0.0f;
            float maxKlubWidth = 0.0f;
            KvalifikantiTak3Lista prevLista = null;
            for (int i = 0; i < 2; ++i)
            {
                prevLista = null;
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    KvalifikantiTak3Lista lista = reportListe[j];
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
            for (int i = 0; i < 2; ++i)
            {
                int j = 0;
                bool prebaciNaSledecuStranu = false;
                while (j < reportListe.Count)
                {
                    KvalifikantiTak3Lista lista = reportListe[j];
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
                        // Lista nije stala na istu stranu
                        float prvaStranaListHeight = contentBounds.Bottom - lista.StartY;
                        float zadnjaStranaListHeight = lista.EndY - contentBounds.Top;
                        if (prvaStranaListHeight + zadnjaStranaListHeight >= contentBounds.Height)
                        {
                            // Lista ne moze cela da stane na stranu cak i da pocnemo sa vrha strane, pa mora da ostane
                            // izlomljena (prvi deo na jednoj strani, drugi deo na drugoj strani).

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
            foreach (KvalifikantiTak3Lista lista in reportListe)
            {
                lista.drawContent(g, contentBounds, pageNum);
            }
        }
    }

    public class KvalifikantiTak3Lista : ReportLista
    {
        private Sprava sprava;
        private bool dveKolone;
        private int columnNumber;
        private bool praznaLista;
        private bool stampajBroj;

        public KvalifikantiTak3Lista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, List<RezultatSprava> rezultati, Sprava sprava,
            bool dveKolone, int columnNumber, DataGridView formGrid, bool stampajBroj)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = sprava;
            this.dveKolone = dveKolone;
            this.columnNumber = columnNumber;
            this.praznaLista = rezultati.Count == 0;
            this.stampajBroj = stampajBroj;

            fetchItems(rezultati);
        }

        public KvalifikantiTak3Lista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, List<RezultatPreskok> rezultati, bool obaPreskoka,
            bool dveKolone, int columnNumber, DataGridView formGrid, bool stampajBroj)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = Sprava.Preskok;
            this.dveKolone = dveKolone;
            this.columnNumber = columnNumber;
            this.praznaLista = rezultati.Count == 0;
            this.stampajBroj = stampajBroj;

            fetchItems(rezultati, obaPreskoka);
        }

        public KvalifikantiTak3Lista(Izvestaj izvestaj, int pageNum, float y,
            Font itemFont, Font itemsHeaderFont, IList<UcesnikTakmicenja3> kvalifikanti, IList<UcesnikTakmicenja3> rezerve,
            Sprava sprava, bool dveKolone, int columnNumber, DataGridView formGrid, bool stampajBroj)
            : base(izvestaj, pageNum, y, itemFont, itemsHeaderFont, formGrid)
        {
            this.sprava = sprava;
            this.dveKolone = dveKolone;
            this.columnNumber = columnNumber;
            this.praznaLista = kvalifikanti.Count == 0 && rezerve.Count == 0;
            this.stampajBroj = stampajBroj;

            fetchItems(kvalifikanti, rezerve);
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

        public void setupContent(Graphics g, RectangleF contentBounds)
        {
            createColumns(g, contentBounds);

            itemHeight = itemFont.GetHeight(g) * 1.3f;
            itemsHeaderHeight = itemsHeaderFont.GetHeight(g) * 3.6f;
            groupHeaderHeight = itemsHeaderHeight;
            float afterGroupHeight = itemHeight;

            createListLayout(groupHeaderHeight, itemHeight, 0f, afterGroupHeight, 0f,
                contentBounds);
        }

        public void setupContent(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
        {
            createColumns(g, contentBounds, imeWidth, klubWidth);

            itemHeight = itemFont.GetHeight(g) * 1.3f;
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
            float brojWidth = 2 * rankWidth;
            float imeWidth = this.formGrid.Columns[1].Width * printWidth / gridWidth;
            float klubWidth = this.formGrid.Columns[2].Width * printWidth / gridWidth;
            float ocenaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;
            if (this.praznaLista)
            {
                // Kada je lista prazna, namerno biram male vrednosti da bih sprecio da velicina kolona prazne liste
                // u Form-u utice na konacnu velicinu kolona. Npr. ako imam dve prazne i dve pune liste, i ako podesim dve
                // pune liste tako da su kolone manje od kolona prazne liste, konacna velicina kolone ce ipak biti ona u 
                // praznoj listi. Ovim se to sprecava.

                // rankWidth je kao gore
                imeWidth = rankWidth * 2;
                klubWidth = rankWidth * 2;
                ocenaWidth = rankWidth * 2;
            }

            doCreateColumns(g, contentBounds, rankWidth, brojWidth, imeWidth, klubWidth, ocenaWidth);
        }

        private void createColumns(Graphics g, RectangleF contentBounds, float imeWidth, float klubWidth)
        {
            float gridWidth = getGridTextWidth(this.formGrid, TEST_TEXT);
            float printWidth = g.MeasureString(TEST_TEXT, itemFont).Width;

            float rankWidthCm = 0.7f;
            float rankWidth = Izvestaj.convCmToInch(rankWidthCm);
            float brojWidth = 2 * rankWidth;
            float ocenaWidth = this.formGrid.Columns[3].Width * printWidth / gridWidth;

            doCreateColumns(g, contentBounds, rankWidth, brojWidth, imeWidth, klubWidth, ocenaWidth);
        }

        private void doCreateColumns(Graphics g, RectangleF contentBounds, float rankWidth, float brojWidth, float imeWidth,
            float klubWidth, float ocenaWidth)
        {
            float xRank = contentBounds.X + (columnNumber - 1) * contentBounds.Width / 2;
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
            float xOcena = xKlub + klubWidth;
            xRightEnd = xOcena + ocenaWidth;

            float rightMargin;
            if (!dveKolone)
                rightMargin = contentBounds.Right;
            else
                rightMargin = contentBounds.Right - (2 - columnNumber) * contentBounds.Width / 2;

            // Kada imam dve kolone, razmak izmedju tabela podesavam da bude isti kao i razmak sa leve i desne strane.
            // TODO3: Sledece izracunavanje delte ne radi dobro kada ima samo jedna kolona (isto i u izvestaju za start
            // liste i rasporede sudija).
            float delta;
            if (columnNumber == 1)
            {
                delta = contentBounds.X - (2 / 3.0f) * (contentBounds.X - (xRightEnd - rightMargin));  // moze da bude i negativno
                if (delta > contentBounds.X)
                    delta = contentBounds.X;
                xRank -= delta;
                xBroj -= delta;
                xIme -= delta;
                xKlub -= delta;
                xOcena -= delta;
                xRightEnd -= delta;
            }
            else
            {
                delta = (1 / 3.0f) * (contentBounds.X - (xRightEnd - rightMargin));  // moze da bude i negativno
                if (delta < 0)
                    delta = 0.0f;
                xRank += delta;
                xBroj += delta;
                xIme += delta;
                xKlub += delta;
                xOcena += delta;
                xRightEnd += delta;
            }

            StringFormat rankFormat = Izvestaj.centerCenterFormat;
            StringFormat brojFormat = Izvestaj.centerCenterFormat;

            StringFormat imeFormat = new StringFormat(StringFormatFlags.NoWrap);
            imeFormat.Alignment = StringAlignment.Near;
            imeFormat.LineAlignment = StringAlignment.Center;

            StringFormat klubFormat = new StringFormat(StringFormatFlags.NoWrap);
            klubFormat.Alignment = StringAlignment.Near;
            klubFormat.LineAlignment = StringAlignment.Center;

            StringFormat ocenaFormat = Izvestaj.centerCenterFormat;

            StringFormat rankHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat brojHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat imeHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat klubHeaderFormat = Izvestaj.centerCenterFormat;
            StringFormat ocenaHeaderFormat = Izvestaj.centerCenterFormat;

            String rankTitle = Opcije.Instance.RedBrojString;
            String brojTitle = Opcije.Instance.BrojString;
            String imeTitle = Opcije.Instance.ImeString;
            String klubTitle = Opcije.Instance.KlubDrzavaString;
            String ocenaTitle = Opcije.Instance.OcenaString;

            Columns.Clear();

            ReportColumn column1 = addColumn(xRank, rankWidth, rankFormat, rankTitle, rankHeaderFormat);
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

        public int getImeColumnIndex()
        {
            return stampajBroj ? 2 : 1;
        }

        public int getKlubColumnIndex()
        {
            return stampajBroj ? 3 : 2;
        }
    }
}
