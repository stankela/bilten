using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class CheckListForm : Form
    {
        private string noSelectMsg;
        private bool shouldSelect;
        public List<int> CheckedIndices = new List<int>();

        public CheckListForm(IList<string> items, IList<int> checkedItems, string header, string caption,
            bool shouldSelect, string noSelectMsg, bool showCancel)
        {
            InitializeComponent();
            Text = caption;
            lblHeader.Text = header;
            this.shouldSelect = shouldSelect;
            this.noSelectMsg = noSelectMsg;

            checkedListBox1.CheckOnClick = true;
            checkedListBox1.Items.Clear();

            for (int i = 0; i < items.Count; ++i)
                checkedListBox1.Items.Add(items[i], checkedItems.Contains(i));

            lblHeader.Left = checkedListBox1.Left;

            int clientWidth = lblHeader.Right + lblHeader.Left;

            Graphics graphics = checkedListBox1.CreateGraphics();
            float maxItemWidth = 0;
            for (int i = 0; i < items.Count; ++i)
            {
                float width = graphics.MeasureString(items[i], checkedListBox1.Font).Width;
                if (width > maxItemWidth)
                    maxItemWidth = width;
            }
            graphics.Dispose();

            int checkedListBoxClientWidth = 2 * checkedListBox1.Left + (int)Math.Ceiling(maxItemWidth)
                + 10/*dimenzije kucice za potvrdu*/;
            if (checkedListBoxClientWidth > clientWidth)
                clientWidth = checkedListBoxClientWidth;

            int minClientWidth = 2 * checkedListBox1.Left + btnOk.Width + btnCancel.Width + (btnCancel.Left - btnOk.Right);
            if (clientWidth < minClientWidth)
                clientWidth = minClientWidth;
            if (clientWidth > Screen.PrimaryScreen.WorkingArea.Width - 20)
                clientWidth = Screen.PrimaryScreen.WorkingArea.Width - 20;

            int checkedListBoxHeigth = checkedListBox1.GetItemRectangle(0).Height * (checkedListBox1.Items.Count + 2);
            int bottomMargin = ClientSize.Height - checkedListBox1.Bottom;
            int clientHeigth = checkedListBox1.Top + checkedListBoxHeigth + bottomMargin;
            if (clientHeigth > Screen.PrimaryScreen.WorkingArea.Height - 40)
                clientHeigth = Screen.PrimaryScreen.WorkingArea.Height - 40;

            ClientSize = new Size(clientWidth, clientHeigth);

            if (!showCancel)
            {
                btnCancel.Enabled = false;
                btnCancel.Visible = false;
                btnOk.Location = btnCancel.Location;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (shouldSelect && checkedListBox1.CheckedItems.Count == 0)
            {
                MessageDialogs.showMessage(noSelectMsg, Text);
                DialogResult = DialogResult.None;
                return;
            }

            foreach (int i in checkedListBox1.CheckedIndices)
                CheckedIndices.Add(i);
        }
    }
}
