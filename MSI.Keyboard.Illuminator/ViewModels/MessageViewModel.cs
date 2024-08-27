using ReactiveUI;

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

    public MessageViewModel(string title, string message)
    {
        Title = title;
        Message = message;
    }
}
