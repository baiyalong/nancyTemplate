using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class Punishment:ModelBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public override void View()
        {

        }
        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.Code,
            this.Name,
            }, ss);
        }
    }
}