using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace MSI.Keyboard.Illuminator.Views;

public partial class MessageWindow : Window
{
    public MessageWindow()
    {
        InitializeComponent();

        KeyUp += (s, e) =>
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
                Close();
        };

        Loaded += (s, e) => closeButton.Focus();
    }

    public void CloseButtonClick(object sender, RoutedEventArgs args) => Close();
}