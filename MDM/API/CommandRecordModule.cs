using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDM.BLL;
using MDM.Models;

namespace MDM.API
{
    public class CommandRecordModule : NancyModule
    {
        public CommandRecordModule()
            : base("/api/commandRecord")
        {
            this.RequiresAuthentication();
            this.module = ModuleBase<CommandRecord>.Instance;
            this.bll = CommandRecordBLL.Instance;

            //获取全部的命令记录
            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<CommandRecord> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<CommandRecord>>(oul, res);
            };

            //获取命令记录数
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<CommandRecord> oul = null;
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
                
                CommandRecord ou = null; 

                var res = this.module.Get(_.id.Value as string, out ou);

                return Response.AsJson<CommandRecord>(ou, res);
            };


        }

        private ModuleBase<CommandRecord> module { get; set; }
        private CommandRecordBLL bll { get; set; }




    }
}