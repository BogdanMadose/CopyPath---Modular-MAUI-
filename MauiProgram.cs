using CommunityToolkit.Maui;
using CopyPath___Modular_MAUI_.Services;
using CopyPath___Modular_MAUI_.ViewModels;
using Microsoft.Extensions.Logging;

namespace CopyPath___Modular_MAUI_
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<FileTransferService>();
            builder.Services.AddSingleton<SettingsPageViewModel>();
            builder.Services.AddSingleton<MainPage>();
            return builder.Build();
        }
    }
}
