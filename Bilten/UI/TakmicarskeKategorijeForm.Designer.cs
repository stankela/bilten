namespace Bilten.UI
{
    partial class TakmicarskeKategorijeForm
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
            this.lstKategorije = new System.Windows.Forms.ListBox();
            this.btnAddKategorija = new System.Windows.Forms.Button();
            this.btnDeleteKategorija = new System.Windows.Forms.Button();
            this.btnMoveUpKategorija = new System.Windows.Forms.Button();
            this.btnMoveDownKategorija = new System.Windows.Forms.Button();
            this.btnZatvori = new System.Windows.Forms.Button();
            this.lblNapomena = new System.Windows.Forms.Label();
            this.lblKategorija = new System.Windows.Forms.Label();
            this.lblTakmicenja = new System.Windows.Forms.Label();
            this.btnAddTakmicenje = new System.Windows.Forms.Button();
            this.btnDeleteTakmicenje = new System.Windows.Forms.Button();
            this.btnMoveUpTakmicenje = new System.Windows.Forms.Button();
            this.btnMoveDownTakmicenje = new System.Windows.Forms.Button();
            this.btnEditTakmicenje = new System.Windows.Forms.Button();
            this.btnEditKategorija = new System.Windows.Forms.Button();
            this.treeViewTakmicenja = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // lstKategorije
            // 
            this.lstKategorije.FormattingEnabled = true;
            this.lstKategorije.Location = new System.Drawing.Point(12, 25);
            this.lstKategorije.Name = "lstKategorije";
            this.lstKategorije.Size = new System.Drawing.Size(120, 108);
            this.lstKategorije.TabIndex = 0;
            // 
            // btnAddKategorija
            // 
            this.btnAddKategorija.Location = new System.Drawing.Point(138, 23);
            this.btnAddKategorija.Name = "btnAddKategorija";
            this.btnAddKategorija.Size = new System.Drawing.Size(75, 23);
            this.btnAddKategorija.TabIndex = 1;
            this.btnAddKategorija.Text = "Dodaj";
            this.btnAddKategorija.UseVisualStyleBackColor = true;
            this.btnAddKategorija.Click += new System.EventHandler(this.btnAddKategorija_Click);
            // 
            // btnDeleteKategorija
            // 
            this.btnDeleteKategorija.Location = new System.Drawing.Point(138, 83);
            this.btnDeleteKategorija.Name = "btnDeleteKategorija";
            this.btnDeleteKategorija.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteKategorija.TabIndex = 2;
            this.btnDeleteKategorija.Text = "Brisi";
            this.btnDeleteKategorija.UseVisualStyleBackColor = true;
            this.btnDeleteKategorija.Click += new System.EventHandler(this.btnDeleteKategorija_Click);
            // 
            // btnMoveUpKategorija
            // 
            this.btnMoveUpKategorija.Location = new System.Drawing.Point(138, 112);
            this.btnMoveUpKategorija.Name = "btnMoveUpKategorija";
            this.btnMoveUpKategorija.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUpKategorija.TabIndex = 3;
            this.btnMoveUpKategorija.Text = "Pomeri gore";
            this.btnMoveUpKategorija.UseVisualStyleBackColor = true;
            this.btnMoveUpKategorija.Click += new System.EventHandler(this.btnMoveUpKategorija_Click);
            // 
            // btnMoveDownKategorija
            // 
            this.btnMoveDownKategorija.Location = new System.Drawing.Point(138, 141);
            this.btnMoveDownKategorija.Name = "btnMoveDownKategorija";
            this.btnMoveDownKategorija.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDownKategorija.TabIndex = 4;
            this.btnMoveDownKategorija.Text = "Pomeri dole";
            this.btnMoveDownKategorija.UseVisualStyleBackColor = true;
            this.btnMoveDownKategorija.Click += new System.EventHandler(this.btnMoveDownKategorija_Click);
            // 
            // btnZatvori
            // 
            this.btnZatvori.Location = new System.Drawing.Point(792, 305);
            this.btnZatvori.Name = "btnZatvori";
            this.btnZatvori.Size = new System.Drawing.Size(75, 23);
            this.btnZatvori.TabIndex = 5;
            this.btnZatvori.Text = "Zatvori";
            this.btnZatvori.UseVisualStyleBackColor = true;
            this.btnZatvori.Click += new System.EventHandler(this.btnZatvori_Click);
            // 
            // lblNapomena
            // 
            this.lblNapomena.Location = new System.Drawing.Point(9, 253);
            this.lblNapomena.Name = "lblNapomena";
            this.lblNapomena.Size = new System.Drawing.Size(260, 44);
            this.lblNapomena.TabIndex = 6;
            this.lblNapomena.Text = "Napomena: Redosled kategorija i takmicenja u listi odredjuje i redosled u drugim " +
    "delovima programa.";
            // 
            // lblKategorija
            // 
            this.lblKategorija.AutoSize = true;
            this.lblKategorija.Location = new System.Drawing.Point(12, 9);
            this.lblKategorija.Name = "lblKategorija";
            this.lblKategorija.Size = new System.Drawing.Size(54, 13);
            this.lblKategorija.TabIndex = 8;
            this.lblKategorija.Text = "Kategorije";
            // 
            // lblTakmicenja
            // 
            this.lblTakmicenja.AutoSize = true;
            this.lblTakmicenja.Location = new System.Drawing.Point(289, 7);
            this.lblTakmicenja.Name = "lblTakmicenja";
            this.lblTakmicenja.Size = new System.Drawing.Size(62, 13);
            this.lblTakmicenja.TabIndex = 9;
            this.lblTakmicenja.Text = "Takmicenja";
            // 
            // btnAddTakmicenje
            // 
            this.btnAddTakmicenje.Location = new System.Drawing.Point(737, 23);
            this.btnAddTakmicenje.Name = "btnAddTakmicenje";
            this.btnAddTakmicenje.Size = new System.Drawing.Size(75, 23);
            this.btnAddTakmicenje.TabIndex = 11;
            this.btnAddTakmicenje.Text = "Dodaj";
            this.btnAddTakmicenje.UseVisualStyleBackColor = true;
            this.btnAddTakmicenje.Click += new System.EventHandler(this.btnAddTakmicenje_Click);
            // 
            // btnDeleteTakmicenje
            // 
            this.btnDeleteTakmicenje.Location = new System.Drawing.Point(737, 81);
            this.btnDeleteTakmicenje.Name = "btnDeleteTakmicenje";
            this.btnDeleteTakmicenje.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteTakmicenje.TabIndex = 12;
            this.btnDeleteTakmicenje.Text = "Brisi";
            this.btnDeleteTakmicenje.UseVisualStyleBackColor = true;
            this.btnDeleteTakmicenje.Click += new System.EventHandler(this.btnDeleteTakmicenje_Click);
            // 
            // btnMoveUpTakmicenje
            // 
            this.btnMoveUpTakmicenje.Location = new System.Drawing.Point(737, 110);
            this.btnMoveUpTakmicenje.Name = "btnMoveUpTakmicenje";
            this.btnMoveUpTakmicenje.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUpTakmicenje.TabIndex = 13;
            this.btnMoveUpTakmicenje.Text = "Pomeri gore";
            this.btnMoveUpTakmicenje.UseVisualStyleBackColor = true;
            this.btnMoveUpTakmicenje.Click += new System.EventHandler(this.btnMoveUpTakmicenje_Click);
            // 
            // btnMoveDownTakmicenje
            // 
            this.btnMoveDownTakmicenje.Location = new System.Drawing.Point(737, 140);
            this.btnMoveDownTakmicenje.Name = "btnMoveDownTakmicenje";
            this.btnMoveDownTakmicenje.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDownTakmicenje.TabIndex = 14;
            this.btnMoveDownTakmicenje.Text = "Pomeri dole";
            this.btnMoveDownTakmicenje.UseVisualStyleBackColor = true;
            this.btnMoveDownTakmicenje.Click += new System.EventHandler(this.btnMoveDownTakmicenje_Click);
            // 
            // btnEditTakmicenje
            // 
            this.btnEditTakmicenje.Location = new System.Drawing.Point(737, 52);
            this.btnEditTakmicenje.Name = "btnEditTakmicenje";
            this.btnEditTakmicenje.Size = new System.Drawing.Size(75, 23);
            this.btnEditTakmicenje.TabIndex = 15;
            this.btnEditTakmicenje.Text = "Promeni";
            this.btnEditTakmicenje.UseVisualStyleBackColor = true;
            this.btnEditTakmicenje.Click += new System.EventHandler(this.btnEditTakmicenje_Click);
            // 
            // btnEditKategorija
            // 
            this.btnEditKategorija.Location = new System.Drawing.Point(138, 52);
            this.btnEditKategorija.Name = "btnEditKategorija";
            this.btnEditKategorija.Size = new System.Drawing.Size(75, 23);
            this.btnEditKategorija.TabIndex = 16;
            this.btnEditKategorija.Text = "Promeni";
            this.btnEditKategorija.UseVisualStyleBackColor = true;
            this.btnEditKategorija.Click += new System.EventHandler(this.btnEditKategorija_Click);
            // 
            // treeViewTakmicenja
            // 
            this.treeViewTakmicenja.Location = new System.Drawing.Point(289, 25);
            this.treeViewTakmicenja.Name = "treeViewTakmicenja";
            this.treeViewTakmicenja.Size = new System.Drawing.Size(442, 272);
            this.treeViewTakmicenja.TabIndex = 17;
            // 
            // TakmicarskeKategorijeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 342);
            this.Controls.Add(this.treeViewTakmicenja);
            this.Controls.Add(this.btnEditKategorija);
            this.Controls.Add(this.btnEditTakmicenje);
            this.Controls.Add(this.btnMoveDownTakmicenje);
            this.Controls.Add(this.btnMoveUpTakmicenje);
            this.Controls.Add(this.btnDeleteTakmicenje);
            this.Controls.Add(this.btnAddTakmicenje);
            this.Controls.Add(this.lblTakmicenja);
            this.Controls.Add(this.lblKategorija);
            this.Controls.Add(this.lblNapomena);
            this.Controls.Add(this.btnZatvori);
            this.Controls.Add(this.btnMoveDownKategorija);
            this.Controls.Add(this.btnMoveUpKategorija);
            this.Controls.Add(this.btnDeleteKategorija);
            this.Controls.Add(this.btnAddKategorija);
            this.Controls.Add(this.lstKategorije);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TakmicarskeKategorijeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TakmicarskeKategorijeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstKategorije;
        private System.Windows.Forms.Button btnAddKategorija;
        private System.Windows.Forms.Button btnDeleteKategorija;
        private System.Windows.Forms.Button btnMoveUpKategorija;
        private System.Windows.Forms.Button btnMoveDownKategorija;
        private System.Windows.Forms.Button btnZatvori;
        private System.Windows.Forms.Label lblNapomena;
        private System.Windows.Forms.Label lblKategorija;
        private System.Windows.Forms.Label lblTakmicenja;
        private System.Windows.Forms.Button btnAddTakmicenje;
        private System.Windows.Forms.Button btnDeleteTakmicenje;
        private System.Windows.Forms.Button btnMoveUpTakmicenje;
        private System.Windows.Forms.Button btnMoveDownTakmicenje;
        private System.Windows.Forms.Button btnEditTakmicenje;
        private System.Windows.Forms.Button btnEditKategorija;
        private System.Windows.Forms.TreeView treeViewTakmicenja;
    }
}