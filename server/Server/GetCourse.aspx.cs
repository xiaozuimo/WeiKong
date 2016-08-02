using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Server
{
    public partial class GetCourse : System.Web.UI.Page
    {
        string user, pwd, code, key, target, clientCookie;
        string viewState;
        string eventArgument, eventTarget, ddlXN, ddlXQ;
        string gradeddlXN, gradeddlXQ;
        string coursePageViewState;

        string transcriptQueryType, transcriptQueryValue;
        string transcriptViewState, transcriptHref;

        HttpClientHandler handler;
        HttpClient client;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                user = Request["user"];
                pwd = Request["pwd"];
                code = Request["code"];
                key = Request["key"];
                target = Request["target"];
                clientCookie = Request["clientCookie"];
                viewState = Request["viewState"];

                //eventArgument = Request["eventArgument"];
                //eventTarget = Request["eventTarget"];
                //ddlXN = Request["ddlXN"];
                //ddlXQ = Request["ddlXQ"];

                gradeddlXN = Request["ddlXN"];
                gradeddlXQ = Request["ddlXQ"];

                eventArgument = "";
                eventTarget = "ddlXQ";
                ddlXN = "2015-2016";
                ddlXQ = "2";

                transcriptQueryType = Request["transcriptQueryType"];
                transcriptViewState = Request["transcriptViewState"];
                transcriptHref = Request["transcriptHref"];

                if (target == "code")
                {
                    GetIdentification();
                }
                else if (target == "transcript" || target == "course")
                {
                    if (String.IsNullOrEmpty(user) || String.IsNullOrEmpty(pwd) || String.IsNullOrEmpty(key) || !CheckKey(key))
                    {
                        Response.Write("参数错误");
                        return;
                    }

                    LoadDefaultPage(user, pwd, code);
                }
                else if (target == "anotherTranscript")
                {
                    if (String.IsNullOrEmpty(key) || !CheckKey(key) || String.IsNullOrEmpty(transcriptViewState))
                    {
                        Response.Write("参数错误");
                        return;
                    }

                    GetTranscriptHtmlAgain();
                }
                else
                {
                    Response.Write("参数错误");
                }
            }
        }

        //检查是否有权限调用接口
        private bool CheckKey(string key)
        {
            List<string> list = new List<string>();
            list.Add("xiaoz");

            return list.Exists(item => item == key);
        }

        //获取验证信息，包括viewState、cookie、验证码
        private async void GetIdentification()
        {
            target = null;
            viewState = "";
            try
            {
                handler = new HttpClientHandler();
                client = new HttpClient(handler);
                
                string data = await client.GetStringAsync("登录接口");
                viewState = Regex.Match(data, "name=\"__VIEWSTATE\" value=\"(.+)\"").Groups[1].Value;

                var res = await client.GetByteArrayAsync("登录接口");
                CookieCollection cookieCollection = handler.CookieContainer.GetCookies(new Uri("登录接口", UriKind.Absolute));
                foreach (Cookie cookie in cookieCollection)
                {
                    //Response.SetCookie(new HttpCookie(cookie.Name, cookie.Value));
                    Response.AppendCookie(new HttpCookie(cookie.Name, cookie.Value));
                }

                Response.Headers.Add("viewState", viewState);
                Response.ContentType = "image/Gif";
                Response.BinaryWrite(res);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Response.Write("请重新获取验证码");
            }
        }

        //登录个人主页面
        string defaultPageHtml;

        private async void LoadDefaultPage(string user, string pwd, string code)
        {
            handler = new HttpClientHandler() { UseCookies = false };
            client = new HttpClient(handler);
            
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("__VIEWSTATE",viewState);
            dic.Add("Button1", "");
            dic.Add("hidPdrs", "");
            dic.Add("hidsc", "");
            dic.Add("lbLanguage", "");
            dic.Add("RadioButtonList1", "%D1%A7%C9%FA");
            dic.Add("TextBox2", pwd);
            dic.Add("txtSecretCode", code);
            dic.Add("txtUserName", user);

            FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
            handler.CookieContainer.SetCookies(new Uri("登录接口", UriKind.Absolute), clientCookie);
            client.DefaultRequestHeaders.Add("Cookie", clientCookie);
            HttpResponseMessage msg = await client.PostAsync("登录接口", content);

            string result = await msg.Content.ReadAsStringAsync();
            
            if (!result.Contains("请登录"))
            {
                defaultPageHtml = result;

                if (target == "course")
                    GetCoursePageHtml();
                else if (target == "transcript")
                    GetTranscriptHtml();
            }
            else
            {
                Match match = Regex.Match(result, @"alert\('(.+)'\);");
                string alert = match.Groups[1].Value;

                Response.Write(alert);
            }
        }

        #region 获取课表并返回Json数据
        string coursePageHtml;

        //获取课表页面Html
        private async void GetCoursePageHtml()
        {
            try
            {
                Match coursePageMatch = Regex.Match(defaultPageHtml, @"<a href=""(xsxkqk(.+)?)""( target.+)学生选课结果查询");
                string href = "某接口" + coursePageMatch.Groups[1].Value;

                client.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "zh-Hans-CN, zh-Hans; q=0.8, en-US; q=0.5, en; q=0.3");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("Host", "某接口");
                client.DefaultRequestHeaders.Add("Referer", "某接口" + user);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");

                HttpResponseMessage msg = await client.GetAsync(href);
                coursePageHtml = await msg.Content.ReadAsStringAsync();

                coursePageViewState = Regex.Match(coursePageHtml, "name=\"__VIEWSTATE\" value=\"(.+)\"").Groups[1].Value;

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("__EVENTARGUMENT", eventArgument);
                dic.Add("__EVENTTARGET", eventTarget);
                dic.Add("ddlXN", ddlXN);
                dic.Add("ddlXQ", ddlXQ);
                dic.Add("__VIEWSTATE", coursePageViewState);
                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                msg = await client.PostAsync(href, content);
                coursePageHtml = await msg.Content.ReadAsStringAsync();

                ParseCoursePage();

                //Response.Write(coursePageHtml);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        List<List<List<Course>>> termlyCourseList = new List<List<List<Course>>>();

        private void ParseCoursePage()
        {
            #region 初始化termlyCourseList
            for (int i = 0; i < 25; i++)
            {
                List<List<Course>> weeklyCourseList = new List<List<Course>>();

                for (int j = 0; j < 7; j++)
                {
                    List<Course> dailyCourseList = new List<Course>();

                    for (int k = 0; k < 12; k++)
                    {
                        dailyCourseList.Add(new Course());
                    }

                    weeklyCourseList.Add(dailyCourseList);
                }

                termlyCourseList.Add(weeklyCourseList);
            }
            #endregion

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(coursePageHtml);

            HtmlNode rootNode = doc.DocumentNode;
            HtmlNode childNode = rootNode.SelectSingleNode("/html[1]/body[1]/div[2]/div[1]/span[1]/table[1]");

            //foreach (HtmlNode node in childNode.ChildNodes)
            //{
            //    if (node.InnerText.Contains("选课课号"))
            //    {
            //        Console.WriteLine(node.XPath);
            //    }
            //}

            //Console.WriteLine(childNode.InnerText);

            List<Course> courseList = new List<Course>();
            foreach (HtmlNode node in childNode.ChildNodes)
            {
                if (!node.InnerText.Contains("选课课号") && node.ChildNodes.Count != 0)
                {
                    Course course = new Course();
                    List<string> list = new List<string>();
                    foreach (HtmlNode innerNode in node.ChildNodes)
                    {
                        Console.WriteLine(innerNode.InnerText.Trim());
                        list.Add(innerNode.InnerText.Trim());
                    }

                    course.CourseNum = list[1];
                    course.CourseCode = list[2];
                    course.Name = list[3];
                    course.Classify = list[4];
                    course.IsSelected = list[5];
                    course.Teacher = list[6];
                    course.Credits = list[7];
                    course.WeeklyHours = list[8];
                    course.LessonTime = list[9];
                    course.LessonVenue = list[10];
                    course.Textbook = list[11];

                    courseList.Add(course);
                }
            }

            foreach (Course course in courseList)
            {
                ParseSingleCourse(course);
            }

            TermlyListToJson();

            Response.Headers.Add("termlyDate", "2016/2/22");
            Response.Headers.Add("weekNum", "20");
            Response.Write(termlyCourseJson);
        }

        private void ParseSingleCourse(Course course)
        {
            string[] lessonTimeArray = course.LessonTime.Split(";".ToCharArray());
            string[] lessonVenueArray = course.LessonVenue.Replace("&nbsp", "").Split(";".ToCharArray());
            
            for (int m = 0; m < lessonTimeArray.Count(); m++)
            {
                string s = lessonTimeArray[m];

                string rankOfWeekString;
                string rankOfDayString;
                string rankOfTermString;
                string note = "";

                if (s.Contains("|"))
                {
                    Match match = Regex.Match(s, "周(.)第(.+?)节{第(.+?)周.([单双])周}");

                    rankOfWeekString = match.Groups[1].Value;
                    rankOfDayString = match.Groups[2].Value;
                    rankOfTermString = match.Groups[3].Value;
                    note = match.Groups[4].Value;
                }
                else
                {
                    Match match = Regex.Match(s, "周(.)第(.+?)节{第(.+?)周}");

                    rankOfWeekString = match.Groups[1].Value;
                    rankOfDayString = match.Groups[2].Value;
                    rankOfTermString = match.Groups[3].Value;
                }

                string[] rankOfDayArray = rankOfDayString.Split(",".ToCharArray());

                int rankOfWeek = -1;
                switch (rankOfWeekString)
                {
                    case "一":
                        rankOfWeek = 0;
                        break;
                    case "二":
                        rankOfWeek = 1;
                        break;
                    case "三":
                        rankOfWeek = 2;
                        break;
                    case "四":
                        rankOfWeek = 3;
                        break;
                    case "五":
                        rankOfWeek = 4;
                        break;
                    case "六":
                        rankOfWeek = 5;
                        break;
                    case "日":
                        rankOfWeek = 6;
                        break;
                    default:
                        rankOfWeek = -1;
                        break;
                }

                if (rankOfWeek >= 0)
                {
                    for (int i = 0; i < rankOfDayArray.Count(); i++)
                    {
                        int rankOfDay = Convert.ToInt32(rankOfDayArray[i]) - 1;
                        string[] rankOfTermArray = rankOfTermString.Split("-".ToCharArray());
                        if (rankOfTermArray.Count() == 2)
                        {
                            int low = Convert.ToInt32(rankOfTermArray[0]);
                            int high = Convert.ToInt32(rankOfTermArray[1]);
                            if (note == "单")
                            {
                                for (int j = low; j <= high; j++)
                                {
                                    if (j % 2 == 1)
                                    {
                                        int rankOfTerm = j - 1;
                                        termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay] = course;

                                        try
                                        {
                                            termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = lessonVenueArray[m];
                                        }
                                        catch
                                        {
                                            //此处应注意!!!考虑使用下列哪种解决方案
                                            //termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = "";
                                            termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = lessonVenueArray[0];
                                        }
                                    }
                                }
                            }
                            else if (note == "双")
                            {
                                for (int j = low; j <= high; j++)
                                {
                                    if (j % 2 == 0)
                                    {
                                        int rankOfTerm = j - 1;
                                        termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay] = course;

                                        try
                                        {
                                            termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = lessonVenueArray[m];
                                        }
                                        catch
                                        {
                                            //此处应注意!!!考虑使用下列哪种解决方案
                                            //termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = "";
                                            termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = lessonVenueArray[0];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int j = low; j <= high; j++)
                                {
                                    int rankOfTerm = j - 1;
                                    termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay] = course;

                                    try
                                    {
                                        termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = lessonVenueArray[m];
                                    }
                                    catch
                                    {
                                        //此处应注意!!!考虑使用下列哪种解决方案
                                        //termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = "";
                                        termlyCourseList[rankOfTerm][rankOfWeek][rankOfDay].LessonVenue = lessonVenueArray[0];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static string termlyCourseJson;

        private void TermlyListToJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonTextWriter jsonTextWriter = new JsonTextWriter(sw);

            jsonTextWriter.WriteStartObject();
            jsonTextWriter.WritePropertyName("TermlyData");
            jsonTextWriter.WriteStartArray();

            for (int n = 0; n < termlyCourseList.Count; n++)
            {
                jsonTextWriter.WriteStartObject();
                jsonTextWriter.WritePropertyName("WeeklyData");
                jsonTextWriter.WriteStartArray();

                for (int i = 0; i < termlyCourseList[n].Count; i++)
                {
                    jsonTextWriter.WriteStartObject();
                    jsonTextWriter.WritePropertyName("DailyData");
                    jsonTextWriter.WriteStartArray();

                    for (int j = 0; j < termlyCourseList[n][i].Count; j++)
                    {
                        jsonTextWriter.WriteStartObject();

                        jsonTextWriter.WritePropertyName("FullName");
                        jsonTextWriter.WriteValue(termlyCourseList[n][i][j].Name);
                        jsonTextWriter.WritePropertyName("Classroom");
                        jsonTextWriter.WriteValue(termlyCourseList[n][i][j].LessonVenue);
                        jsonTextWriter.WritePropertyName("Teacher");
                        jsonTextWriter.WriteValue(termlyCourseList[n][i][j].Teacher);
                        jsonTextWriter.WritePropertyName("Credits");
                        jsonTextWriter.WriteValue(termlyCourseList[n][i][j].Credits);
                        jsonTextWriter.WritePropertyName("Classify");
                        jsonTextWriter.WriteValue(termlyCourseList[n][i][j].Classify);
                        jsonTextWriter.WritePropertyName("CourseCode");
                        jsonTextWriter.WriteValue(termlyCourseList[n][i][j].CourseCode);

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

            termlyCourseJson = sb.ToString();
        } 
        #endregion

        #region 个人成绩查询
        string transcriptHtml;

        private async void GetTranscriptHtml()
        {
            try
            {
                Match transcriptPageMatch = Regex.Match(defaultPageHtml, "(XSCJCX(.+?))\" target");
                string href = "某接口" + transcriptPageMatch.Groups[1].Value;
                Response.Headers.Add("href", href);

                client.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "zh-Hans-CN, zh-Hans; q=0.8, en-US; q=0.5, en; q=0.3");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("Host", "某接口");
                client.DefaultRequestHeaders.Add("Referer", "某接口" + user);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");

                HttpResponseMessage msg = await client.GetAsync(href);
                transcriptHtml = await msg.Content.ReadAsStringAsync();

                transcriptViewState = Regex.Match(transcriptHtml, "name=\"__VIEWSTATE\" value=\"(.+)\"").Groups[1].Value;
                transcriptQueryValue = transcriptQueryType == "btn_xq" ? "学期成绩" : "学年成绩";

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("__EVENTARGUMENT", "");
                dic.Add("__EVENTTARGET", "");
                dic.Add("ddl_kcxz", "");
                dic.Add("hidLanguage", "");
                dic.Add(transcriptQueryType, transcriptQueryValue);
                dic.Add("ddlXN", gradeddlXN);
                dic.Add("ddlXQ", gradeddlXQ);
                dic.Add("__VIEWSTATE", transcriptViewState);
                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                msg = await client.PostAsync(href, content);
                transcriptHtml = await msg.Content.ReadAsStringAsync();

                transcriptViewState = Regex.Match(transcriptHtml, "name=\"__VIEWSTATE\" value=\"(.+)\"").Groups[1].Value;
                Response.Headers.Add("transcriptViewState", transcriptViewState);

                ParseTranscriptPage();
                //Response.Write(coursePageHtml);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Response.Write("服务器维护升级中，暂时无法查询！！");
            }
        }

        private async void GetTranscriptHtmlAgain()
        {
            try
            {
                string href = transcriptHref;
                handler = new HttpClientHandler() { UseCookies = false };
                client = new HttpClient(handler);

                client.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "zh-Hans-CN, zh-Hans; q=0.8, en-US; q=0.5, en; q=0.3");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("Host", "某接口");
                client.DefaultRequestHeaders.Add("Referer", "某接口" + user);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");

                transcriptQueryValue = transcriptQueryType == "btn_xq" ? "学期成绩" : "学年成绩";

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("__EVENTARGUMENT", "");
                dic.Add("__EVENTTARGET", "");
                dic.Add("ddl_kcxz", "");
                dic.Add("hidLanguage", "");
                dic.Add(transcriptQueryType, transcriptQueryValue);
                dic.Add("ddlXN", gradeddlXN);
                dic.Add("ddlXQ", gradeddlXQ);
                dic.Add("__VIEWSTATE", transcriptViewState);
                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                client.DefaultRequestHeaders.Add("Cookie", clientCookie);
                HttpResponseMessage msg = await client.PostAsync(href, content);
                transcriptHtml = await msg.Content.ReadAsStringAsync();

                transcriptViewState = Regex.Match(transcriptHtml, "name=\"__VIEWSTATE\" value=\"(.+)\"").Groups[1].Value;
                Response.Headers.Add("transcriptViewState", transcriptViewState);

                ParseTranscriptPage();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Response.Write("查询失败，请稍后再试吧！！");
            }
        }

        List<Course> TranscriptList = new List<Course>();

        private void ParseTranscriptPage()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(transcriptHtml);
            HtmlNode rootNode = doc.DocumentNode;
            HtmlNode childNode = rootNode.SelectSingleNode("/html[1]/body[1]/div[2]/div[1]/span[1]/div[1]/table[1]");

            for (int i = 2; i < childNode.ChildNodes.Count; i++)
            {
                HtmlNode node = childNode.ChildNodes[i];
                if (node.ChildNodes.Count > 18)
                {
                    Course course = new Course();

                    course.CourseCode = node.ChildNodes[3].InnerText.Replace("&nbsp;", "");
                    course.Name = node.ChildNodes[4].InnerText.Replace("&nbsp;", "");
                    course.Classify = node.ChildNodes[5].InnerText.Replace("&nbsp;", "");
                    course.Credits = node.ChildNodes[7].InnerText.Replace("&nbsp;", "");
                    course.GradePoint = node.ChildNodes[8].InnerText.Replace("&nbsp;", "");
                    course.RegularGrade = node.ChildNodes[9].InnerText.Replace("&nbsp;", "");
                    course.MidtermGrade = node.ChildNodes[10].InnerText.Replace("&nbsp;", "");
                    course.FinalexamGrade = node.ChildNodes[11].InnerText.Replace("&nbsp;", "");
                    course.ExperimentGrade = node.ChildNodes[12].InnerText.Replace("&nbsp;", "");
                    course.TotalMark = node.ChildNodes[13].InnerText.Replace("&nbsp;", "");
                    course.MinorMark = node.ChildNodes[14].InnerText.Replace("&nbsp;", "");
                    course.MakeupTotalGrade = node.ChildNodes[15].InnerText.Replace("&nbsp;", "");
                    course.MakeupGrade = node.ChildNodes[16].InnerText.Replace("&nbsp;", "");
                    course.RetakeGrade = node.ChildNodes[17].InnerText.Replace("&nbsp;", "");
                    course.Academy = node.ChildNodes[18].InnerText.Replace("&nbsp;", "");

                    TranscriptList.Add(course);
                }
            }

            TranscriptListToJson();
        }

        private void TranscriptListToJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonTextWriter jsonTextWriter = new JsonTextWriter(sw);

            jsonTextWriter.WriteStartObject();
            jsonTextWriter.WritePropertyName("TranscriptData");
            jsonTextWriter.WriteStartArray();

            for (int i = 0; i < TranscriptList.Count; i++)
            {
                jsonTextWriter.WriteStartObject();

                jsonTextWriter.WritePropertyName("CourseCode");
                jsonTextWriter.WriteValue(TranscriptList[i].CourseCode);
                jsonTextWriter.WritePropertyName("Name");
                jsonTextWriter.WriteValue(TranscriptList[i].Name);
                jsonTextWriter.WritePropertyName("Classify");
                jsonTextWriter.WriteValue(TranscriptList[i].Classify);
                jsonTextWriter.WritePropertyName("Credits");
                jsonTextWriter.WriteValue(TranscriptList[i].Credits);
                jsonTextWriter.WritePropertyName("GradePoint");
                jsonTextWriter.WriteValue(TranscriptList[i].GradePoint);
                jsonTextWriter.WritePropertyName("RegularGrade");
                jsonTextWriter.WriteValue(TranscriptList[i].RegularGrade);
                jsonTextWriter.WritePropertyName("MidtermGrade");
                jsonTextWriter.WriteValue(TranscriptList[i].MidtermGrade);
                jsonTextWriter.WritePropertyName("FinalexamGrade");
                jsonTextWriter.WriteValue(TranscriptList[i].FinalexamGrade);
                jsonTextWriter.WritePropertyName("ExperimentGrade");
                jsonTextWriter.WriteValue(TranscriptList[i].ExperimentGrade);
                jsonTextWriter.WritePropertyName("TotalMark");
                jsonTextWriter.WriteValue(TranscriptList[i].TotalMark);
                jsonTextWriter.WritePropertyName("MinorMark");
                jsonTextWriter.WriteValue(TranscriptList[i].MinorMark);
                jsonTextWriter.WritePropertyName("MakeupTotalGrade");
                jsonTextWriter.WriteValue(TranscriptList[i].MakeupTotalGrade);
                jsonTextWriter.WritePropertyName("MakeupGrade");
                jsonTextWriter.WriteValue(TranscriptList[i].MakeupGrade);
                jsonTextWriter.WritePropertyName("RetakeGrade");
                jsonTextWriter.WriteValue(TranscriptList[i].RetakeGrade);
                jsonTextWriter.WritePropertyName("Academy");
                jsonTextWriter.WriteValue(TranscriptList[i].Academy);

                jsonTextWriter.WriteEndObject();
            }
            jsonTextWriter.WriteEndArray();
            jsonTextWriter.WriteEndObject();

            string s = sb.ToString();
            Response.Write(sb.ToString());
        } 
        #endregion
    }
}