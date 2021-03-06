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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.prikaziKlubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prikaziDrzavuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dodajPenalizacijuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnSpraveKojeSeBoduju = new System.Windows.Forms.ToolStripMenuItem();
            this.btnIzracunaj = new System.Windows.Forms.Button();
            this.dataGridViewUserControl2 = new Bilten.UI.DataGridViewUserControl();
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
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
            this.btnZatvori.Location = new System.Drawing.Point(532, 12);
            this.btnZatvori.Name = "btnZatvori";
            this.btnZatvori.Size = new System.Drawing.Size(75, 23);
            this.btnZatvori.TabIndex = 4;
            this.btnZatvori.Text = "Zatvori";
            this.btnZatvori.UseVisualStyleBackColor = true;
            this.btnZatvori.Click += new System.EventHandler(this.btnZatvori_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prikaziKlubToolStripMenuItem,
            this.prikaziDrzavuToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(147, 48);
            // 
            // prikaziKlubToolStripMenuItem
            // 
            this.prikaziKlubToolStripMenuItem.Name = "prikaziKlubToolStripMenuItem";
            this.prikaziKlubToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.prikaziKlubToolStripMenuItem.Text = "Prikazi klub";
            this.prikaziKlubToolStripMenuItem.Click += new System.EventHandler(this.prikaziKlubToolStripMenuItem_Click);
            // 
            // prikaziDrzavuToolStripMenuItem
            // 
            this.prikaziDrzavuToolStripMenuItem.Name = "prikaziDrzavuToolStripMenuItem";
            this.prikaziDrzavuToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.prikaziDrzavuToolStripMenuItem.Text = "Prikazi drzavu";
            this.prikaziDrzavuToolStripMenuItem.Click += new System.EventHandler(this.prikaziDrzavuToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dodajPenalizacijuToolStripMenuItem,
            this.mnSpraveKojeSeBoduju});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(190, 70);
            // 
            // dodajPenalizacijuToolStripMenuItem
            // 
            this.dodajPenalizacijuToolStripMenuItem.Name = "dodajPenalizacijuToolStripMenuItem";
            this.dodajPenalizacijuToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.dodajPenalizacijuToolStripMenuItem.Text = "Penalizacija";
            this.dodajPenalizacijuToolStripMenuItem.Click += new System.EventHandler(this.dodajPenalizacijuToolStripMenuItem_Click);
            // 
            // mnSpraveKojeSeBoduju
            // 
            this.mnSpraveKojeSeBoduju.Name = "mnSpraveKojeSeBoduju";
            this.mnSpraveKojeSeBoduju.Size = new System.Drawing.Size(189, 22);
            this.mnSpraveKojeSeBoduju.Text = "Sprave koje se boduju";
            this.mnSpraveKojeSeBoduju.Click += new System.EventHandler(this.mnSpraveKojeSeBoduju_Click);
            // 
            // btnIzracunaj
            // 
            this.btnIzracunaj.Location = new System.Drawing.Point(438, 12);
            this.btnIzracunaj.Name = "btnIzracunaj";
            this.btnIzracunaj.Size = new System.Drawing.Size(75, 23);
            this.btnIzracunaj.TabIndex = 5;
            this.btnIzracunaj.Text = "Izracunaj";
            this.btnIzracunaj.UseVisualStyleBackColor = true;
            this.btnIzracunaj.Click += new System.EventHandler(this.btnIzracunaj_Click);
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
            // RezultatiEkipeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 365);
            this.Controls.Add(this.btnIzracunaj);
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
            this.contextMenuStrip2.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem dodajPenalizacijuToolStripMenuItem;
        private System.Windows.Forms.Button btnIzracunaj;
        private System.Windows.Forms.ToolStripMenuItem mnSpraveKojeSeBoduju;
    }
}