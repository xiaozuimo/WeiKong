using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;

namespace cqmu.MVVM.Helper
{
    public class DeviceHelper
    {
        public static void GetCurrentDeviceInfo()
        {
            var display = DisplayInformation.GetForCurrentView();
            var width = (int)Math.Round(Window.Current.Bounds.Width * display.RawPixelsPerViewPixel);
            var height = (int)Math.Round(Window.Current.Bounds.Height * display.RawPixelsPerViewPixel);


            Debug.WriteLine("width" + width);
            Debug.WriteLine("height" + height);
        }

        public static bool IsWVGAOrWXGA()
        {
            var display = DisplayInformation.GetForCurrentView();
            var width = (int)Math.Round(Window.Current.Bounds.Width * display.RawPixelsPerViewPixel);
            var height = (int)Math.Round(Window.Current.Bounds.Height * display.RawPixelsPerViewPixel);

            if (height / 16 == width / 9)
                return false;
            return true;
        }

        public static double GetRowHeight()
        {
            var display = DisplayInformation.GetForCurrentView();
            if (IsWVGAOrWXGA())
                return (Window.Current.Bounds.Height - 76) / 11;
            return (Window.Current.Bounds.Height - 76) / 12;
        }


        public static bool IsMobile()
            =>
                Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")
                    ? true
                    : false;



    }
}
