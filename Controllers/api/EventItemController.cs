using AppWebServer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;

namespace AppWebServer.Controllers
{
    public class EventItemController : ApiController
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        [ActionName("GetEventDetail")]
        public HttpResponseMessage GetEventDetail(int? eId,int? userId)
        {
            //int userId = 2;
            //int eId = 2;
            string str = @"[{{""eventType"": ""{0}"",""ButtonText"": ""{1}"",""ButtonEnable"": ""{2}"",""detail"": ""{3}""}}]";
            EventPage eventPage = db.EventPage.Find(eId);
            SignRecords signRecord=null;
            if (eventPage.SignForm.Count() > 0)
            {
                if (eventPage.SignForm.First().SignRecords.Count > 0)
                    signRecord = eventPage.SignForm.First().SignRecords.Where((i) => i.userId == userId).FirstOrDefault();
                if (eventPage != null)
                {
                    string button;
                    if (signRecord == null)
                        button = "我要報名";
                    else
                        button = "編輯報名";
                    str = string.Format(str, eventPage.evenType, button, "true", eventPage.html);

                }
            }
            else
            {
                str = string.Format(str, "info", "", "false", eventPage.html);
            }
            return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        [ActionName("GetEventSgin")]
        public HttpResponseMessage GetEventSgin(int? eId, int? userId,int? sId=0)
        {
            //int userId = 2;
            //int eId = 2;
            EventPage eventPage = db.EventPage.Find(eId);
            SignForm signFrom= eventPage.SignForm.First();
            SignRecords signRecords = null;
            if (signFrom.SignRecords.Count>0)
                signRecords = signFrom.SignRecords.Where((i) => i.userId == userId).FirstOrDefault();
            string str="";
            if (eventPage != null)
            {
                string[] feilds = signFrom.field.Split(',');
                JObject values = null;
                if (signRecords != null)
                {
                    values = JsonConvert.DeserializeObject(signRecords.signData) as JObject;
                }
                foreach(string feildname in feilds)
                {
                    str+=@",{""fname"": """+ feildname + @""",""ftype"": """",""fvalue"":"""+ (values==null?"":values[feildname]) + @"""}";
                }
                str ="["+ str.Substring(1)+ "]";

            }
            return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        [ActionName("PostSginData")]
        [HttpPost]
        public HttpResponseMessage PostSginData([FromBody] string data)
        {

            JObject jsondata = JsonConvert.DeserializeObject(data) as JObject;
            string feildValue = System.Text.Encoding.GetEncoding("utf-8").GetString(Convert.FromBase64String(jsondata["data"].ToString()));
            int userId;
            int eId;
            int sId;
            if (int.TryParse(jsondata["userId"].ToString(), out userId) && int.TryParse(jsondata["eId"].ToString(), out eId))
            {
                User user = db.User.Find(userId);
                EventPage eventPage = db.EventPage.Find(eId);
                if (eventPage != null && user.token.CompareTo(jsondata["token"].ToString()) == 0 && DateTime.Now < user.tokendate)
                {
                    sId = eventPage.SignForm.First().sId;
                    SignRecords signRecord = db.SignRecords.SqlQuery("SELECT * FROM SignRecords WHERE userId=" + userId + " AND sId=" + sId).FirstOrDefault();

                    if (signRecord == null)
                    {
                        signRecord = new SignRecords();
                    signRecord.userId = userId;
                    signRecord.sId = sId;
                    db.SignRecords.Add(signRecord);
                    }
                    else
                    {
                        db.Entry(user).State = EntityState.Modified;
                    }
                    signRecord.signData = feildValue;


                    if (db.SaveChanges() > 0)
                    {
                        string str = "isOK";
                        return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
                    }
                }
            }

            return null;
            
        }
    }
}
