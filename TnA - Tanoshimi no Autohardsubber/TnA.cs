using FFmpeg_Output_Wrapper;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MediaInfoDotNet;


namespace TnA___Tanoshimi_no_Autohardsubber
{
    public partial class TnA : Form
    {
        readonly String GUIdir = Path.GetDirectoryName(Application.ExecutablePath);

        readonly String LOG_dir = Path.GetDirectoryName(Application.ExecutablePath) + "\\LOGS";

        readonly String file_settings = Path.GetDirectoryName(Application.ExecutablePath) + "\\settings\\settings.ini";

        readonly String ffmpeg_x86 = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg\\ffmpeg_tna_x86.exe";
        readonly String ffmpeg_x64 = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg\\ffmpeg_tna_x64.exe";

        readonly String mkvmerge = Path.GetDirectoryName(Application.ExecutablePath) + "\\mkvtoolnix\\mkvmerge.exe";

        readonly String temp_folder = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp";
        readonly String fonts_folder = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg\\fonts_v";

        String label_tab_lista = "Lista files e impostazioni";

        String azione_fine_coda = "Non fare niente";

        readonly String[] estensioni_video = { ".mkv", ".mp4", ".m2ts", ".ts", ".avi", ".mov", ".rmvb", ".ogm", ".flv", ".vob", ".mpg", ".mpeg", ".3gp", ".m4v" };

        //readonly String[] estensioni_audio = { ".m4a", ".flac", ".wav", ".aac", ".ogg", ".opus", ".tta", ".ac3", ".dts", ".mp3", ".mka" };

        String comando = String.Empty;

        String durata = String.Empty;

        String fc = String.Empty;

        Boolean pause = false;

        public static Boolean forza_chiusura = false;

        String file_attuale = "Nessuno";

        String file_finale = String.Empty;

        String versione_tna = String.Empty;

        public static DateTime data_last_upd = new DateTime();

        List<String> formati_scelti = new List<String>();
        
        public static List<String> stati_scelti = new List<String>();

        String data_vecchia = String.Empty;

        Int32 indice_percentuale = 0, exit_code = Int32.MinValue, sec_trasc = 0;

        

        Thread t;
        ThreadStart ts;

        System.Diagnostics.Process processo_codifica = new System.Diagnostics.Process();
        System.Diagnostics.Process processo_remux = new System.Diagnostics.Process();

        DownloadFFMPEG df;

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        private static void SuspendProcess(int pid)
        {
            var process = System.Diagnostics.Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (System.Diagnostics.ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        public static void ResumeProcess(int pid)
        {
            var process = System.Diagnostics.Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (System.Diagnostics.ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }



        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;

        String filtro = "File video supportati|";

        public TnA()
        {
            
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT", false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT", false);
            InitializeComponent();

            var screen = Screen.FromPoint(MousePosition);
            this.StartPosition = FormStartPosition.Manual;
            this.Left = screen.Bounds.Left + screen.Bounds.Width / 2 - this.Width / 2;
            this.Top = screen.Bounds.Top + screen.Bounds.Height / 2 - this.Height / 2;

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            cmb_compatibilita.Items.Clear();
            cmb_qualita.Items.Clear();
            cmb_risoluz.Items.Clear();
            cmb_subs.Items.Clear();

            for(Int32 i=0; i<compatibilita.Items.Count; i++)
            {
                cmb_compatibilita.Items.Add(compatibilita.Items[i].ToString());
            }
            for (Int32 i = 0; i < qualita.Items.Count; i++)
            {
                cmb_qualita.Items.Add(qualita.Items[i].ToString());
            }
            for (Int32 i = 0; i < risoluz.Items.Count; i++)
            {
                cmb_risoluz.Items.Add(risoluz.Items[i].ToString());
            }
            for (Int32 i = 0; i < subtitle_mode.Items.Count; i++)
            {
                cmb_subs.Items.Add(subtitle_mode.Items[i].ToString());
            }

            sc_log.Panel2Collapsed = true;

            if (System.IO.File.Exists((Application.StartupPath + "\\TnA.7z")))
                System.IO.File.Delete(Application.StartupPath + "\\TnA.7z");

            foreach (String s in estensioni_video)
            {
                filtro += "*" + s.ToLower() + ";" + "*" + s.ToUpper() + ";";
            }
            filtro = filtro.Trim(';');
            filtro += "|";
            foreach (String s in estensioni_video)
            {
                filtro += "File " + s.Trim('.').ToUpper() + "|*" + s.ToLower() + ";" + "*" + s.ToUpper() + "|";
            }
            filtro = filtro.Trim('|');
            ferma_tutto();
            toolStripComboBox1.Text = "Non fare niente";
            cmb_subs.Text = "Hardsub";

            versione_tna = this.Text.Split(' ')[5].Replace("v", String.Empty);

            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            infoToolStripMenuItem.Image = SystemIcons.Information.ToBitmap();

            if (Directory.Exists(LOG_dir) == false)
                Directory.CreateDirectory(LOG_dir);

            if (Directory.Exists(Path.GetDirectoryName(file_settings)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(file_settings));

            if (!Directory.Exists(temp_folder))
            {
                Directory.CreateDirectory(temp_folder);
            }
            else
            {
                foreach (String s in Directory.GetFiles(temp_folder))
                    System.IO.File.Delete(s);
            }

            if (Directory.Exists(fonts_folder))
            {
                Directory.Delete(fonts_folder, true);
            }
            if (Environment.Is64BitOperatingSystem == true)
            {
                if (System.IO.File.Exists(ffmpeg_x64) == false)
                {
                    this.Enabled = false;
                    bgw_downloadffmpeg.RunWorkerAsync();
                }
            }
            else
            {
                if (System.IO.File.Exists(ffmpeg_x86) == false)
                {
                    this.Enabled = false;
                    bgw_downloadffmpeg.RunWorkerAsync();
                }
            }

            LeggiImpostazioni(file_settings);

            b_avvia.Enabled = false;
            this.Activate();
            if (Environment.GetCommandLineArgs().Count() > 1)
            {
                List<String> d = new List<String>();
                for (Int32 i = 1; i < Environment.GetCommandLineArgs().Count(); i++)
                    d.Add(Environment.GetCommandLineArgs()[i]);
                recupera_fd(d.ToArray());
            }
            bgw_updateschecker.RunWorkerAsync();
        }

        public void LeggiImpostazioni_WindowsState(String Value)
        {
            switch (Value)
            {
                case "Maximized":
                    this.WindowState = FormWindowState.Maximized;
                    break;
                default:
                    this.WindowState = FormWindowState.Normal;
                    break;
                case "Minimized":
                    this.WindowState = FormWindowState.Minimized;
                    break;
            }
        }

        public void LeggiImpostazioni_LarghezzaFinestra(String Value)
        {
            try
            {
                if (Convert.ToInt32(Value) >= 943)
                    this.Width = Convert.ToInt32(Value);
            }
            catch { }
        }

        public void LeggiImpostazioni_AltezzaFinestra(String Value)
        {
            try
            {
                if (Convert.ToInt32(Value) >= 622)
                    this.Height = Convert.ToInt32(Value);
            }
            catch { }
        }

        public void LeggiImpostazioni_Compatibilita(String Value)
        {
            if (cmb_compatibilita.Items.Contains(Value))
                cmb_compatibilita.Text = Value;
            else
                cmb_compatibilita.Text = "Bluray AAC";
        }

        public void LeggiImpostazioni_Sottotitoli(String Value)
        {
            if (cmb_subs.Items.Contains(Value))
                cmb_subs.Text = Value;
            else
                cmb_subs.Text = "Hardsub";
        }

        public void LeggiImpostazioni_Risoluzione(String Value)
        {
            if (cmb_risoluz.Items.Contains(Value))
                cmb_risoluz.Text = Value;
            else
                cmb_risoluz.Text = "720p";
        }

        public void LeggiImpostazioni_Qualita(String Value)
        {
            if (cmb_qualita.Items.Contains(Value))
                cmb_qualita.Text = Value;
            else
                cmb_qualita.Text = "Media";
        }

        public Int32 LeggiImpostazioni_DataUpd(String Value)
        {
            Int32 Freq;
            switch (Value)
            {
                case "m":
                    manualeToolStripMenuItem.CheckState = CheckState.Checked;
                    iToolStripMenuItem.CheckState = CheckState.Unchecked;
                    unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
                    Freq = -1;
                    break;
                case "s":
                    manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                    iToolStripMenuItem.CheckState = CheckState.Checked;
                    unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
                    Freq = 0;
                    break;
                case "1":
                    manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                    iToolStripMenuItem.CheckState = CheckState.Unchecked;
                    unGiornoToolStripMenuItem.CheckState = CheckState.Checked;
                    ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
                    Freq = 1;
                    break;
                case "3":
                    manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                    iToolStripMenuItem.CheckState = CheckState.Unchecked;
                    unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Checked;
                    ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
                    Freq = 3;
                    break;
                case "7":
                    manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                    iToolStripMenuItem.CheckState = CheckState.Unchecked;
                    unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniSettimanaToolStripMenuItem.CheckState = CheckState.Checked;
                    Freq = 7;
                    break;
                default:
                    manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                    iToolStripMenuItem.CheckState = CheckState.Checked;
                    unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                    ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
                    Freq = 0;
                    break;
            }
            return Freq;
        }

        public void LeggiImpostazioni_Clipboard(String Value)
        {
            if (Value == "y")
                monitoraGliAppuntiToolStripMenuItem.CheckState = CheckState.Checked;
            else
                monitoraGliAppuntiToolStripMenuItem.CheckState = CheckState.Unchecked;
        }

        public void LeggiImpostazioni(String FileINI)
        {
            if (System.IO.File.Exists(file_settings))
            {
                IniFile ini = new IniFile(file_settings);

                if (ini.KeyExists("WindowState"))
                {
                    LeggiImpostazioni_WindowsState(ini.Read("WindowState"));
                }

                if (this.WindowState != FormWindowState.Minimized && this.WindowState != FormWindowState.Maximized)
                {
                    if (ini.KeyExists("Width"))
                        LeggiImpostazioni_LarghezzaFinestra(ini.Read("Width"));
                    if (ini.KeyExists("Height"))
                        LeggiImpostazioni_AltezzaFinestra(ini.Read("Height"));
                }

                if (ini.KeyExists("comp"))
                {
                    LeggiImpostazioni_Compatibilita(ini.Read("comp"));
                }
                else
                    cmb_compatibilita.Text = "Bluray AAC";

                if (ini.KeyExists("qual"))
                {
                    LeggiImpostazioni_Qualita(ini.Read("qual"));
                }
                else
                    cmb_qualita.Text = "Media";

                if (ini.KeyExists("risoluz"))
                {
                    LeggiImpostazioni_Risoluzione(ini.Read("risoluz"));
                }
                else
                    cmb_risoluz.Text = "720p";

                if (ini.KeyExists("subs"))
                {
                    LeggiImpostazioni_Sottotitoli(ini.Read("subs"));
                }
                else
                    cmb_subs.Text = "Hardsub";

                if (ini.KeyExists("last_upd"))
                {
                    data_vecchia = ini.Read("last_upd");
                    Int32 fr = -2;
                    if (ini.KeyExists("upd_freq"))
                    {
                        fr = LeggiImpostazioni_DataUpd(ini.Read("upd_freq"));
                    }

                    if (fr != -2)
                    {
                        if (fr == 0)
                        {
                            this.Enabled = false;
                            bgw_update.RunWorkerAsync();
                        }
                        if (fr >= 1)
                        {
                            DateTime data_odierna = DateTime.Now;
                            try
                            {
                                DateTime dt = DateTime.ParseExact(data_vecchia, "dd/MM/yyyy", new CultureInfo("it-IT"));
                                TimeSpan diff = data_odierna - dt;
                                if (diff.Days >= fr)
                                {
                                    this.Enabled = false;
                                    bgw_update.RunWorkerAsync();
                                }
                            }
                            catch
                            {
                                iToolStripMenuItem.CheckState = CheckState.Checked;
                            }
                        }
                    }
                    else
                    {
                        this.Enabled = false;
                        bgw_update.RunWorkerAsync();
                    }
                }
                if (ini.KeyExists("monit_clipb") == true)
                {
                    LeggiImpostazioni_Clipboard(ini.Read("monit_clipb"));
                }
            }
            else
            {
                cmb_compatibilita.Text = "Bluray AAC";
                cmb_risoluz.Text = "720p";
                cmb_qualita.Text = "Media";
            }
        }

        protected override void WndProc(ref Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    DisplayClipboardData();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        void DisplayClipboardData()
        {
            try
            {
                if (monitoraGliAppuntiToolStripMenuItem.CheckState == CheckState.Checked && monitoraGliAppuntiToolStripMenuItem.Enabled == true)
                {
                    System.Collections.Specialized.StringCollection coll = Clipboard.GetFileDropList();
                    String[] f = new string[coll.Count];
                    coll.CopyTo(f, 0);
                    recupera_fd(f);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void DGV_video_DragEnter(object sender, DragEventArgs e)
        {
            if (DGV_video.ReadOnly == false)
            {
                String[] dati = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String s in dati)
                {
                    if (Path.HasExtension(s) == true)
                    {
                        if (estensioni_video.Contains(Path.GetExtension(s).ToLower()) == true)
                        {
                            e.Effect = DragDropEffects.Copy;
                            break;
                        }
                    }
                    else
                    {
                        foreach (String t in Directory.GetFiles(s, "*", SearchOption.AllDirectories))
                        {
                            if (estensioni_video.Contains(Path.GetExtension(t).ToLower()) == true)
                            {
                                e.Effect = DragDropEffects.Copy;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void DGV_video_DragDrop(object sender, DragEventArgs e)
        {
            if (DGV_video.ReadOnly == false)
            {
                String[] dati = (String[])e.Data.GetData(DataFormats.FileDrop);
                recupera_fd(dati);
            }
        }

        public void recupera_fd(String[] dati)
        {
            List<String> temp = new List<String>();
            Boolean es_cart = false;
            try
            {
                foreach (String s in dati)
                {

                    if (Path.HasExtension(s) == true)
                    {
                        if (estensioni_video.Contains(Path.GetExtension(s).ToLower()) == true)
                        {
                            DGV_video.Rows.Add(Path.GetFileName(s), cmb_compatibilita.Text, cmb_risoluz.Text, cmb_qualita.Text, cmb_subs.Text, "PRONTO", Path.GetDirectoryName(s));
                        }
                    }
                    else
                    {
                        Seleziona_formati sf = new Seleziona_formati(estensioni_video);
                        sf.ShowInTaskbar = false;
                        sf.Icon = this.Icon;
                        sf.TopLevel = true;
                        sf.StartPosition = FormStartPosition.CenterParent;
                        if (es_cart == false)
                        {
                            if (sf.ShowDialog() == DialogResult.OK)
                            {
                                es_cart = true;
                            }
                        }
                        if (es_cart == true)
                        {
                            formati_scelti = TnA___Tanoshimi_no_Autohardsubber.Seleziona_formati.formati_scelti;
                            foreach (String t in Directory.GetFiles(s, "*", SearchOption.AllDirectories))
                            {
                                if (formati_scelti.Contains(Path.GetExtension(t).ToLower()) == true)
                                {
                                    DGV_video.Rows.Add(Path.GetFileName(t), cmb_compatibilita.Text, cmb_risoluz.Text, cmb_qualita.Text, cmb_subs.Text, "PRONTO", Path.GetDirectoryName(t));
                                }
                                else
                                {
                                    if (formati_scelti.Contains("TUTTI"))
                                    {
                                        if (estensioni_video.Contains(Path.GetExtension(t).ToLower()) == true)
                                        {
                                            DGV_video.Rows.Add(Path.GetFileName(t), cmb_compatibilita.Text, cmb_risoluz.Text, cmb_qualita.Text, cmb_subs.Text, "PRONTO", Path.GetDirectoryName(t));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (DGV_video.Rows.Count > 0)
                {
                    b_avvia.Enabled = true;
                    tb_help.Visible = false;
                }
                temp.Clear();
                tab_autohardsubber.Text = label_tab_lista + " (Totale files: " + DGV_video.Rows.Count.ToString() + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void b_avvia_Click(object sender, EventArgs e)
        {
            Boolean presente = false, start = true;
            
            foreach (String s in System.IO.Directory.GetFiles(Path.GetDirectoryName(ffmpeg_x64)))
            {
                if (Path.GetFileName(s).ToLower().Contains(Path.GetFileName(ffmpeg_x64)) || Path.GetFileName(s).ToLower().Contains(Path.GetFileName(ffmpeg_x86)))
                {
                    presente = true;
                }
            }
            if (b_avvia.Text.StartsWith("A") && presente == true)
            {
                monitoraGliAppuntiToolStripMenuItem.Enabled = false;
                DGV_video.ClearSelection();
                DGV_video.ContextMenuStrip.Enabled = false;
                indice_percentuale = 0;
                Boolean processati = false;
                foreach (DataGridViewRow r in DGV_video.Rows)
                {
                    if (r.Cells["stato"].Value.ToString().ToLower().Contains("pronto"))
                    {
                        processati = false;
                    }
                    else
                    {
                        processati = true;
                        break;
                    }
                }
                if (processati == true)
                {
                    SelezionaProcessati sp = new SelezionaProcessati();
                    if (sp.ShowDialog() == DialogResult.OK)
                    {
                        foreach (DataGridViewRow r in DGV_video.Rows)
                        {
                            if (stati_scelti.Contains(r.Cells["stato"].Value.ToString().Split(' ')[0]))
                            {
                                r.Cells["stato"].Value = "PRONTO - 0,00%";
                                r.Cells["stato"].Style.BackColor = Color.White;
                            }
                            start = true;
                        }
                    }
                    else
                    {
                        start = false;
                    }
                }
                if (start == true)
                {
                    ts = new ThreadStart(encode);
                    t = new Thread(ts);
                    t.Start();
                    timer_tempo.Start();
                    b_avvia.Text = "Ferma";
                    ripristinaImpostazioniToolStripMenuItem3.Enabled = false;
                    ripristinaImpostazioniToolStripMenuItem3.ShortcutKeys = Keys.None;
                    b_avvia.Image = (Image)TnA___Tanoshimi_no_Autohardsubber.Properties.Resources.stop;
                    b_avvia.FlatAppearance.MouseOverBackColor = Color.OrangeRed;
                    b_pause.Enabled = true;
                    b_agg_cart.Enabled = false;
                    b_agg_files.Enabled = false;
                    b_incolla.Enabled = false;
                    b_rimuovi.Enabled = false;
                    DGV_video.ReadOnly = true;
                    strumentiToolStripMenuItem.Enabled = false;
                    modificaToolStripMenuItem.Enabled = false;
                    pb_tot.Value = 0;
                    ts_perc.Text = pb_tot.Value.ToString() + "%";
                    rtb_codifica.Text = rtb_sottotitoli.Text = String.Empty;
                    l_vel.Text = "Velocità: 0";
                    l_dim_prev.Text = "Dimensione stimata: 0";
                    ts_perc.Text = "0,00%";
                    l_dim_att.Text = "Dimensione attuale: 0";
                    l_temp_rim.Text = "Tempo rimanente: 00:00:00";
                    l_temp_trasc.Text = "Posizione video: 00:00:00";
                    file_attuale = "Nessuno";
                }
            }
            else
            {
                if (presente == true)
                {
                    b_pause.Image = TnA___Tanoshimi_no_Autohardsubber.Properties.Resources.pause;
                    b_pause.Text = "Pausa";
                    b_avvia.Text = "Avvia";
                    b_avvia.Image = (Image)TnA___Tanoshimi_no_Autohardsubber.Properties.Resources.play;
                    b_avvia.FlatAppearance.MouseOverBackColor = Color.LawnGreen;
                    b_pause.Enabled = false;
                    b_agg_cart.Enabled = true;
                    b_agg_files.Enabled = true;
                    b_incolla.Enabled = true;
                    b_rimuovi.Enabled = true;
                    ripristinaImpostazioniToolStripMenuItem3.Enabled = true;
                    ripristinaImpostazioniToolStripMenuItem3.ShortcutKeys = (Keys)Shortcut.CtrlR;
                    DGV_video.ReadOnly = false;
                    timer_tempo.Stop();
                    monitoraGliAppuntiToolStripMenuItem.Enabled = true;
                    DGV_video.ContextMenuStrip.Enabled = true;
                    DGV_video.Columns[DGV_video.Columns["input"].Index].ReadOnly = true;
                    DGV_video.Columns[DGV_video.Columns["stato"].Index].ReadOnly = true;
                    DGV_video.Columns[DGV_video.Columns["percorso_orig"].Index].ReadOnly = true;
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "FERMATO - " + ts_perc.Text;
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.Yellow;
                    strumentiToolStripMenuItem.Enabled = true;
                    modificaToolStripMenuItem.Enabled = true;
                    ferma_tutto();
                    pause = false;
                    file_attuale = "Nessuno";
                }
                else
                {
                    var scelta = MessageBox.Show("Eseguibile di FFmpeg non trovato. Scaricarlo ora?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

                    if (scelta == DialogResult.Yes)
                    {
                        DF(true);
                        if (df.DialogResult == DialogResult.OK)
                        {
                            DialogResult inizio = MessageBox.Show("Iniziare ora la codifica?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (inizio == DialogResult.Yes)
                            {
                                b_avvia_Click(sender, e);
                            }
                        }
                        else
                            MessageBox.Show("Finché mancherà l'eseguibile di FFmpeg, il programma non potrà funzionare.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                        MessageBox.Show("Finché mancherà l'eseguibile di FFmpeg, il programma non potrà funzionare.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        public void encode()
        {
            for (Int32 q = 0; q < DGV_video.Rows.Count; q++)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    l_vel.Text = "Velocità: 0";
                    l_dim_prev.Text = "Dimensione stimata: 0";
                    ts_perc.Text = "0,00%";
                    l_dim_att.Text = "Dimensione attuale: 0";
                    l_temp_rim.Text = "Tempo rimanente: 00:00:00";
                    l_temp_trasc.Text = "Posizione video: 00:00:00";
                });
                if (DGV_video.Rows[q].Cells["stato"].Value.ToString().ToLower().StartsWith("pronto"))
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        rtb_codifica.Text = rtb_sottotitoli.Text = String.Empty;
                    });
                    DataGridViewRow d = DGV_video.Rows[q];
                    sec_trasc = 0;
                    comando = String.Empty;
                    indice_percentuale = d.Index;
                    file_attuale = "Rimozione cartelle temporanee";
                    if (!Directory.Exists(temp_folder))
                    {
                        Directory.CreateDirectory(temp_folder);
                    }
                    else
                    {
                        foreach (String s in Directory.GetFiles(temp_folder))
                            System.IO.File.Delete(s);
                    }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        ts_avanz.Text = "Avanzamento elaborazione - Nessun file in elaborazione";
                    });

                    String file_video = Path.Combine(d.Cells[DGV_video.Columns["percorso_orig"].Index].Value.ToString(), d.Cells[DGV_video.Columns["input"].Index].Value.ToString());
                    String profilo = d.Cells[DGV_video.Columns["compatibilita"].Index].Value.ToString();
                    String qualita = d.Cells[DGV_video.Columns["qualita"].Index].Value.ToString();
                    String risoluzione_f = d.Cells[DGV_video.Columns["risoluz"].Index].Value.ToString();
                    String modalita_subs = d.Cells[DGV_video.Columns["subtitle_mode"].Index].Value.ToString();

                    List<String> OverbordingLines = new List<String>();

                    List<String> file_sub = new List<String>();

                    Boolean ass = false, stop = false;

                    if (Path.GetExtension(file_video).ToLower() == ".mkv" && profilo.StartsWith("Remux") == false && profilo.StartsWith("Workraw") == false && modalita_subs.StartsWith("Hard"))
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            ts_avanz.Text = "Avanzamento elaborazione - Cerco eventuali fonts e sottotitoli";
                        });

                        foreach (String s in Directory.GetFiles(temp_folder))
                        {
                            System.IO.File.Delete(s);
                        }

                        String temp_ffmpeg = String.Empty;

                        if (System.IO.File.Exists(ffmpeg_x64))
                        {
                            temp_ffmpeg = temp_folder + "\\" + Path.GetFileName(ffmpeg_x64);
                            System.IO.File.Copy(ffmpeg_x64, temp_ffmpeg, true);
                        }
                        else
                        {
                            temp_ffmpeg = temp_folder + "\\" + Path.GetFileName(ffmpeg_x86);
                            System.IO.File.Copy(ffmpeg_x86, temp_ffmpeg, true);
                        }

                        Tuple<String, String> SubID = new Tuple<string, string>(String.Empty, String.Empty);

                        System.Diagnostics.ProcessStartInfo psi_extract = new System.Diagnostics.ProcessStartInfo();

                        Environment.CurrentDirectory = temp_folder;

                        foreach (MediaInfoDotNet.Models.TextStream t in new MediaFile(file_video).Text)
                        {
                            if (t.CodecId.ToLower().Contains("s_text/ass"))
                            {
                                if (t.Default == true)
                                {
                                    SubID = new Tuple<string, string>((t.ID - 1).ToString(), ".ass");
                                }
                                if (t.Forced)
                                {
                                    SubID = new Tuple<string, string>((t.ID - 1).ToString(), ".ass");
                                    break;
                                }
                            }
                            if (t.CodecId.ToLower().Contains("s_text/utf"))
                            {
                                if (t.Default == true)
                                {
                                    SubID = new Tuple<string, string>((t.ID - 1).ToString(), ".srt");
                                }
                                if (t.Forced == true)
                                {
                                    SubID = new Tuple<string, string>((t.ID - 1).ToString(), ".srt");
                                    break;
                                }
                            }
                        }

                        if (SubID.Item1 == String.Empty)
                        {
                            if (new MediaFile(file_video).Text.Count > 0)
                            {
                                if (new MediaFile(file_video).Text[0].CodecId.ToLower().Contains("s_text/ass"))
                                    SubID = new Tuple<string, string>((new MediaFile(file_video).Text[0].ID - 1).ToString(), ".ass");
                                if (new MediaFile(file_video).Text[0].CodecId.ToLower().Contains("s_text/utf"))
                                    SubID = new Tuple<string, string>((new MediaFile(file_video).Text[0].ID - 1).ToString(), ".srt");
                            }
                        }

                        psi_extract = new System.Diagnostics.ProcessStartInfo();
                        psi_extract.CreateNoWindow = true;
                        psi_extract.UseShellExecute = false;
                        psi_extract.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                        psi_extract.FileName = Path.GetFileName(temp_ffmpeg);

                        if (String.IsNullOrWhiteSpace(SubID.Item1) == false)
                        {
                            psi_extract.Arguments = " -y -i \"" + file_video + "\" ";
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                ts_avanz.Text = "Avanzamento elaborazione - Estraggo i sottotitoli";
                            });
                            psi_extract.Arguments += " -map 0:" + SubID.Item1 + " -c:s copy \"" + Path.GetDirectoryName(ffmpeg_x64) + "\\subs0" + SubID.Item2 + "\"";
                            file_sub.Add(Path.GetDirectoryName(ffmpeg_x64) + "\\subs0" + SubID.Item2);

                            System.Diagnostics.Process.Start(psi_extract).WaitForExit();
                        }

                        this.Invoke((MethodInvoker)delegate ()
                        {
                            ts_avanz.Text = "Avanzamento elaborazione - Estraggo gli eventuali fonts";
                        });

                        psi_extract = new System.Diagnostics.ProcessStartInfo();
                        psi_extract.CreateNoWindow = true;
                        psi_extract.UseShellExecute = false;
                        psi_extract.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                        temp_ffmpeg = String.Empty;

                        if (System.IO.File.Exists(ffmpeg_x64))
                        {
                            temp_ffmpeg = temp_folder + "\\" + Path.GetFileName(ffmpeg_x64);
                            System.IO.File.Copy(ffmpeg_x64, temp_ffmpeg, true);
                        }
                        else
                        {
                            temp_ffmpeg = temp_folder + "\\" + Path.GetFileName(ffmpeg_x86);
                            System.IO.File.Copy(ffmpeg_x86, temp_ffmpeg, true);
                        }

                        psi_extract.FileName = Path.GetFileName(temp_ffmpeg);

                        psi_extract.Arguments = " -dump_attachment:t \"\" -i \"" + file_video + "\" NUL";

                        System.Diagnostics.Process.Start(psi_extract).WaitForExit();

                        Environment.CurrentDirectory = GUIdir;
                        foreach (String s in Directory.GetFiles(temp_folder))
                        {
                            if (Path.GetExtension(s).ToLower() == ".exe")
                                System.IO.File.Delete(s);
                        }
                        if (Directory.Exists(fonts_folder))
                        {
                            Directory.Delete(fonts_folder, true);
                        }
                        Directory.Move(temp_folder, fonts_folder);
                    }

                    comando = String.Empty;

                    file_attuale = Path.GetFileName(file_video);

                    //Thread.Sleep(2000);

                    if (stop == false)
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {

                        });
                        switch (profilo)
                        {
                            case "Remux MKV":
                                remux_mkv(file_video, profilo);
                                break;
                            case "Remux MP4":
                                remux_mp4(file_video, profilo, qualita);
                                break;
                            default:
                                switch (d.Cells["risoluz"].Value.ToString())
                                {
                                    case "1080p":
                                        Codifica(file_video, qualita, file_sub, fonts_folder, ass, profilo, 1920, 1080, modalita_subs);
                                        break;
                                    case "900p":
                                        Codifica(file_video, qualita, file_sub, fonts_folder, ass, profilo, 1600, 900, modalita_subs);
                                        break;
                                    case "720p":
                                        Codifica(file_video, qualita, file_sub, fonts_folder, ass, profilo, 1280, 720, modalita_subs);
                                        break;
                                    case "576p":
                                        Codifica(file_video, qualita, file_sub, fonts_folder, ass, profilo, 1024, 576, modalita_subs);
                                        break;
                                    case "480p":
                                        Codifica(file_video, qualita, file_sub, fonts_folder, ass, profilo, 864, 486, modalita_subs);
                                        break;
                                    case "396p":
                                        Codifica(file_video, qualita, file_sub, fonts_folder, ass, profilo, 704, 396, modalita_subs);
                                        break;
                                }
                                break;
                        }

                        Thread.Sleep(100);
                    }
                }
            }
            Environment.CurrentDirectory = GUIdir;
            Thread.Sleep(500);
            if (azione_fine_coda.StartsWith("Non") == false)
            {
                switch (azione_fine_coda)
                {
                    case "Chiudi l'applicazione":
                        forza_chiusura = true;
                        this.Close();
                        break;
                    case "Stand-by":
                        Application.SetSuspendState(PowerState.Suspend, true, true);
                        break;
                    default:
                        System.Diagnostics.Process.Start("shutdown", "/s /t 120");
                        if (MessageBox.Show("Il PC si spegnerà entro due minuti a partire dalle " + DateTime.Now.TimeOfDay.ToString() + ".\nPer annullare, premere OK.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start("shutdown", "/a");
                            MessageBox.Show("L'arresto del PC è stato cancellato.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                }
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                b_avvia.Text = "Avvia";
                b_avvia.Image = (Image)TnA___Tanoshimi_no_Autohardsubber.Properties.Resources.play;
                b_avvia.FlatAppearance.MouseOverBackColor = Color.LawnGreen;
                b_pause.Enabled = false;
                b_agg_cart.Enabled = true;
                b_agg_files.Enabled = true;
                b_incolla.Enabled = true;
                b_rimuovi.Enabled = true;
                ripristinaImpostazioniToolStripMenuItem3.Enabled = true;
                ripristinaImpostazioniToolStripMenuItem3.ShortcutKeys = (Keys)Shortcut.CtrlR;
                DGV_video.ReadOnly = false;
                timer_tempo.Stop();
                monitoraGliAppuntiToolStripMenuItem.Enabled = true;
                DGV_video.ContextMenuStrip.Enabled = true;
                DGV_video.Columns[DGV_video.Columns["input"].Index].ReadOnly = true;
                DGV_video.Columns[DGV_video.Columns["stato"].Index].ReadOnly = true;
                DGV_video.Columns[DGV_video.Columns["percorso_orig"].Index].ReadOnly = true;
                strumentiToolStripMenuItem.Enabled = true;
                modificaToolStripMenuItem.Enabled = true;
                file_attuale = "Nessuno";
            });
        }

        public void remux_mp4(String v, String prof, String qual)
        {
            String comando_remux = " -y -i \"" + v + "\"";
            String mp4_finale = Path.GetDirectoryName(v) + "\\" + Path.GetFileNameWithoutExtension(v) + "[" + prof + "].mp4";
            file_finale = mp4_finale;

            MediaFile m = new MediaFile(v);

            Int32 cont_video = 0, cont_audio = 0;
            foreach (MediaInfoDotNet.Models.VideoStream vid in m.Video)
            {
                if (vid.Format.ToLower().Contains("mpeg") || vid.Format.ToLower().Contains("avc") || vid.Format.ToLower().Contains("hevc"))
                {
                    comando_remux += " -map 0:" + (vid.ID - 1).ToString() + " -c:v:" + cont_video.ToString() + " copy";
                    comando_remux += " -r " + vid.FrameRate.ToString().Replace(",", ".");
                    fc = Math.Round((Double)vid.FrameCount/* / 1000.0*/, 0, MidpointRounding.AwayFromZero).ToString();
                    Double altezza = vid.Height;
                    Double larghezza = vid.Width;
                    String aspect = Math.Round(larghezza / altezza, 3, MidpointRounding.AwayFromZero).ToString();
                    durata = TimeSpan.FromMilliseconds(vid.Duration).ToString(@"hh\:mm\:ss");
                    //if (durata.Contains("."))
                    //    durata = durata.Remove(durata.IndexOf(".")).Trim();
                }
                else
                {
                    comando_remux += " -map 0:" + (vid.ID - 1).ToString() + " -c:v:" + cont_video.ToString() + " libx264";
                    comando_remux += " -r " + vid.FrameRate.ToString().Replace(",", ".");
                    fc = Math.Round((Double)vid.FrameCount/* / 1000.0*/, 0, MidpointRounding.AwayFromZero).ToString();
                    Double altezza = vid.Height;
                    if (altezza % 2 != 0)
                        altezza--;
                    Double larghezza = vid.Width;
                    if (larghezza % 2 != 0)
                        larghezza--;
                    String aspect = Math.Round(larghezza / altezza, 3, MidpointRounding.AwayFromZero).ToString();
                    durata = TimeSpan.FromMilliseconds(vid.Duration).ToString("HH:mm:ss");
                    //if (durata.Contains("."))
                    //    durata = durata.Remove(durata.IndexOf(".")).Trim();
                    switch (qual)
                    {
                        case "Altissima":
                            comando += " -crf 14 -preset:v veryslow -aq-mode 3";
                            break;
                        case "Alta":
                            comando += " -crf 17 -preset:v veryslow -aq-mode 3";
                            break;
                        case "Medio-alta":
                            comando += " -crf 18.5 -preset:v slower -aq-mode 3";
                            break;
                        case "Media":
                            comando += " -crf 20 -preset:v medium -aq-mode 2";
                            break;
                        case "Medio-bassa":
                            comando += " -crf 22 -preset:v fast -aq-mode 1";
                            break;
                        case "Bassa":
                            comando += " -crf 25 -preset:v fast -aq-mode 1";
                            break;
                        case "Bassissima":
                            comando += " -crf 27 -preset:v fast -aq-mode 1";
                            break;
                        case "Bozza":
                            comando += " -crf 36 -preset:v superfast -aq-mode 1";
                            break;
                    }

                    comando_remux += " -profile:v:" + cont_video.ToString() + " high -level:v:" + cont_video.ToString() + " 4.1";
                    if (vid.miGetString("ScanType").ToLower().Trim().StartsWith("i") == true)
                    {
                        comando_remux += " -vf yadif";
                    }
                }
                cont_video++;
            }
            foreach (MediaInfoDotNet.Models.AudioStream aud in m.Audio)
            {
                CatturaAudio ca = new CatturaAudio(aud);
                String canali_audio = String.Empty;
                Boolean IsLossless = ca.Lossless;

                canali_audio = ca.Canali.ToString();

                if (ca.Formato.ToLower().Contains("ac-3") || ca.Formato.ToLower().StartsWith("mpeg") || ca.Formato.ToLower().Contains("aac"))
                {
                    comando_remux += " -map 0:" + ca.Indice.ToString() + " -c:a:" + cont_audio.ToString() + " copy";
                }
                else
                {
                    if (IsLossless == true)
                    {
                        comando_remux += " -map 0:" + ca.Indice.ToString() + " -c:a:" + cont_audio.ToString() + " alac -ac:" + cont_audio.ToString() + " " + canali_audio;
                    }
                    else
                    {
                        comando_remux += " -map 0:" + ca.Indice.ToString() + " -c:a:" + cont_audio.ToString() + " aac -ac:" + cont_audio.ToString() + " " + canali_audio;
                        switch (qual)
                        {
                            case "Altissima":
                                comando_remux += " -b:a:" + cont_audio.ToString() + " " + (192 * (Convert.ToInt32(canali_audio) / 2)).ToString() + "k";
                                break;
                            case "Alta":
                                comando_remux += " -b:a:" + cont_audio.ToString() + " " + (160 * (Convert.ToInt32(canali_audio) / 2)).ToString() + "k";
                                break;
                            case "Medio-alta":
                                comando_remux += " -b:a:" + cont_audio.ToString() + " " + (144 * (Convert.ToInt32(canali_audio) / 2)).ToString() + "k";
                                break;
                            case "Media":
                                comando_remux += " -b:a:" + cont_audio.ToString() + " " + (112 * (Convert.ToInt32(canali_audio) / 2)).ToString() + "k";
                                break;
                            case "Medio-bassa":
                                comando_remux += " -b:a:" + cont_audio.ToString() + " " + (96 * (Convert.ToInt32(canali_audio) / 2)).ToString() + "k";
                                break;
                            case "Bassa":
                                comando_remux += " -b:a:" + cont_audio.ToString() + " " + (64 * (Convert.ToInt32(canali_audio) / 2)).ToString() + "k";
                                break;
                            case "Bassissima":
                                comando_remux += " -b:a:" + cont_audio.ToString() + " " + (48 * (Convert.ToInt32(canali_audio) / 2)).ToString() + "k";
                                break;
                        }
                    }
                }
                cont_audio++;
            }
            
            comando_remux += " \"" + mp4_finale + "\"";

            //MessageBox.Show(comando_remux);

            Environment.CurrentDirectory = Path.GetDirectoryName(ffmpeg_x64);
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            processo_codifica = new System.Diagnostics.Process();
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            if (Environment.Is64BitOperatingSystem == true)
            {
                psi.FileName = Path.GetFileName(ffmpeg_x64);
            }
            else
            {
                psi.FileName = Path.GetFileName(ffmpeg_x86);
            }
            
            psi.Arguments = comando_remux;

            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            processo_codifica.OutputDataReceived += Processo_codifica_OutputDataReceived;
            processo_codifica.ErrorDataReceived += Processo_codifica_ErrorDataReceived;
            processo_codifica.StartInfo = psi;
            processo_codifica.Start();
            processo_codifica.BeginOutputReadLine();
            processo_codifica.BeginErrorReadLine();
            processo_codifica.WaitForExit();
            processo_codifica.CancelOutputRead();
            processo_codifica.CancelErrorRead();
            exit_code = processo_codifica.ExitCode;
            if (exit_code == 0)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    ts_perc.Text = "100,00%";
                    pb_tot.Value = pb_tot.Maximum;
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "OK - " + ts_perc.Text;
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.LightGreen;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "ERRORE - " + ts_perc.Text;
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.Red;
                });
            }

            CreaLOG();

        }

        public void remux_mkv(String v, String prof)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(mkvmerge);
            String mkv_finale = Path.GetDirectoryName(v) + "\\" + Path.GetFileNameWithoutExtension(v) + "[" + prof + "].mkv";
            file_finale = mkv_finale;
            String comando_remux = " -o \"" + mkv_finale + "\" \"" + v + "\"";
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.Arguments = comando_remux;
            psi.FileName = Path.GetFileName(mkvmerge);
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            processo_remux = new System.Diagnostics.Process();
            processo_remux.StartInfo = psi;
            processo_remux.ErrorDataReceived += Pr_ErrorDataReceived;
            processo_remux.OutputDataReceived += Pr_OutputDataReceived;
            this.Invoke((MethodInvoker)delegate ()
            {
                ts_avanz.Text = "Avanzamento elaborazione - Remux in corso del file '" + Path.GetFileName(v) + "'";
            });
            processo_remux.Start();
            processo_remux.BeginErrorReadLine();
            processo_remux.BeginOutputReadLine();
            processo_remux.WaitForExit();
            processo_remux.CancelErrorRead();
            processo_remux.CancelOutputRead();
            switch (processo_remux.ExitCode)
            {
                case 0:
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        ts_perc.Text = "100,00%";
                        pb_tot.Value = pb_tot.Maximum;
                        DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "OK - " + ts_perc.Text;
                        DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.LightGreen;
                    });
                    break;
                case 1:
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "ATTENZIONE - " + ts_perc.Text;
                        DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.LightGoldenrodYellow;
                    });
                    break;
                default:
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "ERRORE - " + ts_perc.Text;
                        DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.Red;
                    });
                    break;

            }

            CreaLOG();

            this.Invoke((MethodInvoker)delegate ()
            {
                ts_avanz.Text = "Avanzamento elaborazione - Nessuna elaborazione";
            });
        }

        public void CreaLOG()
        {
            if (Directory.Exists(LOG_dir) == false)
            {
                Directory.CreateDirectory(LOG_dir);
            }
            try
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    List<String> LOG_info = rtb_codifica.Lines.ToList();
                    LOG_info.RemoveAll(String.IsNullOrWhiteSpace);
                    LOG_info.Add("\n");

                    List<String> LOG_subs = rtb_sottotitoli.Lines.ToList();
                    LOG_subs.RemoveAll(String.IsNullOrWhiteSpace);

                    LOG_info.AddRange(LOG_subs);

                    System.IO.File.WriteAllLines(LOG_dir + "\\" + Path.GetFileNameWithoutExtension(file_finale) + " - LOG.txt", LOG_info.ToArray());
                });
            }
            catch { }
        }

        private void Pr_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            String s = e.Data;
            String numero = String.Empty;
            if (String.IsNullOrWhiteSpace(s) == false)
            {
                if (s.StartsWith("Progress"))
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        numero = s.Replace("Progress:", String.Empty);
                        numero = numero.Replace("%", String.Empty);
                        numero = numero.Trim();
                        pb_tot.Value = Convert.ToInt32(numero);
                        
                        ts_perc.Text = pb_tot.Value.ToString() + ".00%";
                        DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "IN CORSO - " + ts_perc.Text;
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        rtb_codifica.Text += s + "\n";
                        rtb_codifica.SelectionStart = rtb_codifica.Text.Length;
                        rtb_codifica.ScrollToCaret();
                    });
                }
            }
        }

        private void Pr_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            String s = e.Data;
            String numero = String.Empty;
            if (String.IsNullOrWhiteSpace(s) == false)
            {
                if (s.StartsWith("Progress"))
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        numero = s.Replace("Progress:", String.Empty);
                        numero = numero.Replace("%", String.Empty);
                        numero = numero.Trim();
                        pb_tot.Value = Convert.ToInt32(numero);
                        ts_perc.Text = pb_tot.Value.ToString() + ".00%";
                        DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "IN CORSO - " + ts_perc.Text;
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        rtb_codifica.Text += s + "\n";
                    });
                }
            }
        }

        public String ResizeAuto(String AspectR, Int32 LargDest, Double LargOrig, Int32 AltDest, Double AltOrig)
        {
            String temp = String.Empty;

            if (Convert.ToDouble(LargOrig / AltOrig) >= (4.0 / 3.0))
            {
                Int32 LFin = CalcolaLarghezzaFinale(Convert.ToInt32(LargOrig), LargDest, Convert.ToInt32(AltOrig));
                if (LFin > 0)
                {
                    temp = ",scale=\"" + LFin + ":trunc(ow/a/2)*2\"";
                }
                else
                {
                    temp = String.Empty;
                }
            }
            else
            {
                if (Convert.ToInt32(AltOrig) > AltDest)
                {
                    if (AltOrig % 2 == 0)
                        AltOrig = AltDest;
                    temp = ",scale=\"trunc(oh*a/2)*2:" + AltOrig + "\"";
                }
                else
                {
                    temp = String.Empty;
                }
            }
            return temp;
        }

        public Int32 CalcolaLarghezzaFinale(Int32 LargO, Int32 LargD, Int32 AltO)
        {
            Int32 temp = 0;
            if (LargO < LargD)
            {
                if (LargO % 2 == 0)
                {
                    if (AltO % 2 == 0)
                        temp = 0;
                    else
                        temp = LargO;
                }
                else
                {
                    temp = LargO - 1;
                }
            }
            else
            {
                if (LargO > LargD)
                {
                    temp = LargD;
                }
                else
                {
                    temp = 0;
                }
            }
            return temp;
        }

        public Int32 CalcolaAltezzaFinale(Int32 AltO, Int32 AltD, Int32 LargO)
        {
            Int32 temp = 0;
            if (AltO < AltD)
            {
                if (AltO % 2 == 0)
                {
                    if (LargO % 2 == 0)
                        temp = 0;
                    else
                        temp = AltO;
                }
                else
                {
                    temp = AltO - 1;
                }
            }
            else
            {
                if (AltO > AltD)
                {
                    temp = AltD;
                }
                else
                {
                    temp = 0;
                }
            }
            return temp;
        }

        private void Processo_codifica_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
                {
                    try
                    {
                        FFmpegOutputWrapperNET ff = new FFmpegOutputWrapperNET(e.Data.Trim());
                        String temp_extra = e.Data;
                        if (e.Data.Trim().ToLower().StartsWith("frame"))
                        {
                            try
                            {
                                Int32 frames = Convert.ToInt32(ff.Frames);
                                pb_tot.Maximum = Convert.ToInt32(TimeSpan.Parse(durata).TotalSeconds);
                                String fps = ff.Fps;
                                l_vel.Text = "Velocità: " + fps + " fps";
                                String size = ff.Size.Replace("kB", "");
                                l_dim_att.Text = "Dimensione attuale: " + Arrotonda(Convert.ToDouble(size) / 1024.0, 2) + " MB";
                                l_temp_trasc.Text = "Posizione video: " + ff.Time.ToString();
                                ts_avanz.Text = "Avanzamento elaborazione - Codifica del file \"" + file_attuale + "\"";
                                ts_perc.Text = Arrotonda(((Double)pb_tot.Value / pb_tot.Maximum) * 100.0, 2) + "%";
                                pb_tot.Value = Convert.ToInt32(ff.Time.TotalSeconds);


                                DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "IN CORSO - " + ts_perc.Text;
                                DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.White;

                                String bitrate = ff.Bitrate.Remove(ff.Bitrate.IndexOf("k"));

                                if (fps.Contains("."))
                                    fps = fps.Remove(fps.IndexOf("."));
                                Int32 tempo_restante = ((Convert.ToInt32(fc) - Convert.ToInt32(ff.Frames)) / Convert.ToInt32(ff.Fps));
                                l_temp_rim.Text = "Tempo rimanente: " + TimeSpan.FromSeconds(tempo_restante).ToString(@"hh\:mm\:ss");
                                Double bitrate_kb = Convert.ToDouble(bitrate.Remove(bitrate.IndexOf("."))) / 8 / 1024.0;
                                Double dimens_stimata = Math.Round(bitrate_kb * TimeSpan.Parse(durata).TotalSeconds, 2);
                                l_dim_prev.Text = "Dimensione stimata: " + dimens_stimata + " MB";
                            }
                            catch// (Exception ex)
                            {
                                //MessageBox.Show(ex.Message);
                                //DGV_log.Rows.Insert(0, DateTime.Now.ToString(), ex.Message + " -> " + ecc);
                            }
                        }
                        else
                        {
                            if (temp_extra.ToLower().Trim().Contains("parsed_ass"))
                            {
                                rtb_sottotitoli.Text += l_temp_trasc.Text.Remove(0, l_temp_trasc.Text.LastIndexOf(' ') + 1) + " ---> " + temp_extra + "\n";
                                rtb_sottotitoli.SelectionStart = rtb_sottotitoli.Text.Length;
                                rtb_sottotitoli.ScrollToCaret();
                            }
                            else
                            {
                                if (temp_extra.ToLower().Trim().StartsWith("frame") == false)
                                {
                                    rtb_codifica.Text += temp_extra + "\n";
                                    rtb_codifica.SelectionStart = rtb_codifica.Text.Length;
                                    rtb_codifica.ScrollToCaret();
                                }
                            }
                        }
                    }
                    catch { }
                });
        }

        private void Processo_codifica_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(e.Data) == false)
                    {
                        rtb_codifica.Text = String.Empty;
                        rtb_codifica.Text += e.Data;
                    }
                }
                catch { }
            });
        }

        public void AvviaCodifica()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(ffmpeg_x64);
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            processo_codifica = new System.Diagnostics.Process();
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            if (Environment.Is64BitOperatingSystem == true)
            {
                psi.FileName = Path.GetFileName(ffmpeg_x64);
            }
            else
            {
                psi.FileName = Path.GetFileName(ffmpeg_x86);
            }
            //psi.FileName = "cmd.exe";
            psi.Arguments = comando;// + " 2>" + Path.GetFileName(ffmpeg_txt);

            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            processo_codifica.OutputDataReceived += Processo_codifica_OutputDataReceived;
            processo_codifica.ErrorDataReceived += Processo_codifica_ErrorDataReceived;
            processo_codifica.StartInfo = psi;
            processo_codifica.Start();
            processo_codifica.BeginOutputReadLine();
            processo_codifica.BeginErrorReadLine();
            processo_codifica.WaitForExit();
            processo_codifica.CancelOutputRead();
            processo_codifica.CancelErrorRead();
            exit_code = processo_codifica.ExitCode;
            if (exit_code == 0)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    ts_perc.Text = "100,00%";
                    pb_tot.Value = pb_tot.Maximum;
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "OK - " + ts_perc.Text;
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.LightGreen;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Value = "ERRORE - " + ts_perc.Text;
                    DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["stato"].Index].Style.BackColor = Color.Red;
                });
            }

            CreaLOG();
        }

        public void Codifica(String video, String qualita, List<String> sub, String ff, Boolean sub_ass, String prof, Int32 largh, Int32 alt, String submode)
        {
            MediaFile media = new MediaFile(video);

            List<CatturaAudio> ca = new List<CatturaAudio>();
            CatturaVideo cv = new CatturaVideo(media);

            ImpostazioniProfiliCodifica.QualitaVideo.Altissima q_altissima = new ImpostazioniProfiliCodifica.QualitaVideo.Altissima(prof);
            ImpostazioniProfiliCodifica.QualitaVideo.Alta q_alta = new ImpostazioniProfiliCodifica.QualitaVideo.Alta(prof);
            ImpostazioniProfiliCodifica.QualitaVideo.MedioAlta q_medioalta = new ImpostazioniProfiliCodifica.QualitaVideo.MedioAlta(prof);
            ImpostazioniProfiliCodifica.QualitaVideo.Media q_media = new ImpostazioniProfiliCodifica.QualitaVideo.Media(prof);
            ImpostazioniProfiliCodifica.QualitaVideo.MedioBassa q_mediobassa = new ImpostazioniProfiliCodifica.QualitaVideo.MedioBassa(prof);
            ImpostazioniProfiliCodifica.QualitaVideo.Bassa q_bassa = new ImpostazioniProfiliCodifica.QualitaVideo.Bassa(prof);
            ImpostazioniProfiliCodifica.QualitaVideo.Bassissima q_bassissima = new ImpostazioniProfiliCodifica.QualitaVideo.Bassissima(prof);
            ImpostazioniProfiliCodifica.QualitaVideo.Bozza q_bozza = new ImpostazioniProfiliCodifica.QualitaVideo.Bozza(prof);

            ImpostazioniProfiliCodifica.ParametriVideo parametri_video = new ImpostazioniProfiliCodifica.ParametriVideo(media);

            fc = Math.Round((Double)cv.TotaleFrames, 0, MidpointRounding.AwayFromZero).ToString();


            Boolean interl = cv.Interlacciato;

            this.Invoke((MethodInvoker)delegate ()
            {
                if (submode.StartsWith("H"))
                {
                    sc_log.Panel2Collapsed = false;
                }
                else
                    sc_log.Panel2Collapsed = true;
            });

            foreach (MediaInfoDotNet.Models.AudioStream a in media.Audio)
            {
                ca.Add(new CatturaAudio(a));
            }

            comando = " -y -i \"" + video + "\" -map 0:" + cv.Indice;

            comando += " -c:v:" + cv.Indice + " " + q_altissima.CODEC;
            if (prof.ToLower().Contains("xbox") && Convert.ToDouble(cv.Framerate) > 30)
                cv.Framerate = "30";

            cv.Framerate = cv.Framerate.ToString().Replace(",", ".");

            if (Convert.ToDouble(cv.Framerate) > 0)
                comando += " -r " + cv.Framerate;

            durata = cv.DurataPrecisa.ToString();
            
            switch (qualita)
            {
                case "Altissima":
                    switch (prof.Split(' ')[0])
                    {
                        case "XviD":
                            comando += " -q:v:" + cv.Indice.ToString() + " " + q_altissima.QP.ToString() + " " + parametri_video.XVID;
                            break;
                        default:
                            comando += " -crf: " + q_altissima.CRF.ToString().Replace(',', '.') + " -preset:v " + q_altissima.Preset + " -aq-mode " + q_altissima.AQmode.ToString();
                            break;
                    }
                    break;
                case "Alta":
                    switch (prof.Split(' ')[0])
                    {
                        case "XviD":
                            comando += " -q:v:" + cv.Indice.ToString() + " " + q_alta.QP.ToString() + " " + parametri_video.XVID;
                            break;
                        default:
                            comando += " -crf: " + q_alta.CRF.ToString().Replace(',', '.') + " -preset:v " + q_alta.Preset + " -aq-mode " + q_alta.AQmode.ToString();
                            break;
                    }
                    break;
                case "Medio-alta":
                    switch (prof.Split(' ')[0])
                    {
                        case "XviD":
                            comando += " -q:v:" + cv.Indice.ToString() + " " + q_medioalta.QP.ToString() + " " + parametri_video.XVID;
                            break;
                        default:
                            comando += " -crf: " + q_medioalta.CRF.ToString().Replace(',', '.') + " -preset:v " + q_medioalta.Preset + " -aq-mode " + q_medioalta.AQmode.ToString();
                            break;
                    }
                    break;
                case "Media":
                    switch (prof.Split(' ')[0])
                    {
                        case "XviD":
                            comando += " -q:v:" + cv.Indice.ToString() + " " + q_media.QP.ToString() + " " + parametri_video.XVID;
                            break;
                        default:
                            comando += " -crf: " + q_media.CRF.ToString().Replace(',', '.') + " -preset:v " + q_media.Preset + " -aq-mode " + q_media.AQmode.ToString();
                            break;
                    }
                    break;
                case "Medio-bassa":
                    switch (prof.Split(' ')[0])
                    {
                        case "XviD":
                            comando += " -q:v:" + cv.Indice.ToString() + " " + q_mediobassa.QP.ToString() + " " + parametri_video.XVID;
                            break;
                        default:
                            comando += " -crf: " + q_mediobassa.CRF.ToString().Replace(',', '.') + " -preset:v " + q_mediobassa.Preset + " -aq-mode " + q_mediobassa.AQmode.ToString();
                            break;
                    }
                    break;
                case "Bassa":
                    switch (prof.Split(' ')[0])
                    {
                        case "XviD":
                            comando += " -q:v:" + cv.Indice.ToString() + " " + q_bassa.QP.ToString() + " " + parametri_video.XVID;
                            break;
                        default:
                            comando += " -crf: " + q_bassa.CRF.ToString().Replace(',', '.') + " -preset:v " + q_bassa.Preset + " -aq-mode " + q_bassa.AQmode.ToString();
                            break;
                    }
                    break;
                case "Bassissima":
                    switch (prof.Split(' ')[0])
                    {
                        case "XviD":
                            comando += " -q:v:" + cv.Indice.ToString() + " " + q_bassissima.QP.ToString() + " " + parametri_video.XVID;
                            break;
                        default:
                            comando += " -crf: " + q_bassissima.CRF.ToString().Replace(',', '.') + " -preset:v " + q_bassissima.Preset + " -aq-mode " + q_bassissima.AQmode.ToString();
                            break;
                    }
                    break;
                case "Bozza":
                    switch (prof.Split(' ')[0])
                    {
                        case "XviD":
                            comando += " -q:v:" + cv.Indice.ToString() + " " + q_bozza.QP.ToString() + " " + parametri_video.XVID;
                            break;
                        default:
                            comando += " -crf: " + q_bozza.CRF.ToString().Replace(',', '.') + " -preset:v " + q_bozza.Preset + " -aq-mode " + q_bozza.AQmode.ToString();
                            break;
                    }
                    break;
            }
            switch (prof.Split(' ')[0])
            {
                case "Bluray":
                    comando += " " + parametri_video.BLURAY;
                    break;
                case "Streaming":
                    if (prof.ToLower().Contains("h.264"))
                        comando += " " + parametri_video.STREAMING;
                    else
                        comando += " " + parametri_video.H265;
                    break;
                case "Workraw":
                    comando += " " + parametri_video.WORKRAW;
                    break;
                case "XviD":
                    comando += " " + parametri_video.XVID;
                    break;
            }
            if (ca.Count > 0)
            {
                List<CatturaAudio> temp3 = new List<CatturaAudio>();
                Int32 cont_audio = 0;
                if (submode.StartsWith("H"))
                {
                    foreach (CatturaAudio c in ca)
                    {
                        if (c.Default == true || c.Forzata == true)
                        {
                            temp3.Clear();
                            temp3.Add(c);
                        }
                    }
                    if (temp3.Count == 0)
                    {
                        temp3.Clear();
                        temp3.Add(new CatturaAudio(media.Audio[0]));
                    }
                }
                else
                {
                    temp3 = ca;
                }

                foreach (CatturaAudio c in temp3)
                {
                    ImpostazioniProfiliAudio parametri_audio = new ImpostazioniProfiliAudio(prof, qualita, c);

                    switch (c.Formato)
                    {
                        case "MP3":
                            if (prof.StartsWith("XviD"))
                            {
                                if (c.BitrateVariabile == true)
                                {
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -b:a:" + cont_audio + " " + parametri_audio.Bitrate + "k";
                                }
                                else
                                {
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " copy";
                                }
                            }
                            else
                            {
                                if (prof.StartsWith("Work"))
                                {
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -q:a:" + cont_audio + " " + parametri_audio.Q;
                                }
                                else
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -b:a:" + cont_audio + " " + parametri_audio.Bitrate + "k";
                            }
                            break;
                        case "AAC":
                            if (prof.StartsWith("Bluray AAC") || prof.StartsWith("PS3") || prof.StartsWith("Xbox") || prof.StartsWith("Streaming"))
                            {
                                if (prof.StartsWith("Streaming") && c.Canali > 2)
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -b:a:" + cont_audio + " " + parametri_audio.Bitrate + "k";
                                else
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " copy";
                            }
                            else
                            {
                                if (prof.StartsWith("Work"))
                                {
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -q:a:" + cont_audio + " " + parametri_audio.Q;
                                }
                                else
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -b:a:" + cont_audio + " " + parametri_audio.Bitrate + "k";
                            }
                            break;
                        case "AC-3":
                            if (prof.StartsWith("Bluray AC3"))
                            {
                                comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " copy";
                            }
                            else
                            {
                                if (prof.StartsWith("Work"))
                                {
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -q:a:" + cont_audio + " " + parametri_audio.Q;
                                }
                                else
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -b:a:" + cont_audio + " " + parametri_audio.Bitrate + "k";
                            }
                            break;
                        case "Vorbis":
                            if (prof.StartsWith("Work"))
                            {
                                if (c.Canali > 1)
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -q:a:" + cont_audio + " " + parametri_audio.Q;
                                else
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " copy";
                            }
                            else
                            {
                                comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -q:a:" + cont_audio + " " + parametri_audio.Q;
                            }
                            break;
                        default:
                            if (prof.StartsWith("Work"))
                            {
                                if (c.Canali > 2)
                                    comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -q:a:" + cont_audio + " " + parametri_audio.Q;
                                comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -q:a:" + cont_audio + " " + parametri_audio.Q;
                            }
                            else
                                comando += " -map 0:" + c.Indice.ToString() + " -c:a:" + cont_audio + " " + parametri_audio.CODEC + " -ac:a:" + cont_audio + " " + parametri_audio.Canali + " -b:a:" + cont_audio + " " + parametri_audio.Bitrate + "k";
                            break;
                    }
                    cont_audio++;
                }
            }
            else
            {
                comando += " -an";
            }

            if (submode.StartsWith("S"))
            {
                if (media.Text.Count > 0)
                    comando += " -map 0:s:? -map 0:t:? -c:t copy -c:s copy";
            }

            if (submode.StartsWith("N"))
            {
                comando += " -sn";
            }

            comando += FiltriCodifica(sub, ff, cv, largh, alt, submode) + " -aspect " + cv.AspectRatio;

            if (prof.ToLower().Contains("xvid"))
            {
                if (submode.StartsWith("H") || submode.StartsWith("N"))
                {
                    file_finale = Path.GetDirectoryName(video) + "\\" + Path.GetFileNameWithoutExtension(video) + "[" + prof + " " + alt.ToString() + "p, " + qualita + "].avi";
                }
                else
                {
                    file_finale = Path.GetDirectoryName(video) + "\\" + Path.GetFileNameWithoutExtension(video) + "[" + prof + " " + alt.ToString() + "p, " + qualita + "].mkv";
                    comando = comando.Replace(" -f avi", String.Empty);
                }
            }
            else
            {
                if (prof.StartsWith("Work") == true)
                {
                    file_finale = Path.GetDirectoryName(video) + "\\" + Path.GetFileNameWithoutExtension(video) + "[" + prof + " " + alt.ToString() + "p, " + qualita + "].mkv";
                }
                else
                {
                    if (submode.StartsWith("H") || submode.StartsWith("N"))
                    {
                        file_finale = Path.GetDirectoryName(video) + "\\" + Path.GetFileNameWithoutExtension(video) + "[" + prof + " " + alt.ToString() + "p, " + qualita + "].mp4";
                    }
                    else
                    {
                        file_finale = Path.GetDirectoryName(video) + "\\" + Path.GetFileNameWithoutExtension(video) + "[" + prof + " " + alt.ToString() + "p, " + qualita + "].mkv";
                    }
                }
            }

            if (ca != null)
                comando += " -metadata:s:a title=\"\"";
            comando += " -metadata description=\"Encoded with " + this.Text + " by Tanoshimi no Sekai Fansub. Come to visit us at https://tnsfansub.com\" \"" + file_finale + "\"";
            //MessageBox.Show(comando);
            AvviaCodifica();
        }

        public String FiltriCodifica(List<String> subs, String dir_ff, CatturaVideo cv, Int32 largh, Int32 alt, String submode)
        {
            String temp = String.Empty;

            if (cv.Interlacciato == true)
            {
                //temp += ",yadif";
            }

            if (submode.StartsWith("H"))
            {
                for (Int32 a = 0; a < subs.Count; a++)
                {
                    if (Path.GetExtension(subs[a]) == ".ass")
                        temp += ",\"ass=" + Path.GetFileName(subs[a]) + ":fontsdir=" + Path.GetFileName(dir_ff) + "\"";
                    else
                        temp += ",\"subtitles=" + Path.GetFileName(subs[a]) + ":fontsdir=" + Path.GetFileName(dir_ff) + "\"";
                }
            }

            temp += ResizeAuto(cv.AspectRatio, largh, cv.Larghezza, alt, cv.Altezza);
            if (String.IsNullOrWhiteSpace(temp) == false)
            {
                return " -vf " + temp.Trim(',');
            }
            else
                return String.Empty;
        }

        public void ferma_tutto()
        {
            try
            {
                if (t != null)
                    t.Abort();
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(ffmpeg_x64)))
                {
                    p.Kill();
                }
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(ffmpeg_x86)))
                {
                    p.Kill();
                }
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(mkvmerge)))
                {
                    p.Kill();
                }
            }
            catch { }
        }

        private void TnA_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SetTopLevel(true);
            if (forza_chiusura == true)
            {
                e.Cancel = false;
                chiudi();
            }
            else
            {
                if (MessageBox.Show("Si sta per chiudere l'applicazione. Continuare?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    chiudi();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public void chiudi()
        {
            try
            {
                ferma_tutto();
                if (Directory.Exists(temp_folder))
                    Directory.Delete(temp_folder, true);
                if (Directory.Exists(fonts_folder))
                {
                    Directory.Delete(fonts_folder, true);
                }
                foreach (String s in Directory.GetFiles(Path.GetDirectoryName(ffmpeg_x64)))
                {
                    if (Path.GetFileName(s).ToLower().Contains("sub"))
                        System.IO.File.Delete(s);
                }
            }
            catch { }
            IniFile ini = new IniFile(file_settings);
            ini.Write("WindowState", this.WindowState.ToString());
            if (manualeToolStripMenuItem.CheckState == CheckState.Checked)
                ini.Write("upd_freq", "m");
            if (iToolStripMenuItem.CheckState == CheckState.Checked)
                ini.Write("upd_freq", "s");
            if (unGiornoToolStripMenuItem.CheckState == CheckState.Checked)
                ini.Write("upd_freq", "1");
            if (ogniTreGiorniToolStripMenuItem.CheckState == CheckState.Checked)
                ini.Write("upd_freq", "3");
            if (ogniSettimanaToolStripMenuItem.CheckState == CheckState.Checked)
                ini.Write("upd_freq", "7");
        }

        private void rimuoviIFileSelezionatiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow d in DGV_video.SelectedRows)
            {
                DGV_video.Rows.Remove(d);
            }
            if (DGV_video.Rows.Count == 0)
            {
                b_avvia.Enabled = false;
                tb_help.Visible = true;
            }
            tab_autohardsubber.Text = label_tab_lista + " (Totale files: " + DGV_video.Rows.Count.ToString() + ")";
        }

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void confermaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow d in DGV_video.SelectedRows)
            {
                d.Cells[DGV_video.Columns["compatibilita"].Index].Value = cmb_compatibilita.Text;
                String p = d.Cells[DGV_video.Columns["compatibilita"].Index].Value.ToString();
            }
        }

        private void confermaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow d in DGV_video.SelectedRows)
            {
                d.Cells[DGV_video.Columns["qualita"].Index].Value = cmb_qualita.Text;
            }
        }

        public static string Arrotonda(double value, int decimalPlaces)
        {
            string formatter = "{0:f" + decimalPlaces + "}";
            return string.Format(formatter, value);
        }

        private void scaricaFFmpegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DF(false);
        }

        public void DF(Boolean auto)
        {
            df = new DownloadFFMPEG(temp_folder, auto);
            df.ShowInTaskbar = true;
            //df.Activate();
            DialogResult gui = df.ShowDialog();
            while (gui == DialogResult.Abort)
            {
                DialogResult scelta = MessageBox.Show("Non è stato possibile avviare il downloader a causa di una possibile mancanza di una connessione ad internet.\nRiprovare?", this.Text, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (scelta == DialogResult.Retry)
                {
                    df = new DownloadFFMPEG(temp_folder, auto);
                    gui = df.ShowDialog();
                }
                else
                    gui = DialogResult.No;
            }
            if (gui == DialogResult.OK)
            {
                if (Environment.Is64BitOperatingSystem == true)
                {
                    try
                    {
                        System.IO.File.Copy(temp_folder + "\\ffmpeg.exe", Path.GetDirectoryName(ffmpeg_x64) + "\\" + Path.GetFileName(ffmpeg_x64), true);
                        MessageBox.Show("FFmpeg scaricato e impostato correttamente. Ora è possibile utilizzare il programma.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        data_last_upd = DateTime.Now;
                        IniFile ini = new IniFile(file_settings);
                        ini.Write("last_upd", data_last_upd.ToShortDateString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    try
                    {
                        System.IO.File.Copy(temp_folder + "\\ffmpeg.exe", Path.GetDirectoryName(ffmpeg_x86) + "\\" + Path.GetFileName(ffmpeg_x86), true);
                        MessageBox.Show("FFmpeg scaricato e impostato correttamente. Ora è possibile utilizzare il programma.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        data_last_upd = DateTime.Now;
                        IniFile ini = new IniFile(file_settings);
                        ini.Write("last_upd", data_last_upd.ToShortDateString());

                        Environment.CurrentDirectory = Path.GetDirectoryName(ffmpeg_x64);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                foreach (String s in System.IO.Directory.GetFiles(temp_folder,"*",SearchOption.AllDirectories))
                {
                    if (s.ToLower().Contains("license"))
                    {
                        System.IO.File.Copy(s, Path.Combine(Path.GetDirectoryName(ffmpeg_x64), Path.GetFileName(s)), true);
                    }
                }
                foreach (String s in System.IO.Directory.GetDirectories(temp_folder))
                    System.IO.Directory.Delete(s, true);
                foreach (String s in Directory.GetFiles(temp_folder))
                    System.IO.File.Delete(s);

            }
        }

        private void testFFmpegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.GetFiles(Path.GetDirectoryName(ffmpeg_x64)).Count() > 0)
            {
                foreach (String s in System.IO.Directory.GetFiles(Path.GetDirectoryName(ffmpeg_x64)))
                {
                    if (Path.GetFileName(s).ToLower().Contains(Path.GetFileName(ffmpeg_x64)) || Path.GetFileName(s).ToLower().Contains(Path.GetFileName(ffmpeg_x86)))
                    {
                        Environment.CurrentDirectory = Path.GetDirectoryName(ffmpeg_x64);
                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                        psi.CreateNoWindow = true;
                        psi.UseShellExecute = false;
                        psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        psi.FileName = "cmd.exe";
                        psi.Arguments = " /c " + Path.GetFileNameWithoutExtension(s) + " -encoders>enc.txt";
                        System.Diagnostics.Process.Start(psi).WaitForExit();
                        psi.Arguments = " /c " + Path.GetFileNameWithoutExtension(s) + " -filters>fil.txt";
                        System.Diagnostics.Process.Start(psi).WaitForExit();
                        if (System.IO.File.Exists(Path.GetDirectoryName(ffmpeg_x64) + "\\enc.txt") && System.IO.File.Exists(Path.GetDirectoryName(ffmpeg_x64) + "\\fil.txt"))
                        {
                            String[] enc = System.IO.File.ReadAllLines(Path.GetDirectoryName(ffmpeg_x64) + "\\enc.txt");
                            System.IO.File.Delete(Path.GetDirectoryName(ffmpeg_x64) + "\\enc.txt");
                            Boolean aac = false, libx264 = false, ac3 = false, vfscale = false, libx265 = false, hqdn3d = false, gradfun = false, xvid = false, mp3 = false, nvenc = false;
                            foreach (String t in enc)
                            {
                                if (t.Contains("AAC (Advanced Audio Coding)"))
                                    aac = true;
                                if (t.Contains("libx264"))
                                    libx264 = true;
                                if (t.Contains("libx265"))
                                    libx265 = true;
                                if (t.Contains("ATSC A/52A (AC-3)"))
                                    ac3 = true;
                                if (t.Contains("libxvid"))
                                    xvid = true;
                                if (t.Contains("libmp3lame"))
                                    mp3 = true;
                                if (t.Contains("nvenc"))
                                    nvenc = true;
                            }
                            String[] filters = System.IO.File.ReadAllLines(Path.GetDirectoryName(ffmpeg_x64) + "\\fil.txt");
                            System.IO.File.Delete(Path.GetDirectoryName(ffmpeg_x64) + "\\fil.txt");
                            Boolean ass = false;
                            foreach (String t in filters)
                            {
                                if (t.Contains("ass") && t.ToLower().Contains("render"))
                                    ass = true;
                                if (t.Contains("scale") && t.Contains("Scale the input video size and/or convert the image format."))
                                    vfscale = true;
                                if (t.Contains("hqdn3d"))
                                    hqdn3d = true;
                                if (t.Contains("gradfun"))
                                    gradfun = true;
                            }
                            TestFFMPEG test = new TestFFMPEG(libx264, aac, ac3, ass, libx265, vfscale, hqdn3d, gradfun, xvid, mp3, nvenc);
                            DialogResult risultato = test.ShowDialog();
                            if (risultato == DialogResult.Yes)
                            {
                                MessageBox.Show("Il test ha dato esito positivo. Il programma può essere usato tranquillamente.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                DialogResult esito = MessageBox.Show("Il test ha dato esito negativo. Scaricare ora la build predefinita?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (esito == DialogResult.Yes)
                                {
                                    scaricaFFmpegToolStripMenuItem_Click(sender, e);
                                    if (df.DialogResult == DialogResult.OK)
                                        testFFmpegToolStripMenuItem_Click(sender, e);
                                }
                            }
                        }
                        break;
                    }
                    else
                    {
                        DialogResult scelta = MessageBox.Show("Eseguibile di FFmpeg non trovato. Scaricarlo ora?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                        if (scelta == DialogResult.Yes)
                        {
                            scaricaFFmpegToolStripMenuItem_Click(sender, e);
                            if (df.DialogResult == DialogResult.OK)
                            {
                                DialogResult inizio = MessageBox.Show("Iniziare ora il test?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (inizio == DialogResult.Yes)
                                {
                                    testFFmpegToolStripMenuItem_Click(sender, e);
                                }
                            }
                            else
                                MessageBox.Show("Non è obbligatorio effettuare un test di FFmpeg.\nSi consiglia di effettuarlo in quanto non tutte le build di FFmpeg sono compatibili.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("Finché mancherà l'eseguibile di FFmpeg, il programma non potrà funzionare.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            else
            {
                DialogResult scelta = MessageBox.Show("Eseguibile di FFmpeg non trovato. Scaricarlo ora?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (scelta == DialogResult.Yes)
                {
                    scaricaFFmpegToolStripMenuItem_Click(sender, e);
                    if (df.DialogResult == DialogResult.OK)
                    {
                        DialogResult inizio = MessageBox.Show("Iniziare ora il test?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (inizio == DialogResult.Yes)
                        {
                            testFFmpegToolStripMenuItem_Click(sender, e);
                        }
                    }
                    else
                        MessageBox.Show("Non è obbligatorio effettuare un test di FFmpeg.\nSi consiglia di effettuarlo in quanto non tutte le build di FFmpeg sono compatibili.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("Finché mancherà l'eseguibile di FFmpeg, il programma non potrà funzionare.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void IncollaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Collections.Specialized.StringCollection data = new System.Collections.Specialized.StringCollection();
            if (Clipboard.GetFileDropList().Count > 0)
                data = Clipboard.GetFileDropList();
            else
            {
                if (Clipboard.ContainsText() == true)
                {
                    String[] c = Clipboard.GetText().Split('\n');
                    for (Int32 i = 0; i < c.Count() - 1; i++)
                        data.Add(c[i]);
                }
            }
            String[] temp = new String[data.Count];
            data.CopyTo(temp, 0);
            recupera_fd(temp);
        }

        private void salvaImpostazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IniFile ini = new IniFile(file_settings);

            ini.Write("comp", cmb_compatibilita.Text);
            ini.Write("risoluz", cmb_risoluz.Text);
            ini.Write("qual", cmb_qualita.Text);
            ini.Write("subs", cmb_subs.Text);

            if (monitoraGliAppuntiToolStripMenuItem.CheckState == CheckState.Checked)
                ini.Write("monit_clipb", "y");
            else
                ini.Write("monit_clipb", "n");
        }

        private void bgw_update_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (DateTime.Now.ToShortDateString().CompareTo(data_vecchia) > 0)
                {
                    DateTime data = new DateTime(Convert.ToInt32(data_vecchia.Split('/')[2]), Convert.ToInt32(data_vecchia.Split('/')[1]), Convert.ToInt32(data_vecchia.Split('/')[0]));
                    //MessageBox.Show(data.ToString
                    TimeSpan t = new TimeSpan();
                    t = DateTime.Today - data;
                    if (t.TotalDays <= 1)
                    {
                        DialogResult scelta = MessageBox.Show("La build corrente di FFmpeg è vecchia di " + t.TotalDays.ToString() + " giorno.\nAggiornare la build di FFmpeg?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (scelta == DialogResult.Yes)
                        {
                            DF(true);
                        }
                    }
                    else
                    {
                        DialogResult scelta = MessageBox.Show("La build corrente di FFmpeg è vecchia di " + t.TotalDays.ToString() + " giorni.\nAggiornare la build di FFmpeg?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (scelta == DialogResult.Yes)
                        {
                            DF(true);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Data non valida: " + data_vecchia + "\n\n" + exc.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //IniFile ini = new IniFile(file_settings);
                //ini.Write("last_upd", DateTime.Now.ToShortDateString());
            }
        }

        private void bgw_update_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Enabled = true;
            this.Activate();
        }

        private void bgw_downloadffmpeg_DoWork(object sender, DoWorkEventArgs e)
        {
            if (MessageBox.Show("Eseguibile di FFmpeg non trovato. Scaricarlo?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                DF(true);
            else
                MessageBox.Show("Finché mancherà l'eseguibile di FFmpeg, il programma non potrà funzionare.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void bgw_downloadffmpeg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Enabled = true;
            this.Activate();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About(this.Text, this.Text.Split(' ')[5]);
            a.Icon = this.Icon;
            a.ShowInTaskbar = false;
            a.ShowDialog();
        }

        private void ll_icons_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(ll_icons.Text);
        }

        private void b_agg_files_Click(object sender, EventArgs e)
        {
            apri_video.Filter = filtro;
            if (apri_video.ShowDialog() == DialogResult.OK)
            {
                foreach (String s in apri_video.FileNames)
                {
                    DGV_video.Rows.Add(Path.GetFileName(s), cmb_compatibilita.Text, cmb_risoluz.Text, cmb_qualita.Text, cmb_subs.Text, "PRONTO", Path.GetDirectoryName(s));
                }
                b_avvia.Enabled = true;
                tab_autohardsubber.Text = label_tab_lista + " (Totale files: " + DGV_video.Rows.Count.ToString() + ")";
                tb_help.Visible = false;
            }
        }

        private void b_agg_cart_Click(object sender, EventArgs e)
        {
            if (apri_cartella.ShowDialog() == DialogResult.OK)
            {
                Seleziona_formati sf = new Seleziona_formati(estensioni_video);
                sf.Icon = this.Icon;
                sf.TopLevel = true;
                sf.StartPosition = FormStartPosition.CenterParent;
                sf.ShowInTaskbar = false;
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    foreach (String s in Directory.GetFiles(apri_cartella.SelectedPath, "*", SearchOption.AllDirectories))
                    {
                        if (TnA___Tanoshimi_no_Autohardsubber.Seleziona_formati.formati_scelti.Contains(Path.GetExtension(s).ToLower()) == true)
                        {
                            DGV_video.Rows.Add(Path.GetFileName(s), cmb_compatibilita.Text, cmb_risoluz.Text, cmb_qualita.Text, cmb_subs.Text, "PRONTO", Path.GetDirectoryName(s));
                        }
                    }
                }
                if (DGV_video.Rows.Count > 0)
                {
                    b_avvia.Enabled = true;
                    tb_help.Visible = false;
                    tab_autohardsubber.Text = label_tab_lista + " (Totale files: " + DGV_video.Rows.Count.ToString() + ")";
                }
            }
        }

        private void b_incolla_Click(object sender, EventArgs e)
        {
            IncollaToolStripMenuItem_Click(sender, e);
        }

        private void B_rimuovi_Click(object sender, EventArgs e)
        {
            rimuoviIFileSelezionatiToolStripMenuItem_Click(sender, e);
        }

        private void TnA_ResizeEnd(object sender, EventArgs e)
        {
            IniFile ini = new IniFile(file_settings);
            ini.Write("Width", this.Width.ToString());
            ini.Write("Heigth", this.Height.ToString());
        }

        private void MonitoraGliAppuntiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (monitoraGliAppuntiToolStripMenuItem.CheckState == CheckState.Checked)
            {
                monitoraGliAppuntiToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                monitoraGliAppuntiToolStripMenuItem.CheckState = CheckState.Checked;
            }
        }

        private void ripristinaImpostazioniToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Ripristinare programma e impostazioni e riavviare l'applicazione?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                forza_chiusura = true;
                if (System.IO.File.Exists(file_settings))
                {
                    System.IO.File.Delete(file_settings);
                }
                foreach (String s in System.IO.Directory.GetDirectories(Path.GetDirectoryName(ffmpeg_x64), "*", SearchOption.AllDirectories))
                {
                    if (s.ToLower().Contains("fonts") == false)
                    {
                        System.IO.Directory.Delete(s, true);
                    }
                }
                foreach (String s in System.IO.Directory.GetFiles(Path.GetDirectoryName(ffmpeg_x64), "*", SearchOption.TopDirectoryOnly))
                {
                    if (s.ToLower().Contains("fonts") == false)
                    {
                        System.IO.File.Delete(s);
                    }
                }
                foreach (String s in System.IO.Directory.GetFiles(LOG_dir, "*", SearchOption.TopDirectoryOnly))
                {
                    System.IO.File.Delete(s);
                }
                Application.Restart();
            }
        }

        private void manualeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (manualeToolStripMenuItem.CheckState == CheckState.Unchecked)
            {
                manualeToolStripMenuItem.CheckState = CheckState.Checked;
                iToolStripMenuItem.CheckState = CheckState.Unchecked;
                unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void iToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (iToolStripMenuItem.CheckState == CheckState.Unchecked)
            {
                manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                iToolStripMenuItem.CheckState = CheckState.Checked;
                unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void unGiornoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (unGiornoToolStripMenuItem.CheckState == CheckState.Unchecked)
            {
                manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                iToolStripMenuItem.CheckState = CheckState.Unchecked;
                unGiornoToolStripMenuItem.CheckState = CheckState.Checked;
                ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void ogniTreGiorniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ogniTreGiorniToolStripMenuItem.CheckState == CheckState.Unchecked)
            {
                manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                iToolStripMenuItem.CheckState = CheckState.Unchecked;
                unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Checked;
                ogniSettimanaToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void ogniSettimanaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ogniSettimanaToolStripMenuItem.CheckState == CheckState.Unchecked)
            {
                manualeToolStripMenuItem.CheckState = CheckState.Unchecked;
                iToolStripMenuItem.CheckState = CheckState.Unchecked;
                unGiornoToolStripMenuItem.CheckState = CheckState.Unchecked;
                ogniTreGiorniToolStripMenuItem.CheckState = CheckState.Unchecked;
                ogniSettimanaToolStripMenuItem.CheckState = CheckState.Checked;
            }
        }

        private void oKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (Int32 i=DGV_video.Rows.Count-1; i>=0; i--)
            {
                if (DGV_video.Rows[i].Cells[DGV_video.Columns["stato"].Index].Value.ToString().Trim().ToLower().Contains(oKToolStripMenuItem.Text.ToLower().Trim()))
                    DGV_video.Rows.RemoveAt(DGV_video.Rows[i].Index);
            }
        }

        private void fermatoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (Int32 i = DGV_video.Rows.Count - 1; i >= 0; i--)
            {
                if (DGV_video.Rows[i].Cells[DGV_video.Columns["stato"].Index].Value.ToString().ToLower().Trim().Contains(fermatoToolStripMenuItem.Text.ToLower().Trim()))
                    DGV_video.Rows.RemoveAt(DGV_video.Rows[i].Index);
            }
        }

        private void timer_tempo_Tick(object sender, EventArgs e)
        {
            TimeSpan tp = TimeSpan.FromSeconds((Double)sec_trasc);
            l_tempo_trasc.Text = "Tempo trascorso: " + tp.ToString(@"hh\:mm\:ss");
            sec_trasc++;
        }

        private void creaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String link = Environment.GetFolderPath(Environment.SpecialFolder.SendTo) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".lnk";
            var shell = new WshShell();
            var shortcut = shell.CreateShortcut(link) as IWshShortcut;
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Application.StartupPath;
            shortcut.Save();
        }

        private void cancellaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.SendTo) + "\\" + Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".lnk");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            azione_fine_coda = toolStripComboBox1.Text;
        }

        private void DGV_video_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV_video.Rows.Count > 0)
            {
                if (e.ColumnIndex == DGV_video.Columns["compatibilita"].Index)
                {
                    String p = DGV_video.Rows[e.RowIndex].Cells[DGV_video.Columns["compatibilita"].Index].Value.ToString();
                }
            }
        }

        private void b_pause_Click(object sender, EventArgs e)
        {
            if (pause == false)
            {
                
                if (DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["compatibilita"].Index].Value.ToString().ToLower().Contains("mkv") == false)
                    SuspendProcess(processo_codifica.Id);
                else
                    SuspendProcess(processo_remux.Id);
                pause = true;
                b_pause.Image = new Bitmap(TnA___Tanoshimi_no_Autohardsubber.Properties.Resources.play, new Size(TnA___Tanoshimi_no_Autohardsubber.Properties.Resources.pause.Width, TnA___Tanoshimi_no_Autohardsubber.Properties.Resources.pause.Height));
                b_pause.Text = "Riprendi";
            }
            else
            {
                
                if (DGV_video.Rows[indice_percentuale].Cells[DGV_video.Columns["compatibilita"].Index].Value.ToString().ToLower().Contains("mkv") == false)
                    ResumeProcess(processo_codifica.Id);
                else
                    ResumeProcess(processo_remux.Id);
                pause = false;
                b_pause.Image = TnA___Tanoshimi_no_Autohardsubber.Properties.Resources.pause;
                b_pause.Text = "Pausa";
            }
        }

        private void erroreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (Int32 i = DGV_video.Rows.Count - 1; i >= 0; i--)
            {
                if (DGV_video.Rows[i].Cells[DGV_video.Columns["stato"].Index].Value.ToString().ToLower().Trim().Contains(erroreToolStripMenuItem.Text.ToLower().Trim()))
                    DGV_video.Rows.RemoveAt(DGV_video.Rows[i].Index);
            }
        }

        private void controllaAggiornamentiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlla_aggiornamenti(false);
        }

        public void controlla_aggiornamenti(Boolean Hidden)
        {
            try
            {
                Updater.TnA_Updater updater = new Updater.TnA_Updater(this.Text.Split(' ')[5], Hidden, this.Icon, this.Text, false, Application.StartupPath);
                try
                {
                    switch (updater.DialogResult)
                    {
                        case DialogResult.Yes:
                            nuovaVersioneDisponibileToolStripMenuItem.Visible = true;
                            controllaAggiornamentiToolStripMenuItem.Visible = false;
                            visualizzaCronologiaVersioniiToolStripMenuItem.Visible = false;
                            break;
                        case DialogResult.None:
                            nuovaVersioneDisponibileToolStripMenuItem.Visible = false;
                            controllaAggiornamentiToolStripMenuItem.Visible = true;
                            visualizzaCronologiaVersioniiToolStripMenuItem.Visible = true;
                            break;
                        //default:
                        //    updater.ShowDialog();
                        //    break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void confermaToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow d in DGV_video.SelectedRows)
            {
                d.Cells[DGV_video.Columns["risoluz"].Index].Value = cmb_risoluz.Text;
            }
        }

        private void NuovaVersioneDisponibileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlla_aggiornamenti(false);
        }

        private void Bgw_updateschecker_DoWork(object sender, DoWorkEventArgs e)
        {
            controlla_aggiornamenti(true);
        }

        private void ApriCartellaLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(LOG_dir);
        }

        private void ConfermaToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow d in DGV_video.SelectedRows)
            {
                d.Cells[DGV_video.Columns["subtitle_mode"].Index].Value = cmb_subs.Text;
            }
        }

        private void Rtb_sottotitoli_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void VisualizzaCronologiaVersioniiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CronologiaVersioni c = new CronologiaVersioni(this.Text, this.Icon);
            c.ShowDialog();
        }

        private void DGV_video_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DGV_video.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

    }

    public class CatturaAudio
    {
        public String Formato { get; set; }
        public String Profilo { get; set; }
        public Double Canali { get; set; }
        public Int32 Indice { get; set; }
        public Boolean BitrateVariabile { get; set; }
        public Boolean Forzata { get; set; }
        public Boolean Default { get; set; }
        public Boolean Lossless { get; set; }

        public CatturaAudio(MediaInfoDotNet.Models.AudioStream a)
        {
            Cattura(a);
        }

        protected void Cattura(MediaInfoDotNet.Models.AudioStream a)
        {
            Forzata = a.Forced;
            Default = a.Default;
            Formato = a.Format;

            if (a.CompressionMode.ToLower().EndsWith("y"))
                Lossless = false;
            else
                Lossless = true;
            
            if(String.IsNullOrWhiteSpace(Formato))
            {
                Formato = String.Empty;
            }

            Profilo = a.FormatProfile;
            
            if(String.IsNullOrWhiteSpace(Profilo))
            {
                Profilo = String.Empty;
            }

            Indice = a.ID - 1;

            Canali = Convert.ToDouble(a.Channels);

            if (a.BitRateMode == "CBR")
            {
                BitrateVariabile = true;
            }
            else
            {
                BitrateVariabile = false;
            }
        }
    }

    public class CatturaVideo
    {
        public String File { get; set; }
        public Int32 Indice { get; set; }
        public String AspectRatio { get; set; }
        public Double Larghezza { get; set; }
        public Double Altezza { get; set; }
        public String Framerate { get; set; }
        public Boolean Interlacciato { get; set; }
        public TimeSpan DurataPrecisa { get; set; }
        public Int32 TotaleFrames { get; set; }
        
        public CatturaVideo(MediaFile media)
        {
            Cattura(media);
        }

        protected void Cattura(MediaFile media)
        {
            File = media.filePath;
            Indice = media.Video[0].ID - 1;
            AspectRatio = media.Video[0].DisplayAspectRatio.ToString().Replace(",", ".");
            if (String.IsNullOrWhiteSpace(AspectRatio))
                AspectRatio = String.Empty;
            Framerate = (media.Video[0].FrameRate).ToString();
            DurataPrecisa = TimeSpan.FromMilliseconds(media.Video[0].Duration);
            TotaleFrames = media.Video[0].FrameCount;
            String interl = media.Video[0].miGetString("ScanType").ToLower().Trim();
            switch (interl)
            {
                case "progressive":
                    Interlacciato = false;
                    break;
                case "interlaced":
                    Interlacciato = true;
                    break;
                default:
                    Interlacciato = false;
                    break;
            }
            Larghezza = media.Video[0].Width;
            Altezza = media.Video[0].Height;
        }
    }

    public class ImpostazioniProfiliCodifica
    {
        public class ParametriVideo
        {
            public String BLURAY { get; }
            public String STREAMING { get; }
            public String XVID { get; }
            public String WORKRAW { get; }
            public String H265 { get; }

            public ParametriVideo(MediaFile media)
            {
                BLURAY = " -profile:v high -level:v 4.1 -bluray-compat 1 -maxrate 50000k -bufsize 70000k -pix_fmt yuv420p";
                STREAMING = " -maxrate 20000k -bufsize 20000k -profile:v high -level:v 4.1 -pix_fmt yuv420p -bluray-compat 1 -x264opts cabac=0:weightp=0:weightb=0:sync-lookahead=0:sliced-threads=1:b-pyramid=0 -keyint_min " + CalcolaGOP(media).ToString() + " -g " + CalcolaGOP(media).ToString();
                XVID = " -fflags +genpts -f avi -vtag XVID -bf 2 -level 5 -use_odml -1 -qmax 10 -qmin 1 -pix_fmt yuv420p -flags +mv4+loop+qpel+aic -qcomp 1.0 -subcmp 7 -mbcmp 7 -precmp 7 -subq 11 -me_range 1023 -mbd rd -profile:v mpeg4_asp -trellis 2";
                WORKRAW = " -profile:v high -level:v 4.1 -pix_fmt yuv420p -maxrate 20000k -bufsize 20000k -partitions +parti4x4+parti8x8+partp8x8+partb8x8 -x264opts b-pyramid=0 -sn -tune fastdecode";
                H265 = " -profile:v main -maxrate 50000k -bufsize 70000k -pix_fmt yuv420p -x265-params pmode=1:pme=1:psy-rd=4:rdoq-level=1:psy-rdoq=10:weightb=1:me=umh:subme=4:rdpenalty=1:open-gop=0:rc-lookahead=40:bframes=4:rd=4:b-adapt=2";
            }

            protected Int32 CalcolaGOP(MediaFile media)
            {
                return (Int32)Math.Ceiling(Convert.ToDecimal(media.Video[0].FrameRate)) * 10;
            }
        }

        public class QualitaVideo
        {
            public class Altissima
            {
                public Double CRF { get; }
                public Double QP { get; }
                public String Preset { get; }
                public Double Bitrate { get; }
                public Double AQmode { get; }
                public Double AQstrength { get; }
                public String CODEC { get; }

                public Altissima(String ProfiloScelto)
                {
                    if (ProfiloScelto.ToLower().Contains("xvid") == true)
                    {
                        QP = 1;
                        CRF = Double.NaN;
                        Preset = String.Empty;
                        AQmode = Double.NaN;
                        AQstrength = Double.NaN;
                        CODEC = "libxvid";
                        Bitrate = Double.NaN;
                    }
                    else
                    {
                        if (ProfiloScelto.ToLower().Contains("workraw") == true)
                        {
                            CRF = 20;
                            QP = Double.NaN;
                            AQstrength = 1.5;
                        }
                        else
                        {
                            if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            {
                                CRF = 17;
                                QP = Double.NaN;
                            }
                            else
                            {
                                CRF = 14;
                                QP = Double.NaN;
                            }
                        }
                        if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            CODEC = "libx265";
                        else
                            CODEC = "libx264";
                        Preset = "veryslow";
                        AQmode = 2;
                        Bitrate = Double.NaN;
                    }
                }
            }
            public class Alta
            {
                public Double CRF { get; }
                public Double QP { get; }
                public String Preset { get; }
                public Double Bitrate { get; }
                public Double AQmode { get; }
                public Double AQstrength { get; }
                public String CODEC { get; }

                public Alta(String ProfiloScelto)
                {
                    if (ProfiloScelto.ToLower().Contains("xvid") == true)
                    {
                        QP = 2;
                        CRF = Double.NaN;
                        Preset = String.Empty;
                        AQmode = Double.NaN;
                        AQstrength = Double.NaN;
                        CODEC = "libxvid";
                        Bitrate = Double.NaN;
                    }
                    else
                    {
                        if (ProfiloScelto.ToLower().Contains("workraw") == true)
                        {
                            CRF = 23;
                            QP = Double.NaN;
                            AQstrength = 1.5;
                        }
                        else
                        {
                            if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            {
                                CRF = 20;
                                QP = Double.NaN;
                            }
                            else
                            {
                                CRF = 17;
                                QP = Double.NaN;
                            }
                        }
                        if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            CODEC = "libx265";
                        else
                            CODEC = "libx264";
                        Preset = "veryslow";
                        AQmode = 2;
                        Bitrate = Double.NaN;
                    }
                }
            }
            public class MedioAlta
            {
                public Double CRF { get; }
                public Double QP { get; }
                public String Preset { get; }
                public Double Bitrate { get; }
                public Double AQmode { get; }
                public Double AQstrength { get; }
                public String CODEC { get; }

                public MedioAlta(String ProfiloScelto)
                {
                    if (ProfiloScelto.ToLower().Contains("xvid") == true)
                    {
                        QP = 3;
                        CRF = Double.NaN;
                        Preset = String.Empty;
                        AQmode = Double.NaN;
                        AQstrength = Double.NaN;
                        CODEC = "libxvid";
                        Bitrate = Double.NaN;
                    }
                    else
                    {
                        if (ProfiloScelto.ToLower().Contains("workraw") == true)
                        {
                            CRF = 26;
                            QP = Double.NaN;
                            AQstrength = 1.6;
                        }
                        else
                        {
                            if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            {
                                CRF = 21.5;
                                QP = Double.NaN;
                            }
                            else
                            {
                                CRF = 18.5;
                                QP = Double.NaN;
                            }
                        }
                        if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            CODEC = "libx265";
                        else
                            CODEC = "libx264";
                        Preset = "slower";
                        AQmode = 2;
                        Bitrate = Double.NaN;
                    }
                }
            }
            public class Media
            {
                public Double CRF { get; }
                public Double QP { get; }
                public String Preset { get; }
                public Double Bitrate { get; }
                public Double AQmode { get; }
                public Double AQstrength { get; }
                public String CODEC { get; }

                public Media(String ProfiloScelto)
                {
                    if (ProfiloScelto.ToLower().Contains("xvid") == true)
                    {
                        QP = 4;
                        CRF = Double.NaN;
                        Preset = String.Empty;
                        AQmode = Double.NaN;
                        AQstrength = Double.NaN;
                        CODEC = "libxvid";
                        Bitrate = Double.NaN;
                    }
                    else
                    {
                        if (ProfiloScelto.ToLower().Contains("workraw") == true)
                        {
                            CRF = 29;
                            QP = Double.NaN;
                            AQstrength = 1.9;
                        }
                        else
                        {
                            if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            {
                                CRF = 23;
                                QP = Double.NaN;
                            }
                            else
                            {
                                CRF = 20;
                                QP = Double.NaN;
                            }
                        }
                        if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            CODEC = "libx265";
                        else
                            CODEC = "libx264";
                        Preset = "medium";
                        AQmode = 3;
                        Bitrate = Double.NaN;
                    }
                }
            }
            public class MedioBassa
            {
                public Double CRF { get; }
                public Double QP { get; }
                public String Preset { get; }
                public Double Bitrate { get; }
                public Double AQmode { get; }
                public Double AQstrength { get; }
                public String CODEC { get; }

                public MedioBassa(String ProfiloScelto)
                {
                    if (ProfiloScelto.ToLower().Contains("xvid") == true)
                    {
                        QP = 5;
                        CRF = Double.NaN;
                        Preset = String.Empty;
                        AQmode = Double.NaN;
                        AQstrength = Double.NaN;
                        CODEC = "libxvid";
                        Bitrate = Double.NaN;
                    }
                    else
                    {
                        if (ProfiloScelto.ToLower().Contains("workraw") == true)
                        {
                            CRF = 32;
                            QP = Double.NaN;
                            AQstrength = 2;
                        }
                        else
                        {
                            if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            {
                                CRF = 25;
                                QP = Double.NaN;
                            }
                            else
                            {
                                CRF = 22;
                                QP = Double.NaN;
                            }
                        }
                        if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            CODEC = "libx265";
                        else
                            CODEC = "libx264";
                        Preset = "fast";
                        AQmode = 3;
                        Bitrate = Double.NaN;
                    }
                }
            }
            public class Bassa
            {
                public Double CRF { get; }
                public Double QP { get; }
                public String Preset { get; }
                public Double Bitrate { get; }
                public Double AQmode { get; }
                public Double AQstrength { get; }
                public String CODEC { get; }

                public Bassa(String ProfiloScelto)
                {
                    if (ProfiloScelto.ToLower().Contains("xvid") == true)
                    {
                        QP = 6;
                        CRF = Double.NaN;
                        Preset = String.Empty;
                        AQmode = Double.NaN;
                        AQstrength = Double.NaN;
                        CODEC = "libxvid";
                        Bitrate = Double.NaN;
                    }
                    else
                    {
                        if (ProfiloScelto.ToLower().Contains("workraw") == true)
                        {
                            CRF = 35;
                            QP = Double.NaN;
                            AQstrength = 2;
                        }
                        else
                        {
                            if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            {
                                CRF = 28;
                                QP = Double.NaN;
                            }
                            else
                            {
                                CRF = 25;
                                QP = Double.NaN;
                            }
                        }
                        if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            CODEC = "libx265";
                        else
                            CODEC = "libx264";
                        Preset = "fast";
                        AQmode = 3;
                        Bitrate = Double.NaN;
                    }
                }
            }
            public class Bassissima
            {
                public Double CRF { get; }
                public Double QP { get; }
                public String Preset { get; }
                public Double Bitrate { get; }
                public Double AQmode { get; }
                public Double AQstrength { get; }
                public String CODEC { get; }

                public Bassissima(String ProfiloScelto)
                {
                    if (ProfiloScelto.ToLower().Contains("xvid") == true)
                    {
                        QP = 7;
                        CRF = Double.NaN;
                        Preset = String.Empty;
                        AQmode = Double.NaN;
                        AQstrength = Double.NaN;
                        CODEC = "libxvid";
                        Bitrate = Double.NaN;
                    }
                    else
                    {
                        if (ProfiloScelto.ToLower().Contains("workraw") == true)
                        {
                            CRF = 38;
                            QP = Double.NaN;
                            AQstrength = 2;
                        }
                        else
                        {
                            if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            {
                                CRF = 30;
                                QP = Double.NaN;
                            }
                            else
                            {
                                CRF = 27;
                                QP = Double.NaN;
                            }
                        }
                        if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            CODEC = "libx265";
                        else
                            CODEC = "libx264";
                        Preset = "faster";
                        AQmode = 3;
                        Bitrate = Double.NaN;
                    }
                }
            }
            public class Bozza
            {
                public Double CRF { get; }
                public Double QP { get; }
                public String Preset { get; }
                public Double Bitrate { get; }
                public Double AQmode { get; }
                public Double AQstrength { get; }
                public String CODEC { get; }

                public Bozza(String ProfiloScelto)
                {
                    if (ProfiloScelto.ToLower().Contains("xvid") == true)
                    {
                        QP = 10;
                        CRF = Double.NaN;
                        Preset = String.Empty;
                        AQmode = Double.NaN;
                        AQstrength = Double.NaN;
                        CODEC = "libxvid";
                        Bitrate = Double.NaN;
                    }
                    else
                    {
                        if (ProfiloScelto.ToLower().Contains("workraw") == true)
                        {
                            CRF = 45;
                            QP = Double.NaN;
                            AQstrength = 2;
                        }
                        else
                        {
                            if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            {
                                CRF = 39;
                                QP = Double.NaN;
                            }
                            else
                            {
                                CRF = 36;
                                QP = Double.NaN;
                            }
                        }
                        if (ProfiloScelto.ToLower().Contains("h.265") == true)
                            CODEC = "libx265";
                        else
                            CODEC = "libx264";
                        Preset = "superfast";
                        AQmode = 1;
                        Bitrate = Double.NaN;
                    }
                }
            }
        }
    }

    public class ImpostazioniProfiliAudio
    {
        public Double Bitrate { get; set; }
        public Double Q { get; set; }
        public Double Canali { get; set; }
        public String CODEC { get; set; }

        public ImpostazioniProfiliAudio(String profilo, String qualita, CatturaAudio audio)
        {
            switch (qualita)
            {
                case "Altissima":
                    if (profilo.ToLower().Contains("xvid"))
                    {
                        Bitrate = 160 * Convert.ToInt32(audio.Canali);
                        Q = Double.NaN;
                        CODEC = "libmp3lame";
                        if (audio.Canali >= 2)
                            Canali = 2;
                        else
                            Canali = audio.Canali;
                    }
                    else
                    {
                        if (profilo.ToLower().Contains("workraw"))
                        {
                            if (audio.Canali >= 1)
                                Canali = 1;
                            else
                                Canali = audio.Canali;
                            Bitrate = Double.NaN;
                            Q = 7;
                            CODEC = "libvorbis";
                        }
                        else
                        {
                            if (profilo.ToLower().Contains("ac3"))
                            {
                                Bitrate = 96 * Convert.ToInt32(audio.Canali);
                                Q = Double.NaN;
                                CODEC = "ac3";
                                Canali = audio.Canali;
                            }
                            else
                            {
                                if (profilo.ToLower().Contains("streaming"))
                                {
                                    if (audio.Canali >= 2)
                                        Canali = 2;
                                    else
                                        Canali = audio.Canali;
                                }
                                else
                                    Canali = audio.Canali;
                                Bitrate = 192 * Convert.ToInt32(Canali) / 2;
                                Q = Double.NaN;
                                CODEC = "aac";
                            }
                        }
                    }
                    break;
                case "Alta":
                    if (profilo.ToLower().Contains("xvid"))
                    {
                        Bitrate = 128 * Convert.ToInt32(audio.Canali);
                        Q = Double.NaN;
                        CODEC = "libmp3lame";
                        if (audio.Canali >= 2)
                            Canali = 2;
                        else
                            Canali = audio.Canali;
                    }
                    else
                    {
                        if (profilo.ToLower().Contains("workraw"))
                        {
                            if (audio.Canali >= 1)
                                Canali = 1;
                            else
                                Canali = audio.Canali;
                            Bitrate = Double.NaN;
                            Q = 6;
                            CODEC = "libvorbis";
                        }
                        else
                        {
                            if (profilo.ToLower().Contains("ac3"))
                            {
                                Bitrate = 96 * Convert.ToInt32(audio.Canali);
                                Q = Double.NaN;
                                CODEC = "ac3";
                                Canali = audio.Canali;
                            }
                            else
                            {
                                if (profilo.ToLower().Contains("streaming"))
                                {
                                    if (audio.Canali >= 2)
                                        Canali = 2;
                                    else
                                        Canali = audio.Canali;
                                }
                                else
                                    Canali = audio.Canali;
                                Bitrate = 160 * Convert.ToInt32(Canali) / 2;
                                Q = Double.NaN;
                                CODEC = "aac";
                            }
                        }
                    }
                    break;
                case "Medio-alta":
                    if (profilo.ToLower().Contains("xvid"))
                    {
                        Bitrate = 112 * Convert.ToInt32(audio.Canali);
                        Q = Double.NaN;
                        CODEC = "libmp3lame";
                        if (audio.Canali >= 2)
                            Canali = 2;
                        else
                            Canali = audio.Canali;
                    }
                    else
                    {
                        if (profilo.ToLower().Contains("workraw"))
                        {
                            if (audio.Canali >= 1)
                                Canali = 1;
                            else
                                Canali = audio.Canali;
                            Bitrate = Double.NaN;
                            Q = 5;
                            CODEC = "libvorbis";
                            Canali = audio.Canali;
                        }
                        else
                        {
                            if (profilo.ToLower().Contains("ac3"))
                            {
                                Bitrate = 80 * Convert.ToInt32(audio.Canali);
                                Q = Double.NaN;
                                CODEC = "ac3";
                                Canali = audio.Canali;
                            }
                            else
                            {
                                if (profilo.ToLower().Contains("streaming"))
                                {
                                    if (audio.Canali >= 2)
                                        Canali = 2;
                                    else
                                        Canali = audio.Canali;
                                }
                                else
                                    Canali = audio.Canali;
                                Bitrate = 144 * Convert.ToInt32(Canali) / 2;
                                Q = Double.NaN;
                                CODEC = "aac";
                            }
                        }
                    }
                    break;
                case "Media":
                    if (profilo.ToLower().Contains("xvid"))
                    {
                        Bitrate = 96 * Convert.ToInt32(audio.Canali);
                        Q = Double.NaN;
                        CODEC = "libmp3lame";
                        if (audio.Canali >= 2)
                            Canali = 2;
                        else
                            Canali = audio.Canali;
                    }
                    else
                    {
                        if (profilo.ToLower().Contains("workraw"))
                        {
                            Bitrate = Double.NaN;
                            Q = 4;
                            CODEC = "libvorbis";
                            if (audio.Canali >= 1)
                                Canali = 1;
                            else
                                Canali = audio.Canali;
                        }
                        else
                        {
                            if (profilo.ToLower().Contains("ac3"))
                            {
                                Bitrate = 80 * Convert.ToInt32(audio.Canali);
                                Q = Double.NaN;
                                CODEC = "ac3";
                                Canali = audio.Canali;
                            }
                            else
                            {
                                if (profilo.ToLower().Contains("streaming"))
                                {
                                    if (audio.Canali >= 2)
                                        Canali = 2;
                                    else
                                        Canali = audio.Canali;
                                }
                                else
                                    Canali = audio.Canali;
                                Bitrate = 128 * Convert.ToInt32(Canali) / 2;
                                Q = Double.NaN;
                                CODEC = "aac";
                            }
                        }
                    }
                    break;
                case "Medio-bassa":
                    if (profilo.ToLower().Contains("xvid"))
                    {
                        Bitrate = 80 * Convert.ToInt32(audio.Canali);
                        Q = Double.NaN;
                        CODEC = "libmp3lame";
                        if (audio.Canali >= 2)
                            Canali = 2;
                        else
                            Canali = audio.Canali;
                    }
                    else
                    {
                        if (profilo.ToLower().Contains("workraw"))
                        {
                            Bitrate = Double.NaN;
                            Q = 3;
                            CODEC = "libvorbis";
                            if (audio.Canali >= 1)
                                Canali = 1;
                            else
                                Canali = audio.Canali;
                        }
                        else
                        {
                            if (profilo.ToLower().Contains("ac3"))
                            {
                                Bitrate = 80 * Convert.ToInt32(audio.Canali);
                                Q = Double.NaN;
                                CODEC = "ac3";
                                Canali = audio.Canali;
                            }
                            else
                            {
                                if (profilo.ToLower().Contains("streaming"))
                                {
                                    if (audio.Canali >= 2)
                                        Canali = 2;
                                    else
                                        Canali = audio.Canali;
                                }
                                else
                                    Canali = audio.Canali;
                                Bitrate = 96 * Convert.ToInt32(Canali) / 2;
                                Q = Double.NaN;
                                CODEC = "aac";
                            }
                        }
                    }
                    break;
                case "Bassa":
                    if (profilo.ToLower().Contains("xvid"))
                    {
                        Bitrate = 64 * Convert.ToInt32(audio.Canali);
                        Q = Double.NaN;
                        CODEC = "libmp3lame";
                        if (audio.Canali >= 2)
                            Canali = 2;
                        else
                            Canali = audio.Canali;
                    }
                    else
                    {
                        if (profilo.ToLower().Contains("workraw"))
                        {
                            Bitrate = Double.NaN;
                            Q = 2;
                            CODEC = "libvorbis";
                            if (audio.Canali >= 1)
                                Canali = 1;
                            else
                                Canali = audio.Canali;
                        }
                        else
                        {
                            if (profilo.ToLower().Contains("ac3"))
                            {
                                Bitrate = 64 * Convert.ToInt32(audio.Canali);
                                Q = Double.NaN;
                                CODEC = "ac3";
                                Canali = audio.Canali;
                            }
                            else
                            {
                                if (profilo.ToLower().Contains("streaming"))
                                {
                                    if (audio.Canali >= 2)
                                        Canali = 2;
                                    else
                                        Canali = audio.Canali;
                                }
                                else
                                    Canali = audio.Canali;
                                Bitrate = 64 * Convert.ToInt32(Canali) / 2;
                                Q = Double.NaN;
                                CODEC = "aac";
                            }
                        }
                    }
                    break;
                case "Bassissima":
                    if (profilo.ToLower().Contains("xvid"))
                    {
                        Bitrate = 48 * Convert.ToInt32(audio.Canali);
                        Q = Double.NaN;
                        CODEC = "libmp3lame";
                        if (audio.Canali >= 2)
                            Canali = 2;
                        else
                            Canali = audio.Canali;
                    }
                    else
                    {
                        if (profilo.ToLower().Contains("workraw"))
                        {
                            Bitrate = Double.NaN;
                            Q = 1;
                            CODEC = "libvorbis";
                            if (audio.Canali >= 1)
                                Canali = 1;
                            else
                                Canali = audio.Canali;
                        }
                        else
                        {
                            if (profilo.ToLower().Contains("ac3"))
                            {
                                Bitrate = 48 * Convert.ToInt32(audio.Canali);
                                Q = Double.NaN;
                                CODEC = "ac3";
                                Canali = audio.Canali;
                            }
                            else
                            {
                                if (profilo.ToLower().Contains("streaming"))
                                {
                                    if (audio.Canali >= 2)
                                        Canali = 2;
                                    else
                                        Canali = audio.Canali;
                                }
                                else
                                    Canali = audio.Canali;
                                Bitrate = 48 * Convert.ToInt32(Canali) / 2;
                                Q = Double.NaN;
                                CODEC = "aac";
                            }
                        }
                    }
                    break;
                case "Bozza":
                    if (profilo.ToLower().Contains("xvid"))
                    {
                        Bitrate = 32 * Convert.ToInt32(audio.Canali);
                        Q = Double.NaN;
                        CODEC = "libmp3lame";
                        if (audio.Canali >= 2)
                            Canali = 2;
                        else
                            Canali = audio.Canali;
                    }
                    else
                    {
                        if (profilo.ToLower().Contains("workraw"))
                        {
                            Bitrate = Double.NaN;
                            Q = 0;
                            CODEC = "libvorbis";
                            if (audio.Canali >= 1)
                                Canali = 1;
                            else
                                Canali = audio.Canali;
                        }
                        else
                        {
                            if (profilo.ToLower().Contains("ac3"))
                            {
                                Bitrate = 32 * Convert.ToInt32(audio.Canali);
                                Q = Double.NaN;
                                CODEC = "ac3";
                                Canali = audio.Canali;
                            }
                            else
                            {

                                if (profilo.ToLower().Contains("streaming"))
                                {
                                    if (audio.Canali >= 2)
                                        Canali = 2;
                                    else
                                        Canali = audio.Canali;
                                }
                                else
                                    Canali = audio.Canali;
                                Bitrate = 32 * Convert.ToInt32(Canali) / 2;
                                Q = Double.NaN;
                                CODEC = "aac";
                            }
                        }
                    }
                    break;
            }
        }
    }
}