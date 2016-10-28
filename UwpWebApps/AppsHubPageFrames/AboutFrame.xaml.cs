using Microsoft.Services.Store.Engagement;
using System;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpWebApps.AppsHubPageFrames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutFrame : Page
    {
        #region Properties

        public string AppName
        {
            get
            {
                return Package.Current.DisplayName;
            }
        }

        public string AppPublisher
        {
            get
            {
                return Package.Current.PublisherDisplayName;
            }
        }

        public string AppPublisherStoreUrl
        {
            get
            {
                return $"ms-windows-store://publisher/?name={AppPublisher}";
            }
        }

        public string AppVersion
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}";
            }
        }

        public string Copyright
        {
            get
            {
                return CurrentAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            }
        }

        private Assembly CurrentAssembly
        {
            get
            {
                return typeof(App).GetTypeInfo().Assembly;
            }
        }

        #endregion

        #region Constructors

        public AboutFrame()
        {
            this.InitializeComponent();

            if (StoreServicesFeedbackLauncher.IsSupported())
            {
                this.feedbackButton.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Methods

        private async void feedbackAppButton_Click(object sender, RoutedEventArgs e)
        {
            var launcher = StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        #endregion
    }
}
