using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDM.Helpers;
using MDM.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nancy;
using Nancy.Responses;
using Nancy.Security;
using MDM.BLL;
using Nancy.ModelBinding;
using MongoDB.Bson;

namespace MDM.API
{
    public class StrategyGroupModule : NancyModule
    {
        public StrategyGroupModule()
            : base("/api/strategyGroup")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<StrategyGroup>.Instance;
            this.bll = StrategyGroupBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<StrategyGroup> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<StrategyGroup>>(oul, res);
            };

            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<StrategyGroup> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;

                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };

            Get["/{id}"] = _ => { 
                StrategyGroup ou = null;  
                var res = this.module.Get(_.id.Value as string, out ou); 
                return Response.AsJson<StrategyGroup>(ou, res);
            };

            Post["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                StrategyGroup mode = this.Bind<StrategyGroup>();
                mode.Version = 1;
                string groupid = this.module.AddTAndReturnID(mode);
                res = HttpStatusCode.OK;
                return Response.AsJson<string>(groupid, res);
                
            };

            Put["/{id}"] = _ =>
            {
                string id = _.id.Value as string;
                StrategyGroup mode=this.Bind<StrategyGroup>();
                //暂缓
                //mode.Version = mode.Version ++;
                return this.module.Update(id, mode);
            };
            Delete["/{id}"] = _ =>
            {
                //删除策略组时 发送默认的策略 
                var res = HttpStatusCode.OK;
                string groupid = _.id.Value;
                if (res == this.module.Delete(groupid))
                {
                    //获取全部的终端信息 
                    BsonArray terminals = StrategyItemBLL.ChangeGroupidAndReturnTerminals(groupid);
                    StrategyItemBLL.SendStrategyItemMsg(terminals);
                }
                return res;
            };
            //设置策略组同终端之间的关联
            Post["/{groupid}/setrelation/{terminalid}"] = SetRelationBetweenTAndG;
            //删除策略组同终端关联 
            Delete["/{groupid}/delete/{terminalid}"] = DeleteTerminalRelation;
            //删除策略组 (解除策略组同策略项、黑名单关联)
            Delete["/delete/{groupid}"] = DeleteRelation;

            //发布时建立应用和用户组的关系 
            Post["/publish/{StrategyGroupId}/"] = SetRelationBetweenStrategyGroupAndGroup;

        }


        private ModuleBase<StrategyGroup> module { get; set; }
        private StrategyGroupBLL bll { get; set; }

        private dynamic SetRelationBetweenTAndG(dynamic arg)
        {
            var groupid = arg.groupid.Value as string;

            var terminalid = arg.terminalid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.SetRelation(groupid, terminalid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic DeleteTerminalRelation(dynamic arg)
        {
            var groupid = arg.groupid.Value as string;

            var terminalid = arg.terminalid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.DeleteTerminalRelation(groupid, terminalid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic DeleteRelation(dynamic arg)
        {
            var groupid = arg.groupid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.DeleteRelation(groupid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic SetRelationBetweenStrategyGroupAndGroup(dynamic arg)
        {
            var res = HttpStatusCode.InternalServerError;
            string strategyGroupId = arg.StrategyGroupId.Value as string;

            var groups = this.Bind<IEnumerable<string>>();
            if (true == StrategyGroupBLL.SetRalationBetweenAppAndUgerGroups(strategyGroupId, groups))
            {
                res = HttpStatusCode.OK;
            }


            return res;
        }


    }
}