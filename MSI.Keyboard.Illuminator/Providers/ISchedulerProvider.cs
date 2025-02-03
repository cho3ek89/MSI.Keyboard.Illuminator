using System.Reactive.Concurrency;

namespace MSI.Keyboard.Illuminator.Providers;

public interface ISchedulerProvider
{
    IScheduler MainThread { get; }
}
