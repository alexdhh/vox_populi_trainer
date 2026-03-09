namespace vox_populi_trainer.ViewModels
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1)]
        public string Label { get; set; }
    }

    public class ModelOutput
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
                    "LbfgsLogisticRegression" => mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression()),
                    "FastTree" => mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.FastTree()),
                    "SdcaLogisticRegression" => mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression()),
                    "FastForest" => mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.FastForest()),
                    _ => mlContext.MulticlassClassification.Trainers.LightGbm()
                };

                var trainingPipeline = dataProcessPipeline
                    .Append(trainer)
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                ITransformer trainedModel = trainingPipeline.Fit(dataView);

                string modelPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VoxPopuli.mlnet");
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
        private async Task ExportModel(CancellationToken cancellationToken)
        {
            try
            {
                string mlnetPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VoxPopuli.mlnet");

                if (!File.Exists(mlnetPath)) return;

                string tempFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExportTemp");
                if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
                Directory.CreateDirectory(tempFolder);

                string destMlnetPath = System.IO.Path.Combine(tempFolder, "VoxPopuli.mlnet");
                File.Copy(mlnetPath, destMlnetPath);

                File.WriteAllText(System.IO.Path.Combine(tempFolder, "VoxPopuli.consumption.cs"), GenerateConsumptionCode());
                File.WriteAllText(System.IO.Path.Combine(tempFolder, "VoxPopuli.training.cs"), GenerateTrainingCode());
                File.WriteAllText(System.IO.Path.Combine(tempFolder, "VoxPopuli.evaluate.cs"), GenerateEvaluateCode());

                string zipPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VoxPopuli_Model.zip");
                if (File.Exists(zipPath)) File.Delete(zipPath);
                ZipFile.CreateFromDirectory(tempFolder, zipPath);

                using var stream = new FileStream(zipPath, FileMode.Open, FileAccess.Read);
                var fileSaverResult = await FileSaver.Default.SaveAsync($"VoxPopuli_{SelectedModel}.zip", stream, cancellationToken);

                stream.Close();
                Directory.Delete(tempFolder, true);
                File.Delete(zipPath);
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", $"Impossible d'exporter : {ex.Message}", "Fermer");
                }
            }
        }

        private string GenerateConsumptionCode()
        {
            return @"// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace Vox_populi_test_model
{
    public partial class VoxPopuli
    {
        /// <summary>
        /// model input class for VoxPopuli.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [LoadColumn(0)]
            [ColumnName(@""Text"")]
            public string Text { get; set; }

            [LoadColumn(1)]
            [ColumnName(@""Label"")]
            public string Label { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for VoxPopuli.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@""Text"")]
            public float[] Text { get; set; }

            [ColumnName(@""Label"")]
            public uint Label { get; set; }

            [ColumnName(@""Features"")]
            public float[] Features { get; set; }

            [ColumnName(@""PredictedLabel"")]
            public string PredictedLabel { get; set; }

            [ColumnName(@""Score"")]
            public float[] Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath(""VoxPopuli.mlnet"");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);


        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        /// <summary>
        /// Use this method to predict scores for all possible labels.
        /// </summary>
        /// <param name=""input"">model input.</param>
        /// <returns><seealso cref="" ModelOutput""/></returns>
        public static IOrderedEnumerable<KeyValuePair<string, float>> PredictAllLabels(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            var result = predEngine.Predict(input);
            return GetSortedScoresWithLabels(result);
        }

        /// <summary>
        /// Map the unlabeled result score array to the predicted label names.
        /// </summary>
        /// <param name=""result"">Prediction to get the labeled scores from.</param>
        /// <returns>Ordered list of label and score.</returns>
        /// <exception cref=""Exception""></exception>
        public static IOrderedEnumerable<KeyValuePair<string, float>> GetSortedScoresWithLabels(ModelOutput result)
        {
            var unlabeledScores = result.Score;
            var labelNames = GetLabels(result);

            Dictionary<string, float> labledScores = new Dictionary<string, float>();
            for (int i = 0; i < labelNames.Count(); i++)
            {
                // Map the names to the predicted result score array
                var labelName = labelNames.ElementAt(i);
                labledScores.Add(labelName.ToString(), unlabeledScores[i]);
            }

            return labledScores.OrderByDescending(c => c.Value);
        }

        /// <summary>
        /// Get the ordered label names.
        /// </summary>
        /// <param name=""result"">Predicted result to get the labels from.</param>
        /// <returns>List of labels.</returns>
        /// <exception cref=""Exception""></exception>
        private static IEnumerable<string> GetLabels(ModelOutput result)
        {
            var schema = PredictEngine.Value.OutputSchema;

            var labelColumn = schema.GetColumnOrNull(""Label"");
            if (labelColumn == null)
            {
                throw new Exception(""Label column not found. Make sure the name searched for matches the name in the schema."");
            }

            // Key values contains an ordered array of the possible labels. This allows us to map the results to the correct label value.
            var keyNames = new VBuffer<ReadOnlyMemory<char>>();
            labelColumn.Value.GetKeyValues(ref keyNames);
            return keyNames.DenseValues().Select(x => x.ToString());
        }

        /// <summary>
        /// Use this method to predict on <see cref=""ModelInput""/>.
        /// </summary>
        /// <param name=""input"">model input.</param>
        /// <returns><seealso cref="" ModelOutput""/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }
    }
}";
        }

        private string GenerateTrainingCode()
        {
            string trainerCode = SelectedModel switch
            {
                "LbfgsRegressionOva" => @"mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(new LbfgsLogisticRegressionBinaryTrainer.Options(){L1Regularization=1F,L2Regularization=1F,LabelColumnName=@""Label"",FeatureColumnName=@""Features""}), labelColumnName:@""Label"")",
                "LbfgsMaximumEntropy" => @"mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName:@""Label"", featureColumnName:@""Features"")",
                "LightGbm" => @"mlContext.MulticlassClassification.Trainers.LightGbm(labelColumnName:@""Label"", featureColumnName:@""Features"")",
                "SdcaMaximumEntropy" => @"mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName:@""Label"", featureColumnName:@""Features"")",
                "LbfgsLogisticRegression" => @"mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(), labelColumnName:@""Label"")",
                "FastTree" => @"mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mlContext.BinaryClassification.Trainers.FastTree(), labelColumnName:@""Label"")",
                "SdcaLogisticRegression" => @"mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(), labelColumnName:@""Label"")",
                "FastForest" => @"mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mlContext.BinaryClassification.Trainers.FastForest(), labelColumnName:@""Label"")",
                _ => @"mlContext.MulticlassClassification.Trainers.LightGbm(labelColumnName:@""Label"", featureColumnName:@""Features"")"
            };

            string template = @"// This file was auto-generated by ML.NET Model Builder.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace Vox_populi_test_model
{
    public partial class VoxPopuli
    {
        public const string RetrainFilePath =  @""C:\Vox Populli 4.0\datasets\python\training_data.csv"";
        public const char RetrainSeparatorChar = ',';
        public const bool RetrainHasHeader =  true;
        public const bool RetrainAllowQuoting =  false;

         /// <summary>
        /// Train a new model with the provided dataset.
        /// </summary>
        /// <param name=""outputModelPath"">File path for saving the model. Should be similar to ""C:\YourPath\ModelName.mlnet""</param>
        /// <param name=""inputDataFilePath"">Path to the data file for training.</param>
        /// <param name=""separatorChar"">Separator character for delimited training file.</param>
        /// <param name=""hasHeader"">Boolean if training file has a header.</param>
        public static void Train(string outputModelPath, string inputDataFilePath = RetrainFilePath, char separatorChar = RetrainSeparatorChar, bool hasHeader = RetrainHasHeader, bool allowQuoting = RetrainAllowQuoting)
        {
            var mlContext = new MLContext();

            var data = LoadIDataViewFromFile(mlContext, inputDataFilePath, separatorChar, hasHeader, allowQuoting);
            var model = RetrainModel(mlContext, data);
            SaveModel(mlContext, model, data, outputModelPath);
        }

        /// <summary>
        /// Load an IDataView from a file path.
        /// </summary>
        /// <param name=""mlContext"">The common context for all ML.NET operations.</param>
        /// <param name=""inputDataFilePath"">Path to the data file for training.</param>
        /// <param name=""separatorChar"">Separator character for delimited training file.</param>
        /// <param name=""hasHeader"">Boolean if training file has a header.</param>
        /// <returns>IDataView with loaded training data.</returns>
        public static IDataView LoadIDataViewFromFile(MLContext mlContext, string inputDataFilePath, char separatorChar, bool hasHeader, bool allowQuoting)
        {
            return mlContext.Data.LoadFromTextFile<ModelInput>(inputDataFilePath, separatorChar, hasHeader, allowQuoting: allowQuoting);
        }


        /// <summary>
        /// Save a model at the specified path.
        /// </summary>
        /// <param name=""mlContext"">The common context for all ML.NET operations.</param>
        /// <param name=""model"">Model to save.</param>
        /// <param name=""data"">IDataView used to train the model.</param>
        /// <param name=""modelSavePath"">File path for saving the model. Should be similar to ""C:\YourPath\ModelName.mlnet.</param>
        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            // Pull the data schema from the IDataView used for training the model
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
            {
                mlContext.Model.Save(model, dataViewSchema, fs);
            }
        }


        /// <summary>
        /// Retrain model using the pipeline generated as part of the training process.
        /// </summary>
        /// <param name=""mlContext""></param>
        /// <param name=""trainData""></param>
        /// <returns></returns>
        public static ITransformer RetrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name=""mlContext""></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.Text.FeaturizeText(inputColumnName:@""Text"",outputColumnName:@""Text"")      
                                    .Append(mlContext.Transforms.Concatenate(@""Features"", new []{@""Text""}))      
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@""Label"",inputColumnName:@""Label"",addKeyValueAnnotationsAsText:false))      
                                    .Append({TRAINER_CODE})      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@""PredictedLabel"",inputColumnName:@""PredictedLabel""));

            return pipeline;
        }
    }
 }";

            return template.Replace("{TRAINER_CODE}", trainerCode);
        }

        private string GenerateEvaluateCode()
        {
            return @"// This file was auto-generated by ML.NET Model Builder.

using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vox_populi_test_model
{
    public partial class VoxPopuli
    {
        /// <summary>
        /// Permutation feature importance (PFI) is a technique to determine the importance 
        /// of features in a trained machine learning model. PFI works by taking a labeled dataset, 
        /// choosing a feature, and permuting the values for that feature across all the examples, 
        /// so that each example now has a random value for the feature and the original values for all other features.
        /// The evaluation metric (e.g. R-squared) is then calculated for this modified dataset, 
        /// and the change in the evaluation metric from the original dataset is computed. 
        /// The larger the change in the evaluation metric, the more important the feature is to the model.
        /// 
        /// PFI typically takes a long time to compute, as the evaluation metric is calculated 
        /// many times to determine the importance of each feature. 
        /// 
        /// </summary>
        /// <param name=""mlContext"">The common context for all ML.NET operations.</param>
        /// <param name=""trainData"">IDataView used to evaluate the model.</param>
        /// <param name=""model"">Model to evaluate.</param>
        /// <param name=""labelColumnName"">Label column being predicted.</param>
        /// <returns>A list of each feature and its importance.</returns>
        public static List<Tuple<string, double>> CalculatePFI(MLContext mlContext, IDataView trainData, ITransformer model, string labelColumnName)
        {
            var preprocessedTrainData = model.Transform(trainData);

            var permutationFeatureImportance =
         mlContext.MulticlassClassification
         .PermutationFeatureImportance(
                 model,
                 preprocessedTrainData,
                 labelColumnName: labelColumnName);

            var featureImportanceMetrics =
                 permutationFeatureImportance
                 .Select((kvp) => new { kvp.Key, kvp.Value.MacroAccuracy })
                 .OrderByDescending(myFeatures => Math.Abs(myFeatures.MacroAccuracy.Mean));

            var featurePFI = new List<Tuple<string, double>>();
            foreach (var feature in featureImportanceMetrics)
            {
                var pfiValue = Math.Abs(feature.MacroAccuracy.Mean);
                featurePFI.Add(new Tuple<string, double>(feature.Key, pfiValue));
            }

            return featurePFI;
        }
    }
}";
        }
    }
}