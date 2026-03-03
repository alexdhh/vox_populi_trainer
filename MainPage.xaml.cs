namespace vox_populi_trainer
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        private async void OnPointerEntered(object sender, PointerEventArgs e)
        {
            if (sender is Button btn)
            {
                await btn.ScaleTo(1.05, 150, Easing.CubicOut);
            }
        }

        private async void OnPointerExited(object sender, PointerEventArgs e)
        {
            if (sender is Button btn)
            {
                await btn.ScaleTo(1.0, 150, Easing.CubicIn);
            }
        }
    }
}
