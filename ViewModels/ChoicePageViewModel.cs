namespace vox_populi_trainer.ViewModels;

public partial class ChoicePageViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task Return()
    {
        await Shell.Current.GoToAsync("///MainPage");
    }

    [RelayCommand]
    private async Task NavigateToCreateModel()
    {
        await Shell.Current.GoToAsync(nameof(CreateModelPage));
    }

    [RelayCommand]
    private async Task NavigateToCustomizeModel()
    {
        await Shell.Current.GoToAsync(nameof(GenerateDataPage));
    }

    [RelayCommand]
    private async Task NavigateToTestModel()
    {
        await Shell.Current.GoToAsync(nameof(TestModelPage));
    }
}
