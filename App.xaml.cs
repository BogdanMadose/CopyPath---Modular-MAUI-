using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;

namespace CopyPath___Modular_MAUI_
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            //ConfigureWindowsAutoSize(window);
            return window;
        }

        private void ConfigureWindowsAutoSize(Window window)
        {
            window.HandlerChanged += (s, e) =>
            {
                if (window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window winUIWindow) return;

                // Get AppWindow for modern window management
                var hWnd = WindowNative.GetWindowHandle(winUIWindow);
                var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                // Initial size
                var initialSize = GetContentSize(window);
                appWindow.Resize(new SizeInt32(
                    (int)initialSize.Width + 32,  // Add padding
                    (int)initialSize.Height + 40
                ));

                winUIWindow.SizeChanged += (sender, args) =>
                {
                    // Reset to content size only if user hasn't manually resized
                    if (!_userResized)
                    {
                        var currentSize = GetContentSize(window);
                        appWindow.Resize(new SizeInt32(
                            (int)currentSize.Width + 32,
                            (int)currentSize.Height + 40
                        ));
                    }
                };

                var initialBounds = appWindow.Size;
                winUIWindow.SizeChanged += (sender, args) =>
                {
                    _userResized = appWindow.Size.Width != initialBounds.Width ||
                                  appWindow.Size.Height != initialBounds.Height;
                };
            };
        }

        private Size GetContentSize(Window window)
        {
            if (window.Page is not ContentPage page) return new Size(800, 600);

            // Force layout calculation
            page.Measure(double.PositiveInfinity, double.PositiveInfinity);
            return new Size(page.Width, page.Height);
        }

        private bool _userResized;
    }
}