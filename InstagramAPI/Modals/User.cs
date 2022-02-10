using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    public class User
    {

      
        
        public string userID { get; set; }
        [JsonProperty("access_token")]
        public string accessToken { get; set; }
       
        public string data_access_expiration_time { get; set; }
        [JsonProperty("id")]       
        public string Id { get; set; }
        public string Mail { get; set; }
       

       


    }
}
