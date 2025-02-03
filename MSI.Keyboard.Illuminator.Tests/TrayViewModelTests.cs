using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless.XUnit;

using Microsoft.Reactive.Testing;

using Moq;

using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Services;
using MSI.Keyboard.Illuminator.Tests.Helpers;
using MSI.Keyboard.Illuminator.ViewModels;

using Xunit;

namespace MSI.Keyboard.Illuminator.Tests;

public class TrayViewModelTests
{
    private readonly TestScheduler scheduler;

    private readonly TestSchedulerProvider schedulerProvider;

    public TrayViewModelTests()
    {
        scheduler = new();
        schedulerProvider = new TestSchedulerProvider(scheduler);
    }

    [AvaloniaFact]
    public void ExitClearsTrayMenuItemsAndTriesToShutDown()
    {
        var desktopMock = new Mock<IClassicDesktopStyleApplicationLifetime>();
        desktopMock.Setup(s => s.TryShutdown(It.IsAny<int>()))
            .Returns(true)
            .Verifiable(Times.Once);

        var appSettingsManagerMock = new Mock<IAppSettingsManager>();
        appSettingsManagerMock.Setup(s => s.GetColorProfiles())
            .Returns([]);

        var trayViewModel = new TrayViewModel(
            desktopMock.Object,
            Mock.Of<IKeyboardService>(),
            appSettingsManagerMock.Object,
            schedulerProvider);

        trayViewModel.Exit.Execute().Subscribe(_ => { }, _ => { });

        desktopMock.Verify();
        Assert.Empty(trayViewModel.TrayMenu.Items);
    }

    [AvaloniaFact]
    public void GenerateTrayMenuBuildsCorrectMenuWithColorProfilesAbsent()
    {
        var appSettingsManagerMock = new Mock<IAppSettingsManager>();
        appSettingsManagerMock.Setup(s => s.GetColorProfiles())
            .Returns([]);

        var trayViewModel = new TrayViewModel(
            Mock.Of<IClassicDesktopStyleApplicationLifetime>(),
            Mock.Of<IKeyboardService>(),
            appSettingsManagerMock.Object,
            schedulerProvider);

        var tmItems = trayViewModel.TrayMenu.Items;

        Assert.Equal(3, tmItems.Count);

        var tmItem0 = tmItems[0] as NativeMenuItem;
        var tmItem1 = tmItems[1] as NativeMenuItemSeparator;
        var tmItem2 = tmItems[2] as NativeMenuItem;

        Assert.NotEmpty(tmItem0.Header);
        Assert.True(ReferenceEquals(trayViewModel.ShowColorProfiles, tmItem0.Command));
        Assert.Null(tmItem0.CommandParameter);
        Assert.NotNull(tmItem0.Icon);

        Assert.NotNull(tmItem1);

        Assert.NotEmpty(tmItem2.Header);
        Assert.True(ReferenceEquals(trayViewModel.Exit, tmItem2.Command));
        Assert.Null(tmItem2.CommandParameter);
        Assert.NotNull(tmItem2.Icon);
    }

    [AvaloniaFact]
    public void GenerateTrayMenuBuildsCorrectMenuWithColorProfilesPresent()
    {
        var colorProfiles = new List<ColorProfile>
        {
            TestDataGenerator.GetDummyColorProfile(0),
            TestDataGenerator.GetDummyColorProfile(1),
            TestDataGenerator.GetDummyColorProfile(2),
        };

        var appSettingsManagerMock = new Mock<IAppSettingsManager>();
        appSettingsManagerMock.Setup(s => s.GetColorProfiles())
            .Returns(colorProfiles);

        var trayViewModel = new TrayViewModel(
            Mock.Of<IClassicDesktopStyleApplicationLifetime>(),
            Mock.Of<IKeyboardService>(),
            appSettingsManagerMock.Object,
            schedulerProvider);

        var tmItems = trayViewModel.TrayMenu.Items;

        Assert.Equal(7, tmItems.Count);

        var tmItem0 = tmItems[0] as NativeMenuItem;
        var tmItem1 = tmItems[1] as NativeMenuItem;
        var tmItem2 = tmItems[2] as NativeMenuItem;
        var tmItem3 = tmItems[3] as NativeMenuItemSeparator;
        var tmItem4 = tmItems[4] as NativeMenuItem;
        var tmItem5 = tmItems[5] as NativeMenuItemSeparator;
        var tmItem6 = tmItems[6] as NativeMenuItem;

        Assert.Equal(colorProfiles[0].Name, tmItem0.Header);
        Assert.True(ReferenceEquals(trayViewModel.SelectColorProfile, tmItem0.Command));
        Assert.True(ReferenceEquals(colorProfiles[0], tmItem0.CommandParameter));
        Assert.Null(tmItem0.Icon);

        Assert.Equal(colorProfiles[1].Name, tmItem1.Header);
        Assert.True(ReferenceEquals(trayViewModel.SelectColorProfile, tmItem1.Command));
        Assert.True(ReferenceEquals(colorProfiles[1], tmItem1.CommandParameter));
        Assert.Null(tmItem1.Icon);

        Assert.Equal(colorProfiles[2].Name, tmItem2.Header);
        Assert.True(ReferenceEquals(trayViewModel.SelectColorProfile, tmItem2.Command));
        Assert.True(ReferenceEquals(colorProfiles[2], tmItem2.CommandParameter));
        Assert.Null(tmItem2.Icon);

        Assert.NotNull(tmItem3);

        Assert.NotEmpty(tmItem4.Header);
        Assert.True(ReferenceEquals(trayViewModel.ShowColorProfiles, tmItem4.Command));
        Assert.Null(tmItem4.CommandParameter);
        Assert.NotNull(tmItem4.Icon);

        Assert.NotNull(tmItem5);

        Assert.NotEmpty(tmItem6.Header);
        Assert.True(ReferenceEquals(trayViewModel.Exit, tmItem6.Command));
        Assert.Null(tmItem6.CommandParameter);
        Assert.NotNull(tmItem6.Icon);
    }

    [AvaloniaFact]
    public void SelectColorProfileChangesKeyboardColorProfile()
    {
        var keyboardService = new Mock<IKeyboardService>();
        keyboardService.Setup(s => s.ApplyConfigurationAsync(It.IsAny<IlluminationConfiguration>()))
            .Verifiable(Times.Once);

        var appSettingsManagerMock = new Mock<IAppSettingsManager>();
        appSettingsManagerMock.Setup(s => s.UpdateActiveColorProfile(It.IsAny<ColorProfile>()))
            .Verifiable(Times.Once);
        appSettingsManagerMock.Setup(s => s.GetColorProfiles())
            .Returns([]);

        var trayViewModel = new TrayViewModel(
            Mock.Of<IClassicDesktopStyleApplicationLifetime>(),
            keyboardService.Object,
            appSettingsManagerMock.Object,
            schedulerProvider);

        trayViewModel.SelectColorProfile
            .Execute(TestDataGenerator.GetDummyColorProfile(0))
            .Subscribe(_ => { }, _ => { });

        keyboardService.Verify();
        appSettingsManagerMock.Verify();
    }

    [AvaloniaTheory]
    [InlineData(0, true, false)]
    [InlineData(1, false, true)]
    [InlineData(2, false, false)]
    public void SelectColorProfileOnTrayMenuSetsIconOnSelectedColorProfile(
        byte activeCpId, bool shouldCp1HaveIcon, bool shouldCp2HaveIcon)
    {
        var colorProfiles = new List<ColorProfile>
        {
            TestDataGenerator.GetDummyColorProfile(0),
            TestDataGenerator.GetDummyColorProfile(1),
        };

        var appSettingsManagerMock = new Mock<IAppSettingsManager>();

        appSettingsManagerMock.Setup(s => s.GetActiveColorProfile())
            .Returns(TestDataGenerator.GetDummyColorProfile(activeCpId));

        appSettingsManagerMock.Setup(s => s.GetColorProfiles())
            .Returns([
                TestDataGenerator.GetDummyColorProfile(0),
                TestDataGenerator.GetDummyColorProfile(1)]);

        var trayViewModel = new TrayViewModel(
            Mock.Of<IClassicDesktopStyleApplicationLifetime>(),
            Mock.Of<IKeyboardService>(),
            appSettingsManagerMock.Object,
            schedulerProvider);

        var tmItems = trayViewModel.TrayMenu.Items;
        var tmItem0 = tmItems[0] as NativeMenuItem;
        var tmItem1 = tmItems[1] as NativeMenuItem;

        Assert.Equal(shouldCp1HaveIcon, tmItem0.Icon != null);
        Assert.Equal(shouldCp2HaveIcon, tmItem1.Icon != null);
    }
}
