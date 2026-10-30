using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Handlers;
using WestmarchBook.Application;
using WestmarchBook.Desktop.Services;
using WestmarchBook.Desktop.ViewModels;
using WestmarchBook.Desktop.Views;
using WestmarchBook.Domain.Security.Tokens;
using WestmarchBook.Infrastructure;
using WestmarchBook.Infrastructure.Migrations;

namespace WestmarchBook.Desktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        EntryHandler.Mapper.AppendToMapping("BorderlessEntry", (handler, view) =>
        {
#if WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = null;
            handler.PlatformView.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.IsSpellCheckEnabled = false;
            handler.PlatformView.IsTextPredictionEnabled = false;

            if (handler.PlatformView is Microsoft.Maui.Platform.MauiPasswordTextBox passwordBox)
            {
                passwordBox.IsObfuscationDelayed = false;
            }
            handler.PlatformView.KeyDown += (_, e) =>
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    view.Completed();
                    e.Handled = true;
                }
            };
#endif
        });

        EntryHandler.Mapper.AppendToMapping(nameof(IEntry.IsPassword), (handler, _) =>
        {
#if WINDOWS
            if (handler.PlatformView is Microsoft.Maui.Platform.MauiPasswordTextBox passwordBox)
            {
                passwordBox.IsObfuscationDelayed = false;
            }
#endif
        });

        EditorHandler.Mapper.AppendToMapping("BorderlessEditor", (handler, _) =>
        {
#if WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = null;
            handler.PlatformView.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
        });

        PickerHandler.Mapper.AppendToMapping("BorderlessPicker", (handler, _) =>
        {
#if WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = null;
#endif
        });

        DatePickerHandler.Mapper.AppendToMapping("BorderlessDatePicker", (handler, _) =>
        {
#if WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = null;
#endif
        });

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        builder.Services.AddSingleton<IConfiguration>(configuration);

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(configuration);

        builder.Services.AddSingleton<DesktopTokenProvider>();
        builder.Services.AddScoped<IAccessTokenProvider>(sp => sp.GetRequiredService<DesktopTokenProvider>());

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<LoginWindow>();
        builder.Services.AddTransient<RegisterWindow>();
        builder.Services.AddTransient<MainWindow>();

        var app = builder.Build();
        App.Services = app.Services;

        using (var scope = app.Services.CreateScope())
        {
            DatabaseMigration.ExecuteMigrations(scope.ServiceProvider).GetAwaiter().GetResult();
        }

        return app;
    }
}
