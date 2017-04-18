namespace Bilten.UI
{
    partial class RezultatskoTakmicenjeDescriptionForm
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
            this.checkedListBoxKategorije = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(246, 228);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(338, 228);
            // 
            // lblNaziv
            // 
            this.lblNaziv.AutoSize = true;
            this.lblNaziv.Location = new System.Drawing.Point(25, 18);
            this.lblNaziv.Name = "lblNaziv";
            this.lblNaziv.Size = new System.Drawing.Size(88, 13);
            this.lblNaziv.TabIndex = 2;
            this.lblNaziv.Text = "Naziv takmicenja";
            // 
            // txtNaziv
            // 
            this.txtNaziv.Location = new System.Drawing.Point(28, 34);
            this.txtNaziv.Name = "txtNaziv";
            this.txtNaziv.Size = new System.Drawing.Size(344, 20);
            this.txtNaziv.TabIndex = 3;
            // 
            // checkedListBoxKategorije
            // 
            this.checkedListBoxKategorije.FormattingEnabled = true;
            this.checkedListBoxKategorije.Location = new System.Drawing.Point(28, 82);
            this.checkedListBoxKategorije.Name = "checkedListBoxKategorije";
            this.checkedListBoxKategorije.Size = new System.Drawing.Size(196, 169);
            this.checkedListBoxKategorije.TabIndex = 4;
            // 
            // RezultatskoTakmicenjeDescriptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 274);
            this.Controls.Add(this.checkedListBoxKategorije);
            this.Controls.Add(this.txtNaziv);
            this.Controls.Add(this.lblNaziv);
            this.Name = "RezultatskoTakmicenjeDescriptionForm";
            this.Text = "RezultatskoTakmicenjeDescriptionForm";
            this.Shown += new System.EventHandler(this.RezultatskoTakmicenjeDescriptionForm_Shown);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.lblNaziv, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.txtNaziv, 0);
            this.Controls.SetChildIndex(this.checkedListBoxKategorije, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNaziv;
        private System.Windows.Forms.TextBox txtNaziv;
        private System.Windows.Forms.CheckedListBox checkedListBoxKategorije;
    }
}