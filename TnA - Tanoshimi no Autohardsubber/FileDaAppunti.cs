using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace TnA___Tanoshimi_no_Autohardsubber
{
    public partial class FileDaAppunti : Form
    {

        public static List<String> sorg = new List<String>();

        public FileDaAppunti(String[] files)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT", false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT", false);
            InitializeComponent();
            foreach (String s in files)
            {
                DGV_files.Rows.Add(s);
            }
        }

        private void b_rimuovi_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow d in DGV_files.SelectedRows)
            {
                DGV_files.Rows.Remove(d);
            }
        }

        private void b_conferma_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow d in DGV_files.Rows)
            {
                sorg.Add(d.Cells[0].Value.ToString());
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
