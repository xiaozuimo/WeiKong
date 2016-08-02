using cqmu.MVVM.Helper;
using cqmu.MVVM.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class CetPage : Page
    {
        List<string> cetDetailList;
        
        public CetDataService Service { get; set; }

        public CetPage()
        {
            this.InitializeComponent();

            Service = new CetDataService();
            this.DataContext = Service;
        }

        public async Task LoadDataAsync(Grid rootGrid, string code, string name)
        {
            cetDetailList = new List<string>();
            cetDetailList = await new NetworkHelper().GetCetResultAsync(code, name);

            for(int i = 0; i < 8; i++)
            {
                TextBlock tbk = new TextBlock();
                tbk.Text = cetDetailList[i];
                tbk.FontSize = 24;
                tbk.TextAlignment = TextAlignment.Left;
                tbk.VerticalAlignment = VerticalAlignment.Center;
                tbk.Foreground = i < 4 ? new SolidColorBrush(Colors.Orange) : new SolidColorBrush(Colors.White);

                Grid.SetRow(tbk, i);
                Grid.SetColumn(tbk, 1);
                rootGrid.Children.Add(tbk);
            }
        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCode.Text) || string.IsNullOrEmpty(txtName.Text))
            {
                await new MessageDialog("准考证号和姓名都不能为空哟，亲(●'◡'●)").ShowAsync();
            }

            if (txtName.Text.Length < 2)
            {
                await new MessageDialog("很抱歉，如此短的姓名导致我们无法查询到您的成绩::>_<::").ShowAsync();

                return;
            }

            SignInGrid.Visibility = Visibility.Collapsed;
            TipPanel.Visibility = Visibility.Visible;

            await LoadDataAsync(detailGrid, txtCode.Text.Trim(), txtName.Text.Trim().Substring(0, 2));

            svDetail.Visibility = Visibility.Visible;
            TipPanel.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ShowAppViewBackButton();
        }
    }
}
