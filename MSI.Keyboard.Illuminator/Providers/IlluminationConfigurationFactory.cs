using MSI.Keyboard.Illuminator.Helpers;
using MSI.Keyboard.Illuminator.Models;

using System;

namespace MSI.Keyboard.Illuminator.Providers;

public static class IlluminationConfigurationFactory
{
    public static IlluminationConfiguration GetIlluminationConfiguration(ColorProfile colorProfile)
    {
        ArgumentNullException.ThrowIfNull(colorProfile);

        var configuration = new IlluminationConfigurationBuilder()
            .ForAllRegions(colorProfile.BlinkingMode)
            .ForRegion(Region.Left, colorProfile.LeftColor.ToSystemColor())
            .ForRegion(Region.Center, colorProfile.CenterColor.ToSystemColor())
            .ForRegion(Region.Right, colorProfile.RightColor.ToSystemColor())
            .Build();

        return configuration;
    }

}
