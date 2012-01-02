namespace Bilten.UI
{
    partial class OceneForm
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.cmbKategorija = new System.Windows.Forms.ComboBox();
            this.cmbSprava = new System.Windows.Forms.ComboBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.spravaGridUserControl1 = new Bilten.UI.SpravaGridUserControl();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(350, 14);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(81, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Unesi ocenu";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(437, 14);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(96, 23);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Promeni ocenu";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // cmbKategorija
            // 
            this.cmbKategorija.FormattingEnabled = true;
            this.cmbKategorija.Location = new System.Drawing.Point(12, 14);
            this.cmbKategorija.Name = "cmbKategorija";
            this.cmbKategorija.Size = new System.Drawing.Size(175, 21);
            this.cmbKategorija.TabIndex = 3;
            this.cmbKategorija.DropDownClosed += new System.EventHandler(this.cmbKategorija_DropDownClosed);
            // 
            // cmbSprava
            // 
            this.cmbSprava.FormattingEnabled = true;
            this.cmbSprava.Location = new System.Drawing.Point(211, 14);
            this.cmbSprava.Name = "cmbSprava";
            this.cmbSprava.Size = new System.Drawing.Size(121, 21);
            this.cmbSprava.TabIndex = 4;
            this.cmbSprava.DropDownClosed += new System.EventHandler(this.cmbSprava_DropDownClosed);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(539, 14);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Brisi ocenu";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(620, 14);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Zatvori";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // spravaGridUserControl1
            // 
            this.spravaGridUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.spravaGridUserControl1.Location = new System.Drawing.Point(12, 60);
            this.spravaGridUserControl1.Name = "spravaGridUserControl1";
            this.spravaGridUserControl1.Size = new System.Drawing.Size(683, 293);
            this.spravaGridUserControl1.TabIndex = 5;
            // 
            // OceneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 365);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.spravaGridUserControl1);
            this.Controls.Add(this.cmbSprava);
            this.Controls.Add(this.cmbKategorija);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Name = "OceneForm";
            this.Text = "OceneForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.ComboBox cmbKategorija;
        private System.Windows.Forms.ComboBox cmbSprava;
        private SpravaGridUserControl spravaGridUserControl1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;

    }
}