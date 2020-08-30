using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppWebServer.Controllers.exchangModels
{
    public class StoreDetail
    {
        public string storeHtml { get; set; }
        public string StoreWeb { get; set; }
        public string storeWebF { get; set; }
        public string stroeMapLocation { get; set; }
        public dynamic storeProductList { get; set; }
        public List<string> images { get; set; }
        public DateTime LastEditDateTime { get; set; }

        public StoreDetail(string storeHtml, string storeWeb, string storeWebF, string stroeMapLocation, string storeProductList, string images, DateTime lastEditDateTime)
        {
            this.storeHtml = storeHtml;
            StoreWeb = storeWeb;
            this.storeWebF = storeWebF;
            this.stroeMapLocation = stroeMapLocation;
            this.storeProductList = Newtonsoft.Json.Linq.JToken.Parse(storeProductList) as dynamic;
            this.images = images.Split(',').ToList<string>();
            LastEditDateTime = lastEditDateTime;
        }
    }
}