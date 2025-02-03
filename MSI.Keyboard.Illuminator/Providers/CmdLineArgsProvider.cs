using Avalonia.Platform;

using MSI.Keyboard.Illuminator.Models;

using System.CommandLine;

namespace MSI.Keyboard.Illuminator.Providers;

public class CmdLineArgsProvider : ICmdLineArgsProvider
{
    protected readonly string[] args;

    protected readonly Option<string> settingsOption;

    protected readonly Option<PlatformThemeVariant> themeOption;

    protected readonly RootCommand rootCommand;

    public CmdLineArgsProvider(string[] args)
    {
        this.args = args;

        settingsOption = new Option<string>(
            name: "--settings",
            getDefaultValue: () => CmdLineArgs.GetDefault().SettingsFilePath);

        themeOption = new Option<PlatformThemeVariant>(
            name: "--theme",
            getDefaultValue: () => CmdLineArgs.GetDefault().Theme);

        rootCommand = [settingsOption, themeOption];
    }

    public CmdLineArgs GetCmdLineArgs()
    {
        var result = rootCommand.Parse(args);

        var settingsFilePath = result.GetValueForOption(settingsOption);
        var theme = result.GetValueForOption(themeOption);

        return new(settingsFilePath, theme);
    }
}
