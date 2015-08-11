using MDM.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.DAL
{
    interface IDAL<T>
        where T : IModel, new()
    {
        bool Add(T t);
        bool AddList(IEnumerable<T> tl);
        bool Delete(string id);
        bool DeleteList(IEnumerable<string> idl);
        bool Update(string id, T t);
        bool UpdateList(IEnumerable<T> tl);
        bool Get(string id, IMongoQuery query, out T ot);
        bool GetList(Pattern pattern, Func<T, bool> fn, out IEnumerable<T> otl);
    }
}
