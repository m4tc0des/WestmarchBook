using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.User.Register;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Desktop.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool _isLoading;

    public bool IsNotLoading => !IsLoading;

    public event Action<string>? RegisterSuccessful;
    public event Action? BackRequested;

    public RegisterViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    [RelayCommand]
    private void Back()
    {
        BackRequested?.Invoke();
    }

    [RelayCommand]
    private async Task Register()
    {
        ErrorMessage = string.Empty;
        HasError = false;
        IsLoading = true;

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var registerUseCase = scope.ServiceProvider.GetRequiredService<IRegisterUserAccountUseCase>();

            await registerUseCase.Execute(new RequestRegisterUserJson
            {
                UserName = UserName.Trim(),
                Email = Email.Trim(),
                Password = Password
            });

            RegisterSuccessful?.Invoke(Email.Trim());
        }
        catch (ErrorOnValidationException ex)
        {
            ErrorMessage = string.Join("\n", ex.GetErrorMessage());
            HasError = true;
        }
        catch (WestmarchBookException ex)
        {
            ErrorMessage = string.Join("\n", ex.GetErrorMessage());
            HasError = true;
        }
        catch (System.Exception)
        {
            ErrorMessage = ResourceMessagesException.UNKNOWN_ERROR;
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
