namespace vox_populi_trainer.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        [RelayCommand]
        private async Task StartAsync()
        {
            await Shell.Current.GoToAsync(nameof(ChoicePage));

            System.Diagnostics.Debug.WriteLine("changement de page");
        }
    }
}
