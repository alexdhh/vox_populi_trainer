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
        public partial string SelectedTimeOption { get; set; }

        [ObservableProperty]
        public partial string CustomTimeInput { get; set; }

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

        private CancellationTokenSource _cancellationTokenSource;

        private string _datasetFilePath;

        public CreateModelPageViewModel()
        {
            UploadText = "Glissez votre fichier ici ou cliquez pour parcourir";
            SubText = "Formats acceptés: CSV, JSON, XLSX";
            IsFileLoaded = false;
            SelectedModel = string.Empty;
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
        [RelayCommand] private void SelectTime(string timeOption) => SelectedTimeOption = timeOption;

        public int GetTrainingTimeInSeconds()
        {
            if (SelectedTimeOption == "30m") return 30 * 60;
            if (SelectedTimeOption == "1h") return 60 * 60;
            if (SelectedTimeOption == "2h") return 2 * 60 * 60;
            if (SelectedTimeOption == "4h") return 4 * 60 * 60;
            if (SelectedTimeOption == "custom" && int.TryParse(CustomTimeInput, out int customSecs)) return customSecs;

            return 1800;
        }

        private void RunActualMLTraining(string dataPath, CancellationToken token)
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

                MainThread.BeginInvokeOnMainThread(() => TrainingStatusText = $"Entraînement en cours ({SelectedModel})...");

                ITransformer trainedModel = trainingPipeline.Fit(dataView);

                if (!token.IsCancellationRequested)
                {
                    string modelPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VoxPopuliModel.zip");
                    mlContext.Model.Save(trainedModel, dataView.Schema, modelPath);

                    _cancellationTokenSource?.Cancel();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TrainingStatusText = "Entraînement terminé et sauvegardé !";
                        TrainingProgress = 1.0;
                        ProgressPercentage = "100%";
                    });
                }
            }
            catch (OperationCanceledException)
            {
                MainThread.BeginInvokeOnMainThread(() => TrainingStatusText = "Temps limite atteint, entraînement arrêté.");
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() => TrainingStatusText = $"Erreur : {ex.Message}");
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
            int totalSeconds = GetTrainingTimeInSeconds();

            TrainingProgress = 0;
            ProgressPercentage = "0%";
            TrainingStatusText = $"Préparation de {SelectedModel}...";

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(totalSeconds));

            Task.Run(() => RunActualMLTraining(_datasetFilePath, _cancellationTokenSource.Token));
            Task.Run(() => RunProgressAnimation(totalSeconds, _cancellationTokenSource.Token));
        }

        private async Task RunProgressAnimation(int totalSeconds, CancellationToken token)
        {
            for (int i = 0; i <= totalSeconds; i++)
            {
                if (token.IsCancellationRequested && i < totalSeconds) break;

                int remaining = totalSeconds - i;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TrainingProgress = (double)i / totalSeconds;
                    ProgressPercentage = $"{(int)(TrainingProgress * 100)}%";

                    TimeSpan time = TimeSpan.FromSeconds(remaining);
                    string timeFormatted = time.TotalHours >= 1 ?
                        $"{(int)time.TotalHours}h {time.Minutes:D2}m {time.Seconds:D2}s" :
                        $"{time.Minutes:D2}m {time.Seconds:D2}s";

                    TimeRemainingText = $"{SelectedModel} - Temps restant : {timeFormatted}";
                });

                await Task.Delay(1000);
            }
        }
    }
}
