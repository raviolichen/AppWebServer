using AppWebServer.Controllers.exchangModels;
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
        public HttpResponseMessage GetEventDetail(int? eId, int? userId,string LastEditDateTime)
        {
            //int userId = 2;
            //int eId = 2;
            EventPage eventPage = db.EventPage.Find(eId);
            SignRecords signRecord = null;
            EventShowControl eventShowControl = null;
            if (eventPage != null)
            {
                string html = eventPage.html;//.Replace("\"", @"\""");
                if (LastEditDateTime !=null&& eventPage.lastEditDateTime == DateTime.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(LastEditDateTime))))
                    html = "cache";
                if (eventPage.SignForm.Count() > 0)
                {

                    string button = "";
                    string isEnable = "";
                    var CurrSignform = eventPage.SignForm.Where(i => i.isEnable.GetValueOrDefault() == true && i.dateStart <= DateTime.Now && i.dateEnd.Value.AddDays(1) >= DateTime.Now).FirstOrDefault();
                    if (CurrSignform != null)
                    {
                        if (CurrSignform.SignRecords.Count > 0)
                            signRecord = CurrSignform.SignRecords.Where((i) => i.userId == userId).FirstOrDefault();
                        int maxSign = CurrSignform.userLimit.GetValueOrDefault();
                        if (signRecord != null)
                        {
                            button = "編輯報名";
                            isEnable = "true";
                        }
                        else if (maxSign != 0 && maxSign > CurrSignform.SignRecords.Count && signRecord == null)
                        {
                            button = "我要報名(剩餘:" + (maxSign - CurrSignform.SignRecords.Count) + ")";
                            isEnable = "true";
                        }
                        else if (signRecord.User.prohibit != null && signRecord.User.prohibit > DateTime.Now)
                        {
                            button = "帳號被停權中(期限:" + signRecord.User.prohibit.Value.ToString("yyyy/MM/dd") + ")";
                            isEnable = "false";
                        }
                        else
                        {
                            button = "已經額滿";
                            isEnable = "false";
                        }
                    }
                    else
                    {
                        button = "目前無法報名，或不是報名期間";
                        isEnable = "false";
                    }
                    eventShowControl = new EventShowControl(eventPage.evenType, button, isEnable, html, "", "", "", "", 0, eventPage.lastEditDateTime.Value);
                }
                else
                {
                    eventShowControl = new EventShowControl("info", "", "false", html, "", "", "", "", 0, eventPage.lastEditDateTime.Value);
                }
            }
            GetVote(userId, eventPage, ref eventShowControl);
            return new HttpResponseMessage { Content = new StringContent("[" + JsonConvert.SerializeObject(eventShowControl) + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        private void GetVote(int? userId, EventPage eventPage, ref EventShowControl eventShowControl)
        {
            if (userId == null && eventShowControl == null && eventPage.Vote.Count == 0)
                return;
            string Button = "";
            string isEnable = "";
            var currVote = eventPage.Vote.Where(i => i.dateStart <= DateTime.Now && i.dateEnd.Value.AddDays(1) >= DateTime.Now && i.isEnable.GetValueOrDefault()).FirstOrDefault();
            if (currVote != null)
            {
                if (currVote.vlog != null && currVote.vlog.Contains(userId.ToString()+","))
                {
                    Button = "已經完成投票。";
                    isEnable = "false";
                }
                else
                {
                    Button = "前往投票";
                    isEnable = "true";
                }
            }
            eventShowControl.VoteButtonEnable = isEnable;
            eventShowControl.VoteButtonText = Button;
            eventShowControl.vId = currVote != null ? currVote.vId.ToString() : "";
            eventShowControl.vtype = currVote != null ? currVote.voteType : "";
            eventShowControl.votecount = currVote != null ? currVote.voteCount.GetValueOrDefault() : 0;
        }
        public HttpResponseMessage GetVoteList(int? Id)
        {
            Vote currvote = db.Vote.Find(Id);
            List<RecordJSON> records = new List<RecordJSON>();
            if (currvote.voteType.Contains("store"))
            {
                store _store;
                foreach (VoteItem voteItem in currvote.VoteItem)
                {
                    if (voteItem.isEnable.GetValueOrDefault())
                    {
                        _store = db.store.Find(voteItem.itemId);
                        if (_store != null && _store.isEnable.GetValueOrDefault())
                            records.Add(new RecordJSON(_store.storeId.ToString(), _store.storeName, _store.sotreAddr, _store.sotrePhone, _store.potos != null ? _store.potos.Split(',')[0] : "", ""));
                    }
                }
            }
            else if (currvote.voteType.Contains("Slvs"))
            {
                Slvs slvs;
                foreach (VoteItem voteItem in currvote.VoteItem)
                {
                    if (voteItem.isEnable.GetValueOrDefault())
                    {
                        slvs = db.Slvs.Find(voteItem.itemId);
                        if (slvs != null && slvs.isEnable.GetValueOrDefault())
                            records.Add(new RecordJSON(slvs.slvId.ToString(), slvs.sname, "", "", slvs.potos != null ? slvs.potos.Split(',')[0] : "", ""));
                    }
                }
            }
            return new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(records), Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        [ActionName("PostVoteData")]
        [HttpPost]
        public HttpResponseMessage PostVoteData([FromBody] string data)
        {
            try
            {
            JObject jsondata = JsonConvert.DeserializeObject(data) as JObject;
            string votedata = System.Text.Encoding.GetEncoding("utf-8").GetString(Convert.FromBase64String(jsondata["data"].ToString()));
            int userId;
            int vId;
                if (votedata.Length > 0 && int.TryParse(jsondata["userId"].ToString(), out userId) && int.TryParse(jsondata["vId"].ToString(), out vId))
                {
                    User user = db.User.Find(userId);
                    Vote currVote = db.Vote.Find(vId);
                    if (currVote != null && user.token.CompareTo(jsondata["token"].ToString()) == 0 && DateTime.Now < user.tokendate)
                    {
                        int itemId_int;
                        VoteItem voteItem;
                        if (currVote.vlog == null)
                            currVote.vlog = user.userId + ",";
                        else
                            currVote.vlog += user.userId + ",";
                        string[] voteitems = votedata.Split(',');
                        foreach (string itemId in voteitems)
                        {
                            itemId_int = int.Parse(itemId);
                            voteItem = db.VoteItem.Where(i => i.itemId == itemId_int).First();
                            if (voteItem != null)
                            {
                                if (voteItem.vcount == null)
                                    voteItem.vcount = 1;
                                else
                                    voteItem.vcount += 1;
                                db.Entry(voteItem).State = EntityState.Modified;
                            }
                        }
                        if (db.SaveChanges() > 0)
                        {
                            return new HttpResponseMessage { Content = new StringContent("isOk", Encoding.GetEncoding("UTF-8"), "application/json") };
                        }
                    }
                }
                return null;
            }
            catch(Exception e)
            {
                return new HttpResponseMessage { Content = new StringContent(e.ToString(), Encoding.GetEncoding("UTF-8"), "application/json") };
            }

        }

        [ActionName("GetEventSgin")]
        public HttpResponseMessage GetEventSgin(int? eId, int? userId, int? sId = 0)
        {
            //int userId = 2;
            //int eId = 2;
            EventPage eventPage = db.EventPage.Find(eId);
            SignRecords signRecords = null;
            SignForm currSignform = eventPage.SignForm.Where(i => i.isEnable.GetValueOrDefault() == true && i.dateStart <= DateTime.Now && i.dateEnd.Value.AddDays(1) >= DateTime.Now).FirstOrDefault();
            if (currSignform.SignRecords.Count > 0)
                signRecords = currSignform.SignRecords.Where((i) => i.userId == userId).FirstOrDefault();
            string str = "";
            if (eventPage != null)
            {
                string[] feilds = currSignform.field.Split(',');
                JObject values = null;
                if (signRecords != null)
                {
                    values = JsonConvert.DeserializeObject(signRecords.signData) as JObject;
                }
                foreach (string feildname in feilds)
                {
                    str += @",{""fname"": """ + feildname + @""",""ftype"": """",""fvalue"":""" + (values == null ? "" : values[feildname]) + @"""}";
                }
                str = "[" + str.Substring(1) + "]";

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
            //取消報名

            if (int.TryParse(jsondata["userId"].ToString(), out userId) && int.TryParse(jsondata["eId"].ToString(), out eId))
            {
                User user = db.User.Find(userId);
                EventPage eventPage = db.EventPage.Find(eId);
                SignForm currSignform = eventPage.SignForm.Where(i => i.isEnable.GetValueOrDefault() == true && i.dateStart <= DateTime.Now && i.dateEnd.Value.AddDays(1) >= DateTime.Now).FirstOrDefault();
                if (eventPage != null && user.token.CompareTo(jsondata["token"].ToString()) == 0 && DateTime.Now < user.tokendate)
                {
                    string str = "";
                    sId = currSignform.sId;
                    SignRecords signRecord = db.SignRecords.SqlQuery("SELECT * FROM SignRecords WHERE userId=" + userId + " AND sId=" + sId).FirstOrDefault();
                    if (feildValue.Contains("\"cancel\":\"true\""))
                    {
                        SignForm signForm = currSignform;
                        db.SignRecords.Remove(signRecord);
                        str = "isCancel,(剩餘:" + (signForm.userLimit - signForm.SignRecords.Count) + ")";

                    }
                    else
                    {
                        if (signRecord == null)
                        {
                            signRecord = new SignRecords
                            {
                                userId = userId,
                                sId = sId
                            };
                            db.SignRecords.Add(signRecord);

                        }
                        else
                        {
                            db.Entry(user).State = EntityState.Modified;
                        }
                        signRecord.signData = feildValue;
                        str = "isOk";
                    }

                    if (db.SaveChanges() > 0)
                    {
                        return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
                    }
                }
            }

            return null;

        }
    }
}
