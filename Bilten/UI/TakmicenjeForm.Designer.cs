namespace Bilten.UI
{
    partial class TakmicenjeForm
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
            this.lblNaziv = new System.Windows.Forms.Label();
            this.txtNaziv = new System.Windows.Forms.TextBox();
            this.lblDatum = new System.Windows.Forms.Label();
            this.txtDatum = new System.Windows.Forms.TextBox();
            this.lblMesto = new System.Windows.Forms.Label();
            this.txtMesto = new System.Windows.Forms.TextBox();
            this.lblGimnastika = new System.Windows.Forms.Label();
            this.cmbGimnastika = new System.Windows.Forms.ComboBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnIzaberiPrvaDvaKola = new System.Windows.Forms.Button();
            this.ckbFinaleKupa = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(219, 371);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(311, 371);
            // 
            // lblNaziv
            // 
            this.lblNaziv.AutoSize = true;
            this.lblNaziv.Location = new System.Drawing.Point(21, 18);
            this.lblNaziv.Name = "lblNaziv";
            this.lblNaziv.Size = new System.Drawing.Size(98, 13);
            this.lblNaziv.TabIndex = 2;
            this.lblNaziv.Text = "Naziv takmicenja  *";
            // 
            // txtNaziv
            // 
            this.txtNaziv.Location = new System.Drawing.Point(24, 34);
            this.txtNaziv.Name = "txtNaziv";
            this.txtNaziv.Size = new System.Drawing.Size(325, 20);
            this.txtNaziv.TabIndex = 3;
            // 
            // lblDatum
            // 
            this.lblDatum.AutoSize = true;
            this.lblDatum.Location = new System.Drawing.Point(21, 125);
            this.lblDatum.Name = "lblDatum";
            this.lblDatum.Size = new System.Drawing.Size(103, 13);
            this.lblDatum.TabIndex = 4;
            this.lblDatum.Text = "Datum odrzavanja  *";
            // 
            // txtDatum
            // 
            this.txtDatum.Location = new System.Drawing.Point(24, 141);
            this.txtDatum.Name = "txtDatum";
            this.txtDatum.Size = new System.Drawing.Size(100, 20);
            this.txtDatum.TabIndex = 5;
            // 
            // lblMesto
            // 
            this.lblMesto.AutoSize = true;
            this.lblMesto.Location = new System.Drawing.Point(21, 183);
            this.lblMesto.Name = "lblMesto";
            this.lblMesto.Size = new System.Drawing.Size(101, 13);
            this.lblMesto.TabIndex = 6;
            this.lblMesto.Text = "Mesto odrzavanja  *";
            // 
            // txtMesto
            // 
            this.txtMesto.Location = new System.Drawing.Point(24, 199);
            this.txtMesto.Name = "txtMesto";
            this.txtMesto.Size = new System.Drawing.Size(100, 20);
            this.txtMesto.TabIndex = 7;
            // 
            // lblGimnastika
            // 
            this.lblGimnastika.AutoSize = true;
            this.lblGimnastika.Location = new System.Drawing.Point(21, 68);
            this.lblGimnastika.Name = "lblGimnastika";
            this.lblGimnastika.Size = new System.Drawing.Size(69, 13);
            this.lblGimnastika.TabIndex = 8;
            this.lblGimnastika.Text = "Gimnastika  *";
            // 
            // cmbGimnastika
            // 
            this.cmbGimnastika.FormattingEnabled = true;
            this.cmbGimnastika.Location = new System.Drawing.Point(24, 84);
            this.cmbGimnastika.Name = "cmbGimnastika";
            this.cmbGimnastika.Size = new System.Drawing.Size(98, 21);
            this.cmbGimnastika.TabIndex = 4;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(24, 275);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(325, 43);
            this.listBox1.TabIndex = 11;
            // 
            // btnIzaberiPrvaDvaKola
            // 
            this.btnIzaberiPrvaDvaKola.Location = new System.Drawing.Point(24, 324);
            this.btnIzaberiPrvaDvaKola.Name = "btnIzaberiPrvaDvaKola";
            this.btnIzaberiPrvaDvaKola.Size = new System.Drawing.Size(130, 23);
            this.btnIzaberiPrvaDvaKola.TabIndex = 12;
            this.btnIzaberiPrvaDvaKola.Text = "Izaberi I kolo i II kolo";
            this.btnIzaberiPrvaDvaKola.UseVisualStyleBackColor = true;
            this.btnIzaberiPrvaDvaKola.Click += new System.EventHandler(this.btnIzaberiPrvaDvaKola_Click);
            // 
            // ckbFinaleKupa
            // 
            this.ckbFinaleKupa.AutoSize = true;
            this.ckbFinaleKupa.Location = new System.Drawing.Point(24, 252);
            this.ckbFinaleKupa.Name = "ckbFinaleKupa";
            this.ckbFinaleKupa.Size = new System.Drawing.Size(81, 17);
            this.ckbFinaleKupa.TabIndex = 10;
            this.ckbFinaleKupa.Text = "Finale kupa";
            this.ckbFinaleKupa.UseVisualStyleBackColor = true;
            // 
            // TakmicenjeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 408);
            this.Controls.Add(this.ckbFinaleKupa);
            this.Controls.Add(this.btnIzaberiPrvaDvaKola);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.cmbGimnastika);
            this.Controls.Add(this.lblGimnastika);
            this.Controls.Add(this.lblNaziv);
            this.Controls.Add(this.txtNaziv);
            this.Controls.Add(this.lblDatum);
            this.Controls.Add(this.txtDatum);
            this.Controls.Add(this.txtMesto);
            this.Controls.Add(this.lblMesto);
            this.Name = "TakmicenjeForm";
            this.Text = "TakmicenjeForm";
            this.Shown += new System.EventHandler(this.TakmicenjeForm_Shown);
            this.Controls.SetChildIndex(this.lblMesto, 0);
            this.Controls.SetChildIndex(this.txtMesto, 0);
            this.Controls.SetChildIndex(this.txtDatum, 0);
            this.Controls.SetChildIndex(this.lblDatum, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.txtNaziv, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblNaziv, 0);
            this.Controls.SetChildIndex(this.lblGimnastika, 0);
            this.Controls.SetChildIndex(this.cmbGimnastika, 0);
            this.Controls.SetChildIndex(this.listBox1, 0);
            this.Controls.SetChildIndex(this.btnIzaberiPrvaDvaKola, 0);
            this.Controls.SetChildIndex(this.ckbFinaleKupa, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNaziv;
        private System.Windows.Forms.TextBox txtNaziv;
        private System.Windows.Forms.Label lblDatum;
        private System.Windows.Forms.TextBox txtDatum;
        private System.Windows.Forms.Label lblMesto;
        private System.Windows.Forms.TextBox txtMesto;
        private System.Windows.Forms.Label lblGimnastika;
        private System.Windows.Forms.ComboBox cmbGimnastika;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnIzaberiPrvaDvaKola;
        private System.Windows.Forms.CheckBox ckbFinaleKupa;
    }
}