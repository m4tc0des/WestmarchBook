using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Desktop.Services;
using WestmarchBook.Desktop.Views;

namespace WestmarchBook.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly DesktopTokenProvider _tokenProvider;
    private readonly IServiceScopeFactory _scopeFactory;

    public event Action? SwitchUserRequested;

    [ObservableProperty]
    private string _userName = "ADMINISTRADOR";

    [ObservableProperty]
    private string _welcomeMessage = string.Empty;

    [ObservableProperty]
    private string _serverName = "127.0.0.1";

    [ObservableProperty]
    private string _databaseName = "westmarchbook";

    [ObservableProperty]
    private string _appVersion = "0.1.0";

    [ObservableProperty]
    private string _currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

    [ObservableProperty]
    private bool _isCadastrosSubmenuVisible;

    public MainViewModel(
        IConfiguration configuration,
        DesktopTokenProvider tokenProvider,
        IServiceScopeFactory scopeFactory)
    {
        _tokenProvider = tokenProvider;
        _scopeFactory = scopeFactory;

        AppVersion = LoginWindow.GetAppVersion();
        LoadConnectionMetadata(configuration);
    }

    public void Initialize(string userName)
    {
        UserName = string.IsNullOrWhiteSpace(userName) ? "ADMINISTRADOR" : userName.ToUpperInvariant();
        WelcomeMessage = $"Saudações, {userName}! Bem-vindo ao Westmarch Book.";
    }

    public void UpdateClock()
    {
        CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

    private void LoadConnectionMetadata(IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("DbConnection") ?? string.Empty;
        foreach (var part in connString.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2) continue;

            var key = kv[0].Trim();
            var val = kv[1].Trim();

            if (key.Equals("Server", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("Host", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("Data Source", StringComparison.OrdinalIgnoreCase))
            {
                ServerName = val;
            }
            else if (key.Equals("Database", StringComparison.OrdinalIgnoreCase) ||
                     key.Equals("Initial Catalog", StringComparison.OrdinalIgnoreCase))
            {
                DatabaseName = val;
            }
        }
    }

    [RelayCommand]
    private void SwitchUser()
    {
        _tokenProvider.Token = string.Empty;
        IsCadastrosSubmenuVisible = false;
        IsBooksListModalVisible = false;
        IsBookFormModalVisible = false;
        IsQuickEntityModalVisible = false;
        IsCopyModalVisible = false;
        IsReadersListModalVisible = false;
        IsReaderFormModalVisible = false;
        SwitchUserRequested?.Invoke();
    }

    [RelayCommand]
    private void ToggleCadastrosSubmenu()
    {
        IsCadastrosSubmenuVisible = !IsCadastrosSubmenuVisible;
    }
}
