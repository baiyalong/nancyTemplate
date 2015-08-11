using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class AppClassify : ModelBase
    {

        public string classifyID { get; set; }
        public string classifyName { get; set; }

        public BsonArray apps { get; set; }

        public string description { get; set; } 
        public override void View()
        {
            this.apps = null;
        }
        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.classifyID,
            this.classifyName
            }, ss);
        }
    }
}