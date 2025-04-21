namespace Bilten.UI
{
    partial class AdminForm
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
            this.btnUpdateFinalaKupa = new System.Windows.Forms.Button();
            this.btnUpdateZbirViseKola = new System.Windows.Forms.Button();
            this.btnDumpFinaleKupa = new System.Windows.Forms.Button();
            this.btnDumpZbirViseKola = new System.Windows.Forms.Button();
            this.btnIspraviFinalaKupa = new System.Windows.Forms.Button();
            this.btnIspraviZbirViseKola = new System.Windows.Forms.Button();
            this.btnDumpStandardnaTakmicenja = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUpdateFinalaKupa
            // 
            this.btnUpdateFinalaKupa.Location = new System.Drawing.Point(24, 25);
            this.btnUpdateFinalaKupa.Name = "btnUpdateFinalaKupa";
            this.btnUpdateFinalaKupa.Size = new System.Drawing.Size(115, 23);
            this.btnUpdateFinalaKupa.TabIndex = 0;
            this.btnUpdateFinalaKupa.Text = "Update finala kupa";
            this.btnUpdateFinalaKupa.UseVisualStyleBackColor = true;
            this.btnUpdateFinalaKupa.Click += new System.EventHandler(this.btnUpdateFinalaKupa_Click);
            // 
            // btnUpdateZbirViseKola
            // 
            this.btnUpdateZbirViseKola.Location = new System.Drawing.Point(24, 71);
            this.btnUpdateZbirViseKola.Name = "btnUpdateZbirViseKola";
            this.btnUpdateZbirViseKola.Size = new System.Drawing.Size(115, 23);
            this.btnUpdateZbirViseKola.TabIndex = 1;
            this.btnUpdateZbirViseKola.Text = "Update zbir vise kola";
            this.btnUpdateZbirViseKola.UseVisualStyleBackColor = true;
            this.btnUpdateZbirViseKola.Click += new System.EventHandler(this.btnUpdateZbirViseKola_Click);
            // 
            // btnDumpFinaleKupa
            // 
            this.btnDumpFinaleKupa.Location = new System.Drawing.Point(24, 119);
            this.btnDumpFinaleKupa.Name = "btnDumpFinaleKupa";
            this.btnDumpFinaleKupa.Size = new System.Drawing.Size(115, 23);
            this.btnDumpFinaleKupa.TabIndex = 2;
            this.btnDumpFinaleKupa.Text = "Dump finale kupa";
            this.btnDumpFinaleKupa.UseVisualStyleBackColor = true;
            this.btnDumpFinaleKupa.Click += new System.EventHandler(this.btnDumpFinaleKupa_Click);
            // 
            // btnDumpZbirViseKola
            // 
            this.btnDumpZbirViseKola.Location = new System.Drawing.Point(24, 163);
            this.btnDumpZbirViseKola.Name = "btnDumpZbirViseKola";
            this.btnDumpZbirViseKola.Size = new System.Drawing.Size(115, 23);
            this.btnDumpZbirViseKola.TabIndex = 3;
            this.btnDumpZbirViseKola.Text = "Dump zbir vise kola";
            this.btnDumpZbirViseKola.UseVisualStyleBackColor = true;
            this.btnDumpZbirViseKola.Click += new System.EventHandler(this.btnDumpZbirViseKola_Click);
            // 
            // btnIspraviFinalaKupa
            // 
            this.btnIspraviFinalaKupa.Location = new System.Drawing.Point(173, 25);
            this.btnIspraviFinalaKupa.Name = "btnIspraviFinalaKupa";
            this.btnIspraviFinalaKupa.Size = new System.Drawing.Size(110, 23);
            this.btnIspraviFinalaKupa.TabIndex = 4;
            this.btnIspraviFinalaKupa.Text = "Ispravi finala kupa";
            this.btnIspraviFinalaKupa.UseVisualStyleBackColor = true;
            this.btnIspraviFinalaKupa.Click += new System.EventHandler(this.btnIspraviFinalaKupa_Click);
            // 
            // btnIspraviZbirViseKola
            // 
            this.btnIspraviZbirViseKola.Location = new System.Drawing.Point(173, 71);
            this.btnIspraviZbirViseKola.Name = "btnIspraviZbirViseKola";
            this.btnIspraviZbirViseKola.Size = new System.Drawing.Size(110, 23);
            this.btnIspraviZbirViseKola.TabIndex = 5;
            this.btnIspraviZbirViseKola.Text = "Ispravi zbir vise kola";
            this.btnIspraviZbirViseKola.UseVisualStyleBackColor = true;
            this.btnIspraviZbirViseKola.Click += new System.EventHandler(this.btnIspraviZbirViseKola_Click);
            // 
            // btnDumpStandardnaTakmicenja
            // 
            this.btnDumpStandardnaTakmicenja.Location = new System.Drawing.Point(24, 203);
            this.btnDumpStandardnaTakmicenja.Name = "btnDumpStandardnaTakmicenja";
            this.btnDumpStandardnaTakmicenja.Size = new System.Drawing.Size(115, 23);
            this.btnDumpStandardnaTakmicenja.TabIndex = 6;
            this.btnDumpStandardnaTakmicenja.Text = "Dump std takmicenja";
            this.btnDumpStandardnaTakmicenja.UseVisualStyleBackColor = true;
            this.btnDumpStandardnaTakmicenja.Click += new System.EventHandler(this.btnDumpStandardnaTakmicenja_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 262);
            this.Controls.Add(this.btnDumpStandardnaTakmicenja);
            this.Controls.Add(this.btnIspraviZbirViseKola);
            this.Controls.Add(this.btnIspraviFinalaKupa);
            this.Controls.Add(this.btnDumpZbirViseKola);
            this.Controls.Add(this.btnDumpFinaleKupa);
            this.Controls.Add(this.btnUpdateZbirViseKola);
            this.Controls.Add(this.btnUpdateFinalaKupa);
            this.Name = "AdminForm";
            this.Text = "AdminForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUpdateFinalaKupa;
        private System.Windows.Forms.Button btnUpdateZbirViseKola;
        private System.Windows.Forms.Button btnDumpFinaleKupa;
        private System.Windows.Forms.Button btnDumpZbirViseKola;
        private System.Windows.Forms.Button btnIspraviFinalaKupa;
        private System.Windows.Forms.Button btnIspraviZbirViseKola;
        private System.Windows.Forms.Button btnDumpStandardnaTakmicenja;
    }
}