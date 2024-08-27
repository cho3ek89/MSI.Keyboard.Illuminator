using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MSI.Keyboard.Illuminator.Views;

public partial class MessageWindow : Window
{
    public MessageWindow()
    {
        InitializeComponent();
    }

    public void MessageCloseButtonClick(object sender, RoutedEventArgs args) => Close();
}