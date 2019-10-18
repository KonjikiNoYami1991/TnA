using System;
using System.Drawing;
using System.Windows.Forms;

namespace TnA___Tanoshimi_no_Autohardsubber
{
    public partial class PasswordStaffer : Form
    {

        String password = String.Empty;
        public static String pswd_digitata = String.Empty;

        public PasswordStaffer(String pswd)
        {
            InitializeComponent();
            this.password = pswd;
        }

        private void cb_visual_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_visual.Checked == true)
            {
                tb_pswd.UseSystemPasswordChar = false;
            }
            else
                tb_pswd.UseSystemPasswordChar = true;
        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            if (tb_pswd.Text == password)
            {
                pswd_digitata = tb_pswd.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                tb_pswd.BackColor = Color.Red;
            }
        }

        private void tb_pswd_TextChanged(object sender, EventArgs e)
        {
            tb_pswd.BackColor = Color.White;
        }

        private void tb_pswd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                if (tb_pswd.Text == password)
                {
                    pswd_digitata = tb_pswd.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    tb_pswd.BackColor = Color.Red;
                }
            }
        }
    }
}
