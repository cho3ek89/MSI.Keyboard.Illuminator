using Avalonia.Platform;

using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;

using Xunit;

namespace MSI.Keyboard.Illuminator.Tests;

public class CmdLineArgsProviderTests
{
    [Theory]
    [ClassData(typeof(GetCmdLineArgsTestData))]
    public void GetCmdLineArgsParsesCommandLineArgumentsCorrectly(
        string[] args, CmdLineArgs expectedArgsModel)
    {
        var provider = new CmdLineArgsProvider(args);
        var actualArgsModel = provider.GetCmdLineArgs();

        Assert.Equal(expectedArgsModel.SettingsFilePath, actualArgsModel.SettingsFilePath);
        Assert.Equal(expectedArgsModel.Theme, actualArgsModel.Theme);
    }

    public class GetCmdLineArgsTestData : TheoryData<string[], CmdLineArgs>
    {
        public GetCmdLineArgsTestData()
        {
            Add(["--settings", "settings.xml", "--theme", "Dark"], new("settings.xml", PlatformThemeVariant.Dark));
            Add(["--settings", "settings.xml"], new("settings.xml", PlatformThemeVariant.Light));
            Add(["--theme", "Dark"], new("appsettings.xml", PlatformThemeVariant.Dark));
            Add([], new("appsettings.xml", PlatformThemeVariant.Light));
        }
    }
}
