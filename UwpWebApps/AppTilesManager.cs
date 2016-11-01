using System;
using System.Threading.Tasks;
using TAlex.Common;
using UwpWebApps.Models;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Markup;


namespace UwpWebApps
{
    public class AppTilesManager
    {
        #region Fields

        private static AppTilesManager _instance;

        private static readonly string AppIconsFolderUri = $"ms-appdata:///local/{AppIconsManager.AppIconsFolderName}/";

        #endregion

        #region Properties

        public static AppTilesManager Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppTilesManager();
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private AppTilesManager()
        {
        }

        #endregion

        #region Methods

        #region Public

        public async Task<bool> RequestCreateTile(AppModel model)
        {
            Argument.RequiresNotNull(model, nameof(model));

            var appTile = await CreateTile(model);
            return await appTile.RequestCreateAsync();
        }

        public async Task<bool> UpdateTile(AppModel model)
        {
            Argument.RequiresNotNull(model, nameof(model));

            if (SecondaryTile.Exists(model.TileId))
            {
                var appTile = await CreateTile(model);
                await appTile.UpdateAsync();
            }

            return true;
        }

        public async Task<bool> DeleteTile(string tileId)
        {
            Argument.RequiresNotNullOrEmpty(tileId, nameof(tileId));

            if (SecondaryTile.Exists(tileId))
            {
                var appTile = new SecondaryTile(tileId);
                return await appTile.RequestDeleteAsync();
            }
            return true;
        }

        #endregion

        #region Private

        private async Task<SecondaryTile> CreateTile(AppModel model)
        {
            var iconPath = model.IconPath;
            if (AppIconsManager.IsAppDataIconFile(model.IconPath))
            {
                var iconFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(model.IconPath));
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(AppIconsManager.AppIconsFolderName, CreationCollisionOption.OpenIfExists);
                var localIconFile = await iconFile.CopyAsync(folder, iconFile.Name, NameCollisionOption.ReplaceExisting);
                iconPath = AppIconsFolderUri + localIconFile.Name;
            }

            var appTile = new SecondaryTile(
                model.TileId,
                model.Name,
                model.Id,
                new Uri(iconPath),
                TileSize.Default);
            appTile.RoamingEnabled = true;
            appTile.VisualElements.BackgroundColor = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), model.AccentColor);
            appTile.VisualElements.ShowNameOnSquare150x150Logo = true;

            return appTile;
        }

        #endregion

        #endregion
    }
}
