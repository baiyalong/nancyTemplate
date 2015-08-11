using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class StrategyRecord:ModelBase
    {
        public string TerminalId { get; set; }

        public string PolicyId { get; set; }

        public string Version { get; set; }

        public List<string> Items { get; set; }

        public List<string> BlackList { get; set; }

    }
}