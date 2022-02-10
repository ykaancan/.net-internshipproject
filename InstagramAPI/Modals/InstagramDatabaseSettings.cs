using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    public class InstagramDatabaseSettings : IInstagramDatabaseSettings
    {
        // public string BusinessAccountCollectionName { get; set; }
        public string LoginCredentialsCollectionName { get; set; }
        public string UsersCollectionName { get; set;}
        public string InstagramBusinessAccountsCollectionName { get; set; }
        public string UserPostsCollectionName { get; set; }
        public string ContainersCollectionName { get; set; }
        public string UserMediasCollectionName { get; set; }
        public string CommentsCollectionName { get; set; }
        public string CommentRepliesCollectionName { get; set; }
        public string HashtagsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IInstagramDatabaseSettings
    {

        string LoginCredentialsCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string InstagramBusinessAccountsCollectionName { get; set; }
        string UserPostsCollectionName { get; set; }
        string UserMediasCollectionName { get; set; }
        string ContainersCollectionName { get; set; }
        string CommentRepliesCollectionName { get; set; }
        string CommentsCollectionName { get; set; }
        string HashtagsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
