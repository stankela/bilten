namespace Bilten.UI
{
    partial class LogoForm
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
            this.lblSlike = new System.Windows.Forms.Label();
            this.btnBrisiSliku = new System.Windows.Forms.Button();
            this.btnDodajSliku = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.listViewLogo = new System.Windows.Forms.ListView();
            this.pictureBoxSlika = new Bilten.UI.PictureBoxPlus();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSlika)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSlike
            // 
            this.lblSlike.AutoSize = true;
            this.lblSlike.Location = new System.Drawing.Point(9, 15);
            this.lblSlike.Name = "lblSlike";
            this.lblSlike.Size = new System.Drawing.Size(30, 13);
            this.lblSlike.TabIndex = 40;
            this.lblSlike.Text = "Slike";
            // 
            // btnBrisiSliku
            // 
            this.btnBrisiSliku.Location = new System.Drawing.Point(93, 203);
            this.btnBrisiSliku.Name = "btnBrisiSliku";
            this.btnBrisiSliku.Size = new System.Drawing.Size(75, 23);
            this.btnBrisiSliku.TabIndex = 3;
            this.btnBrisiSliku.Text = "Brisi sliku";
            this.btnBrisiSliku.UseVisualStyleBackColor = true;
            this.btnBrisiSliku.Click += new System.EventHandler(this.btnBrisiSliku_Click);
            // 
            // btnDodajSliku
            // 
            this.btnDodajSliku.Location = new System.Drawing.Point(12, 203);
            this.btnDodajSliku.Name = "btnDodajSliku";
            this.btnDodajSliku.Size = new System.Drawing.Size(75, 23);
            this.btnDodajSliku.TabIndex = 2;
            this.btnDodajSliku.Text = "Dodaj sliku";
            this.btnDodajSliku.UseVisualStyleBackColor = true;
            this.btnDodajSliku.Click += new System.EventHandler(this.btnDodajSliku_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(544, 308);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(635, 308);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // listViewLogo
            // 
            this.listViewLogo.Location = new System.Drawing.Point(12, 31);
            this.listViewLogo.Name = "listViewLogo";
            this.listViewLogo.Size = new System.Drawing.Size(337, 155);
            this.listViewLogo.TabIndex = 47;
            this.listViewLogo.UseCompatibleStateImageBehavior = false;
            this.listViewLogo.SelectedIndexChanged += new System.EventHandler(this.listViewLogo_SelectedIndexChanged);
            // 
            // pictureBoxSlika
            // 
            this.pictureBoxSlika.BackColor = System.Drawing.Color.White;
            this.pictureBoxSlika.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxSlika.Location = new System.Drawing.Point(384, 31);
            this.pictureBoxSlika.Name = "pictureBoxSlika";
            this.pictureBoxSlika.NoDistort = false;
            this.pictureBoxSlika.Size = new System.Drawing.Size(326, 260);
            this.pictureBoxSlika.TabIndex = 46;
            this.pictureBoxSlika.TabStop = false;
            // 
            // LogoForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(734, 352);
            this.Controls.Add(this.listViewLogo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.pictureBoxSlika);
            this.Controls.Add(this.btnBrisiSliku);
            this.Controls.Add(this.btnDodajSliku);
            this.Controls.Add(this.lblSlike);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogoForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LogoForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSlika)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSlike;
        private System.Windows.Forms.Button btnBrisiSliku;
        private System.Windows.Forms.Button btnDodajSliku;
        private Bilten.UI.PictureBoxPlus pictureBoxSlika;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView listViewLogo;
    }
}