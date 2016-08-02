using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace cqmu.MVVM.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GradeMasterPage : Page
    {
        public List<string> TermList { get; set; } = new List<string>();

        public GradeMasterPage()
        {
            this.InitializeComponent();

            this.DataContext = TermList;

            InitializeTermList();
        }

        private void InitializeTermList()
        {
            int yearEnd = DateTime.Today.Month >= 9 ? DateTime.Today.Year : DateTime.Today.Year - 1;
            int yearStart;

            string userIdSubstring = GradePage.Current.txtUserId.Text.Length >= 4 ? GradePage.Current.txtUserId.Text.Remove(4) : "2013";
            try //防止学号前四位不是数字
            {
                yearStart = Convert.ToInt32(userIdSubstring);
            }
            catch
            {
                yearStart = 2013;
            }

            for (int i = yearEnd; i >= yearStart; i--)
            {
                for (int j = 1; j <= 3; j++)
                {
                    TermList.Add($"{i} - {i + 1}   0{j}");
                }
            }

        }

        private void lvTerm_ItemClick(object sender, ItemClickEventArgs e)
        {
            string parameter = e.ClickedItem as string;

            GradePage.Current.DetailFrame.Navigate(typeof(GradeDetailPage), parameter);
        }
    }
}
