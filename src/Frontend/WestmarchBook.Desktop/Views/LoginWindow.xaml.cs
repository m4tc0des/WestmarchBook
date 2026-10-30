using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Desktop.ViewModels;

namespace WestmarchBook.Desktop.Views;

public partial class LoginWindow : ContentPage
{
    private bool _eventsSubscribed;

    public LoginWindow()
    {
        InitializeComponent();
        VersionLabel.Text = $"« V{GetAppVersion()} »";
        Loaded += OnPageLoaded;
        Unloaded += OnPageUnloaded;

#if WINDOWS
        PasswordTextBox.HandlerChanged += (sender, args) =>
        {
            if (PasswordTextBox.Handler?.PlatformView is Microsoft.Maui.Platform.MauiPasswordTextBox pwdBox)
            {
                pwdBox.IsObfuscationDelayed = false;
            }
        };

        Loaded += (sender, args) =>
        {
            if (PasswordTextBox.Handler?.PlatformView is Microsoft.Maui.Platform.MauiPasswordTextBox pwdBox)
            {
                pwdBox.IsObfuscationDelayed = false;
            }
        };
#endif
    }

    public LoginWindow(LoginViewModel viewModel) : this()
    {
        BindingContext = viewModel;
        SubscribeViewModelEvents(viewModel);
    }

    private void SubscribeViewModelEvents(LoginViewModel viewModel)
    {
        if (_eventsSubscribed) return;

        viewModel.LoginSuccessful += OnLoginSuccessful;
        viewModel.OpenRegisterRequested += OnOpenRegisterRequested;
        _eventsSubscribed = true;
    }

    private void UnsubscribeViewModelEvents(LoginViewModel viewModel)
    {
        if (!_eventsSubscribed) return;

        viewModel.LoginSuccessful -= OnLoginSuccessful;
        viewModel.OpenRegisterRequested -= OnOpenRegisterRequested;
        _eventsSubscribed = false;
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (BindingContext is LoginViewModel vm)
        {
            SubscribeViewModelEvents(vm);
        }
    }

    private void OnPageUnloaded(object? sender, EventArgs e)
    {
        if (BindingContext is LoginViewModel vm)
        {
            UnsubscribeViewModelEvents(vm);
        }
    }

    public static string GetAppVersion()
    {
        try
        {
            var version = AppInfo.Current.VersionString;
            if (!string.IsNullOrWhiteSpace(version))
            {
                return version;
            }
        }
        catch
        {
        }

        return typeof(LoginWindow).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion.Split('+')[0] ?? "0.1.0";
    }

    private void OnInputFocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.Parent is Border border)
        {
            border.Stroke = Color.FromArgb("#D4AF37");
            border.BackgroundColor = Color.FromArgb("#2D171B");
        }
    }

    private void OnInputUnfocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.Parent is Border border)
        {
            border.Stroke = Color.FromArgb("#6B4423");
            border.BackgroundColor = Color.FromArgb("#241316");
        }
    }

    private void OnEmailCompleted(object? sender, EventArgs e)
    {
        Dispatcher.Dispatch(() => PasswordTextBox.Focus());
    }

    private void OnPasswordCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
        {
            vm.LoginCommand.Execute(null);
        }
    }

    private void OnOpenRegisterRequested()
    {
        var window = Window ?? Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
        if (window is null) return;

        var registerWindow = App.Services.GetRequiredService<RegisterWindow>();

        void OnRegisterSuccessful(string registeredEmail)
        {
            registerWindow.ViewModel.RegisterSuccessful -= OnRegisterSuccessful;
            registerWindow.ViewModel.BackRequested -= OnBackRequested;

            if (BindingContext is LoginViewModel loginVm)
            {
                loginVm.SetRegisteredUser(registeredEmail);
            }

            window.Page = this;
            App.ConfigureFixedWindow(window, "Westmarch - Sistema de Gestao de Livros", 440, 640);
            Dispatcher.Dispatch(() => PasswordTextBox.Focus());
        }

        void OnBackRequested()
        {
            registerWindow.ViewModel.RegisterSuccessful -= OnRegisterSuccessful;
            registerWindow.ViewModel.BackRequested -= OnBackRequested;

            window.Page = this;
            App.ConfigureFixedWindow(window, "Westmarch - Sistema de Gestao de Livros", 440, 640);
        }

        registerWindow.ViewModel.RegisterSuccessful += OnRegisterSuccessful;
        registerWindow.ViewModel.BackRequested += OnBackRequested;

        window.Page = registerWindow;
        App.ConfigureFixedWindow(window, "Westmarch Book - Registro", 440, 640);
    }

    private void OnLoginSuccessful(ResponseRegisterUserJson userResponse)
    {
        if (BindingContext is LoginViewModel loginVm)
        {
            UnsubscribeViewModelEvents(loginVm);
        }

        var currentWindow = Window ?? Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
        if (currentWindow is null) return;

        var existingMainWindow = Microsoft.Maui.Controls.Application.Current?.Windows
            .FirstOrDefault(w => w.Page is MainWindow);

        if (existingMainWindow?.Page is MainWindow existingMain && existingMainWindow != currentWindow)
        {
            if (existingMain.BindingContext is MainViewModel existingVm)
            {
                existingVm.Initialize(userResponse.UserName);
            }

            App.AllowLoginClose = true;
#if WINDOWS
            if (currentWindow.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWin)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWin);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWin = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                appWin.Closing -= App.OnAppWindowClosing;
            }
#endif
            Microsoft.Maui.Controls.Application.Current?.CloseWindow(currentWindow);
            App.AllowLoginClose = false;
            return;
        }

        var mainWindow = App.Services.GetRequiredService<MainWindow>();

        if (mainWindow.BindingContext is MainViewModel mainVm)
        {
            mainVm.Initialize(userResponse.UserName);
        }

        var mainAppWindow = new Window(mainWindow);
        mainAppWindow.HandlerChanged += (_, _) => App.ConfigureMaximizedWindow(mainAppWindow);
        if (mainAppWindow.Handler?.PlatformView is not null)
        {
            App.ConfigureMaximizedWindow(mainAppWindow);
        }

        App.AllowLoginClose = true;
#if WINDOWS
        if (currentWindow.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWinInitial)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWinInitial);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWin = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWin.Closing -= App.OnAppWindowClosing;
        }
#endif
        Microsoft.Maui.Controls.Application.Current?.OpenWindow(mainAppWindow);
        Microsoft.Maui.Controls.Application.Current?.CloseWindow(currentWindow);
        App.AllowLoginClose = false;
    }
}
