using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Desktop.Services;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Desktop.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DesktopTokenProvider _tokenProvider;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    [ObservableProperty]
    private bool _hasSuccess;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool _isLoading;

    public bool IsNotLoading => !IsLoading;

    public event Action<ResponseRegisterUserJson>? LoginSuccessful;
    public event Action? OpenRegisterRequested;

    public LoginViewModel(IServiceScopeFactory scopeFactory, DesktopTokenProvider tokenProvider)
    {
        _scopeFactory = scopeFactory;
        _tokenProvider = tokenProvider;
    }

    [RelayCommand]
    private void OpenRegister()
    {
        OpenRegisterRequested?.Invoke();
    }

    public void SetRegisteredUser(string email)
    {
        Email = email;
        Password = string.Empty;
        HasError = false;
        ErrorMessage = string.Empty;
        SuccessMessage = "Conta criada com sucesso! Digite sua senha para entrar.";
        HasSuccess = true;
    }

    [RelayCommand]
    private async Task Login()
    {
        ErrorMessage = string.Empty;
        HasError = false;
        SuccessMessage = string.Empty;
        HasSuccess = false;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = ResourceMessagesException.VALIDATION_LOGIN_INVALID;
            HasError = true;
            return;
        }

        IsLoading = true;

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var loginUseCase = scope.ServiceProvider.GetRequiredService<ILoginWithEmailAndPasswordUseCase>();

            var response = await loginUseCase.Execute(new RequestLoginJson
            {
                Email = Email.Trim(),
                Password = Password
            });

            _tokenProvider.Token = response.Tokens.AccessToken;
            LoginSuccessful?.Invoke(response);
        }
        catch (InvalidLoginException ex)
        {
            ErrorMessage = ex.GetErrorMessage().FirstOrDefault() ?? ResourceMessagesException.VALIDATION_LOGIN_INVALID;
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
