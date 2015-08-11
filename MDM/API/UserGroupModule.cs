using MDM.Helpers;
using MDM.Models;
using MongoDB.Driver;
using Nancy;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.ModelBinding;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using Nancy.Security;
using MDM.BLL;


namespace MDM.API
{
    public class UserGroupModule : NancyModule
    {
        public UserGroupModule()
            : base("/api/userGroup")
        {
            this.RequiresAuthentication();

            this.module = ModuleBase<UserGroup>.Instance;
            this.bll = UserGroupBLL.Instance;

            Get["/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;
                IEnumerable<UserGroup> oul = null;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul))
                {
                    res = HttpStatusCode.OK;
                }
                return Response.AsJson<IEnumerable<UserGroup>>(oul, res);
            };

            //用户组总记录数
            Get["/pageCount/"] = _ =>
            {
                var res = HttpStatusCode.InternalServerError;

                IEnumerable<UserGroup> oul = null;
                int totalCount;
                int pageCount;
                int pageNum;
                if (this.bll.GetList(this.Bind<Pattern>(), out oul, out totalCount, out pageCount, out pageNum))
                {
                    res = HttpStatusCode.OK;
                }

                return Response.AsJson<int>(totalCount, res);
            };

            Get["/{id}"] = _ => { UserGroup ou = null; var res = this.module.Get(_.id.Value as string, out ou); return Response.AsJson<UserGroup>(ou, res); };
            Post["/"] = PostUserGroup;
            Put["/{id}"] = PutUserGroup;
            Delete["/{id}"] = DeleteUserGroup;

            Get["/{gid}/user/"] = GetUserFromGroup;
            Post["/{gid}/user/{uid}"] = AddUserToGroup;
            Delete["/{gid}/user/{uid}"] = DeleteUserFromGroup;

            Get["{uid}/usergroup"] = GetUserGroupFromUser;

        }

        private dynamic PostUserGroup(dynamic arg)
        {
            var res = HttpStatusCode.InternalServerError;
            var userGroup = this.Bind<UserGroup>();
            UserGroup group = null;
            bool reslut = this.bll.Get(null, Query<UserGroup>.EQ(e => e.Name, userGroup.Name), out group);
            if(group!=null && reslut==true)
            {
                res = HttpStatusCode.OK;
                string msg = "当前用户组名称已存在！";
                return Response.AsJson<string>(msg, res);
            }
            
            string groupid=this.module.AddTAndReturnID(userGroup);
            res = HttpStatusCode.OK;
            return Response.AsJson<string>(groupid, res);
        }

        private dynamic PutUserGroup(dynamic arg)
        {
            var id = arg.id.Value as string;
            var userGroup = this.Bind<UserGroup>();
           
            return this.module.Update(id, userGroup);
        }


        private ModuleBase<UserGroup> module { get; set; }
        private UserGroupBLL bll { get; set; }
        private dynamic DeleteUserGroup(dynamic arg)
        {
            var id = arg.id.Value as string;

            var res = HttpStatusCode.InternalServerError;
            if (this.bll.DeleteUserGroup(id))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }
        private dynamic DeleteUserFromGroup(dynamic arg)
        {
            var uid = arg.uid.Value as string;
            var gid = arg.gid.Value as string;

            var res = HttpStatusCode.InternalServerError;
            if (this.bll.DeleteUserFromGroup(uid, gid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        private dynamic AddUserToGroup(dynamic arg)
        {
            var uid = arg.uid.Value as string;
            var gid = arg.gid.Value as string;

            var res = HttpStatusCode.InternalServerError;
            if (this.bll.AddUserToGroup(uid, gid))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }


        private dynamic GetUserFromGroup(dynamic arg)
        {
            var gid = arg.gid.Value as string;

            var res = HttpStatusCode.InternalServerError;
            IEnumerable<User> oul;
            if (this.bll.GetUserFromGroup(gid, out oul))
            {
                res = HttpStatusCode.OK;
            }
            return Response.AsJson<IEnumerable<User>>(oul, res);
        }


        private dynamic GetUserGroupFromUser(dynamic arg)
        {
            var uid = arg.uid.Value as string;
            var res = HttpStatusCode.InternalServerError;
            List<UserGroup> groups;
            if (this.bll.GetGroupFromUser(uid, out groups))
            {
                res = HttpStatusCode.OK;
            }
            return Response.AsJson<List<UserGroup>>(groups, res);
        }

    }
}