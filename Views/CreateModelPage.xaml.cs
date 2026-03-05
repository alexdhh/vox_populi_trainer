namespace vox_populi_trainer.Views;

public partial class CreateModelPage : ContentPage
{
    public CreateModelPage(CreateModelPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectedModel))
            {
                Dispatcher.Dispatch(() =>
                {
                    if (ModelsGrid != null)
                        foreach (var child in ModelsGrid.Children)
                        {
                            if (child is Border border)
                            {
                                if (border.BackgroundColor != Color.FromArgb("#FBF5FF"))
                                {
                                    border.Stroke = Color.FromArgb("#E5E7EB");
                                }
                            }
                        }
                });
            }

            if (e.PropertyName == nameof(viewModel.IsTrainingStarted))
            {
                Dispatcher.Dispatch(() =>
                {
                    if (StripesGrid != null)
                    {
                        StripesGrid.AbortAnimation("StripeAnim"); 
                        if (viewModel.IsTrainingStarted)
                        {
                            var stripeAnimation = new Animation(v => StripesGrid.TranslationX = v, 0, -30);
                            stripeAnimation.Commit(this, "StripeAnim", length: 600, repeat: () => true);
                        }
                    }
                });
            }

            if (e.PropertyName == nameof(viewModel.TrainingProgress))
            {
                Dispatcher.Dispatch(() =>
                {
                    if (ProgressBarFill != null && ProgressBarContainer != null)
                    {
                        double targetWidth = Math.Max(1, ProgressBarContainer.WidthRequest * viewModel.TrainingProgress);

                        ProgressBarFill.AbortAnimation("ProgressAnim");

                        if (viewModel.TrainingProgress == 0)
                        {
                            ProgressBarFill.WidthRequest = 1;
                            return;
                        }

                        ProgressBarFill.Animate("ProgressAnim",
                            new Animation(v => ProgressBarFill.WidthRequest = v, ProgressBarFill.WidthRequest, targetWidth),
                            length: 1000,
                            easing: Easing.Linear);
                    }
                });
            }
        };
    }

    private async void OnBackPointerEntered(object sender, PointerEventArgs e)
    {
        BackIcon.TextColor = Colors.Black;
        BackText.TextColor = Colors.Black;
        if (sender is View view) await view.ScaleTo(1.05, 150, Easing.CubicOut);
    }

    private async void OnBackPointerExited(object sender, PointerEventArgs e)
    {
        BackIcon.TextColor = Color.FromArgb("#4B5563");
        BackText.TextColor = Color.FromArgb("#4B5563");
        if (sender is View view) await view.ScaleTo(1.0, 150, Easing.CubicIn);
    }

    private async void OnUploadPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is BoxView box && box.Parent is Grid grid && grid.Parent is Border border)
        {
            if (border.BackgroundColor == Colors.White)
            {
                border.Stroke = Color.FromArgb("#A855F7");
                border.BackgroundColor = Color.FromArgb("#FBF5FF");
            }
            await border.ScaleTo(1.01, 150, Easing.CubicOut);
        }
    }

    private async void OnUploadPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is BoxView box && box.Parent is Grid grid && grid.Parent is Border border)
        {
            if (border.BackgroundColor == Colors.White || border.BackgroundColor.ToHex() == "#FBF5FF")
            {
                if (BindingContext is CreateModelPageViewModel vm && !vm.IsFileLoaded)
                {
                    border.Stroke = Color.FromArgb("#D1D5DB");
                    border.BackgroundColor = Colors.White;
                }
            }
            await border.ScaleTo(1.0, 150, Easing.CubicIn);
        }
    }

    private async void OnDrop(object sender, DropEventArgs e)
    {
        if (sender is BoxView box && box.Parent is Grid grid && grid.Parent is Border border)
        {
            await border.ScaleTo(1.0, 150, Easing.CubicIn);
        }

        if (BindingContext is CreateModelPageViewModel viewModel)
        {
            string fileName = "training_data.csv";

            try
            {
                var droppedText = await e.Data.GetTextAsync();
                if (!string.IsNullOrWhiteSpace(droppedText))
                {
                    fileName = droppedText.Replace("file:///", "").Replace("file://", "").Trim();
                }
            }
            catch { }

            viewModel.UploadText = fileName;
            viewModel.SubText = "Fichier chargé avec succès !";
            viewModel.IsFileLoaded = true;
            viewModel.SelectedModel = "LbfgsRegressionOva";
        }
    }

    private async void OnModelPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is BoxView box && box.Parent is Grid grid && grid.Parent is Border border)
        {
            await border.ScaleTo(1.02, 100, Easing.CubicOut);

            if (border.BackgroundColor != Color.FromArgb("#FBF5FF"))
            {
                border.Stroke = Color.FromArgb("#A855F7");
            }
        }
    }

    private async void OnModelPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is BoxView box && box.Parent is Grid grid && grid.Parent is Border border)
        {
            await border.ScaleTo(1.0, 100, Easing.CubicIn);

            if (border.BackgroundColor != Color.FromArgb("#FBF5FF"))
            {
                border.Stroke = Color.FromArgb("#E5E7EB");
            }
        }
    }

    private void OnCustomTimeTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue))
            return;

        if (!e.NewTextValue.All(char.IsDigit))
        {
            if (sender is Entry entry)
            {
                entry.Text = e.OldTextValue;
            }
        }
    }

    private async void OnStartButtonEntered(object sender, PointerEventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.05, 150, Easing.CubicOut);
    }

    private async void OnStartButtonExited(object sender, PointerEventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.0, 150, Easing.CubicIn);
    }

    private async void OnStartButtonPressed(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(0.95, 100, Easing.CubicOut);
    }

    private async void OnStartButtonReleased(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.05, 100, Easing.CubicIn);
    }

    private async void OnExportButtonEntered(object sender, PointerEventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.05, 150, Easing.CubicOut);
    }

    private async void OnExportButtonExited(object sender, PointerEventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.0, 150, Easing.CubicIn);
    }

    private async void OnExportButtonPressed(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(0.95, 100, Easing.CubicOut);
    }

    private async void OnExportButtonReleased(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.05, 100, Easing.CubicIn);
    }
}