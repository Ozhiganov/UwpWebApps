using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UwpWebApps.Models;


namespace UwpWebApps
{
    public class ConfigurationManager
    {
        #region Fields

        private static ConfigurationManager _instance;

        ObservableCollection<AppModel> _apps = new ObservableCollection<AppModel>
            {
                new AppModel()
                {
                    Id = "5F89914B-E167-44E4-B6EE-F5211F977633",
                    Name = "YouTube",
                    BaseUrl = "https://youtube.com/",
                    AccentColor = "#CC181E",
                    IconPath = "ms-appx:///AppIcons/youtube.png"
                },

                new AppModel()
                {
                    Id = "4B298274-0F6F-4558-8AB5-58160736EF62",
                    Name = "Google Maps",
                    BaseUrl = "https://www.google.com.ua/maps",
                    AccentColor = "#1CA261",
                    IconPath = "ms-appx:///AppIcons/google-maps.png"
                },

                new AppModel()
                {
                    Id = "660FD349-BF1A-4F2B-8909-C4C872AA72B7",
                    Name = "Google Translate",
                    BaseUrl = "https://translate.google.com/",
                    AccentColor = "#4889F0",
                    IconPath = "ms-appx:///AppIcons/google-translate.png",
                    DOMContentLoadedScript =
@"hideElementById('gbwa');
removeElementById('gt-ft-res');
removeElementById('gt-ft');"
                },

                new AppModel()
                {
                    Id = "DDD64EEA-ED01-4C83-9A3A-AC986EA07EBB",
                    Name = "Google Photos",
                    BaseUrl = "https://photos.google.com/",
                    AccentColor = "#FFA013",
                    IconPath = "ms-appx:///AppIcons/google-photos.png",
                    DOMContentLoadedScript =
@"hideElementById('gbwa');"
                },

                new AppModel()
                {
                    Id = "DC743BD9-916B-460C-8093-781FA0F97206",
                    Name = "Google Play Books",
                    BaseUrl = "https://books.google.com/ebooks/app",
                    AccentColor = "#3FDBFE",
                    IconPath = "ms-appx:///AppIcons/google-play-books.png"
                },

                new AppModel()
                {
                    Id = "697B0E31-E65E-476F-A746-E676C12B23A6",
                    Name = "Lingualeo",
                    BaseUrl = "https://lingualeo.com",
                    AccentColor = "#48B484",
                    IconPath = "ms-appx:///AppIcons/lingualeo.png"
                },
            };

        #endregion

        #region Properties

        public static ConfigurationManager Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigurationManager();
                }
                return _instance;
            }
        }

        #endregion

        #region Constructors

        private ConfigurationManager()
        {

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

        public void RemoveApp(string id)
        {
            var targetApp = _apps.SingleOrDefault(x => x.Id == id);

            if (targetApp == null)
            {
                throw new InvalidOperationException($"The app with id {id} does not exist.");
            }

            _apps.Remove(targetApp);
        }

        public void AddEditApp(AppModel app)
        {
            var existingApp = _apps.SingleOrDefault(x => x.Id == app.Id);
            if (existingApp == null)
            {
                if (string.IsNullOrEmpty(app.Id))
                {
                    app.Id = Guid.NewGuid().ToString();
                }

                _apps.Add(app);
            }
            else
            {
                existingApp.Copy(app);
            }
        }

        #endregion
    }
}
