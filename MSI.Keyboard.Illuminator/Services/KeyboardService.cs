using MSI.Keyboard.Illuminator.Models;

using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace MSI.Keyboard.Illuminator.Services;

public class KeyboardService(IKeyboardDevice keyboardDevice) : IKeyboardService
{
    private readonly IKeyboardDevice keyboardDevice = keyboardDevice;

    private IlluminationConfiguration configuration;

    public async Task ApplyConfigurationAsync(IlluminationConfiguration configuration)
    {
        this.configuration = configuration;

        await ApplyBlinkingModeAsync(this.configuration.BlinkingMode);
        await ApplyRegionColors(this.configuration.RegionColors);
    }

    public bool IsDeviceSupported() => keyboardDevice.IsDeviceSupported();

    public IlluminationConfiguration GetCurrentConfiguration() => configuration;

    protected async Task ApplyRegionColors(IReadOnlyDictionary<Region, Color> regionColors)
    {
        foreach (var regionColor in regionColors)
        {
            await keyboardDevice.ChangeColorAsync(
                regionColor.Key,
                regionColor.Value);
        }
    }

    protected async Task ApplyBlinkingModeAsync(BlinkingMode blinkingMode) => 
        await keyboardDevice.ChangeModeAsync(blinkingMode);
}
