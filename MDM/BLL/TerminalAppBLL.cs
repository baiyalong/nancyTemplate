using System;
using MDM.Models;
using MongoDB.Bson;
using Nancy;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class TerminalAppBLL:BLLBase<TerminalApp>
    {
         public static readonly new TerminalAppBLL Instance = new TerminalAppBLL();
         private TerminalAppBLL()
        {
        }


    }
}