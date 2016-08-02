using cqmu.MVVM.Helper;
using cqmu.MVVM.Service;
using cqmu.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
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
    public sealed partial class CoursePage : Page
    {
        public static CoursePage Current;

        public CoursePageViewModel ViewModel { get; set; }

        public CoursePage()
        {
            this.InitializeComponent();

            Current = this;
            ViewModel = new CoursePageViewModel();
            this.DataContext = ViewModel;
            
            this.Loaded += CoursePage_Loaded;

            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        }

        private async void CoursePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.AppSettings.UserSignedIn)
            {
                SignInGrid.Visibility = Visibility.Collapsed;

                TitlePanel.Visibility = Visibility.Visible;
                CourseGrid.Visibility = Visibility.Visible;
                NavigationButtonGrid.Visibility = DeviceHelper.IsMobile() ? Visibility.Collapsed : Visibility.Visible;

                ViewModel.InitializeRootGrid(CourseGrid);
            }
            else
            {
                if (App.AppSettings.RememberPwd)
                    chkRememberPwd.IsChecked = true;

                SignInGrid.Visibility = Visibility.Visible;

                TitlePanel.Visibility = Visibility.Collapsed;
                CourseGrid.Visibility = Visibility.Collapsed;
                NavigationButtonGrid.Visibility = Visibility.Collapsed;

                networkHelper = new NetworkHelper();

                await networkHelper.GetIdentifyCodeAsync();
                imgVerify.Source = networkHelper.IdentifyCodeImage;
            }
        }
        
        #region 网络请求部分
        NetworkHelper networkHelper;

        private async void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            networkHelper = new NetworkHelper();

            await networkHelper.GetIdentifyCodeAsync();
            imgVerify.Source = networkHelper.IdentifyCodeImage;
        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text) || string.IsNullOrEmpty(txtUserPwd.Password) || string.IsNullOrEmpty(txtCode.Text))
            {
                await new MessageDialog("学号、密码和验证码都不能为空哟，亲(●'◡'●)").ShowAsync();
            }
            else
            {
                SignInGrid.Visibility = Visibility.Collapsed;
                TipPanel.Visibility = Visibility.Visible;

                int status = await networkHelper.GetTermlyJsonAsync(txtUserId.Text, txtUserPwd.Password, txtCode.Text, chkRememberPwd.IsChecked.Value);

                TipPanel.Visibility = Visibility.Collapsed;

                if (status == 1)
                {
                    TitlePanel.Visibility = Visibility.Visible;
                    CourseGrid.Visibility = Visibility.Visible;
                    NavigationButtonGrid.Visibility = DeviceHelper.IsMobile() ? Visibility.Collapsed : Visibility.Visible;
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                    ViewModel.InitializeRootGrid(CourseGrid);
                }
                else if(status == 2)
                {
                    await new MessageDialog("所填信息不正确哟，亲！再检查一下下吧(●'◡'●)").ShowAsync();

                    SignInGrid.Visibility = Visibility.Visible;
                    txtCode.Text = "";

                    networkHelper = new NetworkHelper();
                    await networkHelper.GetIdentifyCodeAsync();
                    imgVerify.Source = networkHelper.IdentifyCodeImage;
                }
                else
                {
                    await new MessageDialog("喔~哦~登录超时了，别弃疗，再试一次吧！(●ˇ∀ˇ●)").ShowAsync();

                    SignInGrid.Visibility = Visibility.Visible;

                    txtCode.Text = "";

                    networkHelper = new NetworkHelper();
                    await networkHelper.GetIdentifyCodeAsync();
                    imgVerify.Source = networkHelper.IdentifyCodeImage;
                }
            }
        }
        #endregion

        private async void btnReimport_Click(object sender, RoutedEventArgs e)
        {
            //隐藏Flyout
            Flyout flyout = btnUserControl.Flyout as Flyout;
            if (flyout != null)
                flyout.Hide();

            //获取验证码
            networkHelper = new NetworkHelper();

            int status = await networkHelper.GetIdentifyCodeAsync();
            
            //处理页面元素
            txtUserId.Text = App.AppSettings.UserId;
            txtUserPwd.Password = App.AppSettings.UserPassword;
            txtCode.Text = "";

            if (!App.AppSettings.RememberPwd)
                txtUserPwd.Password = "";

            if (App.AppSettings.RememberPwd)
                chkRememberPwd.IsChecked = true;

            MenuFlyoutItem item = sender as MenuFlyoutItem;

            TitlePanel.Visibility = Visibility.Collapsed;
            CourseGrid.Visibility = Visibility.Collapsed;
            NavigationButtonGrid.Visibility = Visibility.Collapsed;

            SignInGrid.Visibility = Visibility.Visible;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            //await ApplicationData.Current.ClearAsync();//清空用户设置

            //await (await StorageFolder.GetFolderFromPathAsync("ms-appdata:///Local/cqmu")).DeleteAsync();//删除用户数据

            if (status != 1)
                return;

            imgVerify.Source = networkHelper.IdentifyCodeImage;
        }

        private void btnAddCourse_Click(object sender, RoutedEventArgs e)
        {
            //隐藏Flyout
            Flyout flyout = btnUserControl.Flyout as Flyout;
            if (flyout != null)
                flyout.Hide();

            //禁用用户选项
            btnUserControl.IsEnabled = false;

            //清空面板上的数据
            txtCourseName.Text = "";
            txtClassroom.Text = "";
            txtTeacher.Text = "";
            txtCredit.Text = "";
            txtClassify.Text = "";
            ComboDayOfWeek.SelectedIndex = -1;
            ComboCourseStart.SelectedIndex = -1;
            ComboCourseEnd.SelectedIndex = -1;
            ComboSelectedMode.SelectedIndex = -1;
            ComboWeekStart.SelectedIndex = -1;
            ComboWeekEnd.SelectedIndex = -1;

            AddCourseGrid.Visibility = Visibility.Visible;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.AppFrame.CanGoBack)
                App.AppFrame.BackStack.Clear();
            ShowAppViewBackButton();
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (App.BackRequestTarget == "Default")
            {
                e.Handled = true;

                if (App.AppSettings.UserSignedIn && SignInGrid.Visibility == Visibility.Visible)
                {
                    SignInGrid.Visibility = Visibility.Collapsed;

                    TitlePanel.Visibility = Visibility.Visible;
                    CourseGrid.Visibility = Visibility.Visible;
                    NavigationButtonGrid.Visibility = DeviceHelper.IsMobile() ? Visibility.Collapsed : Visibility.Visible;

                    ViewModel.InitializeRootGrid(CourseGrid);

                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
                //else
                //{
                //    SignInGrid.Visibility = Visibility.Visible;

                //    TitlePanel.Visibility = Visibility.Collapsed;
                //    CourseGrid.Visibility = Visibility.Collapsed;
                //    NavigationButtonGrid.Visibility = Visibility.Collapsed;

                //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                //}
            }
        }
        
    }
}
