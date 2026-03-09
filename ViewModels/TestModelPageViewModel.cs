namespace vox_populi_trainer.ViewModels
{
    // Classes locales pour structurer la donnée du modèle
    public class TestModelInput
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        public string Label { get; set; }
    }

    public class TestModelOutput
    {
        [ColumnName("PredictedLabel")]
        public string Prediction { get; set; }

        public float[] Score { get; set; }
    }

    public partial class TestModelPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        public partial string UploadText { get; set; }

        [ObservableProperty]
        public partial string SubText { get; set; }

        [ObservableProperty]
        public partial bool IsModelLoaded { get; set; }

        [ObservableProperty]
        public partial string InputText { get; set; }

        [ObservableProperty]
        public partial bool IsPredictionVisible { get; set; }

        [ObservableProperty]
        public partial string PredictionResultText { get; set; }

        private string _modelFilePath;
        private MLContext _mlContext;
        private PredictionEngine<TestModelInput, TestModelOutput> _predictionEngine;

        public TestModelPageViewModel()
        {
            UploadText = "Sélectionnez un modèle";
            SubText = "Importez un modèle .mlnet";
            IsModelLoaded = false;
            IsPredictionVisible = false;
            InputText = string.Empty;
        }

        [RelayCommand]
        private async Task ReturnAsync()
        {
            await Shell.Current.GoToAsync(nameof(ChoicePage));
        }

        [RelayCommand]
        private async Task ImportModel()
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".mlnet" } },
                    { DevicePlatform.Android, new[] { "application/octet-stream" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.data" } }
                });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Sélectionnez votre modèle d'intelligence artificielle",
                    FileTypes = customFileType,
                });

                if (result != null)
                {
                    string localPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TestingModel.mlnet");
                    using (var stream = await result.OpenReadAsync())
                    using (var newStream = File.Create(localPath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    _modelFilePath = localPath;

                    _mlContext = new MLContext();
                    ITransformer trainedModel = _mlContext.Model.Load(_modelFilePath, out var modelInputSchema);
                    _predictionEngine = _mlContext.Model.CreatePredictionEngine<TestModelInput, TestModelOutput>(trainedModel);

                    UploadText = result.FileName;
                    SubText = "Modèle chargé et prêt !";
                    IsModelLoaded = true;
                    IsPredictionVisible = false;
                }
            }
            catch (Exception)
            {
                SubText = "Erreur lors du chargement du modèle.";
            }
        }

        [RelayCommand]
        private void GetPrediction()
        {
            if (string.IsNullOrWhiteSpace(InputText) || _predictionEngine == null)
                return;

            try
            {
                var input = new TestModelInput { Text = InputText };

                var prediction = _predictionEngine.Predict(input);

                float confidence = prediction.Score.Max() * 100;

                PredictionResultText = $"Prédiction : {prediction.Prediction} avec une confiance de {confidence:F1}%";
                IsPredictionVisible = true;
            }
            catch (Exception ex)
            {
                PredictionResultText = $"Erreur lors de la prédiction : {ex.Message}";
                IsPredictionVisible = true;
            }
        }

        partial void OnInputTextChanged(string value)
        {
            if (IsPredictionVisible)
                IsPredictionVisible = false;
        }
    }
}
