using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UwpWebApps
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private List<CoreDispatcher> dispatchers = new List<CoreDispatcher>();


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)                                      // First activation
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
                if (rootFrame.Content == null)
                {
                    NavigateTo(e.TileId, e.Arguments, rootFrame);
                }

                Window.Current.Activate();
                this.dispatchers.Add(CoreWindow.GetForCurrentThread().Dispatcher);
            }
            else // Subsequent activations - create new view/UI thread
            {
                var view = CoreApplication.CreateNewView();
                int windowId = 0;
                await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    windowId = ApplicationView.GetApplicationViewIdForWindow(CoreWindow.GetForCurrentThread());
                    var frame = new Frame();
                    NavigateTo(e.TileId, e.Arguments, frame);
                    Window.Current.Content = frame;
                    Window.Current.Activate();
                    ApplicationView.GetForCurrentView().Consolidated += View_Consolidated;
                });

                // Run this on the last dispatcher so the windows get positioned correctly
                bool viewShown;
                await this.dispatchers.Last().RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(windowId);
                });
                this.dispatchers.Add(view.Dispatcher);
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


        private void View_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            this.dispatchers.Remove(CoreWindow.GetForCurrentThread().Dispatcher);
            ApplicationView.GetForCurrentView().Consolidated -= View_Consolidated;
        }

        private void NavigateTo(string tileId, string args, Frame frame)
        {
            var testApp = false;

            if (testApp)
            {
                frame.Navigate(typeof(MainPage), "910607c7-fc99-47af-9ca3-37c470c6d6d9");
            }
            else if (Models.AppModel.IsAppTileId(tileId))
            {
                frame.Navigate(typeof(MainPage), args);
            }
            else
            {
                frame.Navigate(typeof(AppsHubPage), args);
            }
        }
    }
}
