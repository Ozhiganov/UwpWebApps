using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpWebApps
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string startPage;
        private const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36";

        public MainPage()
        {
            this.InitializeComponent();
        }


        private void ChangeTitle(string title)
        {
            var currentView = ApplicationView.GetForCurrentView();
            currentView.Title = title;
        }

        private void NavigateTo(string url)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
            //message.Headers.Add("User-Agent", userAgent);

            webView.NavigateWithHttpRequestMessage(message);
        }

        private async Task<string> InvokeScript(string body)
        {
            return await webView.InvokeScriptAsync("eval", new string[] { body });
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


        private void GoBack()
        {
            if (webView.CanGoBack)
            {
                webView.GoBack();
            }
        }

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            startPage = (string)e.Parameter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            webView.RegisterPropertyChangedCallback(WebView.CanGoBackProperty, CanGoBack_PropertyChanged);
            webView.RegisterPropertyChangedCallback(WebView.CanGoForwardProperty, CanGoForward_PropertyChanged);

            var navManager = Windows.UI.Core.SystemNavigationManager.GetForCurrentView();
            navManager.BackRequested += NavManager_BackRequested;

            webView.NavigationStarting += WebView_NavigationStarting;
            webView.ContentLoading += WebView_ContentLoading;
            webView.DOMContentLoaded += WebView_DOMContentLoaded;

            webView.FrameNavigationStarting += WebView_FrameNavigationStarting;


            NavigateTo(startPage);
        }

        private async void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            //await ChangeUserAgent(userAgent);
        }

        private async void WebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            //await ChangeUserAgent(userAgent);
            //var kk = await InvokeScript("document.head.innerHTML");
        }

        private async void WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            var kk = await InvokeScript("navigator.userAgent");
        }

        private async void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var kk = await InvokeScript("navigator.userAgent");


            ChangeTitle(sender.DocumentTitle);
            progressRing.IsActive = false;
        }

        private async void WebView_FrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            //args.Cancel = true;
        }




        private void NavManager_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            GoBack();
        }

        private void fullscrenAppBarButton_Click(object sender, RoutedEventArgs e)
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

        private void homeAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(startPage);
        }

        private void refreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            webView.Refresh();
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
            webErrorTextBlock.Text = String.Empty;
            progressRing.IsActive = true;
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
            //args.
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
    }
}
