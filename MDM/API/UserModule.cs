using MDM.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDM.Helpers;
using MongoDB.Driver.Builders;
using Nancy.ModelBinding;
using MDM.BLL;
using Nancy.Security;


namespace MDM.API
{
    public class UserModule : NancyModule
    {
        public UserModule()
            : base("/api/user")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<User>.Instance;
            this.bll = UserBLL.Instance;


            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<User> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul,out totalCount, out pageCount,out pageNum))
                {
                    res = HttpStatusCode.OK;
                }
                 
                return Response.AsJson<IEnumerable<User>>(oul, res);
            };

            Get["/username"] = _ => {
                var res = HttpStatusCode.OK;
                return Response.AsJson<string>(this.Context.CurrentUser.UserName, res);
            };

            //获取列表的总记录数
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<User> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };


            Get["/{id}"] = _ => { User ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<User>(ou, res); };

            Post["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                var user = this.Bind<User>();
                User ou = null;
                bool reslut = this.bll.Get(null, Query<User>.EQ(e => e.Name, user.Name), out ou);
                if (ou != null && reslut==true)
                {
                    res = HttpStatusCode.OK;
                    string msg = "当前用户已存在！";
                    return Response.AsJson<string>(msg, res);
                }

                //return this.module.Add(user);
                return UserBLL.Instance.AddAndReturnID(user);
            };
            Put["/{id}"] = _ => { return this.module.Update(_.id.Value as string, this.Bind<User>()); };
            Delete["/{id}"] = DeleteUser;
            //设置用户应用模版
            Post["/{uid}/appTemplate/{tid}"] = SetAppTemplate;

            //Get["/{id}/userGroup"] = GetUsersGroup;

        }

        //private dynamic GetUsersGroup(dynamic arg)
        //{
        //    //var gid = arg.gid.Value as string;

        //    //var res = HttpStatusCode.InternalServerError;
        //    //IEnumerable<User> oul;
        //    //if (this.bll.GetUserFromGroup(gid, out oul))
        //    //{
        //    //    res = HttpStatusCode.OK;
        //    //}
        //    //return Response.AsJson<IEnumerable<User>>(oul, res);
        //}

        private ModuleBase<User> module { get; set; }
        private UserBLL bll { get; set; }

        private dynamic DeleteUser(dynamic arg)
        {
            var id = arg.id.Value as string;
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.DeleteUser(id))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic SetAppTemplate(dynamic arg)
        {

            string uid = arg.uid.Value as string;
            string tid = arg.tid.Value as string;

            var  res = HttpStatusCode.InternalServerError;

            if (this.bll.SetAppTemplate(uid,tid))
            {
                res = HttpStatusCode.OK;
            }

            return res;

        }

       

    }
}