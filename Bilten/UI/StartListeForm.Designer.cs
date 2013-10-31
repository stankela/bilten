namespace Bilten.UI
{
    partial class StartListeForm
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
            this.cmbGrupa = new System.Windows.Forms.ComboBox();
            this.cmbRotacija = new System.Windows.Forms.ComboBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnNewGroup = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.spravaGridGroupUserControl1 = new Bilten.UI.SpravaGridGroupUserControl();
            this.lblGrupa = new System.Windows.Forms.Label();
            this.lblRotacija = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSablon = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnUnesiOcenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPromeniStartListu = new System.Windows.Forms.ToolStripMenuItem();
            this.mnRezultatiViseboj = new System.Windows.Forms.ToolStripMenuItem();
            this.mnRezultatiSprave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnRezultatiEkipno = new System.Windows.Forms.ToolStripMenuItem();
            this.mnOcene = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPrikaziKlub = new System.Windows.Forms.ToolStripMenuItem();
            this.mnPrikaziDrzavu = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnOstaleRotacije = new System.Windows.Forms.Button();
            this.btnPrintUnosOcena = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbGrupa
            // 
            this.cmbGrupa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGrupa.FormattingEnabled = true;
            this.cmbGrupa.Location = new System.Drawing.Point(12, 26);
            this.cmbGrupa.Name = "cmbGrupa";
            this.cmbGrupa.Size = new System.Drawing.Size(66, 21);
            this.cmbGrupa.TabIndex = 0;
            this.cmbGrupa.DropDownClosed += new System.EventHandler(this.cmbGrupa_DropDownClosed);
            // 
            // cmbRotacija
            // 
            this.cmbRotacija.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRotacija.FormattingEnabled = true;
            this.cmbRotacija.Location = new System.Drawing.Point(94, 26);
            this.cmbRotacija.Name = "cmbRotacija";
            this.cmbRotacija.Size = new System.Drawing.Size(57, 21);
            this.cmbRotacija.TabIndex = 1;
            this.cmbRotacija.DropDownClosed += new System.EventHandler(this.cmbRotacija_DropDownClosed);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(319, 24);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(107, 23);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Promeni start listu";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnNewGroup
            // 
            this.btnNewGroup.Location = new System.Drawing.Point(628, 24);
            this.btnNewGroup.Name = "btnNewGroup";
            this.btnNewGroup.Size = new System.Drawing.Size(75, 23);
            this.btnNewGroup.TabIndex = 4;
            this.btnNewGroup.Text = "Nov turnus";
            this.btnNewGroup.UseVisualStyleBackColor = true;
            this.btnNewGroup.Click += new System.EventHandler(this.btnNewGroup_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 66);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1250, 304);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.spravaGridGroupUserControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1242, 278);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // spravaGridGroupUserControl1
            // 
            this.spravaGridGroupUserControl1.Location = new System.Drawing.Point(3, 6);
            this.spravaGridGroupUserControl1.Name = "spravaGridGroupUserControl1";
            this.spravaGridGroupUserControl1.Size = new System.Drawing.Size(444, 206);
            this.spravaGridGroupUserControl1.TabIndex = 0;
            // 
            // lblGrupa
            // 
            this.lblGrupa.AutoSize = true;
            this.lblGrupa.Location = new System.Drawing.Point(12, 10);
            this.lblGrupa.Name = "lblGrupa";
            this.lblGrupa.Size = new System.Drawing.Size(40, 13);
            this.lblGrupa.TabIndex = 5;
            this.lblGrupa.Text = "Turnus";
            // 
            // lblRotacija
            // 
            this.lblRotacija.AutoSize = true;
            this.lblRotacija.Location = new System.Drawing.Point(91, 9);
            this.lblRotacija.Name = "lblRotacija";
            this.lblRotacija.Size = new System.Drawing.Size(46, 13);
            this.lblRotacija.TabIndex = 6;
            this.lblRotacija.Text = "Rotacija";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(1122, 24);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Zatvori";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(205, 24);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(96, 23);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "Nova start lista";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSablon
            // 
            this.btnSablon.Location = new System.Drawing.Point(278, 53);
            this.btnSablon.Name = "btnSablon";
            this.btnSablon.Size = new System.Drawing.Size(107, 23);
            this.btnSablon.TabIndex = 7;
            this.btnSablon.Text = "Sablon start liste";
            this.btnSablon.UseVisualStyleBackColor = true;
            this.btnSablon.Click += new System.EventHandler(this.btnSablon_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(723, 24);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(95, 23);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Brisi start listu";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnUnesiOcenu,
            this.mnPromeniStartListu,
            this.mnRezultatiViseboj,
            this.mnRezultatiSprave,
            this.mnRezultatiEkipno,
            this.mnOcene,
            this.mnPrikaziKlub,
            this.mnPrikaziDrzavu});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(172, 180);
            // 
            // mnUnesiOcenu
            // 
            this.mnUnesiOcenu.Name = "mnUnesiOcenu";
            this.mnUnesiOcenu.Size = new System.Drawing.Size(171, 22);
            this.mnUnesiOcenu.Text = "Unesi ocenu";
            this.mnUnesiOcenu.Click += new System.EventHandler(this.mnUnesiOcenu_Click);
            // 
            // mnPromeniStartListu
            // 
            this.mnPromeniStartListu.Name = "mnPromeniStartListu";
            this.mnPromeniStartListu.Size = new System.Drawing.Size(171, 22);
            this.mnPromeniStartListu.Text = "Promeni start listu";
            this.mnPromeniStartListu.Click += new System.EventHandler(this.mnPromeniStartListu_Click);
            // 
            // mnRezultatiViseboj
            // 
            this.mnRezultatiViseboj.Name = "mnRezultatiViseboj";
            this.mnRezultatiViseboj.Size = new System.Drawing.Size(171, 22);
            this.mnRezultatiViseboj.Text = "Rezultati viseboj";
            this.mnRezultatiViseboj.Click += new System.EventHandler(this.mnRezultatiViseboj_Click);
            // 
            // mnRezultatiSprave
            // 
            this.mnRezultatiSprave.Name = "mnRezultatiSprave";
            this.mnRezultatiSprave.Size = new System.Drawing.Size(171, 22);
            this.mnRezultatiSprave.Text = "Rezultati sprave";
            this.mnRezultatiSprave.Click += new System.EventHandler(this.mnRezultatiSprave_Click);
            // 
            // mnRezultatiEkipno
            // 
            this.mnRezultatiEkipno.Name = "mnRezultatiEkipno";
            this.mnRezultatiEkipno.Size = new System.Drawing.Size(171, 22);
            this.mnRezultatiEkipno.Text = "Rezultati ekipno";
            this.mnRezultatiEkipno.Click += new System.EventHandler(this.mnRezultatiEkipno_Click);
            // 
            // mnOcene
            // 
            this.mnOcene.Name = "mnOcene";
            this.mnOcene.Size = new System.Drawing.Size(171, 22);
            this.mnOcene.Text = "Ocene";
            this.mnOcene.Click += new System.EventHandler(this.mnOcene_Click);
            // 
            // mnPrikaziKlub
            // 
            this.mnPrikaziKlub.Name = "mnPrikaziKlub";
            this.mnPrikaziKlub.Size = new System.Drawing.Size(171, 22);
            this.mnPrikaziKlub.Text = "Prikazi klub";
            this.mnPrikaziKlub.Click += new System.EventHandler(this.mnPrikaziKlub_Click);
            // 
            // mnPrikaziDrzavu
            // 
            this.mnPrikaziDrzavu.Name = "mnPrikaziDrzavu";
            this.mnPrikaziDrzavu.Size = new System.Drawing.Size(171, 22);
            this.mnPrikaziDrzavu.Text = "Prikazi drzavu";
            this.mnPrikaziDrzavu.Click += new System.EventHandler(this.mnPrikaziDrzavu_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(834, 24);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 9;
            this.btnPrint.Text = "Stampaj";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnOstaleRotacije
            // 
            this.btnOstaleRotacije.Location = new System.Drawing.Point(444, 24);
            this.btnOstaleRotacije.Name = "btnOstaleRotacije";
            this.btnOstaleRotacije.Size = new System.Drawing.Size(170, 23);
            this.btnOstaleRotacije.TabIndex = 10;
            this.btnOstaleRotacije.Text = "Kreiraj preostale rotacije";
            this.btnOstaleRotacije.UseVisualStyleBackColor = true;
            this.btnOstaleRotacije.Click += new System.EventHandler(this.btnOstaleRotacije_Click);
            // 
            // btnPrintUnosOcena
            // 
            this.btnPrintUnosOcena.Location = new System.Drawing.Point(925, 24);
            this.btnPrintUnosOcena.Name = "btnPrintUnosOcena";
            this.btnPrintUnosOcena.Size = new System.Drawing.Size(176, 23);
            this.btnPrintUnosOcena.TabIndex = 11;
            this.btnPrintUnosOcena.Text = "Stampaj liste za unos ucena";
            this.btnPrintUnosOcena.UseVisualStyleBackColor = true;
            this.btnPrintUnosOcena.Click += new System.EventHandler(this.btnPrintUnosOcena_Click);
            // 
            // StartListeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1274, 382);
            this.Controls.Add(this.btnPrintUnosOcena);
            this.Controls.Add(this.btnOstaleRotacije);
            this.Controls.Add(this.btnSablon);
            this.Controls.Add(this.lblRotacija);
            this.Controls.Add(this.lblGrupa);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.cmbRotacija);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.cmbGrupa);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnNewGroup);
            this.Name = "StartListeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StartListeForm";
            this.Load += new System.EventHandler(this.StartListeForm_Load);
            this.Shown += new System.EventHandler(this.StartListeForm_Shown);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbGrupa;
        private System.Windows.Forms.ComboBox cmbRotacija;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnNewGroup;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblGrupa;
        private System.Windows.Forms.Label lblRotacija;
        private System.Windows.Forms.Button btnClose;
        private SpravaGridGroupUserControl spravaGridGroupUserControl1;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSablon;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnUnesiOcenu;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnOstaleRotacije;
        private System.Windows.Forms.ToolStripMenuItem mnRezultatiViseboj;
        private System.Windows.Forms.ToolStripMenuItem mnRezultatiSprave;
        private System.Windows.Forms.ToolStripMenuItem mnRezultatiEkipno;
        private System.Windows.Forms.ToolStripMenuItem mnOcene;
        private System.Windows.Forms.ToolStripMenuItem mnPromeniStartListu;
        private System.Windows.Forms.Button btnPrintUnosOcena;
        private System.Windows.Forms.ToolStripMenuItem mnPrikaziKlub;
        private System.Windows.Forms.ToolStripMenuItem mnPrikaziDrzavu;
    }
}
