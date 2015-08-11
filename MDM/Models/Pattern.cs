using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class Pattern
    {
        public int pageNum { get; set; }
        public int pageSize { get; set; }
        public string search { get; set; }

        public static bool verify(List<object> ls, string ss)
        {
            var res = false;
            if (ls == null || ls.Count == 0)
            {

            }
            else if (ss == null || ss == "")
            {
                res = true;
            }
            else
            {
                foreach (var item in ls)
                {
                    if (item != null && item.ToString().Contains(ss))
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }
    }
}