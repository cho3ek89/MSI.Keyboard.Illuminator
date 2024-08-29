namespace MSI.Keyboard.Illuminator.Models;

public class CmdLineArgs(
    string settingsFilePath)
{
    public string SettingsFilePath { get; } = settingsFilePath;

    public static CmdLineArgs GetDefault() => new(
        "appsettings.xml");
}
