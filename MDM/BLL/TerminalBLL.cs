using MDM.DAL;
using MDM.Helpers;
using MDM.Models;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class TerminalBLL : BLLBase<Terminal>
    {
        public static readonly new TerminalBLL Instance = new TerminalBLL();
        private TerminalBLL()
        {
            this.dal = TerminalDAL.Instance;
        }
        private TerminalDAL dal { get; set; }

        //根据客户端登陆信息获取该终端
        public static Terminal GetTerminal(LogonData data)
        {
            Terminal terminal = null;
            try
            {
                if (false == TerminalDAL.Instance.GetByQuery(Query.And(Query<Terminal>.EQ(u => u.IMEI, data.imei),
                                                                 Query<Terminal>.EQ(u => u.PhoneNumber, data.phoneNumber),
                                                                 //Query<Terminal>.EQ(u => u.AppID, data.appID),
                                                                 Query<Terminal>.EQ(u => u.DeviceSN, data.deviceSN),
                                                                 Query<Terminal>.EQ(u => u.Status, 1)), out terminal))
                {
                    terminal = null;
                }
            }
            catch (Exception)
            {

            }
            return terminal;
        }

        internal bool DeleteTerminal(string id)
        {
            var res = false;
            try
            {
                Terminal terminal = null;
                res = this.Get(id, null, out terminal);
                if (res && terminal != null && terminal.User != null)
                {
                    if (res = UserBLL.Instance.DeleteTerminal(terminal.User, id))
                    {
                        res = this.Delete(id);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return res;
        }

        #region
        //internal new bool GetList(Pattern pattern, out IEnumerable<Terminal> oul)
        //{
        //    var res = false;
        //    try
        //    {
        //        if (pattern.pageNum <= 0) { pattern.pageNum = 1; }
        //        if (pattern.pageSize <= 0) { pattern.pageSize = 100; }

        //        Func<Terminal, bool> fn = null;
        //        if (pattern.search != null)
        //        {
        //            fn = x => x.search(pattern.search);
        //        }

        //        res = this.dal.GetList(pattern, fn, out oul);
        //    }
        //    catch (Exception ex)
        //    {
        //        oul = new List<Terminal>();
        //        throw (ex);
        //    }

        //    return res;
        //} 
        #endregion

        public bool GetAllTerminalApp(string terminalid, out List<AppInstallMsg> apps)
        {
            bool res = true;

            TerminalApp terminalappmsg;
            List<AppInstallMsg> list = new List<AppInstallMsg>();
            try
            {
                if ((true == TerminalAppBLL.Instance.GetByQuery(Query<TerminalApp>.EQ(x => x.terminalId, terminalid), out terminalappmsg)) && (terminalappmsg != null))
                {
                    if (terminalappmsg.apps != null)
                    {
                        list = terminalappmsg.apps;
                    }
                }
            }
            catch (Exception)
            {
                res = false;

            }
            apps = list;
            return res;
        }

        public bool GetTerminalFromUser(string uid, out List<Terminal> oul)
        {
            var res = false;
            try
            {

                User user;
                oul = new List<Terminal>();
                res = UserBLL.Instance.GetByQuery(Query<User>.EQ(p => p.ID, uid), out user);
                
                if(user.Terminals==null)
                {
                    return res;
                }
                else if( user.Terminals.Count != 0)
                {
                    res = TerminalBLL.Instance.GetList(user.Terminals, out oul);
                }
              

            }
            catch (Exception)
            {

                throw;
            }

            return res;
        }

        internal bool GetList(MongoDB.Bson.BsonArray uids, out List<Terminal> oul)
        {
            var res = false;
            try
            {
                List<Terminal> li = new List<Terminal>();
                foreach (var item in uids)
                {
                    Terminal ou;

                    if (true == this.dal.GetByQuery(Query<Terminal>.EQ(p => p.ID, item.ToString()), out ou))
                    {
                        if (ou != null)
                        {
                            ou.View();
                            li.Add(ou);
                        }

                    }

                }
                oul = li;
                res = true;

            }
            catch (Exception ex)
            {

                throw (ex);
            }

            return res;
        }

    }

    public class TerminalRouter
    {
        public TerminalRouter() { }

        //根据设备定时上告信息，更新设备信息
        public static bool UpdateRealTimeDevice(ClientData clientData)
        {
            bool res = false;
            try
            {
                RealTimeTerminal data = JsonConvert.DeserializeObject<RealTimeTerminal>(clientData.data.pfData.ToString());
                Terminal t;
                res = TerminalBLL.Instance.Get(clientData.terminalID, null, out t);
                if ((res == true) && (t != null))
                {
                    t.TotalSDSpace = data.totalSDSpace;
                    t.AvailRomSpace = data.availRomSpace;
                    t.AvailSDSpace = data.availSDSpace;
                    t.Power = data.power;
                    res &= TerminalBLL.Instance.Update(clientData.terminalID, t);
                }
            }
            catch (Exception)
            {
            }

            return res;
        }

        public static bool UpdateDeviceMessage(ClientData clientData)
        {
            bool res = false;
            try
            {
                TerminalMessage data = JsonConvert.DeserializeObject<TerminalMessage>(clientData.data.pfData.ToString());
                Terminal t;
                res = TerminalBLL.Instance.Get(clientData.terminalID, null, out t);
                if ((res == true) && (t != null))
                {
                    t.BlueTooth = data.blueTooth;
                    t.KernelVersion = data.kernelVersion;
                    t.DeviceName = data.deviceName;
                    t.DeviceType = data.deviceType;
                    t.Operator = Utils.GetDictText(data.phoneOperator, DictType.Operator);
                    t.OSType = Utils.GetDictText(data.osType, DictType.OS);
                    t.OSVersion = data.osVersion;
                    t.TotalRomSpace = data.totalRomSpace;
                    t.WifiMac = data.wifiMac;
                    res &= TerminalBLL.Instance.Update(clientData.terminalID, t);
                }

                #region 发送策略信息

                StrategyItemBLL.SendStrategyItemMsg(clientData.terminalID);

                #endregion
            }
            catch (Exception)
            {
            }

            return res;
        }

        //根据应用上告信息，更新设备安装的应用信息
        public static bool UpdateAppInfo(ClientData clientData)
        {
            bool res = true;
            TerminalApp terminalApp = new TerminalApp();
            List<AppInstallMsg> list = new List<AppInstallMsg>();
            try
            {

                TerminalApp data = JsonConvert.DeserializeObject<TerminalApp>(clientData.data.pfData.ToString());

                

                for (int i = 0; i < data.apps.Count; i++)
                {
                    AppInstallMsg model = new AppInstallMsg();

                    model.appName = data.apps[i].appName;
                    model.appVersionName = data.apps[i].appVersionName;
                    model.firstInstallTime = data.apps[i].firstInstallTime;
                    model.lastUpdateTime = data.apps[i].lastUpdateTime;
                    model.packageName = data.apps[i].packageName;

                    list.Add(model);

                }

                TerminalApp terminalapp = new TerminalApp();

                terminalapp.terminalId = clientData.terminalID.Trim();
                terminalapp.apps = list;

                //设备应用信息上告
                TerminalApp t;
                if ((true == AppInstallMsgBLL.Instance.GetByQuery(Query<TerminalApp>.EQ(p => p.terminalId, terminalapp.terminalId), out t)) && t == null)
                {
                    if (false == AppInstallMsgBLL.AddAppInstallMsg(terminalapp))
                    {
                        res = false;
                        LogHelper.WriteErrorLog(typeof(TerminalRouter), "终端上告应用信息入库失败");
                    }
                }
                else
                {
                    if(true==AppInstallMsgBLL.Instance.Delete(t.ID))
                    {
                        if (false == AppInstallMsgBLL.AddAppInstallMsg(terminalapp))
                        {
                            res = false;
                            LogHelper.WriteErrorLog(typeof(TerminalRouter), "终端上告应用信息更新失败");
                        }
                    }

                 

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(typeof(TerminalRouter), "安装应用信息入库异常" + ex.Message);
            }

            return res;
        }



    }
}