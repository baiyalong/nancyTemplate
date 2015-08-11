using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using Nancy;
using MDM.Models;
using MDM.Helpers;
using MongoDB.Driver;

namespace MDM.BLL
{
    public class AppClassifyBLL:BLLBase<AppClassify>
    {
        public static readonly new AppClassifyBLL Instance = new AppClassifyBLL();
        private AppClassifyBLL()
        {
        }

        //获取应用分类列表
        public static bool GetAppClassify( string terminalId)
        {
            bool res = false;
            List<AppClassify> list=new List<AppClassify> ();
            Classifylist classifyList = new Classifylist();
            List<SendAppClassify> sendList = new List<SendAppClassify>();
 
            try
            {
                if (true == AppClassifyBLL.Instance.GetByQueryList(null,out list))
                {
                    res = true;
                }

                foreach(AppClassify model in list)
                {
                    SendAppClassify classify = new SendAppClassify();

                    classify.classifyID = model.ID;
                    classify.classifyName = model.classifyName;

                    sendList.Add(classify);
                }

                classifyList.appClassify=sendList;
                
                SendClassifyMsg( terminalId,classifyList);

            }
            catch
            {

            }
            return res;
        }

        public static void SendClassifyMsg(string terminalId,  Classifylist list)
        {
            string id;

            Send2Client clientMsg = new Send2Client();
            clientMsg.commandID = "4";
            clientMsg.pfData = list;

            BsonArray terminals = new BsonArray();
            terminals.Add(terminalId);
            HttpStatusCode result = EdgeServerBLL.SendMessage2Client(terminals, clientMsg, RequestType.defal, out id);
            //后续添加result不为ok时的处理
            if (result != HttpStatusCode.OK)
                LogHelper.WriteInfoLog(typeof(AppBLL), "发送应用分类到EdgeServer失败");
        }
    }

    public class APPClassifyRouter
    {
        public APPClassifyRouter() { }

        //获取应用分类
        public static bool GetAppClassifyList(ClientData clientData)
        {
            var res = false;

            try
            {
              
                if (false ==AppClassifyBLL.GetAppClassify(clientData.terminalID))
                {
                    res = false;
                }

                res = true;
            }
            catch (Exception)
            {
            }


            return res;
        }



    }
}