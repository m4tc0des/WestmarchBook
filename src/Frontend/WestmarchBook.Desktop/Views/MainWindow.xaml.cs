using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Desktop.Services;
using WestmarchBook.Desktop.ViewModels;

namespace WestmarchBook.Desktop.Views;

public partial class MainWindow : ContentPage
{
    private IDispatcherTimer? _clockTimer;
    private bool _keyboardHooked;
    private bool _eventsSubscribed;
    private Window? _loginOverlayWindow;
    private bool _isFormattingDate;

    public MainViewModel ViewModel => (MainViewModel)BindingContext!;

    public MainWindow()
    {
        InitializeComponent();
        StartClockTimer();
        HandlerChanged += OnMainPageHandlerChanged;
        Loaded += OnPageLoaded;
        Unloaded += OnPageUnloaded;
        SizeChanged += (_, _) => ForceLayoutRefresh();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        BindingContext = viewModel;
        SubscribeViewModelEvents(viewModel);
    }

    private void SubscribeViewModelEvents(MainViewModel viewModel)
    {
        if (_eventsSubscribed) return;

        viewModel.SwitchUserRequested += OnSwitchUserRequested;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _eventsSubscribed = true;
    }

    private void UnsubscribeViewModelEvents(MainViewModel viewModel)
    {
        if (!_eventsSubscribed) return;

        viewModel.SwitchUserRequested -= OnSwitchUserRequested;
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _eventsSubscribed = false;
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            SubscribeViewModelEvents(vm);
        }

        if (_clockTimer is { IsRunning: false })
        {
            _clockTimer.Start();
        }

        ForceLayoutRefresh();
    }

    private void OnPageUnloaded(object? sender, EventArgs e)
    {
        _clockTimer?.Stop();

        if (BindingContext is MainViewModel vm)
        {
            UnsubscribeViewModelEvents(vm);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainViewModel.IsBooksListModalVisible)
            or nameof(MainViewModel.IsBookFormModalVisible)
            or nameof(MainViewModel.IsQuickEntityModalVisible)
            or nameof(MainViewModel.IsCadastrosSubmenuVisible)
            or nameof(MainViewModel.IsReadersListModalVisible)
            or nameof(MainViewModel.IsReaderFormModalVisible)
            or nameof(MainViewModel.IsCopyModalVisible))
        {
            ForceLayoutRefresh();
        }
    }

    public void ForceLayoutRefresh()
    {
        Dispatcher.Dispatch(() =>
        {
            InvalidateMeasure();
            WorkspaceArea?.InvalidateMeasure();
            BookFormModalGrid?.InvalidateMeasure();
            QuickEntityModalGrid?.InvalidateMeasure();
            CopyRegistrationModalGrid?.InvalidateMeasure();
#if WINDOWS
            if (Window?.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWin &&
                nativeWin.Content is Microsoft.UI.Xaml.FrameworkElement rootElement)
            {
                rootElement.InvalidateMeasure();
                rootElement.InvalidateArrange();
            }
#endif
        });
    }

    private void OnMainPageHandlerChanged(object? sender, EventArgs e)
    {
#if WINDOWS
        if (_keyboardHooked) return;

        if (Handler?.PlatformView is Microsoft.UI.Xaml.UIElement uiElement)
        {
            var f2Accelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.F2
            };
            f2Accelerator.Invoked += (_, args) =>
            {
                if (BindingContext is MainViewModel vm && vm.SwitchUserCommand.CanExecute(null))
                {
                    vm.SwitchUserCommand.Execute(null);
                    args.Handled = true;
                }
            };

            var f9Accelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.F9
            };
            f9Accelerator.Invoked += (_, args) =>
            {
                if (BindingContext is MainViewModel vm && vm.OpenReadersModalCommand.CanExecute(null))
                {
                    vm.OpenReadersModalCommand.Execute(null);
                    args.Handled = true;
                }
            };

            var f7Accelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.F7
            };
            f7Accelerator.Invoked += (_, args) =>
            {
                if (BindingContext is MainViewModel vm && vm.OpenBooksModalCommand.CanExecute(null))
                {
                    vm.OpenBooksModalCommand.Execute(null);
                    args.Handled = true;
                }
            };

            var escAccelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Escape
            };
            escAccelerator.Invoked += (_, args) =>
            {
                if (BindingContext is not MainViewModel vm) return;

                if (vm.IsCopyModalVisible)
                {
                    vm.CloseCopyModalCommand.Execute(null);
                    args.Handled = true;
                }
                else if (vm.IsReaderFormModalVisible)
                {
                    vm.CloseReaderFormModalCommand.Execute(null);
                    args.Handled = true;
                }
                else if (vm.IsReadersListModalVisible)
                {
                    vm.CloseReadersModalCommand.Execute(null);
                    args.Handled = true;
                }
                else if (vm.IsQuickEntityModalVisible)
                {
                    vm.CloseQuickEntityModalCommand.Execute(null);
                    args.Handled = true;
                }
                else if (vm.IsBookFormModalVisible)
                {
                    vm.CloseBookFormModalCommand.Execute(null);
                    args.Handled = true;
                }
                else if (vm.IsBooksListModalVisible)
                {
                    vm.CloseBooksModalCommand.Execute(null);
                    args.Handled = true;
                }
            };

            uiElement.KeyboardAcceleratorPlacementMode = Microsoft.UI.Xaml.Input.KeyboardAcceleratorPlacementMode.Hidden;
            uiElement.KeyboardAccelerators.Add(f2Accelerator);
            uiElement.KeyboardAccelerators.Add(f9Accelerator);
            uiElement.KeyboardAccelerators.Add(f7Accelerator);
            uiElement.KeyboardAccelerators.Add(escAccelerator);
            _keyboardHooked = true;
        }
#endif
    }

    private void OnPublicationDateTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isFormattingDate || sender is not Entry entry) return;

        var raw = e.NewTextValue ?? string.Empty;
        var digits = new string(raw.Where(char.IsDigit).Take(8).ToArray());

        string formatted = digits.Length switch
        {
            <= 2 => digits,
            <= 4 => $"{digits[..2]}/{digits[2..]}",
            _ => $"{digits[..2]}/{digits[2..4]}/{digits[4..]}"
        };

        if (formatted != raw)
        {
            _isFormattingDate = true;
            entry.Text = formatted;
            entry.CursorPosition = formatted.Length;
            _isFormattingDate = false;
        }
    }

    private void OnPublicationDateSelected(object? sender, DateChangedEventArgs e)
    {
        if (BindingContext is MainViewModel vm && e.NewDate is DateTime selectedDate)
        {
            vm.HasPublicationDate = true;
            vm.PublicationDateText = selectedDate.ToString("dd/MM/yyyy");
        }
    }

    private void OnPublicationDateEntryCompleted(object? sender, EventArgs e)
    {
        Dispatcher.Dispatch(() => BookTitleEntry?.Focus());
    }

    private void OnBookTitleCompleted(object? sender, EventArgs e)
    {
        Dispatcher.Dispatch(() => BookDescriptionEntry?.Focus());
    }

    private void OnBookDescriptionCompleted(object? sender, EventArgs e)
    {
        Dispatcher.Dispatch(() => AuthorPicker?.Focus());
    }

    private void OnSwitchUserRequested()
    {
        if (_loginOverlayWindow is not null) return;

        var loginWindow = App.Services.GetRequiredService<LoginWindow>();
        var overlayWindow = new Window(loginWindow);

        overlayWindow.HandlerChanged += (_, _) =>
        {
            App.ConfigureFixedWindow(overlayWindow, "Westmarch - Login", 440, 640);
#if WINDOWS
            if (overlayWindow.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                if (Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId).Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                {
                    presenter.IsAlwaysOnTop = true;
                    presenter.IsMinimizable = false;
                    presenter.IsMaximizable = false;
                    presenter.IsResizable = false;
                }
            }
#endif
        };
        overlayWindow.Destroying += (_, _) =>
        {
            _loginOverlayWindow = null;
            var tokenProvider = App.Services.GetRequiredService<DesktopTokenProvider>();
            if (string.IsNullOrEmpty(tokenProvider.Token))
            {
                Microsoft.Maui.Controls.Application.Current?.Quit();
            }
        };

        _loginOverlayWindow = overlayWindow;
        Microsoft.Maui.Controls.Application.Current?.OpenWindow(overlayWindow);
    }

    private void OnMinimizeWindowClicked(object? sender, EventArgs e)
    {
        var window = Window ?? Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
        App.MinimizeWindow(window);
    }

    private void StartClockTimer()
    {
        _clockTimer = Dispatcher.CreateTimer();
        _clockTimer.Interval = TimeSpan.FromSeconds(1);
        _clockTimer.Tick += OnClockTimerTick;
        _clockTimer.Start();
    }

    private void OnClockTimerTick(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.UpdateClock();
        }
    }

    private void OnCadastrosPointerEntered(object? sender, PointerEventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.IsCadastrosSubmenuVisible = true;
        }
    }

    private void OnOtherMenuPointerEntered(object? sender, PointerEventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.IsCadastrosSubmenuVisible = false;
        }
    }

    private void OnSubmenuPointerExited(object? sender, PointerEventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.IsCadastrosSubmenuVisible = false;
        }
    }

    private void OnWorkspaceTapped(object? sender, TappedEventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.IsCadastrosSubmenuVisible = false;
        }
    }

    private void OnGenericMenuClicked(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.IsCadastrosSubmenuVisible = false;
        }
    }

    private void OnShortcutPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Border border)
        {
            border.BackgroundColor = Color.FromArgb("#4A1A22");
            border.Stroke = Color.FromArgb("#FFDF73");
        }
    }

    private void OnShortcutPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is Border border)
        {
            border.BackgroundColor = Color.FromArgb("#2E181C");
            border.Stroke = Color.FromArgb("#D4AF37");
        }
    }

    private void OnReadersShortcutTapped(object? sender, TappedEventArgs e)
    {
        if (BindingContext is MainViewModel vm && vm.OpenReadersModalCommand.CanExecute(null))
        {
            vm.OpenReadersModalCommand.Execute(null);
        }
    }

    private void OnBooksShortcutTapped(object? sender, TappedEventArgs e)
    {
        if (BindingContext is MainViewModel vm && vm.OpenBooksModalCommand.CanExecute(null))
        {
            vm.OpenBooksModalCommand.Execute(null);
        }
    }

    private async void OnExitMenuClicked(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.IsCadastrosSubmenuVisible = false;
        }

        var confirm = await DisplayAlertAsync("Confirmação", "Tem certeza que deseja sair?", "Sim", "Não");
        if (confirm)
        {
            _clockTimer?.Stop();
            Microsoft.Maui.Controls.Application.Current?.Quit();
        }
    }

    private async void OnDeleteBookClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not MainViewModel vm) return;

        if (vm.SelectedBook is null)
        {
            await DisplayAlertAsync("Atenção", "Selecione um livro na listagem para deletar.", "OK");
            return;
        }

        var confirm = await DisplayAlertAsync(
            "Confirmar Exclusão",
            $"Tem certeza que deseja deletar o livro '{vm.SelectedBook.Title}' (Código {vm.SelectedBook.Id})?",
            "Sim",
            "Não");

        if (confirm)
        {
            await vm.DeleteSelectedBookConfirmedAsync();
        }
    }

    private void OnSearchBookCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm && vm.SearchBookCommand.CanExecute(null))
        {
            vm.SearchBookCommand.Execute(null);
        }
    }

    private void OnSearchReaderCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm && vm.SearchReaderCommand.CanExecute(null))
        {
            vm.SearchReaderCommand.Execute(null);
        }
    }

    private void OnQuickEntityCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm && vm.SaveQuickEntityCommand.CanExecute(null))
        {
            vm.SaveQuickEntityCommand.Execute(null);
        }
    }

    private void OnCopyRegistrationCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm && vm.AddCopyRegistrationNumberCommand.CanExecute(null))
        {
            vm.AddCopyRegistrationNumberCommand.Execute(null);
            Dispatcher.Dispatch(() => CopyRegistrationNumberEntry?.Focus());
        }
    }
}
