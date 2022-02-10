using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    [BsonIgnoreExtraElements]
    public class Rootobject2
    {
        public Datum1[] data { get; set; }
        public string igMediaId { get; set; }
        public string Mail { get; set; }
    }

    public class Datum1
    {
        public DateTime timestamp { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        
    }

}
