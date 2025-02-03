using Microsoft.Reactive.Testing;

using MSI.Keyboard.Illuminator.Providers;

using System.Reactive.Concurrency;

namespace MSI.Keyboard.Illuminator.Tests.Helpers;

public class TestSchedulerProvider(TestScheduler testScheduler) : ISchedulerProvider
{
    private readonly TestScheduler testScheduler = testScheduler;

    public IScheduler MainThread => testScheduler;
}
