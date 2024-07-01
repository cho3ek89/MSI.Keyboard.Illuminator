using Avalonia.Controls;

using ReactiveUI;

using System.Reactive;

namespace MSI.Keyboard.Illuminator.ViewModels;

public class MessageViewModel : ReactiveObject
{
    protected string title;

    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    protected string message;

    public string Message
    {
        get => message;
        set => this.RaiseAndSetIfChanged(ref message, value);
    }

    public ReactiveCommand<Window, Unit> Close { get; }

    public MessageViewModel(string title, string message)
    {
        Title = title;
        Message = message;

        Close = ReactiveCommand.Create<Window>(window => window?.Close());
    }
}
