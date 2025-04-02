namespace Bilten.UI
{
    partial class PromeniBrojeveForm
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
            this.rbtUzastopniBrojevi = new System.Windows.Forms.RadioButton();
            this.rbtProizvoljniBrojevi = new System.Windows.Forms.RadioButton();
            this.txtUzastopniBrojevi = new System.Windows.Forms.TextBox();
            this.txtProizvoljniBrojevi = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbtUzastopniBrojevi
            // 
            this.rbtUzastopniBrojevi.AutoSize = true;
            this.rbtUzastopniBrojevi.Location = new System.Drawing.Point(33, 44);
            this.rbtUzastopniBrojevi.Name = "rbtUzastopniBrojevi";
            this.rbtUzastopniBrojevi.Size = new System.Drawing.Size(106, 17);
            this.rbtUzastopniBrojevi.TabIndex = 0;
            this.rbtUzastopniBrojevi.TabStop = true;
            this.rbtUzastopniBrojevi.Text = "Uzastopni brojevi";
            this.rbtUzastopniBrojevi.UseVisualStyleBackColor = true;
            this.rbtUzastopniBrojevi.CheckedChanged += new System.EventHandler(this.rbtUzastopniBrojevi_CheckedChanged);
            // 
            // rbtProizvoljniBrojevi
            // 
            this.rbtProizvoljniBrojevi.AutoSize = true;
            this.rbtProizvoljniBrojevi.Location = new System.Drawing.Point(33, 107);
            this.rbtProizvoljniBrojevi.Name = "rbtProizvoljniBrojevi";
            this.rbtProizvoljniBrojevi.Size = new System.Drawing.Size(106, 17);
            this.rbtProizvoljniBrojevi.TabIndex = 1;
            this.rbtProizvoljniBrojevi.TabStop = true;
            this.rbtProizvoljniBrojevi.Text = "Proizvoljni brojevi";
            this.rbtProizvoljniBrojevi.UseVisualStyleBackColor = true;
            this.rbtProizvoljniBrojevi.CheckedChanged += new System.EventHandler(this.rbtProizvoljniBrojevi_CheckedChanged);
            // 
            // txtUzastopniBrojevi
            // 
            this.txtUzastopniBrojevi.Location = new System.Drawing.Point(186, 43);
            this.txtUzastopniBrojevi.Name = "txtUzastopniBrojevi";
            this.txtUzastopniBrojevi.Size = new System.Drawing.Size(51, 20);
            this.txtUzastopniBrojevi.TabIndex = 2;
            // 
            // txtProizvoljniBrojevi
            // 
            this.txtProizvoljniBrojevi.Location = new System.Drawing.Point(186, 106);
            this.txtProizvoljniBrojevi.Name = "txtProizvoljniBrojevi";
            this.txtProizvoljniBrojevi.Size = new System.Drawing.Size(262, 20);
            this.txtProizvoljniBrojevi.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(183, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(302, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "Unesite jedan broj. Selektovanim gimnasticarima bice dodeljeni\r\nuzastopni brojevi" +
    " pocevsi od datog broja";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(183, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(265, 26);
            this.label2.TabIndex = 5;
            this.label2.Text = "Unesite vise brojeva razdvojenih razmakom.\r\nSelektovanim gimnasticarima bice dode" +
    "ljeni dati brojevi";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(315, 162);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(410, 162);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // PromeniBrojeveForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(520, 211);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtProizvoljniBrojevi);
            this.Controls.Add(this.txtUzastopniBrojevi);
            this.Controls.Add(this.rbtProizvoljniBrojevi);
            this.Controls.Add(this.rbtUzastopniBrojevi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PromeniBrojeveForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PromeniBrojeveForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtUzastopniBrojevi;
        private System.Windows.Forms.RadioButton rbtProizvoljniBrojevi;
        private System.Windows.Forms.TextBox txtUzastopniBrojevi;
        private System.Windows.Forms.TextBox txtProizvoljniBrojevi;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}