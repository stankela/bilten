namespace Bilten.UI
{
    partial class SpravaGridGroupUserControl
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
            this.spravaGridUserControl1 = new Bilten.UI.SpravaGridUserControl();
            this.SuspendLayout();
            // 
            // spravaGridUserControl1
            // 
            this.spravaGridUserControl1.Location = new System.Drawing.Point(0, 0);
            this.spravaGridUserControl1.Name = "spravaGridUserControl1";
            this.spravaGridUserControl1.Size = new System.Drawing.Size(501, 275);
            this.spravaGridUserControl1.TabIndex = 0;
            // 
            // SpravaGridGroupUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spravaGridUserControl1);
            this.Name = "SpravaGridGroupUserControl";
            this.Size = new System.Drawing.Size(606, 371);
            this.ResumeLayout(false);

        }

        #endregion

        private SpravaGridUserControl spravaGridUserControl1;
    }
}
