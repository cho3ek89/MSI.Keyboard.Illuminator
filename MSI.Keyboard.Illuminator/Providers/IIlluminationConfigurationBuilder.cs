using MSI.Keyboard.Illuminator.Models;

using System.Drawing;

namespace MSI.Keyboard.Illuminator.Providers;

public interface IIlluminationConfigurationBuilder
{
    IIlluminationConfigurationBuilder ForAllRegions(BlinkingMode blinkingMode);

    IIlluminationConfigurationBuilder ForAllRegions(Color color);

    IIlluminationConfigurationBuilder ForRegion(Region region, Color color);

    IlluminationConfiguration Build();
}
