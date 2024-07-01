using Avalonia.Controls;

using MSI.Keyboard.Illuminator.ViewModels;
using MSI.Keyboard.Illuminator.Views;

namespace MSI.Keyboard.Illuminator.Helpers;

public static class WindowHelper
{
    public static Window GetColorProfilesWindow(ColorProfilesViewModel colorProfilesViewModel)
    {
        var colorProfilesWindow = new ColorProfilesWindow()
        {
            DataContext = colorProfilesViewModel,
        };

        return colorProfilesWindow;
    }

    public static void ShowColorProfilesWindow(ColorProfilesViewModel colorProfilesViewModel)
    {
        var colorProfilesWindow = GetColorProfilesWindow(colorProfilesViewModel);

        colorProfilesWindow.Show();
    }

    public static Window GetMessageWindow(string title, string message)
    {
        var messageViewModel = new MessageViewModel(title, message);

        var window = new MessageWindow()
        {
            DataContext = messageViewModel,
        };

        return window;
    }

    public static void ShowMessageWindow(string title, string message)
    {
        var messageWindow = GetMessageWindow(title, message);

        messageWindow.Show();
    }
}
