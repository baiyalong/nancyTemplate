using MDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.DAL
{
    public sealed class AppDAL : DALBase<App>
    {
        public static readonly new AppDAL Instance = new AppDAL();

        private AppDAL()
        {
        }

    }
}