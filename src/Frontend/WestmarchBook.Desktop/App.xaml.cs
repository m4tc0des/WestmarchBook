using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Desktop.Views;

namespace WestmarchBook.Desktop;

public partial class App : Microsoft.Maui.Controls.Application
{
    public static IServiceProvider Services { get; internal set; } = null!;

    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var loginPage = Services.GetRequiredService<LoginWindow>();
        var window = new Window(loginPage)
        {
            Title = "Westmarch - Sistema de Gestao de Livros"
        };

        window.HandlerChanged += (_, _) => ConfigureFixedWindow(window, "Westmarch - Sistema de Gestao de Livros", 440, 640);

        return window;
    }

#if WINDOWS
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern nint GetSystemMenu(nint hWnd, bool bRevert);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool EnableMenuItem(nint hMenu, uint uIDEnableItem, uint uEnable);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool RedrawWindow(nint hWnd, nint lprcUpdate, nint hrgnUpdate, uint flags);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool UpdateWindow(nint hWnd);

    private const uint SC_CLOSE = 0xF060;
    private const uint MF_BYCOMMAND = 0x00000000;
    private const uint MF_GRAYED = 0x00000001;
    private const uint MF_DISABLED = 0x00000002;
    private const uint RDW_INVALIDATE = 0x0001;
    private const uint RDW_ERASE = 0x0004;
    private const uint RDW_ALLCHILDREN = 0x0080;
    private const uint RDW_UPDATENOW = 0x0100;

    internal static bool AllowLoginClose { get; set; }

    internal static void OnAppWindowClosing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        if (AllowLoginClose) return;

        var activeWindow = Current?.Windows.FirstOrDefault();
        if (activeWindow?.Page is LoginWindow)
        {
            args.Cancel = true;
        }
    }
#endif

    public static void ConfigureFixedWindow(Window window, string title, double width, double height)
    {
        window.Title = title;
        window.Width = width;
        window.Height = height;
        window.MinimumWidth = width;
        window.MinimumHeight = height;
        window.MaximumWidth = width;
        window.MaximumHeight = height;

#if WINDOWS
        if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            var iconPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Resources", "AppIcon", "appicon.ico");
            if (System.IO.File.Exists(iconPath))
            {
                appWindow.SetIcon(iconPath);
            }

            if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
            {
                presenter.Restore();
                presenter.SetBorderAndTitleBar(true, true);
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
                presenter.IsResizable = false;
            }

            if (window.Page is LoginWindow)
            {
                var hMenu = GetSystemMenu(hwnd, false);
                if (hMenu != 0)
                {
                    EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
                }
            }

            appWindow.Closing -= OnAppWindowClosing;
            appWindow.Closing += OnAppWindowClosing;

            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
            if (displayArea is not null)
            {
                var centerX = displayArea.WorkArea.X + (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
                var centerY = displayArea.WorkArea.Y + (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
                appWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));
            }
        }
#endif
    }

    public static void ConfigureMaximizedWindow(Window window, string title = "")
    {
        window.Title = title;
        window.MinimumWidth = 0;
        window.MinimumHeight = 0;
        window.MaximumWidth = double.PositiveInfinity;
        window.MaximumHeight = double.PositiveInfinity;

#if WINDOWS
        if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
        {
            nativeWindow.ExtendsContentIntoTitleBar = false;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            var iconPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Resources", "AppIcon", "appicon.ico");
            if (System.IO.File.Exists(iconPath))
            {
                appWindow.SetIcon(iconPath);
            }

            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
            if (displayArea is not null)
            {
                appWindow.MoveAndResize(displayArea.WorkArea);
            }

            if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(false, false);
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = true;
            }

            if (nativeWindow.Content is Microsoft.UI.Xaml.FrameworkElement rootElement)
            {
                rootElement.InvalidateMeasure();
                rootElement.InvalidateArrange();
                rootElement.UpdateLayout();
            }

            SetWindowPos(hwnd, 0, 0, 0, 0, 0, 0x0027);
            UpdateWindow(hwnd);
            RedrawWindow(hwnd, 0, 0, RDW_INVALIDATE | RDW_ERASE | RDW_ALLCHILDREN | RDW_UPDATENOW);
        }
#endif
    }

    public static void MinimizeWindow(Window? window)
    {
#if WINDOWS
        if (window?.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
            {
                presenter.Minimize();
            }
        }
#endif
    }
}
