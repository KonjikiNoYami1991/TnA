namespace TnA___Tanoshimi_no_Autohardsubber
{
    partial class PasswordStaffer
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
            this.tb_pswd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.b_ok = new System.Windows.Forms.Button();
            this.cb_visual = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tb_pswd
            // 
            this.tb_pswd.Location = new System.Drawing.Point(12, 34);
            this.tb_pswd.Name = "tb_pswd";
            this.tb_pswd.Size = new System.Drawing.Size(481, 20);
            this.tb_pswd.TabIndex = 0;
            this.tb_pswd.UseSystemPasswordChar = true;
            this.tb_pswd.TextChanged += new System.EventHandler(this.tb_pswd_TextChanged);
            this.tb_pswd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_pswd_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Digita la password per attivare la modalità staffer TnS.";
            // 
            // b_ok
            // 
            this.b_ok.Location = new System.Drawing.Point(499, 33);
            this.b_ok.Name = "b_ok";
            this.b_ok.Size = new System.Drawing.Size(75, 20);
            this.b_ok.TabIndex = 2;
            this.b_ok.Text = "OK";
            this.b_ok.UseVisualStyleBackColor = true;
            this.b_ok.Click += new System.EventHandler(this.b_ok_Click);
            // 
            // cb_visual
            // 
            this.cb_visual.AutoSize = true;
            this.cb_visual.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cb_visual.Location = new System.Drawing.Point(454, 10);
            this.cb_visual.Name = "cb_visual";
            this.cb_visual.Size = new System.Drawing.Size(120, 17);
            this.cb_visual.TabIndex = 3;
            this.cb_visual.Text = "Visualizza password";
            this.cb_visual.UseVisualStyleBackColor = true;
            this.cb_visual.CheckedChanged += new System.EventHandler(this.cb_visual_CheckedChanged);
            // 
            // PasswordStaffer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 65);
            this.Controls.Add(this.cb_visual);
            this.Controls.Add(this.b_ok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_pswd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasswordStaffer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Modalità staffer TnS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_pswd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cb_visual;
        public System.Windows.Forms.Button b_ok;
    }
}