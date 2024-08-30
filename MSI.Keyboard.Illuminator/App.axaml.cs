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

            var colorProfilesViewModel = new ColorProfilesViewModel(appSettingsManager);

            DataContext = new TrayViewModel(
                application, 
                keyboardService, 
                appSettingsManager, 
                colorProfilesViewModel);
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
}