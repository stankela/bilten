namespace Bilten.UI
{
    partial class MaxGimIzKlubaUFinaluForm
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
            this.panel4 = new System.Windows.Forms.Panel();
            this.rbtDrzava = new System.Windows.Forms.RadioButton();
            this.rbtKlub = new System.Windows.Forms.RadioButton();
            this.txtMaxTak = new System.Windows.Forms.TextBox();
            this.lblMaxTak = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rbtDrzava);
            this.panel4.Controls.Add(this.rbtKlub);
            this.panel4.Location = new System.Drawing.Point(181, 19);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(66, 42);
            this.panel4.TabIndex = 17;
            // 
            // rbtDrzava
            // 
            this.rbtDrzava.AutoSize = true;
            this.rbtDrzava.Location = new System.Drawing.Point(3, 21);
            this.rbtDrzava.Name = "rbtDrzava";
            this.rbtDrzava.Size = new System.Drawing.Size(59, 17);
            this.rbtDrzava.TabIndex = 1;
            this.rbtDrzava.TabStop = true;
            this.rbtDrzava.Text = "Drzave";
            this.rbtDrzava.UseVisualStyleBackColor = true;
            // 
            // rbtKlub
            // 
            this.rbtKlub.AutoSize = true;
            this.rbtKlub.Location = new System.Drawing.Point(3, 3);
            this.rbtKlub.Name = "rbtKlub";
            this.rbtKlub.Size = new System.Drawing.Size(52, 17);
            this.rbtKlub.TabIndex = 0;
            this.rbtKlub.TabStop = true;
            this.rbtKlub.Text = "Kluba";
            this.rbtKlub.UseVisualStyleBackColor = true;
            // 
            // txtMaxTak
            // 
            this.txtMaxTak.Location = new System.Drawing.Point(251, 28);
            this.txtMaxTak.Name = "txtMaxTak";
            this.txtMaxTak.Size = new System.Drawing.Size(43, 20);
            this.txtMaxTak.TabIndex = 16;
            // 
            // lblMaxTak
            // 
            this.lblMaxTak.AutoSize = true;
            this.lblMaxTak.Location = new System.Drawing.Point(12, 31);
            this.lblMaxTak.Name = "lblMaxTak";
            this.lblMaxTak.Size = new System.Drawing.Size(167, 13);
            this.lblMaxTak.TabIndex = 15;
            this.lblMaxTak.Text = "Maksimalan broj takmicara iz istog";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(138, 88);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(219, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // MaxGimIzKlubaUFinaluForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(310, 126);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.txtMaxTak);
            this.Controls.Add(this.lblMaxTak);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MaxGimIzKlubaUFinaluForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MaxGimIzKlubaUFinaluForm";
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton rbtDrzava;
        private System.Windows.Forms.RadioButton rbtKlub;
        private System.Windows.Forms.TextBox txtMaxTak;
        private System.Windows.Forms.Label lblMaxTak;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}