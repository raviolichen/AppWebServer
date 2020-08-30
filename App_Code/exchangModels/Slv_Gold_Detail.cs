using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppWebServer.Controllers.exchangModels
{
    //            str = string.Format("{{\"potos\":[\"{0}\"],\"html\":\"{1}\",\"url\":\"{2}\"}}", (slvs.potos ?? "").Replace(",", "\",\""), (slvs.html ?? "").Replace("\"", @"\"""), "");
    public class Slv_Gold_Detail
    {
        public List<string> potos { get; set; }
        public string html { get; set; }
        public string url { get; set; }
        public DateTime LastEditDateTime { get; set; }
        public Slv_Gold_Detail(string potos, string html, string url, DateTime LastEditDateTime)
        {
            this.potos = potos.Split(',').ToList();
            this.html = html;
            this.url = url;
            this.LastEditDateTime = LastEditDateTime;
        }
    }
}