using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace AksharaShift;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Deactivated += MainWindow_Deactivated;
    }

    private void MainWindow_Deactivated(object? sender, EventArgs e)
    {
        // Hide window when user clicks away
        this.Visibility = Visibility.Hidden;
        StatusText.Text = "";
    }

    private void BtnML_Click(object sender, RoutedEventArgs e)
    {
        ConvertAndCopy(ConversionType.ML);
    }

    private void BtnFML_Click(object sender, RoutedEventArgs e)
    {
        ConvertAndCopy(ConversionType.FML);
    }

    private void ConvertAndCopy(ConversionType type)
    {
        string input = InputTextBox.Text;
        if (string.IsNullOrWhiteSpace(input)) return;

        try
        {
            string result = type == ConversionType.ML 
                ? MalayalamTextConverter.ConvertToML(input) 
                : MalayalamTextConverter.ConvertToFML(input);

            System.Windows.Clipboard.SetText(result);
            ShowStatus("Copied to Clipboard!");
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error!";
            System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ShowStatus(string message)
    {
        StatusText.Text = message;
        
        // Simple fade out animation for status text could go here, 
        // but for now just clearing it on next activation or simple timer is enough.
        // We actually clear it on Deactivate.
    }

    public void ToggleVisibility()
    {
        if (this.Visibility == Visibility.Visible)
        {
            this.Visibility = Visibility.Hidden;
        }
        else
        {
            // Position near the system tray (bottom right)
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 10;
            this.Top = desktopWorkingArea.Bottom - this.Height - 10;
            
            this.Visibility = Visibility.Visible;
            this.Activate();
            InputTextBox.Focus();
            
            // Auto-paste if clipboard has text? Optional but nice.
            // For now let's just clear status
            StatusText.Text = "";
        }
    }
}