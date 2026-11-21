using CommunityToolkit.Mvvm.ComponentModel;

namespace WestmarchBook.Desktop.ViewModels;

public partial class PublisherListItemViewModel : ObservableObject
{
    [ObservableProperty]
    private long _id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    [NotifyPropertyChangedFor(nameof(StatusBadgeBg))]
    [NotifyPropertyChangedFor(nameof(StatusBadgeFg))]
    private bool _isActive = true;

    public string StatusText => IsActive ? "ATIVO" : "INATIVO";
    public string StatusBadgeBg => IsActive ? "#DCFCE7" : "#F1F5F9";
    public string StatusBadgeFg => IsActive ? "#15803D" : "#64748B";
}
