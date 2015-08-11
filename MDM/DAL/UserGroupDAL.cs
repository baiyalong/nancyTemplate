using System;
using System.Web;
using System.ComponentModel;
using MDM.Models;
using MongoDB.Driver;
using MDM.Helpers;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using System.Linq;



namespace MDM.DAL
{
    public sealed class UserGroupDAL : DALBase<UserGroup>
    {
        public static readonly new UserGroupDAL Instance = new UserGroupDAL();

        private UserGroupDAL()
        {
            this.collection = MongoHelper.GetCollection<UserGroup>();
        }
        private MongoCollection<UserGroup> collection { get; set; }
        
    }
}
