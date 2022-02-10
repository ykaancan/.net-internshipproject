using InstagramAPI.Modals;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Providers.Entities;

namespace InstagramAPI.Services
{
    public class UserLoginService : IUserLoginService
    {
        
        private readonly IMongoCollection<LoginCredentials> _loginCredentials;
        


        public UserLoginService(IInstagramDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _loginCredentials = database.GetCollection<LoginCredentials>(settings.LoginCredentialsCollectionName);
            
        }
        public string Register(string mail, string password, string name, string surname)
        {
            string successful = "You registered successfully.";
            string failure = "There is already an account with this mail adress.";
            LoginCredentials loginCredential = new LoginCredentials();
            loginCredential.Mail = mail;
            loginCredential.Password = password;
            loginCredential.Name = name;
            loginCredential.Surname = surname;
            var query = (from e in _loginCredentials.AsQueryable<LoginCredentials>()
                         select e).Any(e => e.Mail == loginCredential.Mail);
            if (query == false)
            {
                _loginCredentials.InsertOne(loginCredential);
                return successful;
            }
            else return failure;
            
            
        }
        public string Login(string mail, string password)
        {
            string successful = "Login Successful.";
            string failure = "Mail or password is wrong.";
            LoginCredentials loginCredentials = new LoginCredentials();
            loginCredentials.Mail = mail;
            loginCredentials.Password = password;
            var query = (from e in _loginCredentials.AsQueryable<LoginCredentials>()
                         select e).Any(e => e.Mail == loginCredentials.Mail && e.Password == loginCredentials.Password);
            if (query == true)
            {
                return successful;
            }
            else return failure;
                      
        }
     

    }



}

