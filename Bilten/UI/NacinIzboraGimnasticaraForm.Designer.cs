namespace Bilten.UI
{
    partial class NacinIzboraGimnasticaraForm
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
            this.rbtIzRegistra = new System.Windows.Forms.RadioButton();
            this.rbtIzTakmicenja = new System.Windows.Forms.RadioButton();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbtIzRegistra
            // 
            this.rbtIzRegistra.AutoSize = true;
            this.rbtIzRegistra.Location = new System.Drawing.Point(24, 27);
            this.rbtIzRegistra.Name = "rbtIzRegistra";
            this.rbtIzRegistra.Size = new System.Drawing.Size(167, 17);
            this.rbtIzRegistra.TabIndex = 0;
            this.rbtIzRegistra.TabStop = true;
            this.rbtIzRegistra.Text = "Izaberi gimnasticare iz reigistra";
            this.rbtIzRegistra.UseVisualStyleBackColor = true;
            // 
            // rbtIzTakmicenja
            // 
            this.rbtIzTakmicenja.AutoSize = true;
            this.rbtIzTakmicenja.Location = new System.Drawing.Point(24, 60);
            this.rbtIzTakmicenja.Name = "rbtIzTakmicenja";
            this.rbtIzTakmicenja.Size = new System.Drawing.Size(239, 17);
            this.rbtIzTakmicenja.TabIndex = 1;
            this.rbtIzTakmicenja.TabStop = true;
            this.rbtIzTakmicenja.Text = "Izaberi gimnasticare iz prethodnog takmicenja";
            this.rbtIzTakmicenja.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(96, 111);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(188, 111);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // NacinIzboraGimnasticaraForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(291, 160);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.rbtIzTakmicenja);
            this.Controls.Add(this.rbtIzRegistra);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NacinIzboraGimnasticaraForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Izbor gimnasticara";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtIzRegistra;
        private System.Windows.Forms.RadioButton rbtIzTakmicenja;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

    }
}