using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Helpers
{
    /// <summary>
    /// 字典类别
    /// </summary>
    public enum DictType
    {
        /// <summary>
        /// 运营商
        /// </summary>
        Operator = 1,

        /// <summary>
        /// 操作系统
        /// </summary>
        OS = 2,

        /// <summary>
        /// 命令发送状态
        /// </summary>
        Status=3,

        /// <summary>
        /// 命令执行上告状态
        /// </summary>
        CommandStatus=4,
    }

    public enum SendStaus
    {
        /// <summary>
        /// 发送中
        /// </summary>
        Sending = 2,
        /// <summary>
        /// 发送成功
        /// </summary>
        SendSuccess = 0,
        /// <summary>
        /// 发送失败
        /// </summary>
        SendFail = 1,
    }

    public enum RequestType
    {
        /// <summary>
        /// 命令
        /// </summary>
        Command=01,

        /// <summary>
        /// 发送消息
        /// </summary>
        Message=02,

        /// <summary>
        /// 默认
        /// </summary>
        defal=03,

        /// <summary>
        /// 策略
        /// </summary>
        Strategy=04,

    }

    /// <summary>
    /// 运营商的静态类
    /// </summary>
    public static class OperatorEnums
    {
        public const string ChinaUnicom = "中国联通";

        public const string ChinaMobile = "中国移动";

        public const string ChinaTelecom = "中国电信";
    }
    


    /// <summary>
    /// 操作系统的静态类
    /// </summary>
    public static class OSEnums
    {
        public const string Android = "安卓";

        public const string IOS = "IOS";

        public const string WP = "WindowsPhone";
    }

}