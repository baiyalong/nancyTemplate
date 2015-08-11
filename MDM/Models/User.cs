using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace MDM.Models
{
    public class User : ModelBase
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 密码（MD5加密）
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }

        public int Level { get; set; }
        /// <summary>
        /// 应用模版关联id
        /// </summary>
        public string appTemplate { get; set; }

        /// <summary>
        /// 当前用户所属的用户组集合
        /// </summary>
        public BsonArray UserGroups { get; set; }
        /// <summary>
        /// 当前用户拥有的终端集合
        /// </summary>
        public BsonArray Terminals { get; set; }

        public override void View()
        {
            this.UserGroups = null;
            this.Terminals = null;
        }
        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.Name,
            this.PhoneNumber,
            this.Email
            }, ss);
        }
    }
}