using cqmu.MVVM.Service;
using cqmu.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class GradeDetailPage : Page
    {
        public static GradeDetailPage Current { get; set; }

        public GradeDetailPage()
        {
            this.InitializeComponent();

            Current = this;
            
            //lvGradeDetail.ItemsSource = GradeDetailPageViewModel.GradeCollection;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter as string))
            {
                string parameter = (e.Parameter as string).Replace(" ", "");

                if (parameter.Length == 11)
                {
                    string schoolYear = parameter.Remove(9);
                    string term = parameter.Substring(10);

                    GradeTipPanel.Visibility = Visibility.Visible;

                    string result = await GradePage.Current.networkHelper.GetGradeJsonAgainAsync(GradePage.Current.networkHelper, schoolYear, term);

                    GradeTipPanel.Visibility = Visibility.Collapsed;

                    lvGradeDetail.ItemsSource = GradeDataService.ParseGradeDataAsync(result);
                }
            }

        }

    }
}
