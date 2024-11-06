using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using KioskBrowser.Native;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace KioskBrowser;

public partial class App
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        
        SimpleLogger.LogInfo("Starting Swift Kiosk Browser");

        SimpleLogger.LogInfo("Version: " + AppSettings.Version);
        SimpleLogger.LogInfo("Store link: " + AppSettings.MicrosoftStoreLink);

        var window = new MainWindow();
        window.Show();
    }

    private async void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        
        SimpleLogger.LogError(e.Exception, "An unexpected error occurred.");
        
        var appIcon = new BitmapImage(new Uri("pack://application:,,,/Images/app.png"));
        var errorIcon = new BitmapImage(new Uri("pack://application:,,,/Images/error.png"));

        await Current.Dispatcher.InvokeAsync( async () =>
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Application error",
                Content = new StackPanel
                {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(10,10,10,10),
                        Children =
                        {
                            new Wpf.Ui.Controls.TextBlock()
                            {
                                Text = "An unexpected error occurred.",
                                FontSize = 18,
                                Margin = new Thickness(0, 0, 20, 0)
                            },
                            new HyperlinkButton()
                            {
                                Content = "Open log folder",
                                FontSize = 18,
                                Command = new RelayCommand(() =>
                                {
                                    Process.Start("explorer.exe", SimpleLogger.LogDirectoryPath);
                                })
                            }
                        }
                    },
                ShowInTaskbar = true,
                FontSize = 18,
                Icon = appIcon,
                MinHeight = 300,
                MinWidth = 400,
                TaskbarItemInfo = new TaskbarItemInfo
                {
                    Description = "An unexpected error occurred. Please restart the application.",
                    Overlay = errorIcon,
                    ProgressState = TaskbarItemProgressState.Error,
                    ProgressValue = 1.0
                }
            };
            
            if (Current.MainWindow is { IsLoaded: true })
            {
                messageBox.Owner = Current.MainWindow;
            }
            else
            {
                messageBox.Topmost = true;
            }
            
            messageBox.SourceInitialized += (_, _) =>
                ShellHelper.SetAppUserModelId(new WindowInteropHelper(messageBox).Handle, "SwiftLaunchError");
            
            try
            {
                _ = await messageBox.ShowDialogAsync();
            }
            catch (Exception) // Catch any exception that might occur when showing the message box
            {
                MessageBox.Show(
                    "An unexpected error occurred. Please restart the application.",
                    "Application error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        });
    }
}

