using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileUpdateService.Models;
using Windows.Storage;

namespace TileUpdateService.Service
{
    public class CourseDataService
    {
        public async static Task<List<List<Course>>> LoadWeeklyDataFromIsoStoreAsync(int currentWeekNum)
        {
            try
            {
                StorageSettings settings = new StorageSettings();

                string[] courseStartTime = new string[] { "8:30", "9:15", "10:05", "10:55", "11:40", "14:00", "14:45", "15:35", "16:20", "19:00", "19:40", "20:20" };
                string[] courseEndTime = new string[] { "9:10", "9:55", "10:45", "11:35", "12:20", "14:40", "15:25", "16:15", "17:00", "19:40", "20:20", "21:00" };

                List<List<Course>> weeklyList = new List<List<Course>>();//保证列表为空，防止课程累加导致列表过长

                //StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appdata:///Local/cqmu/{App.AppSettings.UserId}/Data/WeeklyData_{currentWeekNum - 1}.txt", UriKind.Absolute)).GetResults();
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appdata:///Local/cqmu/{settings.UserId}/Data/WeeklyData_{currentWeekNum - 1}.txt", UriKind.Absolute));

                string weeklyData = await FileIO.ReadTextAsync(file);

                JObject json = JObject.Parse(weeklyData);
                for (int i = 0; i < json["WeeklyData"].Count(); i++)
                {
                    List<Course> dailyList = new List<Course>();

                    for (int j = 0; j < json["WeeklyData"][i]["DailyData"].Count() - 1; j++)
                    {
                        Course course = new Course();

                        course.FullName = json["WeeklyData"][i]["DailyData"][j]["FullName"].ToString();
                        course.Teacher = json["WeeklyData"][i]["DailyData"][j]["Teacher"].ToString();
                        course.Classroom = json["WeeklyData"][i]["DailyData"][j]["Classroom"].ToString();
                        course.Credits = json["WeeklyData"][i]["DailyData"][j]["Credits"].ToString();
                        course.Classify = json["WeeklyData"][i]["DailyData"][j]["Classify"].ToString();
                        course.StartMark = j;
                        course.StartTime = courseStartTime[j];
                        course.CourseSpan = 1;

                        while (j < json["WeeklyData"][i]["DailyData"].Count() - 1 &&
                               course.FullName == json["WeeklyData"][i]["DailyData"][j + 1]["FullName"].ToString() &&
                               course.Classroom == json["WeeklyData"][i]["DailyData"][j + 1]["Classroom"].ToString() &&
                               course.Classify == json["WeeklyData"][i]["DailyData"][j + 1]["Classify"].ToString())
                        {
                            course.CourseSpan++;
                            j++;
                        }

                        course.EndTime = courseEndTime[j];

                        if (!string.IsNullOrEmpty(course.FullName))
                        {
                            dailyList.Add(course);
                        }
                    }

                    weeklyList.Add(dailyList);
                }

                return weeklyList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }
    }
}
