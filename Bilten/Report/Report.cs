using System;
using System.Drawing;
using Bilten.Exceptions;
using System.Collections.Generic;
using Bilten.Domain;
using System.Windows.Forms;

namespace Bilten.Report
{
	public class ReportColumn
	{
        private ReportLista _lista;
        public ReportLista Lista
        {
            get { return _lista; }
            set { _lista = value; }
        }

        private bool _includeInMarking = false;
        public bool IncludeInMarking
        {
            get { return _includeInMarking; }
            set { _includeInMarking = value; }
        }

		private float x;
		private float width;
		
        public int itemsIndex;

        protected int itemsSpan = 1;

		private StringFormat itemRectFormat = new StringFormat();

        private Image _image;
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private Sprava _sprava = Sprava.Undefined;
        public Sprava Sprava
        {
            get { return _sprava; }
            set { _sprava = value; }
        }

        private bool _split;
        public bool Split
        {
            get { return _split; }
            set { _split = value; }
        }

        private bool _span;
        public bool Span
        {
            get { return _span; }
            set { _span = value; }
        }

        private ReportColumn _spanEndColumn;
        public ReportColumn SpanEndColumn
        {
            get { return _spanEndColumn; }
            set { _spanEndColumn = value; }
        }

        private Brush _brush;
        public Brush Brush
        {
            get { return _brush; }
            set { _brush = value; }
        }

        private bool _drawHeaderRect = true;
        public bool DrawHeaderRect
        {
            get { return _drawHeaderRect; }
            set { _drawHeaderRect = value; }
        }

        private bool _drawItemRect = true;
        public bool DrawItemRect
        {
            get { return _drawItemRect; }
            set { _drawItemRect = value; }
        }

        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public ReportColumn()
		{

		}

        public ReportColumn(int itemsIndex, float x, float width, string headerTitle)
		{
            this.itemsIndex = itemsIndex;
			this.x = x;
			this.width = width;
			this.headerTitle = headerTitle;
		}

		public float X
		{
			get { return x; }
			set { x = value; }
		}

		public float Width
		{
			get { return width; }
			set { width = value; }
		}

        private string format;
        public string Format
		{
			set { format = value; }
		}

		public StringFormat ItemRectFormat
		{
			get { return itemRectFormat; }
			set { itemRectFormat = value; }
		}

		private string headerTitle;
		public string HeaderTitle
		{
			get 
			{
				if (headerTitle == null)
					return String.Empty;
				return headerTitle; 
			}
			set { headerTitle = value; }
		}

		private StringFormat headerFormat = new StringFormat();
		public StringFormat HeaderFormat
		{
			get { return headerFormat; }
			set { headerFormat = value; }
		}

        // static method
        public static string getFormattedString(object[] itemsRow, int itemsIndex, string fmt)
        {
            object item = itemsRow[itemsIndex];
            if (item == null)
                return String.Empty;
            else if (String.IsNullOrEmpty(fmt))
                return item.ToString();
            else
            {
                fmt = "{0:" + fmt + "}";
                return String.Format(fmt, item);
            }
        }

        public string getFormattedString(object[] itemsRow, int itemsIndex)
        {
            return getFormattedString(itemsRow, itemsIndex, format);
        }

        protected RectangleF itemRect;
        public RectangleF getItemRect()
        {
            return itemRect;
        }

        public virtual void draw(Graphics g, Pen pen, object[] itemsRow, Font itemFont, Brush blackBrush)
        {
            string rowNum = this.getFormattedString(itemsRow, itemsRow.Length - 1);
            int currentRow = 0;
            if (!Int32.TryParse(rowNum, out currentRow))
                currentRow = 0;

            if (!Visible)
                return;

            if (this.Brush != null)
            {
                g.FillRectangle(this.Brush, itemRect.X, itemRect.Y,
                    itemRect.Width, itemRect.Height);
            }
            if (Lista != null && Lista.markFirstRows && this.IncludeInMarking
                && currentRow > 0 && currentRow <= Lista.numRowsToMark)
            {
                g.FillRectangle(this.Lista.markFirstRowsBrush, itemRect.X, itemRect.Y,
                    itemRect.Width, itemRect.Height);
            }
            if (this.DrawItemRect)
            {
                g.DrawRectangle(pen, itemRect.X, itemRect.Y,
                    itemRect.Width, itemRect.Height);
            }

            string item = this.getFormattedString(itemsRow, itemsIndex);
            g.DrawString(item, itemFont, blackBrush,
                itemRect, this.ItemRectFormat);
        }

        public void createItemRect(float y, float itemHeight)
        {
            itemRect = new RectangleF(
                this.X, y, this.Width, itemHeight);
        }

        public void offsetY_ItemRect(float itemHeight)
        {
            this.itemRect.Y += itemHeight;
        }

        public int getItemsIndexEnd()
        {
            return itemsIndex + itemsSpan;
        }

        public virtual float getMaxWidth(Graphics g, object[] itemsRow, Font f)
        {
            string str = this.getFormattedString(itemsRow, itemsIndex);
            return g.MeasureString(str, f).Width;
        }
    }

	public class ReportGrupa
	{
		private int start;
		public int Start
		{
			get { return start; }
			set { start = value; }
		}

		private int count;
		public int Count
		{
			get { return count; }
			set { count = value; }
		}

		private object[] data;
		public object[] Data
		{
			get { return data; }
			set { data = value; }
		}

		private ReportGrupa masterGrupa;
		public ReportGrupa MasterGrupa
		{
			get { return masterGrupa; }
			set { masterGrupa = value; }
		}

		private int detailGrupeStart;
		public int DetailGrupeStart
		{
			get { return detailGrupeStart; }
			set { detailGrupeStart = value; }
		}

		private int detailGrupeCount;
		public int DetailGrupeCount
		{
			get { return detailGrupeCount; }
			set { detailGrupeCount = value; }
		}

		public ReportGrupa()
		{

		}

		public ReportGrupa(object[] data, int count)
		{
			this.data = data;
			this.count = count;
		}
		
		public ReportGrupa(object[] data, int start, int count)
		{
			this.data = data;
			this.start = start;
			this.count = count;
		}
		
		public ReportGrupa(int start, int count)
		{
			this.start = start;
			this.count = count;
		}
	}

	public abstract class ReportLista
	{
        private List<ReportColumn> columns = new List<ReportColumn>();
        public List<ReportColumn> Columns
        {
            get { return columns; }
        }

		protected Izvestaj izvestaj;

        protected DataGridView formGrid;
		
        private int firstPageNum;
        public int FirstPageNum
        {
            get { return firstPageNum; }
            set { firstPageNum = value; }
        }

		private float startY;
		public float StartY
		{
			get { return startY; }
			set { startY = value; }
		}

        protected int lastPageNum;
		public int LastPageNum
		{
			get { return lastPageNum; }
		}

		protected float endY;
		public float EndY
		{
			get { return endY; }
		}

		protected Font itemFont;
		public Font ItemFont
		{
			get { return itemFont; }
			set { itemFont = value; }
		}

        private bool groupHeaderVisible = true;
        public bool GroupHeaderVisible
		{
            get { return groupHeaderVisible; }
            set { groupHeaderVisible = value; }
		}

        private bool showHeaderForSecondListOnPage = true;
        public bool ShowHeaderForSecondListOnPage
		{
            get { return showHeaderForSecondListOnPage; }
            set { showHeaderForSecondListOnPage = value; }
		}

        protected float xRightEnd;  // TODO5: Izbrisi ovaj member kada preuredis sve izvestaje
        public float getRightEnd()
        {
            return columns[columns.Count - 1].X + columns[columns.Count - 1].Width;
        }

        public float getLeftEnd()
        {
            return columns[0].X;
        }

        protected bool resizeByGrid;
        public bool ResizeByGrid
        {
            get { return resizeByGrid; }
        }

        protected Font itemsHeaderFont;
		protected Brush blackBrush;
        protected Pen pen;

        public bool markFirstRows = false;
        public int numRowsToMark = 0;
        public Brush markFirstRowsBrush = Brushes.Gainsboro;
    
        protected float itemHeight;
		protected float groupHeaderHeight;
		protected float itemsHeaderHeight;
        protected float groupFooterHeight;
        protected float masterGroupHeaderHeight;

        // TODO5: Ovo bi trebalo da moze da se menja iz programa
        protected const string RANK_MAX_TEXT = "0000";
        protected const string BROJ_MAX_TEXT = "00000";
        protected const string D_MAX_TEXT_UKUPNO = "00.0";
        protected const string E_MAX_TEXT_UKUPNO = "00.000";
        protected const string PENALTY_MAX_TEXT_UKUPNO = "000.0";
        protected const string TOTAL_MAX_TEXT_UKUPNO = "000.000";
        protected const string TOTAL_MAX_TEXT_UKUPNO_BON_PEN = "0000.000";
        protected const string QUAL_MAX_TEXT = "QQ";

        protected const string SKOK_MAX_TEXT = "00";

        protected const string TOTAL_MAX_TEXT_UKUPNO_FINALE_KUPA = "0000.000";
        protected const string KOLO_MAX_TEXT = "00";

        protected const string REDNI_BROJ_MAX_TEXT = "0000";

        protected float getColumnWidth(Graphics g, string maxText, string columnTitle)
        {
            float textWidth = g.MeasureString(maxText, itemFont).Width;
            if (String.IsNullOrEmpty(columnTitle))
                return textWidth;
            float headerWidth = g.MeasureString(columnTitle, itemsHeaderFont).Width;
            return Math.Max(textWidth, headerWidth);
        }

        protected float getColumnWidth(Graphics g, float columnWidth, string columnTitle)
        {
            float headerWidth = g.MeasureString(columnTitle, itemsHeaderFont).Width;
            return Math.Max(columnWidth, headerWidth);
        }

        protected List<object[]> items;
        public List<object[]> Items
		{
			get { return items; }
		}

        protected List<ReportGrupa> groups;
        protected IDictionary<int, List<ReportGroupPart>> listLayout;
        protected bool startNewPageWithGroupHeader = true;

        public static string TEST_TEXT = "0123456789abcdef";

        public static float getGridTextWidth(DataGridView dgw, string text)
        {
            Graphics g = dgw.CreateGraphics();
            // rezultat je smanjen za 5% zato sto kada u gridu namestim velicinu kolone prema najduzem tekstu, velicina
            // teksta koju daje metod MeasureString je nesto veca od velicine kolone.
            float width = g.MeasureString(text, dgw.Font).Width * 0.95f;
            g.Dispose();
            return width;
        }

        public ReportLista(Izvestaj izvestaj, int pageNum, float y, Font itemFont,
            Font itemsHeaderFont, DataGridView formGrid)
		{
			this.izvestaj = izvestaj;
			this.firstPageNum = pageNum;
			this.startY = y;
            this.formGrid = formGrid;

			this.itemFont = itemFont;
			this.itemsHeaderFont = itemsHeaderFont;
			blackBrush = Brushes.Black;
            pen = new Pen(Color.Black, 1 / 72f * 0.25f);
        }

		protected ReportColumn addColumn(float x, float width)
		{
			ReportColumn result = new ReportColumn(columns.Count, x, width, String.Empty);
			columns.Add(result);
            result.Lista = this;
			return result;
		}

		protected ReportColumn addColumn(float x, float width, string headerTitle,
			StringFormat headerFormat)
		{
			ReportColumn result = new ReportColumn(columns.Count, x, width, headerTitle);
			result.HeaderFormat = headerFormat;
			columns.Add(result);
            result.Lista = this;
            return result;
		}

		protected ReportColumn addColumn(float x, float width,
			StringFormat itemRectFormat, string headerTitle,
			StringFormat headerFormat)
		{
			ReportColumn result = new ReportColumn(columns.Count, x, width, headerTitle);
			result.ItemRectFormat = itemRectFormat;
			result.HeaderFormat = headerFormat;
			columns.Add(result);
            result.Lista = this;
            return result;
		}

		protected ReportColumn addColumn(float x, float width, string format,
			StringFormat itemRectFormat, string headerTitle,
			StringFormat headerFormat)
		{
			ReportColumn result = new ReportColumn(columns.Count, x, width, headerTitle);
			result.Format = format;
			result.ItemRectFormat = itemRectFormat;
			result.HeaderFormat = headerFormat;
			columns.Add(result);
            result.Lista = this;
            return result;
		}

        protected ReportColumn addColumn(int itemsIndex, float x, float width, string format,
            StringFormat itemRectFormat, string headerTitle,
            StringFormat headerFormat)
        {
            ReportColumn result = new ReportColumn(itemsIndex, x, width, headerTitle);
            result.Format = format;
            result.ItemRectFormat = itemRectFormat;
            result.HeaderFormat = headerFormat;
            columns.Add(result);
            result.Lista = this;
            return result;
        }

        protected ReportColumn addColumn(float x, float width,
			StringFormat itemRectFormat, string format)
		{
			ReportColumn result = new ReportColumn(columns.Count, x, width, String.Empty);
			result.Format = format;
			result.ItemRectFormat = itemRectFormat;
			columns.Add(result);
            result.Lista = this;
            return result;
		}

        protected ReportColumn addColumn(int itemsIndex, float x, float width,
            StringFormat itemRectFormat, string format)
        {
            ReportColumn result = new ReportColumn(itemsIndex, x, width, String.Empty);
            result.Format = format;
            result.ItemRectFormat = itemRectFormat;
            columns.Add(result);
            result.Lista = this;
            return result;
        }

        public float getColumnMaxWidth(int colIndex, Graphics g)
		{
			ReportColumn col = columns[colIndex];
			float max = 0;
			for (int i = 0; i < items.Count; i++)
			{
				object[] itemsRow = items[i];
                max = Math.Max(max, col.getMaxWidth(g, itemsRow, itemFont));
			}
			return max;
		}

        // Kolone za koje mozemo da podesavamo sirinu kolona (ili pomocu grida, ili na osnovu maksimalne duzine teksta
        // u ovoj koloni u svim listama)
        public virtual List<int> getAdjustableColumnIndexes()
        {
            List<int> result = new List<int>();
            return result;
        }

        protected virtual void doSetupContent(Graphics g, RectangleF contentBounds, List<float> columnWidths,
            List<bool> resizeByGrid)
        {

        }

        public void setupContent(Graphics g, RectangleF contentBounds, int passIndex, List<int> columnIndexes,
            List<float> columnMaxWidths)
        {
            if (columnMaxWidths.Count == 0)
            {
                // Ova grana se izvrsava samo pri prvom pozivu za prvu listu u izvestaju
                for (int i = 0; i < columnIndexes.Count; ++i)
                {
                    columnMaxWidths.Add(0.0f);
                }
            }
            if (passIndex == 0)
            {
                List<bool> rszByGrid = new List<bool>();
                for (int i = 0; i < columnIndexes.Count; ++i)
                {
                    // Koristimo vrednost koja je podesena za listu, ali dozvoljavamo izvedenoj klasi da promeni za neku
                    // od kolona ako je potrebno. Npr. u stampanju viseboja po klubovima i kategorijama nemamo kategoriju u
                    // gridu, tako da resize by grid mora da bude false
                    rszByGrid.Add(ResizeByGrid);
                }
                doSetupContent(g, contentBounds, new List<float>()/* ne koristi se u prvom pasu */, rszByGrid);
                for (int i = 0; i < columnIndexes.Count; ++i)
                {
                    if (columnIndexes[i] != -1)
                    {
                        float width;
                        if (rszByGrid[i])
                            width = Columns[columnIndexes[i]].Width;
                        else
                            width = getColumnMaxWidth(columnIndexes[i], g);
                        if (width > columnMaxWidths[i])
                            columnMaxWidths[i] = width;
                    }
                }
            }
            else
            {
                doSetupContent(g, contentBounds, columnMaxWidths, new List<bool>() /* ne koristi se u drugom pasu */);
            }
        }

		protected void createListLayout(float groupHeaderHeight, float itemHeight, 
            float groupFooterHeight,
			float afterGroupHeight, float newPageTopOffset, RectangleF contentBounds, 
			float masterGroupHeaderHeight, float afterMasterGroupHeight)
		{
			float minHeight = masterGroupHeaderHeight + groupHeaderHeight + itemHeight;
			if (minHeight > contentBounds.Height)
				throw new SmallPageSizeException();

			int pageNum = firstPageNum;
			float y = StartY;

            listLayout = new Dictionary<int, List<ReportGroupPart>>();
			
			ReportGrupa gr = null;
			ReportGrupa prevGrupa;
			
			for (int i = 0; i < groups.Count; i++)
			{
				prevGrupa = gr;
				gr = groups[i];
				
				int recNum = gr.Start;
				int endRec = gr.Start + gr.Count;

				while (recNum < endRec)
				{
					bool newPage = false;
					if (recNum == gr.Start && shouldStartOnNewPage(gr, prevGrupa)
						|| (y + minHeight > contentBounds.Bottom))
					{
						newPage = true;
						pageNum++;
						y = contentBounds.Y + newPageTopOffset;
					}

					float groupPartY = y;

					bool masterGroupHeader = false;
					bool groupHeader = false;
					if (recNum == gr.Start)
					{
						if (masterGroupHeaderHeight != 0f)
						{
							if (prevGrupa == null
								|| !object.ReferenceEquals(gr.MasterGrupa, prevGrupa.MasterGrupa))
							{
								if (prevGrupa != null && !newPage)
								{
									y += afterMasterGroupHeight;
									groupPartY = y;
								}
								masterGroupHeader = true;
								y += masterGroupHeaderHeight;
							}
						}

						groupHeader = true;
						y += groupHeaderHeight;
					}
                    else if (newPage)
                    {
                        if (startNewPageWithGroupHeader)
                        {
                            groupHeader = true;
                            y += groupHeaderHeight;
                        }
                    }

					int numItems;
					if (y + (endRec - recNum) * itemHeight <= contentBounds.Bottom)
						numItems = endRec - recNum;
					else
						numItems = (int)Math.Floor((contentBounds.Bottom - y) / itemHeight);

                    bool groupFooter = recNum + numItems >= endRec;
					ReportGroupPart part = new ReportGroupPart(pageNum, i, recNum, 
						numItems, groupPartY, groupHeader, masterGroupHeader, groupFooter);

                    List<ReportGroupPart> partList;
                    if (listLayout.ContainsKey(pageNum))
                    {
                        partList = listLayout[pageNum];
                    }
                    else
                    {
                        partList = new List<ReportGroupPart>();
                        listLayout.Add(pageNum, partList);
                    }
					partList.Add(part);

					y += numItems * itemHeight;
					recNum += numItems;
				}
                y += groupFooterHeight;
                y += afterGroupHeight;
			}
			this.lastPageNum = pageNum;
			this.endY = y;
		}
		
		protected void createListLayout(float groupHeaderHeight, float itemHeight, 
            float groupFooterHeight,
            float afterGroupHeight, float newPageTopOffset, 
			RectangleF contentBounds)
		{
			createListLayout(groupHeaderHeight, itemHeight, groupFooterHeight,
                afterGroupHeight, newPageTopOffset, contentBounds, 0f, 0f);
		}

		protected virtual bool shouldStartOnNewPage(ReportGrupa grupa, 
			ReportGrupa prevGrupa)
		{
			return false;
		}

        public void drawContent(Graphics g, RectangleF contentBounds, int pageNum)
		{
            if (!listLayout.ContainsKey(pageNum))
                return;

            List<ReportGroupPart> parts = listLayout[pageNum];
			foreach (ReportGroupPart part in parts)
			{
				float y = part.Y;
				if (part.MasterGroupHeader)
				{
					RectangleF masterGroupHeaderRect = new RectangleF(
                        columns[0].X, y, getRightEnd() - getLeftEnd(), masterGroupHeaderHeight);
					drawMasterGroupHeader(g, part.GroupId, masterGroupHeaderRect);
					y += masterGroupHeaderHeight;
				}
				if (part.GroupHeader)
				{
					RectangleF groupHeaderRect = new RectangleF(
                        columns[0].X, y, getRightEnd() - getLeftEnd(), groupHeaderHeight);
                    if (GroupHeaderVisible)
                    {
                        drawGroupHeader(g, part.GroupId, groupHeaderRect);
                    }
					y += groupHeaderHeight;
				}

				foreach (ReportColumn col in columns)
				{
                    col.createItemRect(y, itemHeight);
				}

				for (int i = part.RecNum; i < part.RecNum + part.NumItems; i++)
				{
					object[] itemsRow = items[i];
					foreach (ReportColumn col in columns)
					{
                        col.draw(g, pen, itemsRow, itemFont, blackBrush);
                        col.offsetY_ItemRect(itemHeight);
					}
				}
                if (part.GroupFooter)
                {
                    RectangleF groupFooterRect = new RectangleF(
                        columns[0].X, columns[0].getItemRect().Y, getRightEnd() - getLeftEnd(), groupFooterHeight);
                    drawGroupFooter(g, part.GroupId, groupFooterRect);
                }
			}
		}

		protected virtual void drawMasterGroupHeader(Graphics g, int groupId, RectangleF groupHeaderRect)
		{
		
		}

		protected virtual void drawGroupHeader(Graphics g, int groupId, RectangleF groupHeaderRect)
		{
		
		}

        protected virtual void drawGroupFooter(Graphics g, int groupId, RectangleF groupFooterRect)
        {

        }
    }

	public class ReportGroupPart
	{
		private int groupId;
		public int GroupId
		{
			get { return groupId; }
			set { groupId = value; }
		}

		private int pageNum;
		public int PageNum
		{
			get { return pageNum; }
			set { pageNum = value; }
		}

		private int recNum;
		public int RecNum
		{
			get { return recNum; }
			set { recNum = value; }
		}

		private int numItems;
		public int NumItems
		{
			get { return numItems; }
			set { numItems = value; }
		}

		private float y;
		public float Y
		{
			get { return y; }
			set { y = value; }
		}

		private bool groupHeader;
		public bool GroupHeader
		{
			get { return groupHeader; }
			set { groupHeader = value; }
		}

		private bool masterGroupHeader;
		public bool MasterGroupHeader
		{
			get { return masterGroupHeader; }
			set { masterGroupHeader = value; }
		}

        private bool groupFooter;
        public bool GroupFooter
        {
            get { return groupFooter; }
            set { groupFooter = value; }
        }

        public ReportGroupPart(int pageNum, int groupId, int recNum, int numItems, 
			float y, bool groupHeader, bool masterGroupHeader, bool groupFooter)
		{
			this.pageNum = pageNum;
			this.groupId = groupId;
			this.recNum = recNum;
			this.numItems = numItems;
			this.y = y;
			this.groupHeader = groupHeader;
			this.masterGroupHeader = masterGroupHeader;
            this.groupFooter = groupFooter;
		}

	}

	public class ReportText
	{
		public ReportText()
		{
			
		}

		public ReportText(string text, Font font, Brush brush, RectangleF rect, StringFormat format)
		{
			this.text = text;
			this.font = font;
			this.brush = brush;
			this.rect = rect;
			this.format = format;
		}

		private string text;
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		private Font font;
		public Font Font
		{
			get { return font; }
			set { font = value; }
		}

		private Brush brush;
		public Brush Brush
		{
			get { return brush; }
			set { brush = value; }
		}

		private RectangleF rect;
		public RectangleF Rect
		{
			get { return rect; }
			set { rect = value; }
		}

		private StringFormat format;
		public StringFormat Format
		{
			get { return format; }
			set { format = value; }
		}

		public void draw(Graphics g)
		{
			g.DrawString(text, font, brush, rect, format); 
		}
	}
}
