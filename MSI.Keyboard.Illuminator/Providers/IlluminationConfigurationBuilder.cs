using MSI.Keyboard.Illuminator.Models;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MSI.Keyboard.Illuminator.Providers;

public class IlluminationConfigurationBuilder : IIlluminationConfigurationBuilder
{
    private readonly Dictionary<Region, Color> regionColors;

    private BlinkingMode blinkingMode;

    public IlluminationConfigurationBuilder()
    {
        regionColors = new Dictionary<Region, Color>()
        {
            { Region.Left, Color.Red },
            { Region.Center, Color.Red },
            { Region.Right, Color.Red },
        };

        blinkingMode = BlinkingMode.Normal;
    }

    public IIlluminationConfigurationBuilder ForAllRegions(BlinkingMode blinkingMode)
    {
        this.blinkingMode = blinkingMode;

        return this;
    }

    public IIlluminationConfigurationBuilder ForAllRegions(Color color)
    {
        foreach (var region in regionColors.Keys.ToList())
        {
            ForRegion(region, color);
        }

        return this;
    }

    public IIlluminationConfigurationBuilder ForRegion(Region region, Color color)
    {
        regionColors[region] = color;

        return this;
    }

    public IlluminationConfiguration Build() => new(regionColors, blinkingMode);
}