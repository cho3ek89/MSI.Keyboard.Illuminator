using System.Collections.Generic;
using System.Drawing;

namespace MSI.Keyboard.Illuminator.Models;

public class IlluminationConfiguration(
    IReadOnlyDictionary<Region, Color> regionColors,
    BlinkingMode blinkingMode)
{
    public IReadOnlyDictionary<Region, Color> RegionColors { get; } = regionColors;

    public BlinkingMode BlinkingMode { get; } = blinkingMode;
}
