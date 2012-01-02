using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class EtchedLine : UserControl
    {
        public EtchedLine()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = this.ClientRectangle;

            Pen lightPen = new Pen(this.LightColor, 1.0F);
            Pen darkPen = new Pen(this.DarkColor, 1.0F);

            if (this.Dock == DockStyle.Top)
            {
                int y0 = rect.Top;
                int y1 = rect.Top + 1;

                g.DrawLine(darkPen, rect.Left, y0, rect.Right, y0);
                g.DrawLine(lightPen, rect.Left, y1, rect.Right, y1);
            }
            else if (this.Dock == DockStyle.Bottom)
            {
                int y0 = rect.Bottom - 2;
                int y1 = rect.Bottom - 1;

                g.DrawLine(darkPen, rect.Left, y0, rect.Right, y0);
                g.DrawLine(lightPen, rect.Left, y1, rect.Right, y1);
            }

            base.OnPaint(e);
        }

        Color _lightColor = SystemColors.ControlLightLight;
        Color _darkColor = SystemColors.ControlDark;

        [Category("Appearance")]
        public Color LightColor
        {
            get { return _lightColor; }
            set { _lightColor = value; }
        }

        [Category("Appearance")]
        public Color DarkColor
        {
            get { return _darkColor; }
            set { _darkColor = value; }
        }
    }
}
