namespace Bilten.UI
{
    partial class SpravaGridUserControl
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
            this.pictureBoxSprava = new System.Windows.Forms.PictureBox();
            this.dataGridViewUserControl1 = new Bilten.UI.DataGridViewUserControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSprava)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxSprava
            // 
            this.pictureBoxSprava.BackColor = System.Drawing.Color.White;
            this.pictureBoxSprava.Location = new System.Drawing.Point(152, 3);
            this.pictureBoxSprava.Name = "pictureBoxSprava";
            this.pictureBoxSprava.Size = new System.Drawing.Size(46, 46);
            this.pictureBoxSprava.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxSprava.TabIndex = 1;
            this.pictureBoxSprava.TabStop = false;
            // 
            // dataGridViewUserControl1
            // 
            this.dataGridViewUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserControl1.Location = new System.Drawing.Point(0, 58);
            this.dataGridViewUserControl1.Name = "dataGridViewUserControl1";
            this.dataGridViewUserControl1.Size = new System.Drawing.Size(358, 217);
            this.dataGridViewUserControl1.TabIndex = 0;
            // 
            // SpravaGridUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBoxSprava);
            this.Controls.Add(this.dataGridViewUserControl1);
            this.Name = "SpravaGridUserControl";
            this.Size = new System.Drawing.Size(358, 275);
            this.Resize += new System.EventHandler(this.SpravaGridUserControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSprava)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridViewUserControl dataGridViewUserControl1;
        private System.Windows.Forms.PictureBox pictureBoxSprava;
    }
}
