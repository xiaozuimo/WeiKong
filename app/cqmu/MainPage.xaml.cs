using cqmu.MVVM.Helper;
using cqmu.MVVM.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TileUpdateService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static cqmu.MVVM.Helper.ApplicationViewHelper;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace cqmu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool cancelNavigate = false;

        public static MainPage Current { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            Current = this;

            App.AppFrame = MainFrame;
            App.RootGrid = RootGrid;
            //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            CustomTitleBar();

            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            Loaded += async (s, e) =>
            {
                if (DeviceHelper.IsMobile())
                    await StatusBar.GetForCurrentView().HideAsync();

                rdoCourse.IsChecked = true;
                await TileUpdate.Update();
            };
        }

        private void btnBurgerMenu_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void rdoOption_OnChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rdo = sender as RadioButton;

            if (rdo != null && cancelNavigate == false)
            {
                switch (rdo.Tag.ToString())
                {
                    case "tagCourse":
                        MainFrame.Navigate(typeof(MVVM.Views.CoursePage));
                        break;
                    case "tagGrade":
                        MainFrame.Navigate(typeof(MVVM.Views.GradePage));
                        break;
                    case "tagCet":
                        MainFrame.Navigate(typeof(MVVM.Views.CetPage));
                        break;
                    case "tagHelp":
                        MainFrame.Navigate(typeof(MVVM.Views.HelpPage));
                        break;
                    case "tagAbout":
                        MainFrame.Navigate(typeof(MVVM.Views.AboutPage));
                        break;
                    case "tagSetting":
                        MainFrame.Navigate(typeof(MVVM.Views.SettingPage));
                        break;
                    default:
                        MainFrame.Navigate(typeof(MVVM.Views.CoursePage));
                        break;
                }
                if (MySplitView.DisplayMode != SplitViewDisplayMode.CompactInline)
                    MySplitView.IsPaneOpen = false;
            }

            cancelNavigate = false;
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (App.BackRequestTarget == "Default")
            {
                if (MainFrame == null)
                    return;

                if (MainFrame.CanGoBack)
                {
                    e.Handled = true;
                    MainFrame.GoBack();

                    cancelNavigate = true;

                    switch (MainFrame.SourcePageType.Name)
                    {
                        case "CoursePage":
                            rdoCourse.IsChecked = true;
                            break;
                        case "GradePage":
                            rdoGrade.IsChecked = true;
                            break;
                        case "CetPage":
                            rdoCet.IsChecked = true;
                            break;
                        case "HelpPage":
                            rdoHelp.IsChecked = true;
                            break;
                        case "AboutPage":
                            rdoAbout.IsChecked = true;
                            break;
                        case "SettingPage":
                            rdoSetting.IsChecked = true;
                            break;
                        default:
                            rdoCourse.IsChecked = true;
                            break;
                    }
                }
            }
        }
        
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.AppSettings.BgdImageBrush == null)
                await App.AppSettings.LoadBackgroundAsync();

            App.RootGrid.Background = App.AppSettings.BgdImageBrush;
        }
    }
}
