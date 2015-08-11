using MDM.DAL;
using MDM.Models;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class UserGroupBLL : BLLBase<UserGroup>
    {
        public static readonly new UserGroupBLL Instance = new UserGroupBLL();
        private UserGroupBLL()
        {
            this.dal = UserGroupDAL.Instance;
        }
        private UserGroupDAL dal { get; set; }

        public bool GetGroupFromUser(string uid, out List<UserGroup> oul)
        {
            var res = false;
            try
            {

                User user;
                oul = new List<UserGroup>();
                res = UserBLL.Instance.GetByQuery(Query<User>.EQ(p => p.ID, uid), out user);

                if (user.UserGroups == null)
                {
                    return res;
                }
                else if ( user.UserGroups.Count != 0)
                {
                    res = UserGroupBLL.Instance.GetList(user.UserGroups, out oul);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return res;
        }

        public bool GetUserFromGroup(string gid, out IEnumerable<User> oul)
        {
            var res = false;
            try
            {
                UserGroup userGroup;
                oul = new List<User>();

                res = this.GetByQuery(Query<UserGroup>.EQ(p => p.ID, gid), out userGroup);

                if (res && userGroup != null && userGroup.Users != null && userGroup.Users.Count != 0)
                {

                    res = UserBLL.Instance.GetList(userGroup.Users, out oul);

                }

            }
            catch (Exception)
            {

                throw;
            }


            return res;
        }

        public bool AddUserToGroup(string uid, string gid)
        {
            var res = false;
            try
            {
                UserGroup userGroup;

                if ((true==this.GetByQuery(Query<UserGroup>.EQ(p => p.ID, gid), out userGroup)) && (userGroup != null))
                {
                    res = true;
                    res = UserBLL.Instance.AddUserGroup(uid, gid);
                    if (userGroup.Users == null)
                    {
                        userGroup.Users = new MongoDB.Bson.BsonArray();
                    }
                    if ( !userGroup.Users.Contains(uid))
                    {
                        userGroup.Users.Add(uid);

                        UserGroupBLL.Instance.Update(Query<UserGroup>.EQ(e => e.ID, gid), userGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }

            return res;
        }

        public bool DeleteUserFromGroup(string uid, string gid)
        {
            var res = false;
            try
            {
                UserGroup userGroup = null;
                res = this.GetByQuery(Query<UserGroup>.EQ(p => p.ID, gid), out userGroup);
                if (userGroup != null)
                {
                    res = UserBLL.Instance.DeleteUserGroup(uid, gid);
                    if (userGroup.Users.Contains(uid))
                    {
                        if (userGroup.Users.Remove(uid))
                        {
                             this.Update(gid, userGroup);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return res;
        }

        internal bool DeleteUserGroup(string id)
        {
            var res = false;
            try
            {
                UserGroup userGroup = null;
                res = this.Get(id, null, out userGroup);
                if (res && userGroup != null)
                {
                    if (userGroup.Users != null && userGroup.Users.Count != 0)
                    {
                        res = UserBLL.Instance.DeleteUserGroup(userGroup.Users, id);
                    }
                    if (res)
                    {
                        res = this.Delete(id);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return res;
        }



        internal bool DeleteUser(MongoDB.Bson.BsonArray gids, string uid)
        {
            var res = false;
            try
            {
                UserGroup userGroup = null;
                foreach (var gid in gids)
                {
                    userGroup = null;
                    res = this.Get(gid.ToString(), null, out userGroup);
                    if (res && userGroup != null)
                    {
                        if (res = userGroup.Users.Contains(uid))
                        {
                            if (res = userGroup.Users.Remove(uid))
                            {
                                res = this.Update(gid.ToString(), userGroup);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return res;
        }


        internal bool GetList(MongoDB.Bson.BsonArray uids, out List<UserGroup> oul)
        {
            var res = false;
            try
            {
                List<UserGroup> li = new List<UserGroup>();
                foreach (var item in uids)
                {
                    UserGroup ou;

                    if (true == this.dal.GetByQuery(Query<UserGroup>.EQ(p => p.ID, item.ToString()), out ou))
                    {
                        if (ou != null)
                        {
                            ou.View();
                            li.Add(ou);
                        }

                    }

                }
                oul = li;
                res = true;

            }
            catch (Exception ex)
            {

                throw (ex);
            }

            return res;
        }
    }
}