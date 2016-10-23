using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpWebApps.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpWebApps.AppsHubPageFrames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsFrame : Page
    {
        #region Fields

        List<AppModel> _apps = new List<AppModel>
            {
                new AppModel()
                {
                    Id = "5F89914B-E167-44E4-B6EE-F5211F977633",
                    Name = "YouTube",
                    BaseUrl = "https://youtube.com/",
                    AccentColor = "#CC181E",
                    IconName = "youtube.png"
                },

                new AppModel()
                {
                    Id = "4B298274-0F6F-4558-8AB5-58160736EF62",
                    Name = "Google Maps",
                    BaseUrl = "https://www.google.com.ua/maps",
                    AccentColor = "#1CA261",
                    IconName = "google-maps.png"
                },

                new AppModel()
                {
                    Id = "660FD349-BF1A-4F2B-8909-C4C872AA72B7",
                    Name = "Google Translate",
                    BaseUrl = "https://translate.google.com/",
                    AccentColor = "#4889F0",
                    IconName = "google-translate.png"
                },

                new AppModel()
                {
                    Id = "DDD64EEA-ED01-4C83-9A3A-AC986EA07EBB",
                    Name = "Google Photos",
                    BaseUrl = "https://photos.google.com/",
                    AccentColor = "#FFA013"
                },

                new AppModel()
                {
                    Id = "DC743BD9-916B-460C-8093-781FA0F97206",
                    Name = "Google Play Books",
                    BaseUrl = "https://books.google.com/ebooks/app",
                    AccentColor = "#3FDBFE"
                },

                new AppModel()
                {
                    Id = "697B0E31-E65E-476F-A746-E676C12B23A6",
                    Name = "Lingualeo",
                    BaseUrl = "https://lingualeo.com",
                    AccentColor = "#48B484"
                },
            };

        #endregion

        #region Constructors

        public AppsFrame()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            appsGrid.ItemsSource = _apps;
        }

        private async void appsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedApp = e.ClickedItem as AppModel;

            var appTile = new SecondaryTile(
                $"WebApp-{selectedApp.Id}",
                selectedApp.Name,
                selectedApp.BaseUrl,
                new Uri($"ms-appx:///AppIcons/{selectedApp.IconName}"),
                TileSize.Default);
            appTile.VisualElements.BackgroundColor = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), selectedApp.AccentColor);
            appTile.VisualElements.ShowNameOnSquare150x150Logo = true;

            await appTile.RequestCreateAsync();
        }

        #endregion
    }
}
