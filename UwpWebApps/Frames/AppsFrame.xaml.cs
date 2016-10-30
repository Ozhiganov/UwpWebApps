using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpWebApps.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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

namespace UwpWebApps.Frames
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

        private void addAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddEditAppFrame), null);
        }

        private async void appsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedApp = e.ClickedItem as AppModel;
            await AppTilesManager.Current.RequestCreateTile(selectedApp);
        }

        private void appsGridViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var senderElement = sender as FrameworkElement;
            var menu = FlyoutBase.GetAttachedFlyout(senderElement) as MenuFlyout;
            menu.ShowAt(senderElement, e.GetPosition(senderElement));
        }

        private void editAppMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var appModel = ((FrameworkElement)sender).DataContext as AppModel;
            Frame.Navigate(typeof(AddEditAppFrame), appModel);
        }

        private async void removeAppMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var appModel = ((FrameworkElement)sender).DataContext as AppModel;

            var dialog = new MessageDialog($"Are you sure you want to remove {appModel.Name} app?");
            dialog.Commands.Add(new UICommand("Yes", (c) => { RemoveAppCommandHandler(appModel.Id); }, 0));
            dialog.Commands.Add(new UICommand("No") { Id = 1 });
            
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            await dialog.ShowAsync();
        }

        private void RemoveAppCommandHandler(string appId)
        {
            ConfigurationManager.Current.RemoveApp(appId);
        }

        #endregion
    }
}
