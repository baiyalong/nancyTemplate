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
using System.Linq;
using MongoDB.Bson;

namespace MDM.DAL
{
    public sealed class UserDAL : DALBase<User>
    {
        public static readonly new UserDAL Instance = new UserDAL();

        private UserDAL()
        {
            this.collection = MongoHelper.GetCollection<User>();
        }
        private MongoCollection<User> collection { get; set; }

        //internal bool GetList(MongoDB.Bson.BsonArray uids, out IEnumerable<User> oul)
        //{
        //    oul = this.collection.AsQueryable<User>().Where(u => uids.Contains((BsonValue)new BsonObjectId(new ObjectId(u.ID)))).ToList();
        //    return true;
        //}
    }
}
