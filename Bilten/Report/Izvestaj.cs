using System;
using System.Drawing;
using Bilten.Domain;
using System.Drawing.Printing;

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

        public Izvestaj()
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
        }

        private void createFormats()
		{
            centerCenterFormat = new StringFormat();
            centerCenterFormat.Alignment = StringAlignment.Center;
            centerCenterFormat.LineAlignment = StringAlignment.Center;

            nearCenterFormat = new StringFormat();
            nearCenterFormat.Alignment = StringAlignment.Near;
            nearCenterFormat.LineAlignment = StringAlignment.Center;

            header1Format = centerCenterFormat;
            header2Format = centerCenterFormat;
            header3Format = centerCenterFormat;
            header4Format = centerCenterFormat;
            footerFormat = centerCenterFormat;

            dateFormat = new StringFormat();
			dateFormat.Alignment = StringAlignment.Far;
			dateFormat.LineAlignment = StringAlignment.Near;
		}

		public virtual void BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			//createFonts();
			//e.Cancel = true;
		}

		private void createFonts()
		{
            header1Font = new Font(
                Opcije.Instance.Header1Font, 
                Opcije.Instance.Header1FontSize, 
                Opcije.Instance.getFontStyle(
                    Opcije.Instance.Header1FontBold, Opcije.Instance.Header1FontItalic));
            header2Font = new Font(
                Opcije.Instance.Header2Font,
                Opcije.Instance.Header2FontSize,
                Opcije.Instance.getFontStyle(
                Opcije.Instance.Header2FontBold, Opcije.Instance.Header2FontItalic));
            header3Font = new Font(
                Opcije.Instance.Header3Font,
                Opcije.Instance.Header3FontSize,
                Opcije.Instance.getFontStyle(
                Opcije.Instance.Header3FontBold, Opcije.Instance.Header3FontItalic));
            header4Font = new Font(
                Opcije.Instance.Header4Font,
                Opcije.Instance.Header4FontSize,
                Opcije.Instance.getFontStyle(
                Opcije.Instance.Header4FontBold, Opcije.Instance.Header4FontItalic));
            footerFont = new Font(
                Opcije.Instance.FooterFont,
                Opcije.Instance.FooterFontSize,
                Opcije.Instance.getFontStyle(
                Opcije.Instance.FooterFontBold, Opcije.Instance.FooterFontItalic));
       
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
     /*       g.DrawRectangle(pen, headerBounds.X, headerBounds.Y,
                headerBounds.Width, headerBounds.Height);

            g.DrawRectangle(pen, header1Bounds.X, header1Bounds.Y,
                header1Bounds.Width, header1Bounds.Height);
            g.DrawRectangle(pen, header2Bounds.X, header2Bounds.Y,
                header2Bounds.Width, header2Bounds.Height);
            g.DrawRectangle(pen, header3Bounds.X, header3Bounds.Y,
                header3Bounds.Width, header3Bounds.Height);
            g.DrawRectangle(pen, header4Bounds.X, header4Bounds.Y,
                header4Bounds.Width, header4Bounds.Height);
*/
			g.DrawString(Header1Text, header1Font, blackBrush, header1Bounds, header1Format); 
			g.DrawString(Header2Text, header2Font, blackBrush, header2Bounds, header2Format);
            g.DrawString(Header3Text, header3Font, blackBrush, header3Bounds, header3Format);
            g.DrawString(Header4Text, header4Font, blackBrush, header4Bounds, header4Format);
        }

        private void drawFooter(Graphics g, int pageNum)
        {
     /*       g.DrawRectangle(pen, footerBounds.X, footerBounds.Y,
                footerBounds.Width, footerBounds.Height);
*/
            g.DrawString(FooterText, footerFont, blackBrush, footerBounds, footerFormat);
            
            String page = "Strana";
            String from = "/";
            string datum = TimeOfPrint.ToShortDateString();
            string vreme = TimeOfPrint.ToShortTimeString();
    //        g.DrawString(datum + " " + vreme, pageNumFont, blackBrush,
      //          footerBounds.Right, footerBounds.Top, dateFormat);
            g.DrawString(String.Format("{0} {1}{2}{3}", page, pageNum, from, LastPageNum), pageNumFont, blackBrush,
                footerBounds.Right, footerBounds.Top + pageNumFont.GetHeight(g) * 1.5f, dateFormat);
        }

		public void setupContent(Graphics g, RectangleF marginBounds)
		{
			calculateHeaderBounds(g, marginBounds);
            calculateContentBounds(g, marginBounds);
            calculateFooterBounds(g, marginBounds);
            
            doSetupContent(g);
			contentSetupDone = true;
		}

		private void calculateHeaderBounds(Graphics g, RectangleF marginBounds)
		{
	        float headerSection1RelHeight = 1.0f / 5;
            float headerSection2RelHeight = 1.0f / 5;
            float headerSection3RelHeight = 1.0f / 5;
            float headerSection4RelHeight = 1.0f / 5;

            headerBounds = new RectangleF(marginBounds.Location, 
				new SizeF(marginBounds.Width, getHeaderHeight(g, marginBounds)));

            float headerSec1Height = headerSection1RelHeight * headerBounds.Height;
            float headerSec2Height = headerSection2RelHeight * headerBounds.Height;
            float headerSec3Height = headerSection3RelHeight * headerBounds.Height;
            float headerSec4Height = headerSection4RelHeight * headerBounds.Height;
            float headerSpaceHeight = headerBounds.Height - 
                (headerSec1Height + headerSec2Height + headerSec3Height + headerSec4Height);

            header1Bounds = new RectangleF(headerBounds.Location,
                new SizeF(headerBounds.Width, headerSec1Height));
            header2Bounds = new RectangleF(headerBounds.X, header1Bounds.Bottom,
                headerBounds.Width, headerSec2Height);
            headerSpaceBounds = new RectangleF(headerBounds.X,
                header2Bounds.Bottom, headerBounds.Width, headerSpaceHeight);
            header3Bounds = new RectangleF(headerBounds.X, headerSpaceBounds.Bottom,
                headerBounds.Width, headerSec3Height);
            header4Bounds = new RectangleF(headerBounds.X, header3Bounds.Bottom,
                headerBounds.Width, headerSec4Height);
        }

        public virtual float getHeaderHeight(Graphics g, RectangleF marginBounds)
        {
            return convCmToInch(3.5f);
        }

        private void calculateContentBounds(Graphics g, RectangleF marginBounds)
		{
			float headerHeight = getHeaderHeight(g, marginBounds);
            float footerHeight = getFooterHeight(g, marginBounds);

            contentBounds = new RectangleF(marginBounds.X, 
				marginBounds.Y + headerHeight, 
				marginBounds.Width, 
				marginBounds.Height - headerHeight - footerHeight);
		}

        public virtual float getFooterHeight(Graphics g, RectangleF marginBounds)
        {
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

        public static void scaleImageIsotropically(Graphics g, Image image, RectangleF rect)
        {
            SizeF sizef = new SizeF(image.Width / image.HorizontalResolution,
                                    image.Height / image.VerticalResolution);

            float fScale = Math.Min(rect.Width / sizef.Width,
                                    rect.Height / sizef.Height);

            sizef.Width *= fScale;
            sizef.Height *= fScale;

            g.DrawImage(image, rect.X + (rect.Width - sizef.Width) / 2,
                                  rect.Y + (rect.Height - sizef.Height) / 2,
                                  sizef.Width, sizef.Height);
        }
    }
}
