using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace cqmu.MVVM.Helper
{
    public static class ApplicationViewHelper
    {
        public static void ShowAppViewBackButton()
        {
            var view = SystemNavigationManager.GetForCurrentView();
            view.AppViewBackButtonVisibility = App.AppFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        public static void CustomTitleBar()
        {
            var view = ApplicationView.GetForCurrentView();

            // active
            view.TitleBar.BackgroundColor = new Color() { A = 0xFF, R = 0x13, G = 0x59, B = 0x95 };
            view.TitleBar.ForegroundColor = Colors.White;

            // inactive
            view.TitleBar.InactiveBackgroundColor = new Color() { A = 0xFF, R = 0x13, G = 0x59, B = 0x95 };
            view.TitleBar.InactiveForegroundColor = Colors.LightGray;

            // button
            view.TitleBar.ButtonBackgroundColor = new Color() { A = 0xFF, R = 0x13, G = 0x59, B = 0x95 };
            view.TitleBar.ButtonForegroundColor = Colors.White;

            view.TitleBar.ButtonHoverBackgroundColor = Colors.OrangeRed;
            view.TitleBar.ButtonHoverForegroundColor = Colors.White;

            view.TitleBar.ButtonPressedBackgroundColor = new Color() { A = 0xFF, R = 0x13, G = 0x59, B = 0x95 };
            view.TitleBar.ButtonPressedForegroundColor = Colors.White;

            view.TitleBar.ButtonInactiveBackgroundColor = new Color() { A = 0xFF, R = 0x13, G = 0x59, B = 0x95 };
            view.TitleBar.ButtonInactiveForegroundColor = Colors.LightGray;
        }
    }
}
