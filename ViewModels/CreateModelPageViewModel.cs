namespace vox_populi_trainer.ViewModels
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1)]
        public string Label { get; set; }
    }

    public class ModeOutput
    {
        [ColumnName("PredictedLabel")]
        public string Prediction { get; set; }

        public float[] Score { get; set; }
    }

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
        public partial bool IsTrainingStarted { get; set; }

        [ObservableProperty]
        public partial double TrainingProgress { get; set; }

        [ObservableProperty]
        public partial string ProgressPercentage { get; set; }

        [ObservableProperty]
        public partial string TrainingStatusText { get; set; }

        [ObservableProperty]
        public partial string TimeRemainingText { get; set; }

        [ObservableProperty]
        public partial bool IsTrainingCompleted { get; set; }

        private string _datasetFilePath;
        private bool _isTrainingFinished = false;

        public CreateModelPageViewModel()
        {
            UploadText = "Glissez votre fichier ici ou cliquez pour parcourir";
            SubText = "Formats acceptés: CSV, JSON, XLSX";
            IsFileLoaded = false;
            SelectedModel = string.Empty;
            IsTrainingCompleted = false;
        }

        [RelayCommand]
        private async Task ReturnAsync()
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
                    string localPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "training_data.csv");
                    using (var stream = await result.OpenReadAsync())
                    using (var newStream = System.IO.File.Create(localPath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    _datasetFilePath = localPath;

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

        [RelayCommand] private void SelectModel(string modelName) => SelectedModel = modelName;

        private void RunActualMLTraining(string dataPath)
        {
            try
            {
                var mlContext = new MLContext(seed: 0);

                IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                    path: dataPath,
                    hasHeader: false,
                    separatorChar: ',');

                var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                    .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Text"));

                IEstimator<ITransformer> trainer = SelectedModel switch
                {
                    "LbfgsRegressionOva" => mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression()),
                    "LbfgsMaximumEntropy" => mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(),
                    "LightGbm" => mlContext.MulticlassClassification.Trainers.LightGbm(),
                    "SdcaMaximumEntropy" => mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(),
                    "LbfgsLogisticRegression" => mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(),
                    "FastTree" => mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.FastTree()),
                    "SdcaLogisticRegression" => mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression()),
                    "FastForest" => mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.FastForest()),
                    _ => mlContext.MulticlassClassification.Trainers.LightGbm()
                };

                var trainingPipeline = dataProcessPipeline
                    .Append(trainer)
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                ITransformer trainedModel = trainingPipeline.Fit(dataView);

                string modelPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VoxPopuliModel.mlnet");
                mlContext.Model.Save(trainedModel, dataView.Schema, modelPath);

                _isTrainingFinished = true;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TrainingStatusText = "Entraînement terminé !";
                    TimeRemainingText = TimeRemainingText.Replace("Temps écoulé", "Temps total");
                    TrainingProgress = 1.0;
                    ProgressPercentage = "100%";
                    IsTrainingCompleted = true;
                });
            }
            catch (Exception ex)
            {
                _isTrainingFinished = true;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TrainingStatusText = "Erreur d'entraînement";
                    TimeRemainingText = ex.Message;
                });
            }
        }

        [RelayCommand]
        private void StartTraining()
        {
            if (string.IsNullOrEmpty(_datasetFilePath) || !System.IO.File.Exists(_datasetFilePath))
            {
                IsTrainingStarted = true;
                TrainingStatusText = "Erreur : Dataset introuvable. Veuillez réimporter le fichier.";
                ProgressPercentage = "Erreur";
                return;
            }

            IsTrainingStarted = true;
            IsTrainingCompleted = false;
            _isTrainingFinished = false;

            TrainingProgress = 0.01;
            ProgressPercentage = "0%";
            TrainingStatusText = $"Entraînement en cours...";
            TimeRemainingText = $"{SelectedModel} - Temps écoulé : 00m 00s";

            Task.Run(() => RunActualMLTraining(_datasetFilePath));
            Task.Run(() => RunProgressAnimation());
        }

        private async Task RunProgressAnimation()
        {
            int elapsedSeconds = 0;

            while (!_isTrainingFinished)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    double progress = 0.99 * (1.0 - Math.Exp(-elapsedSeconds / 120.0));

                    TrainingProgress = Math.Max(0.01, progress);
                    ProgressPercentage = $"{(int)(TrainingProgress * 100)}%";

                    if (TrainingProgress > 0.98)
                    {
                        TrainingStatusText = "Finalisation en cours, merci de patienter...";
                    }

                    TimeSpan time = TimeSpan.FromSeconds(elapsedSeconds);
                    string timeFormatted = time.TotalHours >= 1 ?
                        $"{(int)time.TotalHours}h {time.Minutes:D2}m {time.Seconds:D2}s" :
                        $"{time.Minutes:D2}m {time.Seconds:D2}s";

                    TimeRemainingText = $"{SelectedModel} - Temps écoulé : {timeFormatted}";
                });

                await Task.Delay(1000);
                elapsedSeconds++;
            }
        }

        [RelayCommand]
        private async Task ExportModel()
        {
            try
            {
                string sourcePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VoxPopuliModel.mlnet");

                string destFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string destPath = System.IO.Path.Combine(destFolder, $"VoxPopuli_{SelectedModel}.mlnet");

                if (System.IO.File.Exists(sourcePath))
                {
                    System.IO.File.Copy(sourcePath, destPath, true);

                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Succès", $"Modèle exporté avec succès sur votre Bureau :\n{destPath}", "Super !");
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", $"Impossible d'exporter : {ex.Message}", "Fermer");
                }
            }
        }
    }
}