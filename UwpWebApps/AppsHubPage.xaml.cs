﻿using System;
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
        public AppsHubPage()
        {
            this.InitializeComponent();
        }

        #region Event Handlers

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            menuItemsListBox.SelectedIndex = 0;
        }

        private void hamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
        }

        private void menuItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainSplitView.IsPaneOpen = false;

            var selectedlistBoxItem = menuItemsListBox.SelectedItem as ListBoxItem;
            var selectedFrame = selectedlistBoxItem.Tag as string;

            Type navigationType = null;
            switch (selectedFrame.ToLower())
            {
                case Constants.AppsHubFrames.Apps:
                    navigationType = typeof(AppsHubPageFrames.AppsFrame);
                    break;

                case Constants.AppsHubFrames.About:
                    navigationType = typeof(AppsHubPageFrames.AboutFrame);
                    break;

                case Constants.AppsHubFrames.Credits:
                    navigationType = typeof(AppsHubPageFrames.CreditsFrame);
                    break;
            }

            contentFrame.Navigate(navigationType);
        }

        #endregion
    }
}