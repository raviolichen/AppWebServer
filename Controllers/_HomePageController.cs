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
            HomePageJSON homePageJSON = new HomePageJSON();
            homePageJSON.EventList = new List<EventItem>();
            homePageJSON.Banner = new List<string>();
            foreach(Banner banner in db.Banner.ToList())
            {
                homePageJSON.Banner.Add(banner.url);
            }
           foreach(EventPage eventPage in db.EventPage.ToList())
            {
                homePageJSON.EventList.Add(new EventItem(eventPage.eid, eventPage.url, eventPage.name,eventPage.postDate.Value.ToString("yyyy-MM-dd"),""));
            }
            string str = JsonConvert.SerializeObject(homePageJSON);
            return  new HttpResponseMessage { Content = new StringContent("["+str+"]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }

    }
    class HomePageJSON
    {
       public List<String> Banner { get; set; }
        public List<EventItem> EventList { get; set; }
    }
    class EventItem
    {
        public int eId { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string date { get; set; }
        public string text { get; set; }
        public EventItem(int eId, string url,string title, string date,string text)
        {
            this. eId=eId;
            this. url=url;
            this. title=title;
            this. date=date;
            this. text=text;
        }
    }
}
