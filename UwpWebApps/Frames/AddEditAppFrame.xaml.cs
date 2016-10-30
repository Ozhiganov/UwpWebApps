using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UwpWebApps.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace UwpWebApps.Frames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEditAppFrame : Page
    {
        #region Fields

        private static readonly int IconMinWidth = 128;
        private static readonly int IconMaxWidth = 512;
        private static readonly int IconMinHeight = 128;
        private static readonly int IconMaxHeight = 512;

        private static readonly string IconContentType = "image/png";

        private Stream _iconStream;

        #endregion

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

        #region Auxiliary

        private async Task<bool> ValidateImage(StorageFile file)
        {
            string errorMessage = null;

            if (file.ContentType != IconContentType)
            {
                errorMessage = "Selected file type does not support, please select PNG image.";
            }
            else
            {
                using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(fileStream);

                    if (bitmapImage.PixelHeight == 0 || bitmapImage.PixelWidth == 0)
                    {
                        errorMessage = "Selected file is not an image.";
                    }
                    else if (bitmapImage.PixelWidth < IconMinWidth ||
                            bitmapImage.PixelWidth > IconMaxWidth ||
                            bitmapImage.PixelHeight < IconMinHeight ||
                            bitmapImage.PixelHeight > IconMaxHeight)
                    {
                        errorMessage = $"Selected image is not in valid resolution (min. resolution: {IconMinWidth}x{IconMinHeight}, max. resolution: {IconMaxWidth}x{IconMaxHeight})";
                    }
                }
            }


            if (!string.IsNullOrEmpty(errorMessage))
            {
                var dialog = new MessageDialog(errorMessage, "Error");
                dialog.Commands.Add(new UICommand("OK"));
                dialog.CancelCommandIndex = 0;

                await dialog.ShowAsync();
                return false;
            }

            return true;
        }

        #endregion

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Model = new AppModel
            {
                Id = Guid.NewGuid().ToString(),
                IconPath = "ms-appx:///AppIcons/default.png"
            };
            if (e.Parameter is AppModel)
            {
                Model = ((AppModel)e.Parameter).Clone();
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
            if (_iconStream != null)
            {
                var appIconsFolderName = "app_icons";
                var iconFileName = $"{Model.Id}.png";

                var storageFolder = ApplicationData.Current.LocalFolder;
                var appIconsFolder = await storageFolder.CreateFolderAsync(appIconsFolderName, CreationCollisionOption.OpenIfExists);
                var iconFile = await appIconsFolder.CreateFileAsync(iconFileName, CreationCollisionOption.ReplaceExisting);

                using (var stream = await iconFile.OpenStreamForWriteAsync())
                {
                    await _iconStream.CopyToAsync(stream);
                }
                Model.IconPath = $"ms-appdata:///local/{appIconsFolderName}/{iconFileName}";
            }

            ConfigurationManager.Current.AddEditApp(Model);

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
            openPicker.FileTypeFilter.Add(".png");

            var file = await openPicker.PickSingleFileAsync();

            var type = file.ContentType;

            if (file != null)
            {
                var isIconValid = await ValidateImage(file);

                if (isIconValid)
                {
                    using (var stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(stream);
                        iconImage.Source = bitmapImage;

                        stream.Seek(0);
                        _iconStream = new MemoryStream();
                        await stream.AsStream().CopyToAsync(_iconStream);
                        _iconStream.Position = 0;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
