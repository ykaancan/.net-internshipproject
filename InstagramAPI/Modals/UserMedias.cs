using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    [BsonIgnoreExtraElements]
    public class Rootobject1
    {
        public Datum[] data { get; set; }
        public Paging paging { get; set; }
        public string Mail { get; set; }
        
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
    }

    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
    }

}
