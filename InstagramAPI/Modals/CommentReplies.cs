using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    [BsonIgnoreExtraElements]
    public class Datum2
    {
        public DateTime timestamp { get; set; }
        public string text { get; set; }
        public string id { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class Rootobject3
    {
        public List<Datum2> data { get; set; }
        public string Mail { get; set; }
        public string commentId { get; set; }
    }
}
