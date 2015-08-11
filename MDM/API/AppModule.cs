using MDM.Helpers;
using MDM.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nancy;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;
using MDM.BLL;
using Nancy.ModelBinding;
using MongoDB.Driver.Builders;


namespace MDM.API
{
    public class AppModule : NancyModule
    {
        public AppModule()
            : base("/api/app")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<App>.Instance;
            this.bll = AppBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<App> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<App>>(oul, res);
            };

            //获取列表总计路数
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<App> oul = null;
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
                var res = HttpStatusCode.InternalServerError;
                App ou = null;
                string id = _.id.Value as string;
                App app;

                if (true == AppBLL.Instance.GetByQuery(Query<App>.EQ(p => p.ID, id), out app))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<App>(app, res);
            };

            Post["/"] = _ =>
            {
                App model = this.Bind<App>();
                return this.module.AddTAndReturnID(model);
            };

            Put["/{id}"] = _ =>
            {
                var id = _.id.Value as string;
                App model = this.Bind<App>();
                return this.module.Update(id, model);
            };
            Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };
            //设置应用分类及应用关联
            Post["/{appid}/classify/{classifyid}"] = SetAppClassifyList;
            //取消应用分类及应用关联
            Delete["/{appid}/classify/{classifyid}"] = CancleAppClassify;
            //获取指定应用分类下的所有应用
            Get["/classify/{classifyid}"] = GetClassifyApps;
            //删除app   
            Delete["/delApp/{appid}/"] = DeleteApp;
            //发布时建立应用和用户组的关系 
            Post["/publish/{appId}/"] = SetRelationBetweenAppAndGroup;

        }

        private ModuleBase<App> module { get; set; }
        private AppBLL bll { get; set; }

        private dynamic SetAppClassifyList(dynamic arg)
        {
            var appid = arg.appid.Value as string;

            var classifyid = arg.classifyid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.SetAppClassify(appid, classifyid))
            {
                res = HttpStatusCode.OK;
            }
            return res;

        }

        private dynamic CancleAppClassify(dynamic arg)
        {
            var appid = arg.appid.Value as string;

            var classifyid = arg.classifyid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.CancleAppClassifyRelation(appid, classifyid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic DeleteAppRelation(dynamic arg)
        {
            var appid = arg.appid.Value as string;
            var tid = arg.tid.Value as string;
            var cid = arg.cid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.DeleteAppClassifyRelation(appid, tid, cid))
            {
                res = HttpStatusCode.OK;
            }
            return res;


        }

        private dynamic GetClassifyApps(dynamic arg)
        {
            string classifyid = arg.classifyid.Value as string;

            var res = HttpStatusCode.InternalServerError;
            List<App> apps;

            if (true == this.bll.GetAllClassifyApps(classifyid, out apps))
            {
                res = HttpStatusCode.OK;
            }
            return Response.AsJson<List<App>>(apps, res);
        }

        private dynamic DeleteApp(dynamic arg)
        {
            string appid = arg.appid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (true == this.bll.DeleteAppRelation(appid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic SetRelationBetweenAppAndGroup(dynamic arg)
        {
            var res = HttpStatusCode.InternalServerError;
            string appid = arg.appid.Value as string;

            var groups = this.Bind<IEnumerable<string>>();
            if (true == AppBLL.SetRalationBetweenAppAndUgerGroups(appid, groups))
            {
                res = HttpStatusCode.OK;
            }


            return res;
        }
    }
}