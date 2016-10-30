using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
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
        #region Fields

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

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Model = new AppModel
            {
                Id = Guid.NewGuid().ToString(),
                IconPath = AppIconsManager.DefaultIconPath
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
            await ConfigurationManager.Current.AddEditApp(Model, _iconStream);
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

            try
            {
                await AppIconsManager.Current.ValidateImage(file);

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
            catch (Exception exc)
            {
                var dialog = new MessageDialog(exc.Message, "Error");
                dialog.Commands.Add(new UICommand("OK"));
                dialog.CancelCommandIndex = 0;

                await dialog.ShowAsync();
            }
        }

        #endregion

        #endregion
    }
}
