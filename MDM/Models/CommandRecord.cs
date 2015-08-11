using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class CommandRecord:ModelBase 
    {
        public string CommandId { get; set; }

        public string CommandName { get; set; }

        public string terminalId { get; set; }

        public string user { get; set; }

        public string phoneNumber { get; set; }

        public string deviceName { get; set; }

        public string imei { get; set; }

        public int status { get; set; }

        public string statusName { get; set; }

        public string SendTime { get; set; }

        public string UpdateTime { get; set; }

        public string requestId { get; set; }

        public string reportId { get; set; }

        public string sendCommndUser { get; set; }
        
    }
}