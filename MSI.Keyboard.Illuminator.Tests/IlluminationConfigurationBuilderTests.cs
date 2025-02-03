using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;

using System.Drawing;

using Xunit;

namespace MSI.Keyboard.Illuminator.Tests;

public class IlluminationConfigurationBuilderTests
{
    [Fact]
    public void ForAllRegionsSetsBlinkingModeCorrectly()
    {
        var builder = new IlluminationConfigurationBuilder();
        builder.ForAllRegions(BlinkingMode.Breathe);

        var configuration = builder.Build();

        Assert.Equal(BlinkingMode.Breathe, configuration.BlinkingMode);
    }

    [Fact]
    public void ForAllRegionsSetsColorCorrectlyForAllRegions()
    {
        var builder = new IlluminationConfigurationBuilder();
        builder.ForAllRegions(Color.Red);

        var configuration = builder.Build();
        var regionColors = configuration.RegionColors;

        Assert.Equal(Color.Red, regionColors[Region.Left]);
        Assert.Equal(Color.Red, regionColors[Region.Center]);
        Assert.Equal(Color.Red, regionColors[Region.Right]);
    }

    [Fact]
    public void ForRegionSetsColorCorrectlyForEveryRegion()
    {
        var builder = new IlluminationConfigurationBuilder();
        builder.ForRegion(Region.Left, Color.Red);
        builder.ForRegion(Region.Center, Color.Green);
        builder.ForRegion(Region.Right, Color.Blue);

        var configuration = builder.Build();
        var regionColors = configuration.RegionColors;

        Assert.Equal(Color.Red, regionColors[Region.Left]);
        Assert.Equal(Color.Green, regionColors[Region.Center]);
        Assert.Equal(Color.Blue, regionColors[Region.Right]);
    }
}
