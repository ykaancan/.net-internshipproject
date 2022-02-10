using InstagramAPI.Modals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class FacebookLoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("Login");
            
        }
       
    }
}

