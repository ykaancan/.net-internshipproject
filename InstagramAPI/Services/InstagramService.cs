using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using InstagramAPI.Modals;
using MongoDB.Driver;
using Constant = InstagramAPI.Modals.Constant;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MongoDB.Bson.IO;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace InstagramAPI.Services
{
    public class InstagramService : IInstagramService
    {
        private readonly HttpClient _httpClient;      
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<LoginCredentials> _loginCredentials;
        private readonly IMongoCollection<Rootobject> _rootObject;
        private readonly IMongoCollection<UserPosts> _userPosts;
        private readonly IMongoCollection<Containers> _containers;
        private readonly IMongoCollection<Rootobject1> _rootObject1;
        private readonly IMongoCollection<Rootobject2> _rootObject2;
        private readonly IMongoCollection<Rootobject3> _rootobject3;
        private readonly IMongoCollection<Root> _root;
        


        public static Cloudinary cloudinary;

        public InstagramService(IInstagramDatabaseSettings settings)
        {
            _httpClient = new HttpClient();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.UsersCollectionName);
            _loginCredentials = database.GetCollection<LoginCredentials>(settings.LoginCredentialsCollectionName);
            _rootObject = database.GetCollection<Rootobject>(settings.InstagramBusinessAccountsCollectionName);
            _userPosts = database.GetCollection<UserPosts>(settings.UserPostsCollectionName);
            _containers = database.GetCollection<Containers>(settings.ContainersCollectionName);
            _rootObject1 = database.GetCollection<Rootobject1>(settings.UserMediasCollectionName);
            _rootObject2 = database.GetCollection<Rootobject2>(settings.CommentsCollectionName);
            _rootobject3 = database.GetCollection<Rootobject3>(settings.CommentRepliesCollectionName);
            _root = database.GetCollection<Root>(settings.HashtagsCollectionName);
            //  _businessAccounts = database.GetCollection<BusinessAccount>(settings.BusinessAccountCollectionName);
            Account account = new Account(Constant.CLOUD_NAME, Constant.API_KEY, Constant.API_SECRET);
            cloudinary = new Cloudinary(account);
            
        }
        public string GetInsights(string name, string currentMail) // businessAccountlar için a stringinde tutulan bilgileri alıyor.
        {
            User user = new User();
            string z = "";
            var query = (from e in _users.AsQueryable<User>()
                         where e.Mail == currentMail
                         select e);
            foreach (User i in query)
            {
                user.accessToken = i.accessToken;
            }
            var query1 = (from e in _rootObject.AsQueryable<Rootobject>()
                          select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach (Rootobject i in query1)
            {
                z = i.instagram_business_account.id;
            }
            string a = "{followers_count,media_count,media{comments_count,like_count}}";
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"{z}?fields=business_discovery.username({name}){a}&access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result;

        }

        public string getIgId(string currentMail) // login yapıp facebookLogin yaptiktan sonra instagramID mutlaka alınmalı.
        {
            User user = new User();
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);

            foreach (User i in query)
            {
                user.Id = i.Id;
                user.accessToken = i.accessToken;
            }
            
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"{user.Id}?fields=instagram_business_account&access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;
            Rootobject igIds = JsonConvert.DeserializeObject<Rootobject>(result);
            igIds.instagram_business_account.Mail = currentMail;
            var query5 = (from e in _rootObject.AsQueryable<Rootobject>()
                          select e).Any(e => e.id == igIds.id);
            if(query5 == true)
            { 
                _rootObject.ReplaceOne(a => a.id == igIds.id, igIds);
            }
            else
            {
                _rootObject.InsertOne(igIds);
            }
            
            return result;
        }
        public void GetPagesOfAccessTokensOwner(string accessToken, string userID, string data_access_expiration_time, string currentMail) // AccessToken almak için FacebookLogin calistirildiginda buraya yönlendiriyor.
        {
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"me/accounts?access_token={accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;           
            JObject a = JObject.Parse(result);
            IList<JToken> results = a["data"].Children().ToList();
            IList<User> searchResults = new List<User>();
            foreach (JToken resultz in results)
            {
                User user = resultz.ToObject<User>();
                searchResults.Add(user);
                user.data_access_expiration_time = data_access_expiration_time;
                user.userID = userID;
                user.Mail = currentMail;
                LoginCredentials loginCredentials = new LoginCredentials();
                var query = (from e in _loginCredentials.AsQueryable<LoginCredentials>()
                             select e).Any(e => e.Mail == currentMail);
                if (query == true)
                {
                    user.Mail = currentMail;                                                                                      
                }
                var query1 = (from b in _users.AsQueryable<User>()
                             select b).Any(b => b.Mail == currentMail);
                if(query1 == true)
                {
                    _users.ReplaceOne(a => a.Mail == currentMail, user);
                }
                else
                {
                    _users.InsertOne(user);
                }
            }
        }
  
        public string GetComments(string currentMail,string igMediaId)  // girilen media idsine ait commentleri aliyor.
        {
            User user = new User();
            var query7 = (from e in _users.AsQueryable<User>()
                          select e).Where(e => e.Mail == currentMail);
            foreach (User i in query7)
            {
                user.accessToken = i.accessToken;
            }
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"/{igMediaId}/comments?access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;
            Rootobject2 rootobject2 = JsonConvert.DeserializeObject<Rootobject2>(result);
            rootobject2.igMediaId = igMediaId;
            rootobject2.Mail = currentMail;
            _rootObject2.InsertOne(rootobject2);
            return result;         
        }
        public string GetAllComments(string currentMail)  // giriş yapan kullanıya ait bütün yorumları alıyor.
        {
            User user = new User();
            var query10 = (from e in _users.AsQueryable<User>()
                           select e).Where(e => e.Mail == currentMail);
            foreach(User i in query10)
            {
                user.accessToken = i.accessToken;
            }
            Rootobject1 rootobject1 = new Rootobject1();
            Rootobject2 rootobject2 = new Rootobject2();
            var query = (from e in _rootObject1.AsQueryable<Rootobject1>()
                           select e).Where(e => e.Mail == currentMail);
            foreach(Rootobject1 a in query)
            {
                _httpClient.BaseAddress = new Uri(Constant.baseAddress);
                for (int i = 0; i < a.data.Length; i++) { 
               rootobject2.igMediaId = a.data[i].id ;
              //  string b = rootobject2.igMediaId;               
                HttpResponseMessage response = _httpClient.GetAsync($"{rootobject2.igMediaId}/comments?access_token={user.accessToken}").Result;
                string result = response.Content.ReadAsStringAsync().Result;
                Rootobject2 rootobject22 = JsonConvert.DeserializeObject<Rootobject2>(result);
                rootobject22.igMediaId = rootobject2.igMediaId;
                rootobject22.Mail = currentMail;
                    var query3 = (from e in _rootObject2.AsQueryable<Rootobject2>()
                                  select e).Any(e => e.igMediaId == rootobject22.igMediaId);
                    if(query3 == true)
                    {
                        _rootObject2.ReplaceOne(e => e.igMediaId == rootobject22.igMediaId, rootobject22);
                    }
                    else
                    {
                        _rootObject2.InsertOne(rootobject22);
                    }
                
                }               
            }
            return "All comments are obtained.";
        }
        public string GetAccountMetrics(string currentMail)  // istekte girilen  metric kısmında hesapla ilgili bilgileri alıyor
        {
            User user = new User();
            Rootobject rootobject = new Rootobject();
            string b = "";
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach (User i in query)
            {
                user.accessToken = i.accessToken;
            }
            var query1 = (from e in _rootObject.AsQueryable<Rootobject>()
                          select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach (Rootobject i in query1)
            {
                string a = i.instagram_business_account.id;
                b = a;
            }
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"{b}/insights?metric=impressions,reach,profile_views&period=day&access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
        public string HashtagSearch(string hashtag,string currentMail)  // giriş yapan kullanıcının accessToken'ı ile girilen hashtagle ilgili postları alıyor.
        {
            User user = new User();
            string hashtagIdWillBeUsed="";
            string IdWillBeUsed="";
            var query1 = (from e in _users.AsQueryable<User>()
                          select e).Where(e => e.Mail == currentMail);
            foreach(User i in query1)
            {
                user.accessToken = i.accessToken;
            }
            Rootobject rootobject = new Rootobject();
            
            var query = (from e in _rootObject.AsQueryable<Rootobject>()
                         select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach(Rootobject k in query)
            {              
                IdWillBeUsed= k.instagram_business_account.id;
            }
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            HttpResponseMessage response = _httpClient.GetAsync($"ig_hashtag_search?user_id={IdWillBeUsed}&q={hashtag}&access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;
            Root root = JsonConvert.DeserializeObject<Root>(result);
            root.Hashtag = hashtag;
            root.Mail = currentMail;           
            hashtagIdWillBeUsed = root.data[0].id;
            _root.InsertOne(root);
            HttpResponseMessage response2 = _httpClient.GetAsync($"{hashtagIdWillBeUsed}/recent_media?access_token={user.accessToken}&user_id={IdWillBeUsed}&fields=media_type,caption,comments_count,like_count,media_url").Result;
            string result2 = response2.Content.ReadAsStringAsync().Result;
            return result2;
        }
        public string HideorUnhideComment(string currentMail, string comment,string igMediaId,string state) // HideUnhideComment methodu daha efektif olduğu için bunu kullanmıyorum.
        {
            string idWhichWillBeUsed = "";
            User user = new User();
            Rootobject2 rootobject2 = new Rootobject2();
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach (User i in query)
            {
                user.accessToken = i.accessToken;
            }
            var query1 = (from e in _rootObject2.AsQueryable<Rootobject2>()
                          select e).Where(e => e.igMediaId == igMediaId);
            foreach(Rootobject2 b in query1)
            {
                
                for (int k = 0; k < b.data.Length; k++)
                {
                    if(b.data[k].text == comment)
                    {
                        idWhichWillBeUsed = b.data[k].id ;
                    }                   
                }             
            }         

            string postUri = $"https://graph.facebook.com/{idWhichWillBeUsed}?hide={state}&access_token={user.accessToken}";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>($"idWhichWillBeUsed",idWhichWillBeUsed),               
            });        
            HttpResponseMessage response = _httpClient.PostAsync(postUri, content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result;

        }
        public string HideUnhideComment(string commentId,string hide,string currentMail) // hide kısmını true veya false yaparak bu commentId'ye ait commenti gizleyip görünür yapabiliyorsunuz.
        {
            User user = new User();
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach(User i in query)
            {
                user.accessToken = i.accessToken;
            }
            string postUri = $"https://graph.facebook.com/{commentId}?hide={hide}&access_token={user.accessToken}";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>($"commentId",commentId),
            });
            HttpResponseMessage response = _httpClient.PostAsync(postUri, content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
    
        public string HideorUnhideComment2(string currentMail, string comment, string state) // HideUnhideMethodu kullanılıyor.
        {
            User user = new User();
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach (User i in query)
            {
                user.accessToken = i.accessToken;
            }
            Rootobject1 rootobject1 = new Rootobject1();
            var query2 = (from e in _rootObject1.AsQueryable<Rootobject1>()
                          select e).Where(e => e.Mail == currentMail);
            int length = 0;
            string igMediaId;
            string idWhichWillBeUsed;
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            foreach (Rootobject1 i in query2)
            {
                length = i.data.Length;
                for (int ö = 0; ö < length; ö++)
                {
                    igMediaId = i.data[ö].id;
                    var query5 = (from e in _rootObject2.AsQueryable<Rootobject2>()
                                  select e).Where(e => e.igMediaId == igMediaId);
                    foreach (Rootobject2 b in query5)
                    {
                        for (int l = 0; l < b.data.Length; l++)
                        {
                            if(b.data[l].text == comment)
                            {
                                idWhichWillBeUsed = b.data[l].id;
                                string postUri = $"https://graph.facebook.com/{idWhichWillBeUsed}?hide={state}&access_token={user.accessToken}";
                                var content = new FormUrlEncodedContent(new[]
                                {
                new KeyValuePair<string,string>($"idWhichWillBeUsed",idWhichWillBeUsed),
            });
                                HttpResponseMessage response = _httpClient.PostAsync(postUri, content).Result;
                                string result = response.Content.ReadAsStringAsync().Result;
                                return result;

                            }
                        }
                    }
                }
               
            }
            return "Comment hidden.";
        }
        public string GetAccountImpressions(string currentMail)  // bu kullanıcıya ait hesapla ilgili bilgileri dönüyor.
        {
            User user = new User();
            Rootobject rootobject = new Rootobject();
            string c = "";
            var query3 = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach(User i in query3)
            {
                user.accessToken = i.accessToken;
            }
            var query4 = (from e in _rootObject.AsQueryable<Rootobject>()
                          select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach(Rootobject i in query4)
            {
                string d = i.instagram_business_account.id;
                c = d;
            }
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"{c}/insights?metric=impressions,reach,profile_views&period=day&access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
    

        public string GetUserMedia(string currentMail) // giriş yapan kullanıcının bütün media öğelerini aliyor.
        {
            User user = new User();
            Rootobject rootobject = new Rootobject();
            string b = "";
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach (User i in query)
            {
                user.accessToken = i.accessToken;
            }
            var query1 = (from e in _rootObject.AsQueryable<Rootobject>()
                          select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach (Rootobject i in query1)
            {
                string a = i.instagram_business_account.id;
                b = a;
            }
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            HttpResponseMessage response = _httpClient.GetAsync($"/{b}/media?access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;
            Rootobject1 userMedia = JsonConvert.DeserializeObject<Rootobject1>(result);
            userMedia.Mail = currentMail;
            var query2 = (from e in _rootObject1.AsQueryable<Rootobject1>()
                          select e).Any(e => e.Mail == currentMail);
            if(query2 == true)
            {
                _rootObject1.ReplaceOne(e => e.Mail == currentMail, userMedia);
            }
            else
            {
                _rootObject1.InsertOne(userMedia);
            }                          
            return result;
        }
        public string PostAReplyToAComment(string currentMail,string igCommentId,string message) // commentId ile bir comment'e yanıt yapılabiliyor.
        {
            User user = new User();
            var query8 = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach (User i in query8)
            {
                user.accessToken = i.accessToken;
            }
            string postUri = $"https://graph.facebook.com/{igCommentId}/replies?message={message}&access_token={user.accessToken}";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>($"message",message),
            });

            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            HttpResponseMessage response = _httpClient.PostAsync(postUri,content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
            
        }
        public string DeleteComment(string currentMail,string igCommentId) // bu commentId'ye ait commenti siliyor
        {
            User user = new User();
            var query9 = (from e in _users.AsQueryable<User>()
                          select e).Where(e => e.Mail == currentMail);
            foreach(User i in query9)
            {
                user.accessToken = i.accessToken;
            }
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            HttpResponseMessage response = _httpClient.DeleteAsync($"{igCommentId}?access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
        public string DeleteComment2(string currentMail, string comment)
        {
            User user = new User();
            var query9 = (from e in _users.AsQueryable<User>()
                          select e).Where(e => e.Mail == currentMail);
            foreach (User i in query9)
            {
                user.accessToken = i.accessToken;
            }
            Rootobject1 rootobject1 = new Rootobject1();
            var query2 = (from e in _rootObject1.AsQueryable<Rootobject1>()
                          select e).Where(e => e.Mail == currentMail);
            int length = 0;
            string igMediaId;
            string idWhichWillBeUsed;
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            foreach (Rootobject1 i in query2)
            {
                length = i.data.Length;
                for (int ö = 0; ö < length; ö++)
                {
                    igMediaId = i.data[ö].id;
                    var query5 = (from e in _rootObject2.AsQueryable<Rootobject2>()
                                  select e).Where(e => e.igMediaId == igMediaId);
                    foreach (Rootobject2 b in query5)
                    {
                        for (int l = 0; l < b.data.Length; l++)
                        {
                            if (b.data[l].text == comment)
                            {
                                idWhichWillBeUsed = b.data[l].id;
                                _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
                                HttpResponseMessage response = _httpClient.DeleteAsync($"{idWhichWillBeUsed}?access_token={user.accessToken}").Result;
                                string result = response.Content.ReadAsStringAsync().Result;
                                return result;

                            }
                        }
                    }
                }
                
            }
            return "Comment couldn't find.";

        }
        public string GetAllRepliesToComments(string currentMail)   // kullanıcıya ait database'e kaydedilmiş bütün yorumlara gelen yanıtları alıp database'e ekliyor
        {
            User user = new User();
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
           foreach(User i in query)
            {
                user.accessToken = i.accessToken;
            }
            Rootobject1 rootobject1 = new Rootobject1();
            var query2 = (from e in _rootObject1.AsQueryable<Rootobject1>()
                          select e).Where(e => e.Mail == currentMail);
            int length = 0;
            string igMediaId;
            string idWhichWillBeUsed;
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            foreach (Rootobject1 i in query2)
            {
                length = i.data.Length ;
                for(int ö = 0; ö < length; ö++)
                {
                    igMediaId = i.data[ö].id;
                    var query5 = (from e in _rootObject2.AsQueryable<Rootobject2>()
                                  select e).Where(e => e.igMediaId == igMediaId);
                    foreach(Rootobject2 b in query5)
                    {
                        for(int l = 0; l < b.data.Length; l++)
                        {
                            idWhichWillBeUsed = b.data[l].id;
                            HttpResponseMessage response = _httpClient.GetAsync($"{idWhichWillBeUsed}/replies?access_token={user.accessToken}").Result;
                            string result = response.Content.ReadAsStringAsync().Result;
                            Rootobject3 rootobject3 = JsonConvert.DeserializeObject<Rootobject3>(result);
                            rootobject3.Mail = currentMail;
                            rootobject3.commentId = idWhichWillBeUsed;
                            var query20 = (from z in _rootobject3.AsQueryable<Rootobject3>()
                                           select z).Any(z => z.commentId == idWhichWillBeUsed);
                            if(query20 == true)
                            {
                                _rootobject3.ReplaceOne(z => z.commentId == idWhichWillBeUsed, rootobject3);
                            }
                            else
                            {
                                _rootobject3.InsertOne(rootobject3);
                            }
                           
                        }
                    }
                }
            }       
            return "All replies to all comments are obtained";
        }

        [Obsolete]
        public string Upload(string currentMail, string caption,IFormFile file)  // verilen caption ile bir post atmak için
        {
            
            string a = "";
            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                 file.CopyToAsync(stream);
            }
            var uploadParams = new ImageUploadParams()
            {
               
                File = new FileDescription(filePath)
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            var url = uploadResult.SecureUri.AbsoluteUri;
            UserPosts userPosts = new UserPosts();
            User user = new User();
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach (User i in query)
            {
                user.accessToken = i.accessToken;
            }

            var query1 = (from e in _rootObject.AsQueryable<Rootobject>()
                          select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach (Rootobject i in query1)
            {
                a = i.instagram_business_account.id;
                userPosts.igID = a;
                userPosts.Url = url;
                _userPosts.InsertOne(userPosts);
            }
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            string postUri = $"https://graph.facebook.com/{userPosts.igID}/media?image_url={url}&caption={caption}&access_token={user.accessToken}";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>($"url",url),
                new KeyValuePair<string,string>($"caption",caption)
            });
            HttpResponseMessage response = _httpClient.PostAsync(postUri, content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            /*Containers container = new Containers();
            var query2 = (from e in _userPosts.AsQueryable<UserPosts>()
                         select e).Where(e => e.Url == url);
            foreach(UserPosts i in query2)
            {
                container.Url = i.Url;
            }   
            */
            Containers container = JsonConvert.DeserializeObject<Containers>(result);
            container.Url = url;
            _containers.InsertOne(container);

            string postUri2 = $"https://graph.facebook.com/{userPosts.igID}/media_publish?creation_id={container.id}&access_token={user.accessToken}";
            var content2 = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>($"container.id",container.id),
                new KeyValuePair<string,string>($"userPosts.igID",userPosts.igID),
                new KeyValuePair<string, string>($"user.accessToken",user.accessToken)
            });
            HttpResponseMessage response1 = _httpClient.PostAsync(postUri2, content2).Result;
            string result1 = response1.Content.ReadAsStringAsync().Result;
            return result1;

        }
        [Obsolete]
        public string UploadVideo(string currentMail, string caption, IFormFile video)  // caption ile video paylaşımı
        {

            string a = "";
            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                video.CopyToAsync(stream);
            }
            var uploadParams = new VideoUploadParams()
            {

                File = new FileDescription(filePath),
            /*    PublicId = "myfolder/video",
                EagerTransforms = new List<Transformation>()
                {
                      new EagerTransformation().Width(300).Height(300).Crop("pad").AudioCodec("h264"),
                      new EagerTransformation().Width(160).Height(100).Crop("crop").Gravity("south").AudioCodec("h264"),
                }
             
                */
             };            
            var uploadResult = cloudinary.Upload(uploadParams);
            var url = uploadResult.SecureUri.AbsoluteUri;

            UserPosts userPosts = new UserPosts();
            User user = new User();
            var query = (from e in _users.AsQueryable<User>()
                         select e).Where(e => e.Mail == currentMail);
            foreach (User i in query)
            {
                user.accessToken = i.accessToken;
            }

            var query1 = (from e in _rootObject.AsQueryable<Rootobject>()
                          select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach (Rootobject i in query1)
            {
                a = i.instagram_business_account.id;
                userPosts.igID = a;
                userPosts.Url = url;
                _userPosts.InsertOne(userPosts);
            }
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");
            string postUri = $"https://graph.facebook.com/{userPosts.igID}/media?media_type=VIDEO&video_url={url}&caption={caption}&access_token={user.accessToken}";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>($"url",url),
                new KeyValuePair<string,string>($"caption",caption)
            });
            HttpResponseMessage response = _httpClient.PostAsync(postUri, content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            /*Containers container = new Containers();
            var query2 = (from e in _userPosts.AsQueryable<UserPosts>()
                         select e).Where(e => e.Url == url);
            foreach(UserPosts i in query2)
            {
                container.Url = i.Url;
            }   
            */
            Containers container = JsonConvert.DeserializeObject<Containers>(result);
            container.Url = url;
            _containers.InsertOne(container);

            string postUri2 = $"https://graph.facebook.com/{userPosts.igID}/media_publish?creation_id={container.id}&access_token={user.accessToken}";
            var content2 = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>($"container.id",container.id),
                new KeyValuePair<string,string>($"userPosts.igID",userPosts.igID),
                new KeyValuePair<string, string>($"user.accessToken",user.accessToken)
            });
            HttpResponseMessage response1 = _httpClient.PostAsync(postUri2, content2).Result;
            string result1 = response1.Content.ReadAsStringAsync().Result;
            return result1;

        }

        public string GetMediaObjectsWhichYouMentioned(string currentMail)  // giriş yapmış kullanıcının etiketlendiği bütün medya objelerini alıyor
        {
            string c = "";
            User user = new User();
            var query1 = (from z in _users.AsQueryable<User>()
                          select z).Where(z => z.Mail == currentMail);
            foreach(User i in query1)
            {
                user.accessToken = i.accessToken;
            }
            Rootobject rootobject = new Rootobject();
                    
            var query = (from e in _rootObject.AsQueryable<Rootobject>()
                         select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach(Rootobject i in query)
            {
                c= i.instagram_business_account.id;
                
            }
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"{c}/tags?access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }
        public string GetCommentsWhichYouMentioned(string currentMail,string commentId)   // giriş yapan kullanıcının bahsedildiği bütün commentleri alıyor
        {
            string c = "";
            User user = new User();
            var query1 = (from z in _users.AsQueryable<User>()
                          select z).Where(z => z.Mail == currentMail);
            foreach (User i in query1)
            {
                user.accessToken = i.accessToken;
            }
            Rootobject rootobject = new Rootobject();

            var query = (from e in _rootObject.AsQueryable<Rootobject>()
                         select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach (Rootobject i in query)
            {
                c = i.instagram_business_account.id;

            }
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"{c}?fields=mentioned_comment.comment_id({commentId})&access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }
        public string GetMediaWhichYouMentionedInComment(string currentMail, string mediaId)
        {
            string c = "";
            User user = new User();
            var query1 = (from z in _users.AsQueryable<User>()
                          select z).Where(z => z.Mail == currentMail);
            foreach (User i in query1)
            {
                user.accessToken = i.accessToken;
            }
            Rootobject rootobject = new Rootobject();

            var query = (from e in _rootObject.AsQueryable<Rootobject>()
                         select e).Where(e => e.instagram_business_account.Mail == currentMail);
            foreach (Rootobject i in query)
            {
                c = i.instagram_business_account.id;

            }
            _httpClient.BaseAddress = new Uri(Constant.baseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"{c}?fields=mentioned_media.media_id({mediaId})&access_token={user.accessToken}").Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }
    }     
}
