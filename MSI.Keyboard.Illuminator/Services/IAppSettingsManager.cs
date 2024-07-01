using MSI.Keyboard.Illuminator.Models;

using System.Collections.Generic;

namespace MSI.Keyboard.Illuminator.Services;

public interface IAppSettingsManager
{
    ColorProfile GetActiveColorProfile();

    void UpdateActiveColorProfile(ColorProfile colorProfile);

    List<ColorProfile> GetColorProfiles();

    void UpdateColorProfiles(IEnumerable<ColorProfile> colorProfiles);

    void LoadSettings();

    void SaveSettings();
}
