using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.Publisher.DeleteById;
using WestmarchBook.Application.UseCases.Publisher.Register;
using WestmarchBook.Application.UseCases.Publisher.Update;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Repositories.Publisher;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Desktop.ViewModels;

public partial class MainViewModel
{
    private readonly List<PublisherListItemViewModel> _allPublishersCache = [];

    [ObservableProperty]
    private PublisherListItemViewModel? _selectedPublisher;

    [ObservableProperty]
    private string _publisherScreenSearchQuery = string.Empty;

    [ObservableProperty]
    private string _publisherScreenStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasPublisherScreenStatusMessage;

    [ObservableProperty]
    private bool _isPublisherScreenStatusError;

    [ObservableProperty]
    private bool _isPublisherFormModalVisible;

    [ObservableProperty]
    private bool _isPublisherFormEditMode;

    [ObservableProperty]
    private string _publisherFormModalTitle = "Incluir Nova Editora";

    [ObservableProperty]
    private string _publisherFormCode = "0001";

    [ObservableProperty]
    private string _publisherFormName = string.Empty;

    [ObservableProperty]
    private string _publisherFormErrorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasPublisherFormError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPublisherFormNotLoading))]
    private bool _isPublisherFormLoading;

    public bool IsPublisherFormNotLoading => !IsPublisherFormLoading;

    public ObservableCollection<PublisherListItemViewModel> Publishers { get; } = [];

    public async Task LoadPublishersScreenAsync()
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var publisherRepository = scope.ServiceProvider.GetRequiredService<IPublisherReadOnlyRepository>();
            var publishers = await publisherRepository.GetAllActive();

            var items = publishers
                .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(p => p.Id)
                .Select(p => new PublisherListItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsActive = p.Active
                })
                .ToList();

            var options = items
                .Select(p => new EntityOptionItem(p.Id, p.Name))
                .ToList();

            DispatchToUi(() =>
            {
                _allPublishersCache.Clear();
                Publishers.Clear();
                AvailablePublishers.Clear();

                foreach (var item in items)
                {
                    _allPublishersCache.Add(item);
                    Publishers.Add(item);
                }

                foreach (var option in options)
                {
                    AvailablePublishers.Add(option);
                }

                SelectedPublisher = Publishers.FirstOrDefault();
            });
        }
        catch (System.Exception)
        {
        }
    }

    [RelayCommand]
    private async Task SearchPublishersScreen()
    {
        HasPublisherScreenStatusMessage = false;

        if (_allPublishersCache.Count == 0)
        {
            await LoadPublishersScreenAsync();
        }

        var term = PublisherScreenSearchQuery?.Trim() ?? string.Empty;

        var filtered = (string.IsNullOrWhiteSpace(term)
            ? _allPublishersCache
            : _allPublishersCache.Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Id.ToString().Equals(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Id)
            .ToList();

        Publishers.Clear();
        foreach (var item in filtered)
        {
            Publishers.Add(item);
        }

        SelectedPublisher = Publishers.FirstOrDefault();
        PublisherScreenStatusMessage = $"{filtered.Count} editora(s) exibida(s).";
        IsPublisherScreenStatusError = false;
        HasPublisherScreenStatusMessage = true;
    }

    [RelayCommand]
    private async Task ClearPublisherScreenSearch()
    {
        PublisherScreenSearchQuery = string.Empty;
        HasPublisherScreenStatusMessage = false;

        if (_allPublishersCache.Count == 0)
        {
            await LoadPublishersScreenAsync();
        }

        Publishers.Clear();
        foreach (var item in _allPublishersCache.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ThenBy(p => p.Id))
        {
            Publishers.Add(item);
        }

        SelectedPublisher = Publishers.FirstOrDefault();
    }

    [RelayCommand]
    private void OpenAddPublisherScreenModal()
    {
        HasPublisherScreenStatusMessage = false;
        IsPublisherFormEditMode = false;
        PublisherFormModalTitle = "Incluir Nova Editora";
        var nextId = (_allPublishersCache.Count > 0 ? _allPublishersCache.Max(p => p.Id) : 0) + 1;
        PublisherFormCode = nextId.ToString("0000");
        PublisherFormName = string.Empty;
        PublisherFormErrorMessage = string.Empty;
        HasPublisherFormError = false;
        IsPublisherFormModalVisible = true;
        OnFocusPublisherFormNameRequested?.Invoke();
    }

    [RelayCommand]
    private void OpenEditPublisherScreenModal()
    {
        HasPublisherScreenStatusMessage = false;

        if (SelectedPublisher is null)
        {
            SetPublisherScreenStatus("Selecione uma editora na listagem antes de clicar em Editar.", isError: true);
            return;
        }

        IsPublisherFormEditMode = true;
        PublisherFormModalTitle = $"Editar Editora #{SelectedPublisher.Id:0000}";
        PublisherFormCode = SelectedPublisher.Id.ToString("0000");
        PublisherFormName = SelectedPublisher.Name;
        PublisherFormErrorMessage = string.Empty;
        HasPublisherFormError = false;
        IsPublisherFormModalVisible = true;
        OnFocusPublisherFormNameRequested?.Invoke();
    }

    [RelayCommand]
    private void ClosePublisherFormModal()
    {
        IsPublisherFormModalVisible = false;
        PublisherFormName = string.Empty;
        PublisherFormErrorMessage = string.Empty;
        HasPublisherFormError = false;
    }

    [RelayCommand]
    private async Task SavePublisherForm()
    {
        if (IsPublisherFormLoading) return;

        PublisherFormErrorMessage = string.Empty;
        HasPublisherFormError = false;
        IsPublisherFormLoading = true;

        try
        {
            var trimmedName = PublisherFormName?.Trim() ?? string.Empty;
            await using var scope = _scopeFactory.CreateAsyncScope();

            if (IsPublisherFormEditMode)
            {
                if (SelectedPublisher is null)
                {
                    PublisherFormErrorMessage = ResourceMessagesException.VALIDATION_PUBLISHER_NOT_FOUND;
                    HasPublisherFormError = true;
                    return;
                }

                var publisherId = SelectedPublisher.Id;
                var updatePublisherUseCase = scope.ServiceProvider.GetRequiredService<IUpdatePublisherUseCase>();
                await updatePublisherUseCase.Execute(publisherId, new RequestUpdatePublisherJson { Name = trimmedName });

                IsPublisherFormModalVisible = false;
                PublisherFormName = string.Empty;
                await LoadPublishersScreenAsync();
                await LoadCatalogAsync();
                SelectedPublisher = Publishers.FirstOrDefault(p => p.Id == publisherId) ?? Publishers.FirstOrDefault();
                SetPublisherScreenStatus($"Editora #{publisherId:0000} atualizada com sucesso!", isError: false);
                return;
            }

            var registerPublisherUseCase = scope.ServiceProvider.GetRequiredService<IRegisterPublisherUseCase>();
            var response = await registerPublisherUseCase.Execute(new RequestRegisterPublisherJson { Name = trimmedName });

            IsPublisherFormModalVisible = false;
            PublisherFormName = string.Empty;
            await LoadPublishersScreenAsync();
            SelectedPublisher = Publishers.FirstOrDefault(p => p.Id == response.Id) ?? Publishers.FirstOrDefault();
            SetPublisherScreenStatus($"Editora #{response.Id:0000} '{response.Name}' cadastrada com sucesso!", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            PublisherFormErrorMessage = string.Join("\n", ex.GetErrorMessage());
            HasPublisherFormError = true;
        }
        catch (System.Exception)
        {
            PublisherFormErrorMessage = ResourceMessagesException.UNKNOWN_ERROR;
            HasPublisherFormError = true;
        }
        finally
        {
            IsPublisherFormLoading = false;
        }
    }

    public async Task DeleteSelectedPublisherScreenConfirmedAsync()
    {
        HasPublisherScreenStatusMessage = false;

        if (SelectedPublisher is null)
        {
            SetPublisherScreenStatus("Selecione uma editora na listagem para deletar.", isError: true);
            return;
        }

        try
        {
            var id = SelectedPublisher.Id;
            var name = SelectedPublisher.Name;

            await using var scope = _scopeFactory.CreateAsyncScope();
            var deletePublisherUseCase = scope.ServiceProvider.GetRequiredService<IDeletePublisherByIdUseCase>();
            await deletePublisherUseCase.Execute(id);

            _allPublishersCache.Remove(SelectedPublisher);
            Publishers.Remove(SelectedPublisher);
            var availableItem = AvailablePublishers.FirstOrDefault(p => p.Id == id);
            if (availableItem is not null)
            {
                AvailablePublishers.Remove(availableItem);
            }

            SelectedPublisher = Publishers.FirstOrDefault();
            SetPublisherScreenStatus($"Editora #{id:0000} '{name}' removida com sucesso.", isError: false);
        }
        catch (WestmarchBookException ex)
        {
            SetPublisherScreenStatus(string.Join("\n", ex.GetErrorMessage()), isError: true);
        }
        catch (System.Exception)
        {
            SetPublisherScreenStatus(ResourceMessagesException.UNKNOWN_ERROR, isError: true);
        }
    }

    private void SetPublisherScreenStatus(string message, bool isError)
    {
        PublisherScreenStatusMessage = message;
        IsPublisherScreenStatusError = isError;
        HasPublisherScreenStatusMessage = true;
    }
}
