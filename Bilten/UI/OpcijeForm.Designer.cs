namespace Bilten.UI
{
    partial class OpcijeForm
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
            this.lblBrojDecimala = new System.Windows.Forms.Label();
            this.lblBrojDecD = new System.Windows.Forms.Label();
            this.txtBrojDecD = new System.Windows.Forms.TextBox();
            this.lblBrojDecE1 = new System.Windows.Forms.Label();
            this.txtBrojDecE1 = new System.Windows.Forms.TextBox();
            this.lblBrojDecE = new System.Windows.Forms.Label();
            this.txtBrojDecE = new System.Windows.Forms.TextBox();
            this.lblPen = new System.Windows.Forms.Label();
            this.txtBrojDecPen = new System.Windows.Forms.TextBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtBrojDecTotal = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBrojDecimala
            // 
            this.lblBrojDecimala.AutoSize = true;
            this.lblBrojDecimala.Location = new System.Drawing.Point(12, 18);
            this.lblBrojDecimala.Name = "lblBrojDecimala";
            this.lblBrojDecimala.Size = new System.Drawing.Size(103, 13);
            this.lblBrojDecimala.TabIndex = 0;
            this.lblBrojDecimala.Text = "Broj decimala ocena";
            // 
            // lblBrojDecD
            // 
            this.lblBrojDecD.AutoSize = true;
            this.lblBrojDecD.Location = new System.Drawing.Point(62, 44);
            this.lblBrojDecD.Name = "lblBrojDecD";
            this.lblBrojDecD.Size = new System.Drawing.Size(15, 13);
            this.lblBrojDecD.TabIndex = 1;
            this.lblBrojDecD.Text = "D";
            // 
            // txtBrojDecD
            // 
            this.txtBrojDecD.Location = new System.Drawing.Point(83, 41);
            this.txtBrojDecD.Name = "txtBrojDecD";
            this.txtBrojDecD.Size = new System.Drawing.Size(32, 20);
            this.txtBrojDecD.TabIndex = 2;
            // 
            // lblBrojDecE1
            // 
            this.lblBrojDecE1.AutoSize = true;
            this.lblBrojDecE1.Location = new System.Drawing.Point(40, 70);
            this.lblBrojDecE1.Name = "lblBrojDecE1";
            this.lblBrojDecE1.Size = new System.Drawing.Size(36, 13);
            this.lblBrojDecE1.TabIndex = 3;
            this.lblBrojDecE1.Text = "E1-E6";
            // 
            // txtBrojDecE1
            // 
            this.txtBrojDecE1.Location = new System.Drawing.Point(83, 67);
            this.txtBrojDecE1.Name = "txtBrojDecE1";
            this.txtBrojDecE1.Size = new System.Drawing.Size(32, 20);
            this.txtBrojDecE1.TabIndex = 4;
            // 
            // lblBrojDecE
            // 
            this.lblBrojDecE.AutoSize = true;
            this.lblBrojDecE.Location = new System.Drawing.Point(64, 96);
            this.lblBrojDecE.Name = "lblBrojDecE";
            this.lblBrojDecE.Size = new System.Drawing.Size(14, 13);
            this.lblBrojDecE.TabIndex = 5;
            this.lblBrojDecE.Text = "E";
            // 
            // txtBrojDecE
            // 
            this.txtBrojDecE.Location = new System.Drawing.Point(83, 93);
            this.txtBrojDecE.Name = "txtBrojDecE";
            this.txtBrojDecE.Size = new System.Drawing.Size(32, 20);
            this.txtBrojDecE.TabIndex = 6;
            // 
            // lblPen
            // 
            this.lblPen.AutoSize = true;
            this.lblPen.Location = new System.Drawing.Point(40, 122);
            this.lblPen.Name = "lblPen";
            this.lblPen.Size = new System.Drawing.Size(37, 13);
            this.lblPen.TabIndex = 7;
            this.lblPen.Text = "Penal.";
            // 
            // txtBrojDecPen
            // 
            this.txtBrojDecPen.Location = new System.Drawing.Point(83, 119);
            this.txtBrojDecPen.Name = "txtBrojDecPen";
            this.txtBrojDecPen.Size = new System.Drawing.Size(32, 20);
            this.txtBrojDecPen.TabIndex = 8;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(16, 148);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(62, 13);
            this.lblTotal.TabIndex = 9;
            this.lblTotal.Text = "Kon. ocena";
            // 
            // txtBrojDecTotal
            // 
            this.txtBrojDecTotal.Location = new System.Drawing.Point(83, 145);
            this.txtBrojDecTotal.Name = "txtBrojDecTotal";
            this.txtBrojDecTotal.Size = new System.Drawing.Size(32, 20);
            this.txtBrojDecTotal.TabIndex = 10;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(281, 231);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(373, 231);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // OpcijeForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(478, 266);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtBrojDecTotal);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.txtBrojDecPen);
            this.Controls.Add(this.lblPen);
            this.Controls.Add(this.txtBrojDecE);
            this.Controls.Add(this.lblBrojDecE);
            this.Controls.Add(this.txtBrojDecE1);
            this.Controls.Add(this.lblBrojDecE1);
            this.Controls.Add(this.txtBrojDecD);
            this.Controls.Add(this.lblBrojDecD);
            this.Controls.Add(this.lblBrojDecimala);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpcijeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpcijeForm";
            this.Shown += new System.EventHandler(this.OpcijeForm_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OpcijeForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpcijeForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBrojDecimala;
        private System.Windows.Forms.Label lblBrojDecD;
        private System.Windows.Forms.TextBox txtBrojDecD;
        private System.Windows.Forms.Label lblBrojDecE1;
        private System.Windows.Forms.TextBox txtBrojDecE1;
        private System.Windows.Forms.Label lblBrojDecE;
        private System.Windows.Forms.TextBox txtBrojDecE;
        private System.Windows.Forms.Label lblPen;
        private System.Windows.Forms.TextBox txtBrojDecPen;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.TextBox txtBrojDecTotal;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}