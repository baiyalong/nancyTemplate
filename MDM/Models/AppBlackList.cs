using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class AppBlackList:ModelBase
    {

        public string GroupId { get; set; }
        public string ApkCode { get; set; }
        public string ApkName { get; set; }
    }
}