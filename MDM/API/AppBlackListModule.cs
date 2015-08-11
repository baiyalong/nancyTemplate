using MDM.Models;
using MDM.BLL;
using MongoDB.Driver.Builders;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.API
{
    public class AppBlackListModule : NancyModule
    {
        public AppBlackListModule()
            : base("/api/AppBlackList")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<AppBlackList>.Instance;
            this.bll = AppBlackListBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<AppBlackList> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<AppBlackList>>(oul, res);
            };
            
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<AppBlackList> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;

                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };

            Get["/{id}"] = _ => { AppBlackList ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<AppBlackList>(ou, res); };

            //通过策略组ID获取黑名单
            Get["/strategyGroup/{Id}"] = _ =>
            {
                string Id = _.Id.Value as string;
                List<AppBlackList> list;
                var res = HttpStatusCode.InternalServerError;

                if (true == AppBlackListBLL.Instance.GetByQueryList(Query<AppBlackList>.EQ(p => p.GroupId, Id), out list))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<List<AppBlackList>>(list, res);
            };

            Post["/"] = _ => {

                var res = HttpStatusCode.OK;
                AppBlackList model = this.Bind<AppBlackList>();
                if (res == this.module.Add(model))
                {

                    StrategyItemBLL.SendAllTerminalStrategyByGroupId(model.GroupId);
                }
                //黑名单添加时 其策略信息要重新发送 
                //if (1 == 1)
                //{
                //    StrategyItemBLL.SendAllTerminalStrategyByGroupId(model.GroupId);
                //}
                
                return res;
            };
            Put["/{id}"] = _ => {

                string id = _.id.Value;
                var res = HttpStatusCode.OK;
                AppBlackList model = this.Bind<AppBlackList>();
                if (res == this.module.Update(id, model))
                {
                    StrategyItemBLL.SendAllTerminalStrategyByGroupId(model.GroupId);
                }
                //黑名单重新修改时 发送策略信息
                //if (1 == 1)
                //{
                //    StrategyItemBLL.SendAllTerminalStrategyByGroupId(model.GroupId);
                //}

                return res; 
            };
            Delete["/{id}"] = _ => {

                var res = HttpStatusCode.OK;
                string id = _.id.Value;
                //需要记录当前删除项关联的策略组id
                string groupid = AppBlackListBLL.GetGroupidById(id);

                if (res == this.module.Delete(id))
                {   //取消关联关系后发送 
                    StrategyItemBLL.SendAllTerminalStrategyByGroupId(groupid);
                }

                return res;
            };
            
            //设置策略组及黑名单关联
            Post["/{blacklistid}/setrelation/{groupid}"] = SetRelationBetweenSAndA;
            //取消策略项及黑名单关联
            Delete["/{blacklistid}/canclerelation/{groupid}"] = CancleRelationBetweenSAndA;
        }

        private ModuleBase<AppBlackList> module { get; set; }
        private AppBlackListBLL bll { get; set; }

        private dynamic SetRelationBetweenSAndA(dynamic arg)
        {
            var blacklistid = arg.blacklistid.Value as string;

            var groupid = arg.groupid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.SetRelation(blacklistid, groupid))
            {
                res = HttpStatusCode.OK;
            }
            return res;

        }


        private dynamic CancleRelationBetweenSAndA(dynamic arg)
        {
            var blacklistid = arg.blacklistid.Value as string;

            var groupid = arg.groupid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.CancleRelation(blacklistid, groupid))
            {
                res = HttpStatusCode.OK;
            }
            return res;

        }


    }
}