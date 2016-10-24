using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpWebApps.Models;


namespace UwpWebApps
{
    public class ConfigurationManager
    {
        #region Fields

        private static ConfigurationManager _instance;

        List<AppModel> _apps = new List<AppModel>
            {
                new AppModel()
                {
                    Id = "5F89914B-E167-44E4-B6EE-F5211F977633",
                    Name = "YouTube",
                    BaseUrl = "https://youtube.com/",
                    AccentColor = "#CC181E",
                    IconName = "youtube.png"
                },

                new AppModel()
                {
                    Id = "4B298274-0F6F-4558-8AB5-58160736EF62",
                    Name = "Google Maps",
                    BaseUrl = "https://www.google.com.ua/maps",
                    AccentColor = "#1CA261",
                    IconName = "google-maps.png"
                },

                new AppModel()
                {
                    Id = "660FD349-BF1A-4F2B-8909-C4C872AA72B7",
                    Name = "Google Translate",
                    BaseUrl = "https://translate.google.com/",
                    AccentColor = "#4889F0",
                    IconName = "google-translate.png",
                    DOMContentLoadedScript =
@"
function removeElement(el) {
    el.parentNode.removeChild(el);
}

var googleAppsButton = document.getElementById('gbwa');
googleAppsButton.style.visibility = 'hidden';
        
removeElement(document.getElementById('gt-ft-res'));
removeElement(document.getElementById('gt-ft'));
"
                },

                new AppModel()
                {
                    Id = "DDD64EEA-ED01-4C83-9A3A-AC986EA07EBB",
                    Name = "Google Photos",
                    BaseUrl = "https://photos.google.com/",
                    AccentColor = "#FFA013",
                    IconName = "google-photos.png",
                    DOMContentLoadedScript =
@"
function removeElement(el) {
    el.parentNode.removeChild(el);
}

var googleAppsButton = document.getElementById('gbwa');
googleAppsButton.style.visibility = 'hidden';
"
                },

                new AppModel()
                {
                    Id = "DC743BD9-916B-460C-8093-781FA0F97206",
                    Name = "Google Play Books",
                    BaseUrl = "https://books.google.com/ebooks/app",
                    AccentColor = "#3FDBFE",
                    IconName = "google-play-books.png"
                },

                new AppModel()
                {
                    Id = "697B0E31-E65E-476F-A746-E676C12B23A6",
                    Name = "Lingualeo",
                    BaseUrl = "https://lingualeo.com",
                    AccentColor = "#48B484",
                    IconName = "lingualeo.png"
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
            var apps = GetApps();
            var targetApp = apps.FirstOrDefault(x => x.Id == id);
            if (targetApp == null)
            {
                throw new InvalidOperationException($"The App with id {id} does not exist.");
            }

            return targetApp;
        }

        #endregion
    }
}
