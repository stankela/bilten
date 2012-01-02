namespace Bilten.UI
{
    partial class KvalifikantiTak3Form
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
            this.cmbSprava = new System.Windows.Forms.ComboBox();
            this.lblRezerve = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            this.spravaGridUserControl1 = new Bilten.UI.SpravaGridUserControl();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbTakmicenje
            // 
            this.cmbTakmicenje.FormattingEnabled = true;
            this.cmbTakmicenje.Location = new System.Drawing.Point(12, 12);
            this.cmbTakmicenje.Name = "cmbTakmicenje";
            this.cmbTakmicenje.Size = new System.Drawing.Size(268, 21);
            this.cmbTakmicenje.TabIndex = 0;
            this.cmbTakmicenje.DropDownClosed += new System.EventHandler(this.cmbTakmicenje_DropDownClosed);
            // 
            // cmbSprava
            // 
            this.cmbSprava.FormattingEnabled = true;
            this.cmbSprava.Location = new System.Drawing.Point(297, 12);
            this.cmbSprava.Name = "cmbSprava";
            this.cmbSprava.Size = new System.Drawing.Size(121, 21);
            this.cmbSprava.TabIndex = 1;
            this.cmbSprava.DropDownClosed += new System.EventHandler(this.cmbSprava_DropDownClosed);
            // 
            // lblRezerve
            // 
            this.lblRezerve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRezerve.AutoSize = true;
            this.lblRezerve.Location = new System.Drawing.Point(12, 210);
            this.lblRezerve.Name = "lblRezerve";
            this.lblRezerve.Size = new System.Drawing.Size(47, 13);
            this.lblRezerve.TabIndex = 3;
            this.lblRezerve.Text = "Rezerve";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(442, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(113, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Dodaj kvalifikanta";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(561, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(103, 23);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Ukloni kvalifikanta";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dataGridViewUserControl1
            // 
            this.dataGridViewUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl1.ColumnHeaderSorting = true;
            this.dataGridViewUserControl1.Location = new System.Drawing.Point(15, 226);
            this.dataGridViewUserControl1.Name = "dataGridViewUserControl1";
            this.dataGridViewUserControl1.Size = new System.Drawing.Size(669, 127);
            this.dataGridViewUserControl1.TabIndex = 4;
            // 
            // spravaGridUserControl1
            // 
            this.spravaGridUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.spravaGridUserControl1.Location = new System.Drawing.Point(12, 54);
            this.spravaGridUserControl1.Name = "spravaGridUserControl1";
            this.spravaGridUserControl1.Size = new System.Drawing.Size(672, 140);
            this.spravaGridUserControl1.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(670, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Zatvori";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // KvalifikantiTak3Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 365);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dataGridViewUserControl1);
            this.Controls.Add(this.lblRezerve);
            this.Controls.Add(this.spravaGridUserControl1);
            this.Controls.Add(this.cmbSprava);
            this.Controls.Add(this.cmbTakmicenje);
            this.Name = "KvalifikantiTak3Form";
            this.Text = "KvalifikantiTak3Form";
            this.Shown += new System.EventHandler(this.KvalifikantiTak3Form_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTakmicenje;
        private System.Windows.Forms.ComboBox cmbSprava;
        private SpravaGridUserControl spravaGridUserControl1;
        private System.Windows.Forms.Label lblRezerve;
        private DataGridViewUserControl dataGridViewUserControl1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
    }
}