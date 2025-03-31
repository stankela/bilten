//---------------------------------------------
// PictureBoxPlus.cs © 2001 by Charles Petzold
//---------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bilten.UI
{
	class PictureBoxPlus: PictureBox
	{
        // PictureBox that can scale image isotropically

		bool noDistort = false;

		public bool NoDistort
		{
			get { return noDistort; }
			set 
			{
				noDistort = value;
				Invalidate();
			}
		}
    
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Image !=  null && NoDistort && SizeMode == PictureBoxSizeMode.StretchImage)
				scaleImageIsotropically(e.Graphics, Image, ClientRectangle);
			else
				base.OnPaint(e);
		}

		public static void scaleImageIsotropically(Graphics g, Image image, RectangleF rect)
		{
			SizeF sizef = new SizeF(image.Width / image.HorizontalResolution,
									image.Height / image.VerticalResolution);

			float fScale = Math.Min(rect.Width  / sizef.Width,
									rect.Height / sizef.Height);

			sizef.Width  *= fScale;
			sizef.Height *= fScale;
          
			g.DrawImage(image, rect.X + (rect.Width  - sizef.Width ) / 2,
								  rect.Y + (rect.Height - sizef.Height) / 2,
								  sizef.Width, sizef.Height);
		}
	}
}
