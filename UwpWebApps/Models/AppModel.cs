using Prism.Validation;
using System.ComponentModel.DataAnnotations;
using UwpWebApps.Validation;


namespace UwpWebApps.Models
{
    public class AppModel : ValidatableBindableBase
    {
        #region Fields

        private static readonly string WebAppTilePrefix = "WebApp-";

        private string _name;
        private string _baseUrl;
        private string _accentColor;
        private string _iconPath;
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
        public string IconPath
        {
            get { return _iconPath; }
            set { SetProperty(ref _iconPath, value); }
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
                IconPath = IconPath
            };
        }

        public void Copy(AppModel from)
        {
            Name = from.Name;
            AccentColor = from.AccentColor;
            BaseUrl = from.BaseUrl;
            DOMContentLoadedScript = from.DOMContentLoadedScript;
            IconPath = from.IconPath;
        }

        private void AppModel_ErrorsChanged(object sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsValid));
        }

        #endregion
    }
}
