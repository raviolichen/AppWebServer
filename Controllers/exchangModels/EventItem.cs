using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppWebServer.Controllers.exchangModels
{
    public class EventItem
    {
        public int eId { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string date { get; set; }
        public string text { get; set; }
        public EventItem(int eId, string url, string title, string date, string text)
        {
            this.eId = eId;
            this.url = url;
            this.title = title;
            this.date = date;
            this.text = text;
        }
    }
}