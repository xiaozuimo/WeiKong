using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace TileUpdateService.Service
{
    public class StorageSettings
    {
        /// <summary>
        /// 本地设置
        /// </summary>
        private static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        private string _userId;//学号

        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                LocalSettings.Values["UserId"] = value;
            }
        }


        private string _userPassword;//教务处密码

        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                _userPassword = value;
                LocalSettings.Values["UserPassword"] = value;
            }
        }

        private DateTime _termlyDate;//开学日期

        public DateTime TermlyDate
        {
            get { return _termlyDate; }
            set
            {
                _termlyDate = value;
                LocalSettings.Values["TermlyDate"] = value.ToString();
            }
        }

        private int _weekNum;//每学期周数

        public int WeekNum
        {
            get { return _weekNum; }
            set
            {
                _weekNum = value;
                LocalSettings.Values["WeekNum"] = value;
            }
        }

        private bool _rememberPwd;//是否记住密码

        public bool RememberPwd
        {
            get { return _rememberPwd; }
            set
            {
                _rememberPwd = value;
                LocalSettings.Values["RememberPwd"] = value.ToString();
            }
        }

        private bool _userSignedIn;//用户是否已经登录

        public bool UserSignedIn
        {
            get { return _userSignedIn; }
            set
            {
                _userSignedIn = value;
                LocalSettings.Values["UserSignedIn"] = value.ToString();
            }
        }

        private bool _pushNotification;//是否推送通知

        public bool PushNotification
        {
            get { return _pushNotification; }
            set
            {
                _pushNotification = value;
                LocalSettings.Values["PushNotification"] = value.ToString();
            }
        }

        private DateTime _notificationDate;//推送日期

        public DateTime NotificationDate
        {
            get { return _notificationDate; }
            set
            {
                _notificationDate = value;
                LocalSettings.Values["NotificationDate"] = value.ToString();
            }
        }

        public StorageSettings()
        {
            if (LocalSettings.Values.ContainsKey("UserId"))
                _userId = LocalSettings.Values["UserId"].ToString();
            else
                _userId = "";

            if (LocalSettings.Values.ContainsKey("UserPassword"))
                _userPassword = LocalSettings.Values["UserPassword"].ToString();
            else
                _userPassword = "";

            if (LocalSettings.Values.ContainsKey("TermlyDate"))
                _termlyDate = Convert.ToDateTime(LocalSettings.Values["TermlyDate"]);
            else
                _termlyDate = new DateTime(2016, 2, 22);

            if (LocalSettings.Values.ContainsKey("WeekNum"))
                _weekNum = Convert.ToInt32(LocalSettings.Values["WeekNum"]);
            else
                _weekNum = 20;

            if (LocalSettings.Values.ContainsKey("RememberPwd"))
                _rememberPwd = Convert.ToBoolean(LocalSettings.Values["RememberPwd"]);
            else
                _rememberPwd = false;

            if (LocalSettings.Values.ContainsKey("UserSignedIn"))
                _userSignedIn = Convert.ToBoolean(LocalSettings.Values["UserSignedIn"]);
            else
                _userSignedIn = false;

            if (LocalSettings.Values.ContainsKey("PushNotification"))
                _pushNotification = Convert.ToBoolean(LocalSettings.Values["PushNotification"]);
            else
                _pushNotification = true;

            if (LocalSettings.Values.ContainsKey("NotificationDate"))
                _notificationDate = Convert.ToDateTime(LocalSettings.Values["NotificationDate"]);
            else
                _notificationDate = new DateTime(2016, 1, 1);
        }
    }
}
