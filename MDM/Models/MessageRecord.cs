using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class MessageRecord:ModelBase
    {

        public string MessageContent { get; set; }

        public string title { get; set; }
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

        public string currentUser { get; set; }

    }
}