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

        [ObservableProperty]
        public partial string SelectedModel { get; set; }

        [ObservableProperty]
        public partial string SelectedTimeOption { get; set; }

        [ObservableProperty]
        public partial string CustomTimeInput { get; set; }

        public CreateModelPageViewModel()
        {
            UploadText = "Glissez votre fichier ici ou cliquez pour parcourir";
            SubText = "Formats acceptés: CSV, JSON, XLSX";
            IsFileLoaded = false;
            SelectedModel = string.Empty;
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
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
                    SelectedModel = "LbfgsRegressionOva";
                }
            }
            catch (Exception)
            {
                SubText = "Erreur lors de l'importation du fichier.";
            }
        }

        [RelayCommand]
        private void SelectModel(string modelName)
        {
            SelectedModel = modelName;
        }

        [RelayCommand]
        private void SelectTime(string timeOption)
        {
            SelectedTimeOption = timeOption;
        }

        public int GetTrainingTimeInSeconds()
        {
            if (SelectedTimeOption == "30m") return 30 * 60;
            if (SelectedTimeOption == "1h") return 60 * 60;
            if (SelectedTimeOption == "2h") return 2 * 60 * 60;
            if (SelectedTimeOption == "4h") return 4 * 60 * 60;
            if (SelectedTimeOption == "custom" && int.TryParse(CustomTimeInput, out int customSecs)) return customSecs;

            return 1800;
        }

        [RelayCommand]
        private async Task StartTraining()
        {
            System.Diagnostics.Debug.WriteLine($"Début de l'entraînement avec {UploadText} sur le modèle {SelectedModel}");
        }
    }
}
