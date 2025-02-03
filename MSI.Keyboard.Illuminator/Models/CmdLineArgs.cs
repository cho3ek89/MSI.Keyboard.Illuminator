using Avalonia.Platform;

namespace MSI.Keyboard.Illuminator.Models;

public class CmdLineArgs(
    string settingsFilePath,
    PlatformThemeVariant theme)
{
    public string SettingsFilePath { get; } = settingsFilePath;

    public PlatformThemeVariant Theme { get; } = theme;

    public static CmdLineArgs GetDefault() => new(
        "appsettings.xml",
        PlatformThemeVariant.Light);
}
