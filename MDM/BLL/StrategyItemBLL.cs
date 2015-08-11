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
    public class StrategyItemBLL : BLLBase<StrategyItem>
    {
        public static readonly new StrategyItemBLL Instance = new StrategyItemBLL();
        private StrategyItemBLL()
        {
        }

        public static void SendAllTerminalStrategyByGroupId(string groupid)
        {
            SendStrategyItem item = new SendStrategyItem();

            item.pfRequestID = Guid.NewGuid().ToString();

            StrategyGroup GroupMsg = GetStrategyGroup(groupid);
            if (GroupMsg != null)
            {
                item.policyID = GroupMsg.ID;
                item.version = GroupMsg.Version;
            }
            item.appBlackList = GetAppBlackList(groupid);
            //策略项的详细内容
            item.details = GetStrategyDetailMsg(groupid);

            LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "策略项发生改变时发送策略信息：其策略组policyID:" + groupid);

            BsonArray array = GetTerminals(GroupMsg.UserGroup);
            SendStrategyMsg(array, item);

        }

        public static void SendStrategyItemMsg(string terminal)
        {

            SendStrategyItem item = new SendStrategyItem();

            item.pfRequestID = Guid.NewGuid().ToString();

            Terminal padMsg = GetPolicyID(terminal);
            if (padMsg.policyID != null)
            {
                StrategyGroup GroupMsg = GetStrategyGroup(padMsg.policyID);
                //已发布 0  待发布1  待编辑2
                if (GroupMsg != null && GroupMsg.status == 0)//新增加 
                {
                    item.policyID = GroupMsg.ID;
                    item.version = GroupMsg.Version;
                }
                item.appBlackList = GetAppBlackList(GroupMsg.ID);
                //策略项的详细内容
                item.details = GetStrategyDetailMsg(GroupMsg.ID);

                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "终端id：" + terminal + "其策略组policyID:" + GroupMsg.ID);
            }
            else
            {
                string Name = "默认策略组";
                StrategyGroup model = StrategyGroupBLL.GetStrategyGroupByName(Name);
                if (model != null && model.status == 0)
                {
                    item.policyID = model.ID;
                    item.version = model.Version;
                }
                item.appBlackList = GetAppBlackList(model.ID);

                //策略项的详细内容
                item.details = GetStrategyDetailMsg(model.ID);

                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "发送默认策略组--终端id：" + terminal + "其策略组policyID:" + model.ID);

            }

            BsonArray array = new BsonArray();
            array.Add(terminal);
            SendStrategyMsg(array, item);
        }

        public static void SendStrategyItemMsg(BsonArray terminals)
        {

            SendStrategyItem item = new SendStrategyItem();

            item.pfRequestID = Guid.NewGuid().ToString();

            string Name = "默认策略组";
            StrategyGroup model = StrategyGroupBLL.GetStrategyGroupByName(Name);
            if (model != null && model.status == 0)
            {
                item.policyID = model.ID;
                item.version = model.Version;
            }
            item.appBlackList = GetAppBlackList(model.ID);

            //策略项的详细内容
            item.details = GetStrategyDetailMsg(model.ID);

            //LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "发送默认策略组--终端id：" + terminal + "其策略组policyID:" + model.ID);

            BsonArray array = new BsonArray();
            array.Add(terminals);
            SendStrategyMsg(array, item);
        }


        public static BsonArray GetTerminals(string[] groupids)
        {
            IEnumerable<User> usergroup;
            List<Terminal> terminals;
            BsonArray array = new BsonArray();
            foreach (string groupid in groupids)
            {
                if ((true == UserGroupBLL.Instance.GetUserFromGroup(groupid, out  usergroup)) && (usergroup != null))
                {
                    foreach (User user in usergroup)
                    {
                        if ((true == TerminalBLL.Instance.GetByQueryList(Query<Terminal>.EQ(p => p.UserId, user.ID), out  terminals)) && (terminals != null))
                        {
                            foreach (Terminal item in terminals)
                            {
                                array.Add(item.ID);
                            }
                        }
                    }
                }
            }

            return array;
        }

        public static BsonArray ChangeGroupidAndReturnTerminals(string groupid)
        {

            List<Terminal> terminals;
            BsonArray array = new BsonArray();
            if ((true == TerminalBLL.Instance.GetByQueryList(Query<Terminal>.EQ(p => p.policyID, groupid), out  terminals)) && (terminals != null))
            {
                foreach (Terminal item in terminals)
                {
                    item.policyID = null;
                    array.Add(item.ID);
                }
            }

            return array;
        }


        public static List<StrategyDetail> GetStrategyDetailMsg(string GroupId)
        {
            List<StrategyDetail> list = new List<StrategyDetail>();
            List<StrategyItem> outList;

            if (true == StrategyItemBLL.Instance.GetByQueryList(Query<StrategyItem>.EQ(p => p.GroupID, GroupId), out outList))
            {
                foreach (StrategyItem model in outList)
                {
                    StrategyDetail detail = new StrategyDetail();
                    detail.startTime = model.startTime;
                    detail.endTime = model.endTime;
                    detail.areaInfo = model.reginalInterval;

                    try
                    {
                        Strategy strategy = StrategyItemBLL.GetStrategy(model.StrategyID);
                        if (strategy != null)
                        {
                            detail.itemCode = Convert.ToInt32(strategy.Code.TrimStart().TrimEnd());
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteErrorLog(typeof(StrategyItemBLL), "获取策略信息失败");
                    }


                    Punishment punishment = StrategyItemBLL.GetPunishment(model.punishmentID);
                    if (punishment != null)
                    {
                        detail.action = punishment.Code;
                    }

                    list.Add(detail);
                }
            }

            return list;
        }

        public static Strategy GetStrategy(string id)
        {

            Strategy strategy;

            bool res = StrategyBLL.Instance.GetByQuery(Query.And(Query<Strategy>.EQ(p => p.ID, id)), out strategy);

            if (strategy == null)
            {
                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "策略code为空");
            }

            return strategy;

        }

        public static Punishment GetPunishment(string id)
        {

            Punishment punishment;

            bool res = PunishmentBLL.Instance.GetByQuery(Query.And(Query<Punishment>.EQ(p => p.ID, id)), out punishment);

            if (punishment == null)
            {
                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "惩罚code为空");
            }

            return punishment;

        }

        public static StrategyGroup GetStrategyGroup(string id)
        {
            StrategyGroup strategygroup;

            bool res = StrategyGroupBLL.Instance.GetByQuery(Query.And(Query<StrategyGroup>.EQ(p => p.ID, id)), out strategygroup);

            if (strategygroup == null)
            {
                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "策略组信息为空");
            }

            return strategygroup;
        }

        public static string[] GetAppBlackList(string id)
        {
            List<AppBlackList> appblacklist;
            List<string> appcodeNameList = new List<string>();

            bool res = AppBlackListBLL.Instance.GetByQueryList(Query.And(Query<AppBlackList>.EQ(p => p.GroupId, id)), out appblacklist);

            if (appblacklist.Count == 0)
            {
                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "策略组id为" + id + "所对应的黑名单信息为空");
            }
            else
            {
                foreach (AppBlackList model in appblacklist)
                {
                    appcodeNameList.Add(model.ApkCode);
                }
            }

            return appcodeNameList.ToArray();
        }

        public static Terminal GetPolicyID(string terminal)
        {
            Terminal ter;

            bool res = TerminalBLL.Instance.GetByQuery(Query.And(Query<Terminal>.EQ(p => p.ID, terminal)), out ter);

            if (ter == null)
            {
                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "终端信息为空");
            }

            return ter;
        }


        public static void SendStrategyMsg(BsonArray terminals, SendStrategyItem data)
        {

            Send2Client clientMsg = new Send2Client();
            clientMsg.commandID = "1";
            clientMsg.pfData = data;

            HttpStatusCode result = EdgeServerBLL.SendMessage2Client(terminals, clientMsg, RequestType.Strategy);

            if (result != HttpStatusCode.OK)
                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "登录成功后发送策略信息到EdgeServer失败");

        }

        public bool SetRelation(string strategyitemid, string groupid)
        {
            var res = false;

            StrategyItem strategyitem;
            StrategyGroup strategygroup;

            #region 策略项中策略组的配置
            if (strategyitemid != null)
            {
                if ((true == StrategyItemBLL.Instance.GetByQuery(Query<StrategyItem>.EQ(p => p.ID, strategyitemid), out strategyitem)) && (strategyitem != null))
                {
                    strategyitem.GroupID = groupid;
                    if (true == StrategyItemBLL.Instance.Update(Query.And(Query<StrategyItem>.EQ(p => p.ID, strategyitemid)), strategyitem))
                    {
                        res = true;

                    }
                }
            }
            #endregion

            if (groupid != null)
            {
                if ((true == StrategyGroupBLL.Instance.GetByQuery(Query.And(Query<StrategyGroup>.EQ(p => p.ID, groupid)), out strategygroup)) && (strategygroup != null))
                {
                    if (strategygroup.Item == null)
                    {
                        strategygroup.Item = new BsonArray();
                    }

                    if (!strategygroup.Item.Contains(strategyitemid))
                    {
                        strategygroup.Item.Add(strategyitemid);
                    }

                    if (true == StrategyGroupBLL.Instance.Update(Query.And(Query<StrategyGroup>.EQ(p => p.ID, groupid)), strategygroup))
                    {
                        res = true;
                    }
                }

            }

            return res;
        }

        public bool CancleRelation(string strategyitemid, string groupid)
        {
            bool res = false;

            StrategyGroup t;
            if ((true == StrategyGroupBLL.Instance.GetByQuery(Query<StrategyGroup>.EQ(x => x.ID, groupid), out t)) && (t != null))
            {
                if (t.Item.Contains(strategyitemid))
                {
                    t.Item.Remove(strategyitemid);

                    if (true == StrategyGroupBLL.Instance.Update(Query<StrategyGroup>.EQ(x => x.ID, groupid), t))
                    {
                        res = true;
                    }
                }
                else
                {
                    res = true;
                }
            }

            StrategyItem p;
            if ((true == StrategyItemBLL.Instance.GetByQuery(Query<StrategyItem>.EQ(x => x.ID, strategyitemid), out p)) && (p != null))
            {
                p.GroupID = string.Empty;
                if ((true == StrategyItemBLL.Instance.Update(Query<StrategyItem>.EQ(x => x.ID, strategyitemid), p)))
                {
                    res = true;
                }
            }

            return res;
        }

        public static string GetGroupidByStrateid(string strateid)
        {
            string Groupid = string.Empty;
            StrategyItem item;

            if ((true == StrategyItemBLL.Instance.GetByQuery(Query<StrategyItem>.EQ(p => p.ID, strateid), out item)) && (item != null))
            {
                if (item.GroupID != null)
                {
                    Groupid = item.GroupID;
                }

            }

            return Groupid;
        }




    }


}