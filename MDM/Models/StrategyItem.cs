using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class StrategyItem : ModelBase
    {
        public string GroupID { get; set; }

        public string StrategyID { get; set; }

        public string punishmentID { get; set; }

        public string startTime { get; set; }

        public string endTime { get; set; }

        public string reginalInterval { get; set; }

        public string Parameters { get; set; }


        public override void View()
        {

        }
        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.startTime,
            this.endTime,
            this.reginalInterval,
            this.Parameters,
            }, ss);
        }

    }
}