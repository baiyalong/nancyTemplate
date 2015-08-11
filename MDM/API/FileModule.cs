using MDM.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;

namespace MDM.API
{
    public class FileModule : NancyModule
    {
        public FileModule()
            : base("/api/file")
        {
            // this.RequiresAuthentication();

            Post["/"] = _ =>
            {
                return Response.AsJson<IEnumerable<string>>(MongoHelper.AddFiles(this.Request.Files), HttpStatusCode.OK);
            };
            Delete["/{name}"] = _ =>
            {
                MongoHelper.DeleteFile(_.name.Value as string);
                return HttpStatusCode.OK;
            };
        }
    };
    public class FilesModule : NancyModule
    {
        public FilesModule()
            : base("/api/files")
        {
            // this.RequiresAuthentication();

            Post["/"] = _ =>
            {
                return Response.AsJson<IEnumerable<string>>(MongoHelper.addFiles(this.Request.Files), HttpStatusCode.OK);
            };
            Delete["/{id}"] = _ =>
            {
                MongoHelper.deleteFile(_.id.Value as string);
                return HttpStatusCode.OK;
            };
            Get["/{id}"] = _ => {
                return Response.AsJson<String>(MongoHelper.getFileInfo(_.id.Value as string));
            };
        }
    }
}


