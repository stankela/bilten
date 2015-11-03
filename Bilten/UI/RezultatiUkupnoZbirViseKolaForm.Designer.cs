namespace Bilten.UI
{
    partial class RezultatiUkupnoZbirViseKolaForm
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
            this.cmbTakmicenje = new System.Windows.Forms.ComboBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            this.SuspendLayout();
            // 
            // cmbTakmicenje
            // 
            this.cmbTakmicenje.FormattingEnabled = true;
            this.cmbTakmicenje.Location = new System.Drawing.Point(12, 12);
            this.cmbTakmicenje.Name = "cmbTakmicenje";
            this.cmbTakmicenje.Size = new System.Drawing.Size(343, 21);
            this.cmbTakmicenje.TabIndex = 1;
            this.cmbTakmicenje.DropDownClosed += new System.EventHandler(this.cmbTakmicenja_DropDownClosed);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(388, 10);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Stampaj";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(483, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Zatvori";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dataGridViewUserControl1
            // 
            this.dataGridViewUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl1.ColumnHeaderSorting = true;
            this.dataGridViewUserControl1.Location = new System.Drawing.Point(12, 58);
            this.dataGridViewUserControl1.Name = "dataGridViewUserControl1";
            this.dataGridViewUserControl1.Size = new System.Drawing.Size(688, 287);
            this.dataGridViewUserControl1.TabIndex = 0;
            // 
            // RezultatiUkupnoZbirViseKolaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 357);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.dataGridViewUserControl1);
            this.Controls.Add(this.cmbTakmicenje);
            this.Name = "RezultatiUkupnoZbirViseKolaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RezultatiUkupnoZbirViseKolaForm";
            this.Shown += new System.EventHandler(this.RezultatiUkupnoZbirViseKolaForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTakmicenje;
        private DataGridViewUserControl dataGridViewUserControl1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
    }
}