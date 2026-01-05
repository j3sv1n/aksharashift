using System;
using System.Drawing;
using System.Windows;

namespace AksharaShift;

public partial class App : System.Windows.Application
{
    private System.Windows.Forms.NotifyIcon? notifyIcon;
    private MainWindow? mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        try
        {
            // Initialize Main Window (starts hidden)
            mainWindow = new MainWindow();
            
            // Create system tray icon
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Visible = true;
            notifyIcon.Text = "AksharaShift Converter";
            
            // Left click to toggle window
            notifyIcon.MouseClick += (s, args) => 
            {
                if (args.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    mainWindow.ToggleVisibility();
                }
            };

            // Context Menu
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, (s, args) => Shutdown());
            notifyIcon.ContextMenuStrip = contextMenu;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Startup Error: {ex.Message}", "Error");
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        notifyIcon?.Dispose();
        base.OnExit(e);
    }
}

