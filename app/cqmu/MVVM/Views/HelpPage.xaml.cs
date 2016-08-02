using cqmu.MVVM.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class HelpPage : Page
    {
        public HelpPage()
        {
            this.InitializeComponent();

            tbkTitle.Margin = DeviceHelper.IsMobile() ? new Thickness(32, -5, 0, 8) : new Thickness(0, -5, 0, 8);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ShowAppViewBackButton();
        }
    }
}
