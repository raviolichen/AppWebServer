using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppWebServer.Controllers.exchangModels
{
    public class RecordJSON
    {
        public string id { get; set; }
        public string title { get; set; }
        public string subtext { get; set; }
        public string data { get; set; }
        public string photo { get; set; }
        public string url { get; set; }
        public RecordJSON() { }
        public RecordJSON(string Id,string title,string subtext,string data,string photo,string url)
        {
            this.id = Id;
            this.title = title;
            this.subtext = subtext;
            this.data = data;
            this.photo = photo;
            this.url = url;
        }

    }
}