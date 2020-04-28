using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace TnA___Tanoshimi_no_Autohardsubber
{
    public partial class About : Form
    {
        
        public About(String titolo, String versione)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT", false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT", false);
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            l_titolo.Text = titolo;
            l_vers.Text = "Versione: " + versione;
            this.Text = "Informazioni - " + titolo;
        }

        private void pb_logo_Click(object sender, EventArgs e)
        {
            Process.Start("https://tnsfansub.com/");
        }

        private void pb_logo_MouseEnter(object sender, EventArgs e)
        {
            l_sito.Visible = true;
        }

        private void pb_logo_MouseLeave(object sender, EventArgs e)
        {
            l_sito.Visible = false;
        }

        private void ll_source_code_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/KonjikiNoYami1991/TnA");
        }

        private void ll_license_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("notepad.exe", Application.StartupPath + "\\LICENSE");
        }
    }
}
