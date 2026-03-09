namespace vox_populi_trainer.Views;

public partial class TestModelPage : ContentPage
{
	public TestModelPage(TestModelPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (PredictButton != null)
        {
            PredictButton.Scale = 1.0;
            VisualStateManager.GoToState(PredictButton, "Normal");
            PredictButton.Unfocus();
        }
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

    private async void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.02, 150, Easing.CubicOut);
    }

    private async void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.0, 150, Easing.CubicIn);
    }

    private async void OnPredictButtonPressed(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(0.95, 100, Easing.CubicOut);
    }

    private async void OnPredictButtonReleased(object sender, EventArgs e)
    {
        if (sender is View view) await view.ScaleTo(1.0, 100, Easing.CubicIn);
    }

    private async void OnUploadPointerEntered(object sender, PointerEventArgs e)
    {
        var vm = BindingContext as ViewModels.TestModelPageViewModel;
        if (vm != null && !vm.IsModelLoaded)
        {
            UploadBorder.Stroke = Color.Parse("#A855F7");
            UploadBorder.BackgroundColor = Color.Parse("#FBF5FF");
        }

        await UploadBorder.ScaleTo(1.02, 150, Easing.CubicOut);
    }

    private async void OnUploadPointerExited(object sender, PointerEventArgs e)
    {
        var vm = BindingContext as ViewModels.TestModelPageViewModel;
        if (vm != null && !vm.IsModelLoaded)
        {
            UploadBorder.Stroke = Color.Parse("#D1D5DB");
            UploadBorder.BackgroundColor = Colors.White;
        }

        await UploadBorder.ScaleTo(1.0, 150, Easing.CubicIn);
    }
}