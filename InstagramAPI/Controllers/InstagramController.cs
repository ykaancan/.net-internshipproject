using InstagramAPI.Modals;
using InstagramAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace InstagramAPI.Controllers
{
    [Route("/[controller]")]


    public class InstagramController : Controller
    {
        
        private readonly IInstagramService _instagramService;
        
        public InstagramController(IInstagramService instagramService)
        {
            _instagramService = instagramService;
           

        }       

        [HttpGet]
        [Route("GetPagesOfAccessTokensOwner")]
        public void GetPagesOfAccessTokensOwner(string accessToken, string userID, string data_access_expiration_time)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            _instagramService.GetPagesOfAccessTokensOwner(accessToken, userID, data_access_expiration_time, currentMail);
            

        }
     

        [HttpGet]
        [Route("getIgId")]
        public IActionResult getIgId()
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);          
            return Ok(_instagramService.getIgId(currentMail));
        }
        [HttpGet]
        [Route("GetInsights")]
        public IActionResult GetInsights(string name)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetInsights(name, currentMail));

        }
        [HttpGet]
        [Route("GetComments")]
        public IActionResult GetComments(string igMediaId)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetComments(currentMail,igMediaId));
        }
     
        [HttpGet]
        [Route("GetAccountMetrics")]
        public IActionResult GetAccountMetrics()
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetAccountMetrics(currentMail));
        }

        [HttpGet]
        [Route("GetAccountImpressions")]
        public IActionResult GetAccountImpressions()
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetAccountImpressions(currentMail));
        }      
       
        [HttpGet]
        [Route("GetUserMedia")]
        public IActionResult GetUserMedia()
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetUserMedia(currentMail));
        }
       
        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload(IFormFile file,string caption)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.Upload(currentMail,caption,file));
        }
        [HttpPost]
        [Route("PostReplyToAComment")]
        public IActionResult PostReplyToAComment(string igCommentId,string message)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.PostAReplyToAComment(currentMail,igCommentId,message));
        }
        [HttpDelete]
        [Route("DeleteComment")]
        public IActionResult DeleteComment(string igCommentId)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.DeleteComment(currentMail, igCommentId));
        }
        [HttpGet]
        [Route("GetAllComments")]
        public IActionResult GetAllComments()
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetAllComments(currentMail));
        }
        [HttpPost]
        [Route("HideorUnhideComment")]
        public IActionResult HideorUnhideComment(string comment,string igMediaId,string state)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.HideorUnhideComment(currentMail, comment, igMediaId, state));
        }
        [HttpGet]
        [Route("GetAllRepliesToComments")]
        public IActionResult GetAllRepliesToComments()
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetAllRepliesToComments(currentMail));
        }
        [HttpPost]
        [Route("HideorUnhideComment2")]
        public IActionResult HideorUnhideComment2(string comment,string state)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.HideorUnhideComment2(currentMail, comment, state));
        }
        [HttpDelete]
        [Route("DeleteComment2")]
        public IActionResult DeleteComment2(string comment)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.DeleteComment2(currentMail, comment));
        }
        [HttpGet]
        [Route("HashtagSearch")]
        public IActionResult HashtagSearch(string hashtag)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.HashtagSearch(hashtag, currentMail));
        }
        [HttpPost]
        [Route("UploadVideo")]
        public IActionResult UploadVideo(IFormFile video, string caption)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.UploadVideo(currentMail, caption, video));
        }
        [HttpGet]
        [Route("GetMediaObjectsWhichYouMentioned")]
        public IActionResult GetMediaObjectsWhichYouMentioned()
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetMediaObjectsWhichYouMentioned(currentMail));

        }
        [HttpGet]
        [Route("GetCommentsWhichYouMentioned")]
        public IActionResult GetCommentsWhichYouMentioned(string commentId)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetCommentsWhichYouMentioned(currentMail,commentId));
        }
        [HttpGet]
        [Route("GetMediaWhichYouMentionedInComment")]
        public IActionResult GetMediaWhichYouMentionedInComment(string mediaId)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.GetMediaWhichYouMentionedInComment(currentMail, mediaId));
        }
        [HttpPost]
        [Route("HideUnhideComment")]
        public IActionResult HideUnhideComment(string commentId,string hide)
        {
            const string SessionMail = "";
            string currentMail = HttpContext.Session.GetString(SessionMail);
            return Ok(_instagramService.HideUnhideComment(commentId, hide, currentMail));
        }
      
    }
}


