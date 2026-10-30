using WestmarchBook.Desktop.ViewModels;

namespace WestmarchBook.Desktop.Views;

public partial class RegisterWindow : ContentPage
{
    public RegisterViewModel ViewModel => (RegisterViewModel)BindingContext!;

    public RegisterWindow()
    {
        InitializeComponent();

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

    public RegisterWindow(RegisterViewModel viewModel) : this()
    {
        BindingContext = viewModel;
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

    private void OnUserNameCompleted(object? sender, EventArgs e)
    {
        Dispatcher.Dispatch(() => EmailTextBox.Focus());
    }

    private void OnEmailCompleted(object? sender, EventArgs e)
    {
        Dispatcher.Dispatch(() => PasswordTextBox.Focus());
    }

    private void OnPasswordCompleted(object? sender, EventArgs e)
    {
        if (ViewModel.RegisterCommand.CanExecute(null))
        {
            ViewModel.RegisterCommand.Execute(null);
        }
    }
}
