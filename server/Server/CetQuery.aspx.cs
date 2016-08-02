using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Server
{
    public partial class CetQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                string code = Request["code"];
                string name = HttpUtility.UrlEncode(Request["name"].Substring(0, 2), Encoding.GetEncoding("GB2312"));
                string key = Request["key"];

                if (CheckKey(key))
                {
                    GetCetGrade(code, name);
                }
                else
                {
                    Response.Write("抱歉，您暂无权调用此接口！！");
                }
            }
            else
            {
                Response.Write("2015年12月笔试或11月口试准考证号");
            }
        }

        //检查是否有权限调用接口
        private bool CheckKey(string key)
        {
            List<string> list = new List<string>();
            list.Add("xiaoz");

            return list.Exists(item => item == key);
        }

        private async void GetCetGrade(string code, string name)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Referer", "某接口");

                StringContent content = new StringContent(string.Format("id={0}&name={1}", code, name.ToUpper()));
                HttpResponseMessage msg = await client.PostAsync("某接口", content);
                if (msg.StatusCode == HttpStatusCode.OK)
                {
                    string result = await msg.Content.ReadAsStringAsync();
                    if (result == "4")
                    {
                        Response.Write("准考证号与姓名不匹配！！");
                    }
                    else
                    {
                        Response.Write(result);
                    }
                }
                else
                {
                    Response.Write("查询失败！！");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}