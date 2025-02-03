using ReactiveUI;

using System.Reactive.Concurrency;

namespace MSI.Keyboard.Illuminator.Providers;

public class SchedulerProvider : ISchedulerProvider
{
    public IScheduler MainThread => RxApp.MainThreadScheduler;
}
