using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.Author.Register;
using WestmarchBook.Application.UseCases.Publisher.Register;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Desktop.ViewModels;

public partial class MainViewModel
{
    [ObservableProperty]
    private bool _isQuickEntityModalVisible;

    [ObservableProperty]
    private bool _isQuickEntityPublisher;

    [ObservableProperty]
    private string _quickEntityModalTitle = string.Empty;

    [ObservableProperty]
    private string _quickEntityName = string.Empty;

    [ObservableProperty]
    private string _quickEntityError = string.Empty;

    [ObservableProperty]
    private bool _hasQuickEntityError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsQuickEntityNotLoading))]
    private bool _isQuickEntityLoading;

    public bool IsQuickEntityNotLoading => !IsQuickEntityLoading;

    [RelayCommand]
    private void OpenQuickPublisherModal()
    {
        IsQuickEntityPublisher = true;
        QuickEntityModalTitle = "Cadastro Rápido de Publicadora";
        QuickEntityName = string.Empty;
        QuickEntityError = string.Empty;
        HasQuickEntityError = false;
        IsQuickEntityModalVisible = true;
    }

    [RelayCommand]
    private void OpenQuickAuthorModal()
    {
        IsQuickEntityPublisher = false;
        QuickEntityModalTitle = "Cadastro Rápido de Autor";
        QuickEntityName = string.Empty;
        QuickEntityError = string.Empty;
        HasQuickEntityError = false;
        IsQuickEntityModalVisible = true;
    }

    [RelayCommand]
    private async Task SaveQuickEntity()
    {
        QuickEntityError = string.Empty;
        HasQuickEntityError = false;
        IsQuickEntityLoading = true;

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            if (IsQuickEntityPublisher)
            {
                var registerPublisherUseCase = scope.ServiceProvider.GetRequiredService<IRegisterPublisherUseCase>();
                var response = await registerPublisherUseCase.Execute(new RequestRegisterPublisherJson
                {
                    Name = QuickEntityName.Trim()
                });

                var newPublisher = new EntityOptionItem(response.Id, response.Name);
                AvailablePublishers.Add(newPublisher);
                SelectedPublisherOption = newPublisher;
            }
            else
            {
                var registerAuthorUseCase = scope.ServiceProvider.GetRequiredService<IRegisterAuthorUseCase>();
                var response = await registerAuthorUseCase.Execute(new RequestRegisterAuthorJson
                {
                    Name = QuickEntityName.Trim()
                });

                var newAuthor = new EntityOptionItem(response.Id, response.Name);
                AvailableAuthors.Add(newAuthor);
                SelectedAuthorOption = newAuthor;
            }

            IsQuickEntityModalVisible = false;
        }
        catch (WestmarchBookException ex)
        {
            QuickEntityError = string.Join("\n", ex.GetErrorMessage());
            HasQuickEntityError = true;
        }
        catch (System.Exception)
        {
            QuickEntityError = ResourceMessagesException.UNKNOWN_ERROR;
            HasQuickEntityError = true;
        }
        finally
        {
            IsQuickEntityLoading = false;
        }
    }

    [RelayCommand]
    private void CloseQuickEntityModal()
    {
        IsQuickEntityModalVisible = false;
        QuickEntityError = string.Empty;
        HasQuickEntityError = false;
    }
}
