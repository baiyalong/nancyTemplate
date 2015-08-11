using MDM.Helpers;
using MDM.Models;
using MongoDB.Driver;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Authentication.Token;
using Nancy.Security;
using Nancy.ModelBinding;
using MDM.BLL;
using MongoDB.Driver.Builders;



namespace MDM.Controller
{
    public class AuthModule : NancyModule
    {
        public MongoCollection<Admin> UserCollection = MongoHelper.GetCollection<Admin>();
        public AuthModule(ITokenizer tokenizer)
            : base("/api")
        {
            this.bll = BLLBase<Admin>.Instance;

            Post["/login"] = _ =>
            {
                var u = this.Bind<Admin>();

                var userIdentity = this.ValidateAdmin(u.UserName, u.Password);

                if (userIdentity == null)
                {
                    return HttpStatusCode.Unauthorized;
                }

                var token = tokenizer.Tokenize(userIdentity, Context);
                return Response.AsJson(new
                {
                    UserName = u.UserName,
                    Token = token
                });

            };
            Delete["/logout"] = _ =>
            {
                //delete token
                //??

                return 200;
            };

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Get["/Administrator/{userName}/{password}"] = _ =>
            {
                var userName = _.userName.Value as string;
                var password = _.password.Value as string;

                Admin oa = null;
                if (this.bll.Get(null, Query<Admin>.EQ(e => e.UserName, userName), out oa) && oa != null)
                {
                    return 500;
                }

                var res = this.bll.Add(new Admin()
                  {
                      UserName = userName,
                      Password = md5Hash.hash(password)
                  });

                return res ? 200 : 500;
            };
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


            //Get["/validation"] = _ =>
            //{
            //    this.RequiresAuthentication();
            //    return "Yay! You are authenticated!";
            //};

            //Get["/admin"] = _ =>
            //{
            //    this.RequiresClaims(new[] { "admin" });
            //    return "Yay! You are authorized!";
            //};
        }
        private BLLBase<Admin> bll { get; set; }

        private IUserIdentity ValidateAdmin(string userName, string password)
        {
            IUserIdentity identity = null;
            Admin oa = null;

            var query = Query<Admin>.EQ(e => e.UserName, userName);
            var res = this.bll.Get(null, query, out oa);

            if (res && oa != null && md5Hash.verify(password, oa.Password))
            {
                identity = new Admin() { UserName = userName, Password = password, Claims = new List<string>() { } };
            }

            return identity;
        }
    }
}