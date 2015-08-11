using MDM.Helpers;
using MDM.Models;
using MongoDB.Driver;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;
using Nancy.ModelBinding;
using MDM.BLL;
using MongoDB.Driver.Builders;

namespace MDM.API
{
    public class AppTemplateModule : NancyModule
    {
        public AppTemplateModule()
            : base("/api/appTemplate")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<AppTemplate>.Instance;
            this.bll = AppTemplateBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<AppTemplate> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<AppTemplate>>(oul, res);
            };

            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<AppTemplate> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;

                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<int>(totalCount, res);
            };


            Get["/{id}"] = _ => { AppTemplate ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<AppTemplate>(ou, res); };

            Post["/"] = _ =>
            {
                return this.module.Add(this.Bind<AppTemplate>());
            };

            Put["/{id}"] = _ => { return this.module.Update(_.id.Value as string, this.Bind<AppTemplate>()); };
            Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };
            //设置应用同应用模版关联关系
            Post["/{templateid}/app/{appid}/"] =  SetRelationBetweenTandA;
            Post["/{tid}"] = SetApps;
            //取消模版同应用的关联关系  
            Delete["/{templateid}/app/{deleteappid}/"] = DeleteApp;
            //获取指定模版下的所有应用
            Get["/{templateid}/app/"] = GetTemplateApps;

        }

        private ModuleBase<AppTemplate> module { get; set; }
        private AppTemplateBLL bll { get; set; }

        private dynamic SetApps(dynamic arg)
        {

            var tid = arg.tid.Value as string;
            var res = HttpStatusCode.InternalServerError;
            var appids = this.Bind<IEnumerable<string>>();

            if (appids.Count() == 0)
            {
                res = HttpStatusCode.OK;
                LogHelper.WriteErrorLog(typeof(AppTemplateModule), "未收到app数据_SetApps");
                return res;
            }

            if (this.bll.SetAppTemplate(tid, appids))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic DeleteApp(dynamic arg)
        {

            var tid = arg.templateid.Value as string;
            var appid = arg.deleteappid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            #region 保留
            //var appids = this.Bind<IEnumerable<string>>();
            //if (appids.Count() == 0)
            //{
            //    res = HttpStatusCode.OK;
            //    LogHelper.WriteErrorLog(typeof(AppTemplateModule), "未收到app数据_DeleteApp");
            //    return res;
            //} 
            #endregion

            if (this.bll.DelAppFormTemplate(tid, appid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic SetRelationBetweenTandA(dynamic arg)
        {
            var tid = arg.templateid.Value as string;
            var appid = arg.appid.Value as string;

            var res = HttpStatusCode.InternalServerError;

            if (this.bll.SetAppTemplate(tid, appid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic GetTemplateApps(dynamic arg)
        {
            string templateid = arg.templateid.Value as string;
            
            var res = HttpStatusCode.InternalServerError;
            List<App> apps;

            if( true == this.bll.GetAllAppMsg(templateid,out apps))
            {
                res = HttpStatusCode.OK;
            }
            
            return Response.AsJson<List<App>>(apps, res);
        }

    }
}