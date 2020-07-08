using AppWebServer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace AppWebServer.Controllers
{
    public class UserController : ApiController
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        [HttpGet]
        public HttpResponseMessage getUser(string deviceId)
        {
            SHA512 token=new SHA512CryptoServiceProvider();
            User user = null;
            if (db.User.ToList().Count > 0)
                user = db.User.SqlQuery("SELECT * FROM [User] WHERE deviceId LIKE +'"+deviceId+"'").FirstOrDefault();
            string encodeString = DateTime.Now.ToString("yyyyMMddHHmmss") + user.deviceId;
            byte[] source = Encoding.Default.GetBytes(encodeString);
            byte[] crypto = token.ComputeHash(source);
            if (user == null)
            {
                user = new User { deviceId = deviceId,token= Convert.ToBase64String(crypto),tokendate=DateTime.Now.AddHours(4) };
                db.User.Add(user);                    
            }
            else
            {
                user.token = Convert.ToBase64String(crypto);
                user.tokendate = DateTime.Now.AddHours(4);
                db.Entry(user).State = EntityState.Modified;
            }
            db.SaveChanges();
            return new HttpResponseMessage { Content = new StringContent("{\"userId\":\"" + user.userId.ToString() + "\",\"token\":\"" + Convert.ToBase64String(crypto) + "\"}", Encoding.GetEncoding("UTF-8"), "application/json") };

        }
    }
}
