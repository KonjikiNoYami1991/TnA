namespace TnA___Tanoshimi_no_Autohardsubber
{
    partial class FileDaAppunti
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
            this.DGV_files = new System.Windows.Forms.DataGridView();
            this.b_conferma = new System.Windows.Forms.Button();
            this.b_rimuovi = new System.Windows.Forms.Button();
            this.file = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_files)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_files
            // 
            this.DGV_files.AllowUserToAddRows = false;
            this.DGV_files.AllowUserToResizeColumns = false;
            this.DGV_files.AllowUserToResizeRows = false;
            this.DGV_files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGV_files.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_files.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.file});
            this.DGV_files.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.DGV_files.Location = new System.Drawing.Point(12, 12);
            this.DGV_files.Name = "DGV_files";
            this.DGV_files.Size = new System.Drawing.Size(728, 208);
            this.DGV_files.TabIndex = 0;
            // 
            // b_conferma
            // 
            this.b_conferma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_conferma.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_conferma.Location = new System.Drawing.Point(599, 226);
            this.b_conferma.Name = "b_conferma";
            this.b_conferma.Size = new System.Drawing.Size(141, 23);
            this.b_conferma.TabIndex = 2;
            this.b_conferma.Text = "Conferma";
            this.b_conferma.UseVisualStyleBackColor = true;
            this.b_conferma.Click += new System.EventHandler(this.b_conferma_Click);
            // 
            // b_rimuovi
            // 
            this.b_rimuovi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_rimuovi.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_rimuovi.Location = new System.Drawing.Point(12, 226);
            this.b_rimuovi.Name = "b_rimuovi";
            this.b_rimuovi.Size = new System.Drawing.Size(141, 23);
            this.b_rimuovi.TabIndex = 3;
            this.b_rimuovi.Text = "Rimuovi selezionati";
            this.b_rimuovi.UseVisualStyleBackColor = true;
            this.b_rimuovi.Click += new System.EventHandler(this.b_rimuovi_Click);
            // 
            // file
            // 
            this.file.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.file.HeaderText = "File";
            this.file.Name = "file";
            this.file.ReadOnly = true;
            this.file.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FileDaAppunti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 261);
            this.Controls.Add(this.b_rimuovi);
            this.Controls.Add(this.b_conferma);
            this.Controls.Add(this.DGV_files);
            this.Name = "FileDaAppunti";
            this.Text = "FileDaAppunti";
            ((System.ComponentModel.ISupportInitialize)(this.DGV_files)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button b_conferma;
        private System.Windows.Forms.Button b_rimuovi;
        public System.Windows.Forms.DataGridView DGV_files;
        private System.Windows.Forms.DataGridViewTextBoxColumn file;
    }
}