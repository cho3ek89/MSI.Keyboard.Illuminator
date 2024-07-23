using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using MSI.Keyboard.Illuminator.Helpers;
using MSI.Keyboard.Illuminator.Providers;
using MSI.Keyboard.Illuminator.Services;
using MSI.Keyboard.Illuminator.ViewModels;

using System;
using System.CommandLine;
using System.IO;

namespace MSI.Keyboard.Illuminator;

public partial class App : Application
{
    protected IAppSettingsManager appSettingsManager;

    protected IKeyboardService keyboardService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime application)
        {
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            keyboardService = GetKeyboardService();
            WarnIfDeviceIsNotSupported();

            appSettingsManager = GetAppSettingsManager(application.Args);
            InitializeSettings();
            application.Exit += (s, e) => FinalizeSettings();

            var colorProfilesViewModel = new ColorProfilesViewModel(appSettingsManager);

            DataContext = new TrayViewModel(
                application, 
                colorProfilesViewModel, 
                keyboardService,
                appSettingsManager);
        }

        base.OnFrameworkInitializationCompleted();
    }
    protected static IKeyboardService GetKeyboardService()
    {
        var keyboardDevice = new KeyboardDevice();

        return new KeyboardService(keyboardDevice);
    }

    protected void WarnIfDeviceIsNotSupported()
    {
        if (keyboardService.IsDeviceSupported())
            return;

        WindowHelper.ShowMessageWindow(
            Illuminator.Resources.Resources.DeviceNotFoundErrorTitle,
            Illuminator.Resources.Resources.DeviceNotFoundErrorMessage);
    }

    protected static IAppSettingsManager GetAppSettingsManager(params string[] args)
    {
        var fileOption = new Option<FileInfo>(
            name: "--settings",
            getDefaultValue: () => new FileInfo("appsettings.xml"), 
            description: Illuminator.Resources.Resources.SettingsParameterDescription);

        var settingsFile = fileOption.Parse(args).GetValueForOption(fileOption);

        var appSettingsStreamer = new AppSettingsStreamer(settingsFile);

        return new AppSettingsManager(appSettingsStreamer);
    }

    protected void InitializeSettings()
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

    protected void FinalizeSettings()
    {
        try
        {
            appSettingsManager.SaveSettings();
        }
        catch (Exception)
        {
            WindowHelper.ShowMessageWindow(
                Illuminator.Resources.Resources.AppSettingsSavingErrorTitle,
                Illuminator.Resources.Resources.AppSettingsSavingErrorMessage);
        }
    }
}