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
                new AppModel() { Id = "4B298274-0F6F-4558-8AB5-58160736EF62", Name = "Google Maps", BaseUrl = "https://www.google.com.ua/maps" }
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
