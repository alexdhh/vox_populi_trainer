namespace vox_populi_trainer.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        // Title for each page
        [ObservableProperty]
        private string title = string.Empty;

        // Shows loading spinner, disables UI actions
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        // Useful convenience property
        public bool IsNotBusy => !IsBusy;

        public BaseViewModel(string title = "")
        {
            Title = title;
        }

        protected async Task ShowAlert(string title, string message, string button)
        {
            var window = Application.Current?.Windows != null && Application.Current.Windows.Count > 0
                ? Application.Current.Windows[0]
                : null;

            if (window?.Page != null)
            {
                await window.Page.DisplayAlertAsync(title, message, button);
            }
        }
    }
}
