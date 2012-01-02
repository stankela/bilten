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
    public partial class FilterGimnasticarUcesnikForm : FilterForm
    {
        public FilterGimnasticarUcesnikForm(int takmicenjeId, Nullable<Gimnastika> gimnastika,
            TakmicarskaKategorija startKategorija)
        {
            InitializeComponent();
            filterGimnasticarUcesnikUserControl1.initialize(takmicenjeId, gimnastika,
                startKategorija);
        }

        protected override void OnFilter(EventArgs e)
        {
            GimnasticarUcesnikFilter filter = filterGimnasticarUcesnikUserControl1.getFilter();
            if (filter != null)
            {
                this.filterObject = filter;
                base.OnFilter(e);
            }
        }

        protected override void OnResetFilter()
        {
            filterGimnasticarUcesnikUserControl1.resetFilter();
        }
    }
}