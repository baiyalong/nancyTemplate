using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDM.Models;

namespace MDM.DAL
{
    public class AppClassifyDAL:DALBase<AppClassify>
    {
        public static readonly new AppClassifyDAL Instance = new AppClassifyDAL();

        private AppClassifyDAL()
        {
        }
    }
}