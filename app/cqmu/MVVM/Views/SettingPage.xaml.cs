using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using cqmu.MVVM.Extension;
using static cqmu.MVVM.Helper.ApplicationViewHelper;
using Windows.UI.Core;
using cqmu.MVVM.Helper;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace cqmu.MVVM.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private bool settingPageLoaded = false;

        public SettingPage()
        {
            this.InitializeComponent();

            switchNotification.IsOn = App.AppSettings.PushNotification;
            settingPageLoaded = true;

            tbkTitle.Margin = DeviceHelper.IsMobile() ? new Thickness(32, -5, 0, 8) : new Thickness(0, -5, 0, 8);
        }

        private void switchNotification_Toggled(object sender, RoutedEventArgs e)
        {
            if (settingPageLoaded)
                App.AppSettings.PushNotification = switchNotification.IsOn;
        }

        private async void imgDefault_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.AppSettings.BgdPath != "Pic_1.jpg")
            {
                App.AppSettings.BgdPath = "Pic_1.jpg";
                await App.AppSettings.LoadBackgroundAsync();

                App.RootGrid.Background = App.AppSettings.BgdImageBrush;
            }
        }

        private async void btnAddBackground_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            var file = await picker.PickSingleFileAsync();

            if (file == null)
                return;

            StorageFolder folder = await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("cqmu", CreationCollisionOption.OpenIfExists))
                                   .CreateFolderAsync("Background", CreationCollisionOption.OpenIfExists);

            if (await folder.FileExists("Pic_0.jpg"))
                await (await folder.GetFileAsync("Pic_0.jpg")).DeleteAsync();

            await file.CopyAsync(folder,"Pic_0.jpg");

            App.AppSettings.BgdPath = "Pic_0.jpg";
            await App.AppSettings.LoadBackgroundAsync();

            App.RootGrid.Background = App.AppSettings.BgdImageBrush;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ShowAppViewBackButton();
        }
    }
}
