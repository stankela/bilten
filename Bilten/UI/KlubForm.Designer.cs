namespace Bilten.UI
{
    partial class KlubForm
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
            this.lblKod = new System.Windows.Forms.Label();
            this.lblMesto = new System.Windows.Forms.Label();
            this.txtKod = new System.Windows.Forms.TextBox();
            this.txtNaziv = new System.Windows.Forms.TextBox();
            this.cmbMesto = new System.Windows.Forms.ComboBox();
            this.btnAddMesto = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblNaziv
            // 
            this.lblNaziv.AutoSize = true;
            this.lblNaziv.Location = new System.Drawing.Point(25, 19);
            this.lblNaziv.Name = "lblNaziv";
            this.lblNaziv.Size = new System.Drawing.Size(73, 13);
            this.lblNaziv.TabIndex = 2;
            this.lblNaziv.Text = "Naziv kluba  *";
            // 
            // lblKod
            // 
            this.lblKod.AutoSize = true;
            this.lblKod.Location = new System.Drawing.Point(24, 74);
            this.lblKod.Name = "lblKod";
            this.lblKod.Size = new System.Drawing.Size(80, 13);
            this.lblKod.TabIndex = 4;
            this.lblKod.Text = "Skraceni kod  *";
            // 
            // lblMesto
            // 
            this.lblMesto.AutoSize = true;
            this.lblMesto.Location = new System.Drawing.Point(25, 130);
            this.lblMesto.Name = "lblMesto";
            this.lblMesto.Size = new System.Drawing.Size(46, 13);
            this.lblMesto.TabIndex = 6;
            this.lblMesto.Text = "Mesto  *";
            // 
            // txtKod
            // 
            this.txtKod.Location = new System.Drawing.Point(27, 90);
            this.txtKod.Name = "txtKod";
            this.txtKod.Size = new System.Drawing.Size(61, 20);
            this.txtKod.TabIndex = 5;
            // 
            // txtNaziv
            // 
            this.txtNaziv.Location = new System.Drawing.Point(27, 35);
            this.txtNaziv.Name = "txtNaziv";
            this.txtNaziv.Size = new System.Drawing.Size(274, 20);
            this.txtNaziv.TabIndex = 3;
            // 
            // cmbMesto
            // 
            this.cmbMesto.FormattingEnabled = true;
            this.cmbMesto.Location = new System.Drawing.Point(28, 146);
            this.cmbMesto.Name = "cmbMesto";
            this.cmbMesto.Size = new System.Drawing.Size(121, 21);
            this.cmbMesto.TabIndex = 7;
            // 
            // btnAddMesto
            // 
            this.btnAddMesto.Location = new System.Drawing.Point(155, 145);
            this.btnAddMesto.Name = "btnAddMesto";
            this.btnAddMesto.Size = new System.Drawing.Size(25, 23);
            this.btnAddMesto.TabIndex = 8;
            this.btnAddMesto.Text = "...";
            this.btnAddMesto.UseVisualStyleBackColor = true;
            this.btnAddMesto.Click += new System.EventHandler(this.btnAddMesto_Click);
            // 
            // KlubForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(396, 231);
            this.Controls.Add(this.btnAddMesto);
            this.Controls.Add(this.cmbMesto);
            this.Controls.Add(this.txtNaziv);
            this.Controls.Add(this.txtKod);
            this.Controls.Add(this.lblMesto);
            this.Controls.Add(this.lblKod);
            this.Controls.Add(this.lblNaziv);
            this.Name = "KlubForm";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblNaziv, 0);
            this.Controls.SetChildIndex(this.lblKod, 0);
            this.Controls.SetChildIndex(this.lblMesto, 0);
            this.Controls.SetChildIndex(this.txtKod, 0);
            this.Controls.SetChildIndex(this.txtNaziv, 0);
            this.Controls.SetChildIndex(this.cmbMesto, 0);
            this.Controls.SetChildIndex(this.btnAddMesto, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNaziv;
        private System.Windows.Forms.Label lblKod;
        private System.Windows.Forms.Label lblMesto;
        protected System.Windows.Forms.TextBox txtKod;
        protected System.Windows.Forms.TextBox txtNaziv;
        private System.Windows.Forms.ComboBox cmbMesto;
        private System.Windows.Forms.Button btnAddMesto;

    }
}
