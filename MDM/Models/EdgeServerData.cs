using MDM.Helpers;
using MDM.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MDM.Models
{
    #region 从EdgeServer接收的消息格式

    public class EdgeServerData
    {
        public int serviceID { get; set; }

        public object payload { get; set; }
    }

    //EdgeServer将数据发送至客户端后，给平台的回告
    public class ResponseData
    {
        public string responseID { get; set; }
        public string terminalID { get; set; }
        public int status { get; set; }
    }

    //客户端经EdgeServer转发的用户登录数据
    public class LogonData
    {
        public string username { get; set; }
        public string password { get; set; }
        public string imei { get; set; }
        public string phoneNumber { get; set; }
        public string appID { get; set; }
        public string deviceSN { get; set; }
    }

    public class EdgeServerResponse
    {
        public HttpStatusCode status { get; set; }

        public LogonResult result { get; set; }

        public EdgeServerResponse(HttpStatusCode code) { status = code; result = null; }
        public EdgeServerResponse(HttpStatusCode code, LogonResult logon) { status = code; result = logon; }
    }

    //平台回复给EdgeServer的登录结果信息
    public class LogonResult
    {
        public int result { get; set; }

        public string terminalID { get; set; }
    }
    #endregion

    #region 发送到EdgeServer的消息格式

    public class Send2EdgeServer
    {
        public string requestID { get; set; }

        public BsonArray terminals { get; set; }

        public object data { get; set; }
    }

    #endregion
}