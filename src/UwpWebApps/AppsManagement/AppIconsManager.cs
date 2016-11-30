using System;
using System.IO;
using System.Threading.Tasks;
using UwpWebApps.Models;
using Windows.Storage;


namespace UwpWebApps.AppsManagement
{
    public class AppIconsManager
    {
        #region Fields

        private static AppIconsManager _instance;

        public static readonly string DefaultTileIconPath = "ms-appx:///Resources/AppIcons/default_tile.png";
        public static readonly string DefaultListIconPath = "ms-appx:///Resources/AppIcons/default_list.png";
        public static readonly string IconFileExtension = ".png";

        public static readonly string AppIconsFolderName = "app_icons";
        private static readonly string AppIconsRoamingFolderUri = $"ms-appdata:///roaming/{AppIconsFolderName}/";
        private static readonly string AppIconsLocalFolderUri = $"ms-appdata:///local/{AppIconsFolderName}/";

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

        public async Task UploadIcons(AppModel appModel)
        {
            await UploadIcon(
                appModel.TileIconUploadStream,
                $"{appModel.Id}-tile{IconFileExtension}",
                (iconPath) => appModel.TileIconPath = iconPath);

            await UploadIcon(
                appModel.ListIconUploadStream,
                $"{appModel.Id}-list{IconFileExtension}",
                (iconPath) => appModel.ListIconPath = iconPath);
        }

        private async Task UploadIcon(Stream iconStream, string iconFileName, Action<string> iconPathSetter)
        {
            if (iconStream != null)
            {
                var appIconsFolder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync(AppIconsFolderName, CreationCollisionOption.OpenIfExists);
                var iconFile = await appIconsFolder.CreateFileAsync(iconFileName, CreationCollisionOption.ReplaceExisting);

                using (var stream = await iconFile.OpenStreamForWriteAsync())
                {
                    await iconStream.CopyToAsync(stream);
                }
                iconPathSetter($"{AppIconsRoamingFolderUri}{iconFileName}");
            }
        }

        public async Task DeleteIcon(string iconPath)
        {
            if (IsRoamingAppDataIconFile(iconPath))
            {
                var iconFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(iconPath));
                await iconFile?.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public async Task<string> CopyToLocalAppData(string iconFilePath)
        {
            if (IsRoamingAppDataIconFile(iconFilePath))
            {
                var roamingIconFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(iconFilePath));
                var localFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(AppIconsFolderName, CreationCollisionOption.OpenIfExists);
                var localIconFile = await roamingIconFile.CopyAsync(localFolder, roamingIconFile.Name, NameCollisionOption.ReplaceExisting);
                return AppIconsLocalFolderUri + localIconFile.Name;
            }
            return iconFilePath;
        }

        private static bool IsRoamingAppDataIconFile(string filePath)
        {
            return filePath?.ToLowerInvariant().StartsWith(AppIconsRoamingFolderUri) ?? false;
        }

        #endregion
    }
}
