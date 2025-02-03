using Avalonia.Media;

using ReactiveUI;

using System;

namespace MSI.Keyboard.Illuminator.Models;

public class ColorProfile : ReactiveObject, ICloneable
{
    protected string name;

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    protected BlinkingMode blinkingMode;

    public BlinkingMode BlinkingMode
    {
        get => blinkingMode;
        set => this.RaiseAndSetIfChanged(ref blinkingMode, value);
    }

    protected Color leftColor;

    public Color LeftColor
    {
        get => leftColor;
        set => this.RaiseAndSetIfChanged(ref leftColor, value);
    }

    protected Color centerColor;

    public Color CenterColor
    {
        get => centerColor;
        set => this.RaiseAndSetIfChanged(ref centerColor, value);
    }

    protected Color rightColor;

    public Color RightColor
    {
        get => rightColor;
        set => this.RaiseAndSetIfChanged(ref rightColor, value);
    }

    public ColorProfile() { }

    public ColorProfile(
        string name,
        BlinkingMode blinkingMode,
        Color leftColor,
        Color centerColor,
        Color rightColor)
    {
        Name = name;
        BlinkingMode = blinkingMode;
        LeftColor = leftColor;
        CenterColor = centerColor;
        RightColor = rightColor;
    }

    public static ColorProfile GetDefault() => new(
        "Default",
        BlinkingMode.Normal,
        new(255, 255, 0, 0),
        new(255, 255, 0, 0),
        new(255, 255, 0, 0)
        );

    public static bool operator ==(ColorProfile left, ColorProfile right)
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(ColorProfile left, ColorProfile right) =>
        !(left == right);

    public object Clone() => new ColorProfile(
        (string)Name.Clone(),
        BlinkingMode,
        LeftColor,
        CenterColor,
        RightColor);

    public override bool Equals(object obj)
    {
        if (obj is not ColorProfile other)
            return false;

        return Name == other.Name
            && BlinkingMode == other.BlinkingMode
            && LeftColor == other.LeftColor
            && CenterColor == other.CenterColor
            && RightColor == other.RightColor;
    }

    public override int GetHashCode() => HashCode.Combine(
        Name,
        BlinkingMode,
        LeftColor,
        CenterColor,
        RightColor);
}
