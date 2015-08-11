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

namespace MDM.API
{
    public class AppCatalogModule : NancyModule
    {

        public MongoCollection<AppCatalog> AppCatalogCollection = MongoHelper.GetCollection<AppCatalog>();

        public AppCatalogModule()
            : base("/api/appCatalog")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<AppCatalog>.Instance;
            this.bll = AppCatalogBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<AppCatalog> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
               
                return Response.AsJson<IEnumerable<AppCatalog>>(oul, res);
            };

            //获取列表总计路数
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<AppCatalog> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;

                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };

            Get["/{id}"] = _ => { AppCatalog ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<AppCatalog>(ou, res); };
            Post["/"] = _ => { return this.module.Add(this.Bind<AppCatalog>()); };
            Put["/{id}"] = _ => { return this.module.Update(_.id.Value as string, this.Bind<AppCatalog>()); };
            Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };

        }

        private ModuleBase<AppCatalog> module { get; set; }
        private AppCatalogBLL bll { get; set; }
    }


}
