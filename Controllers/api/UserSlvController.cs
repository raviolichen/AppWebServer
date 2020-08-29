using AppWebServer.Controllers.exchangModels;
using AppWebServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AppWebServer.Controllers.api
{
    public class UserSlvController : ApiController
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        public HttpResponseMessage GetUserSlv(int Id, int page,int pagecount)
        {
            string str = "";
            List<RecordJSON> records = new List<RecordJSON>();
            List<UserSlv> userSlvs = db.UserSlv.Where(i => i.userId == Id).ToList();
            int pageIndex = page * pagecount;
            for (int i = pageIndex; i < (pageIndex + pagecount) && i < userSlvs.Count; i++)
            {
                //str += string.Format(",{{\"id\":\"{0}\",\"title\":\"{1}\",\"data\":\"銀幣：{2}\",\"subtext\":\"截止日期：{3}\",\"photo\":\"{4}\",\"url\":\"\"}}", userSlv.eId, userSlv.name, userSlv.currNum, userSlv.dateEnd.Value.ToString("yyyy-MM-dd"), userSlv.EventPage.url);
                records.Add(new RecordJSON(userSlvs[i].eId.ToString(), userSlvs[i].name, "銀幣：" + userSlvs[i].currNum, "截止日期：" + userSlvs[i].dateEnd.Value.ToString("yyyy-MM-dd"), userSlvs[i].EventPage.url, ""));
            }
            return new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(records), Encoding.GetEncoding("UTF-8"), "application/json") };

            //return new HttpResponseMessage { Content = new StringContent("[" +(str.Length>0?str.Substring(1):"") + "]", Encoding.GetEncoding("UTF-8"), "application/json") };

        }
        public HttpResponseMessage GetOwnerSlv(int Id, int userId)
        {
            List<int> keys = new List<int>();
            string str = "";
            List<ownerSlvsdetail> owners = db.ownerSlvsdetail.Where(i => (i.eId == Id) && (i.userId == userId)).OrderBy(i => i.slvId).ToList();
            foreach (ownerSlvsdetail item in owners)
            {
                if (item.num > 0 && !keys.Contains(item.slvId) && item.isGuid.GetValueOrDefault())
                {
                    str += string.Format(",{{\"id\":\"{0}\",\"title\":\"{1}\",\"data\":\"銀幣數量：{2}\",\"subtext\":\"取得日期：{3}\",\"photo\":\"{4}\",\"url\":\"\"}}", item.slvId, item.sname, item.num, item.dateGet.Value.ToString("yyyy-MM-dd"), item.potos.Split(',').FirstOrDefault());
                    keys.Add(item.slvId);
                }
            }
            return new HttpResponseMessage { Content = new StringContent("[" + (str.Length > 0 ? str.Substring(1) : "") + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        public HttpResponseMessage GetSlvDetail(int Id)
        {
            string str = "";
            Slvs slvs = db.Slvs.Find(Id);
            str = string.Format("{{\"potos\":[\"{0}\"],\"html\":\"{1}\",\"url\":\"{2}\"}}", (slvs.potos ?? "").Replace(",", "\",\""), (slvs.html ?? "").Replace("\"", @"\"""), "");
            return new HttpResponseMessage { Content = new StringContent("[" + str + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
    }
}
