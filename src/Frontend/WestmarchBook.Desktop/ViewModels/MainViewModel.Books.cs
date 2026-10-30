using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.Author.GetAll;
using WestmarchBook.Application.UseCases.Book.ActiveById;
using WestmarchBook.Application.UseCases.Book.DeleteById;
using WestmarchBook.Application.UseCases.Book.GetAll;
using WestmarchBook.Application.UseCases.Book.GetById;
using WestmarchBook.Application.UseCases.Book.InactiveById;
using WestmarchBook.Application.UseCases.Book.Register;
using WestmarchBook.Application.UseCases.Book.Update;
using WestmarchBook.Application.UseCases.Publisher.GetAll;
using WestmarchBook.Communication.Enums;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Desktop.ViewModels;

public partial class MainViewModel
{
    [ObservableProperty]
    private bool _isBooksListModalVisible;

    [ObservableProperty]
    private BookListItemViewModel? _selectedBook;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _searchByCode = true;

    [ObservableProperty]
    private string _listStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasListStatusMessage;

    [ObservableProperty]
    private bool _isListStatusError;

    [ObservableProperty]
    private bool _isBookFormModalVisible;

    [ObservableProperty]
    private string _bookFormTitle = ResourceMessagesException.BOOK_FORM_TITLE_CREATE;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _bookCode = string.Empty;

    [ObservableProperty]
    private string _bookTitle = string.Empty;

    [ObservableProperty]
    private string _bookDescription = string.Empty;

    [ObservableProperty]
    private EntityOptionItem? _selectedAuthorOption;

    [ObservableProperty]
    private EntityOptionItem? _selectedPublisherOption;

    [ObservableProperty]
    private string _publicationDateText = string.Empty;

    [ObservableProperty]
    private DateTime _publicationDate = DateTime.Today;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPublicationDateEmpty))]
    private bool _hasPublicationDate;

    public bool IsPublicationDateEmpty => !HasPublicationDate;

    [ObservableProperty]
    private GenreOptionItem? _selectedGenreToAdd;

    [ObservableProperty]
    private LanguageOptionItem? _selectedLanguageToAdd;

    [ObservableProperty]
    private string _formErrorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasFormError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormNotLoading))]
    private bool _isFormLoading;

    public bool IsFormNotLoading => !IsFormLoading;

    public ObservableCollection<string> BookRegistrationNumbers { get; } = [];

    public bool HasNoBookRegistrationNumbers => BookRegistrationNumbers.Count == 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoBookRegistrationNumbers))]
    [NotifyPropertyChangedFor(nameof(CopySummaryText))]
    private bool _isCopyModalVisible;

    [ObservableProperty]
    private string _copyRegistrationNumberInput = string.Empty;

    [ObservableProperty]
    private string _copyFeedbackMessage = string.Empty;

    [ObservableProperty]
    private bool _hasCopyFeedbackMessage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CopyFeedbackBackgroundColor))]
    [NotifyPropertyChangedFor(nameof(CopyFeedbackStrokeColor))]
    [NotifyPropertyChangedFor(nameof(CopyFeedbackTextColor))]
    private bool _isCopyFeedbackError;

    public Color CopyFeedbackBackgroundColor => IsCopyFeedbackError ? Color.FromArgb("#3D1418") : Color.FromArgb("#1A2D1B");
    public Color CopyFeedbackStrokeColor => IsCopyFeedbackError ? Color.FromArgb("#E57373") : Color.FromArgb("#85C989");
    public Color CopyFeedbackTextColor => IsCopyFeedbackError ? Color.FromArgb("#FFB4B4") : Color.FromArgb("#C8F2CA");

    public string CopySummaryText => $"Total de exemplares cadastrados: {BookRegistrationNumbers.Count}";

    public ObservableCollection<BookListItemViewModel> Books { get; } = [];
    public ObservableCollection<EntityOptionItem> AvailableAuthors { get; } = [];
    public ObservableCollection<EntityOptionItem> AvailablePublishers { get; } = [];
    public ObservableCollection<GenreOptionItem> SelectedGenres { get; } = [];
    public ObservableCollection<LanguageOptionItem> SelectedLanguages { get; } = [];

    public IReadOnlyList<GenreOptionItem> AvailableGenres { get; } =
        Enum.GetValues<Genre>().Select(g => new GenreOptionItem(g, GetGenreNamePtBr(g))).ToList();

    public IReadOnlyList<LanguageOptionItem> AvailableLanguages { get; } =
        Enum.GetValues<Language>().Select(l => new LanguageOptionItem(l, GetLanguageNamePtBr(l))).ToList();

    public static string GetGenreNamePtBr(Genre genre) => genre switch
    {
        Genre.Fiction => "Ficção",
        Genre.Fantasy => "Fantasia",
        Genre.ScienceFiction => "Ficção Científica",
        Genre.Thriller => "Suspense",
        Genre.Romance => "Romance",
        Genre.DarkRomance => "Dark Romance",
        Genre.Horror => "Terror",
        Genre.Biography => "Biografia",
        Genre.History => "História",
        Genre.SelfHelp => "Autoajuda",
        Genre.Science => "Ciência",
        Genre.Philosophy => "Filosofia",
        Genre.Poetry => "Poesia",
        Genre.Adventure => "Aventura",
        Genre.Dystopian => "Distopia",
        Genre.Classic => "Clássico",
        Genre.Action => "Ação",
        _ => genre.ToString()
    };

    public static string GetLanguageNamePtBr(Language language) => language switch
    {
        Language.English => "Inglês",
        Language.Spanish => "Espanhol",
        Language.French => "Francês",
        Language.German => "Alemão",
        Language.Italian => "Italiano",
        Language.BrazilianPortuguese => "Português (Brasil)",
        Language.Portuguese => "Português (Portugal)",
        Language.Russian => "Russo",
        Language.Japanese => "Japonês",
        Language.Chinese => "Chinês",
        Language.Arabic => "Árabe",
        _ => language.ToString()
    };

    public DateTime MaximumPublicationDate { get; } = DateTime.Today;

    [RelayCommand]
    private void OpenBooksModal()
    {
        IsCadastrosSubmenuVisible = false;
        ClearListStatus();
        IsBooksListModalVisible = true;
    }

    [RelayCommand]
    private void CloseBooksModal()
    {
        IsBooksListModalVisible = false;
        IsBookFormModalVisible = false;
        IsQuickEntityModalVisible = false;
        ClearListStatus();
    }

    private static BookListItemViewModel MapToBookListItem(ResponseBookJson book)
    {
        var firstAuthor = book.Authors.FirstOrDefault();
        var parsedGenres = book.Genres
            .Select(g => Enum.TryParse<Genre>(g, true, out var parsed) ? (Genre?)parsed : null)
            .Where(g => g.HasValue)
            .Select(g => g!.Value)
            .ToList();
        var parsedLanguages = book.Languages
            .Select(l => Enum.TryParse<Language>(l, true, out var parsed) ? (Language?)parsed : null)
            .Where(l => l.HasValue)
            .Select(l => l!.Value)
            .ToList();

        return new BookListItemViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            PublisherId = book.Publisher?.Id ?? 0,
            PublisherName = book.Publisher?.Name ?? string.Empty,
            AuthorId = firstAuthor?.Id ?? 0,
            AuthorName = book.Authors.Count > 0
                ? string.Join(", ", book.Authors.Select(a => a.Name).Where(n => !string.IsNullOrEmpty(n)))
                : string.Empty,
            PublicationDate = book.PublicationDate,
            Genres = parsedGenres,
            Languages = parsedLanguages,
            GenresSummary = string.Join(", ", parsedGenres.Select(GetGenreNamePtBr)),
            LanguagesSummary = string.Join(", ", parsedLanguages.Select(GetLanguageNamePtBr)),
            IsActive = book.Active
        };
    }

    [RelayCommand]
    private async Task SearchBook()
    {
        ClearListStatus();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var getAllBooksUseCase = scope.ServiceProvider.GetRequiredService<IGetAllBooksUseCase>();
                var allBooks = await getAllBooksUseCase.Execute();

                Books.Clear();
                foreach (var book in allBooks)
                {
                    Books.Add(MapToBookListItem(book));
                }

                if (Books.Count > 0)
                {
                    SelectedBook = Books[0];
                    SetListStatus($"Listando todos os livros cadastrados ({Books.Count} encontrado(s)).", isError: false);
                }
                else
                {
                    SetListStatus("Nenhum livro cadastrado encontrado.", isError: false);
                }
            }
            catch (System.Exception ex)
            {
                SetListStatus(ex.Message, isError: true);
            }
            return;
        }

        if (!long.TryParse(SearchText.Trim(), out var bookId) || bookId <= 0)
        {
            var matchByTitle = Books.FirstOrDefault(b => b.Title.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase));
            if (matchByTitle is not null)
            {
                SelectedBook = matchByTitle;
                SetListStatus($"Livro '{matchByTitle.Title}' selecionado.", isError: false);
                return;
            }

            SetListStatus("Informe um Código (ID numérico) válido para buscar na base.", isError: true);
            return;
        }

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var getBookByIdUseCase = scope.ServiceProvider.GetRequiredService<IGetBookByIdUseCase>();
            var book = await getBookByIdUseCase.Execute(bookId);

            var existing = Books.FirstOrDefault(b => b.Id == book.Id);
            if (existing is not null)
            {
                Books.Remove(existing);
            }

            var item = MapToBookListItem(book);
            Books.Insert(0, item);
            SelectedBook = item;
            SetListStatus($"Livro #{book.Id} '{book.Title}' carregado com sucesso.", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            SetListStatus(string.Join("\n", ex.GetErrorMessage()), isError: true);
        }
        catch (System.Exception)
        {
            SetListStatus(ResourceMessagesException.UNKNOWN_ERROR, isError: true);
        }
    }

    private async Task LoadBookFormLookupsAsync(bool fetchNextBookId)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            if (fetchNextBookId)
            {
                var getAllBooksUseCase = scope.ServiceProvider.GetRequiredService<IGetAllBooksUseCase>();
                var nextId = await getAllBooksUseCase.GetNextBookId();
                BookCode = nextId.ToString();
            }

            var getAllAuthorsUseCase = scope.ServiceProvider.GetRequiredService<IGetAllAuthorsUseCase>();
            var authors = await getAllAuthorsUseCase.Execute();
            AvailableAuthors.Clear();
            foreach (var author in authors)
            {
                AvailableAuthors.Add(new EntityOptionItem(author.Id, author.Name));
            }

            var getAllPublishersUseCase = scope.ServiceProvider.GetRequiredService<IGetAllPublishersUseCase>();
            var publishers = await getAllPublishersUseCase.Execute();
            AvailablePublishers.Clear();
            foreach (var publisher in publishers)
            {
                AvailablePublishers.Add(new EntityOptionItem(publisher.Id, publisher.Name));
            }
        }
        catch
        {
            if (fetchNextBookId && string.IsNullOrWhiteSpace(BookCode))
            {
                var localMax = Books.Count > 0 ? Books.Max(b => b.Id) : 0;
                BookCode = (localMax + 1).ToString();
            }
        }
    }

    [RelayCommand]
    private async Task OpenIncludeBookModal()
    {
        ClearListStatus();
        IsEditMode = false;
        BookFormTitle = ResourceMessagesException.BOOK_FORM_TITLE_CREATE;
        BookCode = "1";
        BookTitle = string.Empty;
        BookDescription = string.Empty;
        SelectedAuthorOption = null;
        SelectedPublisherOption = null;
        PublicationDate = DateTime.Today;
        PublicationDateText = string.Empty;
        HasPublicationDate = false;
        SelectedGenreToAdd = null;
        SelectedLanguageToAdd = null;
        FormErrorMessage = string.Empty;
        HasFormError = false;

        SelectedGenres.Clear();
        SelectedLanguages.Clear();
        BookRegistrationNumbers.Clear();
        OnPropertyChanged(nameof(HasNoBookRegistrationNumbers));
        OnPropertyChanged(nameof(CopySummaryText));
        IsCopyModalVisible = false;

        IsBookFormModalVisible = true;
        await LoadBookFormLookupsAsync(fetchNextBookId: true);
    }

    [RelayCommand]
    private async Task OpenEditBookModal()
    {
        ClearListStatus();

        if (SelectedBook is null)
        {
            SetListStatus("Selecione um livro na listagem para alterar.", isError: true);
            return;
        }

        IsEditMode = true;
        BookFormTitle = ResourceMessagesException.BOOK_FORM_TITLE_EDIT;
        BookCode = SelectedBook.Id.ToString();
        BookTitle = SelectedBook.Title;
        BookDescription = SelectedBook.Description;
        PublicationDate = SelectedBook.PublicationDate;
        PublicationDateText = SelectedBook.PublicationDate.ToString("dd/MM/yyyy");
        HasPublicationDate = true;
        FormErrorMessage = string.Empty;
        HasFormError = false;

        SelectedGenres.Clear();
        foreach (var genre in SelectedBook.Genres)
        {
            var option = AvailableGenres.FirstOrDefault(g => g.Value == genre);
            if (option is not null && !SelectedGenres.Contains(option))
            {
                SelectedGenres.Add(option);
            }
        }

        SelectedLanguages.Clear();
        var firstLanguage = SelectedBook.Languages.FirstOrDefault();
        if (SelectedBook.Languages.Count > 0)
        {
            var langOption = AvailableLanguages.FirstOrDefault(l => l.Value == firstLanguage);
            if (langOption is not null)
            {
                SelectedLanguages.Add(langOption);
                SelectedLanguageToAdd = langOption;
            }
        }

        IsBookFormModalVisible = true;
        await LoadBookFormLookupsAsync(fetchNextBookId: false);

        SelectedAuthorOption = AvailableAuthors.FirstOrDefault(a => a.Id == SelectedBook.AuthorId);
        SelectedPublisherOption = AvailablePublishers.FirstOrDefault(p => p.Id == SelectedBook.PublisherId);
    }

    [RelayCommand]
    private async Task ActivateBook()
    {
        ClearListStatus();

        if (SelectedBook is null)
        {
            SetListStatus("Selecione um livro na listagem para ativar.", isError: true);
            return;
        }

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var activeBookUseCase = scope.ServiceProvider.GetRequiredService<IActiveBookByIdUseCase>();
            await activeBookUseCase.Execute(SelectedBook.Id);

            SelectedBook.IsActive = true;
            SetListStatus($"Livro #{SelectedBook.Id} '{SelectedBook.Title}' foi ATIVADO.", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            SetListStatus(string.Join("\n", ex.GetErrorMessage()), isError: true);
        }
        catch (System.Exception)
        {
            SetListStatus(ResourceMessagesException.UNKNOWN_ERROR, isError: true);
        }
    }

    [RelayCommand]
    private async Task DeactivateBook()
    {
        ClearListStatus();

        if (SelectedBook is null)
        {
            SetListStatus("Selecione um livro na listagem para desativar.", isError: true);
            return;
        }

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var inactiveBookUseCase = scope.ServiceProvider.GetRequiredService<IInactiveBookByIdUseCase>();
            await inactiveBookUseCase.Execute(SelectedBook.Id);

            SelectedBook.IsActive = false;
            SetListStatus($"Livro #{SelectedBook.Id} '{SelectedBook.Title}' foi INATIVADO.", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            SetListStatus(string.Join("\n", ex.GetErrorMessage()), isError: true);
        }
        catch (System.Exception)
        {
            SetListStatus(ResourceMessagesException.UNKNOWN_ERROR, isError: true);
        }
    }

    [RelayCommand]
    private async Task ToggleActiveBook()
    {
        if (SelectedBook is { IsActive: true })
        {
            await DeactivateBook();
        }
        else
        {
            await ActivateBook();
        }
    }

    public async Task DeleteSelectedBookConfirmedAsync()
    {
        ClearListStatus();

        if (SelectedBook is null)
        {
            SetListStatus("Selecione um livro na listagem para deletar.", isError: true);
            return;
        }

        try
        {
            var id = SelectedBook.Id;
            var title = SelectedBook.Title;

            await using var scope = _scopeFactory.CreateAsyncScope();
            var deleteBookUseCase = scope.ServiceProvider.GetRequiredService<IDeleteBookByIdUseCase>();
            await deleteBookUseCase.Execute(id);

            Books.Remove(SelectedBook);
            SelectedBook = Books.FirstOrDefault();
            SetListStatus($"Livro #{id} '{title}' removido com sucesso.", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            SetListStatus(string.Join("\n", ex.GetErrorMessage()), isError: true);
        }
        catch (System.Exception)
        {
            SetListStatus(ResourceMessagesException.UNKNOWN_ERROR, isError: true);
        }
    }

    [RelayCommand]
    private void AddGenre()
    {
        if (SelectedGenreToAdd is { } genre && !SelectedGenres.Contains(genre))
        {
            SelectedGenres.Add(genre);
        }
    }

    [RelayCommand]
    private void RemoveGenre(GenreOptionItem genre)
    {
        SelectedGenres.Remove(genre);
    }

    [RelayCommand]
    private void AddLanguage()
    {
        if (SelectedLanguageToAdd is { } language)
        {
            SelectedLanguages.Clear();
            SelectedLanguages.Add(language);
        }
    }

    [RelayCommand]
    private void RemoveLanguage(LanguageOptionItem language)
    {
        SelectedLanguages.Remove(language);
    }

    private static bool TryParsePublicationDate(string input, out DateTime parsedDate)
    {
        var formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "ddMMyyyy", "yyyy-MM-dd" };
        return DateTime.TryParseExact(
            input.Trim(),
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out parsedDate);
    }

    [RelayCommand]
    private async Task SaveBook()
    {
        FormErrorMessage = string.Empty;
        HasFormError = false;

        if (!TryParsePublicationDate(PublicationDateText, out var parsedDate))
        {
            FormErrorMessage = "Informe uma Data de Publicação válida no formato dd/mm/aaaa.";
            HasFormError = true;
            return;
        }

        PublicationDate = parsedDate;
        HasPublicationDate = true;

        var authorOption = SelectedAuthorOption;
        if (authorOption is null)
        {
            FormErrorMessage = ResourceMessagesException.VALIDATION_BOOK_AUTHORS_REQUIRED;
            HasFormError = true;
            return;
        }

        var publisherOption = SelectedPublisherOption;
        if (publisherOption is null)
        {
            FormErrorMessage = ResourceMessagesException.VALIDATION_BOOK_PUBLISHER_REQUIRED;
            HasFormError = true;
            return;
        }

        if (SelectedGenreToAdd is { } pendingGenre && !SelectedGenres.Contains(pendingGenre) && SelectedGenres.Count == 0)
        {
            SelectedGenres.Add(pendingGenre);
        }

        if (SelectedLanguageToAdd is { } pendingLanguage && SelectedLanguages.Count == 0)
        {
            SelectedLanguages.Add(pendingLanguage);
        }

        if (!IsEditMode && BookRegistrationNumbers.Count == 0)
        {
            FormErrorMessage = ResourceMessagesException.VALIDATION_BOOK_REGISTRATIONNUMBER_REQUIRED;
            HasFormError = true;
            return;
        }

        IsFormLoading = true;

        try
        {
            var trimmedTitle = BookTitle.Trim();
            var description = string.IsNullOrWhiteSpace(BookDescription) ? trimmedTitle : BookDescription.Trim();

            await using var scope = _scopeFactory.CreateAsyncScope();

            if (IsEditMode)
            {
                if (SelectedBook is null || !long.TryParse(BookCode, out var bookId))
                {
                    FormErrorMessage = ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND;
                    HasFormError = true;
                    return;
                }

                var updateRequest = new RequestUpdateBookJson
                {
                    Title = trimmedTitle,
                    Description = description,
                    PublicationDate = parsedDate.Date,
                    PublisherId = publisherOption.Id,
                    Authors = [authorOption.Id],
                    Genres = SelectedGenres.Select(g => g.Value).Distinct().ToList(),
                    Languages = SelectedLanguages.Take(1).Select(l => l.Value).ToList(),
                    RegistrationNumbers = BookRegistrationNumbers.ToList()
                };

                var updateBookUseCase = scope.ServiceProvider.GetRequiredService<IUpdateBookUseCase>();
                await updateBookUseCase.Execute(bookId, updateRequest);

                SelectedBook.Title = updateRequest.Title;
                SelectedBook.Description = updateRequest.Description;
                SelectedBook.PublicationDate = updateRequest.PublicationDate;
                SelectedBook.PublisherId = publisherOption.Id;
                SelectedBook.PublisherName = publisherOption.Name;
                SelectedBook.AuthorId = authorOption.Id;
                SelectedBook.AuthorName = authorOption.Name;
                SelectedBook.Genres = updateRequest.Genres.ToList();
                SelectedBook.Languages = updateRequest.Languages.ToList();
                SelectedBook.GenresSummary = string.Join(", ", updateRequest.Genres.Select(GetGenreNamePtBr));
                SelectedBook.LanguagesSummary = string.Join(", ", updateRequest.Languages.Select(GetLanguageNamePtBr));

                IsBookFormModalVisible = false;
                SetListStatus($"Livro #{bookId} '{updateRequest.Title}' alterado com sucesso!", isError: false);
                return;
            }

            var request = new RequestRegisterBookJson
            {
                Title = trimmedTitle,
                Description = description,
                PublicationDate = parsedDate.Date,
                PublisherId = publisherOption.Id,
                Authors = [authorOption.Id],
                Genres = SelectedGenres.Select(g => g.Value).Distinct().ToList(),
                Languages = SelectedLanguages.Take(1).Select(l => l.Value).ToList(),
                RegistrationNumbers = BookRegistrationNumbers.ToList()
            };

            var registerBookUseCase = scope.ServiceProvider.GetRequiredService<IRegisterBookUseCase>();
            var response = await registerBookUseCase.Execute(request);

            if (response is null)
            {
                FormErrorMessage = ResourceMessagesException.UNKNOWN_ERROR;
                HasFormError = true;
                return;
            }

            BookCode = response.Id.ToString();

            var newItem = new BookListItemViewModel
            {
                Id = response.Id,
                Title = response.Title,
                Description = request.Description,
                PublisherId = publisherOption.Id,
                PublisherName = publisherOption.Name,
                AuthorId = authorOption.Id,
                AuthorName = authorOption.Name,
                PublicationDate = request.PublicationDate,
                Genres = request.Genres.ToList(),
                Languages = request.Languages.ToList(),
                GenresSummary = string.Join(", ", request.Genres.Select(GetGenreNamePtBr)),
                LanguagesSummary = string.Join(", ", request.Languages.Select(GetLanguageNamePtBr)),
                IsActive = true
            };

            Books.Insert(0, newItem);
            SelectedBook = newItem;
            IsBookFormModalVisible = false;
            BookRegistrationNumbers.Clear();
            OnPropertyChanged(nameof(HasNoBookRegistrationNumbers));
            OnPropertyChanged(nameof(CopySummaryText));
            SetListStatus($"Livro #{response.Id} '{response.Title}' Inclus�o com sucesso!", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            FormErrorMessage = string.Join("\n", ex.GetErrorMessage());
            HasFormError = true;
        }
        catch (System.Exception)
        {
            FormErrorMessage = ResourceMessagesException.UNKNOWN_ERROR;
            HasFormError = true;
        }
        finally
        {
            IsFormLoading = false;
        }
    }

    [RelayCommand]
    private void OpenCopyModal()
    {
        CopyRegistrationNumberInput = string.Empty;
        CopyFeedbackMessage = string.Empty;
        HasCopyFeedbackMessage = false;
        IsCopyFeedbackError = false;
        IsCopyModalVisible = true;
    }

    [RelayCommand]
    private void CloseCopyModal()
    {
        IsCopyModalVisible = false;
        CopyRegistrationNumberInput = string.Empty;
        CopyFeedbackMessage = string.Empty;
        HasCopyFeedbackMessage = false;
        OnPropertyChanged(nameof(HasNoBookRegistrationNumbers));
    }

    [RelayCommand]
    private void AddCopyRegistrationNumber()
    {
        var reg = CopyRegistrationNumberInput?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(reg))
        {
            CopyFeedbackMessage = ResourceMessagesException.VALIDATION_BOOK_REGISTRATIONNUMBER_REQUIRED;
            IsCopyFeedbackError = true;
            HasCopyFeedbackMessage = true;
            return;
        }

        if (BookRegistrationNumbers.Any(r => string.Equals(r, reg, StringComparison.OrdinalIgnoreCase)))
        {
            CopyFeedbackMessage = $"O exemplar '{reg}' já foi incluído na lista deste livro.";
            IsCopyFeedbackError = true;
            HasCopyFeedbackMessage = true;
            return;
        }

        BookRegistrationNumbers.Add(reg);
        CopyRegistrationNumberInput = string.Empty;
        CopyFeedbackMessage = $"Exemplar '{reg}' incluído com sucesso!";
        IsCopyFeedbackError = false;
        HasCopyFeedbackMessage = true;
        OnPropertyChanged(nameof(HasNoBookRegistrationNumbers));
        OnPropertyChanged(nameof(CopySummaryText));
    }

    [RelayCommand]
    private void RemoveCopyRegistrationNumber(string reg)
    {
        if (BookRegistrationNumbers.Contains(reg))
        {
            BookRegistrationNumbers.Remove(reg);
            OnPropertyChanged(nameof(HasNoBookRegistrationNumbers));
            OnPropertyChanged(nameof(CopySummaryText));
        }
    }

    [RelayCommand]
    private void CloseBookFormModal()
    {
        IsBookFormModalVisible = false;
        FormErrorMessage = string.Empty;
        HasFormError = false;
    }

    private void ClearListStatus()
    {
        ListStatusMessage = string.Empty;
        HasListStatusMessage = false;
        IsListStatusError = false;
    }

    private void SetListStatus(string message, bool isError)
    {
        ListStatusMessage = message;
        IsListStatusError = isError;
        HasListStatusMessage = true;
    }
}
