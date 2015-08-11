using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace MDM.Helpers
{
    public class Utils
    {
        /// <summary>
        /// 通用类型转换函数
        /// </summary>
        /// <param name="value">传入对象</param>
        /// <param name="defaultValue">默认值</param>
        public static T DataConvert<T>(object value, T defaultValue)
            where T : IConvertible
        {
            T result = defaultValue;
            if (value == null) value = string.Empty;
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                Type type = typeof(T);
                if (result != null) type = result.GetType();
                try
                {
                    object cValue = Convert.ChangeType(value, type, System.Globalization.CultureInfo.CurrentCulture);
                    result = (T)cValue;
                }
                catch { }
            }
            return result;
        }

        //根据clientData中的interfaceID字段读取路由信息，将要路由到的类和方法名放入返回的List中
        #region MyRegion
        public static List<string> GetRouter(int interfaceID)
        {
            //1--MDM.BLL.TerminalRouter--UpdateRealTimeDevice--实时信息上告
            //2--MDM.BLL.TerminalRouter--UpdateDeviceMessage--设备信息上告
            //10--MDM.BLL.TerminalRouter--UpdateAppInfo--应用信息上告
            //11--MDM.BLL.AppRouter--GetAppList--应用列表查询
            //12--MDM.BLL.AppRouter--GetAppDetail--应用详情查询
            //15--MDM.BLL.APPClassifyRouter--GetAppClassifyList--应用分类查询
            //16--MDM.BLL.StrategyGroupRouter--StrategyReport--策略违规上告 
            //17--MDM.BLL.AppBlackListRouter--AppBlackListReport--黑名单违规上告
            //13--策略下达回复上告 暂缓
            //14--MDM.BLL.CommandBLL--CommandReportMsg--命令下达回复上告 


            List<string> classes = new List<string>() { "MDM.BLL.TerminalRouter", "MDM.BLL.TerminalRouter", 
                                                        "MDM.BLL.TerminalRouter", "MDM.BLL.AppRouter",
                                                        "MDM.BLL.AppRouter","MDM.BLL.APPClassifyRouter",                                                     "MDM.BLL.AppRouter","MDM.BLL.APPClassifyRouter",
                                                        "MDM.BLL.StrategyGroupRouter","MDM.BLL.AppBlackListRouter",
                                                        "MDM.BLL.CommandRouter"};
                                 
            List<string> methods = new List<string>() { "UpdateRealTimeDevice", "UpdateDeviceMessage",
                                                        "UpdateAppInfo", "GetAppList", 
                                                        "GetAppDetail","GetAppClassifyList",
                                                        "StrategyReport","AppBlackListReport",
                                                        "CommandReportMsg"};
            List<int> keys = new List<int>() { 1, 2,
                                               10, 11, 12,15,16,17,14};

            int k = keys.IndexOf(interfaceID);

            if(k==-1)
            {
                return null;
            }
            List<string> router = new List<string>() { classes[k], methods[k] };
            return router;

        } 
        #endregion


        public static List<string> GetResponsRouter(int interfaceID)
        {
           
            //93--下达命令回告 
            //95--下达消息回告  可能消息下发不需要回告  

            List<string> classes = new List<string>() { "MDM.BLL.CommandBLL", "MDM.BLL.MessageBLL" };
            List<string> methods = new List<string>() { "ResponseMsg", "ResponseMsgRecord" };
            List<int> keys = new List<int>() {  93,95 };

            int k = keys.IndexOf(interfaceID);

            if(k==-1)
            {
                return null;
            }

            List<string> router = new List<string>() { classes[k], methods[k] };
            return router;

        }


        /// <summary>
        /// 获取字典显示字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="statusType"></param>
        /// <returns></returns>
        public static string GetDictText(int value, DictType type)
        {
            string text = "未知";
            Dictionary<string, int> dict = GetDict(type);
            List<string> keys = dict.Keys.ToList();
            List<int> values = dict.Values.ToList();
            int index = values.FindIndex(item => item == value);
            if (index != -1 && index < keys.Count) text = keys[index];
            return text;
        }

        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="statusType"></param>
        /// <returns></returns>
        public static int GetDictValue(string text, DictType type)
        {
            int value = 0;
            Dictionary<string, int> dict = GetDict(type);
            List<string> keys = dict.Keys.ToList();
            List<int> values = dict.Values.ToList();
            int index = keys.FindIndex(item => item == text);
            if (index != -1 && index < values.Count) value = values[index];
            return value;
        }

        /// <summary>
        /// 根据枚举值获取状态字典
        /// </summary>
        /// <param name="value"></param>
        /// <param name="statusType"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetDict(DictType type)
        {
            List<string> keys = null;
            List<int> values = null;
            switch (type)
            {
                case DictType.Operator:
                    keys = new List<string>() { "中国移动", "中国联通", "中国移动", "中国电信", "中国电信", "中国联通", "中国移动" };
                    values = new List<int>() { 46000, 46001, 46002, 46003, 46005, 46006, 46007 };
                    break;
                case DictType.OS:
                    keys = new List<string>() { "安卓", "IOS", "WindowsPhone" };
                    values = new List<int>() { 0, 1, 2 };
                    break;
                case DictType.Status:
                    keys = new List<string>() { "正在发送中","发送成功","发送失败"};
                    values = new List<int>() { 2, 0, 1 };
                    break;
                case DictType.CommandStatus:
                    keys = new List<string>() { "执行成功","执行失败"};
                    values = new List<int>() { 0, 1 };
                    break;
            }
            Dictionary<string, int> kvs = new Dictionary<string, int>();
            keys.ForEach(key =>
            {
                if (!kvs.ContainsKey(key) && values.Count >= kvs.Count) kvs.Add(key, values[kvs.Count]);
            });
            return kvs;
        }

        
        //返回请求id
        public static string ReturnRequestID(RequestType type )
        {
            #region 
            //StringBuilder SbRequestId = new StringBuilder();

            //Random random = new Random();

            //for (int i = 0; i < 2;i++ )
            //{
            //     SbRequestId.Append(random.Next(1, 500000).ToString());
            //} 
            #endregion

            string RequestID = string.Empty;

            switch (type)
            {
                case RequestType.Command:
                    RequestID = "93" + Guid.NewGuid().ToString();
                    break;

                case RequestType.Message:
                    RequestID = "95" + Guid.NewGuid().ToString();
                    break;

                case RequestType.Strategy:
                    RequestID = "99" + Guid.NewGuid().ToString();
                    break;

                case RequestType.defal:
                    RequestID = "97" + Guid.NewGuid().ToString();
                    break;

            }
            return RequestID;
        }


    }
}