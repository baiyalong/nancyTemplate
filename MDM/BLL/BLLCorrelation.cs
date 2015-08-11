using MDM.DAL;
using MDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class BLLCorrelation<T, S>
        where T : ModelBase, new()
        where S : ModelBase, new()
    {
        public static readonly BLLCorrelation<T, S> Instance = new BLLCorrelation<T, S>();
        protected BLLCorrelation()
        {
            this.dal = DALCorrelation<T, S>.Instance;
        }
        private DALCorrelation<T, S> dal { get; set; }



        public bool Add()
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

        public bool Delete()
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

        public bool Get()
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