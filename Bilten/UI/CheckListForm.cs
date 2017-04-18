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
        public List<int> CheckedIndices = new List<int>();

        public CheckListForm(IList<string> items, string caption, string noSelectMsg)
        {
            InitializeComponent();
            Text = caption;
            this.noSelectMsg = noSelectMsg;

            checkedListBox1.CheckOnClick = true;
            checkedListBox1.Items.Clear();

            foreach (string item in items)
                checkedListBox1.Items.Add(item, true);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count == 0)
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
