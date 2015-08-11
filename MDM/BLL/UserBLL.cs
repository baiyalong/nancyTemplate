using MDM.DAL;
using MDM.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class UserBLL : BLLBase<User>
    {
        public static readonly new UserBLL Instance = new UserBLL();
        private UserBLL()
        {
            this.dal = UserDAL.Instance;
        }
        private UserDAL dal { get; set; }
        public static User GetUserByUsername(string username)
        {
            User user = null;
            try
            {
                username = username.ToLower();
                if (false == UserDAL.Instance.GetByQuery(Query<User>.EQ(u => u.Name, username),  out user))
                {
                    user = null;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return user;
        }

        public static List<string>  GetUserGroupById(string id)
        {
            List<string> list = new List<string>();
            User user = null;
            try
            {
                if (false == UserDAL.Instance.GetByQuery(Query<User>.EQ(u => u.ID, id), out user))
                {
                    user = null;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            if ((user != null) && (user.UserGroups != null))
            {
                for (int i = 0; i < user.UserGroups.Count; i++)
                {
                    list.Add(user.UserGroups[i].ToString());
                }
            }

            return list;
        }

        internal bool DeleteUser(string id)
        {
            var res = false;
            try
            {
                User user = null;
                res = this.Get(id, null, out user);
                if (res && user != null)
                {
                    if (user.UserGroups != null && user.UserGroups.Count != 0)
                    {
                        res = UserGroupBLL.Instance.DeleteUser(user.UserGroups, id);
                    }
                    if (res && user.Terminals != null && user.Terminals.Count != 0)
                    {
                        foreach (var tid in user.Terminals)
                        {
                            res = TerminalBLL.Instance.Delete(tid.ToString());
                        }
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

        internal bool DeleteTerminal(string uid, string tid)
        {
            var res = false;
            try
            {
                User user = null;
                res = this.Get(uid, null, out user);
                if (res && user != null && user.Terminals.Contains(tid))
                {
                    if (res = user.Terminals.Remove(tid))
                    {
                        res = this.Update(uid, user);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return res;
        }

        public bool DeleteUserGroup(MongoDB.Bson.BsonArray uids, string gid)
        {
            var res = false;
            try
            {
                User user = null;
                foreach (var uid in uids)
                {
                    user = null;
                    res = this.Get(uid.ToString(), null, out user);
                    if (res && user != null)
                    {
                        if (res = user.UserGroups.Contains(gid))
                        {
                            if (res = user.UserGroups.Remove(gid))
                            {
                                res = this.Update(uid.ToString(), user);
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

        public bool DeleteUserGroup(string uid, string gid)
        {
            var res = false;
            try
            {
                User user = null;
                res = this.GetByQuery(Query<User>.EQ(p=>p.ID,uid), out user);
                if (res && user != null && user.UserGroups!=null)
                {
                    if (user.UserGroups.Contains(gid))
                    {
                        if (user.UserGroups.Remove(gid))
                        {
                             this.Update(uid, user);
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

        public bool AddUserGroup(string uid, string gid)
        {
            var res = false;
            try
            {
                User user = null;
                
                if((true== UserBLL.Instance.GetByQuery(Query<User>.EQ(e=>e.ID,uid),out user)) &&(user!=null))
                {
                    res = true;
                    if (user.UserGroups == null)
                    {
                        user.UserGroups = new BsonArray();
                    }
                    if (!user.UserGroups.Contains(gid))
                    {
                        user.UserGroups.Add(gid);

                        UserBLL.Instance.Update(Query<User>.EQ(e => e.ID, uid), user);
                      

                    }
                }
               
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }
            return res;
        }


        internal bool GetList(MongoDB.Bson.BsonArray uids, out IEnumerable<User> oul)
        {
            var res = false;
            try
            {
                List<User> li = new List<User>();
                foreach (var item in uids)
                {
                     User ou;
                        
                     if( true== this.dal.GetByQuery(Query<User>.EQ(p=>p.ID,item.ToString()),out ou))
                     {
                          if(ou!=null)
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

        //建立用户与应用模版的关联 
        public bool SetAppTemplate( string uid,string tid)
        {
            bool res = false;
            User user;
            if (true== UserBLL.Instance.GetByQuery(Query<User>.EQ(p=>p.ID,uid),out user))
            {
                if(user!=null)
                {
                    user.appTemplate = tid;

                    if (true == UserBLL.Instance.Update(Query<User>.EQ(p => p.ID, uid), user))
                    {
                        res = true;
                    }
                }
                else
                {
                    res = false;
                }
               
            }

            return res;
        }


    }
}