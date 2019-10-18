using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace TnA___Tanoshimi_no_Autohardsubber
{
    public partial class TestFFMPEG : Form
    {

        DialogResult risultato = DialogResult.No;
        Boolean chiudi = false;

        public TestFFMPEG(Boolean x264, Boolean aac, Boolean ac3, Boolean ass, Boolean libx265, Boolean vfscale, Boolean hqdn3d, Boolean gradfun, Boolean xvid, Boolean mp3, Boolean nvenc)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT", false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT", false);
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (nvenc == true)
            {
                DGV_test.Rows.Add("Codec video Nvidia H.264", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Codec video Nvidia H.264", "Non trovato");
                risultato = DialogResult.No;
            }
            if (x264 == true)
            {
                DGV_test.Rows.Add("Codec video libx264", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Codec video libx264", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((libx265 == true))
            {
                DGV_test.Rows.Add("Codec video libx265", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Codec video libx265", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((xvid == true))
            {
                DGV_test.Rows.Add("Codec video libxvid", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Codec video libxvid", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((aac == true))
            {
                DGV_test.Rows.Add("Codec audio AAC", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Codec audio AAC", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((ac3 == true))
            {
                DGV_test.Rows.Add("Codec audio AC3", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Codec audio AC3", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((mp3 == true))
            {
                DGV_test.Rows.Add("Codec audio MP3", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Codec audio MP3", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((ass == true))
            {
                DGV_test.Rows.Add("Filtro video ASS", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Filtro video ASS", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((vfscale == true))
            {
                DGV_test.Rows.Add("Filtro video Scale", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Filtro video Scale", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((hqdn3d == true))
            {
                DGV_test.Rows.Add("Filtro video Hqdn3d", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Filtro video Hqdn3d", "Non trovato");
                risultato = DialogResult.No;
            }
            if ((gradfun == true))
            {
                DGV_test.Rows.Add("Filtro video Gradfun", "Trovato");
                risultato = DialogResult.Yes;
            }
            else
            {
                DGV_test.Rows.Add("Filtro video Gradfun", "Non trovato");
                risultato = DialogResult.No;
            }
        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = risultato;
            chiudi = true;
            this.Close();
        }

        private void TestFFMPEG_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (chiudi == true)
                e.Cancel = false;
            else
                e.Cancel = true;
        }
    }
}
