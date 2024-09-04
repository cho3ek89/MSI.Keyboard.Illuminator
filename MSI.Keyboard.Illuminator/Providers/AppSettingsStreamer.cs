using Avalonia.Media;

using MSI.Keyboard.Illuminator.Models;

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MSI.Keyboard.Illuminator.Providers;

public class AppSettingsStreamer(
    string settingsFilePath) : IAppSettingsStreamer
{
    protected readonly string settingsFilePath = settingsFilePath;

    protected readonly Encoding encoding = Encoding.UTF8;

    public AppSettings LoadSettings()
    {
        using var stream = GetStreamReader();

        var xml = XDocument.Load(stream, LoadOptions.None);

        return GetAppSettings(xml.Root);
    }

    public void SaveSettings(AppSettings appSettings)
    {
        var xml = GetXDocument(appSettings);

        using var writer = GetXmlTextWriter();

        xml.Save(writer);
    }

    protected StreamReader GetStreamReader() => new(settingsFilePath, encoding);

    protected XmlTextWriter GetXmlTextWriter() => new(settingsFilePath, encoding)
    {
        Formatting = Formatting.Indented,
    };

    protected static XElement GetXElement(Color color, string name) => new(
        name,
        new XElement(nameof(Color.A), color.A),
        new XElement(nameof(Color.R), color.R),
        new XElement(nameof(Color.G), color.G),
        new XElement(nameof(Color.B), color.B));

    protected static XElement GetXElement(ColorProfile colorProfile, string name = null) => new(
        name ?? nameof(ColorProfile),
        new XElement(nameof(ColorProfile.Name), colorProfile.Name),
        new XElement(nameof(ColorProfile.BlinkingMode), colorProfile.BlinkingMode),
        GetXElement(colorProfile.LeftColor, nameof(colorProfile.LeftColor)),
        GetXElement(colorProfile.CenterColor, nameof(colorProfile.CenterColor)),
        GetXElement(colorProfile.RightColor, nameof(colorProfile.RightColor)));

    protected static XElement GetXElement(AppSettings appSettings)
    {
        var appSettingsElement = new XElement(nameof(AppSettings));

        if (appSettings.ActiveColorProfile != null)
        {
            var activeColorProfileElement = GetXElement(
                appSettings.ActiveColorProfile, 
                nameof(appSettings.ActiveColorProfile));

            appSettingsElement.Add(activeColorProfileElement);
        }

        var colorProfilesElement = new XElement(nameof(AppSettings.ColorProfiles));

        foreach (var colorProfile in appSettings.ColorProfiles)
            colorProfilesElement.Add(GetXElement(colorProfile));

        appSettingsElement.Add(colorProfilesElement);

        return appSettingsElement;
    }

    protected static XDocument GetXDocument(AppSettings appSettings) => new(
        new XDeclaration("1.0", "utf-8", "yes"),
        GetXElement(appSettings));

    protected static Color GetColor(XElement element) => new(
        byte.Parse(element.Element(nameof(Color.A)).Value),
        byte.Parse(element.Element(nameof(Color.R)).Value),
        byte.Parse(element.Element(nameof(Color.G)).Value),
        byte.Parse(element.Element(nameof(Color.B)).Value));

    protected static ColorProfile GetColorProfile(XElement element) => new(
        element.Element(nameof(ColorProfile.Name)).Value,
        Enum.Parse<BlinkingMode>(
            element.Element(nameof(ColorProfile.BlinkingMode)).Value),
        GetColor(element.Element(nameof(ColorProfile.LeftColor))),
        GetColor(element.Element(nameof(ColorProfile.CenterColor))),
        GetColor(element.Element(nameof(ColorProfile.RightColor))));

    protected static AppSettings GetAppSettings(XElement element)
    {
        var appSettings = new AppSettings();

        var activeColorProfileElement = element.Element(nameof(AppSettings.ActiveColorProfile));

        if (activeColorProfileElement != null)
            appSettings.ActiveColorProfile = GetColorProfile(activeColorProfileElement);


        var colorProfilesElements = element
            .Element(nameof(AppSettings.ColorProfiles))
            .Elements();

        foreach (var colorProfileElement in colorProfilesElements)
            appSettings.ColorProfiles.Add(GetColorProfile(colorProfileElement));

        return appSettings;
    }
}
