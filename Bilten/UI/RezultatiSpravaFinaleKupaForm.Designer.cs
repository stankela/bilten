namespace Bilten.UI
{
    partial class RezultatiSpravaFinaleKupaForm
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
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnQ = new System.Windows.Forms.ToolStripMenuItem();
            this.mnR = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPrazno = new System.Windows.Forms.ToolStripMenuItem();
            this.spravaGridUserControl1 = new Bilten.UI.SpravaGridUserControl();
            this.btnIzracunaj = new System.Windows.Forms.Button();
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
            this.btnClose.Location = new System.Drawing.Point(639, 13);
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
            this.mnQ,
            this.mnR,
            this.mnPrazno});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(111, 70);
            // 
            // mnQ
            // 
            this.mnQ.Name = "mnQ";
            this.mnQ.Size = new System.Drawing.Size(110, 22);
            this.mnQ.Text = "Q";
            this.mnQ.Click += new System.EventHandler(this.mnQ_Click);
            // 
            // mnR
            // 
            this.mnR.Name = "mnR";
            this.mnR.Size = new System.Drawing.Size(110, 22);
            this.mnR.Text = "R";
            this.mnR.Click += new System.EventHandler(this.mnR_Click);
            // 
            // mnPrazno
            // 
            this.mnPrazno.Name = "mnPrazno";
            this.mnPrazno.Size = new System.Drawing.Size(110, 22);
            this.mnPrazno.Text = "Prazno";
            this.mnPrazno.Click += new System.EventHandler(this.mnPrazno_Click);
            // 
            // spravaGridUserControl1
            // 
            this.spravaGridUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spravaGridUserControl1.Location = new System.Drawing.Point(12, 56);
            this.spravaGridUserControl1.Name = "spravaGridUserControl1";
            this.spravaGridUserControl1.Size = new System.Drawing.Size(822, 295);
            this.spravaGridUserControl1.TabIndex = 2;
            // 
            // btnIzracunaj
            // 
            this.btnIzracunaj.Location = new System.Drawing.Point(549, 13);
            this.btnIzracunaj.Name = "btnIzracunaj";
            this.btnIzracunaj.Size = new System.Drawing.Size(75, 23);
            this.btnIzracunaj.TabIndex = 7;
            this.btnIzracunaj.Text = "Izracunaj";
            this.btnIzracunaj.UseVisualStyleBackColor = true;
            this.btnIzracunaj.Click += new System.EventHandler(this.btnIzracunaj_Click);
            // 
            // RezultatiSpravaFinaleKupaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 363);
            this.Controls.Add(this.btnIzracunaj);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.spravaGridUserControl1);
            this.Controls.Add(this.cmbSprava);
            this.Controls.Add(this.cmbTakmicenje);
            this.Name = "RezultatiSpravaFinaleKupaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RezultatiSpravaFinaleKupaForm";
            this.Shown += new System.EventHandler(this.RezultatiSpravaFinaleKupaForm_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTakmicenje;
        private System.Windows.Forms.ComboBox cmbSprava;
        private SpravaGridUserControl spravaGridUserControl1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnQ;
        private System.Windows.Forms.ToolStripMenuItem mnR;
        private System.Windows.Forms.ToolStripMenuItem mnPrazno;
        private System.Windows.Forms.Button btnIzracunaj;
    }
}
