using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    public class url
    {
        [BsonId]     
        public string Url { get; set; }

    }
}
