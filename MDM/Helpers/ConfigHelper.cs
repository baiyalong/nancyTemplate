using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MDM.Helpers
{
    public class ConfigHelper
    {
        /// <summary>
        /// 在配置文件中获得字符串
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T GetValue<T>(string key, T defaultValue)
           where T : IConvertible
        {
            T value = defaultValue;
            if (ConfigurationManager.AppSettings[key] != null)
            {
                string obj = ConfigurationManager.AppSettings[key];
                value = Utils.DataConvert<T>(obj, defaultValue);
            }
            return value;
        }
    }
}