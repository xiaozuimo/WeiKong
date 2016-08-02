using cqmu.MVVM.Helper;
using cqmu.MVVM.Models;
using cqmu.MVVM.Service;
using cqmu.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace cqmu.MVVM.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GradePage : Page
    {
        public static GradePage Current { get; set; }
        
        public GradePage()
        {
            this.InitializeComponent();

            Current = this;

            SystemNavigationManager.GetForCurrentView().BackRequested += GradePage_BackRequested;

            Loaded += async (s, e) =>
            {
                //请求验证码
                networkHelper = new NetworkHelper();

                await networkHelper.GetIdentifyCodeAsync();
                imgVerify.Source = networkHelper.IdentifyCodeImage;

                //处理页面元素
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("UserId") &&
                    ApplicationData.Current.LocalSettings.Values.ContainsKey("UserPassword"))
                {
                    txtUserId.Text = App.AppSettings.UserId;
                    txtUserPwd.Password = App.AppSettings.UserPassword;
                    txtCode.Text = "";

                    if (!App.AppSettings.RememberPwd)
                        txtUserPwd.Password = "";

                    //if (App.AppSettings.RememberPwd)
                    //    chkRememberPwd.IsChecked = true;
                }
            };
        }

        #region 网络请求部分
        public NetworkHelper networkHelper;

        private async void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            networkHelper = new NetworkHelper();

            await networkHelper.GetIdentifyCodeAsync();
            imgVerify.Source = networkHelper.IdentifyCodeImage;
        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text.Trim()) || string.IsNullOrEmpty(txtUserPwd.Password.Trim()) || string.IsNullOrEmpty(txtCode.Text.Trim()))
            {
                await new MessageDialog("学号、密码和验证码都不能为空哟，亲(●'◡'●)").ShowAsync();
            }
            else
            {
                SignInGrid.Visibility = Visibility.Collapsed;
                TipPanel.Visibility = Visibility.Visible;
                
                string result = await networkHelper.GetGradeJsonAsync(txtUserId.Text.Trim(), txtUserPwd.Password.Trim(), txtCode.Text.Trim());

                TipPanel.Visibility = Visibility.Collapsed;

                if (result.Contains("["))
                {
                    MasterFrame.Visibility = Visibility.Visible;
                    DetailFrame.Visibility = Visibility.Collapsed;

                    App.BackRequestTarget = "Grade";

                    if (DeviceHelper.IsMobile())
                    {
                        MainPage.Current.btnBurgerMenu.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Trim()))
                    {
                        await new MessageDialog(result).ShowAsync();
                    }

                    SignInGrid.Visibility = Visibility.Visible;

                    txtCode.Text = "";

                    networkHelper = new NetworkHelper();
                    await networkHelper.GetIdentifyCodeAsync();
                    imgVerify.Source = networkHelper.IdentifyCodeImage;
                }
            }
        }
        #endregion

        private void UpdateUI()
        {
            DetailFrame.Visibility = DetailFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;

            //if (DetailFrame.Visibility == Visibility.Visible)
            //{
            //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = DetailFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            //}
            //else
            //{
            //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = MainPage.Current.MainFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            //}

            //if (DetailFrame.Visibility == Visibility.Visible && AdaptiveStates.CurrentState.Name == "NarrowState")
            if (SignInGrid.Visibility == Visibility.Collapsed)
            {
                App.BackRequestTarget = "Grade";

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                App.BackRequestTarget = "Default";

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = MainPage.Current.MainFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateUI();
        }
        
        private void DetailFrame_Navigated(object sender, NavigationEventArgs e)
        {
            UpdateUI();
        }

        private async void GradePage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (DetailFrame.Visibility == Visibility.Visible && AdaptiveStates.CurrentState.Name == "NarrowState")
            {
                e.Handled = true;
                //App.BackRequestTarget = "Default";

                DetailFrame.Visibility = Visibility.Collapsed;

                while (DetailFrame.CanGoBack)
                    DetailFrame.BackStack.Clear();
                while (MasterFrame.CanGoBack)
                    MasterFrame.BackStack.Clear();
            }
            else if(SignInGrid.Visibility == Visibility.Collapsed)
            {
                e.Handled = true;
                App.BackRequestTarget = "Default";

                MasterFrame.Visibility = Visibility.Collapsed;
                DetailFrame.Visibility = Visibility.Collapsed;
                SignInGrid.Visibility = Visibility.Visible;

                txtUserId.Text = App.AppSettings.UserId;
                txtUserPwd.Password = App.AppSettings.UserPassword;
                txtCode.Text = "";

                if (!App.AppSettings.RememberPwd)
                    txtUserPwd.Password = "";

                networkHelper = new NetworkHelper();
                await networkHelper.GetIdentifyCodeAsync();
                imgVerify.Source = networkHelper.IdentifyCodeImage;

                MainPage.Current.btnBurgerMenu.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            App.BackRequestTarget = "Default";

            MasterFrame.Visibility = Visibility.Collapsed;
            DetailFrame.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MasterFrame.Navigate(typeof(GradeMasterPage));
            DetailFrame.Navigate(typeof(GradeDetailPage));
        }
    }
}
