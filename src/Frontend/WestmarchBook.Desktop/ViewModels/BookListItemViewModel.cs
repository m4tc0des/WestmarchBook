using CommunityToolkit.Mvvm.ComponentModel;
using WestmarchBook.Communication.Enums;

namespace WestmarchBook.Desktop.ViewModels;

public partial class BookListItemViewModel : ObservableObject
{
    [ObservableProperty]
    private long _id;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private long _publisherId;

    [ObservableProperty]
    private string _publisherName = string.Empty;

    [ObservableProperty]
    private long _authorId;

    [ObservableProperty]
    private string _authorName = string.Empty;

    [ObservableProperty]
    private DateTime _publicationDate;

    [ObservableProperty]
    private string _genresSummary = string.Empty;

    [ObservableProperty]
    private string _languagesSummary = string.Empty;

    public IList<Genre> Genres { get; set; } = [];

    public IList<Language> Languages { get; set; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private bool _isActive = true;

    public string StatusText => IsActive ? "ATIVO" : "INATIVO";
}
