using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppWebServer.Controllers.exchangModels
{
    public class HomePageJSON
    {
        public List<String> Banner { get; set; }
        public List<EventItem> EventList { get; set; }
    }
}