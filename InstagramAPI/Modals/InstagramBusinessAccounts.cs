using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    [BsonIgnoreExtraElements]
    public class Rootobject
    {
        public Instagram_Business_Account instagram_business_account { get; set; }
        public string id { get; set; }
    }

    public class Instagram_Business_Account
    {
        public string id { get; set; }
        public string Mail { get; set; }
    }

}
