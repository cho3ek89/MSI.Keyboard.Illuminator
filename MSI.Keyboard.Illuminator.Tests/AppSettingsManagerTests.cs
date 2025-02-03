using Moq;

using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;
using MSI.Keyboard.Illuminator.Services;
using MSI.Keyboard.Illuminator.Tests.Helpers;

using Xunit;

namespace MSI.Keyboard.Illuminator.Tests;

public class AppSettingsManagerTests
{
    [Fact]
    public void GetActiveColorProfileCreatesDeepCopy()
    {
        AppSettings savedAppSettings = null;
        var manager = GetAppSettingsManager(result => savedAppSettings = result);

        var colorProfile = manager.GetActiveColorProfile();

        manager.SaveSettings();

        Assert.False(ReferenceEquals(colorProfile, savedAppSettings.ActiveColorProfile));
    }

    [Fact]
    public void GetColorProfilesCreatesDeepCopy()
    {
        AppSettings savedAppSettings = null;
        var manager = GetAppSettingsManager(result => savedAppSettings = result);

        var colorProfiles = manager.GetColorProfiles();

        manager.SaveSettings();

        Assert.Equal(colorProfiles.Count, savedAppSettings.ColorProfiles.Count);

        for (var i = 0; i < colorProfiles.Count; i++)
        {
            Assert.False(ReferenceEquals(colorProfiles[i], savedAppSettings.ColorProfiles[i]));
        }
    }

    [Fact]
    public void UpdateActiveColorProfileCreatesDeepCopy()
    {
        AppSettings savedAppSettings = null;
        var manager = GetAppSettingsManager(result => savedAppSettings = result);

        var colorProfile = TestDataGenerator.GetDummyColorProfile(3);

        manager.UpdateActiveColorProfile(colorProfile);

        manager.SaveSettings();

        Assert.False(ReferenceEquals(colorProfile, savedAppSettings.ActiveColorProfile));
    }

    [Fact]
    public void UpdateColorProfilesCreatesDeepCopy()
    {
        AppSettings savedAppSettings = null;
        var manager = GetAppSettingsManager(result => savedAppSettings = result);

        var colorProfiles = new List<ColorProfile> 
        {
            TestDataGenerator.GetDummyColorProfile(3), 
            TestDataGenerator.GetDummyColorProfile(4) 
        };

        manager.UpdateColorProfiles(colorProfiles);

        manager.SaveSettings();

        Assert.Equal(colorProfiles.Count, savedAppSettings.ColorProfiles.Count);

        for (var i = 0; i < colorProfiles.Count; i++)
        {
            Assert.False(ReferenceEquals(colorProfiles[i], savedAppSettings.ColorProfiles[i]));
        }
    }

    private static AppSettingsManager GetAppSettingsManager(Action<AppSettings> saveSettingsCallback)
    {
        var manager = new AppSettingsManager(GetAppSettingsStreamer(saveSettingsCallback));
        manager.LoadSettings();

        return manager;
    }

    private static IAppSettingsStreamer GetAppSettingsStreamer(Action<AppSettings> saveSettingsCallback)
    {
        var appSettingsStreamerMock = new Mock<IAppSettingsStreamer>();
        appSettingsStreamerMock.Setup(s => s.LoadSettings()).Returns(new AppSettings()
        {
            ActiveColorProfile = TestDataGenerator.GetDummyColorProfile(0),
            ColorProfiles =
            [
                TestDataGenerator.GetDummyColorProfile(0),
                TestDataGenerator.GetDummyColorProfile(1),
                TestDataGenerator.GetDummyColorProfile(2),
            ],
        });

        appSettingsStreamerMock.Setup(s => s.SaveSettings(It.IsAny<AppSettings>()))
            .Callback(saveSettingsCallback);

        return appSettingsStreamerMock.Object;
    }
}
