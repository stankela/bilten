namespace Bilten.UI
{
    partial class RotacijeForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtRotirajEkipeRotirajGim = new System.Windows.Forms.RadioButton();
            this.rbtNerotirajEkipeRotGim = new System.Windows.Forms.RadioButton();
            this.rbtRotirajSve = new System.Windows.Forms.RadioButton();
            this.rbtNerotirajNista = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbtJednaSprava = new System.Windows.Forms.RadioButton();
            this.rbtDveSprave = new System.Windows.Forms.RadioButton();
            this.rbtTriSprave = new System.Windows.Forms.RadioButton();
            this.rbtSestSprava = new System.Windows.Forms.RadioButton();
            this.rbtCetiriSprave = new System.Windows.Forms.RadioButton();
            this.lblBrojSprava = new System.Windows.Forms.Label();
            this.lblNacinRotacije = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtNerotirajNista);
            this.panel1.Controls.Add(this.rbtRotirajSve);
            this.panel1.Controls.Add(this.rbtNerotirajEkipeRotGim);
            this.panel1.Controls.Add(this.rbtRotirajEkipeRotirajGim);
            this.panel1.Location = new System.Drawing.Point(23, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(339, 107);
            this.panel1.TabIndex = 0;
            // 
            // rbtRotirajEkipeRotirajGim
            // 
            this.rbtRotirajEkipeRotirajGim.AutoSize = true;
            this.rbtRotirajEkipeRotirajGim.Location = new System.Drawing.Point(4, 6);
            this.rbtRotirajEkipeRotirajGim.Name = "rbtRotirajEkipeRotirajGim";
            this.rbtRotirajEkipeRotirajGim.Size = new System.Drawing.Size(239, 17);
            this.rbtRotirajEkipeRotirajGim.TabIndex = 0;
            this.rbtRotirajEkipeRotirajGim.TabStop = true;
            this.rbtRotirajEkipeRotirajGim.Text = "Rotiraj ekipe, rotiraj gimnasticare unutar ekipe";
            this.rbtRotirajEkipeRotirajGim.UseVisualStyleBackColor = true;
            // 
            // rbtNerotirajEkipeRotGim
            // 
            this.rbtNerotirajEkipeRotGim.AutoSize = true;
            this.rbtNerotirajEkipeRotGim.Location = new System.Drawing.Point(4, 29);
            this.rbtNerotirajEkipeRotGim.Name = "rbtNerotirajEkipeRotGim";
            this.rbtNerotirajEkipeRotGim.Size = new System.Drawing.Size(251, 17);
            this.rbtNerotirajEkipeRotGim.TabIndex = 1;
            this.rbtNerotirajEkipeRotGim.TabStop = true;
            this.rbtNerotirajEkipeRotGim.Text = "Ne rotiraj ekipe, rotiraj gimnasticare unutar ekipe";
            this.rbtNerotirajEkipeRotGim.UseVisualStyleBackColor = true;
            // 
            // rbtRotirajSve
            // 
            this.rbtRotirajSve.AutoSize = true;
            this.rbtRotirajSve.Location = new System.Drawing.Point(4, 52);
            this.rbtRotirajSve.Name = "rbtRotirajSve";
            this.rbtRotirajSve.Size = new System.Drawing.Size(232, 17);
            this.rbtRotirajSve.TabIndex = 2;
            this.rbtRotirajSve.TabStop = true;
            this.rbtRotirajSve.Text = "Rotiraj sve gimnasticare nezavisno od ekipe";
            this.rbtRotirajSve.UseVisualStyleBackColor = true;
            // 
            // rbtNerotirajNista
            // 
            this.rbtNerotirajNista.AutoSize = true;
            this.rbtNerotirajNista.Location = new System.Drawing.Point(4, 75);
            this.rbtNerotirajNista.Name = "rbtNerotirajNista";
            this.rbtNerotirajNista.Size = new System.Drawing.Size(92, 17);
            this.rbtNerotirajNista.TabIndex = 3;
            this.rbtNerotirajNista.TabStop = true;
            this.rbtNerotirajNista.Text = "Ne rotiraj nista";
            this.rbtNerotirajNista.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbtCetiriSprave);
            this.panel2.Controls.Add(this.rbtSestSprava);
            this.panel2.Controls.Add(this.rbtTriSprave);
            this.panel2.Controls.Add(this.rbtDveSprave);
            this.panel2.Controls.Add(this.rbtJednaSprava);
            this.panel2.Location = new System.Drawing.Point(23, 176);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 123);
            this.panel2.TabIndex = 1;
            // 
            // rbtJednaRotacija
            // 
            this.rbtJednaSprava.AutoSize = true;
            this.rbtJednaSprava.Location = new System.Drawing.Point(4, 5);
            this.rbtJednaSprava.Name = "rbtJednaRotacija";
            this.rbtJednaSprava.Size = new System.Drawing.Size(31, 17);
            this.rbtJednaSprava.TabIndex = 0;
            this.rbtJednaSprava.TabStop = true;
            this.rbtJednaSprava.Text = "1";
            this.rbtJednaSprava.UseVisualStyleBackColor = true;
            // 
            // rbtDveRotacije
            // 
            this.rbtDveSprave.AutoSize = true;
            this.rbtDveSprave.Location = new System.Drawing.Point(4, 28);
            this.rbtDveSprave.Name = "rbtDveRotacije";
            this.rbtDveSprave.Size = new System.Drawing.Size(31, 17);
            this.rbtDveSprave.TabIndex = 1;
            this.rbtDveSprave.TabStop = true;
            this.rbtDveSprave.Text = "2";
            this.rbtDveSprave.UseVisualStyleBackColor = true;
            // 
            // rbtTriRotacije
            // 
            this.rbtTriSprave.AutoSize = true;
            this.rbtTriSprave.Location = new System.Drawing.Point(4, 51);
            this.rbtTriSprave.Name = "rbtTriRotacije";
            this.rbtTriSprave.Size = new System.Drawing.Size(31, 17);
            this.rbtTriSprave.TabIndex = 2;
            this.rbtTriSprave.TabStop = true;
            this.rbtTriSprave.Text = "3";
            this.rbtTriSprave.UseVisualStyleBackColor = true;
            // 
            // rbtSestRotacija
            // 
            this.rbtSestSprava.AutoSize = true;
            this.rbtSestSprava.Location = new System.Drawing.Point(4, 97);
            this.rbtSestSprava.Name = "rbtSestRotacija";
            this.rbtSestSprava.Size = new System.Drawing.Size(31, 17);
            this.rbtSestSprava.TabIndex = 3;
            this.rbtSestSprava.TabStop = true;
            this.rbtSestSprava.Text = "6";
            this.rbtSestSprava.UseVisualStyleBackColor = true;
            // 
            // rbtCetiriRotacije
            // 
            this.rbtCetiriSprave.AutoSize = true;
            this.rbtCetiriSprave.Location = new System.Drawing.Point(4, 74);
            this.rbtCetiriSprave.Name = "rbtCetiriRotacije";
            this.rbtCetiriSprave.Size = new System.Drawing.Size(31, 17);
            this.rbtCetiriSprave.TabIndex = 4;
            this.rbtCetiriSprave.TabStop = true;
            this.rbtCetiriSprave.Text = "4";
            this.rbtCetiriSprave.UseVisualStyleBackColor = true;
            // 
            // lblBrojSprava
            // 
            this.lblBrojSprava.AutoSize = true;
            this.lblBrojSprava.Location = new System.Drawing.Point(23, 160);
            this.lblBrojSprava.Name = "lblBrojSprava";
            this.lblBrojSprava.Size = new System.Drawing.Size(131, 13);
            this.lblBrojSprava.TabIndex = 2;
            this.lblBrojSprava.Text = "Broj sprava koje se rotiraju";
            // 
            // lblNacinRotacije
            // 
            this.lblNacinRotacije.AutoSize = true;
            this.lblNacinRotacije.Location = new System.Drawing.Point(24, 20);
            this.lblNacinRotacije.Name = "lblNacinRotacije";
            this.lblNacinRotacije.Size = new System.Drawing.Size(72, 13);
            this.lblNacinRotacije.TabIndex = 3;
            this.lblNacinRotacije.Text = "Nacin rotacije";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(259, 299);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(351, 299);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // RotacijeForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(455, 340);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblNacinRotacije);
            this.Controls.Add(this.lblBrojSprava);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RotacijeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nacin rotacije";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbtRotirajSve;
        private System.Windows.Forms.RadioButton rbtNerotirajEkipeRotGim;
        private System.Windows.Forms.RadioButton rbtRotirajEkipeRotirajGim;
        private System.Windows.Forms.RadioButton rbtNerotirajNista;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbtSestSprava;
        private System.Windows.Forms.RadioButton rbtTriSprave;
        private System.Windows.Forms.RadioButton rbtDveSprave;
        private System.Windows.Forms.RadioButton rbtJednaSprava;
        private System.Windows.Forms.RadioButton rbtCetiriSprave;
        private System.Windows.Forms.Label lblBrojSprava;
        private System.Windows.Forms.Label lblNacinRotacije;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}