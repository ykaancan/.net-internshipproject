using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    [BsonIgnoreExtraElements]
    public class LoginCredentials
    {
      
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }      
        public string AccessToken { get; set; }
    }
}
