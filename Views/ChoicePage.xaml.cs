namespace vox_populi_trainer.Views;

public partial class ChoicePage : ContentPage
{
	public ChoicePage()
	{
		InitializeComponent();
		BindingContext = new ChoicePageViewModel();
    }

    private async void OnCardPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Border card)
        {
            card.Stroke = Color.FromArgb("#A855F7");
            if (card.Shadow != null)
            {
                card.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#A855F7"));
                card.Shadow.Opacity = 0.3f;
            }

            var scaleTask = card.ScaleTo(1.02, 150, Easing.CubicOut);

            if (card.Content is Layout layout && layout.Children.Count > 0 && layout.Children[^1] is BoxView line)
            {
                await Task.WhenAll(scaleTask, line.FadeTo(1, 150, Easing.CubicOut));
            }
            else
            {
                await scaleTask;
            }
        }
    }

    private async void OnCardPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Border card)
        {
            card.Stroke = Color.FromArgb("#E5E7EB");
            if (card.Shadow != null)
            {
                card.Shadow.Opacity = 0f;
                card.Shadow.Brush = new SolidColorBrush(Colors.Transparent);
            }

            var scaleTask = card.ScaleTo(1.0, 150, Easing.CubicIn);

            if (card.Content is Layout layout && layout.Children.Count > 0 && layout.Children[^1] is BoxView line)
            {
                await Task.WhenAll(scaleTask, line.FadeTo(0, 150, Easing.CubicIn));
            }
            else
            {
                await scaleTask;
            }
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
}