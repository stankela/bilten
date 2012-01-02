using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;

namespace Bilten.UI
{
    public partial class FilterGimnasticarForm : FilterForm
    {
        public FilterGimnasticarForm(Nullable<Gimnastika> gimnastika)
        {
            InitializeComponent();
            filterGimnasticarUserControl1.initialize(gimnastika);
        }

        protected override void OnFilter(EventArgs e)
        {
            GimnasticarFilter filter = filterGimnasticarUserControl1.getFilter();
            if (filter != null)
            {
                this.filterObject = filter;
                base.OnFilter(e);
            }
        }

        protected override void OnResetFilter()
        {
            filterGimnasticarUserControl1.resetFilter();
        }
    }
}

