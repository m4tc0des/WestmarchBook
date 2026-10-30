using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.Readers.GetAll;
using WestmarchBook.Application.UseCases.Readers.GetById;
using WestmarchBook.Application.UseCases.Readers.Register;
using WestmarchBook.Application.UseCases.Readers.Update;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Desktop.ViewModels;

public partial class MainViewModel
{
    [ObservableProperty]
    private bool _isReadersListModalVisible;

    [ObservableProperty]
    private ReaderListItemViewModel? _selectedReader;

    [ObservableProperty]
    private string _readerSearchText = string.Empty;

    [ObservableProperty]
    private string _readerListStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasReaderListStatusMessage;

    [ObservableProperty]
    private bool _isReaderListStatusError;

    [ObservableProperty]
    private bool _isReaderFormModalVisible;

    [ObservableProperty]
    private string _readerFormTitle = ResourceMessagesException.READER_FORM_TITLE_CREATE;

    [ObservableProperty]
    private bool _isReaderEditMode;

    [ObservableProperty]
    private string _readerCode = string.Empty;

    [ObservableProperty]
    private string _readerFullName = string.Empty;

    [ObservableProperty]
    private string _readerEmail = string.Empty;

    [ObservableProperty]
    private string _readerPhone = string.Empty;

    [ObservableProperty]
    private string _readerDocumentNumber = string.Empty;

    [ObservableProperty]
    private string _readerFormErrorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasReaderFormError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReaderFormNotLoading))]
    private bool _isReaderFormLoading;

    public bool IsReaderFormNotLoading => !IsReaderFormLoading;

    public ObservableCollection<ReaderListItemViewModel> Readers { get; } = [];

    [RelayCommand]
    private async Task OpenReadersModal()
    {
        IsCadastrosSubmenuVisible = false;
        ClearReaderListStatus();
        IsReadersListModalVisible = true;
        await LoadAllReadersAsync();
    }

    [RelayCommand]
    private void CloseReadersModal()
    {
        IsReadersListModalVisible = false;
        IsReaderFormModalVisible = false;
        ClearReaderListStatus();
    }

    [RelayCommand]
    private async Task OpenIncludeReaderModal()
    {
        ClearReaderListStatus();
        IsReaderEditMode = false;
        ReaderFormTitle = ResourceMessagesException.READER_FORM_TITLE_CREATE;
        ReaderFullName = string.Empty;
        ReaderEmail = string.Empty;
        ReaderPhone = string.Empty;
        ReaderDocumentNumber = string.Empty;
        ReaderFormErrorMessage = string.Empty;
        HasReaderFormError = false;

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var getAllReadersUseCase = scope.ServiceProvider.GetRequiredService<IGetAllReadersUseCase>();
            var nextId = await getAllReadersUseCase.GetNextReaderId();
            ReaderCode = nextId.ToString();
        }
        catch
        {
            var localMax = Readers.Count > 0 ? Readers.Max(r => r.Id) : 0;
            ReaderCode = (localMax + 1).ToString();
        }

        IsReaderFormModalVisible = true;
    }

    [RelayCommand]
    private void OpenEditReaderModal()
    {
        ClearReaderListStatus();

        if (SelectedReader is null)
        {
            SetReaderListStatus("Selecione um leitor para alterar.", isError: true);
            return;
        }

        IsReaderEditMode = true;
        ReaderFormTitle = ResourceMessagesException.READER_FORM_TITLE_EDIT;
        ReaderCode = SelectedReader.Id.ToString();
        ReaderFullName = SelectedReader.FullName;
        ReaderEmail = SelectedReader.Email;
        ReaderPhone = SelectedReader.Phone;
        ReaderDocumentNumber = SelectedReader.DocumentNumber;
        ReaderFormErrorMessage = string.Empty;
        HasReaderFormError = false;

        IsReaderFormModalVisible = true;
    }

    [RelayCommand]
    private void CloseReaderFormModal()
    {
        IsReaderFormModalVisible = false;
        ReaderFormErrorMessage = string.Empty;
        HasReaderFormError = false;
    }

    [RelayCommand]
    private async Task SaveReader()
    {
        if (IsReaderFormLoading) return;

        ReaderFormErrorMessage = string.Empty;
        HasReaderFormError = false;
        IsReaderFormLoading = true;

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            if (IsReaderEditMode)
            {
                if (!long.TryParse(ReaderCode, out var readerId))
                {
                    ReaderFormErrorMessage = ResourceMessagesException.VALIDATION_READER_ID_INVALID;
                    HasReaderFormError = true;
                    return;
                }

                var request = new RequestUpdateReaderJson
                {
                    FullName = ReaderFullName.Trim(),
                    Email = ReaderEmail.Trim(),
                    Phone = ReaderPhone.Trim(),
                    DocumentNumber = ReaderDocumentNumber.Trim()
                };

                var updateReaderUseCase = scope.ServiceProvider.GetRequiredService<IUpdateReaderUseCase>();
                await updateReaderUseCase.Execute(readerId, request);

                if (SelectedReader is not null && SelectedReader.Id == readerId)
                {
                    SelectedReader.FullName = request.FullName;
                    SelectedReader.Email = request.Email;
                    SelectedReader.Phone = request.Phone;
                    SelectedReader.DocumentNumber = request.DocumentNumber;
                }

                IsReaderFormModalVisible = false;
                SetReaderListStatus($"Leitor #{readerId} alterado com sucesso.", isError: false);
                await LoadAllReadersAsync();
            }
            else
            {
                var request = new RequestRegisterReaderJson
                {
                    FullName = ReaderFullName.Trim(),
                    Email = ReaderEmail.Trim(),
                    Phone = ReaderPhone.Trim(),
                    DocumentNumber = ReaderDocumentNumber.Trim()
                };

                var registerReaderUseCase = scope.ServiceProvider.GetRequiredService<IRegisterReaderAccountUseCase>();
                var response = await registerReaderUseCase.Execute(request);

                IsReaderFormModalVisible = false;
                SetReaderListStatus($"Leitor '{response.Name}' cadastrado com sucesso.", isError: false);
                await LoadAllReadersAsync();
            }
        }
        catch (ErrorOnValidationException ex)
        {
            ReaderFormErrorMessage = string.Join("\n", ex.GetErrorMessage());
            HasReaderFormError = true;
        }
        catch (WestmarchBookException ex)
        {
            ReaderFormErrorMessage = string.Join("\n", ex.GetErrorMessage());
            HasReaderFormError = true;
        }
        catch (System.Exception ex)
        {
            ReaderFormErrorMessage = ex.Message;
            HasReaderFormError = true;
        }
        finally
        {
            IsReaderFormLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchReader()
    {
        ClearReaderListStatus();

        if (string.IsNullOrWhiteSpace(ReaderSearchText))
        {
            await LoadAllReadersAsync();
            return;
        }

        var text = ReaderSearchText.Trim();

        if (long.TryParse(text, out var readerId) && readerId > 0)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var getReaderByIdUseCase = scope.ServiceProvider.GetRequiredService<IGetReaderByIdUseCase>();
                var reader = await getReaderByIdUseCase.Execute(readerId);

                var existing = Readers.FirstOrDefault(r => r.Id == reader.Id);
                if (existing is not null)
                {
                    SelectedReader = existing;
                }
                else
                {
                    var item = new ReaderListItemViewModel
                    {
                        Id = reader.Id,
                        FullName = reader.FullName,
                        Email = reader.Email,
                        Phone = reader.Phone,
                        DocumentNumber = reader.DocumentNumber,
                        IsActive = reader.Active
                    };
                    Readers.Insert(0, item);
                    SelectedReader = item;
                }
                SetReaderListStatus($"Leitor #{reader.Id} '{reader.FullName}' localizado.", isError: false);
                return;
            }
            catch
            {
            }
        }

        var matched = Readers.FirstOrDefault(r => r.FullName.Contains(text, StringComparison.OrdinalIgnoreCase)
                                               || r.DocumentNumber.Contains(text, StringComparison.OrdinalIgnoreCase));
        if (matched is not null)
        {
            SelectedReader = matched;
            SetReaderListStatus($"Leitor '{matched.FullName}' selecionado.", isError: false);
            return;
        }

        SetReaderListStatus(ResourceMessagesException.VALIDATION_READER_NOT_FOUND, isError: true);
    }

    [RelayCommand]
    private void ActivateReader()
    {
        SetReaderListStatus(ResourceMessagesException.FEATURE_IN_DEVELOPMENT, isError: false);
    }

    [RelayCommand]
    private void DeactivateReader()
    {
        SetReaderListStatus(ResourceMessagesException.FEATURE_IN_DEVELOPMENT, isError: false);
    }

    private async Task LoadAllReadersAsync()
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var getAllReadersUseCase = scope.ServiceProvider.GetRequiredService<IGetAllReadersUseCase>();
            var all = await getAllReadersUseCase.Execute();

            Readers.Clear();
            foreach (var r in all)
            {
                Readers.Add(new ReaderListItemViewModel
                {
                    Id = r.Id,
                    FullName = r.FullName,
                    Email = r.Email,
                    Phone = r.Phone,
                    DocumentNumber = r.DocumentNumber,
                    IsActive = r.Active
                });
            }
            if (Readers.Count > 0 && SelectedReader is null)
            {
                SelectedReader = Readers[0];
            }
            SetReaderListStatus($"Total de {Readers.Count} leitor(es) cadastrado(s).", isError: false);
        }
        catch (System.Exception ex)
        {
            SetReaderListStatus(ex.Message, isError: true);
        }
    }

    private void SetReaderListStatus(string message, bool isError)
    {
        ReaderListStatusMessage = message;
        IsReaderListStatusError = isError;
        HasReaderListStatusMessage = !string.IsNullOrWhiteSpace(message);
    }

    private void ClearReaderListStatus()
    {
        ReaderListStatusMessage = string.Empty;
        IsReaderListStatusError = false;
        HasReaderListStatusMessage = false;
    }
}
