using InstagramAPI.Modals;
using InstagramAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Controllers
{

    [Route("[controller]")]
    public class UserLoginController : Controller
    {
        public const string SessionMail = "";
        private readonly IUserLoginService _userLoginService;
        public UserLoginController(IUserLoginService userLoginService)
        {
            _userLoginService = userLoginService;
        }
        [HttpPost]
        [Route("Register")]
        public string Register(string mail, string password, string name, string surname)
        {
            return _userLoginService.Register(mail, password, name, surname);

        }

        [HttpPost]
        [Route("Login")]
        public string Login(string mail,string password )
        {
            HttpContext.Session.SetString(SessionMail,mail);
            return _userLoginService.Login(mail,password);                              
          
        }
        [HttpGet]       
        public IActionResult UserLogin()
        {
            return View("UserLogin");
        }
      
      /*  [HttpGet]
        public string GetCurrentMail()
        {
            if (abc == "")
            {
                abc = SessionMail;
            }
            return abc;
            
        }
      */


    }
}
