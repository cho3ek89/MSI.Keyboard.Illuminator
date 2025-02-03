using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;

using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace MSI.Keyboard.Illuminator.Services;

public class AppSettingsManager(
    IAppSettingsStreamer appSettingsStreamer) : IAppSettingsManager
{
    protected IAppSettingsStreamer appSettingsStreamer = appSettingsStreamer;

    protected AppSettings appSettings = new();

    public ColorProfile GetActiveColorProfile() =>
        appSettings.ActiveColorProfile?.Clone() as ColorProfile;

    public void UpdateActiveColorProfile(ColorProfile colorProfile) =>
        appSettings.ActiveColorProfile = colorProfile?.Clone() as ColorProfile;

    public List<ColorProfile> GetColorProfiles() =>
        GetDeepCopy(appSettings.ColorProfiles);

    public void UpdateColorProfiles(IEnumerable<ColorProfile> colorProfiles) =>
        appSettings.ColorProfiles = GetDeepCopy(colorProfiles);

    public void LoadSettings() =>
        appSettings = appSettingsStreamer.LoadSettings();

    public void SaveSettings() =>
        appSettingsStreamer.SaveSettings(appSettings);

    protected static List<ColorProfile> GetDeepCopy(IEnumerable<ColorProfile> colorProfiles) =>
        colorProfiles.Select(s => s?.Clone() as ColorProfile).ToList();
}
