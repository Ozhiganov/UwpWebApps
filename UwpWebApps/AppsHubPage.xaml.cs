using System;
using System.Collections.Generic;
using UwpWebApps.Models;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpWebApps
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsHubPage : Page
    {
        public AppsHubPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var apps = new List<AppModel>
            {
                new AppModel() { Id = "5F89914B-E167-44E4-B6EE-F5211F977633", Name = "YouTube", BaseUrl = "https://youtube.com/" },
                new AppModel() { Id = "4B298274-0F6F-4558-8AB5-58160736EF62", Name = "Google Maps", BaseUrl = "https://www.google.com.ua/maps" },
                new AppModel() { Id = "660FD349-BF1A-4F2B-8909-C4C872AA72B7", Name = "Google Translate", BaseUrl = "https://translate.google.com/" },
                new AppModel() { Id = "DDD64EEA-ED01-4C83-9A3A-AC986EA07EBB", Name = "Google Photos", BaseUrl = "https://photos.google.com/" },
                new AppModel() { Id = "DC743BD9-916B-460C-8093-781FA0F97206", Name = "Google Play Books", BaseUrl = "https://books.google.com/ebooks/app" },
                new AppModel() { Id = "697B0E31-E65E-476F-A746-E676C12B23A6", Name = "Lingualeo", BaseUrl = "https://lingualeo.com" },
            };

            appsGrid.ItemsSource = apps;
        }

        private async void appsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedApp = e.ClickedItem as AppModel;

            var appTile = new SecondaryTile(
                $"WebApp-{selectedApp.Id}",
                selectedApp.Name,
                selectedApp.BaseUrl,
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"),
                TileSize.Default);

            await appTile.RequestCreateAsync();
        }
    }
}
