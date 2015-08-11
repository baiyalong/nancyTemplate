using MDM.DAL;
using MDM.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class BLLBase<T> : IBLL<T>
        where T : ModelBase, new()
    {
        public static readonly BLLBase<T> Instance = new BLLBase<T>();
        protected BLLBase()
        {
            this.dal = DALBase<T>.Instance;
        }
        private DALBase<T> dal { get; set; }
        public bool Add(T t)
        {
            var res = false;
            try
            {
                res = dal.Add(t);
            }
            catch (Exception)
            {

            }
            return res;
        }

        public string AddTAndReturnID(T t)
        {
            string ID = string.Empty;

            try
            {
                ID = dal.AddAndReturnID(t);
            }
            catch(Exception ex)
            {
                

            }
            return ID;
        }
        public string AddAndReturnID(T t)
        {
            string id = string.Empty;
            try
            {
                id = dal.AddAndReturnID(t);
            }
            catch (Exception)
            {

            }
            return id;
        }

        public bool AddList(IEnumerable<T> tl)
        {
            var res = false;
            try
            {
                res = dal.AddList(tl);
            }
            catch (Exception)
            {

            }
            return res;
        }
        public bool Delete(string id)
        {
            var res = false;
            try
            {
                res = dal.Delete(id);
            }
            catch (Exception)
            {

            }
            return res;
        }
        public bool DeleteList(IEnumerable<string> idl)
        {
            var res = false;
            try
            {
                res = dal.DeleteList(idl);
            }
            catch (Exception)
            {

            }
            return res;
        }
        public bool Update(string id, T t)
        {
            var res = false;
            try
            {
                res = dal.Update(id, t);
            }
            catch (Exception)
            {

            }
            return res;
        }

        
        public bool Update(IMongoQuery query, T t)
        {
            var res = false;

            try
            {   
                res = dal.Update(query,t);
            }
            catch (Exception ex)
            {
                string msg= ex.Message;
            }
            return res;


        }


        public bool UpdateList(IEnumerable<T> tl)
        {
            var res = false;
            try
            {
                res = dal.UpdateList(tl);
            }
            catch (Exception)
            {

            }
            return res;
        }
        public bool Get(string id, IMongoQuery query, out T ot)
        {
            var res = false;
            try
            {
                res = dal.Get(id, query, out ot);
                ot.View();
            }
            catch (Exception)
            {
                ot = new T();
            }
            return res;
        }

        public bool GetByQuery(IMongoQuery query, out T ot)
        {
            var res = false;
            try
            {
                res = dal.GetByQuery(query, out ot);
                //ot.View();  //含义不明 
            }
            catch (Exception)
            {
                ot = new T();
            }
            return res;
        }
        public bool GetByQueryS(IMongoQuery query, out T ot)
        {
            var res = false;
            try
            {
                res = dal.GetByQuery(query, out ot);
                ot.View();  
            }
            catch (Exception)
            {
                ot = new T();
            }
            return res;
        }


        //获取应用列表
        public bool GetByQueryList(IMongoQuery query, out List<T> ot)
        {
            var res = false;
            try
            {
                res = dal.GetByQueryList(query, out ot);

            }
            catch (Exception)
            {
                ot = new List<T>();
            }
            return res;
        }

         public bool GetList(Pattern pattern, out IEnumerable<T> otl)
        {
            var res = false;
            try
            {
                if (pattern.pageNum <= 0) { pattern.pageNum = 1; }
                if (pattern.pageSize <= 0) { pattern.pageSize = 200; }

                Func<T, bool> fn = x => { return true; };
                if (pattern.search != null)
                {
                    fn = x => x.search(pattern.search);
                }

                res = this.dal.GetList(pattern, fn, out otl);

                foreach (var item in otl)
                {
                    item.View();
                }
            }
            catch (Exception ex)
            {
                otl = new List<T>();
                throw (ex);
            }

            return res;
        }

        //新增加多参数类型 
        public bool GetList(Pattern pattern, out IEnumerable<T> otl,out int totalCount,out int pageCount,out int pageNum)
        {

            var res = false;

            try
            {
                if (pattern.pageNum <= 0) { pattern.pageNum = 1; }
                if (pattern.pageSize <= 0) { pattern.pageSize = 100; }

                Func<T, bool> fn = x => { return true; };
                if (pattern.search != null)
                {
                    fn = x => x.search(pattern.search);
                }

                res = this.dal.GetList(pattern, fn, out otl,out totalCount ,out pageCount,out pageNum);
                   

                foreach (var item in otl)
                {
                    item.View();
                }
            }
            catch (Exception ex)
            {
                otl = new List<T>();
                throw (ex);
            }

            return res;
        }


        public bool CorrelationAdd()
        {
            var res = false;
            try
            {

            }
            catch (Exception)
            {

                throw;
            }

            return res;
        }

        public bool CorrelationDelete()
        {
            var res = false;
            try
            {

            }
            catch (Exception)
            {

                throw;
            }

            return res;
        }

        public bool CorrelationGet()
        {
            var res = false;
            try
            {

            }
            catch (Exception)
            {

                throw;
            }

            return res;
        }
    }
}
