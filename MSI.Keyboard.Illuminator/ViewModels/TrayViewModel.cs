using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

using MSI.Keyboard.Illuminator.Helpers;
using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;
using MSI.Keyboard.Illuminator.Services;
using MSI.Keyboard.Illuminator.Views;

using ReactiveUI;

using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace MSI.Keyboard.Illuminator.ViewModels;

public class TrayViewModel : ReactiveObject
{
    protected readonly IClassicDesktopStyleApplicationLifetime application;

    protected readonly IKeyboardService keyboardService;

    protected readonly IAppSettingsManager appSettingsManager;

    public ColorProfilesViewModel ColorProfilesViewModel { get; }
    

    protected NativeMenu trayMenu;

    public NativeMenu TrayMenu
    {
        get => trayMenu;
        set => this.RaiseAndSetIfChanged(ref trayMenu, value);
    }
    
    public ReactiveCommand<ColorProfile, Unit> SelectColorProfile { get; }

    public ReactiveCommand<Unit, Unit> ShowColorProfiles { get; }

    public ReactiveCommand<Unit, Unit> Exit { get; }

    public TrayViewModel(
        IClassicDesktopStyleApplicationLifetime application, 
        IKeyboardService keyboardService, 
        IAppSettingsManager appSettingsManager, 
        ColorProfilesViewModel colorProfilesViewModel)
    {
        ColorProfilesViewModel = colorProfilesViewModel;
        this.application = application;
        this.keyboardService = keyboardService;
        this.appSettingsManager = appSettingsManager;

        InitializeSettings();

        SelectColorProfile = ReactiveCommand.CreateFromTask<ColorProfile>(async colorProfile =>
        {
            var configuration = IlluminationConfigurationFactory
                .GetIlluminationConfiguration(colorProfile);

            await keyboardService.ApplyConfigurationAsync(configuration);
            appSettingsManager.UpdateActiveColorProfile(colorProfile);

            SelectColorProfileOnTrayMenu(colorProfile);
        });

        SelectColorProfile.ThrownExceptions.Subscribe(ex =>
        {
            WindowHelper.ShowMessageWindow(
                Resources.Resources.ColorChangeErrorTitle,
                Resources.Resources.ColorChangeErrorMessage);
        });

        ShowColorProfiles = ReactiveCommand.Create(() =>
        {
            if (this.application.Windows.Any(w => w is ColorProfilesWindow))
                return;

            ColorProfilesViewModel.LoadColorProfiles();
            WindowHelper.ShowColorProfilesWindow(ColorProfilesViewModel);
        });

        Exit = ReactiveCommand.Create(() => FinalizeSettingsAndExit());

        ColorProfilesViewModel.Save.Subscribe(x => 
        {
            GenerateTrayMenu();
            SelectColorProfileOnTrayMenu(appSettingsManager.GetActiveColorProfile());

            CloseAllColorProfilesWindows();
        });

        ColorProfilesViewModel.Cancel.Subscribe(x => 
            CloseAllColorProfilesWindows());

        GenerateTrayMenu();

        var activeColorProfile = appSettingsManager.GetActiveColorProfile();
        if (activeColorProfile != null)
            SelectColorProfile.Execute(activeColorProfile).Subscribe(_ => { }, _ => { });
    }

    /// <summary>
    /// Generates tray menu.
    /// </summary>
    protected void GenerateTrayMenu()
    {
        var newTrayMenu = new NativeMenu();

        foreach (var colorProfile in appSettingsManager.GetColorProfiles())
        {
            newTrayMenu.Add(new NativeMenuItem() 
            { 
                Header = colorProfile.Name,
                Command = SelectColorProfile,
                CommandParameter = colorProfile,
            });
        }

        if (newTrayMenu.Items.Any())
            newTrayMenu.Add(new NativeMenuItemSeparator());

        newTrayMenu.Add(new NativeMenuItem() 
        {
            Header = Resources.Resources.ColorProfilesButtonText, 
            Command = ShowColorProfiles,
            Icon = AssetsHelper.GetImageFromAssets("palette16.png"), 
        });
        newTrayMenu.Add(new NativeMenuItemSeparator());
        newTrayMenu.Add(new NativeMenuItem() 
        {
            Header = Resources.Resources.ExitButtonText, 
            Command = Exit, 
            Icon = AssetsHelper.GetImageFromAssets("exit16.png"), 
        });

        TrayMenu = newTrayMenu;
    }

    /// <summary>
    /// Selects a given <paramref name="colorProfile"/> on tray menu, if it is present there.
    /// </summary>
    /// <param name="colorProfile">A color profile to be selected.</param>
    protected void SelectColorProfileOnTrayMenu(ColorProfile colorProfile)
    {
        foreach (var item in TrayMenu.Items.SkipLast(4).Cast<NativeMenuItem>())
        {
            if (item == null) continue;

            item.Icon = item.CommandParameter as ColorProfile == colorProfile
                ? AssetsHelper.GetImageFromAssets("checked-mark16.png")
                : null;
        }
    }

    /// <summary>
    /// Closes all <see cref="ColorProfilesWindow"/> windows.
    /// </summary>
    protected void CloseAllColorProfilesWindows()
    {
        for (int i = application.Windows.Count - 1; i >= 0; i--)
        {
            var colorProfilesWindow = application.Windows[i] as ColorProfilesWindow;
            colorProfilesWindow?.Close();
        }
    }

    /// <summary>
    /// Initializes an application settings loading, shows error in case of any issues.
    /// </summary>
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
                Resources.Resources.AppSettingsLoadingErrorTitle,
                Resources.Resources.AppSettingsLoadingErrorMessage);
        }
    }

    /// <summary>
    /// Initializes an application settings saving and shutdown, shows error in case of any issues.
    /// </summary>
    protected void FinalizeSettingsAndExit()
    {
        try
        {
            appSettingsManager.SaveSettings();
            application.Shutdown(0);
        }
        catch (Exception)
        {
            // in case of an issue show error window and postpone
            // the shutdown until user closes that window

            var saveSettingsErrorWindow  = WindowHelper.GetMessageWindow(
                Resources.Resources.AppSettingsSavingErrorTitle,
                Resources.Resources.AppSettingsSavingErrorMessage);

            saveSettingsErrorWindow.Closed += (_, _) => application.Shutdown(0);
            saveSettingsErrorWindow.Show();

            var emptyTrayMenu = new NativeMenu();
            TrayMenu = emptyTrayMenu;
        }
    }
}
