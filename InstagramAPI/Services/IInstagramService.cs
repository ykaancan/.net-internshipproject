using CloudinaryDotNet.Actions;
using InstagramAPI.Modals;
using Microsoft.AspNetCore.Http;

namespace InstagramAPI.Services
{
    public interface IInstagramService
    {
        string GetInsights(string name, string currentMail);
       
        
        string getIgId(string currentMail);
        void GetPagesOfAccessTokensOwner(string accessToken,string userID,string data_access_expiration_time,string currentMail);
        string GetComments(string currentMail, string igMediaId);
        string GetAccountMetrics(string currentMail);
        string GetAccountImpressions(string currentMail);
       
        string GetUserMedia(string currentMail);      
        string Upload(string currentMail,string caption,IFormFile file);
        string PostAReplyToAComment(string currentMail, string igCommentId, string message);
        string DeleteComment(string currentMail, string igCommentId);
        string GetAllComments(string currentMail);
        string HideorUnhideComment(string currentMail, string comment, string igMediaId, string state);
        string GetAllRepliesToComments(string currentMail);
        string HideorUnhideComment2(string currentMail, string comment, string state);
        string DeleteComment2(string currentMail, string comment);
        string HashtagSearch(string hashtag, string currentMail);
        string UploadVideo(string currentMail, string caption, IFormFile video);
        string GetMediaObjectsWhichYouMentioned(string currentMail);
        string GetCommentsWhichYouMentioned(string currentMail,string commentId);
        string GetMediaWhichYouMentionedInComment(string currentMail, string mediaId);
        string HideUnhideComment(string commentId, string hide, string currentMail);

    }
}