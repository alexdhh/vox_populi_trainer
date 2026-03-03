namespace vox_populi_trainer.ViewModels
{
    public partial class CreateModelPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        public partial string UploadText { get; set; }

        [ObservableProperty]
        public partial string SubText { get; set; }

        [ObservableProperty]
        public partial bool IsFileLoaded { get; set; }

        public CreateModelPageViewModel()
        {
            UploadText = "Glissez votre fichier ici ou cliquez pour parcourir";
            SubText = "Formats acceptés: CSV, JSON, XLSX";
            IsFileLoaded = false;
        }

        [RelayCommand]
        private async Task Return()
        {
            await Shell.Current.GoToAsync(nameof(ChoicePage));
        }

        [RelayCommand]
        private async Task ImportDataset()
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".csv", ".json", ".xlsx" } },
                { DevicePlatform.Android, new[] { "text/csv", "application/json", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                { DevicePlatform.MacCatalyst, new[] { "public.comma-separated-values-text", "public.json", "org.openxmlformats.spreadsheetml.sheet" } }
            });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Sélectionnez votre dataset d'entraînement",
                    FileTypes = customFileType,
                });

                if (result != null)
                {
                    UploadText = result.FileName;
                    SubText = "Fichier chargé avec succès !";
                    IsFileLoaded = true;
                }
            }
            catch (Exception)
            {
                SubText = "Erreur lors de l'importation du fichier.";
            }
        }

        [RelayCommand]
        private async Task StartTraining()
        {
            System.Diagnostics.Debug.WriteLine("Début de l'entraînement avec le fichier : " + UploadText);
        }
    }
}
