using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Configuration;
using System.Web.Services;
using System.Threading;
using System.IO;
using System.Net.Sockets;

namespace FileTransfer
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {


        [WebMethod]
        public string UpLoadUrl(string fileName, string type)
        {
            string upLoadUrl = string.Empty;

            //不同文件类型在服务器上对应的目录
            string directory = ConfigurationManager.AppSettings[type].ToString();

            //问题：指定好的文件目录怎样转成url？  
            upLoadUrl = "http://10.192.17.95:22/";

            string uploadUrl = Server.MapPath((directory));

            return upLoadUrl;
        }




        //上传   注：byte[] 参数的数据类型无法在调试的时候使用                           
        //[WebMethod]  
        //public bool  UploadFile(string fileName ,string type,byte[] content)
        //{
        //    bool result = false;
        //    //不同文件类型在服务器上对应的目录
        //    string directory = ConfigurationManager.AppSettings[type].ToString();
        //    //将指定的文件保存到指定目录下
        //    if (SaveFile(fileName, type, content, directory))
        //    {
        //        result = true;
        //    }
        //    else
        //    {
        //        result = false;
        //    }
        //    return result;
        //}

        //文件保存 
        private bool SaveFile(string fileName,string type,byte[] Content,string directory)
        {
            bool result = true;


            return result;
        }

        //下载   
        [WebMethod]  
        public byte[]  DownLoadUrl(string fileName,  string  url ,string type )
        {
            
               // HttpClientCertificate
               // TcpClient
            byte[] bytes = null;

            //1. 将url映射为本地路径  具体还不清楚  

            //读取本地的文件流  然后将它返回。

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
 
            
            return bytes;
        }


        private bool RemoteFileExists(string fileUrl)
        {
            bool result = false;
            System.Net.WebResponse response = null;
            try
            {
                System.Net.WebRequest req = WebRequest.Create(fileUrl);
                response = req.GetResponse();
                result = response == null ? false : true;

            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }


    }
}
