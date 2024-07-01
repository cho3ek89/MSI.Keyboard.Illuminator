namespace MSI.Keyboard.Illuminator.Helpers;

public static class ColorExtensions
{
    public static Avalonia.Media.Color ToAvaloniaColor(this System.Drawing.Color color) =>
        Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

    public static System.Drawing.Color ToSystemColor(this Avalonia.Media.Color color) =>
        System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
}
