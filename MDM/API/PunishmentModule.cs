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

namespace MDM.API
{
    public class PunishmentModule : NancyModule
    {

        public PunishmentModule()
            : base("/api/punishment")
        {
            this.RequiresAuthentication();
            this.module = ModuleBase<Punishment>.Instance;
            this.bll = PunishmentBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<Punishment> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<Punishment>>(oul, res);
            };

            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<Punishment> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;

                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };

            Get["/{id}"] = _ => { Punishment ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<Punishment>(ou, res); };
            Post["/"] = _ => { return this.module.Add(this.Bind<Punishment>()); };
            Put["/{id}"] = _ => { return this.module.Update(_.id.Value as string, this.Bind<Punishment>()); };
            Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };

        }

        private ModuleBase<Punishment> module { get; set; }
        private PunishmentBLL bll { get; set; }

    }
}