using MDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class StrategyBLL : BLLBase<Strategy>
    {
        public static readonly new StrategyBLL Instance = new StrategyBLL();
        private StrategyBLL()
        {
        }
    }
}