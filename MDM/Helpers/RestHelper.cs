using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using Nancy;
using System.Text;
using System.Configuration;

namespace MDM.Helpers
{
    public class RestHelper
    {
        public static Nancy.HttpStatusCode RemoteRequest(Uri requestUri, string methodType, string json)
        {
            try
            {
                byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(json);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                //发送数据
                request.Method = methodType;
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.KeepAlive = false;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                LogHelper.WriteInfoLog(typeof(RestHelper), "发送数据EdgeServer: " + json);
                requestStream.Close();
                //接收返回值
                HttpWebResponse res = (HttpWebResponse)request.GetResponse();
                StreamReader sReader = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
                //string jsonResult = sReader.ReadToEnd();
                sReader.Close();
                res.Close();
                //result = JsonConvert.DeserializeObject(jsonResult, typeof(T)) as T;
                return (Nancy.HttpStatusCode)res.StatusCode;
            }
            catch (Exception ex)
            {
                LogHelper.WriteInfoLog(typeof(RestHelper), "发送数据到Edge错误" + ex.Message + " " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            }
            return Nancy.HttpStatusCode.NotImplemented;
        }

        public static Uri GetServiceUrlFromConfig(string serviceName)
        {
            string usersLdapPath = ConfigurationManager.AppSettings[serviceName].ToString();

            return new Uri(usersLdapPath);
        }

        //从http请求中提取body信息
        public static string GetBodyFromRequest(Request data)
        {
            byte[] streamdata = new byte[(int)data.Body.Length];
            data.Body.Read(streamdata, 0, (int)data.Body.Length);

            return Encoding.UTF8.GetString(streamdata);
        }
        
    }
}