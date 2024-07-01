namespace MSI.Keyboard.Illuminator.Models;

public class AppSettingsStreamerOptions(
    string appSettingsFilePath, 
    bool isXmlIndented = true)
{
    public string AppSettingsFilePath { get; } = appSettingsFilePath;

    public bool IsXmlIndented { get; } = isXmlIndented;
}
