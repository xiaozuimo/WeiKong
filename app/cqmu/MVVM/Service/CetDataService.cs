using cqmu.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace cqmu.MVVM.Service
{
    public class CetDataService : ViewModelBase
    {
        private string _txtCodePlaceholder;

        public string TxtCodePlaceholder
        {
            get { return _txtCodePlaceholder; }
            set
            {
                _txtCodePlaceholder = value;
                NotifyPropertyChanged("TxtCodePlaceholder");
            }
        }
        
        public CetDataService()
        {
            LoadData();
        }

        private async void LoadData()
        {
            TxtCodePlaceholder = "准考证号";

            if (NetworkInterface.GetIsNetworkAvailable())
                TxtCodePlaceholder = await new HttpClient().GetStringAsync(APIs.APIs.APICetQuery);
            else
            {
                await new MessageDialog("哦~啊~网络出故障了，检查下网络连接吧(T_T)！！").ShowAsync();

                TxtCodePlaceholder = "准考证号";
                //TxtCodePlaceholder = "2015年12月笔试或11月口试准考证号";
            }
        }
    }
}
