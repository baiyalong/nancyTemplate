using MDM.BLL;
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

namespace MDM.API
{
    public class ModuleBase<T> : IModule<T>
         where T : ModelBase, new()
    {
        public static readonly ModuleBase<T> Instance = new ModuleBase<T>();
        protected ModuleBase()
        {
            this.bll = BLLBase<T>.Instance;
        }
        public BLLBase<T> bll { get; set; }

        public HttpStatusCode Add(T t)
        {
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.Add(t))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }

        public string AddTAndReturnID(T t)
        {
            string Id = string.Empty;
            Id= this.bll.AddTAndReturnID(t);

            if(Id!=null)
            {
                return Id;
            }
            else
            {
                return "";
            }
            
        }

        public HttpStatusCode AddList(IEnumerable<T> tl)
        {
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.AddList(tl))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }
        public HttpStatusCode Delete(string id)
        {
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.Delete(id))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }
        public HttpStatusCode DeleteList(IEnumerable<string> ids)
        {
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.DeleteList(ids))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }
        public HttpStatusCode Update(string id, T t)
        {
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.Update(id, t))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }
        public HttpStatusCode UpdateList(IEnumerable<T> tl)
        {
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.UpdateList(tl))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }
        
        public HttpStatusCode Get(string id, out T ot)
        {
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.GetByQueryS(Query<StrategyItem>.EQ(p => p.ID, id), out ot))
            {
                res = HttpStatusCode.OK;
            }
            return res;
        }
        public HttpStatusCode GetList(Pattern pattern, out IEnumerable<T> otl)
        {
            var res = HttpStatusCode.InternalServerError;
            if (this.bll.GetList(pattern, out otl))
            {
                foreach (var item in otl)
                {
                    item.View();
                }
                res = HttpStatusCode.OK;
            }
            return res;
        }
    }
}