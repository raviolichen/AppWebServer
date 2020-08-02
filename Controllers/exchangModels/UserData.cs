using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppWebServer.Controllers.exchangModels
{
    public class UserData
    {
        //                return new HttpResponseMessage { Content = new StringContent("{\"userName\":\""+user.userName+"\",\"golds\":\""+user.goldnum+"\",\"result\":\"true\",\"userId\":\"" + user.userId.ToString() + "\",\"token\":\"" + Convert.ToBase64String(crypto) + "\"}", Encoding.GetEncoding("UTF-8"), "application/json") };
        public string userName { set; get; }
        public string golds { set; get; }
        public string result { set; get; }
        public string userId { set; get; }
        public string token { set; get; }
        public string proxy { set; get; }
        public string messagePublish { set; get; }
        public UserData() { }

        public UserData(string userName, string userId, string token, string proxy, string golds, string result,string messagePublish)
        {
            this.userName = userName;
            this.golds = golds;
            this.result = result;
            this.userId = userId;
            this.token = token;
            this.proxy = proxy;
            this.messagePublish = messagePublish;
        }
    }
}