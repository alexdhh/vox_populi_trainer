namespace vox_populi_trainer.Views;

public partial class GenerateDataPage : ContentPage
{
    public GenerateDataPage(GenerateDataPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.IsGenerating))
            {
                Dispatcher.Dispatch(() =>
                {
                    if (StripesGrid != null)
                    {
                        StripesGrid.AbortAnimation("StripeAnim");
                        if (viewModel.IsGenerating)
                        {
                            var stripeAnimation = new Animation(v => StripesGrid.TranslationX = v, 0, -30);
                            stripeAnimation.Commit(this, "StripeAnim", length: 600, repeat: () => true);
                        }
                    }
                });
            }

            if (e.PropertyName == nameof(viewModel.Progress))
            {
                Dispatcher.Dispatch(() =>
                {
                    if (ProgressBarFill != null && ProgressBarContainer != null)
                    {
                        double targetWidth = Math.Max(1, ProgressBarContainer.WidthRequest * viewModel.Progress);

                        ProgressBarFill.AbortAnimation("ProgressAnim");

                        if (viewModel.Progress == 0)
                        {
                            ProgressBarFill.WidthRequest = 1;
                            return;
                        }

                        ProgressBarFill.Animate("ProgressAnim",
                            new Animation(v => ProgressBarFill.WidthRequest = v, ProgressBarFill.WidthRequest, targetWidth),
                            length: 250,
                            easing: Easing.Linear);
                    }
                });
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (StartButton != null)
        {
            StartButton.Scale = 1.0;
            VisualStateManager.GoToState(StartButton, "Normal");
            StartButton.Unfocus();
        }

        if (ExportButton != null)
        {
            ExportButton.Scale = 1.0;
            VisualStateManager.GoToState(ExportButton, "Normal");
            ExportButton.Unfocus();
        }
    }

    private void OnBackPointerEntered(object sender, PointerEventArgs e)
    {
        BackButtonBorder.BackgroundColor = Microsoft.Maui.Graphics.Color.Parse("#E5E7EB");

        BackButtonBorder.ScaleTo(1.1, 150, Easing.CubicOut);
    }

    private void OnBackPointerExited(object sender, PointerEventArgs e)
    {
        BackButtonBorder.BackgroundColor = Microsoft.Maui.Graphics.Color.Parse("#F3F4F6");
        BackButtonBorder.ScaleTo(1.0, 150, Easing.CubicIn);
    }

    private async void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.05, 150, Easing.CubicOut);
    }

    private async void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.0, 150, Easing.CubicIn);
    }

    private async void OnStartButtonPressed(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(0.95, 100, Easing.CubicOut);
    }

    private async void OnStartButtonReleased(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.0, 100, Easing.CubicIn);
    }

    private async void OnExportButtonPressed(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(0.95, 100, Easing.CubicOut);
    }

    private async void OnExportButtonReleased(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.0, 100, Easing.CubicIn);
    }
}