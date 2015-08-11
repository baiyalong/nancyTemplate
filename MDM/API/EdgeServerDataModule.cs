using MDM.BLL;
using MDM.Helpers;
using MDM.Models;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Modules
{
    public class EdgeServerDataModule : NancyModule
    {
        public EdgeServerDataModule()
        {
            Get["/"] = parameters =>
            {
                //EdgeServerBLL.Send2EdgeServer("EdgeServerInterface", "POST", "{'id':'124'}");
                return HttpStatusCode.OK;
            };
            
            Post["/EdgeServerData"] = _ =>
            {
                string datastring = RestHelper.GetBodyFromRequest(Request);
                LogHelper.WriteInfoLog(typeof(EdgeServerDataModule), "收到EdgeServer发送的消息：" + datastring);
                EdgeServerData data = new EdgeServerData();
                try
                {
                    data = JsonConvert.DeserializeObject<EdgeServerData>(datastring);
                }
                catch (Exception)
                {
                    return HttpStatusCode.BadRequest;
                }

                EdgeServerResponse resp = EdgeServerBLL.EdgeServerDilivery(data);
                return Response.AsJson(resp.result, resp.status);
            };

        }
    }
}