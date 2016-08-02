using cqmu.MVVM.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static cqmu.MVVM.Helper.ApplicationViewHelper;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace cqmu.MVVM.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();

            PackageVersion version = Package.Current.Id.Version;
            tbkVersion.Text = $"Version {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

            tbkTitle.Margin = DeviceHelper.IsMobile() ? new Thickness(32, -5, 0, 8) : new Thickness(0, -5, 0, 8);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ShowAppViewBackButton();
        }

        private async void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.Email.EmailMessage mail = new Windows.ApplicationModel.Email.EmailMessage();
            mail.To.Add(new Windows.ApplicationModel.Email.EmailRecipient("272078613@qq.com", "小醉魔"));
            mail.Subject = "用户的来信";
            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(mail);
        }

        private async void btnReview_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review?ProductId=9nblggh6gr35"));
        }

        private async void btnCommunicate_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText("519151717");
            Clipboard.SetContent(dp);

            await new MessageDialog("群号已复制到剪切板！ 欢迎加入用户交流群：519151717").ShowAsync();
        }

        private async void btnCheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp?ProductId=9nblggh6gr35"));
        }
    }
}
