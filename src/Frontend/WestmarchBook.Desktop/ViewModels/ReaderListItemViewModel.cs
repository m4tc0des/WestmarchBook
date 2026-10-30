using CommunityToolkit.Mvvm.ComponentModel;

namespace WestmarchBook.Desktop.ViewModels;

public partial class ReaderListItemViewModel : ObservableObject
{
    [ObservableProperty]
    private long _id;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    private string _documentNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private bool _isActive = true;

    public string StatusText => IsActive ? "ATIVO" : "INATIVO";
}
