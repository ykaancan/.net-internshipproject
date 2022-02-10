using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Services
{
    public interface IUserLoginService
    {
        string Register(string mail, string password, string name, string surname);
        string Login(string mail, string password);       
    }
}
