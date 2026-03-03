namespace vox_populi_trainer.ViewModels;

public partial class ChoicePageViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task Return()
    {
        // Retour à la page d'accueil
        await Shell.Current.GoToAsync("///MainPage");
    }

    [RelayCommand]
    private async Task NavigateToCreateModel()
    {
        // à faire
    }

    [RelayCommand]
    private async Task NavigateToCustomizeModel()
    {
        // à faire
    }
}
