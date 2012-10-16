namespace Bilten.UI
{
    partial class SelectGimnasticariPrethTakmForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblTakmicenja = new System.Windows.Forms.Label();
            this.lblGimnasticari = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.dataGridViewUserControl2 = new Bilten.UI.DataGridViewUserControl();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(905, 402);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(999, 402);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblTakmicenja
            // 
            this.lblTakmicenja.AutoSize = true;
            this.lblTakmicenja.Location = new System.Drawing.Point(12, 23);
            this.lblTakmicenja.Name = "lblTakmicenja";
            this.lblTakmicenja.Size = new System.Drawing.Size(62, 13);
            this.lblTakmicenja.TabIndex = 4;
            this.lblTakmicenja.Text = "Takmicenja";
            // 
            // lblGimnasticari
            // 
            this.lblGimnasticari.AutoSize = true;
            this.lblGimnasticari.Location = new System.Drawing.Point(460, 26);
            this.lblGimnasticari.Name = "lblGimnasticari";
            this.lblGimnasticari.Size = new System.Drawing.Size(64, 13);
            this.lblGimnasticari.TabIndex = 7;
            this.lblGimnasticari.Text = "Gimnasticari";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(15, 42);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(422, 383);
            this.treeView1.TabIndex = 8;
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // dataGridViewUserControl2
            // 
            this.dataGridViewUserControl2.ColumnHeaderSorting = true;
            this.dataGridViewUserControl2.Location = new System.Drawing.Point(463, 42);
            this.dataGridViewUserControl2.Name = "dataGridViewUserControl2";
            this.dataGridViewUserControl2.Size = new System.Drawing.Size(611, 341);
            this.dataGridViewUserControl2.TabIndex = 6;
            // 
            // SelectTakmicenjeKategorijaForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1106, 437);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.lblGimnasticari);
            this.Controls.Add(this.dataGridViewUserControl2);
            this.Controls.Add(this.lblTakmicenja);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "SelectTakmicenjeKategorijaForm";
            this.Text = "SelectTakmicenjeKategorijaForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTakmicenja;
        private DataGridViewUserControl dataGridViewUserControl2;
        private System.Windows.Forms.Label lblGimnasticari;
        private System.Windows.Forms.TreeView treeView1;
    }
}