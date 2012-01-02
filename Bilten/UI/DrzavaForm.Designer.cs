namespace Bilten.UI
{
    partial class DrzavaForm
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
            this.lblKod = new System.Windows.Forms.Label();
            this.txtKod = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(11, 124);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(103, 124);
            // 
            // lblNaziv
            // 
            this.lblNaziv.AutoSize = true;
            this.lblNaziv.Location = new System.Drawing.Point(9, 12);
            this.lblNaziv.Name = "lblNaziv";
            this.lblNaziv.Size = new System.Drawing.Size(79, 13);
            this.lblNaziv.TabIndex = 2;
            this.lblNaziv.Text = "Naziv drzave  *";
            // 
            // txtNaziv
            // 
            this.txtNaziv.Location = new System.Drawing.Point(12, 28);
            this.txtNaziv.Name = "txtNaziv";
            this.txtNaziv.Size = new System.Drawing.Size(100, 20);
            this.txtNaziv.TabIndex = 3;
            // 
            // lblKod
            // 
            this.lblKod.AutoSize = true;
            this.lblKod.Location = new System.Drawing.Point(9, 65);
            this.lblKod.Name = "lblKod";
            this.lblKod.Size = new System.Drawing.Size(123, 13);
            this.lblKod.TabIndex = 4;
            this.lblKod.Text = "Skraceni kod (3 slova)  *";
            // 
            // txtKod
            // 
            this.txtKod.Location = new System.Drawing.Point(12, 81);
            this.txtKod.Name = "txtKod";
            this.txtKod.Size = new System.Drawing.Size(45, 20);
            this.txtKod.TabIndex = 5;
            // 
            // DrzavaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(209, 167);
            this.Controls.Add(this.lblNaziv);
            this.Controls.Add(this.txtNaziv);
            this.Controls.Add(this.txtKod);
            this.Controls.Add(this.lblKod);
            this.Name = "DrzavaForm";
            this.Text = "DrzavaForm";
            this.Controls.SetChildIndex(this.lblKod, 0);
            this.Controls.SetChildIndex(this.txtKod, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.txtNaziv, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblNaziv, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNaziv;
        private System.Windows.Forms.TextBox txtNaziv;
        private System.Windows.Forms.Label lblKod;
        private System.Windows.Forms.TextBox txtKod;
    }
}