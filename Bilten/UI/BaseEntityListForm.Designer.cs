using System.Windows.Forms;

namespace Bilten.UI
{
    partial class BaseEntityListForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnNewItem = new System.Windows.Forms.Button();
            this.btnPrintPreview = new System.Windows.Forms.Button();
            this.btnEditItem = new System.Windows.Forms.Button();
            this.btnPrintItem = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.btnDeleteItem = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnApplySort = new System.Windows.Forms.Button();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.btnShowHelp = new System.Windows.Forms.Button();
            this.btnDuplicate = new System.Windows.Forms.Button();
            this.pnlListPlaceholder = new System.Windows.Forms.Panel();
            this.pnlPager = new System.Windows.Forms.Panel();
            this.lblPageCounter = new System.Windows.Forms.Label();
            this.btnShowLastPage = new System.Windows.Forms.Button();
            this.btnShowNextPage = new System.Windows.Forms.Button();
            this.btnShowPreviousPage = new System.Windows.Forms.Button();
            this.btnShowFirstPage = new System.Windows.Forms.Button();
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            this.layoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlListPlaceholder.SuspendLayout();
            this.pnlPager.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutPanel
            // 
            this.layoutPanel.ColumnCount = 2;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutPanel.Controls.Add(this.panel1, 1, 0);
            this.layoutPanel.Controls.Add(this.panel2, 0, 1);
            this.layoutPanel.Controls.Add(this.pnlListPlaceholder, 0, 0);
            this.layoutPanel.Controls.Add(this.pnlPager, 0, 2);
            this.layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPanel.Location = new System.Drawing.Point(0, 0);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.RowCount = 3;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.layoutPanel.Size = new System.Drawing.Size(539, 371);
            this.layoutPanel.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnNewItem);
            this.panel1.Controls.Add(this.btnPrintPreview);
            this.panel1.Controls.Add(this.btnEditItem);
            this.panel1.Controls.Add(this.btnPrintItem);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnApplyFilter);
            this.panel1.Controls.Add(this.btnDeleteItem);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(442, 3);
            this.panel1.Name = "panel1";
            this.layoutPanel.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(94, 325);
            this.panel1.TabIndex = 0;
            // 
            // btnNewItem
            // 
            this.btnNewItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewItem.Location = new System.Drawing.Point(6, 3);
            this.btnNewItem.Name = "btnNewItem";
            this.btnNewItem.Size = new System.Drawing.Size(82, 28);
            this.btnNewItem.TabIndex = 0;
            this.btnNewItem.Text = "Dodaj";
            this.btnNewItem.UseVisualStyleBackColor = true;
            // 
            // btnPrintPreview
            // 
            this.btnPrintPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrintPreview.Location = new System.Drawing.Point(6, 285);
            this.btnPrintPreview.Name = "btnPrintPreview";
            this.btnPrintPreview.Size = new System.Drawing.Size(82, 28);
            this.btnPrintPreview.TabIndex = 6;
            this.btnPrintPreview.Text = "Preview";
            this.btnPrintPreview.UseVisualStyleBackColor = true;
            // 
            // btnEditItem
            // 
            this.btnEditItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditItem.Location = new System.Drawing.Point(6, 50);
            this.btnEditItem.Name = "btnEditItem";
            this.btnEditItem.Size = new System.Drawing.Size(82, 28);
            this.btnEditItem.TabIndex = 1;
            this.btnEditItem.Text = "Promeni";
            this.btnEditItem.UseVisualStyleBackColor = true;
            // 
            // btnPrintItem
            // 
            this.btnPrintItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrintItem.Location = new System.Drawing.Point(6, 238);
            this.btnPrintItem.Name = "btnPrintItem";
            this.btnPrintItem.Size = new System.Drawing.Size(82, 28);
            this.btnPrintItem.TabIndex = 5;
            this.btnPrintItem.Text = "Print";
            this.btnPrintItem.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(6, 191);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(82, 28);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Zatvori";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyFilter.Location = new System.Drawing.Point(6, 144);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(82, 28);
            this.btnApplyFilter.TabIndex = 3;
            this.btnApplyFilter.Text = "Filtriraj";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            // 
            // btnDeleteItem
            // 
            this.btnDeleteItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteItem.Location = new System.Drawing.Point(6, 97);
            this.btnDeleteItem.Name = "btnDeleteItem";
            this.btnDeleteItem.Size = new System.Drawing.Size(82, 28);
            this.btnDeleteItem.TabIndex = 2;
            this.btnDeleteItem.Text = "Brisi";
            this.btnDeleteItem.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnApplySort);
            this.panel2.Controls.Add(this.btnRefreshList);
            this.panel2.Controls.Add(this.btnShowHelp);
            this.panel2.Controls.Add(this.btnDuplicate);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 285);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(433, 43);
            this.panel2.TabIndex = 1;
            // 
            // btnApplySort
            // 
            this.btnApplySort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApplySort.Location = new System.Drawing.Point(311, 8);
            this.btnApplySort.Name = "btnApplySort";
            this.btnApplySort.Size = new System.Drawing.Size(82, 28);
            this.btnApplySort.TabIndex = 3;
            this.btnApplySort.Text = "Sort";
            this.btnApplySort.UseVisualStyleBackColor = true;
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefreshList.Location = new System.Drawing.Point(210, 8);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(82, 28);
            this.btnRefreshList.TabIndex = 4;
            this.btnRefreshList.Text = "Refresh";
            this.btnRefreshList.UseVisualStyleBackColor = true;
            // 
            // btnShowHelp
            // 
            this.btnShowHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowHelp.Location = new System.Drawing.Point(109, 8);
            this.btnShowHelp.Name = "btnShowHelp";
            this.btnShowHelp.Size = new System.Drawing.Size(82, 28);
            this.btnShowHelp.TabIndex = 1;
            this.btnShowHelp.Text = "Help";
            this.btnShowHelp.UseVisualStyleBackColor = true;
            // 
            // btnDuplicate
            // 
            this.btnDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDuplicate.Location = new System.Drawing.Point(8, 8);
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.Size = new System.Drawing.Size(82, 28);
            this.btnDuplicate.TabIndex = 0;
            this.btnDuplicate.Text = "Duplicate";
            this.btnDuplicate.UseVisualStyleBackColor = true;
            // 
            // pnlListPlaceholder
            // 
            this.pnlListPlaceholder.Controls.Add(this.dataGridViewUserControl1);
            this.pnlListPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlListPlaceholder.Location = new System.Drawing.Point(3, 3);
            this.pnlListPlaceholder.Name = "pnlListPlaceholder";
            this.pnlListPlaceholder.Size = new System.Drawing.Size(433, 276);
            this.pnlListPlaceholder.TabIndex = 2;
            // 
            // pnlPager
            // 
            this.layoutPanel.SetColumnSpan(this.pnlPager, 2);
            this.pnlPager.Controls.Add(this.lblPageCounter);
            this.pnlPager.Controls.Add(this.btnShowLastPage);
            this.pnlPager.Controls.Add(this.btnShowNextPage);
            this.pnlPager.Controls.Add(this.btnShowPreviousPage);
            this.pnlPager.Controls.Add(this.btnShowFirstPage);
            this.pnlPager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPager.Location = new System.Drawing.Point(3, 334);
            this.pnlPager.Name = "pnlPager";
            this.pnlPager.Size = new System.Drawing.Size(533, 34);
            this.pnlPager.TabIndex = 3;
            // 
            // lblPageCounter
            // 
            this.lblPageCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPageCounter.AutoSize = true;
            this.lblPageCounter.Location = new System.Drawing.Point(426, 12);
            this.lblPageCounter.Name = "lblPageCounter";
            this.lblPageCounter.Size = new System.Drawing.Size(35, 13);
            this.lblPageCounter.TabIndex = 4;
            this.lblPageCounter.Text = "label1";
            // 
            // btnShowLastPage
            // 
            this.btnShowLastPage.Location = new System.Drawing.Point(311, 3);
            this.btnShowLastPage.Name = "btnShowLastPage";
            this.btnShowLastPage.Size = new System.Drawing.Size(82, 28);
            this.btnShowLastPage.TabIndex = 3;
            this.btnShowLastPage.Text = ">|";
            this.btnShowLastPage.UseVisualStyleBackColor = true;
            // 
            // btnShowNextPage
            // 
            this.btnShowNextPage.Location = new System.Drawing.Point(210, 4);
            this.btnShowNextPage.Name = "btnShowNextPage";
            this.btnShowNextPage.Size = new System.Drawing.Size(82, 28);
            this.btnShowNextPage.TabIndex = 2;
            this.btnShowNextPage.Text = ">>";
            this.btnShowNextPage.UseVisualStyleBackColor = true;
            // 
            // btnShowPreviousPage
            // 
            this.btnShowPreviousPage.Location = new System.Drawing.Point(109, 4);
            this.btnShowPreviousPage.Name = "btnShowPreviousPage";
            this.btnShowPreviousPage.Size = new System.Drawing.Size(82, 28);
            this.btnShowPreviousPage.TabIndex = 1;
            this.btnShowPreviousPage.Text = "<<";
            this.btnShowPreviousPage.UseVisualStyleBackColor = true;
            // 
            // btnShowFirstPage
            // 
            this.btnShowFirstPage.Location = new System.Drawing.Point(8, 4);
            this.btnShowFirstPage.Name = "btnShowFirstPage";
            this.btnShowFirstPage.Size = new System.Drawing.Size(82, 28);
            this.btnShowFirstPage.TabIndex = 0;
            this.btnShowFirstPage.Text = "|<";
            this.btnShowFirstPage.UseVisualStyleBackColor = true;
            // 
            // dataGridViewUserControl1
            // 
            this.dataGridViewUserControl1.ColumnHeaderSorting = true;
            this.dataGridViewUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewUserControl1.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewUserControl1.Name = "dataGridViewUserControl1";
            this.dataGridViewUserControl1.Size = new System.Drawing.Size(433, 276);
            this.dataGridViewUserControl1.TabIndex = 0;
            // 
            // BaseEntityListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 371);
            this.Controls.Add(this.layoutPanel);
            this.MinimumSize = new System.Drawing.Size(547, 405);
            this.Name = "BaseEntityListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BaseEntityListForm";
            this.layoutPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlListPlaceholder.ResumeLayout(false);
            this.pnlPager.ResumeLayout(false);
            this.pnlPager.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        protected Button btnNewItem;
        protected Button btnEditItem;
        protected Button btnClose;
        protected Button btnDeleteItem;
        protected Button btnRefreshList;
        protected Button btnPrintItem;
        protected Button btnPrintPreview;
        protected Panel pnlPager;
        protected Button btnShowFirstPage;
        protected Button btnShowLastPage;
        protected Button btnShowNextPage;
        protected Button btnShowPreviousPage;
        protected Label lblPageCounter;
        protected Panel pnlListPlaceholder;
        protected TableLayoutPanel layoutPanel;
        protected Button btnApplySort;
        protected Button btnApplyFilter;
        protected Button btnShowHelp;
        protected Button btnDuplicate;
        protected DataGridViewUserControl dataGridViewUserControl1;
    }
}
