using Prism.Windows.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UwpWebApps.Infrastructure.Validation;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;


namespace UwpWebApps.Models
{
    public class AppModel : ValidatableBindableBase
    {
        #region Fields

        private static readonly string WebAppTilePrefix = "WebApp-";

        private string _name;
        private string _baseUrl;
        private string _accentColor;
        private string _tileIconPath;
        private string _listIconPath;
        private string _domContentLoadedScript;

        #endregion

        #region Properties

        public string Id { get; set; }

        public string TileId
        {
            get
            {
                return WebAppTilePrefix + Id;
            }
        }

        [Required]
        [StringLength(30)]
        [RegularExpression("[a-zA-Z0-9_@ -]+", ErrorMessage = "The field Name contains invalid characters.")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [Required]
        [UrlValidator(ErrorMessage = "The field BaseUrl is invalid Url.")]
        public string BaseUrl
        {
            get { return _baseUrl; }
            set { SetProperty(ref _baseUrl, value); }
        }

        public string AccentColor
        {
            get { return _accentColor; }
            set { SetProperty(ref _accentColor, value); }
        }

        [Required]
        public string TileIconPath
        {
            get { return _tileIconPath; }
            set { SetProperty(ref _tileIconPath, value); }
        }

        [IgnoreDataMember]
        public Stream TileIconUploadStream
        {
            get;
            private set;
        }

        public string ListIconPath
        {
            get { return _listIconPath; }
            set { SetProperty(ref _listIconPath, value); }
        }

        [IgnoreDataMember]
        public Stream ListIconUploadStream
        {
            get;
            private set;
        }

        [StringLength(2000)]
        public string DOMContentLoadedScript
        {
            get { return _domContentLoadedScript; }
            set { SetProperty(ref _domContentLoadedScript, value); }
        }

        public bool IsValid
        {
            get
            {
                return Errors.Errors.Count == 0;
            }
        }

        #endregion

        #region Constructors

        public AppModel()
        {
            AccentColor = "DodgerBlue";

            ValidateProperties();
            ErrorsChanged += AppModel_ErrorsChanged;
        }

        #endregion

        #region Methods

        public static bool IsAppTileId(string tileId)
        {
            return tileId.StartsWith(WebAppTilePrefix);
        }

        public AppModel Clone()
        {
            return new AppModel
            {
                Id = Id,
                Name = Name,
                AccentColor = AccentColor,
                BaseUrl = BaseUrl,
                DOMContentLoadedScript = DOMContentLoadedScript,
                TileIconPath = TileIconPath,
                ListIconPath = ListIconPath
            };
        }

        public void Copy(AppModel from)
        {
            Name = from.Name;
            AccentColor = from.AccentColor;
            BaseUrl = from.BaseUrl;
            DOMContentLoadedScript = from.DOMContentLoadedScript;
            TileIconPath = from.TileIconPath;
            ListIconPath = from.ListIconPath;
        }

        public override string ToString()
        {
            return Name;
        }

        public async Task SetIcon(StorageFile iconFile, IconType iconType)
        {
            IconManager iconManager = null;
            switch (iconType)
            {
                case IconType.Tile:
                    iconManager = new TileIconManager();
                    break;

                case IconType.List:
                    iconManager = new ListIconManager();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await iconManager.SetImage(this, iconFile);
        }

        private void AppModel_ErrorsChanged(object sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsValid));
        }

        #endregion

        #region Nested Types

        public enum IconType
        {
            Tile,
            List
        }

        private abstract class IconManager
        {
            public abstract int IconMinWidth { get; }
            public abstract int IconMaxWidth { get; }
            public abstract int IconMinHeight { get; }
            public abstract int IconMaxHeight { get; }

            public abstract string IconContentType { get; }


            public async Task SetImage(AppModel model, StorageFile iconFile)
            {
                await Validate(iconFile);

                using (var iconStream = await iconFile.OpenAsync(FileAccessMode.Read))
                {
                    var memoryStream = new MemoryStream();
                    await iconStream.AsStream().CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    UpdateImage(model, memoryStream);
                }
            }

            public abstract void UpdateImage(AppModel model, Stream stream);


            private async Task Validate(StorageFile file)
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
                    throw new Exception(errorMessage);
                }
            }
        }

        private class TileIconManager : IconManager
        {
            public override int IconMinWidth { get { return 128; } }
            public override int IconMaxWidth { get { return 512; } }
            public override int IconMinHeight { get { return 128; } }
            public override int IconMaxHeight { get { return 512; } }

            public override string IconContentType { get { return "image/png"; } }

            public override void UpdateImage(AppModel model, Stream stream)
            {
                model.TileIconUploadStream = stream;
            }
        }

        private class ListIconManager : IconManager
        {
            public override int IconMinWidth { get { return 32; } }
            public override int IconMaxWidth { get { return 32; } }
            public override int IconMinHeight { get { return 32; } }
            public override int IconMaxHeight { get { return 32; } }

            public override string IconContentType { get { return "image/png"; } }

            public override void UpdateImage(AppModel model, Stream stream)
            {
                model.ListIconUploadStream = stream;
            }
        }

        #endregion
    }
}
