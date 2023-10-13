using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PrecisionGuard
{
    static class Program
    {
        static NotifyIcon notifyIcon;

        [STAThread]
        static void Main()
        {
            // Hide console window
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            // Initialize NotifyIcon
            InitializeNotifyIcon();

            // Start monitoring for game processes
            StartMonitoring();

            // Run application
            Application.Run();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("icon.ico"),
                ContextMenuStrip = new ContextMenuStrip()
            };
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, ExitApp);
            notifyIcon.Visible = true;
        }

        static void ExitApp(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        static void StartMonitoring()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Process[] processes = Process.GetProcesses();
                    bool isGameRunning = processes.Any(p => p.ProcessName.Contains("YourGameExecutable"));

                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Mouse", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("MouseSpeed", isGameRunning ? "0" : "1");
                        }
                    }

                    Thread.Sleep(5000);
                }
            }).Start();
        }
    }
}
