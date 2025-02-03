using Avalonia.Media;

using MSI.Keyboard.Illuminator.Models;

namespace MSI.Keyboard.Illuminator.Tests.Helpers;

public class TestDataGenerator
{
    public static ColorProfile GetDummyColorProfile(byte i) => new(
        i.ToString(),
        (BlinkingMode)i,
        Color.FromArgb(i, i, i, i),
        Color.FromArgb(i, i, i, i),
        Color.FromArgb(i, i, i, i));
}
