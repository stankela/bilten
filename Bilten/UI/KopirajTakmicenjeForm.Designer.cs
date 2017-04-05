namespace Bilten.UI
{
    partial class KopirajTakmicenjeForm
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
            this.lblTakmicenje = new System.Windows.Forms.Label();
            this.txtTakmicenje = new System.Windows.Forms.TextBox();
            this.btnIzaberi = new System.Windows.Forms.Button();
            this.lblTakmicenja = new System.Windows.Forms.Label();
            this.lstRezTakmicenja = new System.Windows.Forms.CheckedListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTakmicenje
            // 
            this.lblTakmicenje.AutoSize = true;
            this.lblTakmicenje.Location = new System.Drawing.Point(18, 17);
            this.lblTakmicenje.Name = "lblTakmicenje";
            this.lblTakmicenje.Size = new System.Drawing.Size(62, 13);
            this.lblTakmicenje.TabIndex = 0;
            this.lblTakmicenje.Text = "Takmicenje";
            // 
            // txtTakmicenje
            // 
            this.txtTakmicenje.BackColor = System.Drawing.SystemColors.Window;
            this.txtTakmicenje.Location = new System.Drawing.Point(21, 33);
            this.txtTakmicenje.Name = "txtTakmicenje";
            this.txtTakmicenje.ReadOnly = true;
            this.txtTakmicenje.Size = new System.Drawing.Size(339, 20);
            this.txtTakmicenje.TabIndex = 1;
            this.txtTakmicenje.TabStop = false;
            // 
            // btnIzaberi
            // 
            this.btnIzaberi.Location = new System.Drawing.Point(366, 31);
            this.btnIzaberi.Name = "btnIzaberi";
            this.btnIzaberi.Size = new System.Drawing.Size(63, 23);
            this.btnIzaberi.TabIndex = 2;
            this.btnIzaberi.Text = "Izaberi";
            this.btnIzaberi.UseVisualStyleBackColor = true;
            this.btnIzaberi.Click += new System.EventHandler(this.btnIzaberi_Click);
            // 
            // lblTakmicenja
            // 
            this.lblTakmicenja.AutoSize = true;
            this.lblTakmicenja.Location = new System.Drawing.Point(18, 84);
            this.lblTakmicenja.Name = "lblTakmicenja";
            this.lblTakmicenja.Size = new System.Drawing.Size(62, 13);
            this.lblTakmicenja.TabIndex = 3;
            this.lblTakmicenja.Text = "Takmicenja";
            // 
            // lstRezTakmicenja
            // 
            this.lstRezTakmicenja.FormattingEnabled = true;
            this.lstRezTakmicenja.Location = new System.Drawing.Point(21, 100);
            this.lstRezTakmicenja.Name = "lstRezTakmicenja";
            this.lstRezTakmicenja.Size = new System.Drawing.Size(419, 169);
            this.lstRezTakmicenja.TabIndex = 4;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(256, 364);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(354, 364);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // KopirajTakmicenjeForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(463, 409);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lstRezTakmicenja);
            this.Controls.Add(this.lblTakmicenja);
            this.Controls.Add(this.btnIzaberi);
            this.Controls.Add(this.txtTakmicenje);
            this.Controls.Add(this.lblTakmicenje);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KopirajTakmicenjeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KopirajTakmicenjeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTakmicenje;
        private System.Windows.Forms.TextBox txtTakmicenje;
        private System.Windows.Forms.Button btnIzaberi;
        private System.Windows.Forms.Label lblTakmicenja;
        private System.Windows.Forms.CheckedListBox lstRezTakmicenja;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}