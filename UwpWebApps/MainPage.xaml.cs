using System;
using System.Threading.Tasks;
using UwpWebApps.Models;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;


namespace UwpWebApps
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Fields

        private AppModel _app;

        #endregion

        #region Constructors

        public MainPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        #region Auxiliary

        private void ChangeTitle(string title)
        {
            var currentView = ApplicationView.GetForCurrentView();
            currentView.Title = title;
        }

        private void NavigateToPage(string url)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
            webView.NavigateWithHttpRequestMessage(message);
        }

        private async Task<string> InvokeScript(string body)
        {
            var scriptTemplate =
@"
(function() {
    // common functions
    function removeElementById(id) {
        var elem = document.getElementById(id);
        elem.parentNode.removeChild(elem);
    }

    function removeElementByClassName(className) {
        var elem = document.querySelector('.' + className);
        elem.parentNode.removeChild(elem);    
    }

    function hideElementById(id) {
        var elem = document.getElementById(id);
        elem.style.visibility = 'hidden';
    }

    function forEachElement(selector, fn) {
        var elements = document.querySelectorAll(selector);

        for (var i = 0; i < elements.length; i++) {
            fn(elements[i]);
        }
    }

    #body
})();
";

            return await webView.InvokeScriptAsync("eval", new string[] { scriptTemplate.Replace("#body", body) });
        }

        private async Task ChangeUserAgent(string userAgent)
        {
            var body =
                @"
                    function setUserAgent(window, userAgent) {
                        if (window.navigator.userAgent != userAgent) {
                            var userAgentProp = { get: function () { return userAgent; } };
                            try {
                                Object.defineProperty(window.navigator, 'userAgent', userAgentProp);
                            } catch (e) {
                                window.navigator = Object.create(navigator, {
                                    userAgent: userAgentProp
                                });
                            }
                        }
                    }

                    setUserAgent(window, '" + userAgent + @"');";

            await InvokeScript(body);
        }

        private void EnterExitFullscreenMode()
        {
            var currentView = ApplicationView.GetForCurrentView();

            if (!currentView.IsFullScreenMode)
            {
                currentView.TryEnterFullScreenMode();
            }
            else
            {
                currentView.ExitFullScreenMode();
            }
        }

        private void GoBack()
        {
            if (webView.CanGoBack)
            {
                webView.GoBack();
            }
        }

        private void RefreshPage()
        {
            webView.Refresh();
        }

        #endregion

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _app = AppsManager.Current.GetApp((string)e.Parameter);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().Consolidated += ApplicationView_Consolidated;

            webView.RegisterPropertyChangedCallback(WebView.CanGoBackProperty, CanGoBack_PropertyChanged);
            webView.RegisterPropertyChangedCallback(WebView.CanGoForwardProperty, CanGoForward_PropertyChanged);

            var navManager = SystemNavigationManager.GetForCurrentView();
            navManager.BackRequested += NavManager_BackRequested;

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += AcceleratorKeyActivated;

            NavigateToPage(_app.BaseUrl);
        }

        private void ApplicationView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            webView.NavigateToString("about:blank");
        }

        private void AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType == CoreAcceleratorKeyEventType.KeyDown)
            {
                switch (args.VirtualKey)
                {
                    case VirtualKey.F5:
                        args.Handled = true;
                        RefreshPage();
                        break;

                    case VirtualKey.F11:
                        args.Handled = true;
                        EnterExitFullscreenMode();
                        break;
                }
            }
        }

        private void NavManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            GoBack();
        }

        private void fullscrenAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            EnterExitFullscreenMode();
        }

        private void homeAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(_app.BaseUrl);
        }

        private void refreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void backAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void forwardAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            webView.GoForward();
        }


        private void CanGoBack_PropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            var canGoBack = (bool)sender.GetValue(dp);

            var navManager = Windows.UI.Core.SystemNavigationManager.GetForCurrentView();

            navManager.AppViewBackButtonVisibility = canGoBack ?
                Windows.UI.Core.AppViewBackButtonVisibility.Visible :
                Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;

            backButton.IsEnabled = canGoBack;
        }

        private void CanGoForward_PropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            var canGoForward = (bool)sender.GetValue(dp);
            forwardButton.IsEnabled = canGoForward;
        }


        private void webView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            webErrorTextBlock.Text = string.Empty;
            contentOverlay.Visibility = Visibility.Visible;
            progressRing.IsActive = true;
        }

        private void webView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            //await ChangeUserAgent(userAgent);
            //var kk = await InvokeScript("document.head.innerHTML");
        }

        private async void webView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            var script = _app.DOMContentLoadedScript;
            if (!string.IsNullOrWhiteSpace(script))
            {
                try
                {
                    await InvokeScript(script);
                }
                catch (Exception exc)
                {
                    //var toastContent = new Windows.UI.Notifications.ToastCon
                    //new ToastContent()

                    //ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
                }
            }
        }

        private void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            ChangeTitle(sender.DocumentTitle);
            contentOverlay.Visibility = Visibility.Collapsed;
            progressRing.IsActive = false;
        }

        private void webView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            webErrorTextBlock.Text = $"Web Error: {e.WebErrorStatus}";
        }

        private void webView_PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            var permType = args.PermissionRequest.PermissionType;

            args.PermissionRequest.Allow();
        }

        private void webView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            if (args.Uri.Host == new Uri(_app.BaseUrl).Host)
            {
                args.Handled = true;
                webView.Navigate(args.Uri);
            }    
        }

        private void webView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var currentView = ApplicationView.GetForCurrentView();

            if (sender.ContainsFullScreenElement)
            {
                currentView.TryEnterFullScreenMode();
            }
            else
            {
                currentView.ExitFullScreenMode();
            }
        }

        #endregion

        #endregion
    }
}
