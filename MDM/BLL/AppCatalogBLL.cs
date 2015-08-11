using MDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class AppCatalogBLL : BLLBase<AppCatalog>
    {
        public static readonly new AppCatalogBLL Instance = new AppCatalogBLL();
        private AppCatalogBLL()
        {
        }
    }
}