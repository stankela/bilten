namespace Bilten.UI
{
    partial class FilterGimnasticarUcesnikForm
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
            this.filterGimnasticarUcesnikUserControl1 = new Bilten.UI.FilterGimnasticarUcesnikUserControl();
            this.SuspendLayout();
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(191, 132);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(373, 132);
            // 
            // btnResetFilter
            // 
            this.btnResetFilter.Location = new System.Drawing.Point(283, 132);
            // 
            // filterGimnasticarUcesnikUserControl1
            // 
            this.filterGimnasticarUcesnikUserControl1.Location = new System.Drawing.Point(19, 12);
            this.filterGimnasticarUcesnikUserControl1.Name = "filterGimnasticarUcesnikUserControl1";
            this.filterGimnasticarUcesnikUserControl1.Size = new System.Drawing.Size(438, 88);
            this.filterGimnasticarUcesnikUserControl1.TabIndex = 4;
            // 
            // FilterGimnasticarUcesnikForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 171);
            this.Controls.Add(this.filterGimnasticarUcesnikUserControl1);
            this.Name = "FilterGimnasticarUcesnikForm";
            this.Text = "FilterGimnasticarUcesnikForm";
            this.Controls.SetChildIndex(this.btnResetFilter, 0);
            this.Controls.SetChildIndex(this.btnFilter, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.filterGimnasticarUcesnikUserControl1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private FilterGimnasticarUcesnikUserControl filterGimnasticarUcesnikUserControl1;
    }
}