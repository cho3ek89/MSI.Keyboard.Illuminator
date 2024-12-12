using Avalonia.Controls.Selection;

using MSI.Keyboard.Illuminator.Models;
using MSI.Keyboard.Illuminator.Services;

using ReactiveUI;

using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace MSI.Keyboard.Illuminator.ViewModels;

public class ColorProfilesViewModel : ReactiveObject
{
    protected readonly IAppSettingsManager appSettingsManager;

    protected ObservableCollection<ColorProfileViewModel> colorProfileViewModels;

    public ObservableCollection<ColorProfileViewModel> ColorProfileViewModels
    {
        get => colorProfileViewModels;
        set => this.RaiseAndSetIfChanged(ref colorProfileViewModels, value);
    }

    protected SelectionModel<ColorProfileViewModel> selection;

    public SelectionModel<ColorProfileViewModel> Selection
    {
        get => selection;
        set => this.RaiseAndSetIfChanged(ref selection, value);
    }

    public ReactiveCommand<Unit, Unit> Save { get; }

    public ReactiveCommand<Unit, Unit> AddNewColorProfile { get; }

    public ReactiveCommand<Unit, Unit> RemoveSelectedColorProfile { get; }

    public ReactiveCommand<Unit, Unit> MoveSelectedColorProfileUp { get; }

    public ReactiveCommand<Unit, Unit> MoveSelectedColorProfileDown { get; }

    public ColorProfilesViewModel(
        IAppSettingsManager appSettingsManager)
    {
        this.appSettingsManager = appSettingsManager;

        Selection = new();

        Save = ReactiveCommand.Create(() =>
        {
            appSettingsManager.UpdateColorProfiles(
                ColorProfileViewModels.Select(s => s.ColorProfile).Distinct());
        });

        AddNewColorProfile = ReactiveCommand.Create(() =>
        {
            var newColorProfile = ColorProfile.GetDefault();
            newColorProfile.Name = $"{Resources.Resources.DefaultProfileName} {ColorProfileViewModels?.Count + 1 ?? 1}";

            ColorProfileViewModels.Add(new ColorProfileViewModel(newColorProfile));
        });

        var canRemoveSelectedColorProfile = this
            .WhenAnyValue(s => s.Selection.SelectedItem)
            .Select(s => s != null);

        RemoveSelectedColorProfile = ReactiveCommand.Create(() =>
        {
            var idx = Selection.SelectedIndex;
            ColorProfileViewModels.Remove(Selection.SelectedItem);

            idx = ColorProfileViewModels.Count > idx ? idx : ColorProfileViewModels.Count - 1;

            selection.SelectedIndex = idx;

        }, canRemoveSelectedColorProfile);

        void MoveSelectedColorProfile(bool moveUp)
        {
            var oldIdx = Selection.SelectedIndex;
            var newIdx = Selection.SelectedIndex + (moveUp ? -1 : 1);

            ColorProfileViewModels.Move(oldIdx, newIdx);
            Selection.SelectedIndex = newIdx;
        }

        var canMoveSelectedColorProfileUp = this
            .WhenAnyValue(s => s.Selection.SelectedIndex)
            .Select(s => s > 0);

        MoveSelectedColorProfileUp = ReactiveCommand.Create(
            () => MoveSelectedColorProfile(true), 
            canMoveSelectedColorProfileUp);

        var canMoveSelectedColorProfileDown = this
            .WhenAnyValue(s => s.Selection.SelectedIndex)
            .Select(s => s >= 0 && s < ColorProfileViewModels.Count - 1);

        MoveSelectedColorProfileDown = ReactiveCommand.Create(
            () => MoveSelectedColorProfile(false), 
            canMoveSelectedColorProfileDown);
    }

    public void LoadColorProfiles()
    {
        var colorProfiles = appSettingsManager.GetColorProfiles()
            .Select(cp => new ColorProfileViewModel(cp))
            .ToList();

        ColorProfileViewModels = new ObservableCollection<ColorProfileViewModel>(colorProfiles);

        if (ColorProfileViewModels.Any())
            Selection.SelectedIndex = 0;
    }
}
