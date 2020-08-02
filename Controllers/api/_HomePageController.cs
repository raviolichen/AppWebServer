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

namespace AppWebServer.Controllers
{


    public class _HomePageController : ApiController
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        public HttpResponseMessage GetHomePageJson()
        {
            HomePageJSON homePageJSON = new HomePageJSON
            {
                EventList = new List<EventItem>(),
                Banner = new List<string>()
            };
            if (db.Banner.Count() > 0)
            {
                foreach (Banner banner in db.Banner.OrderByDescending(o=>o.Id).ToList())
                {
                    if(banner.dateStart <= DateTime.Now && banner.dateEnd.Value.AddDays(1) >= DateTime.Now)
                        homePageJSON.Banner.Add(banner.url);
                }
            }
            if (db.EventPage.Count() > 0)
            {
                foreach (EventPage eventPage in db.EventPage.OrderByDescending(o => o.eid).ToList())
                {
                    if (eventPage.dateStart <= DateTime.Now && eventPage.dateEnd.Value.AddDays(1) >= DateTime.Now)
                        homePageJSON.EventList.Add(new EventItem(eventPage.eid, eventPage.url, eventPage.name, eventPage.postDate.Value.ToString("yyyy-MM-dd"), ""));
                }
            }
            string str = JsonConvert.SerializeObject(homePageJSON);
            return  new HttpResponseMessage { Content = new StringContent("["+str+"]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }

    }
}
