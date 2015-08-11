using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    #region 从客户端收到的消息定义

    public class ClientData
    {
        public string terminalID { get; set; }

        public PFData data { get; set; }
    }

    public class PFData
    {
        public int interfaceID { get; set; }
        public string deviceKey { get; set; }
        public object pfData { get; set; }
    }

    //App应用详情
    public class AppMessage
    {
        public string AppId { get; set; }
    }

    //App应用分类
    public class ClassifyMessage
    {
        public string classifyID { get; set; }
    }

    //违规策略
    public class StrategyMessage
    {
        public string policyVersion;

        public string policyID;

        public List<string> itemCode;
    }

    //黑名单违规上告 
    public class BlackListMessage
    {
        public string policyVersion;

        public string policyID;

        public List<string> blackList;
    }

    public class CommandMessage
    {
        public string parameter;

        public string responseID;

        public string result;
    }

    #region 终端信息上告

    //实时信息上告
    public class RealTimeTerminal
    {
        public string power { get; set; }

        public string availRomSpace { get; set; }

        public string totalSDSpace { get; set; }

        public string availSDSpace { get; set; }

    }


    //设备信息上告
    public class TerminalMessage
    {
        public string wifiMac { get; set; }

        public string blueTooth { get; set; }

        public string deviceName { get; set; }

        public string deviceType { get; set; }

        public int osType { get; set; }

        public string osVersion { get; set; }

        public string kernelVersion { get; set; }

        public int phoneOperator { get; set; }

        public string totalRomSpace { get; set; }
    }
    #endregion


    #endregion

    #region 发送到客户端的消息定义
    public class Send2Client
    {
        public string commandID { get; set; }

        public object pfData { get; set; }
       
    }

    //发送应用详情消息定义
    public  class  AppDetail
    {
        public AppDetailModel appDetail { get; set; }
    }

    //发送应用列表消息定义
    public class Apps
    {
        public string classifyID { get; set; }
        public List<AppModel> apps { get; set; }
    }

    //发送应用分类的消息定义
    public class Classifylist
    {
        public List<SendAppClassify> appClassify { get; set; }
    }
    //发送命令
    public class PfRequest
    {
        public string pfRequestID { get; set; }

        public string action { get; set; }

        public string extraInfo { get; set; }
    }
    //发送消息
    public class SendMessage
    {
        public string message { get; set; }

        public string title { get; set; }
    }
    //发送应用分类消息定义
    public class SendAppClassify
    {
        public string classifyID { get; set; }
        public string classifyName { get; set; }
    }

    //发送策略的消息定义
    public class SendStrategyItem
    {
        public string pfRequestID { get; set; }

        public string policyID { get; set; }

        public int version { get; set; }

        public List<StrategyDetail> details { get; set; }

        public string[] appBlackList { get; set; }

    }

    public class StrategyDetail
    {
        public string startTime { get; set; }

        public string endTime { get; set; }

        public string areaInfo { get; set; }

        public int itemCode { get; set; }

        public string action { get; set; }

    }

    #endregion
}