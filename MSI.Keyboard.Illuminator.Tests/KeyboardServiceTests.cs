using Moq;

using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Providers;
using MSI.Keyboard.Illuminator.Services;

using System.Drawing;

using Xunit;

namespace MSI.Keyboard.Illuminator.Tests;

public class KeyboardServiceTests
{
    [Fact]
    public async Task ApplyConfigurationAsyncChangesKeyboardColorAndBlinkingMode()
    {
        var keyboardDeviceMock = new Mock<IKeyboardDevice>();

        keyboardDeviceMock.Setup(s => s.ChangeColorAsync(Region.Left, Color.Red))
            .Verifiable(Times.Once);
        keyboardDeviceMock.Setup(s => s.ChangeColorAsync(Region.Center, Color.Green))
            .Verifiable(Times.Once);
        keyboardDeviceMock.Setup(s => s.ChangeColorAsync(Region.Right, Color.Blue))
            .Verifiable(Times.Once);
        keyboardDeviceMock.Setup(s => s.ChangeModeAsync(BlinkingMode.Demo))
            .Verifiable(Times.Once);

        var builder = new IlluminationConfigurationBuilder();
        builder.ForAllRegions(BlinkingMode.Demo);
        builder.ForRegion(Region.Left, Color.Red);
        builder.ForRegion(Region.Center, Color.Green);
        builder.ForRegion(Region.Right, Color.Blue);

        var configuration = builder.Build();

        var keyboardService = new KeyboardService(keyboardDeviceMock.Object);

        await keyboardService.ApplyConfigurationAsync(configuration);

        keyboardDeviceMock.Verify();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IsDeviceSupportedChecksIfSupportedKeyboardIsPresent(bool expectedIsKeyboardPresent)
    {
        var keyboardDeviceMock = new Mock<IKeyboardDevice>();

        keyboardDeviceMock.Setup(s => s.IsDeviceSupported())
            .Returns(expectedIsKeyboardPresent)
            .Verifiable(Times.Once);

        var keyboardService = new KeyboardService(keyboardDeviceMock.Object);

        var actualIsKeyboardPresent = keyboardService.IsDeviceSupported();

        Assert.Equal(expectedIsKeyboardPresent, actualIsKeyboardPresent);

        keyboardDeviceMock.Verify();
    }
}
