using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using cqmu.MVVM.Extension;
using System.Diagnostics;
using System.IO;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;

namespace cqmu.MVVM.Service
{
    public class StorageSettings
    {
        /// <summary>
        /// 本地设置
        /// </summary>
        private static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public string BackgroundTask => "CqmuTask";

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

        private string _bgdPath;//背景图片路径

        public string BgdPath
        {
            get { return _bgdPath; }
            set
            {
                _bgdPath = value;
                LocalSettings.Values["BgdPath"] = value;
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


        public ImageBrush BgdImageBrush { get; set; }


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
                _termlyDate = Convert.ToDateTime(LocalSettings.Values["TermlyDate"]) ;
            else
                _termlyDate = new DateTime(2016,2,22);

            if (LocalSettings.Values.ContainsKey("WeekNum"))
                _weekNum = Convert.ToInt32(LocalSettings.Values["WeekNum"]);
            else
                _weekNum = 20;

            if (LocalSettings.Values.ContainsKey("BgdPath"))
                _bgdPath = LocalSettings.Values["BgdPath"].ToString();
            else
            {
                _bgdPath = "Pic_1.jpg";

                CopyImageToIsoStoreAsync().Wait();
            }

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

            RegisteBackgroundTask();
        }

        private async void RegisteBackgroundTask()
        {
            await Task.Delay(1000);

            //foreach (var cur in BackgroundTaskRegistration.AllTasks)
            //{
            //    cur.Value.Unregister(true);
            //}

            bool taskRegistered = BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == BackgroundTask);
            if (!taskRegistered)
            {
                var status = await BackgroundExecutionManager.RequestAccessAsync();
                if (status != BackgroundAccessStatus.Denied)
                {
                    var builder = new BackgroundTaskBuilder();
                    builder.Name = BackgroundTask;
                    builder.TaskEntryPoint = "BackgroundTask.CqmuTask";
                    builder.SetTrigger(new TimeTrigger(20, false));
                    //builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable, false));
                    //builder.AddCondition(new SystemCondition(SystemConditionType.FreeNetworkAvailable));
                    BackgroundTaskRegistration register = builder.Register();
                }
                else
                {
                    await new MessageDialog("后台代理被禁用").ShowAsync();
                }
            }
            else
            {
                var cur = BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name == BackgroundTask);
                BackgroundTaskRegistration task = (BackgroundTaskRegistration)cur.Value;
            }
        }

        private async Task CopyImageToIsoStoreAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Background/Pic_1.jpg", UriKind.Absolute));

            StorageFolder folder = await (await localFolder.CreateFolderAsync("cqmu", CreationCollisionOption.OpenIfExists))
                                   .CreateFolderAsync("Background", CreationCollisionOption.OpenIfExists);

            //StorageFile tempFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///Local/cqmu/Background/Pic_1.jpg"));
            //await tempFile.DeleteAsync();

            if (!await folder.FileExists(file.Name))
            {
                await file.CopyAsync(folder);
                Debug.WriteLine("Copy Succeed!!");
            }
            else
            {
                Debug.WriteLine("File Existed!!");
            }
        }

        public async Task<int> LoadBackgroundAsync()
        {
            try
            {
                BitmapImage img = new BitmapImage();
                
                img.SetSource(await (await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appdata:///Local/cqmu/Background/{BgdPath}", UriKind.Absolute))).OpenAsync(FileAccessMode.Read));

                BgdImageBrush = new ImageBrush() { ImageSource = img, Stretch = Stretch.UniformToFill };

                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return -1;
            }
        }
    }
}
