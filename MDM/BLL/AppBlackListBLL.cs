using MDM.Helpers;
using MDM.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class AppBlackListBLL : BLLBase<AppBlackList>
    {
        public static readonly new AppBlackListBLL Instance = new AppBlackListBLL();
        private AppBlackListBLL()
        {
        }

        public bool SetRelation(string blacklistid, string groupid)
        {
            var res = false;

            AppBlackList appblack;
            StrategyGroup strategygroup;

            if (blacklistid != null)
            {
                if ((true == AppBlackListBLL.Instance.GetByQuery(Query<AppBlackList>.EQ(p => p.ID, blacklistid), out appblack)) && (appblack != null))
                {
                    appblack.GroupId = groupid;
                    if (true == AppBlackListBLL.Instance.Update(Query.And(Query<AppBlackList>.EQ(p => p.ID, blacklistid)), appblack))
                    {
                        res = true;

                    }
                }
            }

            if (groupid != null)
            {
                if ((true == StrategyGroupBLL.Instance.GetByQuery(Query.And(Query<StrategyGroup>.EQ(p => p.ID, groupid)), out strategygroup)) && (strategygroup != null))
                {
                    if (strategygroup.AppBlackItem == null)
                    {
                        strategygroup.AppBlackItem = new BsonArray();
                    }

                    if (!strategygroup.AppBlackItem.Contains(blacklistid))
                    {
                        strategygroup.AppBlackItem.Add(blacklistid);
                    }

                    if (true == StrategyGroupBLL.Instance.Update(Query.And(Query<StrategyGroup>.EQ(p => p.ID, groupid)), strategygroup))
                    {
                        res = true;
                    }
                }

            }

            return res;
        }


        public bool CancleRelation(string blacklistid, string groupid)
        {
            bool res = false;

            StrategyGroup t;
            if ((true == StrategyGroupBLL.Instance.GetByQuery(Query<StrategyGroup>.EQ(x => x.ID, groupid), out t)) && (t != null))
            {
                if (t.AppBlackItem.Contains(blacklistid))
                {
                    t.AppBlackItem.Remove(blacklistid);

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

            AppBlackList p;
            if ((true == AppBlackListBLL.Instance.GetByQuery(Query<AppBlackList>.EQ(x => x.ID, blacklistid), out p)) && (p != null))
            {
                p.GroupId = string.Empty;
                if ((true == AppBlackListBLL.Instance.Update(Query<AppBlackList>.EQ(x => x.ID, blacklistid), p)))
                {
                    res = true;
                }
            }

            return res;
        }

        public static string GetGroupidById(string id)
        {
            string Groupid = string.Empty;

            AppBlackList item;
            if ((true == AppBlackListBLL.Instance.GetByQuery(Query<AppBlackList>.EQ(p => p.ID,id), out item)) && (item != null))
            {
                if (item.GroupId != null)
                {
                    Groupid = item.GroupId;
                }
            }

            return Groupid;
        }

    }

    public class AppBlackListRouter
    {
        public AppBlackListRouter() { }

        public static bool AppBlackListReport(ClientData clientData)
        {
            bool res = false;

            try
            {
                BlackListMessage msg = JsonConvert.DeserializeObject<BlackListMessage>(clientData.data.pfData.ToString());

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

                                if (model.BlackList == null)
                                {
                                    model.BlackList = new List<string>();
                                }

                                foreach (string item in msg.blackList)
                                {
                                    model.BlackList.Add(item);
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

                                if (sr.BlackList == null)
                                {
                                    sr.BlackList = new List<string>();
                                }

                                foreach (string item in msg.blackList)
                                {
                                    if(!sr.BlackList.Contains(item))
                                    {
                                        sr.BlackList.Add(item);
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
                LogHelper.WriteInfoLog(typeof(AppBlackListBLL), "黑名单违规上告插入记录异常" + ex.Message);

                res = false;
            }

            return res;
        }


    }

}