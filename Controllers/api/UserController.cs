using AppWebServer.Controllers.exchangModels;
using AppWebServer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        public HttpResponseMessage GetUser(string deviceId)
        {
            SHA512 token = new SHA512CryptoServiceProvider();
            User user = null;
            if (db.User.ToList().Count > 0)
                user = db.User.SqlQuery("SELECT * FROM [User] WHERE deviceId LIKE +'" + deviceId + "'").FirstOrDefault();
            string _messagePublishes="";

            if (user == null)
            {
                // user = new User { deviceId = deviceId,token= Convert.ToBase64String(crypto),tokendate=DateTime.Now.AddHours(4) };
                // db.User.Add(user);         
                List<messagePublish> messagePublishs = db.messagePublish.Where(i => i.pDateEnd >= DateTime.Now && i.pDateStart <= DateTime.Now && i.p_users.ToLower().CompareTo("all") == 0).ToList();
                foreach(messagePublish messages in messagePublishs)
                {
                    _messagePublishes += "‧"+messages.pmessage + "\n\n";
                }
                return new HttpResponseMessage { Content = new StringContent("{\"result\":\"未登記的手機，將會被限制報名等相關功能，如要開啟，請至會員頁面進行登記作業。\",\"messagePublish\":\""+ _messagePublishes + "\"}", Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else
            {
                string encodeString = DateTime.Now.ToString("yyyyMMddHHmmss") + deviceId;
                byte[] source = Encoding.Default.GetBytes(encodeString);
                byte[] crypto = token.ComputeHash(source);
                user.token = Convert.ToBase64String(crypto);
                user.tokendate = DateTime.Now.AddHours(4);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                bool proxy = false;
                if (user.powerGroup != null && user.powerGroup != 0)
                {
                    try
                    {
                        int proxyValue = int.Parse(ConfigurationManager.AppSettings["proxyPowerGroup"].FirstOrDefault().ToString());
                        int powerful = db.UserGroup.Find(user.powerGroup).powerlevel;
                        if (powerful >= proxyValue)
                            proxy = true;
                    }
                    catch { }
                }
                List<messagePublish> messagePublishs = db.messagePublish.Where(i => i.pDateEnd >= DateTime.Now && i.pDateStart <= DateTime.Now && 
                (i.p_users.ToLower().CompareTo("all") == 0||i.p_users.Contains(user.userId+",")||i.p_users.CompareTo(user.powerGroup.Value.ToString())==0)).ToList();
                foreach (messagePublish messages in messagePublishs)
                {
                    _messagePublishes += "‧" + messages.pmessage + "\n\n";
                }
                UserData userData = new UserData(user.userName, user.userId.ToString(), Convert.ToBase64String(crypto), proxy.ToString(), user.goldnum.ToString(), "true", _messagePublishes);
                return new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(userData), Encoding.GetEncoding("UTF-8"), "application/json") };
            }



        }
        [HttpPost]
        public HttpResponseMessage PostUser([FromBody] string data)
        {
            JObject jsondata = JsonConvert.DeserializeObject(data) as JObject;
            SHA512 token = new SHA512CryptoServiceProvider();
            string deviceId = jsondata["deviceId"].ToString();
            string encodeString = DateTime.Now.ToString("yyyyMMddHHmmss") + deviceId;
            byte[] source = Encoding.Default.GetBytes(encodeString);
            byte[] crypto = token.ComputeHash(source);
            User user = null;
            User phoneuser = null;
            if (db.User.ToList().Count > 0)
            {
                user = db.User.SqlQuery("SELECT * FROM [User] WHERE deviceId LIKE +'" + deviceId + "'").FirstOrDefault();
                phoneuser = db.User.SqlQuery("SELECT * FROM [User] WHERE phone LIKE +'" + jsondata["phone"].ToString() + "'").FirstOrDefault();
            }
            if (user == null)
            {
                if (phoneuser == null)
                {
                    user = new User { goldnum = 0, userName = jsondata["userName"].ToString(), phone = jsondata["phone"].ToString(), deviceId = deviceId, token = Convert.ToBase64String(crypto), tokendate = DateTime.Now.AddHours(4) };
                    db.User.Add(user);
                }
                else
                {
                    return new HttpResponseMessage { Content = new StringContent("{\"result\":\"您所註冊的手機號碼已經被使用，請確認您已經綁定的手機，如被盜用請聯絡本所。\",\"userId\":\"0\"}", Encoding.GetEncoding("UTF-8"), "application/json") };
                }
            }
            else
            {
                user.token = Convert.ToBase64String(crypto);
                user.tokendate = DateTime.Now.AddHours(4);
                db.Entry(user).State = EntityState.Modified;

            }
            db.SaveChanges();
            return new HttpResponseMessage { Content = new StringContent("{\"userName\":\"" + user.userName + "\",\"golds\":\"" + user.goldnum + "\",\"result\":\"true\",\"userId\":\"" + user.userId.ToString() + "\",\"token\":\"" + Convert.ToBase64String(crypto) + "\"}", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
    }
}
