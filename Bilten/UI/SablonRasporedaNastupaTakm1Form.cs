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
    public partial class SablonRasporedaNastupaTakm1Form : EntityDetailForm
    {
        private RasporedNastupa rasporedNastupa;
        private byte grupa;

        public SablonRasporedaNastupaTakm1Form(Nullable<int> sablonId,
            RasporedNastupa rasporedNastupa, byte grupa)
        {
            InitializeComponent();
            this.rasporedNastupa = rasporedNastupa;
            this.grupa = grupa;
            initialize(sablonId, true);
        }

        protected override DomainObject createNewEntity()
        {
            SablonRasporedaNastupaTakm1 result = new SablonRasporedaNastupaTakm1();
            result.RasporedNastupa = rasporedNastupa;
            result.Grupa = grupa;
            return result;
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Sablon rasporeda nastupa";

            initializeGrid();
            updateUI();
        }

        protected void initializeGrid()
        {
            dataGridViewUserControl1.DataGridView.SelectionMode = 
                DataGridViewSelectionMode.CellSelect;
            dataGridViewUserControl1.DataGridView.MultiSelect = false;

            string[] sprave = Sprave.getSpraveNazivi(((SablonRasporedaNastupaTakm1)entity).RasporedNastupa.Pol);

            foreach (string sprava in sprave)
            {
                dataGridViewUserControl1.AddColumn(sprava, String.Empty, 100);
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SablonRasporedaNastupaTakm1 sablon = (SablonRasporedaNastupaTakm1)entity;
            SablonRasporedaNastupaTakm1ItemForm f = 
                new SablonRasporedaNastupaTakm1ItemForm(sablon);
            if (f.ShowDialog() == DialogResult.OK)
            {
                SablonRasporedaNastupaTakm1Item item = 
                    (SablonRasporedaNastupaTakm1Item)f.Entity;
                sablon.addItem(item);
                updateUI();
            }
        }

        private void updateUI()
        {
            dataGridViewUserControl1.DataGridView.Rows.Clear();
            
            SablonRasporedaNastupaTakm1 sablon = (SablonRasporedaNastupaTakm1)entity;
            byte max = sablon.getMaxRedBroj();
            if (max == 0)
                max = 1;
            dataGridViewUserControl1.DataGridView.Rows.Add(max);

            foreach (SablonRasporedaNastupaTakm1Item item in sablon.Items)
            {
                updateCell(item);
            }
        }

        private void updateCell(SablonRasporedaNastupaTakm1Item item)
        {
            SablonRasporedaNastupaTakm1 sablon = (SablonRasporedaNastupaTakm1)entity;
            DataGridViewRow row = 
                dataGridViewUserControl1.DataGridView.Rows[item.RedBroj - 1];
            int colIndex = Sprave.indexOf(item.Sprava, sablon.RasporedNastupa.Pol);
            DataGridViewCell cell = row.Cells[colIndex];
            cell.Value = item.KodBrojUcesnika;

        }

        protected override DomainObject getEntityById(int id)
        {
            IList<SablonRasporedaNastupaTakm1> result = 
                dataContext.ExecuteNamedQuery<SablonRasporedaNastupaTakm1>(
                "FindSablonRasporedaNastupaById",
                new string[] { "sablonId" },
                new object[] { id });
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            updateUI();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SablonRasporedaNastupaTakm1Item selItem = getSelectedItem();
            if (selItem == null)
                return;
            string msgFmt = "Da li zelite da izbrisete ekipu '{0} ({1})'?";
            if (!MessageDialogs.queryConfirmation(String.Format(
                msgFmt, selItem.Ekipa.Naziv, selItem.Ekipa.Kod), this.Text))
                return;

            SablonRasporedaNastupaTakm1 sablon = (SablonRasporedaNastupaTakm1)entity;
            sablon.removeItem(selItem);
            updateUI();
        }

        private SablonRasporedaNastupaTakm1Item getSelectedItem()
        {
            DataGridViewSelectedCellCollection selCells =
                dataGridViewUserControl1.DataGridView.SelectedCells;
            if (selCells.Count == 0)
                return null;
            DataGridViewCell selCell = selCells[0];

            SablonRasporedaNastupaTakm1 sablon = (SablonRasporedaNastupaTakm1)entity;
            Sprava[] sprave = Sprave.getSprave(sablon.RasporedNastupa.Pol);

            return sablon.getItem(sprave[selCell.ColumnIndex], selCell.RowIndex + 1);
        }

    }
}