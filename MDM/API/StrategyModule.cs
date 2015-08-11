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


namespace MDM.API
{
    public class StrategyModule : NancyModule
    {
        public StrategyModule()
            : base("/api/strategy")
        {
            //this.RequiresAuthentication();

            this.module = ModuleBase<Strategy>.Instance;
            this.bll = StrategyBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<Strategy> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<Strategy>>(oul, res);
            };

            //返回策略总记录数
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<Strategy> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;

                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };

            Get["/{id}"] = _ => { Strategy ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<Strategy>(ou, res); };
            Post["/"] = _ => { return this.module.Add(this.Bind<Strategy>()); };
            Put["/{id}"] = _ => { return this.module.Update(_.id.Value as string, this.Bind<Strategy>()); };
            Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };

        }

        private ModuleBase<Strategy> module { get; set; }
        private StrategyBLL bll { get; set; }
    }
}