namespace UwpWebApps.Models
{
    public class AppModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string BaseUrl { get; set; }

        public string AccentColor { get; set; }

        public string IconName { get; set; }

        public AppModel()
        {
            AccentColor = "Red";
            IconName = "default.png";
        }
    }
}
