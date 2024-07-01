using Avalonia.Media.Imaging;
using Avalonia.Platform;

using System;
using System.IO;

namespace MSI.Keyboard.Illuminator.Helpers;

public static class AssetsHelper
{
    public static Bitmap GetImageFromAssets(string pathToImage)
    {
        var basePath = @"avares://MSI.Keyboard.Illuminator/Assets";
        var fullPath = Path.Combine(basePath, pathToImage);
        var uri = new Uri(fullPath);

        using var assetStream = AssetLoader.Open(uri);

        return new(assetStream);
    }
}
