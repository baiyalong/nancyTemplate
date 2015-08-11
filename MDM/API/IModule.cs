using MDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using System.Web;

namespace MDM.API
{
    interface IModule<T>
        where T : IModel, new()
    {
        HttpStatusCode Add(T t);
        HttpStatusCode AddList(IEnumerable<T> tl);
        HttpStatusCode Delete(string id);
        HttpStatusCode DeleteList(IEnumerable<string> idl);
        HttpStatusCode Update(string id, T t);
        HttpStatusCode UpdateList(IEnumerable<T> tl);
        HttpStatusCode Get(string id, out T ot);
        HttpStatusCode GetList(Pattern pattern, out IEnumerable<T> otl);
    }
}