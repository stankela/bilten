namespace Bilten.UI
{
    partial class EkipeForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnAddEkipa = new System.Windows.Forms.Button();
            this.btnEditEkipa = new System.Windows.Forms.Button();
            this.btnDeleteEkipa = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.ekipeUserControl1 = new Bilten.UI.EkipeUserControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 56);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(690, 289);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ekipeUserControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(682, 263);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnAddEkipa
            // 
            this.btnAddEkipa.Location = new System.Drawing.Point(12, 12);
            this.btnAddEkipa.Name = "btnAddEkipa";
            this.btnAddEkipa.Size = new System.Drawing.Size(75, 23);
            this.btnAddEkipa.TabIndex = 1;
            this.btnAddEkipa.Text = "Dodaj ekipu";
            this.btnAddEkipa.UseVisualStyleBackColor = true;
            this.btnAddEkipa.Click += new System.EventHandler(this.btnAddEkipa_Click);
            // 
            // btnEditEkipa
            // 
            this.btnEditEkipa.Location = new System.Drawing.Point(105, 12);
            this.btnEditEkipa.Name = "btnEditEkipa";
            this.btnEditEkipa.Size = new System.Drawing.Size(75, 23);
            this.btnEditEkipa.TabIndex = 2;
            this.btnEditEkipa.Text = "Promeni";
            this.btnEditEkipa.UseVisualStyleBackColor = true;
            this.btnEditEkipa.Click += new System.EventHandler(this.btnEditEkipa_Click);
            // 
            // btnDeleteEkipa
            // 
            this.btnDeleteEkipa.Location = new System.Drawing.Point(199, 12);
            this.btnDeleteEkipa.Name = "btnDeleteEkipa";
            this.btnDeleteEkipa.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteEkipa.TabIndex = 3;
            this.btnDeleteEkipa.Text = "Brisi";
            this.btnDeleteEkipa.UseVisualStyleBackColor = true;
            this.btnDeleteEkipa.Click += new System.EventHandler(this.btnDeleteEkipa_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(289, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Zatvori";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ekipeUserControl1
            // 
            this.ekipeUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ekipeUserControl1.Location = new System.Drawing.Point(6, 6);
            this.ekipeUserControl1.Name = "ekipeUserControl1";
            this.ekipeUserControl1.Size = new System.Drawing.Size(670, 251);
            this.ekipeUserControl1.TabIndex = 0;
            // 
            // EkipeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 357);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDeleteEkipa);
            this.Controls.Add(this.btnEditEkipa);
            this.Controls.Add(this.btnAddEkipa);
            this.Controls.Add(this.tabControl1);
            this.Name = "EkipeForm";
            this.Text = "EkipeForm";
            this.Load += new System.EventHandler(this.EkipeForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnAddEkipa;
        private System.Windows.Forms.Button btnEditEkipa;
        private System.Windows.Forms.Button btnDeleteEkipa;
        private System.Windows.Forms.Button btnClose;
        private EkipeUserControl ekipeUserControl1;
    }
}