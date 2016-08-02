using cqmu.MVVM.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace cqmu.MVVM.Helper
{
    public class NetworkHelper
    {
        public string ClientCookie { get; set; }

        public string ViewState { get; set; }

        public string GradeHref { get; set; }

        public BitmapImage IdentifyCodeImage { get; set; }
        
        public async Task<int> GetIdentifyCodeAsync()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                await new MessageDialog("哦~啊~网络出故障了，检查下网络连接吧(T_T)！！").ShowAsync();

                return 0;
            }
            
            try
            {
                IdentifyCodeImage = new BitmapImage();
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("target", "code");
                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                HttpResponseMessage msg = await client.PostAsync(APIs.APIs.APIGetCourse, content);

                //ViewState
                if (msg.Headers.Contains("viewState"))
                    ViewState = msg.Headers.GetValues("viewState").ToList()[0];

                //ClientCookie
                CookieCollection collection = handler.CookieContainer.GetCookies(new Uri(APIs.APIs.APIGetCourse, UriKind.Absolute));
                foreach (Cookie cookie in collection)
                    ClientCookie += $"{cookie.Name}={cookie.Value}";
                
                //IdentifyCodeImage
                await IdentifyCodeImage.SetSourceAsync((await msg.Content.ReadAsStreamAsync()).AsRandomAccessStream());
                
                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        public async Task<int> GetTermlyJsonAsync(string id, string pwd, string code, bool rememberPwd)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                await new MessageDialog("哦~啊~网络出故障了，检查下网络连接吧(T_T)！！").ShowAsync();

                return 0;
            }

            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("user", id);
                dic.Add("pwd", pwd);
                dic.Add("code", code);
                dic.Add("key", "xiaoz");
                dic.Add("clientCookie", ClientCookie);
                dic.Add("viewState", ViewState);
                dic.Add("target", "course");

                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                HttpResponseMessage msg = await client.PostAsync(APIs.APIs.APIGetCourse + (new Random()).Next(), content);

                string termlyJson = await msg.Content.ReadAsStringAsync();
                if(msg.StatusCode == HttpStatusCode.OK)
                {
                    if (termlyJson.Contains("["))
                    {
                        //存储用户信息
                        App.AppSettings.UserId = id;
                        App.AppSettings.UserPassword = pwd;
                        App.AppSettings.RememberPwd = rememberPwd;

                        //将开学时间和总周数保存至本地设置
                        if (msg.Headers.Contains("termlyDate"))
                            App.AppSettings.TermlyDate = Convert.ToDateTime(msg.Headers.GetValues("termlyDate").ToList()[0]);
                        if (msg.Headers.Contains("weekNum"))
                            App.AppSettings.WeekNum = Convert.ToInt32(msg.Headers.GetValues("weekNum").ToList()[0]);

                        //将Json保存至本地
                        return await CourseDataService.SaveTermlyJsonToIsoStoreAsync(termlyJson);
                    }
                    else
                    {
                        Debug.WriteLine(termlyJson);

                        return 2;//信息不正确
                    }
                }
                else
                {
                    return 3;//登录超时等情况
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return -1;//发生异常
            }
        }

        public async Task<string> GetGradeJsonAsync(string id, string pwd, string code)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                await new MessageDialog("哦~啊~网络出故障了，检查下网络连接吧(T_T)！！").ShowAsync();

                return null;
            }

            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);

                int optedYear = DateTime.Today.Month >= 9 ? DateTime.Today.Year : DateTime.Today.Year - 1;
                string optedTerm = "1";

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("user", id);
                dic.Add("pwd", pwd);
                dic.Add("code", code);
                dic.Add("key", "xiaoz");
                dic.Add("clientCookie", ClientCookie);
                dic.Add("viewState", ViewState);
                dic.Add("target", "transcript");

                dic.Add("transcriptQueryType", "btn_xq");
                dic.Add("ddlXN", $"{optedYear}-{optedYear + 1}");
                dic.Add("ddlXQ", optedTerm);

                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                HttpResponseMessage msg = await client.PostAsync(APIs.APIs.APIGetCourse + new Random().Next(), content);

                if (msg.Headers.Contains("transcriptViewState"))
                {
                    ViewState = msg.Headers.GetValues("transcriptViewState").ToList()[0];
                }
                if (msg.Headers.Contains("href"))
                {
                    GradeHref = msg.Headers.GetValues("href").ToList()[0];
                }
                
                if (msg.StatusCode == HttpStatusCode.OK)
                {
                    return await msg.Content.ReadAsStringAsync();
                }
                else
                {
                    return "身份验证失败，再试一下吧（＞﹏＜）";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }

        public async Task<string> GetGradeJsonAgainAsync(NetworkHelper helper, string schoolYear, string term)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                await new MessageDialog("哦~啊~网络出故障了，检查下网络连接吧(T_T)！！").ShowAsync();

                return null;
            }

            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("key", "xiaoz");
                dic.Add("target", "anotherTranscript");
                dic.Add("clientCookie", helper.ClientCookie);
                dic.Add("transcriptViewState", helper.ViewState);
                dic.Add("transcriptHref", helper.GradeHref);

                dic.Add("transcriptQueryType", "btn_xq");
                dic.Add("ddlXN", schoolYear);
                dic.Add("ddlXQ", term);

                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                HttpResponseMessage msg = await client.PostAsync(APIs.APIs.APIGetCourse + new Random().Next(), content);

                if (msg.Headers.Contains("transcriptViewState"))
                {
                    ViewState = msg.Headers.GetValues("transcriptViewState").ToList()[0];
                }

                if (msg.StatusCode == HttpStatusCode.OK)
                {
                    return await msg.Content.ReadAsStringAsync();
                }
                else
                {
                    return "身份验证失败，再试一下吧（＞﹏＜）";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }

        public async Task<List<string>> GetCetResultAsync(string code,string name)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                await new MessageDialog("哦~啊~网络出故障了，检查下网络连接吧(T_T)！！").ShowAsync();

                return null;
            }

            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("code", code);
                dic.Add("name", name);
                dic.Add("key", "xiaoz");

                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                HttpResponseMessage msg = await client.PostAsync(APIs.APIs.APICetQuery, content);

                if (msg.StatusCode == HttpStatusCode.OK)
                {
                    string result = await msg.Content.ReadAsStringAsync();
                    string[] detail = result.Split(",".ToCharArray());

                    if (detail.Length == 7)
                    {
                        List<string> detailList = new List<string>();
                        detailList.Add(detail[6].Trim());
                        detailList.Add(detail[5].Trim());
                        detailList.Add("CET-" + detail[0].Trim());
                        detailList.Add(code);
                        detailList.Add(detail[4].Trim());
                        detailList.Add(detail[1].Trim());
                        detailList.Add(detail[2].Trim());
                        detailList.Add(detail[3].Trim());

                        return detailList;
                    }
                    else
                    {
                        await new MessageDialog(result).ShowAsync();

                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }
    }
}
