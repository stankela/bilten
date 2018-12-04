namespace Bilten.UI
{
    partial class RezultatiUkupnoForm
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
            this.btnClose = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.prikaziKlubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prikaziDrzavuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnQ = new System.Windows.Forms.ToolStripMenuItem();
            this.mnR = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPrazno = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPromeniPoredakZaIsteOcene = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPenalizacija = new System.Windows.Forms.ToolStripMenuItem();
            this.btnIzracunaj = new System.Windows.Forms.Button();
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            this.btnStampajKvalifikante = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbTakmicenje
            // 
            this.cmbTakmicenje.FormattingEnabled = true;
            this.cmbTakmicenje.Location = new System.Drawing.Point(12, 12);
            this.cmbTakmicenje.Name = "cmbTakmicenje";
            this.cmbTakmicenje.Size = new System.Drawing.Size(343, 21);
            this.cmbTakmicenje.TabIndex = 1;
            this.cmbTakmicenje.DropDownClosed += new System.EventHandler(this.cmbTakmicenja_DropDownClosed);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(388, 10);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Stampaj";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(718, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Zatvori";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prikaziKlubToolStripMenuItem,
            this.prikaziDrzavuToolStripMenuItem,
            this.mnQ,
            this.mnR,
            this.mnPrazno,
            this.mnPromeniPoredakZaIsteOcene,
            this.mnPenalizacija});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(236, 158);
            // 
            // prikaziKlubToolStripMenuItem
            // 
            this.prikaziKlubToolStripMenuItem.Name = "prikaziKlubToolStripMenuItem";
            this.prikaziKlubToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.prikaziKlubToolStripMenuItem.Text = "Prikazi klub";
            this.prikaziKlubToolStripMenuItem.Click += new System.EventHandler(this.prikaziKlubToolStripMenuItem_Click);
            // 
            // prikaziDrzavuToolStripMenuItem
            // 
            this.prikaziDrzavuToolStripMenuItem.Name = "prikaziDrzavuToolStripMenuItem";
            this.prikaziDrzavuToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.prikaziDrzavuToolStripMenuItem.Text = "Prikazi drzavu";
            this.prikaziDrzavuToolStripMenuItem.Click += new System.EventHandler(this.prikaziDrzavuToolStripMenuItem_Click);
            // 
            // mnQ
            // 
            this.mnQ.Name = "mnQ";
            this.mnQ.Size = new System.Drawing.Size(235, 22);
            this.mnQ.Text = "Q";
            this.mnQ.Click += new System.EventHandler(this.mnQ_Click);
            // 
            // mnR
            // 
            this.mnR.Name = "mnR";
            this.mnR.Size = new System.Drawing.Size(235, 22);
            this.mnR.Text = "R";
            this.mnR.Click += new System.EventHandler(this.mnR_Click);
            // 
            // mnPrazno
            // 
            this.mnPrazno.Name = "mnPrazno";
            this.mnPrazno.Size = new System.Drawing.Size(235, 22);
            this.mnPrazno.Text = "Prazno";
            this.mnPrazno.Click += new System.EventHandler(this.mnPrazno_Click);
            // 
            // mnPromeniPoredakZaIsteOcene
            // 
            this.mnPromeniPoredakZaIsteOcene.Name = "mnPromeniPoredakZaIsteOcene";
            this.mnPromeniPoredakZaIsteOcene.Size = new System.Drawing.Size(235, 22);
            this.mnPromeniPoredakZaIsteOcene.Text = "Promeni poredak za iste ocene";
            this.mnPromeniPoredakZaIsteOcene.Click += new System.EventHandler(this.mnPromeniPoredakZaIsteOcene_Click);
            // 
            // mnPenalizacija
            // 
            this.mnPenalizacija.Name = "mnPenalizacija";
            this.mnPenalizacija.Size = new System.Drawing.Size(235, 22);
            this.mnPenalizacija.Text = "Penalizacija";
            this.mnPenalizacija.Click += new System.EventHandler(this.mnPenalizacija_Click);
            // 
            // btnIzracunaj
            // 
            this.btnIzracunaj.Location = new System.Drawing.Point(480, 10);
            this.btnIzracunaj.Name = "btnIzracunaj";
            this.btnIzracunaj.Size = new System.Drawing.Size(75, 23);
            this.btnIzracunaj.TabIndex = 4;
            this.btnIzracunaj.Text = "Izracunaj";
            this.btnIzracunaj.UseVisualStyleBackColor = true;
            this.btnIzracunaj.Click += new System.EventHandler(this.btnIzracunaj_Click);
            // 
            // dataGridViewUserControl1
            // 
            this.dataGridViewUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl1.ColumnHeaderSorting = true;
            this.dataGridViewUserControl1.Location = new System.Drawing.Point(12, 58);
            this.dataGridViewUserControl1.Name = "dataGridViewUserControl1";
            this.dataGridViewUserControl1.Size = new System.Drawing.Size(1049, 287);
            this.dataGridViewUserControl1.TabIndex = 0;
            // 
            // btnStampajKvalifikante
            // 
            this.btnStampajKvalifikante.Location = new System.Drawing.Point(571, 10);
            this.btnStampajKvalifikante.Name = "btnStampajKvalifikante";
            this.btnStampajKvalifikante.Size = new System.Drawing.Size(132, 23);
            this.btnStampajKvalifikante.TabIndex = 5;
            this.btnStampajKvalifikante.Text = "Stampaj kvalifikante";
            this.btnStampajKvalifikante.UseVisualStyleBackColor = true;
            this.btnStampajKvalifikante.Click += new System.EventHandler(this.btnStampajKvalifikante_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(872, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RezultatiUkupnoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 357);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnStampajKvalifikante);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnIzracunaj);
            this.Controls.Add(this.dataGridViewUserControl1);
            this.Controls.Add(this.cmbTakmicenje);
            this.Controls.Add(this.btnClose);
            this.Name = "RezultatiUkupnoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RezultatiUkupnoForm";
            this.Shown += new System.EventHandler(this.RezultatiUkupnoForm_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTakmicenje;
        private DataGridViewUserControl dataGridViewUserControl1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnQ;
        private System.Windows.Forms.ToolStripMenuItem mnR;
        private System.Windows.Forms.ToolStripMenuItem mnPrazno;
        private System.Windows.Forms.ToolStripMenuItem prikaziKlubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prikaziDrzavuToolStripMenuItem;
        private System.Windows.Forms.Button btnIzracunaj;
        private System.Windows.Forms.ToolStripMenuItem mnPromeniPoredakZaIsteOcene;
        private System.Windows.Forms.ToolStripMenuItem mnPenalizacija;
        private System.Windows.Forms.Button btnStampajKvalifikante;
        private System.Windows.Forms.Button button1;
    }
}