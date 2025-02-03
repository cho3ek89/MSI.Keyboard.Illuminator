using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

using MSI.Keyboard.Illuminator.Helpers;
using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;
using MSI.Keyboard.Illuminator.Services;
using MSI.Keyboard.Illuminator.ViewModels;

using System;
using System.IO;

namespace MSI.Keyboard.Illuminator;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime application)
        {
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var keyboardService = GetKeyboardService();
            WarnIfDeviceIsNotSupported(keyboardService);

            var cmdLineArgs = GetCmdLineArgs(application);

            RequestedThemeVariant = (ThemeVariant)cmdLineArgs.Theme;

            var appSettingsManager = GetAppSettingsManager(cmdLineArgs);
            InitializeSettings(appSettingsManager);

            application.ShutdownRequested += (_, shutdownEventArgs) => 
                FinalizeSettingsAndExit(appSettingsManager, application, shutdownEventArgs);

            var schedulerProvider = new SchedulerProvider();

            DataContext = new TrayViewModel(
                application, 
                keyboardService, 
                appSettingsManager,
                schedulerProvider);
        }

        base.OnFrameworkInitializationCompleted();
    }

    protected static CmdLineArgs GetCmdLineArgs(IClassicDesktopStyleApplicationLifetime application)
    {
        try
        {
            var cmdLineArgsProvider = new CmdLineArgsProvider(application.Args);

            return cmdLineArgsProvider.GetCmdLineArgs();
        }
        catch (Exception)
        {
            WindowHelper.ShowMessageWindow(
                Illuminator.Resources.Resources.CmdLineArgsParsingErrorTitle,
                Illuminator.Resources.Resources.CmdLineArgsParsingErrorMessage);

            return CmdLineArgs.GetDefault();
        }
    }

    protected static IKeyboardService GetKeyboardService()
    {
        var keyboardDevice = new KeyboardDevice();

        return new KeyboardService(keyboardDevice);
    }

    protected static void WarnIfDeviceIsNotSupported(IKeyboardService keyboardService)
    {
        if (keyboardService.IsDeviceSupported())
            return;

        WindowHelper.ShowMessageWindow(
            Illuminator.Resources.Resources.DeviceNotFoundErrorTitle,
            Illuminator.Resources.Resources.DeviceNotFoundErrorMessage);
    }

    protected static IAppSettingsManager GetAppSettingsManager(CmdLineArgs cmdLineArgs)
    {
        var appSettingsStreamer = new AppSettingsStreamer(cmdLineArgs.SettingsFilePath);

        return new AppSettingsManager(appSettingsStreamer);
    }

    /// <summary>
    /// Initializes an application settings loading, shows error in case of any issues.
    /// </summary>
    protected static void InitializeSettings(IAppSettingsManager appSettingsManager)
    {
        try
        {
            appSettingsManager.LoadSettings();
        }
        catch (FileNotFoundException)
        {
            // supress and use default settings
        }
        catch (Exception)
        {
            WindowHelper.ShowMessageWindow(
                Illuminator.Resources.Resources.AppSettingsLoadingErrorTitle,
                Illuminator.Resources.Resources.AppSettingsLoadingErrorMessage);
        }
    }

    /// <summary>
    /// Initializes an application settings saving and shutdown, shows error in case of any issues.
    /// </summary>
    protected static void FinalizeSettingsAndExit(
        IAppSettingsManager appSettingsManager, 
        IClassicDesktopStyleApplicationLifetime application, 
        ShutdownRequestedEventArgs shutdownEventArgs)
    {
        try
        {
            appSettingsManager.SaveSettings();
        }
        catch (Exception)
        {
            // in case of an issue show error window and postpone
            // the shutdown until user closes that window

            shutdownEventArgs.Cancel = true;

            var saveSettingsErrorWindow = WindowHelper.GetMessageWindow(
                Illuminator.Resources.Resources.AppSettingsSavingErrorTitle,
                Illuminator.Resources.Resources.AppSettingsSavingErrorMessage);

            saveSettingsErrorWindow.Closed += (_, _) => application.Shutdown(0);
            saveSettingsErrorWindow.Show();
        }
    }
}