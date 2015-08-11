using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class UserGroup : ModelBase
    {
        /// <summary>
        /// 用户组名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 用户组描述
        /// </summary>
        public String Description { get; set; }

        public int Level { get; set; }
        /// <summary>
        /// 当前用户组包含的用户集合
        /// </summary>
        public BsonArray Users { get; set; }

        public IEnumerable<String> UsersHelper { get; set; }
        public override void View()
        {
            this.Users = null;
        }
        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.Name,
            this.Description
            }, ss);
        }
    }
}