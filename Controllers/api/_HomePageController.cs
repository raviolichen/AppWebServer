using AppWebServer.Controllers.exchangModels;
using AppWebServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AppWebServer.Controllers
{
    public class _HomePageController : ApiController
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        private DateTime CompareDateTime_Min(DateTime curr, DateTime start, DateTime End)
        {
            if (start > DateTime.Now)
            {
                return curr > start ? start : curr;
            }
            else
            {
                return curr > End ? End : curr;
            }

        }
        public HttpResponseMessage GetHomePageJson(string md5)
        {
            DateTime ExpiredDatetime = DateTime.Now.AddYears(2000);
            HomePageJSON homePageJSON = new HomePageJSON
            {
                EventList = new List<EventItem>(),
                Banner = new List<string>()
            };
            Cache cache = db.Cache.Where(c =>c.name.CompareTo("HomePageJSON") ==0 && c.expired == true && c.expiredDateTime >= DateTime.Now).FirstOrDefault();
            if (cache != null)
            {
                if (md5 != null && cache.md5.CompareTo(md5) == 0)
                    return new HttpResponseMessage { Content = new StringContent("cache", Encoding.GetEncoding("UTF-8"), "application/json") };
                else
                    return new HttpResponseMessage { Content = new StringContent("[" + cache.data + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else
            {
                if (db.Banner.Count() > 0)
                {
                    foreach (Banner banner in db.Banner.OrderByDescending(o => o.Id).ToList())
                    {
                        if (banner.dateStart <= DateTime.Now && banner.dateEnd.Value.AddDays(1) >= DateTime.Now)
                        {
                            homePageJSON.Banner.Add(banner.url);
                            ExpiredDatetime = CompareDateTime_Min(ExpiredDatetime, banner.dateStart.GetValueOrDefault(), banner.dateEnd.Value.AddDays(1));
                        }
                    }
                }
                if (db.EventPage.Count() > 0)
                {
                    foreach (EventPage eventPage in db.EventPage.OrderByDescending(o => o.eid).ToList())
                    {
                        if (eventPage.dateStart <= DateTime.Now && eventPage.dateEnd.Value.AddDays(1) >= DateTime.Now)
                        {
                            homePageJSON.EventList.Add(new EventItem(eventPage.eid, eventPage.url, eventPage.name, eventPage.postDate.Value.ToString("yyyy-MM-dd"), ""));
                            ExpiredDatetime = CompareDateTime_Min(ExpiredDatetime, eventPage.dateStart.GetValueOrDefault(), eventPage.dateEnd.Value.AddDays(1));
                        }
                    }
                }
                homePageJSON.md5 = Utility.CreateMD5(JsonConvert.SerializeObject(homePageJSON));
                cache = db.Cache.Where(c => c.name.CompareTo("HomePageJSON") == 0).FirstOrDefault();
                if (cache == null)
                {
                    cache = new Cache();
                    db.Cache.Add(cache);
                }
                else
                {
                    db.Entry(cache).State = EntityState.Modified;
                }
                cache.name = "HomePageJSON";
                cache.data = JsonConvert.SerializeObject(homePageJSON);
                cache.expired = true;
                cache.expiredDateTime = ExpiredDatetime;
                cache.md5 = homePageJSON.md5;
                db.SaveChanges();
            }
            return new HttpResponseMessage { Content = new StringContent("[" + cache.data + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }

    }
}
