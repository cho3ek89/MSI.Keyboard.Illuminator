using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;
using MSI.Keyboard.Illuminator.Tests.Helpers;

using System.Security.Cryptography;

using Xunit;

namespace MSI.Keyboard.Illuminator.Tests;

public class AppSettingsStreamerTests
{
    [Theory]
    [ClassData(typeof(LoadAndSaveSettingsTestData))]
    public void LoadSettingsReadsAppSettingsFromFileCorrectly(
        AppSettings expectedAppSettings, string actualSettingsFilePath)
    {
        var appSettingsStreamer = new AppSettingsStreamer(actualSettingsFilePath);
        var actualAppSettings = appSettingsStreamer.LoadSettings();

        Assert.Equal(expectedAppSettings, actualAppSettings, AreAppSettingsEqual);
    }

    [Theory]
    [ClassData(typeof(LoadAndSaveSettingsTestData))]
    public void SaveSettingsWritesAppSettingsToFileCorrectly(
        AppSettings actualAppSettings, string expectedSettingsFilePath)
    {
        var actualSettingsFilePath = Path.GetTempFileName();
        var appSettingsStreamer = new AppSettingsStreamer(actualSettingsFilePath);
        appSettingsStreamer.SaveSettings(actualAppSettings);

        var expectedSettingsFileHash = GetHashFromFileContent(expectedSettingsFilePath);
        var actualSettingsFileHash = GetHashFromFileContent(actualSettingsFilePath);

        File.Delete(actualSettingsFilePath);

        Assert.Equal(actualSettingsFileHash, expectedSettingsFileHash);
    }

    public class LoadAndSaveSettingsTestData : TheoryData<AppSettings, string>
    {
        public LoadAndSaveSettingsTestData()
        {
            Add(new AppSettings
            {
                ActiveColorProfile = TestDataGenerator.GetDummyColorProfile(0),
                ColorProfiles =
                [
                    TestDataGenerator.GetDummyColorProfile(0),
                    TestDataGenerator.GetDummyColorProfile(1),
                    TestDataGenerator.GetDummyColorProfile(2)
                ],
            },
            GetSettingsFilePath("full.xml"));

            Add(new AppSettings
            {
                ActiveColorProfile = null,
                ColorProfiles =
                [
                    TestDataGenerator.GetDummyColorProfile(0),
                    TestDataGenerator.GetDummyColorProfile(1),
                    TestDataGenerator.GetDummyColorProfile(2)
                ],
            },
            GetSettingsFilePath("without_active_color_profile.xml"));

            Add(new AppSettings
            {
                ActiveColorProfile = TestDataGenerator.GetDummyColorProfile(0),
                ColorProfiles = [],
            },
            GetSettingsFilePath("without_color_profiles.xml"));

            Add(new AppSettings
            {
                ActiveColorProfile = null,
                ColorProfiles = [],
            },
            GetSettingsFilePath("empty.xml"));
        }

        protected static string GetSettingsFilePath(string settingsFileName) =>
            Path.Combine("TestData", "AppSettingsStreamerTests", settingsFileName);
    }

    private static bool AreAppSettingsEqual(AppSettings left, AppSettings right)
    {
        if (left.ActiveColorProfile != right.ActiveColorProfile)
            return false;

        if (left.ColorProfiles.Count != right.ColorProfiles.Count)
            return false;

        for (var i = 0; i < left.ColorProfiles.Count; i++)
        {
            if (left.ColorProfiles[i] != right.ColorProfiles[i])
                return false;
        }

        return true;
    }

    private static byte[] GetHashFromFileContent(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);

        return md5.ComputeHash(stream);
    }
}
