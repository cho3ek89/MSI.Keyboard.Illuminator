using MSI.Keyboard.Illuminator.Models;

using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace MSI.Keyboard.Illuminator.Services;

public interface IKeyboardDevice
{
    /// <summary>
    /// Changes the color of a keyboard for specific region.
    /// </summary>
    /// <param name="region">The region of a keyboard.</param>
    /// <param name="color">The color of a region's buttons.</param>
    /// <exception cref="IOException">Thrown when keyboard device has not been found.</exception>
    Task ChangeColorAsync(Region region, Color color);

    /// <summary>
    /// Checks if the current device is supported.
    /// </summary>
    /// <returns><c>true</c> if a device is supported, otherwise <c>false</c>.</returns>
    bool IsDeviceSupported();

    /// <summary>
    /// Sets the mode of keyboard's leds blinking.
    /// </summary>
    /// <param name="blinkingMode">The specific blinking effect.</param>
    /// <exception cref="IOException">Thrown when keyboard device has not been found.</exception>
    Task ChangeModeAsync(BlinkingMode blinkingMode);
}
