using MDM.Helpers;
using MDM.Models;
using MongoDB.Driver;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.ModelBinding;
using MDM.BLL;
using Nancy.Security;


namespace MDM.API
{
    public class TerminalModule : NancyModule
    {
        public TerminalModule()
            : base("/api/terminal")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<Terminal>.Instance;
            this.bll = TerminalBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<Terminal> oul = null;

                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<IEnumerable<Terminal>>(oul, res);
            };

            //客户端总记录数
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<Terminal> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };

            Get["/{id}"] = _ => { Terminal ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<Terminal>(ou, res); };
            //Post["/"] = _ => { return this.module.Add(this.Bind<Terminal>()); };
            //Put["/{id}"] = _ => { return this.module.Update(_.id.Value as string, this.Bind<Terminal>()); };
            Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };
            //获取当前某一终端下安装的应用信息
            Get["/{terminalid}/terminalapp"] = GetAllTerminalApps;

            Get["/{tid}/userterminal"] = GetTerminalFromUser;
        }

        private ModuleBase<Terminal> module { get; set; }
        private TerminalBLL bll { get; set; }
        private dynamic DeleteTerminal(dynamic arg)
        {
            var id = arg.id.Value as string;
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.DeleteTerminal(id))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic GetAllTerminalApps(dynamic arg)
        {
            string terminalid = arg.terminalid.Value as string;

            var res = HttpStatusCode.InternalServerError;
            List<AppInstallMsg> apps;

            if (true == this.bll.GetAllTerminalApp(terminalid, out apps))
            {
                res = HttpStatusCode.OK;
            }
            return Response.AsJson<List<AppInstallMsg>>(apps, res);
        }

        private dynamic GetTerminalFromUser(dynamic arg)
        {
            var uid = arg.tid.Value as string;
            var res = HttpStatusCode.InternalServerError;
            List<Terminal> terminals;
            if (this.bll.GetTerminalFromUser(uid, out terminals))
            {
                res = HttpStatusCode.OK;
            }
            return Response.AsJson<List<Terminal>>(terminals, res);
        }
    }
}