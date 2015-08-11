using MDM.Helpers;
using MDM.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.DAL
{
    public class DALCorrelation<T, S>
        where T : ModelBase, new()
        where S : ModelBase, new()
    {

        public static readonly DALCorrelation<T, S> Instance = new DALCorrelation<T, S>();
        protected DALCorrelation()
        {
            this.TCollection = MongoHelper.GetCollection<T>();
            this.SCollection = MongoHelper.GetCollection<S>();
        }
        private MongoCollection<T> TCollection { get; set; }
        private MongoCollection<S> SCollection { get; set; }



        public bool Add(string tid, string sid, Func<T, IEnumerable<string>> tfn, Func<S, IEnumerable<string>> sfn)
        {
            var res = true;

            this.TCollection.Update(Query<T>.EQ(t => t.ID, tid), Update<T>.AddToSet<string>(x => tfn(x), sid));
            this.SCollection.Update(Query<S>.EQ(t => t.ID, tid), Update<T>.AddToSet<string>(x => tfn(x), sid));


            return res;
        }

        public bool Delete(string tid, string sid, Func<T, IEnumerable<string>> tfn, Func<S, IEnumerable<string>> sfn)
        {
            var res = true;

            this.TCollection.Update(Query<T>.EQ(t => t.ID, tid), Update<T>.AddToSet<string>(x => tfn(x), sid));
            this.SCollection.Update(Query<S>.EQ(t => t.ID, tid), Update<T>.AddToSet<string>(x => tfn(x), sid));


            return res;
        }

        public bool Get()
        {
            var res = false;



            return res;
        }
    }
}