using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.Author.DeleteById;
using WestmarchBook.Application.UseCases.Author.Register;
using WestmarchBook.Application.UseCases.Author.Update;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;
using IChangeAuthorIllustrationUseCase = WestmarchBook.Application.UseCases.Author.ChangeIllustration.IChangeIllustrationUseCase;
using IDeleteAuthorIllustrationUseCase = WestmarchBook.Application.UseCases.Author.DeleteIllustration.IDeleteIllustrationUseCase;

namespace WestmarchBook.Desktop.ViewModels;

public partial class MainViewModel
{
    private readonly List<AuthorListItemViewModel> _allAuthorsCache = [];
    private byte[]? _pendingAuthorPhotoBytes;
    private string? _pendingAuthorPreviewTempPath;

    [ObservableProperty]
    private AuthorListItemViewModel? _selectedAuthor;

    [ObservableProperty]
    private string _authorScreenSearchQuery = string.Empty;

    [ObservableProperty]
    private string _authorScreenStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasAuthorScreenStatusMessage;

    [ObservableProperty]
    private bool _isAuthorScreenStatusError;

    [ObservableProperty]
    private bool _isAuthorFormModalVisible;

    [ObservableProperty]
    private bool _isAuthorFormEditMode;

    [ObservableProperty]
    private string _authorFormModalTitle = "Incluir Novo Autor";

    [ObservableProperty]
    private string _authorFormCode = "0001";

    [ObservableProperty]
    private string _authorFormName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAuthorFormPhotoPreview))]
    private string? _authorFormPhotoPreviewPath;

    public bool HasAuthorFormPhotoPreview => !string.IsNullOrWhiteSpace(AuthorFormPhotoPreviewPath);

    [ObservableProperty]
    private string _authorFormErrorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasAuthorFormError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAuthorFormNotLoading))]
    private bool _isAuthorFormLoading;

    public bool IsAuthorFormNotLoading => !IsAuthorFormLoading;

    [ObservableProperty]
    private bool _isAuthorPhotoViewerModalVisible;

    [ObservableProperty]
    private string _authorPhotoViewerTitle = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAuthorPhotoViewerImage))]
    [NotifyPropertyChangedFor(nameof(HasNoAuthorPhotoViewerImage))]
    private string? _authorPhotoViewerImagePath;

    public bool HasAuthorPhotoViewerImage => !string.IsNullOrWhiteSpace(AuthorPhotoViewerImagePath);
    public bool HasNoAuthorPhotoViewerImage => !HasAuthorPhotoViewerImage;

    public ObservableCollection<AuthorListItemViewModel> Authors { get; } = [];

    public async Task LoadAuthorsScreenAsync()
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var authorRepository = scope.ServiceProvider.GetRequiredService<IAuthorReadOnlyRepository>();
            var authors = await authorRepository.GetAllActive();

            var items = authors
                .OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(a => a.Id)
                .Select(a => new AuthorListItemViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsActive = a.Active,
                    PhotoPath = a.HasImage ? _localMediaService.GetExistingAuthorIllustrationPath(a.Id) : null
                })
                .ToList();

            var options = items
                .Select(a => new EntityOptionItem(a.Id, a.Name))
                .ToList();

            DispatchToUi(() =>
            {
                _allAuthorsCache.Clear();
                Authors.Clear();
                AvailableAuthors.Clear();

                foreach (var item in items)
                {
                    _allAuthorsCache.Add(item);
                    Authors.Add(item);
                }

                foreach (var option in options)
                {
                    AvailableAuthors.Add(option);
                }

                SelectedAuthor = Authors.FirstOrDefault();
            });
        }
        catch (System.Exception)
        {
        }
    }

    [RelayCommand]
    private async Task SearchAuthorsScreen()
    {
        HasAuthorScreenStatusMessage = false;

        if (_allAuthorsCache.Count == 0)
        {
            await LoadAuthorsScreenAsync();
        }

        var term = AuthorScreenSearchQuery?.Trim() ?? string.Empty;

        var filtered = (string.IsNullOrWhiteSpace(term)
            ? _allAuthorsCache
            : _allAuthorsCache.Where(a =>
                a.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                a.Id.ToString().Equals(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(a => a.Id)
            .ToList();

        Authors.Clear();
        foreach (var item in filtered)
        {
            Authors.Add(item);
        }

        SelectedAuthor = Authors.FirstOrDefault();
        AuthorScreenStatusMessage = $"{filtered.Count} autor(es) exibido(s).";
        IsAuthorScreenStatusError = false;
        HasAuthorScreenStatusMessage = true;
    }

    [RelayCommand]
    private async Task ClearAuthorScreenSearch()
    {
        AuthorScreenSearchQuery = string.Empty;
        HasAuthorScreenStatusMessage = false;

        if (_allAuthorsCache.Count == 0)
        {
            await LoadAuthorsScreenAsync();
        }

        Authors.Clear();
        foreach (var item in _allAuthorsCache.OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase).ThenBy(a => a.Id))
        {
            Authors.Add(item);
        }

        SelectedAuthor = Authors.FirstOrDefault();
    }

    [RelayCommand]
    private void OpenAddAuthorScreenModal()
    {
        HasAuthorScreenStatusMessage = false;
        IsAuthorFormEditMode = false;
        AuthorFormModalTitle = "Incluir Novo Autor";
        var nextId = (_allAuthorsCache.Count > 0 ? _allAuthorsCache.Max(a => a.Id) : 0) + 1;
        AuthorFormCode = nextId.ToString("0000");
        ResetAuthorFormState();
        IsAuthorFormModalVisible = true;
        OnFocusAuthorFormNameRequested?.Invoke();
    }

    [RelayCommand]
    private void OpenEditAuthorScreenModal()
    {
        HasAuthorScreenStatusMessage = false;

        if (SelectedAuthor is null)
        {
            SetAuthorScreenStatus("Selecione um autor na listagem antes de clicar em Editar.", isError: true);
            return;
        }

        ResetAuthorFormState();
        IsAuthorFormEditMode = true;
        AuthorFormModalTitle = $"Editar Autor #{SelectedAuthor.Id:0000}";
        AuthorFormCode = SelectedAuthor.Id.ToString("0000");
        AuthorFormName = SelectedAuthor.Name;
        AuthorFormPhotoPreviewPath = SelectedAuthor.PhotoPath;
        IsAuthorFormModalVisible = true;
        OnFocusAuthorFormNameRequested?.Invoke();
    }

    [RelayCommand]
    private void CloseAuthorFormModal()
    {
        IsAuthorFormModalVisible = false;
        ResetAuthorFormState();
    }

    private void ResetAuthorFormState()
    {
        AuthorFormName = string.Empty;
        AuthorFormErrorMessage = string.Empty;
        HasAuthorFormError = false;
        _pendingAuthorPhotoBytes = null;
        _localMediaService.DeleteImage(_pendingAuthorPreviewTempPath);
        _pendingAuthorPreviewTempPath = null;
        AuthorFormPhotoPreviewPath = null;
    }

    [RelayCommand]
    private async Task PickAuthorFormPhoto()
    {
        if (IsAuthorFormLoading) return;

        AuthorFormErrorMessage = string.Empty;
        HasAuthorFormError = false;

        try
        {
            await using var stream = await _photoPickerService.PickPhotoAsync();
            if (stream is null)
            {
                return;
            }

            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer);
            var bytes = buffer.ToArray();

            if (IsAuthorFormEditMode && SelectedAuthor is not null)
            {
                IsAuthorFormLoading = true;
                await using var scope = _scopeFactory.CreateAsyncScope();
                var changeIllustrationUseCase = scope.ServiceProvider.GetRequiredService<IChangeAuthorIllustrationUseCase>();
                await using var uploadStream = new MemoryStream(bytes);
                await changeIllustrationUseCase.Execute(SelectedAuthor.Id, uploadStream);

                var savedPath = _localMediaService.GetAuthorIllustrationPath(SelectedAuthor.Id);
                SelectedAuthor.PhotoPath = null;
                SelectedAuthor.PhotoPath = savedPath;
                AuthorFormPhotoPreviewPath = null;
                AuthorFormPhotoPreviewPath = savedPath;
                SetAuthorScreenStatus($"Foto do autor #{SelectedAuthor.Id:0000} atualizada com sucesso!", isError: false);
            }
            else
            {
                _localMediaService.DeleteImage(_pendingAuthorPreviewTempPath);
                _pendingAuthorPhotoBytes = bytes;
                await using var previewStream = new MemoryStream(bytes);
                _pendingAuthorPreviewTempPath = await _localMediaService.SavePreviewImageAsync(previewStream);
                AuthorFormPhotoPreviewPath = null;
                AuthorFormPhotoPreviewPath = _pendingAuthorPreviewTempPath;
            }
        }
        catch (WestmarchBookException ex)
        {
            AuthorFormErrorMessage = string.Join("\n", ex.GetErrorMessage());
            HasAuthorFormError = true;
        }
        catch (System.Exception)
        {
            AuthorFormErrorMessage = ResourceMessagesException.UNKNOWN_ERROR;
            HasAuthorFormError = true;
        }
        finally
        {
            IsAuthorFormLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAuthorPhoto()
    {
        if (IsAuthorFormLoading) return;

        if (IsAuthorFormModalVisible && !IsAuthorFormEditMode)
        {
            _pendingAuthorPhotoBytes = null;
            _localMediaService.DeleteImage(_pendingAuthorPreviewTempPath);
            _pendingAuthorPreviewTempPath = null;
            AuthorFormPhotoPreviewPath = null;
            return;
        }

        if (SelectedAuthor is null)
        {
            SetAuthorScreenStatus("Selecione um autor na listagem para remover a foto.", isError: true);
            return;
        }

        IsAuthorFormLoading = true;

        try
        {
            var authorId = SelectedAuthor.Id;
            var authorName = SelectedAuthor.Name;
            var currentPath = SelectedAuthor.PhotoPath ?? _localMediaService.GetAuthorIllustrationPath(authorId);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var deleteIllustrationUseCase = scope.ServiceProvider.GetRequiredService<IDeleteAuthorIllustrationUseCase>();
            await deleteIllustrationUseCase.Execute(authorId);

            _localMediaService.DeleteImage(currentPath);
            SelectedAuthor.PhotoPath = null;
            if (IsAuthorFormModalVisible)
            {
                AuthorFormPhotoPreviewPath = null;
            }

            SetAuthorScreenStatus($"Foto do autor #{authorId:0000} '{authorName}' removida com sucesso.", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            var message = string.Join("\n", ex.GetErrorMessage());
            if (IsAuthorFormModalVisible)
            {
                AuthorFormErrorMessage = message;
                HasAuthorFormError = true;
            }
            else
            {
                SetAuthorScreenStatus(message, isError: true);
            }
        }
        catch (System.Exception)
        {
            if (IsAuthorFormModalVisible)
            {
                AuthorFormErrorMessage = ResourceMessagesException.UNKNOWN_ERROR;
                HasAuthorFormError = true;
            }
            else
            {
                SetAuthorScreenStatus(ResourceMessagesException.UNKNOWN_ERROR, isError: true);
            }
        }
        finally
        {
            IsAuthorFormLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveAuthorForm()
    {
        if (IsAuthorFormLoading) return;

        AuthorFormErrorMessage = string.Empty;
        HasAuthorFormError = false;
        IsAuthorFormLoading = true;

        try
        {
            var trimmedName = AuthorFormName?.Trim() ?? string.Empty;
            await using var scope = _scopeFactory.CreateAsyncScope();

            if (IsAuthorFormEditMode)
            {
                if (SelectedAuthor is null)
                {
                    AuthorFormErrorMessage = ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND;
                    HasAuthorFormError = true;
                    return;
                }

                var authorId = SelectedAuthor.Id;
                var updateAuthorUseCase = scope.ServiceProvider.GetRequiredService<IUpdateAuthorUseCase>();
                await updateAuthorUseCase.Execute(authorId, new RequestUpdateAuthorJson { Name = trimmedName });

                IsAuthorFormModalVisible = false;
                ResetAuthorFormState();
                await LoadAuthorsScreenAsync();
                await LoadCatalogAsync();
                SelectedAuthor = Authors.FirstOrDefault(a => a.Id == authorId) ?? Authors.FirstOrDefault();
                SetAuthorScreenStatus($"Autor #{authorId:0000} atualizado com sucesso!", isError: false);
                return;
            }

            var registerAuthorUseCase = scope.ServiceProvider.GetRequiredService<IRegisterAuthorUseCase>();
            var response = await registerAuthorUseCase.Execute(new RequestRegisterAuthorJson { Name = trimmedName });

            if (_pendingAuthorPhotoBytes is not null)
            {
                var changeIllustrationUseCase = scope.ServiceProvider.GetRequiredService<IChangeAuthorIllustrationUseCase>();
                await using var uploadStream = new MemoryStream(_pendingAuthorPhotoBytes);
                await changeIllustrationUseCase.Execute(response.Id, uploadStream);
            }

            IsAuthorFormModalVisible = false;
            ResetAuthorFormState();
            await LoadAuthorsScreenAsync();
            SelectedAuthor = Authors.FirstOrDefault(a => a.Id == response.Id) ?? Authors.FirstOrDefault();
            SetAuthorScreenStatus($"Autor #{response.Id:0000} '{response.Name}' cadastrado com sucesso!", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            AuthorFormErrorMessage = string.Join("\n", ex.GetErrorMessage());
            HasAuthorFormError = true;
        }
        catch (System.Exception)
        {
            AuthorFormErrorMessage = ResourceMessagesException.UNKNOWN_ERROR;
            HasAuthorFormError = true;
        }
        finally
        {
            IsAuthorFormLoading = false;
        }
    }

    [RelayCommand]
    private void OpenAuthorPhotoViewerModal()
    {
        HasAuthorScreenStatusMessage = false;

        if (SelectedAuthor is null)
        {
            SetAuthorScreenStatus("Selecione um autor na listagem para visualizar a foto.", isError: true);
            return;
        }

        AuthorPhotoViewerTitle = SelectedAuthor.Name;
        AuthorPhotoViewerImagePath = SelectedAuthor.HasPhoto ? SelectedAuthor.PhotoPath : null;
        IsAuthorPhotoViewerModalVisible = true;
    }

    [RelayCommand]
    private void CloseAuthorPhotoViewerModal()
    {
        IsAuthorPhotoViewerModalVisible = false;
        AuthorPhotoViewerTitle = string.Empty;
        AuthorPhotoViewerImagePath = null;
    }

    public async Task DeleteSelectedAuthorScreenConfirmedAsync()
    {
        HasAuthorScreenStatusMessage = false;

        if (SelectedAuthor is null)
        {
            SetAuthorScreenStatus("Selecione um autor na listagem para deletar.", isError: true);
            return;
        }

        try
        {
            var id = SelectedAuthor.Id;
            var name = SelectedAuthor.Name;
            var photoPath = SelectedAuthor.PhotoPath ?? _localMediaService.GetAuthorIllustrationPath(id);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var deleteAuthorUseCase = scope.ServiceProvider.GetRequiredService<IDeleteAuthorByIdUseCase>();
            await deleteAuthorUseCase.Execute(id);

            _localMediaService.DeleteImage(photoPath);

            _allAuthorsCache.Remove(SelectedAuthor);
            Authors.Remove(SelectedAuthor);
            var availableItem = AvailableAuthors.FirstOrDefault(a => a.Id == id);
            if (availableItem is not null)
            {
                AvailableAuthors.Remove(availableItem);
            }

            SelectedAuthor = Authors.FirstOrDefault();
            SetAuthorScreenStatus($"Autor #{id:0000} '{name}' removido com sucesso.", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            SetAuthorScreenStatus(string.Join("\n", ex.GetErrorMessage()), isError: true);
        }
        catch (System.Exception)
        {
            SetAuthorScreenStatus(ResourceMessagesException.UNKNOWN_ERROR, isError: true);
        }
    }

    private void SetAuthorScreenStatus(string message, bool isError)
    {
        AuthorScreenStatusMessage = message;
        IsAuthorScreenStatusError = isError;
        HasAuthorScreenStatusMessage = true;
    }
}
