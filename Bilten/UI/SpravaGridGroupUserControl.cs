using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;

namespace Bilten.UI
{
    public partial class SpravaGridGroupUserControl : UserControl
    {
        public event EventHandler<SpravaGridRightClickEventArgs> SpravaGridRightClick;
        
        public SpravaGridGroupUserControl()
        {
            InitializeComponent();
        }

        public SpravaGridUserControl[] SpravaGridUserControls
        {
            get
            {
                List<SpravaGridUserControl> result = new List<SpravaGridUserControl>();
                foreach (Control c in this.Controls)
                {
                    SpravaGridUserControl c2 = c as SpravaGridUserControl;
                    if (c2 != null)
                        result.Add(c2);
                }
                return result.ToArray();
            }
        }

        public SpravaGridUserControl this[Sprava sprava]
        {
            get
            {
                foreach (SpravaGridUserControl c in this.SpravaGridUserControls)
                {
                    if (c.Sprava == sprava)
                        return c;
                }
                return null;
            }
        }

        public void init(Sprava[] sprave)
        {
            // Najpre izbaci sve kontrole koje su dodate u prethodnom pozivu funkcije init, osim prve.
            List<Control> controls = new List<Control>();
            foreach (Control c in this.Controls)
            {
                controls.Add(c);
            }
            foreach (Control c in controls)
            {
                SpravaGridUserControl c2 = c as SpravaGridUserControl;
                if (c2 != null && c2 != spravaGridUserControl1)
                    this.Controls.Remove(c2);
            }

            int width = spravaGridUserControl1.Width;
            int height = spravaGridUserControl1.Height;
            int xOffset = 50;
            int yOffset = 25;

            // xOffset i yOffset je razmak izmedju sprava. razmak pre prve sprave i posle poslednje sprave je nula.
            this.Width = 2 * width + xOffset;
            int brojSprava = sprave.Length;
            this.Height = ((brojSprava+1) / 2) * height + ((brojSprava+1) / 2 - 1) * yOffset;

            int tabIndex = spravaGridUserControl1.TabIndex;
            for (int i = 0; i < brojSprava; i++)
            {
                Sprava sprava = sprave[i];
                if (i == 0)
                {
                    // Za svaki slucaj najpre uklanjam event handlere (za slucaj da je metod init vec pozivan),
                    // mada predpostavljam da cak i da se operacija += pozove dvaput uzastopce sa istim handlerom
                    // da ce samo jedan handler biti dodat.
                    spravaGridUserControl1.SpravaGridMouseDown -= new EventHandler<SpravaGridMouseDownEventArgs>(spravaGrid_MouseDown);
                    spravaGridUserControl1.SpravaGridMouseUp -= new EventHandler<SpravaGridMouseUpEventArgs>(spravaGrid_MouseUp);
                    
                    spravaGridUserControl1.SpravaGridMouseDown += new EventHandler<SpravaGridMouseDownEventArgs>(spravaGrid_MouseDown);
                    spravaGridUserControl1.SpravaGridMouseUp += new EventHandler<SpravaGridMouseUpEventArgs>(spravaGrid_MouseUp);
                    spravaGridUserControl1.init(sprava);
                    continue;
                }

                int row = i / 2;
                int col = i % 2;
                SpravaGridUserControl userControl = new SpravaGridUserControl();
                userControl.Location =
                    new Point(col * (width + xOffset), row * (height + yOffset));
                userControl.Size = new Size(width, height);
                userControl.TabIndex = tabIndex + i;
                userControl.SpravaGridMouseDown += new EventHandler<SpravaGridMouseDownEventArgs>(spravaGrid_MouseDown);
                userControl.SpravaGridMouseUp += new EventHandler<SpravaGridMouseUpEventArgs>(spravaGrid_MouseUp);
                userControl.init(sprava);
                this.Controls.Add(userControl);
            }
        }

        private void spravaGrid_MouseDown(object sender, SpravaGridMouseDownEventArgs e)
        {
            foreach (SpravaGridUserControl c in SpravaGridUserControls)
            {
                if (c.Sprava != e.Sprava)
                    c.clearSelection();
            }
        }

        private void spravaGrid_MouseUp(object sender, SpravaGridMouseUpEventArgs e)
        {
            if (e.MouseEventArgs.Button == MouseButtons.Right)
                OnSpravaGridRightClick(new SpravaGridRightClickEventArgs(e.Sprava, e.MouseEventArgs));
        }

        protected virtual void OnSpravaGridRightClick(SpravaGridRightClickEventArgs e)
        {
            // Save the delegate field in a temporary field for thread safety
            EventHandler<SpravaGridRightClickEventArgs> temp = SpravaGridRightClick;

            if (temp != null)
                temp(this, e);
        }

        public void clearSelection()
        {
            foreach (SpravaGridUserControl c in SpravaGridUserControls)
                c.clearSelection();
        }

        public Sprava SelectedSprava
        {
            get
            {
                foreach (SpravaGridUserControl c in SpravaGridUserControls)
                {
                    if (c.DataGridViewUserControl.DataGridView.SelectedRows.Count > 0)
                        return c.Sprava;
                }
                return Sprava.Undefined;
            }
        }
    }
}
