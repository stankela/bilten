namespace Bilten.UI
{
    partial class RezultatiSpravaForm
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
            this.cmbSprava = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.prikaziKlubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prikaziDrzavuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnQ = new System.Windows.Forms.ToolStripMenuItem();
            this.mnR = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPrazno = new System.Windows.Forms.ToolStripMenuItem();
            this.btnIzracunaj = new System.Windows.Forms.Button();
            this.btnStampajKvalifikante = new System.Windows.Forms.Button();
            this.spravaGridUserControl1 = new Bilten.UI.SpravaGridUserControl();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbTakmicenje
            // 
            this.cmbTakmicenje.FormattingEnabled = true;
            this.cmbTakmicenje.Location = new System.Drawing.Point(12, 12);
            this.cmbTakmicenje.Name = "cmbTakmicenje";
            this.cmbTakmicenje.Size = new System.Drawing.Size(286, 21);
            this.cmbTakmicenje.TabIndex = 0;
            this.cmbTakmicenje.DropDownClosed += new System.EventHandler(this.cmbTakmicenje_DropDownClosed);
            // 
            // cmbSprava
            // 
            this.cmbSprava.FormattingEnabled = true;
            this.cmbSprava.Location = new System.Drawing.Point(316, 12);
            this.cmbSprava.Name = "cmbSprava";
            this.cmbSprava.Size = new System.Drawing.Size(121, 21);
            this.cmbSprava.TabIndex = 1;
            this.cmbSprava.DropDownClosed += new System.EventHandler(this.cmbSprava_DropDownClosed);
            this.cmbSprava.DropDown += new System.EventHandler(this.cmbSprava_DropDown);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(539, 13);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(620, 13);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(458, 13);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "Stampaj";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(721, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
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
            this.mnPrazno});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(152, 114);
            // 
            // prikaziKlubToolStripMenuItem
            // 
            this.prikaziKlubToolStripMenuItem.Name = "prikaziKlubToolStripMenuItem";
            this.prikaziKlubToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.prikaziKlubToolStripMenuItem.Text = "Prikazi klub";
            this.prikaziKlubToolStripMenuItem.Click += new System.EventHandler(this.prikaziKlubToolStripMenuItem_Click);
            // 
            // prikaziDrzavuToolStripMenuItem
            // 
            this.prikaziDrzavuToolStripMenuItem.Name = "prikaziDrzavuToolStripMenuItem";
            this.prikaziDrzavuToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.prikaziDrzavuToolStripMenuItem.Text = "Prikazi drzavu";
            this.prikaziDrzavuToolStripMenuItem.Click += new System.EventHandler(this.prikaziDrzavuToolStripMenuItem_Click);
            // 
            // mnQ
            // 
            this.mnQ.Name = "mnQ";
            this.mnQ.Size = new System.Drawing.Size(151, 22);
            this.mnQ.Text = "Q";
            this.mnQ.Click += new System.EventHandler(this.mnQ_Click);
            // 
            // mnR
            // 
            this.mnR.Name = "mnR";
            this.mnR.Size = new System.Drawing.Size(151, 22);
            this.mnR.Text = "R";
            this.mnR.Click += new System.EventHandler(this.mnR_Click);
            // 
            // mnPrazno
            // 
            this.mnPrazno.Name = "mnPrazno";
            this.mnPrazno.Size = new System.Drawing.Size(151, 22);
            this.mnPrazno.Text = "Prazno";
            this.mnPrazno.Click += new System.EventHandler(this.mnPrazno_Click);
            // 
            // btnIzracunaj
            // 
            this.btnIzracunaj.Location = new System.Drawing.Point(806, 13);
            this.btnIzracunaj.Name = "btnIzracunaj";
            this.btnIzracunaj.Size = new System.Drawing.Size(75, 23);
            this.btnIzracunaj.TabIndex = 7;
            this.btnIzracunaj.Text = "Izracunaj";
            this.btnIzracunaj.UseVisualStyleBackColor = true;
            this.btnIzracunaj.Click += new System.EventHandler(this.btnIzracunaj_Click);
            // 
            // btnStampajKvalifikante
            // 
            this.btnStampajKvalifikante.Location = new System.Drawing.Point(721, 42);
            this.btnStampajKvalifikante.Name = "btnStampajKvalifikante";
            this.btnStampajKvalifikante.Size = new System.Drawing.Size(132, 23);
            this.btnStampajKvalifikante.TabIndex = 8;
            this.btnStampajKvalifikante.Text = "Stampaj kvalifikante";
            this.btnStampajKvalifikante.UseVisualStyleBackColor = true;
            this.btnStampajKvalifikante.Click += new System.EventHandler(this.btnStampajKvalifikante_Click);
            // 
            // spravaGridUserControl1
            // 
            this.spravaGridUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.spravaGridUserControl1.Location = new System.Drawing.Point(12, 56);
            this.spravaGridUserControl1.Name = "spravaGridUserControl1";
            this.spravaGridUserControl1.Size = new System.Drawing.Size(1002, 295);
            this.spravaGridUserControl1.TabIndex = 2;
            // 
            // RezultatiSpravaForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1026, 363);
            this.Controls.Add(this.btnStampajKvalifikante);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnIzracunaj);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.spravaGridUserControl1);
            this.Controls.Add(this.cmbSprava);
            this.Controls.Add(this.cmbTakmicenje);
            this.Name = "RezultatiSpravaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RezultatiSpravaForm";
            this.Shown += new System.EventHandler(this.RezultatiSpravaForm_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTakmicenje;
        private System.Windows.Forms.ComboBox cmbSprava;
        private SpravaGridUserControl spravaGridUserControl1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnQ;
        private System.Windows.Forms.ToolStripMenuItem mnR;
        private System.Windows.Forms.ToolStripMenuItem mnPrazno;
        private System.Windows.Forms.Button btnIzracunaj;
        private System.Windows.Forms.ToolStripMenuItem prikaziKlubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prikaziDrzavuToolStripMenuItem;
        private System.Windows.Forms.Button btnStampajKvalifikante;
    }
}
