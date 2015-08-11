using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class Command : ModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.Code,
            this.Description
            }, ss);
        }
    }

   

  
}