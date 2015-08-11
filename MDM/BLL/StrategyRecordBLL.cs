using System;
using MDM.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class StrategyRecordBLL:BLLBase<StrategyRecord>
    {
         public static readonly new StrategyRecordBLL Instance = new StrategyRecordBLL();
         private StrategyRecordBLL()
        {
        }
    }
}