using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class Message : ModelBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public IEnumerable<String> Terminals { get; set; }
        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.Title,
            this.Content
            }, ss);
        }
    }




}