using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UwpWebApps.Models;
using Windows.UI.StartScreen;

namespace UwpWebApps.AppsManagement
{
    public class AppsJumpListManager
    {
        #region Fields

        private static AppsJumpListManager _instance;

        #endregion

        #region Properties

        public static AppsJumpListManager Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppsJumpListManager();
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private AppsJumpListManager()
        {
        }

        #endregion

        #region Methods

        public async Task UpdateList(IEnumerable<AppModel> apps)
        {
            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();

            foreach (var app in apps)
            {
                JumpListItem jumpBoxItem = JumpListItem.CreateWithArguments(app.Id, app.Name);
                jumpBoxItem.Logo = new Uri(app.IconPath);
                jumpBoxItem.GroupName = "Apps";
                jumpList.Items.Add(jumpBoxItem);
            }

            await jumpList.SaveAsync();
        }

        #endregion
    }
}
