using MSI.Keyboard.Illuminator.Models;

using System;
using System.Threading.Tasks;

namespace MSI.Keyboard.Illuminator.Services;

public interface IKeyboardService
{
    /// <summary>
    /// Gets the current illumination configuration.
    /// </summary>
    /// <returns>An instance of <see cref="IlluminationConfiguration"/> object.</returns>
    IlluminationConfiguration GetCurrentConfiguration();

    /// <summary>
    /// Applies an illumination configuration.
    /// </summary>
    /// <param name="configuration">An illumination configuration.</param>
    /// <exception cref="InvalidOperationException">Thrown when keyboard device not found.</exception>
    /// <exception cref="InvalidProgramException">If unrecognized IO exception occured.</exception>
    Task ApplyConfigurationAsync(IlluminationConfiguration configuration);

    /// <summary>
    /// Checks if the current device is supported.
    /// </summary>
    /// <returns><c>true</c> if a device is supported, otherwise <c>false</c>.</returns>
    bool IsDeviceSupported();
}
