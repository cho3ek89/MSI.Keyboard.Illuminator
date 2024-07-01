using System.Collections.Generic;

namespace MSI.Keyboard.Illuminator.Models;

public class AppSettings
{
    public ColorProfile ActiveColorProfile { get; set; }

    public List<ColorProfile> ColorProfiles { get; set; } = [];
}
