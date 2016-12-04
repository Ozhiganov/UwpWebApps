using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UwpWebApps.AppsManagement;
using UwpWebApps.Models;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
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

        private static string DOMContentLoadedScriptTemplate;

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
            if (string.IsNullOrEmpty(DOMContentLoadedScriptTemplate))
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resources/Scripts/DOMContentLoadedScriptTemplate.js"));
                DOMContentLoadedScriptTemplate = await FileIO.ReadTextAsync(file);
            }

            try
            {
                return await webView.InvokeScriptAsync("eval", new string[] { DOMContentLoadedScriptTemplate.Replace("//#body", body) });
            }
            catch (Exception exc)
            {
                var dialog = new MessageDialog(exc.Message, "Invoke Script Error");
                await dialog.ShowAsync();
                return null;
            }
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _app = (await AppsManager.GetCurrent()).GetApp((string)e.Parameter);
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

            var navManager = SystemNavigationManager.GetForCurrentView();

            navManager.AppViewBackButtonVisibility = canGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;

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

        private async void webView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            var script = _app.ContentLoadingScript;
            if (args.Uri != null && !string.IsNullOrWhiteSpace(script))
            {
                await InvokeScript(
@"

setUserAgent(window, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36');

MediaSource.isTypeSupported = function(type) {
    console.log('call MediaSource.isTypeSupported(' + type + ')');
    return true;
}

//-------------------------
var origAppendBuffer = SourceBuffer.prototype.appendBuffer;
var newAppendBuffer = function () {
    console.log('call appendBuffer ' + this.updating);
    this.addEventListener('updateend', function () {
        console.log('fire updateend ' + JSON.stringify(arguments));
    });    

    this.updating = true;

    var updateStartEvent = new CustomEvent('updatstart', { 'isTrusted' : true });
    this.dispatchEvent(updateStartEvent);

    this.updating = false;

    var updateEvent = new CustomEvent('update', { 'isTrusted' : true });
    this.dispatchEvent(updateEvent);

    var updateEndEvent = new CustomEvent('updateend', { 'isTrusted' : true });
    this.dispatchEvent(updateEndEvent);
}
SourceBuffer.prototype.appendBuffer = newAppendBuffer;

// -----------------------------
var origAddSourceBuffer = MediaSource.prototype.addSourceBuffer;
var newAddSourceBuffer = function () {
    arguments[0] = 'audio/mp4';
    return origAddSourceBuffer.apply(this, arguments);
}
MediaSource.prototype.addSourceBuffer = newAddSourceBuffer;

//-------------------------------

window.musicElement = document.createElement('audio');

//---------------------------

var origFetch = window.fetch;

var newFetch = function () {
    var url = arguments[0];

    if (window.musicElement.src != url) {
        console.log('call fetch: ' + url);
        window.musicElement.src = url;
        window.musicElement.play();
    }
    return origFetch.apply(this, arguments);
};
window.fetch = newFetch;


//------------------------
var origOpen = XMLHttpRequest.prototype.open;

var newOpen = function () {
    var url = arguments[1];
    //console.log('XHR: ' + url);
    origOpen.apply(this, arguments);
};
XMLHttpRequest.prototype.open = newOpen;

"
                    );
            }
        }

        private async void webView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            var script = _app.DOMContentLoadedScript;
            if (args.Uri != null && !string.IsNullOrWhiteSpace(script))
            {
                await InvokeScript(script);
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

        private void webView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Debug.WriteLine($"SCRIPT_NOTIFY: {e.Value}");
        }

        #endregion

        #endregion
    }
}
