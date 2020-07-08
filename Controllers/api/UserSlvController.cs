using AppWebServer.Models;
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
        public HttpResponseMessage GetUserSlv(int Id)
        {
            string str = "";
            List<UserSlv> userSlvs = db.UserSlv.Where(i => i.userId == Id).ToList();
            foreach (UserSlv userSlv in userSlvs)
            {

                str += string.Format(",{{\"id\":\"{0}\",\"title\":\"{1}\",\"data\":\"銀幣：{2}\",\"subtext\":\"截止日期：{3}\",\"photo\":\"{4}\",\"url\":\"\"}}", userSlv.eId, userSlv.name, userSlv.currNum, userSlv.dateEnd.Value.ToString("yyyy-MM-dd"), userSlv.EventPage.url);


            }
            return new HttpResponseMessage { Content = new StringContent("[" +(str.Length>0?str.Substring(1):"") + "]", Encoding.GetEncoding("UTF-8"), "application/json") };

        }
        public HttpResponseMessage GetOwnerSlv(int Id)
        {
            List<int> keys = new List<int>();
            string str = "";
            List<ownerSlvsdetail> owners = db.ownerSlvsdetail.Where(i => i.eId == Id).OrderBy(i => i.slvId).ToList();
            foreach (ownerSlvsdetail item in owners)
            {
                if (item.num > 0 && !keys.Contains(item.slvId))
                {
                    str += string.Format(",{{\"id\":\"{0}\",\"title\":\"{1}\",\"data\":\"銀幣數量：{2}\",\"subtext\":\"取得日期：{3}\",\"photo\":\"{4}\",\"url\":\"\"}}", item.slvId, item.sname, item.num, item.dateGet.Value.ToString("yyyy-MM-dd"), item.potos.Split(',').FirstOrDefault());
                    keys.Add(item.slvId);
                }
            }
            return new HttpResponseMessage { Content = new StringContent("[" + str.Substring(1) + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        public HttpResponseMessage GetSlvDetail(int Id)
        {
            string str = "";
            Slvs slvs = db.Slvs.Find(Id);
            str = string.Format("{{\"potos\":[\"{0}\"],\"html\":\"{1}\",\"url\":\"{2}\"}}", slvs.potos.Replace(",","\",\""), slvs.html, "");
            return new HttpResponseMessage { Content = new StringContent("[" + str + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
    }
}
