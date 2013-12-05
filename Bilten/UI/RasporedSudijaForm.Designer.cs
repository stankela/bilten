namespace Bilten.UI
{
    partial class RasporedSudijaForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.spravaGridGroupUserControl1 = new Bilten.UI.SpravaGridGroupUserControl();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnPromeniRasporedSudija = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPrikaziKlub = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPrikaziDrzavu = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStampaj = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 49);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(820, 319);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.spravaGridGroupUserControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(812, 293);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // spravaGridGroupUserControl1
            // 
            this.spravaGridGroupUserControl1.Location = new System.Drawing.Point(6, 6);
            this.spravaGridGroupUserControl1.Name = "spravaGridGroupUserControl1";
            this.spravaGridGroupUserControl1.Size = new System.Drawing.Size(461, 281);
            this.spravaGridGroupUserControl1.TabIndex = 0;
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(124, 12);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Promeni";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(451, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Zatvori";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(16, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(92, 23);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "Nov raspored";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(214, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(94, 23);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Brisi raspored";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnPromeniRasporedSudija,
            this.mnPrikaziKlub,
            this.mnPrikaziDrzavu});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(201, 70);
            // 
            // mnPromeniRasporedSudija
            // 
            this.mnPromeniRasporedSudija.Name = "mnPromeniRasporedSudija";
            this.mnPromeniRasporedSudija.Size = new System.Drawing.Size(200, 22);
            this.mnPromeniRasporedSudija.Text = "Promeni raspored sudija";
            this.mnPromeniRasporedSudija.Click += new System.EventHandler(this.mnPromeniRasporedSudija_Click);
            // 
            // mnPrikaziKlub
            // 
            this.mnPrikaziKlub.Name = "mnPrikaziKlub";
            this.mnPrikaziKlub.Size = new System.Drawing.Size(200, 22);
            this.mnPrikaziKlub.Text = "Prikazi klub";
            this.mnPrikaziKlub.Click += new System.EventHandler(this.mnPrikaziKlub_Click);
            // 
            // mnPrikaziDrzavu
            // 
            this.mnPrikaziDrzavu.Name = "mnPrikaziDrzavu";
            this.mnPrikaziDrzavu.Size = new System.Drawing.Size(200, 22);
            this.mnPrikaziDrzavu.Text = "Prikazi drzavu";
            this.mnPrikaziDrzavu.Click += new System.EventHandler(this.mnPrikaziDrzavu_Click);
            // 
            // btnStampaj
            // 
            this.btnStampaj.Location = new System.Drawing.Point(329, 12);
            this.btnStampaj.Name = "btnStampaj";
            this.btnStampaj.Size = new System.Drawing.Size(75, 23);
            this.btnStampaj.TabIndex = 5;
            this.btnStampaj.Text = "Stampaj";
            this.btnStampaj.UseVisualStyleBackColor = true;
            this.btnStampaj.Click += new System.EventHandler(this.btnStampaj_Click);
            // 
            // RasporedSudijaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 380);
            this.Controls.Add(this.btnStampaj);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnClose);
            this.Name = "RasporedSudijaForm";
            this.Text = "RasporedSudijaForm";
            this.Load += new System.EventHandler(this.RasporedSudijaForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnClose;
        private SpravaGridGroupUserControl spravaGridGroupUserControl1;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnPrikaziKlub;
        private System.Windows.Forms.ToolStripMenuItem mnPrikaziDrzavu;
        private System.Windows.Forms.ToolStripMenuItem mnPromeniRasporedSudija;
        private System.Windows.Forms.Button btnStampaj;
    }
}