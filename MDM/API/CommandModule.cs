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
    public class CommandModule : NancyModule
    {
        public CommandModule()
            : base("/api/command")
        {
            this.RequiresAuthentication();
            this.module = ModuleBase<Command>.Instance;
            this.bll = CommandBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<Command> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<Command>>(oul, res);
            };

            Get["/{id}"] = _ => { Command ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<Command>(ou, res); };
            Post["/"] = _ =>
            {
                return this.module.Add(this.Bind<Command>());
            };
            Put["/{id}"] = _ => { return this.module.Update(_.id.Value as string, this.Bind<Command>()); };
            Delete["/{id}"] = _ => { return this.module.Delete(_.id.Value as string); };

            Post["/{cid}"] = SendCommandToTerminals;
        }

        private ModuleBase<Command> module { get; set; }
        private CommandBLL bll { get; set; }

        private dynamic SendCommandToTerminals(dynamic arg)
        {

            var cid = arg.cid.Value as string;
            var res = HttpStatusCode.InternalServerError;

            var tids = this.Bind<IEnumerable<string>>();

            if (tids.Count() == 0)
            {
                res = HttpStatusCode.OK;
                LogHelper.WriteErrorLog(typeof(CommandModule), "此命令终端id为空" );
                return res;
            }
            string logionUser = this.Context.CurrentUser.UserName;

            #region 模拟数据
            //List<string> list = new List<string>() { "546c317e7acb02786501eb19" };

            //List<string> list2 = new List<string>() { };

            //IEnumerable<string> tids = list.Concat(list2); 
            #endregion

            if (this.bll.SendCommandToTerminals(cid, tids,logionUser))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }






    }

}
