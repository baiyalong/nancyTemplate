using MDM.Helpers;
using MDM.Models;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDM.BLL;
using MongoDB.Driver.Builders;

namespace MDM.API
{
    public class AppClassifyModule : NancyModule
    {
        public AppClassifyModule()
            : base("/api/appClassify")
        {
            this.RequiresAuthentication();
            this.module = ModuleBase<AppClassify>.Instance;
            this.bll = AppClassifyBLL.Instance;
            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<AppClassify> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<AppClassify>>(oul, res);
            };


            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<AppClassify> oul = null;
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
               
                AppClassify ou = new AppClassify (); 

                var res = this.module.Get(_.id.Value as string, out ou);

                return Response.AsJson<AppClassify>(ou, res); 
            };
            Post["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                var classify = this.Bind<AppClassify>();
                AppClassify model = null;
                bool reslut = this.bll.Get(null, Query<AppClassify>.EQ(e => e.classifyName, classify.classifyName), out model);
                if (model != null && reslut == true)
                {
                    res = HttpStatusCode.OK;
                    string msg = "当前应用分类名称已存在！";
                    return Response.AsJson<string>(msg, res);
                }

                return this.module.Add(this.Bind<AppClassify>());
            };

            Put["/{id}"] = _ => { return this.module.Update(_.id.Value as string, this.Bind<AppClassify>()); };

            Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };

        }

        private ModuleBase<AppClassify> module { get; set; }

        private AppClassifyBLL bll { get; set; }





    }
}