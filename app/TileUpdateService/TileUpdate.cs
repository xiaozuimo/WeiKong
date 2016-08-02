using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileUpdateService.Service;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace TileUpdateService
{
    public class TileUpdate
    {
        public async static Task Update()
        {
            StorageSettings settings = new StorageSettings();
            
            if (settings.UserSignedIn)
            {
                //计算当前周数以及当天是一周中的哪一天
                int daySpan = (DateTime.Today - settings.TermlyDate).Days;
                if (daySpan < 0 || daySpan / 7 > settings.WeekNum)
                    return;

                int weekOfTerm = daySpan / 7;
                int temp = (int)DateTime.Today.DayOfWeek;//0表示一周的开始(周日),所以需要根据自身需要做个修订
                int dayOfWeek = temp == 0 ? 6 : --temp;

                //获取所需课程数据 (上课20分钟内不更新)
                var weeklyList = await CourseDataService.LoadWeeklyDataFromIsoStoreAsync(weekOfTerm + 1);//当前周数据
                var dailyList = weeklyList[dayOfWeek];//当天数据
                var result = from c in dailyList
                             where !string.IsNullOrEmpty(c.FullName) && DateTime.Now < Convert.ToDateTime(c.StartTime).AddMinutes(20)
                             select c;
                Debug.WriteLine(result.Count());

                //更新磁贴
                if (result.Count() > 0)
                {
                    var c = result.First();
                    XmlDocument wideTileXML = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150IconWithBadgeAndText);
                    XmlNodeList wideTileTestAttributes = wideTileXML.GetElementsByTagName("text");
                    wideTileTestAttributes[0].AppendChild(wideTileXML.CreateTextNode(c.FullName));
                    wideTileTestAttributes[1].AppendChild(wideTileXML.CreateTextNode($"时间： {c.StartTime} - {c.EndTime}"));
                    wideTileTestAttributes[2].AppendChild(wideTileXML.CreateTextNode("地点： " + c.Classroom));

                    XmlNodeList wideTileImageAttributes = wideTileXML.GetElementsByTagName("image");
                    ((XmlElement)wideTileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Toast.png");
                    TileNotification notification = new TileNotification(wideTileXML);
                    //notification.ExpirationTime = DateTime
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
                    //TileUpdateManager.CreateTileUpdaterForSecondaryTile("SecondaryTile")?.Update(notification);

                    XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
                    XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
                    badgeElement.SetAttribute("value", result.Count().ToString());
                    BadgeNotification badge = new BadgeNotification(badgeXml);
                    BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
                    //TileUpdateManager.CreateTileUpdaterForSecondaryTile("SecondaryTile")?.Update(notification);

                    //未开放二级磁贴选项
                    //if (SecondaryTile.Exists("SecondaryTile"))
                    //{
                    //    notification = new TileNotification(wideTileXML);
                    //    badge = new BadgeNotification(badgeXml);
                    //    TileUpdateManager.CreateTileUpdaterForSecondaryTile("SecondaryTile").Update(notification);
                    //    BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile("SecondaryTile").Update(badge);
                    //}
                }
                else
                {
                    //清空磁贴
                    TileUpdateManager.CreateTileUpdaterForApplication().Clear();
                    BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

                    //if (SecondaryTile.Exists("SecondaryTile"))//应用中未设置二级磁贴
                    //{
                    //    TileUpdateManager.CreateTileUpdaterForSecondaryTile("SecondaryTile").Clear();
                    //    BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile("SecondaryTile").Clear();
                    //}

                    if (settings.PushNotification && DateTime.Now.Hour >= 21 && settings.NotificationDate != DateTime.Today)//每天晚上9点钟以后推送一次
                    {
                        //检查次日是否有课
                        if (++dayOfWeek == 7)
                        {
                            weekOfTerm++;
                            dayOfWeek = 0;
                        }
                        weeklyList = await CourseDataService.LoadWeeklyDataFromIsoStoreAsync(weekOfTerm);
                        dailyList = weeklyList[dayOfWeek];

                        result = from c in dailyList
                                 where !string.IsNullOrEmpty(c.FullName)
                                 select c;
                        Debug.WriteLine(result.Count());

                        if (result.Count() != 0)
                        {
                            settings.NotificationDate = DateTime.Today;

                            XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                            var toastTextElements = toastXML.GetElementsByTagName("text");
                            toastTextElements[0].AppendChild(toastXML.CreateTextNode(String.Format($"亲，你明天共有{result.Count()}节课，不要迟到哟！(●ˇ∀ˇ●)")));
                            ToastNotification notification = new ToastNotification(toastXML);
                            ToastNotificationManager.CreateToastNotifier().Show(notification);
                        }
                    }
                }
            }
        }

    }
}
