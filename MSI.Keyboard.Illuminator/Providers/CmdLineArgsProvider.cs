using MSI.Keyboard.Illuminator.Models;

using System.CommandLine;

namespace MSI.Keyboard.Illuminator.Providers
{
    public class CmdLineArgsProvider : ICmdLineArgsProvider
    {
        protected readonly string[] args;

        protected readonly Option<string> settingsOption;

        protected readonly RootCommand rootCommand;

        public CmdLineArgsProvider(string[] args)
        {
            this.args = args;

            settingsOption = new Option<string>(
                name: "--settings",
                getDefaultValue: () => CmdLineArgs.GetDefault().SettingsFilePath);

            rootCommand = [settingsOption];
        }

        public CmdLineArgs GetCmdLineArgs()
        {
            var result = rootCommand.Parse(args);

            var settingsFilePath = result.GetValueForOption(settingsOption);

            return new(settingsFilePath);
        }
    }
}
