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


namespace MDM.API
{
    public class MessageModule : NancyModule
    {
        public MessageModule()
            : base("/api/message")
        {
            this.RequiresAuthentication();
            this.module = ModuleBase<MessageRecord>.Instance;
            this.bll = MessageBLL.Instance;


            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<MessageRecord> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<MessageRecord>>(oul, res);
            };

            //消息下发总记录数
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<MessageRecord> oul = null;
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

                MessageRecord ou = null;

                var res = this.module.Get(_.id.Value as string, out ou);

                return Response.AsJson<MessageRecord>(ou, res);
            };

            Post["/{title}"] = SendMsg;

        }

        private ModuleBase<MessageRecord> module { get; set; }

        private MessageBLL bll { get; set; }


        private dynamic SendMsg(dynamic arg)
        {
            var res = HttpStatusCode.InternalServerError;

            var message = this.Bind<Message>();
            var title = message.Title;
            var msg = message.Content;
            var tids = message.Terminals;
            string logionUser = this.Context.CurrentUser.UserName;
            #region 模拟数据
            //List<string> list = new List<string>() { "546c317e7acb021e6501eb19" };

            //List<string> list2 = new List<string>() { "546c31d47acb021e6501eb1a" };

            //IEnumerable<string> tids = list.Concat(list2); 
            #endregion

            if (tids.Count() == 0)
            {
                res = HttpStatusCode.OK;
                LogHelper.WriteErrorLog(typeof(MessageModule), "此消息终端id为空");
                return res;
            }

            if (this.bll.SendMessageToTerminals(msg, title, tids,logionUser))
            {
                res = HttpStatusCode.OK;
            }


            return res;


        }


    }
}