using Avalonia.Controls;
using Avalonia.Interactivity;

using MSI.Keyboard.Illuminator.ViewModels;

using System;

namespace MSI.Keyboard.Illuminator.Views;

public partial class ColorProfilesWindow : Window
{
    private IDisposable onSave;

    public ColorProfilesWindow()
    {
        DataContextChanged += (_, _) =>
        {
            var viewModel = (ColorProfilesViewModel)DataContext;

            onSave = viewModel.Save.Subscribe(_ =>
            {
                onSave.Dispose();
                Close();
            });
        };

        InitializeComponent();
    }

    private void CancelClick(object sender, RoutedEventArgs args) => Close();
}