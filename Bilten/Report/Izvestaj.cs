using System;
using System.Drawing;
using Bilten.Domain;
using System.Drawing.Printing;
using Bilten.UI;

namespace Bilten.Report
{
	public class Izvestaj
	{
        public static StringFormat centerCenterFormat;
        public static StringFormat nearCenterFormat;

		private StringFormat header1Format;
		private StringFormat header2Format;
        private StringFormat header3Format;
        private StringFormat header4Format;
        private StringFormat footerFormat;
        private StringFormat dateFormat;

		private Font header1Font;
		private Font header2Font;
        private Font header3Font;
        private Font header4Font;
        private Font pageNumFont;
        private Font footerFont;
		protected Brush blackBrush;
        protected Pen pen;

		protected RectangleF headerBounds;
        protected RectangleF header1Bounds;
        protected RectangleF header2Bounds;
        protected RectangleF header3Bounds;
        protected RectangleF header4Bounds;
        protected RectangleF headerSpaceBounds;

        protected RectangleF pageBounds;
        protected RectangleF contentBounds;
        protected RectangleF footerBounds;

		protected int lastPageNum;
		public int LastPageNum
		{
			get { return lastPageNum; }
		}

		private bool a4;
		public bool A4
		{
			get { return a4; }
            set { a4 = value; }
		}

        private bool landscape;
        public bool Landscape
        {
            get { return landscape; }
            set { landscape = value; }
        }

        private Margins _margins;
        public Margins Margins
        {
            get { return _margins; }
            set { _margins = value; }
        }

		private string header1Text;
        public string Header1Text
		{
            get { return header1Text; }
            set { header1Text = value; }
		}

        private string header2Text;
        public string Header2Text
        {
            get { return header2Text; }
            set { header2Text = value; }
        }

        private string header3Text;
        public string Header3Text
        {
            get { return header3Text; }
            set { header3Text = value; }
        }

        private string header4Text;
        public string Header4Text
        {
            get { return header4Text; }
            set { header4Text = value; }
        }

        private string footerText;
        public string FooterText
        {
            get { return footerText; }
            set { footerText = value; }
        }

        private string documentName;
		public string DocumentName
		{
			get { return documentName; }
			set { documentName = value; }
		}

		private bool contentSetupDone;
		public bool ContentSetupDone
		{
			get { return contentSetupDone; }
		}

		private DateTime timeOfPrint;
		public DateTime TimeOfPrint
		{
			get { return timeOfPrint; }
			set { timeOfPrint = value; }
		}

        private Takmicenje takmicenje;
        private Image logo1Image;
        private Image logo2Image;
        private Image logo3Image;
        private Image logo4Image;
        private Image logo5Image;

        public Izvestaj(Takmicenje takmicenje)
        {
            Header1Text = Opcije.Instance.Header1Text;
            Header2Text = Opcije.Instance.Header2Text;
            Header3Text = Opcije.Instance.Header3Text;
            Header4Text = Opcije.Instance.Header4Text;
            FooterText = Opcije.Instance.FooterText;
            DocumentName = Opcije.Instance.Header4Text;

            createFormats();
            createFonts();

            A4 = true;
            this.takmicenje = takmicenje;

            // Slike ucitavam u konstruktoru iz dva razloga. Prvo, ako izvestaj ima vise strana, time izbegavam da
            // ponovo ucitavam sliku za svaku stranu. Drugo, ako neka slika fali, stampam poruku o gresci samo jednom,
            // umsto za svaku stranu posebno.
            if (!String.IsNullOrEmpty(takmicenje.Logo1RelPath))
            {
                try
                {
                    logo1Image = Image.FromFile(takmicenje.Logo1RelPath);
                }
                catch (Exception)
                {
                    MessageDialogs.showError("Ne mogu da pronadjem sliku \"" + takmicenje.Logo1RelPath + "\"", "Greska");
                }
            }
            if (!String.IsNullOrEmpty(takmicenje.Logo2RelPath))
            {
                try
                {
                    logo2Image = Image.FromFile(takmicenje.Logo2RelPath);
                }
                catch (Exception)
                {
                    MessageDialogs.showError("Ne mogu da pronadjem sliku \"" + takmicenje.Logo2RelPath + "\"", "Greska");
                }
            }
            if (!String.IsNullOrEmpty(takmicenje.Logo3RelPath))
            {
                try
                {
                    logo3Image = Image.FromFile(takmicenje.Logo3RelPath);
                }
                catch (Exception)
                {
                    MessageDialogs.showError("Ne mogu da pronadjem sliku \"" + takmicenje.Logo3RelPath + "\"", "Greska");
                }
            }
            if (!String.IsNullOrEmpty(takmicenje.Logo4RelPath))
            {
                try
                {
                    logo4Image = Image.FromFile(takmicenje.Logo4RelPath);
                }
                catch (Exception)
                {
                    MessageDialogs.showError("Ne mogu da pronadjem sliku \"" + takmicenje.Logo4RelPath + "\"", "Greska");
                }
            }
            if (!String.IsNullOrEmpty(takmicenje.Logo5RelPath))
            {
                try
                {
                    logo5Image = Image.FromFile(takmicenje.Logo5RelPath);
                }
                catch (Exception)
                {
                    MessageDialogs.showError("Ne mogu da pronadjem sliku \"" + takmicenje.Logo5RelPath + "\"", "Greska");
                }
            }
        }

        private void createFormats()
		{
            centerCenterFormat = new StringFormat();
            centerCenterFormat.Alignment = StringAlignment.Center;
            centerCenterFormat.LineAlignment = StringAlignment.Center;

            nearCenterFormat = new StringFormat();
            nearCenterFormat.Alignment = StringAlignment.Near;
            nearCenterFormat.LineAlignment = StringAlignment.Center;

            StringFormat centerNearFormat = new StringFormat();
            centerNearFormat.Alignment = StringAlignment.Center;
            centerNearFormat.LineAlignment = StringAlignment.Near;

            header1Format = centerNearFormat;
            header2Format = centerCenterFormat;
            header3Format = centerCenterFormat;
            header4Format = centerCenterFormat;
            footerFormat = centerCenterFormat;

            dateFormat = new StringFormat();
			dateFormat.Alignment = StringAlignment.Far;
			dateFormat.LineAlignment = StringAlignment.Center;
		}

		public virtual void BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			//createFonts();
			//e.Cancel = true;
		}

        protected virtual FontStyle getFontStyle(bool bold, bool italic)
        {
            if (!bold && !italic)
                return FontStyle.Regular;
            else if (bold && !italic)
                return FontStyle.Bold;
            else if (!bold && italic)
                return FontStyle.Italic;
            else
                return FontStyle.Bold | FontStyle.Italic;
        }

        private void createFonts()
		{
            header1Font = new Font(
                Opcije.Instance.Header1Font, 
                Opcije.Instance.Header1FontSize, 
                getFontStyle(Opcije.Instance.Header1FontBold, Opcije.Instance.Header1FontItalic));
            header2Font = new Font(
                Opcije.Instance.Header2Font,
                Opcije.Instance.Header2FontSize,
                getFontStyle(Opcije.Instance.Header2FontBold, Opcije.Instance.Header2FontItalic));
            header3Font = new Font(
                Opcije.Instance.Header3Font,
                Opcije.Instance.Header3FontSize,
                getFontStyle(Opcije.Instance.Header3FontBold, Opcije.Instance.Header3FontItalic));
            header4Font = new Font(
                Opcije.Instance.Header4Font,
                Opcije.Instance.Header4FontSize,
                getFontStyle(Opcije.Instance.Header4FontBold, Opcije.Instance.Header4FontItalic));
            footerFont = new Font(
                Opcije.Instance.FooterFont,
                Opcije.Instance.FooterFontSize,
                getFontStyle(Opcije.Instance.FooterFontBold, Opcije.Instance.FooterFontItalic));
       
            pageNumFont = new Font("Arial", 8);
			blackBrush = Brushes.Black;
            pen = new Pen(Color.Black, 1 / 72f * 0.25f);
		}

		public virtual void EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			//releaseFonts();
		}

        public static float convCmToInch(float x)
        {
            return x / 2.54f;
        }

        public void drawPage(Graphics g, int pageNum)
		{
			drawHeader(g, pageNum);
			drawContent(g, pageNum);
            drawFooter(g, pageNum);
        }

		public virtual void drawHeader(Graphics g, int pageNum)
		{
            /*g.DrawRectangle(pen, headerBounds.X, headerBounds.Y,
                headerBounds.Width, headerBounds.Height);

            g.DrawRectangle(pen, header1Bounds.X, header1Bounds.Y,
                header1Bounds.Width, header1Bounds.Height);
            g.DrawRectangle(pen, header2Bounds.X, header2Bounds.Y,
                header2Bounds.Width, header2Bounds.Height);
            g.DrawRectangle(pen, header3Bounds.X, header3Bounds.Y,
                header3Bounds.Width, header3Bounds.Height);
            g.DrawRectangle(pen, header4Bounds.X, header4Bounds.Y,
                header4Bounds.Width, header4Bounds.Height);*/

			g.DrawString(Header1Text, header1Font, blackBrush, header1Bounds, header1Format); 
			g.DrawString(Header2Text, header2Font, blackBrush, header2Bounds, header2Format);
            g.DrawString(Header3Text, header3Font, blackBrush, header3Bounds, header3Format);
            g.DrawString(Header4Text, header4Font, blackBrush, header4Bounds, header4Format);

            // TODO5: Kada postoji logo1 a ne postoji logo2, treba prvo pokusati da se centrira unutar regiona izmedju
            // dva logoa (kao sto se i sada radi). Ako to ne moze da stane, treba prvo proveriti da li header 1 moze da
            // stane u srednji i desni region. Ako moze, stampati tako da je x koordinata tamo gde je pocetak srednjeg
            // regiona. Ako ne moze, stampati u dva reda, centrirano u srednjem regionu (kao sto se i sada radi).

            float logoHeight = getHeaderLogoHeight();
            if (!String.IsNullOrEmpty(takmicenje.Logo1RelPath))
            {
                // Stampaj sa leve strane hedera
                RectangleF logo1Bounds = new RectangleF(pageBounds.Left, pageBounds.Top, logoHeight, logoHeight);
                //pictureBounds.Inflate(-0.05f * pictureWidth, -0.05f * pictureHeight);
                drawLogo(g, logo1Bounds, logo1Image);
            }
            if (!String.IsNullOrEmpty(takmicenje.Logo2RelPath))
            {
                // Stampaj sa desne strane hedera
                RectangleF logo2Bounds = new RectangleF(pageBounds.Right - logoHeight, pageBounds.Top,
                    logoHeight, logoHeight);
                drawLogo(g, logo2Bounds, logo2Image);
            }
        }

        private float getHeaderLogoHeight()
        {
            float rel = 0.16f;
            if (pageBounds.Width < pageBounds.Height)
            {
                return rel * pageBounds.Width;
            }
            else
            {
                return rel * pageBounds.Height;
            }
        }

        private void drawFooter(Graphics g, int pageNum)
        {
            float height = pageBounds.Bottom - footerBounds.Top;  // ovo je takodje i width za logo
            float y = footerBounds.Top;

            float endX = 0.0f;
            if (!String.IsNullOrEmpty(takmicenje.Logo3RelPath)
                && String.IsNullOrEmpty(takmicenje.Logo4RelPath) && String.IsNullOrEmpty(takmicenje.Logo5RelPath))
            { 
                // Centriraj logo3 po sredini futera
                float x = pageBounds.Left + (pageBounds.Width - height) / 2;
                RectangleF logo3Bounds = new RectangleF(x, y, height, height);
                drawLogo(g, logo3Bounds, logo3Image);
                endX = logo3Bounds.Right;
            }
            else if (!String.IsNullOrEmpty(takmicenje.Logo3RelPath) && !String.IsNullOrEmpty(takmicenje.Logo4RelPath)
                && String.IsNullOrEmpty(takmicenje.Logo5RelPath))
            {
                // Centriraj po sredini futera logo3, logo4
                float medjuRazmak = height * 0.2f;
                float sredinaStrane = pageBounds.Left + pageBounds.Width / 2;
                float leviX = sredinaStrane - medjuRazmak/2 - height;
                float desniX = sredinaStrane + medjuRazmak / 2;
                RectangleF logo3Bounds = new RectangleF(leviX, y, height, height);
                RectangleF logo4Bounds = new RectangleF(desniX, y, height, height);
                drawLogo(g, logo3Bounds, logo3Image);
                drawLogo(g, logo4Bounds, logo4Image);
                endX = logo4Bounds.Right;
            }
            else if (!String.IsNullOrEmpty(takmicenje.Logo3RelPath) && !String.IsNullOrEmpty(takmicenje.Logo4RelPath)
                && !String.IsNullOrEmpty(takmicenje.Logo5RelPath))
            {
                // Centriraj po sredini futera logo3, logo4, logo5
                float medjuRazmak = height * 0.2f;
                float srednjiX = pageBounds.Left + (pageBounds.Width - height) / 2;
                float leviX = srednjiX - medjuRazmak - height;
                float desnix = srednjiX + height + medjuRazmak;
                RectangleF logo3Bounds = new RectangleF(leviX, y, height, height);
                RectangleF logo4Bounds = new RectangleF(srednjiX, y, height, height);
                RectangleF logo5Bounds = new RectangleF(desnix, y, height, height);
                drawLogo(g, logo3Bounds, logo3Image);
                drawLogo(g, logo4Bounds, logo4Image);
                drawLogo(g, logo5Bounds, logo5Image);
                endX = logo5Bounds.Right;
            }

            //g.DrawRectangle(pen, footerBounds.X, footerBounds.Y, footerBounds.Width, footerBounds.Height);

            String page = "Strana";
            String from = "/";
            string datum = TimeOfPrint.ToShortDateString();
            string vreme = TimeOfPrint.ToShortTimeString();
            string pageText = String.Format("{0} {1}{2}{3}", page, pageNum, from, LastPageNum);

            if (String.IsNullOrEmpty(takmicenje.Logo3RelPath) && String.IsNullOrEmpty(takmicenje.Logo4RelPath)
                && String.IsNullOrEmpty(takmicenje.Logo5RelPath))
            {
                g.DrawString(FooterText, footerFont, blackBrush, footerBounds, footerFormat);
                g.DrawString(pageText, pageNumFont, blackBrush,
                    footerBounds.Right, footerBounds.Top + pageNumFont.GetHeight(g) * 1.5f, dateFormat);
            }
            else
            {
                // TODO5: Probaj da nadjes neko generalnije resenje.
                float x = footerBounds.Right + (pageBounds.Right - footerBounds.Right) / 2;
                float y1 = footerBounds.Y + footerBounds.Height / 2;
                // Uz desnu ivicu i po sredini gornje linije (racuna se u odnosu na dve linije koje se seku u tacki (x, y1)
                g.DrawString(FooterText, footerFont, blackBrush, x, y1, dateFormat);

                float y2 = footerBounds.Bottom;
                StringFormat fmt = new StringFormat();
                fmt.Alignment = StringAlignment.Far;
                fmt.LineAlignment = StringAlignment.Near;
                // Uz desnu i gornju ivicu (racuna se u odnosu na dve linije koje se seku u tacki (x, y2)
                g.DrawString(pageText, pageNumFont, blackBrush, x, y2, fmt);
            }
            // g.DrawString(datum + " " + vreme, pageNumFont, blackBrush,
            //      footerBounds.Right, footerBounds.Top, dateFormat);
        }

        private void drawLogo(Graphics g, RectangleF bounds, Image image)
        {
            if (image != null)
            {
                Izvestaj.scaleImageIsotropically(g, image, bounds);
            }
            else
            {
                // Slika nije pronadjena. Stampaj prazan okvir
                using (Pen pen = new Pen(Color.Black, 1 / 72f * 0.25f))
                {
                    g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                }
            }
        }

        public void setupContent(Graphics g, RectangleF marginBounds, RectangleF pageBounds)
		{
            this.pageBounds = pageBounds;

			calculateHeaderBounds(g, marginBounds);
            calculateContentBounds(g, marginBounds);
            calculateFooterBounds(g, marginBounds);
            
            doSetupContent(g);
			contentSetupDone = true;
		}

		private void calculateHeaderBounds(Graphics g, RectangleF marginBounds)
		{
            // Potrebno za g.MeasureString (headeri mogu da se prostiru u dva reda)
            StringFormat fmt = new StringFormat();
            fmt.Alignment = StringAlignment.Near;
            fmt.LineAlignment = StringAlignment.Near;

            // Ovo je potrebno za neka podesavanja u hederu. Ovo je visina za header1 tekst kada bi ceo stao u jedan red.
            // Prava visina za header1 tekst (koja moze da zauzima dva reda) se izracunava dole u headerSec1Height.
            float header1Height = g.MeasureString(Header1Text, header1Font, new PointF(), fmt).Height;    
            
            PointF header1TopLeft;
            float width;
            if (!String.IsNullOrEmpty(takmicenje.Logo1RelPath))
            {
                header1TopLeft = new PointF(pageBounds.Left + getHeaderLogoHeight(), pageBounds.Top + header1Height);
                width = pageBounds.Width - 2 * getHeaderLogoHeight();
            }
            else
            {
                if (!String.IsNullOrEmpty(takmicenje.Logo2RelPath))
                {
                    header1TopLeft = new PointF(pageBounds.Left + getHeaderLogoHeight(), pageBounds.Top + header1Height);
                    width = pageBounds.Width - 2 * getHeaderLogoHeight();
                }
                else
                {
                    header1TopLeft = marginBounds.Location;
                    width = marginBounds.Width;
                }
            }

            float x = header1TopLeft.X;

            SizeF layoutArea = new SizeF(width, /*dummy*/width);  // koristi se samo za merenje (da sve ispravno stane u
                                                                  // dva reda ako treba), visina je nebitna
            if (!String.IsNullOrEmpty(Header1Text))
            {
                float headerSec1Height = g.MeasureString(Header1Text, header1Font, layoutArea, fmt).Height;
                header1Bounds = new RectangleF(header1TopLeft, new SizeF(width, headerSec1Height));
            }
            else
            {
                // Prazan rectangle koji ima ispravnu Bottom koordinatu.
                header1Bounds = new RectangleF(header1TopLeft, new SizeF());
            }

            PointF header2TopLeft = new PointF(x, header1Bounds.Bottom);
            if (!String.IsNullOrEmpty(Header2Text))
            {
                float headerSec2Height = g.MeasureString(Header2Text, header2Font, layoutArea, fmt).Height;
                header2Bounds = new RectangleF(header2TopLeft, new SizeF(width, headerSec2Height));
            }
            else
            {
                header2Bounds = new RectangleF(header2TopLeft, new SizeF());
            }

            float header2AfterHeight = header1Height;
            PointF header3TopLeft = new PointF(x, header2Bounds.Bottom + header2AfterHeight);
            if (!String.IsNullOrEmpty(Header3Text))
            {
                float headerSec3Height = g.MeasureString(Header3Text, header3Font, layoutArea, fmt).Height;
                header3Bounds = new RectangleF(header3TopLeft, new SizeF(width, headerSec3Height));
            }
            else
            {
                header3Bounds = new RectangleF(header3TopLeft, new SizeF());
            }

            PointF header4TopLeft = new PointF(x, header3Bounds.Bottom);
            if (!String.IsNullOrEmpty(Header4Text))
            {
                float headerSec4Height = g.MeasureString(Header4Text, header4Font, layoutArea, fmt).Height;
                header4Bounds = new RectangleF(header4TopLeft, new SizeF(width, headerSec4Height));
            }
            else
            {
                header4Bounds = new RectangleF(header4TopLeft, new SizeF());
            }

            headerBounds = new RectangleF(header1TopLeft, new SizeF(width, header4Bounds.Bottom - header1Bounds.Top));
        }

        // TODO3: Nadji bolji nacin za podesavanje velicine hedera i futera
        protected bool kvalifikantiIzvestaj;

        public virtual float getHeaderHeight(Graphics g, RectangleF marginBounds)
        {
            if (kvalifikantiIzvestaj)
                return convCmToInch(2.5f);
            else
                return convCmToInch(3.5f);
        }

        private void calculateContentBounds(Graphics g, RectangleF marginBounds)
		{
			float headerHeight = getHeaderHeight(g, marginBounds);
            float footerHeight = getFooterHeight(g, marginBounds);

            float headerBottom = header4Bounds.Bottom + convCmToInch(0.5f);
            contentBounds = new RectangleF(marginBounds.X, headerBottom,
                marginBounds.Width, marginBounds.Bottom - headerBottom - footerHeight);
		}

        public virtual float getFooterHeight(Graphics g, RectangleF marginBounds)
        {
            if (kvalifikantiIzvestaj)
                return convCmToInch(0.3f);
            else
                return convCmToInch(1f);
        }

        private void calculateFooterBounds(Graphics g, RectangleF marginBounds)
        {
            float footerHeight = getFooterHeight(g, marginBounds);
            footerBounds = new RectangleF(marginBounds.X, marginBounds.Bottom - footerHeight,
                marginBounds.Width, footerHeight);
        }

        protected virtual void doSetupContent(Graphics g)
		{
		
		}

		public virtual void drawContent(Graphics g, int pageNum)
		{

		}
		
		public void QueryPageSettings(object sender, System.Drawing.Printing.QueryPageSettingsEventArgs e)
		{
			// Set margins to .5" all the way around
			//e.PageSettings.Margins = new Margins(50, 50, 50, 50);
		}

        public static void scaleImageIsotropically(Graphics g, Image image, RectangleF rect, float reductionRatio = 1f)
        {
            SizeF sizef = new SizeF(image.Width / image.HorizontalResolution,
                                    image.Height / image.VerticalResolution);

            float fScale = Math.Min(rect.Width / sizef.Width,
                                    rect.Height / sizef.Height);

            sizef.Width *= fScale;
            sizef.Height *= (fScale * reductionRatio);

            g.DrawImage(image, rect.X + (rect.Width - sizef.Width) / 2,
                                  rect.Y + (rect.Height - sizef.Height) / 2,
                                  sizef.Width, sizef.Height);
        }
    }
}
