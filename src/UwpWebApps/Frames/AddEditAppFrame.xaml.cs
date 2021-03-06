﻿using System;
using System.Linq;
using System.Reflection;
using UwpWebApps.AppsManagement;
using UwpWebApps.Models;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace UwpWebApps.Frames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEditAppFrame : Page
    {
        #region Properties

        private AppModel Model; 

        #endregion

        #region Constructors

        public AddEditAppFrame()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Model = new AppModel
            {
                Id = Guid.NewGuid().ToString(),
                TileIconPath = AppIconsManager.DefaultTileIconPath,
                ListIconPath = AppIconsManager.DefaultListIconPath
            };
            if (e.Parameter is AppModel)
            {
                TopAppBar.Content = "Edit App";
                Model = ((AppModel)e.Parameter).Clone();
            }
            else
            {
                TopAppBar.Content = "Add App";
            }

            DataContext = Model;

            var colors = typeof(Colors).GetTypeInfo().DeclaredProperties.Select(x => x.Name).ToList();
            if (!colors.Any(x => x == Model.AccentColor))
            {
                colors.Insert(0, Model.AccentColor);
            }
            accentColorComboBox.ItemsSource = colors;
        }

        private async void saveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await (await AppsManager.GetCurrent()).AddEditApp(Model);
            Frame.Navigate(typeof(AppsFrame));
        }

        private void cancelAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void selectIconButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(AppIconsManager.IconFileExtension);

            var file = await openPicker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            try
            {
                var contentControl = (ContentControl)sender;
                var iconType = (AppModel.IconType)Enum.Parse(typeof(AppModel.IconType), (string)contentControl.Tag);
                await Model.SetIcon(file, iconType);

                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(stream);
                    var sourceImage = contentControl.Content as Image;
                    sourceImage.Source = bitmapImage;
                }
            }
            catch (Exception exc)
            {
                var dialog = new MessageDialog(exc.Message, "Error");
                dialog.Commands.Add(new UICommand("OK"));
                dialog.CancelCommandIndex = 0;

                await dialog.ShowAsync();
            }
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Tab)
            {
                var textBox = (TextBox)e.OriginalSource;
                var originalStartPosition = textBox.SelectionStart;

                // SelectionStart treats "\r\n" as a single character.
                // So if you've a TextBox with just the text "\r\n" and the cursor is at the end, SelectionStart is
                // - for a UWP-app: 1
                // - for a WPF-app: 2
                // => so for a UWP-app, we need to solve this:
                var startPosition = GetRealStartPositionTakingCareOfNewLines(originalStartPosition, textBox.Text);

                var beforeText = textBox.Text.Substring(0, startPosition);
                var afterText = textBox.Text.Substring(startPosition, textBox.Text.Length - startPosition);
                var tabSpaces = 4;
                var tab = new string(' ', tabSpaces);
                textBox.Text = beforeText + tab + afterText;
                textBox.SelectionStart = originalStartPosition + tabSpaces;

                e.Handled = true;
            }
        }

        #endregion

        #region Helpers

        private int GetRealStartPositionTakingCareOfNewLines(int startPosition, string text)
        {
            int newStartPosition = startPosition;
            int currentPosition = 0;
            bool previousWasReturn = false;
            foreach (var character in text)
            {
                if (character == '\n')
                {
                    if (previousWasReturn)
                    {
                        newStartPosition++;
                    }
                }

                if (newStartPosition <= currentPosition)
                {
                    break;
                }

                if (character == '\r')
                {
                    previousWasReturn = true;
                }
                else
                {
                    previousWasReturn = false;
                }

                currentPosition++;
            }

            return newStartPosition;
        }

        #endregion

        #endregion
    }
}
