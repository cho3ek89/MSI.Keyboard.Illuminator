using MSI.Keyboard.Illuminator.Models;

using ReactiveUI;

namespace MSI.Keyboard.Illuminator.ViewModels;

public class ColorProfileViewModel(ColorProfile colorProfile) : ReactiveObject
{
    public static BlinkingMode[] BlinkingModes => [ 
        BlinkingMode.Normal, 
        BlinkingMode.Gaming, 
        BlinkingMode.Breathe, 
        BlinkingMode.Demo, 
        BlinkingMode.Wave, ];

    public ColorProfile ColorProfile { get; } = colorProfile;
}
