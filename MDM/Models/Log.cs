using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDM.Models
{
    class Log
    {

        [BsonId]
        public ObjectId ID { get; set; }

        public String Header { get; set; }
        public String Body { get; set; }

    }
}
