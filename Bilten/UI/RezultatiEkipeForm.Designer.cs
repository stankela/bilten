namespace Bilten.UI
{
    partial class RezultatiEkipeForm
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
            this.components = new System.ComponentModel.Container();
            this.cmbTakmicenje = new System.Windows.Forms.ComboBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnZatvori = new System.Windows.Forms.Button();
            this.dataGridViewUserControl2 = new Bilten.UI.DataGridViewUserControl();
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.prikaziKlubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prikaziDrzavuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbTakmicenje
            // 
            this.cmbTakmicenje.FormattingEnabled = true;
            this.cmbTakmicenje.Location = new System.Drawing.Point(12, 12);
            this.cmbTakmicenje.Name = "cmbTakmicenje";
            this.cmbTakmicenje.Size = new System.Drawing.Size(310, 21);
            this.cmbTakmicenje.TabIndex = 0;
            this.cmbTakmicenje.DropDownClosed += new System.EventHandler(this.cmbTakmicenje_DropDownClosed);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(344, 12);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "Stampaj";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnZatvori
            // 
            this.btnZatvori.Location = new System.Drawing.Point(446, 12);
            this.btnZatvori.Name = "btnZatvori";
            this.btnZatvori.Size = new System.Drawing.Size(75, 23);
            this.btnZatvori.TabIndex = 4;
            this.btnZatvori.Text = "Zatvori";
            this.btnZatvori.UseVisualStyleBackColor = true;
            this.btnZatvori.Click += new System.EventHandler(this.btnZatvori_Click);
            // 
            // dataGridViewUserControl2
            // 
            this.dataGridViewUserControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl2.ColumnHeaderSorting = true;
            this.dataGridViewUserControl2.Location = new System.Drawing.Point(12, 218);
            this.dataGridViewUserControl2.Name = "dataGridViewUserControl2";
            this.dataGridViewUserControl2.Size = new System.Drawing.Size(652, 135);
            this.dataGridViewUserControl2.TabIndex = 2;
            // 
            // dataGridViewUserControl1
            // 
            this.dataGridViewUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl1.ColumnHeaderSorting = true;
            this.dataGridViewUserControl1.Location = new System.Drawing.Point(12, 53);
            this.dataGridViewUserControl1.Name = "dataGridViewUserControl1";
            this.dataGridViewUserControl1.Size = new System.Drawing.Size(652, 138);
            this.dataGridViewUserControl1.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prikaziKlubToolStripMenuItem,
            this.prikaziDrzavuToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 70);
            // 
            // prikaziKlubToolStripMenuItem
            // 
            this.prikaziKlubToolStripMenuItem.Name = "prikaziKlubToolStripMenuItem";
            this.prikaziKlubToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.prikaziKlubToolStripMenuItem.Text = "Prikazi klub";
            this.prikaziKlubToolStripMenuItem.Click += new System.EventHandler(this.prikaziKlubToolStripMenuItem_Click);
            // 
            // prikaziDrzavuToolStripMenuItem
            // 
            this.prikaziDrzavuToolStripMenuItem.Name = "prikaziDrzavuToolStripMenuItem";
            this.prikaziDrzavuToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.prikaziDrzavuToolStripMenuItem.Text = "Prikazi drzavu";
            this.prikaziDrzavuToolStripMenuItem.Click += new System.EventHandler(this.prikaziDrzavuToolStripMenuItem_Click);
            // 
            // RezultatiEkipeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 365);
            this.Controls.Add(this.btnZatvori);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.dataGridViewUserControl2);
            this.Controls.Add(this.dataGridViewUserControl1);
            this.Controls.Add(this.cmbTakmicenje);
            this.Name = "RezultatiEkipeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RezultatiEkipeForm";
            this.Shown += new System.EventHandler(this.RezultatiEkipeForm_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTakmicenje;
        private DataGridViewUserControl dataGridViewUserControl1;
        private DataGridViewUserControl dataGridViewUserControl2;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnZatvori;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem prikaziKlubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prikaziDrzavuToolStripMenuItem;
    }
}