using Avalonia.Headless.XUnit;

using Microsoft.Reactive.Testing;

using Moq;

using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Services;
using MSI.Keyboard.Illuminator.Tests.Helpers;
using MSI.Keyboard.Illuminator.ViewModels;

using System.Reactive.Linq;

using Xunit;

namespace MSI.Keyboard.Illuminator.Tests;

public class ColorProfilesViewModelTests
{
    private readonly TestScheduler scheduler;

    private readonly Mock<IAppSettingsManager> appSettingsManagerMock;

    private readonly ColorProfilesViewModel viewModel;

    public ColorProfilesViewModelTests()
    {
        appSettingsManagerMock = new Mock<IAppSettingsManager>();
        appSettingsManagerMock.Setup(s => s.GetColorProfiles()).Returns(
        [
            TestDataGenerator.GetDummyColorProfile(0),
            TestDataGenerator.GetDummyColorProfile(1),
            TestDataGenerator.GetDummyColorProfile(2),
        ]);

        scheduler = new();
        var schedulerProvider = new TestSchedulerProvider(scheduler);

        viewModel = new ColorProfilesViewModel(
            appSettingsManagerMock.Object,
            schedulerProvider);

        viewModel.LoadColorProfiles();
        viewModel.Selection.Source = viewModel.ColorProfileViewModels;
    }

    [AvaloniaFact]
    public void AddNewColorProfileCreatesNewProfile()
    {
        viewModel.AddNewColorProfile.Execute().Subscribe();

        Assert.Equal(4, viewModel.ColorProfileViewModels.Count);
    }

    [AvaloniaFact]
    public void MoveSelectedColorProfileDownMovesProfileDown()
    {
        viewModel.Selection.Select(1);

        viewModel.MoveSelectedColorProfileDown.Execute()
            .Subscribe(_ => { });

        Assert.Equal(
            ["0", "2", "1"],
            viewModel.ColorProfileViewModels.Select(s => s.ColorProfile.Name.ToString()));
    }

    [AvaloniaFact]
    public void MoveSelectedColorProfileUpMovesProfileUp()
    {
        viewModel.Selection.Select(1);

        viewModel.MoveSelectedColorProfileUp.Execute()
            .Subscribe(_ => { });

        Assert.Equal(
            ["1", "0", "2"],
            viewModel.ColorProfileViewModels.Select(s => s.ColorProfile.Name.ToString()));
    }

    [AvaloniaFact]
    public void RemoveSelectedColorProfileDeletesSelectedProfile()
    {
        viewModel.Selection.Select(1);

        viewModel.RemoveSelectedColorProfile.Execute().Subscribe();

        Assert.Equal(2, viewModel.ColorProfileViewModels.Count);
    }

    [AvaloniaFact]
    public void SaveCallsUpdateColorProfiles()
    {
        viewModel.Save.Execute().Subscribe();

        appSettingsManagerMock.Verify(
            v => v.UpdateColorProfiles(It.IsAny<IEnumerable<ColorProfile>>()),
            Times.Once);
    }
}
