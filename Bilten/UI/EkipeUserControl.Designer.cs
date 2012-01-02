namespace Bilten.UI
{
    partial class EkipeUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblEkipe = new System.Windows.Forms.Label();
            this.lblClanovi = new System.Windows.Forms.Label();
            this.dgwUserControlClanovi = new Bilten.UI.DataGridViewUserControl();
            this.dgwUserControlEkipe = new Bilten.UI.DataGridViewUserControl();
            this.SuspendLayout();
            // 
            // lblEkipe
            // 
            this.lblEkipe.AutoSize = true;
            this.lblEkipe.Location = new System.Drawing.Point(0, 0);
            this.lblEkipe.Name = "lblEkipe";
            this.lblEkipe.Size = new System.Drawing.Size(34, 13);
            this.lblEkipe.TabIndex = 0;
            this.lblEkipe.Text = "Ekipe";
            // 
            // lblClanovi
            // 
            this.lblClanovi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblClanovi.AutoSize = true;
            this.lblClanovi.Location = new System.Drawing.Point(-3, 186);
            this.lblClanovi.Name = "lblClanovi";
            this.lblClanovi.Size = new System.Drawing.Size(71, 13);
            this.lblClanovi.TabIndex = 2;
            this.lblClanovi.Text = "Clanovi ekipe";
            // 
            // dgwUserControlClanovi
            // 
            this.dgwUserControlClanovi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgwUserControlClanovi.Location = new System.Drawing.Point(0, 202);
            this.dgwUserControlClanovi.Name = "dgwUserControlClanovi";
            this.dgwUserControlClanovi.Size = new System.Drawing.Size(430, 163);
            this.dgwUserControlClanovi.TabIndex = 3;
            // 
            // dgwUserControlEkipe
            // 
            this.dgwUserControlEkipe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgwUserControlEkipe.Location = new System.Drawing.Point(0, 16);
            this.dgwUserControlEkipe.Name = "dgwUserControlEkipe";
            this.dgwUserControlEkipe.Size = new System.Drawing.Size(430, 151);
            this.dgwUserControlEkipe.TabIndex = 1;
            // 
            // EkipeUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgwUserControlClanovi);
            this.Controls.Add(this.lblClanovi);
            this.Controls.Add(this.dgwUserControlEkipe);
            this.Controls.Add(this.lblEkipe);
            this.Name = "EkipeUserControl";
            this.Size = new System.Drawing.Size(430, 368);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEkipe;
        private DataGridViewUserControl dgwUserControlEkipe;
        private System.Windows.Forms.Label lblClanovi;
        private DataGridViewUserControl dgwUserControlClanovi;
    }
}
