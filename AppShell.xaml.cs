namespace vox_populi_trainer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Navigation

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(ChoicePage), typeof(ChoicePage));
            Routing.RegisterRoute(nameof(CreateModelPage), typeof(CreateModelPage));
        }
    }
}
