using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class FilterForm : Form
    {
        protected object filterObject;
        public object FilterObject
        {
            get { return filterObject; }
        }

        public event EventHandler Filter;

        public FilterForm()
        {
            InitializeComponent();
            this.TopMost = true;
            this.Text = "Filtriraj";
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            OnFilter(EventArgs.Empty);
        }

        protected virtual void OnFilter(EventArgs e)
        {
            EventHandler tmp = Filter;
            if (tmp != null)
                Filter(this, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            OnResetFilter();
        }

        protected virtual void OnResetFilter()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}