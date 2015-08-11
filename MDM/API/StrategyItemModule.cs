using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using MDM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDM.Models;
using MDM.BLL;
using MongoDB.Driver.Builders;

namespace MDM.API
{
    public class StrategyItemModule : NancyModule
    {

        public StrategyItemModule()
            : base("/api/strategyItem")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<StrategyItem>.Instance;
            this.bll = StrategyItemBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<StrategyItem> oul = null;
                if (true == this.bll.GetList(this.Bind<Pattern>(), out oul))
                {

                    foreach (StrategyItem model in oul)
                    {
                        if (model.startTime.Length >= 3)
                        {
                            model.startTime = model.startTime.Insert(2, ":");
                        }
                        if (model.endTime.Length >= 3)
                        {
                            model.endTime = model.endTime.Insert(2, ":");
                        }

                    }
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<StrategyItem>>(oul, res);
            };

            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<StrategyItem> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;

                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };

            Get["/{id}"] = _ =>
            {
                StrategyItem ou = null;
                var res = this.module.Get(_.id.Value as string, out ou);
                return Response.AsJson<StrategyItem>(ou, res);
            };

            //通过策略组ID获取策略项
            Get["strategyGroup/{GroupId}"] = _ =>
            {
                string GroupId = _.GroupId.Value as string;
                List<StrategyItem> list;
                var res = HttpStatusCode.InternalServerError;

                if (true == StrategyItemBLL.Instance.GetByQueryList(Query<StrategyItem>.EQ(p => p.GroupID, GroupId), out list))
                {

                    foreach (StrategyItem model in list)
                    {
                        if (model.startTime.Length >= 3)
                        {
                            model.startTime = model.startTime.Insert(2, ":");
                        }

                        if (model.startTime.Length >= 3)
                        {
                            model.endTime = model.endTime.Insert(2, ":");
                        }

                    }
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<List<StrategyItem>>(list, res);
            };


            Post["/"] = _ =>
            {

                var res = HttpStatusCode.OK;
                StrategyItem model = this.Bind<StrategyItem>();
                string StrategyItemid = this.module.AddTAndReturnID(model);

                if (StrategyItemid != null)
                {
                    StrategyItemBLL.SendAllTerminalStrategyByGroupId(model.GroupID);
                }
                //添加之后发送策略信息
                //if (1 == 1)
                //{
                //    StrategyItemBLL.SendAllTerminalStrategyByGroupId(model.GroupID);
                //}

                return Response.AsJson<string>(StrategyItemid, res); ;
            };


            Put["/{id}"] = _ =>
            {
                string id = _.id.Value;
                var res = HttpStatusCode.OK;

                StrategyItem model = this.Bind<StrategyItem>();
                if (res == this.module.Update(id, model))
                {
                    StrategyItemBLL.SendAllTerminalStrategyByGroupId(model.GroupID);
                }
                //这个是修改之后发送策略信息  
                //if (1 == 1)
                //{
                //    StrategyItemBLL.SendAllTerminalStrategyByGroupId(model.GroupID);
                //}

                return res;
            };

            //Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };

            Delete["/{id}"] = _ =>
            {

                var res = HttpStatusCode.OK;
                string id = _.id.Value;
                //需要记录当前删除项关联的策略组id
                string groupid = StrategyItemBLL.GetGroupidByStrateid(id);

                if (res == this.module.Delete(id))
                {   //取消关联关系
                    StrategyItemBLL.SendAllTerminalStrategyByGroupId(groupid);
                }

                return res;

            };



            //设置策略项及策略组关联
            Post["/{strategyitemid}/setrelation/{groupid}"] = SetRelationBetweenStratergyGroup;
            //取消策略项及策略组关联
            Delete["/{strategyitemid}/canclerelation/{groupid}"] = CancleRelationBetweenStratergyGroup;

        }

        private ModuleBase<StrategyItem> module { get; set; }
        private StrategyItemBLL bll { get; set; }


        private dynamic SetRelationBetweenStratergyGroup(dynamic arg)
        {
            var strategyitemid = arg.strategyitemid.Value as string;

            var groupid = arg.groupid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.SetRelation(strategyitemid, groupid))
            {
                res = HttpStatusCode.OK;
            }
            return res;

        }


        private dynamic CancleRelationBetweenStratergyGroup(dynamic arg)
        {
            var strategyitemid = arg.strategyitemid.Value as string;

            var groupid = arg.groupid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.CancleRelation(strategyitemid, groupid))
            {
                res = HttpStatusCode.OK;
            }
            return res;

        }



    }





}
