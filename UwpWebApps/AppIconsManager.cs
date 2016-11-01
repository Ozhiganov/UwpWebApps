using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.Common;
using UwpWebApps.Models;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace UwpWebApps
{
    public class AppIconsManager
    {
        #region Fields

        public static readonly string DefaultIconPath = "ms-appx:///AppIcons/default.png";
        public static readonly string IconFileExtension = ".png";

        private static AppIconsManager _instance;

        private static readonly int IconMinWidth = 128;
        private static readonly int IconMaxWidth = 512;
        private static readonly int IconMinHeight = 128;
        private static readonly int IconMaxHeight = 512;

        private static readonly string IconContentType = "image/png";

        public static readonly string AppIconsFolderName = "app_icons";
        private static readonly string AppIconsFolderUri = $"ms-appdata:///roaming/{AppIconsFolderName}/";

        #endregion

        #region Properties

        public static AppIconsManager Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppIconsManager();
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private AppIconsManager()
        {
        }

        #endregion

        #region Methods

        public async Task UploadIcon(Stream iconFileStream, AppModel appModel)
        {
            if (iconFileStream != null)
            {
                var iconFileName = $"{appModel.Id}{IconFileExtension}";
                var appIconsFolder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync(AppIconsFolderName, CreationCollisionOption.OpenIfExists);
                var iconFile = await appIconsFolder.CreateFileAsync(iconFileName, CreationCollisionOption.ReplaceExisting);

                using (var stream = await iconFile.OpenStreamForWriteAsync())
                {
                    await iconFileStream.CopyToAsync(stream);
                }
                appModel.IconPath = $"{AppIconsFolderUri}{iconFileName}";
            }
        }

        public async Task DeleteIcon(string iconPath)
        {
            if (IsAppDataIconFile(iconPath))
            {
                var storageFolder = ApplicationData.Current.LocalFolder;
                var appIconsFolder = await storageFolder.GetFolderAsync(AppIconsFolderName);

                var iconFileName = iconPath.Substring(AppIconsFolderUri.Length);
                var iconFile = await appIconsFolder.GetFileAsync(iconFileName);

                await iconFile?.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public async Task ValidateImage(StorageFile file)
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

        public static bool IsAppDataIconFile(string filePath)
        {
            return filePath?.ToLowerInvariant().StartsWith(AppIconsFolderUri) ?? false;
        }



        #endregion
    }
}
