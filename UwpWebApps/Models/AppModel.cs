namespace UwpWebApps.Models
{
    public class AppModel
    {
        #region Fields

        private static readonly string WebAppTilePrefix = "WebApp-";

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

        public string Name { get; set; }

        public string BaseUrl { get; set; }

        public string AccentColor { get; set; }

        public string IconName { get; set; }

        public string DOMContentLoadedScript { get; set; }

        #endregion

        #region Constructors

        public AppModel()
        {
            AccentColor = "Red";
            IconName = "default.png";
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
                IconName = IconName
            };
        }

        public void Copy(AppModel from)
        {
            Name = from.Name;
            AccentColor = from.AccentColor;
            BaseUrl = from.BaseUrl;
            DOMContentLoadedScript = from.DOMContentLoadedScript;
            IconName = from.IconName;
        }

        #endregion
    }
}
