using MDM.Helpers;
using MDM.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.DAL
{
    public class DALBase<T> : IDAL<T>
        where T : ModelBase, new()
    {
        public static readonly DALBase<T> Instance = new DALBase<T>();

        protected DALBase()
        {
            this.TCollection = MongoHelper.GetCollection<T>();
        }

        private MongoCollection<T> TCollection { get; set; }

        public bool Add(T t)
        {
            var res = TCollection.Insert<T>(t);
            return res.Ok;
        }

        public string AddAndReturnID(T t)
        {
            var res = TCollection.Insert<T>(t);

            if (res.Ok == true)
                return t.ID;
            else
                return string.Empty;
        }

        public bool AddList(IEnumerable<T> tl)
        {
            var res = TCollection.InsertBatch<T>(tl);
            var re = true;
            foreach (var item in res)
            {
                if (!item.Ok)
                {
                    re = false;
                    break;
                }
            }
            return re;
        }
        public bool Delete(string id)
        {
            var res = TCollection.Remove(Query<T>.EQ(q => q.ID, id));
            return res.Ok;
        }
        public bool DeleteList(IEnumerable<string> idl)
        {
            var res = true;
            foreach (var item in idl)
            {
                if (!this.Delete(item))
                {
                    res = false;
                }
            }
            return res;
        }
        public bool Update(string id, T t)
        {
            var res = TCollection.Update(Query<T>.EQ(q => q.ID, id), Update<T>.Replace(t));
            return res.Ok;
        }

        //新增更新条件信息
        public bool Update(IMongoQuery query, T t)
        {


            var res = TCollection.Update(query, Update<T>.Replace(t));

            //var res=TCollection.Update(query,t,UpdateFlags.Multi)

            return res.Ok;
        }


        public bool UpdateList(IEnumerable<T> tl)
        {
            var res = true;
            foreach (var item in tl)
            {
                if (!this.Update(item.ID, item))
                {
                    res = false;
                }
            }
            return res;
        }




        public bool Get(string id, IMongoQuery query, out T ot)
        {
            var res = true;
            if (id != null)
            {
                ot = TCollection.FindOne(Query<T>.EQ(q => q.ID, id));
            }
            else
            {
                ot = TCollection.FindOne(query);
            }
            if (ot == null)
            {
                ot = new T();
                res = false;
            }
            return res;
        }

        public bool GetByQuery(IMongoQuery query, out T ot)
        {
            var res = true;
            if (query != null)
            {
                ot = TCollection.FindOne(query);
            }
            else
            {
                ot = new T();
                res = false;
            }
            return res;
        }

        //获取应用列表 （暂无查询条件）
        public bool GetByQueryList(IMongoQuery query, out List<T> ot)
        {
            var res = true;

            var collections = TCollection.Find(query);

            ot = ConvertData(collections);

            //if (ot.Count == 0)
            //{
            //    res = false;
            //}

            return res;
        }

        //数据类型转换 
        private List<T> ConvertData(MongoCursor<T> cursor)
        {
            List<T> list = new List<T>();

            foreach (T collection in cursor)
            {
                if (collection != null)
                {
                    list.Add(collection);
                }
            }

            return list;
        }



        //public bool GetList(int pageNum, int pageSize, Func<T, bool> filter, out IEnumerable<T> otl)
        //{
        //    var res = true;
        //    otl = TCollection.AsQueryable<T>().Where(filter).Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
        //    return res;
        //}
        public bool GetList(Pattern pattern, Func<T, bool> fn, out IEnumerable<T> otl)
        {
            var res = true;

            otl = TCollection.AsQueryable<T>().Where(fn).Skip((pattern.pageNum - 1) * pattern.pageSize).Take(pattern.pageSize).ToList();




            return res;
        }


        // star 新增加    
        public bool GetList(Pattern pattern, Func<T, bool> fn, out IEnumerable<T> otl, out int TotalCount, out int pageCount, out int pageNum)
        {
            var res = true;
            //总条数
            TotalCount = Convert.ToInt32(TCollection.Count());
            //总页数 

            //pageCount = Convert.ToInt32( Math.Ceiling(350.00 / pattern.pageSize));

            pageCount = Convert.ToInt32(Math.Ceiling((decimal)TotalCount / pattern.pageSize));

            pageNum = pattern.pageNum;

            otl = TCollection.AsQueryable<T>().Where(fn).Skip((pattern.pageNum - 1) * pattern.pageSize).Take(pattern.pageSize).ToList();


            return res;
        }


    }
}