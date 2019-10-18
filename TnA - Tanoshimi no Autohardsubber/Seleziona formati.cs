using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace TnA___Tanoshimi_no_Autohardsubber
{
    public partial class Seleziona_formati : Form
    {

        public static List<String> formati_scelti = new List<string>();

        public Seleziona_formati(String[] estensioni)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT", false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT", false);
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.Text = "Seleziona i formati da considerare";
            clb_estensioni.Items.Clear();
            clb_estensioni.Sorted = true;
            foreach(String s in estensioni)
            {
                clb_estensioni.Items.Add("Solo file " + s);
            }
        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            salva();
        }

        public void salva()
        {
            formati_scelti = new List<String>();
            for (Int32 i = 0; i < clb_estensioni.CheckedItems.Count; i++)
            {
                String temp = clb_estensioni.GetItemText(clb_estensioni.CheckedItems[i]);
                temp = temp.Replace("Solo file ", "");
                temp = temp.Trim();
                formati_scelti.Add(temp);
                //MessageBox.Show(TnA.formati_scelti[i]);
            }
        }

        private void b_sel_all_Click(object sender, EventArgs e)
        {
            for(Int32 i=0; i<clb_estensioni.Items.Count; i++)
            {
                clb_estensioni.SetItemChecked(i, true);
            }
        }

        private void b_des_all_Click(object sender, EventArgs e)
        {
            for (Int32 i = 0; i < clb_estensioni.Items.Count; i++)
            {
                clb_estensioni.SetItemChecked(i, false);
            }
        }

        private void b_inv_sel_Click(object sender, EventArgs e)
        {
            for (Int32 i = 0; i < clb_estensioni.Items.Count; i++)
            {
                if (clb_estensioni.GetItemCheckState(i) == CheckState.Unchecked)
                    clb_estensioni.SetItemChecked(i, true);
                else
                    clb_estensioni.SetItemChecked(i, false);
            }
        }

        private void Seleziona_formati_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void b_annulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
