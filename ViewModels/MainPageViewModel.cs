namespace vox_populi_trainer.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        [RelayCommand]
        private async Task StartAsync()
        {
            // Remplacez "ChoicePage" par le nom de la page suivante lorsque nous la créerons
            // await Shell.Current.GoToAsync(nameof(ChoicePage));

            System.Diagnostics.Debug.WriteLine("Le bouton Commencer a été cliqué !");
        }
    }
}
