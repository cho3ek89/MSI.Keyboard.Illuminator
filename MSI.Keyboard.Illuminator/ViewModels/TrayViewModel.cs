using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

using MSI.Keyboard.Illuminator.Helpers;
using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;
using MSI.Keyboard.Illuminator.Services;
using MSI.Keyboard.Illuminator.Views;

using ReactiveUI;

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace MSI.Keyboard.Illuminator.ViewModels;

public class TrayViewModel : ReactiveObject
{
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
        ColorProfilesViewModel colorProfilesViewModel, 
        IKeyboardService keyboardService,
        IAppSettingsManager appSettingsManager)
    {
        ColorProfilesViewModel = colorProfilesViewModel;
        this.keyboardService = keyboardService;
        this.appSettingsManager = appSettingsManager;

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
            if (application.Windows.Any(w => w is ColorProfilesWindow))
                return;

            WindowHelper.ShowColorProfilesWindow(colorProfilesViewModel);
        });

        Exit = ReactiveCommand.Create(() => application.Shutdown(0));

        ColorProfilesViewModel.Save.Subscribe(x => 
        {
            GenerateTrayMenu();
            SelectColorProfileOnTrayMenu(appSettingsManager.GetActiveColorProfile());

            CloseAllColorProfilesWindows(application);
        });

        ColorProfilesViewModel.Cancel.Subscribe(x => 
            CloseAllColorProfilesWindows(application));

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
    protected static void CloseAllColorProfilesWindows(
        IClassicDesktopStyleApplicationLifetime application)
    {
        for (int i = application.Windows.Count - 1; i >= 0; i--)
        {
            var colorProfilesWindow = application.Windows[i] as ColorProfilesWindow;
            colorProfilesWindow?.Close();
        }
    }
}
