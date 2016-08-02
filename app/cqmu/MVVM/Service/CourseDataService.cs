using cqmu.MVVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace cqmu.MVVM.Service
{
    public class CourseDataService
    {
        //public async Task GetCourseDataFromServer(string userId, string userPwd, string code)
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    dic.Add("user", userId);
        //}

        public async static Task<int> SaveTermlyJsonToIsoStoreAsync(string termlyJson)
        {
            try
            {
                //将用户登录状态设置为已登录
                App.AppSettings.UserSignedIn = true;

                //将整个学期的数据存一份，便于以后再次解析(如：将周日作为一周的开始)
                JObject json = JObject.Parse(termlyJson);

                StorageFolder folder = await (await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("cqmu", CreationCollisionOption.OpenIfExists))
                                       .CreateFolderAsync(App.AppSettings.UserId, CreationCollisionOption.OpenIfExists))
                                       .CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);

                StorageFile file = await folder.CreateFileAsync("TermlyData.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, termlyJson);

                //将整份Json以周为单位进行拆分，并分别存放
                for(int i = 0; i < App.AppSettings.WeekNum; i++)
                {
                    StorageFile weeklyFile = await folder.CreateFileAsync($"WeeklyData_{i}.txt",CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(weeklyFile, json["TermlyData"][i].ToString());
                }

                return 1;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return -1;
            }
        }

        public async static Task<List<List<Course>>> LoadWeeklyDataFromIsoStoreAsync(List<List<Course>> weeklyList, int currentWeekNum)
        {
            try
            {
                string[] courseStartTime = new string[] { "8:30", "9:15", "10:05", "10:55", "11:40", "14:00", "14:45", "15:35", "16:20", "19:00", "19:40", "20:20" };
                string[] courseEndTime = new string[] { "9:10", "9:55", "10:45", "11:35", "12:20", "14:40", "15:25", "16:15", "17:00", "19:40", "20:20", "21:00" };

                weeklyList = new List<List<Course>>();//保证列表为空，防止课程累加导致列表过长

                //StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appdata:///Local/cqmu/{App.AppSettings.UserId}/Data/WeeklyData_{currentWeekNum - 1}.txt", UriKind.Absolute)).GetResults();
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appdata:///Local/cqmu/{App.AppSettings.UserId}/Data/WeeklyData_{currentWeekNum - 1}.txt", UriKind.Absolute));
                
                string weeklyData = await FileIO.ReadTextAsync(file);

                JObject json = JObject.Parse(weeklyData);
                for(int i = 0; i < json["WeeklyData"].Count(); i++)
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
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                
                return null;
            }
        } 

        //对将要添加的课程进行加工
        public async static Task<string> ProcessCourse(Course processCourse, int mode, int dayOfWeek, int weekStart, int weekEnd)
        {
            try
            {
                //1.将整学期课表的Json转化为List
                var termlyList = await ConvertTermlyJsonToListAsync();

                if (termlyList != null)
                {
                    //2.把将要添加的课程融入List
                    for (int i = weekStart; i <= weekEnd; i++)
                    {
                        if ((mode == 0 && i % 2 == 0) ||
                            (mode == 1 && i % 2 == 1) ||
                             mode == 2)
                        {
                            for (int m = processCourse.StartMark; m < processCourse.StartMark + processCourse.CourseSpan; m++)
                                termlyList[i][dayOfWeek][m] = processCourse;
                        }
                    }

                    //3.将更新后的List序列化为Json，并返回
                    return ConvertTermlyListToJson(termlyList);
                }
                else
                {
                    await new MessageDialog("发生未知错误，请重新试试吧::>_<::").ShowAsync();

                    return null;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }

        public async static Task DeleteCourse(List<List<Course>> weeklyList, int dayIndex, int courseIndex, int currentWeekNum)
        {
            try
            {
                //获取到修改后当前周的课表List
                weeklyList[dayIndex].RemoveAt(courseIndex);

                var completelyWeeklyList = new List<List<Course>>();

                for (int i = 0; i < 7; i++)
                {
                    List<Course> dailyList = new List<Course>();

                    for (int j = 0; j < 12; j++)
                    {
                        dailyList.Add(new Course());
                    }

                    for (int m = 0; m < weeklyList[i].Count(); m++)
                    {
                        for (int n = weeklyList[i][m].StartMark; n < weeklyList[i][m].StartMark + weeklyList[i][m].CourseSpan; n++)
                        {
                            dailyList[n] = weeklyList[i][m];
                        }
                    }

                    completelyWeeklyList.Add(dailyList);
                }

                //将旧的Json替换掉
                var termlyList = await ConvertTermlyJsonToListAsync();
                termlyList[currentWeekNum - 1] = completelyWeeklyList;

                string termlyJson = ConvertTermlyListToJson(termlyList);

                await SaveTermlyJsonToIsoStoreAsync(termlyJson);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async static Task<List<List<List<Course>>>> ConvertTermlyJsonToListAsync()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appdata:///Local/cqmu/{App.AppSettings.UserId}/Data/TermlyData.txt", UriKind.Absolute));
                JObject oldTermlyJson = JObject.Parse(await FileIO.ReadTextAsync(file));

                var termlyList = new List<List<List<Course>>>();

                for (int i = 0; i < App.AppSettings.WeekNum; i++)
                {
                    var weeklyList = new List<List<Course>>();

                    for (int j = 0; j < oldTermlyJson["TermlyData"][i]["WeeklyData"].Count(); j++)
                    {
                        var dailyList = new List<Course>();

                        for (int k = 0; k < oldTermlyJson["TermlyData"][i]["WeeklyData"][j]["DailyData"].Count(); k++)
                        {
                            Course course = new Course();

                            course.FullName = oldTermlyJson["TermlyData"][i]["WeeklyData"][j]["DailyData"][k]["FullName"].ToString();
                            course.Teacher = oldTermlyJson["TermlyData"][i]["WeeklyData"][j]["DailyData"][k]["Teacher"].ToString();
                            course.Classroom = oldTermlyJson["TermlyData"][i]["WeeklyData"][j]["DailyData"][k]["Classroom"].ToString();
                            course.Credits = oldTermlyJson["TermlyData"][i]["WeeklyData"][j]["DailyData"][k]["Credits"].ToString();
                            course.Classify = oldTermlyJson["TermlyData"][i]["WeeklyData"][j]["DailyData"][k]["Classify"].ToString();

                            dailyList.Add(course);
                        }

                        weeklyList.Add(dailyList);
                    }

                    termlyList.Add(weeklyList);
                }

                return termlyList;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }

        private static string ConvertTermlyListToJson(List<List<List<Course>>> termlyList)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonTextWriter jsonTextWriter = new JsonTextWriter(sw);

            jsonTextWriter.WriteStartObject();
            jsonTextWriter.WritePropertyName("TermlyData");
            jsonTextWriter.WriteStartArray();

            for (int n = 0; n < termlyList.Count; n++)
            {
                jsonTextWriter.WriteStartObject();
                jsonTextWriter.WritePropertyName("WeeklyData");
                jsonTextWriter.WriteStartArray();

                for (int i = 0; i < termlyList[n].Count; i++)
                {
                    jsonTextWriter.WriteStartObject();
                    jsonTextWriter.WritePropertyName("DailyData");
                    jsonTextWriter.WriteStartArray();

                    for (int j = 0; j < termlyList[n][i].Count; j++)
                    {
                        jsonTextWriter.WriteStartObject();

                        jsonTextWriter.WritePropertyName("FullName");
                        jsonTextWriter.WriteValue(termlyList[n][i][j].FullName);
                        jsonTextWriter.WritePropertyName("Classroom");
                        jsonTextWriter.WriteValue(termlyList[n][i][j].Classroom);
                        jsonTextWriter.WritePropertyName("Teacher");
                        jsonTextWriter.WriteValue(termlyList[n][i][j].Teacher);
                        jsonTextWriter.WritePropertyName("Credits");
                        jsonTextWriter.WriteValue(termlyList[n][i][j].Credits);
                        jsonTextWriter.WritePropertyName("Classify");
                        jsonTextWriter.WriteValue(termlyList[n][i][j].Classify);
                        jsonTextWriter.WritePropertyName("CourseCode");
                        jsonTextWriter.WriteValue(termlyList[n][i][j].CourseCode);

                        jsonTextWriter.WriteEndObject();
                    }
                    jsonTextWriter.WriteEndArray();
                    jsonTextWriter.WriteEndObject();
                }
                jsonTextWriter.WriteEndArray();
                jsonTextWriter.WriteEndObject();
            }
            jsonTextWriter.WriteEndArray();
            jsonTextWriter.WriteEndObject();

            string termlyCourseJson = sb.ToString();

            return termlyCourseJson;
        }
    }
}
