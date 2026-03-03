namespace vox_populi_trainer.Views;

public partial class CreateModelPage : ContentPage
{
    public CreateModelPage(CreateModelPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
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
        if (sender is Border border)
        {
            border.Stroke = Color.FromArgb("#A855F7");
            border.BackgroundColor = Color.FromArgb("#FBF5FF");
            await border.ScaleTo(1.01, 150, Easing.CubicOut);
        }
    }

    private async void OnUploadPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Border border)
        {
            border.Stroke = Color.FromArgb("#D1D5DB");
            border.BackgroundColor = Colors.White;
            await border.ScaleTo(1.0, 150, Easing.CubicIn);
        }
    }

    private async void OnDrop(object sender, DropEventArgs e)
    {
        if (sender is Border border)
        {
            border.Stroke = Color.FromArgb("#D1D5DB");
            border.BackgroundColor = Colors.White;
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
        }
    }
}