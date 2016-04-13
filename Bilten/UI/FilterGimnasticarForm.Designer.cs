namespace Bilten.UI
{
    partial class FilterGimnasticarForm
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
            this.filterGimnasticarUserControl1 = new Bilten.UI.FilterGimnasticarUserControl();
            this.SuspendLayout();
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(302, 177);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(484, 177);
            // 
            // btnResetFilter
            // 
            this.btnResetFilter.Location = new System.Drawing.Point(394, 177);
            // 
            // filterGimnasticarUserControl1
            // 
            this.filterGimnasticarUserControl1.Location = new System.Drawing.Point(12, 21);
            this.filterGimnasticarUserControl1.Name = "filterGimnasticarUserControl1";
            this.filterGimnasticarUserControl1.Size = new System.Drawing.Size(561, 127);
            this.filterGimnasticarUserControl1.TabIndex = 3;
            // 
            // FilterGimnasticarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(586, 212);
            this.Controls.Add(this.filterGimnasticarUserControl1);
            this.Name = "FilterGimnasticarForm";
            this.Controls.SetChildIndex(this.btnResetFilter, 0);
            this.Controls.SetChildIndex(this.filterGimnasticarUserControl1, 0);
            this.Controls.SetChildIndex(this.btnFilter, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private FilterGimnasticarUserControl filterGimnasticarUserControl1;
    }
}
