using System;
using System.Linq;
using System.Collections.Generic;
using UwpWebApps.Models;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpWebApps
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsHubPage : Page
    {
        #region Fields

        private List<MenuItemModel> _menuItems = new List<MenuItemModel>
        {
            new MenuItemModel { Id = "apps", FontIcon = "\xE71D", Title = "Apps" },
            new MenuItemModel { Id = "about", FontIcon = "\xE946", Title = "About" },
            new MenuItemModel { Id = "credits", FontIcon = "\xEB51", Title = "Credits" }
        };

        #endregion

        #region Properties

        public List<MenuItemModel> MenuItems
        {
            get
            {
                return _menuItems;
            }
        }

        #endregion

        #region Constructors

        public AppsHubPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            menuItemsListBox.ItemsSource = MenuItems;
            menuItemsListBox.SelectedIndex = 0;
        }

        private void hamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
        }

        private void menuItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainSplitView.IsPaneOpen = false;

            var selectedlistBoxItem = menuItemsListBox.SelectedItem as MenuItemModel;

            Type navigationType = null;
            switch (selectedlistBoxItem.Id)
            {
                case Constants.AppsHubFrames.Apps:
                    navigationType = typeof(Frames.AppsFrame);
                    break;

                case Constants.AppsHubFrames.About:
                    navigationType = typeof(Frames.AboutFrame);
                    break;

                case Constants.AppsHubFrames.Credits:
                    navigationType = typeof(Frames.CreditsFrame);
                    break;
            }

            contentFrame.Navigate(navigationType);
        }

        #endregion
    }
}
