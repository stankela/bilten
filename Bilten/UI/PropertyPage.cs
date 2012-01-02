using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class PropertyPage : UserControl
    {
        public PropertyPage()
        {
            InitializeComponent();
        }

        #region Overridables
        public new virtual string Text
        {
            get { return this.GetType().Name; }
        }

        public virtual void OnSetActive()
        {
        }

        public virtual void OnApply()
        {
        }

        #endregion
    }
}
