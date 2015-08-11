using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class StrategyGroup : ModelBase
    {
        public string Name { get; set; }

        public int Version { get; set; }

        public BsonArray Item { get; set; }

        public BsonArray AppBlackItem { get; set; }

        public string description { get; set; }
        public int status { get; set; }

        public string[] UserGroup { get; set; }

        public override void View()
        {
            this.Item = null;
            this.AppBlackItem = null;
        }
        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.Name,
            this.Version,
            }, ss);
        }
    }
}