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

    private async void OnReturnPointerEntered(object sender, PointerEventArgs e)
    {
        BackIcon.TextColor = Colors.Black;
        BackText.TextColor = Colors.Black;

        if (sender is View view)
        {
            await view.ScaleTo(1.05, 150, Easing.CubicOut);
        }
    }

    private async void OnReturnPointerExited(object sender, PointerEventArgs e)
    {
        BackIcon.TextColor = Color.FromArgb("#4B5563");
        BackText.TextColor = Color.FromArgb("#4B5563");

        if (sender is View view)
        {
            await view.ScaleTo(1.0, 150, Easing.CubicIn);
        }
    }
}