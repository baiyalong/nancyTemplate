using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{

    public class TerminalApp : ModelBase
    {

        public string terminalId { get; set; }

        public List<AppInstallMsg> apps { get; set; }

    }


    public class AppInstallMsg 
    {
        public string appName { get; set; }

        public string appVersionName { get; set; }

        public string firstInstallTime { get; set; }

        public string lastUpdateTime { get; set; }

        public string packageName { get; set; }
      

    }

}