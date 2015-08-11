using MDM.DAL;
using MDM.Helpers;
using MDM.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MDM.BLL
{
    public class AppInstallMsgBLL : BLLBase<TerminalApp>
    {
        public static readonly new AppInstallMsgBLL Instance = new AppInstallMsgBLL();

        public AppInstallMsgBLL()
        {

        }

        //应用信息上告入库  
        public static bool AddAppInstallMsg(TerminalApp model)
        {
            bool res = true;

            try
            {
                
                if( false== AppInstallMsgBLL.Instance.Add(model))
                {
                    res = false;
                }
            }
            catch(Exception ex)
            {
                
                string dd=ex.Message;
            }

            return res;
        }


    }
}