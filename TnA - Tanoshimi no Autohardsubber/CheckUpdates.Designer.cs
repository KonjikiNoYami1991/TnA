namespace Updater
{
    partial class TnA_Updater
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TnA_Updater));
            this.b_verifica = new System.Windows.Forms.Button();
            this.b_agg = new System.Windows.Forms.Button();
            this.l_vers_install = new System.Windows.Forms.Label();
            this.pb_perc_down = new System.Windows.Forms.ProgressBar();
            this.l_vers_att = new System.Windows.Forms.Label();
            this.l_dim_down = new System.Windows.Forms.Label();
            this.b_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // b_verifica
            // 
            this.b_verifica.Location = new System.Drawing.Point(307, 12);
            this.b_verifica.Name = "b_verifica";
            this.b_verifica.Size = new System.Drawing.Size(294, 23);
            this.b_verifica.TabIndex = 1;
            this.b_verifica.Text = "VERIFICA DI NUOVO";
            this.b_verifica.UseVisualStyleBackColor = true;
            this.b_verifica.Click += new System.EventHandler(this.b_verifica_Click);
            // 
            // b_agg
            // 
            this.b_agg.Location = new System.Drawing.Point(307, 52);
            this.b_agg.Name = "b_agg";
            this.b_agg.Size = new System.Drawing.Size(294, 23);
            this.b_agg.TabIndex = 2;
            this.b_agg.Text = "SCARICA AGGIORNAMENTO";
            this.b_agg.UseVisualStyleBackColor = true;
            this.b_agg.Click += new System.EventHandler(this.b_agg_Click);
            // 
            // l_vers_install
            // 
            this.l_vers_install.AutoSize = true;
            this.l_vers_install.Location = new System.Drawing.Point(12, 18);
            this.l_vers_install.Name = "l_vers_install";
            this.l_vers_install.Size = new System.Drawing.Size(95, 13);
            this.l_vers_install.TabIndex = 3;
            this.l_vers_install.Text = "Versione installata:";
            // 
            // pb_perc_down
            // 
            this.pb_perc_down.Enabled = false;
            this.pb_perc_down.Location = new System.Drawing.Point(15, 92);
            this.pb_perc_down.Name = "pb_perc_down";
            this.pb_perc_down.Size = new System.Drawing.Size(277, 23);
            this.pb_perc_down.TabIndex = 4;
            // 
            // l_vers_att
            // 
            this.l_vers_att.AutoSize = true;
            this.l_vers_att.Location = new System.Drawing.Point(12, 43);
            this.l_vers_att.Name = "l_vers_att";
            this.l_vers_att.Size = new System.Drawing.Size(84, 13);
            this.l_vers_att.TabIndex = 5;
            this.l_vers_att.Text = "Versione nuova:";
            // 
            // l_dim_down
            // 
            this.l_dim_down.AutoSize = true;
            this.l_dim_down.Enabled = false;
            this.l_dim_down.Location = new System.Drawing.Point(12, 76);
            this.l_dim_down.Name = "l_dim_down";
            this.l_dim_down.Size = new System.Drawing.Size(85, 13);
            this.l_dim_down.TabIndex = 8;
            this.l_dim_down.Text = "0/0 MB scaricati";
            // 
            // b_cancel
            // 
            this.b_cancel.Location = new System.Drawing.Point(307, 92);
            this.b_cancel.Name = "b_cancel";
            this.b_cancel.Size = new System.Drawing.Size(294, 23);
            this.b_cancel.TabIndex = 9;
            this.b_cancel.Text = "NON AGGIORNARE";
            this.b_cancel.UseVisualStyleBackColor = true;
            this.b_cancel.Click += new System.EventHandler(this.b_cancel_Click);
            // 
            // TnA_Updater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 126);
            this.ControlBox = false;
            this.Controls.Add(this.b_cancel);
            this.Controls.Add(this.l_dim_down);
            this.Controls.Add(this.l_vers_att);
            this.Controls.Add(this.pb_perc_down);
            this.Controls.Add(this.l_vers_install);
            this.Controls.Add(this.b_agg);
            this.Controls.Add(this.b_verifica);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(633, 169);
            this.MinimumSize = new System.Drawing.Size(633, 169);
            this.Name = "TnA_Updater";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TnA Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button b_verifica;
        private System.Windows.Forms.Button b_agg;
        private System.Windows.Forms.Label l_vers_install;
        private System.Windows.Forms.ProgressBar pb_perc_down;
        private System.Windows.Forms.Label l_vers_att;
        private System.Windows.Forms.Label l_dim_down;
        private System.Windows.Forms.Button b_cancel;
    }
}

