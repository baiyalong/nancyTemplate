using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using MDM.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MDM.Helpers;

namespace MDM.BLL
{
    public class StrategyGroupBLL : BLLBase<StrategyGroup>
    {
        public static readonly new StrategyGroupBLL Instance = new StrategyGroupBLL();
        private StrategyGroupBLL()
        {
        }

        public bool SetRelation(string groupid, string terminalid)
        {
            bool res = false;

            Terminal temp;

            if ((true == TerminalBLL.Instance.GetByQuery(Query<Terminal>.EQ(p => p.ID, terminalid), out temp)) && (temp != null))
            {

                temp.policyID = groupid;
                if (true == TerminalBLL.Instance.Update(Query<Terminal>.EQ(p => p.ID, terminalid), temp))
                {
                    res = true;

                    #region 关联建立成功发送策略信息 
                    //StrategyGroupBLL.SendStrategyMsg(groupid, terminalid);  
                    StrategyItemBLL.SendStrategyItemMsg(terminalid); 
                    #endregion
                }
            }
            return res;
        }

        #region 往指定终端发送策略信息
        public static StrategyGroup GetStrategyGroupByName(string Name)
        {
            StrategyGroup strategygroup;

            bool res = StrategyGroupBLL.Instance.GetByQuery(Query.And(Query<StrategyGroup>.EQ(p => p.Name, Name)), out strategygroup);

            if (strategygroup == null)
            {
                LogHelper.WriteInfoLog(typeof(StrategyItemBLL), "策略组信息为空");
            }

            return strategygroup;
        }

        public static void SendStrategyMsg(string strategygroupId, string termialid)
        {
            SendStrategyItem item = new SendStrategyItem();
            try
            {
                if (strategygroupId != null)
                {
                    item.policyID = strategygroupId;
                    StrategyGroup GroupMsg = StrategyItemBLL.GetStrategyGroup(strategygroupId);
                    if (GroupMsg != null)
                    {
                        item.version = GroupMsg.Version;
                    }
                    item.appBlackList = StrategyItemBLL.GetAppBlackList(strategygroupId);
                    //策略项的详细内容
                    item.details = StrategyItemBLL.GetStrategyDetailMsg(strategygroupId);

                    LogHelper.WriteInfoLog(typeof(StrategyGroupBLL), "发送策略信息：终端id为：" + termialid + "-策略组id为：" + strategygroupId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(typeof(StrategyGroupBLL), "发送策略信息异常：终端id为：" + termialid + "-策略组id为：" + strategygroupId + "异常信息：" + ex.Message);
            }

            BsonArray array = new BsonArray();
            array.Add(termialid);
            StrategyItemBLL.SendStrategyMsg(array, item);
        } 
        #endregion

        public bool DeleteTerminalRelation(string groupid, string terminalid)
        {
            bool res = false;

            Terminal temp;

            if (true == TerminalBLL.Instance.GetByQuery(Query<Terminal>.EQ(p => p.ID, terminalid), out temp))
            {

                if (temp.policyID == groupid)
                {
                    temp.policyID = string.Empty;
                }

                if (true == TerminalBLL.Instance.Update(Query<Terminal>.EQ(p => p.ID, terminalid), temp))
                {
                    res = true;
                    //向终端发送策略信息 
                    StrategyItemBLL.SendStrategyItemMsg(terminalid); 
                }
            }

            return res;
        }


        public bool DeleteRelation(string groupid)
        {
            bool res = false;

            StrategyGroup temp;

            if ((true == StrategyGroupBLL.Instance.GetByQuery(Query<StrategyGroup>.EQ(p => p.ID, groupid
                ), out temp)) && (temp != null))
            {
                try
                {
                    if (temp.Item.Count > 0)
                    {
                        for (int i = 0; i < temp.Item.Count; i++)
                        {
                            StrategyItem item;
                            if ((true == StrategyItemBLL.Instance.GetByQuery(Query<StrategyItem>.EQ(p => p.ID, temp.Item[i].ToString()), out item)) && (item != null))
                            {
                                if (item.GroupID == groupid)
                                {
                                    item.GroupID = string.Empty;
                                }

                                if (true == StrategyItemBLL.Instance.Update(Query<StrategyItem>.EQ(p => p.ID, temp.Item[i].ToString()), item))
                                {
                                    res = true;


                                }

                            }
                        }
                    }

                    if (temp.AppBlackItem.Count > 0)
                    {
                        for (int i = 0; i < temp.AppBlackItem.Count; i++)
                        {
                            AppBlackList list;
                            if ((true == AppBlackListBLL.Instance.GetByQuery(Query<AppBlackList>.EQ(p => p.ID, temp.AppBlackItem[i].ToString()), out list)) && (list != null))
                            {
                                if (list.GroupId == groupid)
                                {
                                    list.GroupId = string.Empty;
                                }

                                if (true == AppBlackListBLL.Instance.Update(Query<AppBlackList>.EQ(p => p.ID, temp.AppBlackItem[i].ToString()), list))
                                {
                                    res = true;
                                }
                            }
                        }
                    }

                    if (true == StrategyGroupBLL.Instance.Delete(groupid))
                    {
                        res = true;
                    }


                }
                catch (Exception ex)
                {
                    res = false;
                    LogHelper.WriteErrorLog(typeof(StrategyGroupBLL), "删除策略组异常信息：" + ex.Message);

                }

            }

            return res;
        }


        public static bool SetRalationBetweenAppAndUgerGroups(string id, IEnumerable<string> groups)
        {
            bool res = false;
            List<string> usergroups = new List<string>();
            List<string> terminals = new List<string>();
            BsonArray array = new BsonArray();
            string msg = string.Empty;
            string appcontent = string.Empty;
           
            foreach (var g in groups)
            {
                usergroups.Add(g);
            }

            StrategyGroup strategygroup;
            if ((true == StrategyGroupBLL.Instance.GetByQuery(Query<StrategyGroup>.EQ(x => x.ID, id), out strategygroup)) && (strategygroup != null))
            {
                res = true;
                strategygroup.UserGroup = usergroups.ToArray();

                if (true == StrategyGroupBLL.Instance.Update(strategygroup.ID, strategygroup))
                {
                    terminals = GetBsonArray(usergroups);
                    if (terminals.Count != 0)
                    {
                        
                        for (int i = 0; i < terminals.Count; i++)
                        {
                            if (!array.Contains(terminals[i].ToString()))
                            {
                                array.Add(terminals[i].ToString());
                            }

                        }
                        //终端发送的部分  暂缓 

                        //SendTerminalMsg(array, msg, appcontent);
                    }
                    else
                    {
                        LogHelper.WriteErrorLog(typeof(AppBLL), "无获取终端信息,不发送策略应用信息");
                    }

                }
            }

            return res;
        }

        private static List<string> GetBsonArray(List<string> groups)
        {

            List<string> tids = new List<string>();

            IEnumerable<Terminal> oul;

            Pattern pattern = new Pattern();
            pattern.pageNum = 1;
            pattern.pageSize = 500;

            if ((true == TerminalBLL.Instance.GetList(pattern, out oul)) && (oul != null))
            {
                foreach (var t in oul)
                {
                    if (t.UserGroup != null)
                    {
                        for (int i = 0; i < t.UserGroup.Count(); i++)
                        {
                            if (groups.Contains(t.UserGroup[i].ToString()))
                            {
                                tids.Add(t.ID);
                            }
                        }
                    }

                }
            }

            return tids;
        }
    }

    public class StrategyGroupRouter
    {
        public StrategyGroupRouter() { }

        public static bool StrategyReport(ClientData clientData)
        {
            bool res = false;
            try
            {
                StrategyMessage msg = JsonConvert.DeserializeObject<StrategyMessage>(clientData.data.pfData.ToString());

                Terminal t;

                if ((true == TerminalBLL.Instance.GetByQuery(Query<Terminal>.EQ(p => p.ID, clientData.terminalID), out t)) && (t != null))
                {
                    StrategyGroup strategroup;
                    if ((true == StrategyGroupBLL.Instance.GetByQuery(Query<StrategyGroup>.EQ(p => p.ID, msg.policyID), out strategroup)) && (strategroup != null))
                    {
                        //同一策略组下且版本相同
                        if (msg.policyVersion == strategroup.Version.ToString() && msg.policyID == t.policyID)
                        {
                            //同一版本
                            StrategyRecord sr;
                            if (true == StrategyRecordBLL.Instance.GetByQuery(Query<StrategyRecord>.EQ(p => p.TerminalId, clientData.terminalID), out sr) && (sr == null))
                            {
                                #region 新增策略违规上告信息
                                StrategyRecord model = new StrategyRecord();
                                model.TerminalId = clientData.terminalID;
                                model.PolicyId = msg.policyID;
                                model.Version = msg.policyVersion;

                                if (model.Items == null)
                                {
                                    model.Items = new List<string>();
                                }

                                foreach (string item in msg.itemCode)
                                {
                                    Strategy strategy;
                                    if ((true == StrategyBLL.Instance.GetByQuery(Query<Strategy>.EQ(p => p.Code, item), out strategy)) && (strategy != null))
                                    {

                                        model.Items.Add(strategy.Name);
                                    }

                                }
                                #endregion

                                if (true == StrategyRecordBLL.Instance.Add(model))
                                {
                                    res = true;
                                }
                            }
                            else
                            {
                               
                                #region 更新策略违规上告信息
                                
                                sr.TerminalId = clientData.terminalID;
                                sr.Version = msg.policyVersion;

                                if (sr.Items == null)
                                {
                                    sr.Items = new List<string>();
                                }

                                foreach (string item in msg.itemCode)
                                {
                                    Strategy strategy;
                                    if ((true == StrategyBLL.Instance.GetByQuery(Query<Strategy>.EQ(p => p.Code, item), out strategy)) && (strategy != null))
                                    {
                                        if (!sr.Items.Contains(strategy.Name))
                                        {
                                            sr.Items.Add(strategy.Name);
                                        }
                                        
                                    }

                                }
                                #endregion

                                if (true == StrategyRecordBLL.Instance.Update(Query<StrategyRecord>.EQ(p => p.TerminalId, clientData.terminalID), sr))
                                {
                                    res = true;
                                }
                            }
                            
                        }
                        else
                        {
                            //同一策略的不同版本 
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteInfoLog(typeof(StrategyGroupBLL), "策略违规上告插入记录异常" + ex.Message);

                res = false;
            }

            return res;
        }
    }
}