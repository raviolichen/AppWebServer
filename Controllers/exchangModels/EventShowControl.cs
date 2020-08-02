using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppWebServer.Controllers.exchangModels
{
    public class EventShowControl
    {
        public string eventType { get; set; }
        public string ButtonText { get; set; }
        public string ButtonEnable { get; set; }
        public string detail { get; set; }
        public string VoteButtonText { get; set; }
        public string VoteButtonEnable { get; set; }
        public string vId { get; set; }
        public string vtype { get; set; }
        public int votecount { get; set; }
        public EventShowControl(string eventType,string ButtonText, string ButtonEnable,string detail,string VoteButtonText, string VoteButtonEnable,string vId,string vtype,int votecount)
        {
            this. eventType= eventType;
            this. ButtonText= ButtonText;
            this. ButtonEnable= ButtonEnable;
            this. detail= detail;
            this. VoteButtonText= VoteButtonText;
            this. VoteButtonEnable= VoteButtonEnable;
            this. vId=vId;
            this.vtype = vtype;
            this.votecount = votecount;
        }
    }
}