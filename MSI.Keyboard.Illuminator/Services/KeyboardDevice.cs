using HidSharp;

using MSI.Keyboard.Illuminator.Models;

using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace MSI.Keyboard.Illuminator.Services;

public class KeyboardDevice : IKeyboardDevice
{
    protected const int VendorId = 6000;

    protected const int ProductId = 65280;

    public async Task ChangeColorAsync(Region region, Color color)
    {
        var intensity = (int)Math.Round(100.0 / 255 * color.A, MidpointRounding.AwayFromZero);

        var colorFragmentValue = new Func<int, byte>(c => (byte)(c * (intensity / 100d)));

        var data = new byte[]
        {
            1,
            2,
            64,
            (byte)region,
            colorFragmentValue(color.R),
            colorFragmentValue(color.G),
            colorFragmentValue(color.B),
            0,
        };

        await SetFeature(data);
    }

    public async Task ChangeModeAsync(BlinkingMode blinkingMode)
    {
        var data = new byte[]
        {
            1,
            2,
            65,
            (byte)blinkingMode,
            0,
            0,
            0,
            236,
        };

        await SetFeature(data);
    }

    public bool IsDeviceSupported() => 
        DeviceList.Local.TryGetHidDevice(out _, VendorId, ProductId);

    protected static Task SetFeature(byte[] data)
    {
        var device = DeviceList.Local
            .GetHidDeviceOrNull(VendorId, ProductId)
            ?? throw new IOException("MSI keyboard has not been found!");

        using var stream = device.Open();
        stream.SetFeature(data);

        return Task.CompletedTask;
    }
}
