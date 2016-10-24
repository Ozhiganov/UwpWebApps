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
        #region Constructors

        public AppsFrame()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            appsGrid.ItemsSource = ConfigurationManager.Current.GetApps();
        }

        private async void appsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedApp = e.ClickedItem as AppModel;

            var appTile = new SecondaryTile(
                $"WebApp-{selectedApp.Id}",
                selectedApp.Name,
                selectedApp.Id,
                new Uri($"ms-appx:///AppIcons/{selectedApp.IconName}"),
                TileSize.Default);
            appTile.VisualElements.BackgroundColor = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), selectedApp.AccentColor);
            appTile.VisualElements.ShowNameOnSquare150x150Logo = true;

            await appTile.RequestCreateAsync();
        }

        #endregion
    }
}
