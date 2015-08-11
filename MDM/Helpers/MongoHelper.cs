using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using System.Configuration;
using System.IO;
using Nancy;

namespace MDM.Helpers
{
    public static class MongoHelper
    {
        static readonly MongoClient client;
        static readonly MongoServer server;
        static readonly MongoDatabase db;
        static readonly MongoGridFS gridFs;
        static MongoHelper()
        {
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["MongoDBConnection"].ConnectionString);
            client = new MongoClient(url);
            server = client.GetServer();
            db = server.GetDatabase(url.DatabaseName);
            gridFs = db.GridFS;
        }
        public static MongoCollection<T> GetCollection<T>(string name = null)
        {
            return db.GetCollection<T>(String.IsNullOrEmpty(name) ? typeof(T).Name : name);
        }
        public static string AddFile(HttpFile file)
        {
            var tick = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var fileName = file.Name.Replace(".", tick + ".");
            return gridFs.Upload(file.Value, fileName).Name.ToString();
        }
        public static IEnumerable<string> AddFiles(IEnumerable<HttpFile> files)
        {
            List<string> names = new List<string>();
            foreach (var file in files)
            {
                names.Add(MongoHelper.AddFile(file));
            }
            return names;
        }
        public static void DeleteFile(string name)
        {
            gridFs.Delete(name);
        }
        public static void DeleteFiles(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                MongoHelper.DeleteFile(name);
            }
        }
        //public static Stream GetFile(string id)
        //{
        //    var file = gridFs.FindOneById(ObjectId.Parse(id));
        //    return file.OpenRead();
        //}
        //public static Stream GetFiles(BsonArray ids)
        //{
        //    gridFs.
        //}
        public static string addFile(HttpFile file)
        {
            return gridFs.Upload(file.Value, file.Key).Id.ToString();
        }
        public static IEnumerable<string> addFiles(IEnumerable<HttpFile> files)
        {
            List<string> ids = new List<string>();
            foreach (var file in files)
            {
                ids.Add(MongoHelper.addFile(file));
            }
            return ids;
        }
        public static void deleteFile(string id)
        {
            gridFs.DeleteById(new BsonObjectId(ObjectId.Parse(id)));
        }
        public static void deleteFiles(IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                MongoHelper.deleteFile(id);
            }
        }
        public static String getFileInfo(string id)
        {
            var oid = new BsonObjectId(ObjectId.Parse(id));
            var info = gridFs.FindOneById(oid);

            return info.Name;
        }
        public static IEnumerable<String> getFileInfos(IEnumerable<string> ids)
        {
            List<String> infos = new List<String>();
            foreach (var item in ids)
            {
                infos.Add(MongoHelper.getFileInfo(item));
            }
            return infos;
        }
    }
}