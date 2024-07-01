using MSI.Keyboard.Illuminator.Models;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MSI.Keyboard.Illuminator.Providers;

public interface IAppSettingsStreamer
{
    public FileInfo GetSourceFile();

    AppSettings LoadSettings();

    Task<AppSettings> LoadSettingsAsync();

    Task<AppSettings> LoadSettingsAsync(CancellationToken cancellationToken);

    void SaveSettings(AppSettings appSettings);

    Task SaveSettingsAsync(AppSettings appSettings);

    Task SaveSettingsAsync(AppSettings appSettings, CancellationToken cancellationToken);
}
