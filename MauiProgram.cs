namespace vox_populi_trainer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiCommunityToolkit()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ChoicePageViewModel>();
            builder.Services.AddTransient<ChoicePage>();
            builder.Services.AddTransient<CreateModelPageViewModel>();
            builder.Services.AddTransient<CreateModelPage>();
            builder.Services.AddTransient<GenerateDataPageViewModel>();
            builder.Services.AddTransient<GenerateDataPage>();
            builder.Services.AddTransient<TestModelPageViewModel>();
            builder.Services.AddTransient<TestModelPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
