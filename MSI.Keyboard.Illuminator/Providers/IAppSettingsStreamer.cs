using MSI.Keyboard.Illuminator.Models;

namespace MSI.Keyboard.Illuminator.Providers;

public interface IAppSettingsStreamer
{
    AppSettings LoadSettings();

    void SaveSettings(AppSettings appSettings);
}
