using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpWebApps.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace UwpWebApps.Frames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEditAppFrame : Page
    {
        #region Properties

        private AppModel Model; 

        #endregion

        #region Constructors

        public AddEditAppFrame()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Model = new AppModel();
            if (e.Parameter is AppModel)
            {
                Model = ((AppModel)e.Parameter).Clone();
            }

            DataContext = Model;

            var colors = typeof(Colors).GetTypeInfo().DeclaredProperties.Select(x => x.Name).ToList();
            if (!colors.Any(x => x == Model.AccentColor))
            {
                colors.Insert(0, Model.AccentColor);
            }
            accentColorComboBox.ItemsSource = colors;
        }

        private void saveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.Current.AddEditApp(Model);
            Frame.Navigate(typeof(AppsFrame));
        }

        private void cancelAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        #endregion
    }
}
