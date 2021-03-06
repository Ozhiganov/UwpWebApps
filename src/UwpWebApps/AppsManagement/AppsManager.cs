﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UwpWebApps.Models;
using Windows.Storage;


namespace UwpWebApps.AppsManagement
{
    public class AppsManager
    {
        #region Fields

        private static AppsManager _instance;

        private static readonly AppModel[] DefaultApps = new[]
        {
            new AppModel
            {
                Id = "5F89914B-E167-44E4-B6EE-F5211F977633",
                Name = "YouTube",
                BaseUrl = "https://youtube.com/",
                AccentColor = "#CC181E",
                TileIconPath = "ms-appx:///Resources/AppIcons/youtube_tile.png",
                ListIconPath = "ms-appx:///Resources/AppIcons/youtube_list.png"
            },
            new AppModel
            {
                Id = "4B298274-0F6F-4558-8AB5-58160736EF62",
                Name = "Google Maps",
                BaseUrl = "https://www.google.com.ua/maps",
                AccentColor = "#1CA261",
                TileIconPath = "ms-appx:///Resources/AppIcons/google-maps_tile.png",
                ListIconPath = "ms-appx:///Resources/AppIcons/google-maps_list.png",
                DOMContentLoadedScript =
@"hideElement('#gbwa');"
            },
            new AppModel
            {
                Id = "660FD349-BF1A-4F2B-8909-C4C872AA72B7",
                Name = "Google Translate",
                BaseUrl = "https://translate.google.com/",
                AccentColor = "#4889F0",
                TileIconPath = "ms-appx:///Resources/AppIcons/google-translate_tile.png",
                ListIconPath = "ms-appx:///Resources/AppIcons/google-translate_list.png",
                DOMContentLoadedScript =
@"hideElement('#gbwa');
removeElement('#gt-ft-res');
removeElement('#gt-ft');"
            },
            new AppModel
            {
                Id = "DDD64EEA-ED01-4C83-9A3A-AC986EA07EBB",
                Name = "Google Photos",
                BaseUrl = "https://photos.google.com/",
                AccentColor = "#FFA013",
                TileIconPath = "ms-appx:///Resources/AppIcons/google-photos_tile.png",
                ListIconPath = "ms-appx:///Resources/AppIcons/google-photos_list.png",
                DOMContentLoadedScript =
@"hideElement('#gbwa');"
            },
            new AppModel
            {
                Id = "DC743BD9-916B-460C-8093-781FA0F97206",
                Name = "Google Play Books",
                BaseUrl = "https://play.google.com/books",
                AccentColor = "#3FDBFE",
                TileIconPath = "ms-appx:///Resources/AppIcons/google-play-books_tile.png",
                ListIconPath = "ms-appx:///Resources/AppIcons/google-play-books_list.png",
                DOMContentLoadedScript =
@"changeLinkUrl('#gbl', '/books');
hideElement('#gbwa');
removeElement('.show-all-hover-zone');
removeElements('.nav-list-item.id-track-click.hidden-item');"
            },
            new AppModel
            {
                Id = "697B0E31-E65E-476F-A746-E676C12B23A6",
                Name = "Lingualeo",
                BaseUrl = "https://lingualeo.com",
                AccentColor = "#48B484",
                TileIconPath = "ms-appx:///Resources/AppIcons/lingualeo_tile.png",
                ListIconPath = "ms-appx:///Resources/AppIcons/lingualeo_list.png"
            }
        };

        private static readonly string WebAppsListFilename = "WebAppsList.json";

        private ObservableCollection<AppModel> _apps;
        private AppTilesManager _appTilesManager;
        private AppIconsManager _appIconsManager;
        private AppsJumpListManager _appsJumpListManager;

        #endregion

        #region Properties

        public async static Task<AppsManager> GetCurrent()
        {
            if (_instance == null)
            {
                _instance = new AppsManager();
                await _instance.LoadAppsFromFile();
            }
                
            return _instance;
        }

        #endregion

        #region Constructors

        private AppsManager()
        {
            _appTilesManager = AppTilesManager.Current;
            _appIconsManager = AppIconsManager.Current;
            _appsJumpListManager = AppsJumpListManager.Current;
        }

        #endregion

        #region Methods

        public IEnumerable<AppModel> GetApps()
        {
            return _apps;
        }

        public AppModel GetApp(string id)
        {
            var targetApp = _apps.FirstOrDefault(x => x.Id == id);
            if (targetApp == null)
            {
                throw new InvalidOperationException($"The App with id {id} does not exist.");
            }

            return targetApp;
        }

        public async void RemoveApp(string id)
        {
            var targetApp = _apps.SingleOrDefault(x => x.Id == id);

            if (targetApp == null)
            {
                throw new InvalidOperationException($"The app with id {id} does not exist.");
            }

            _apps.Remove(targetApp);
            await SaveAppsToFile();

            await _appTilesManager.DeleteTile(targetApp.TileId);
            await _appIconsManager.DeleteIcon(targetApp.TileIconPath);
            await _appIconsManager.DeleteIcon(targetApp.ListIconPath);
            await _appsJumpListManager.UpdateList(GetApps());
        }

        public async Task AddEditApp(AppModel app)
        {
            await _appIconsManager.UploadIcons(app);

            var existingApp = _apps.SingleOrDefault(x => x.Id == app.Id);
            if (existingApp == null) // Create
            {
                if (string.IsNullOrEmpty(app.Id))
                {
                    app.Id = Guid.NewGuid().ToString();
                }

                _apps.Add(app);
            }
            else // Update
            {
                existingApp.Copy(app);
                await _appTilesManager.UpdateTile(existingApp);
            }

            await SaveAppsToFile();
            await _appsJumpListManager.UpdateList(GetApps());
        }

        protected async Task SaveAppsToFile()
        {
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<AppModel>));
            var content = string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _apps);
                stream.Position = 0;
                content = new StreamReader(stream).ReadToEnd();
            }

            var webAppsFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(WebAppsListFilename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(webAppsFile, content);
        }

        protected async Task LoadAppsFromFile()
        {
            var appsListFile = await ApplicationData.Current.RoamingFolder.TryGetItemAsync(WebAppsListFilename);
            
            if (appsListFile == null)
            {
                _apps = new ObservableCollection<AppModel>(DefaultApps);
            }
            else
            {
                var serializer = new DataContractJsonSerializer(typeof(IEnumerable<AppModel>));
                var content = string.Empty;

                var appsString = await FileIO.ReadTextAsync((IStorageFile)appsListFile);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(appsString ?? string.Empty)))
                {
                    var apps = serializer.ReadObject(stream) as IEnumerable<AppModel>;
                    _apps = new ObservableCollection<AppModel>(apps);
                }
            }
        }

        #endregion
    }
}
