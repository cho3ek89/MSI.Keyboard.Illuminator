using MSI.Keyboard.Illuminator.Models;

namespace MSI.Keyboard.Illuminator.Providers;

public interface ICmdLineArgsProvider
{
    CmdLineArgs GetCmdLineArgs();
}
