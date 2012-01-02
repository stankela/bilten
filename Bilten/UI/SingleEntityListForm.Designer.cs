using System.Windows.Forms;

namespace Bilten.UI
{
    partial class SingleEntityListForm<T>
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgwItemList = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgwItemList)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNewItem
            // 
            this.btnNewItem.Click += new System.EventHandler(this.btnNewItem_Click);
            // 
            // btnEditItem
            // 
            this.btnEditItem.Click += new System.EventHandler(this.btnEditItem_Click);
            // 
            // btnClose
            // 
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDeleteItem
            // 
            this.btnDeleteItem.Click += new System.EventHandler(this.btnDeleteItem_Click);
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.Click += new System.EventHandler(this.btnRefreshList_Click);
            // 
            // btnShowFirstPage
            // 
            this.btnShowFirstPage.Click += new System.EventHandler(this.btnShowFirstPage_Click);
            // 
            // btnShowLastPage
            // 
            this.btnShowLastPage.Click += new System.EventHandler(this.btnShowLastPage_Click);
            // 
            // btnShowNextPage
            // 
            this.btnShowNextPage.Click += new System.EventHandler(this.btnShowNextPage_Click);
            // 
            // btnShowPreviousPage
            // 
            this.btnShowPreviousPage.Click += new System.EventHandler(this.btnShowPreviousPage_Click);
            // 
            // dgwItemList
            // 
            this.dgwItemList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgwItemList.Location = new System.Drawing.Point(0, 0);
            this.dgwItemList.Name = "dgwItemList";
            this.dgwItemList.TabIndex = 0;
            this.dgwItemList.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgwItemList_ColumnHeaderMouseClick);
            // 
            // SingleEntityListForm
            // 
            this.pnlListPlaceholder.Controls.Add(dgwItemList);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "SingleEntityListForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgwItemList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        
        protected DataGridView dgwItemList;
    }
}
