using AppWebServer.Controllers.exchangModels;
using AppWebServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace AppWebServer.Controllers.api
{
    public class AboutStoreController : ApiController
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        public HttpResponseMessage GetStoreType(string md5)
        {
            Cache cache = db.Cache.Where(c => c.name.CompareTo("StoreType") == 0 && c.expired == true).FirstOrDefault();
            if (cache != null)
            {
                if (md5 != null && cache.md5.CompareTo(md5) == 0)
                    return new HttpResponseMessage { Content = new StringContent("cache", Encoding.GetEncoding("UTF-8"), "application/json") };
                else
                    return new HttpResponseMessage { Content = new StringContent("[" + cache.data + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else
            {
                var storeType = db.StoreType.ToList();
                string str = "";
                if (storeType.Count() > 0)
                {
                    foreach (StoreType _storeType in storeType)
                    {
                        str += string.Format(",{{" + @"""TId"":""{0}"",""TName"":""{1}"",""Images"":[""{2}""]" + "}}", _storeType.stId, _storeType.stName, _storeType.stpotos.Replace(",", @""","""));
                    }
                }
                string josn = (str.Length > 0 ? str.Substring(1) : "");
                md5 = Utility.CreateMD5(josn);
                cache = db.Cache.Where(c => c.name.CompareTo("StoreType") == 0).FirstOrDefault();
                if (cache == null)
                {
                    cache = new Cache();
                    db.Cache.Add(cache);
                }
                else
                {
                    db.Entry(cache).State = EntityState.Modified;
                }
                cache.name = "StoreType";
                cache.data = josn + ",{\"md5\":\"" + md5 + "\"}";
                cache.expired = true;
                cache.expiredDateTime = DateTime.Now;
                cache.md5 = md5;
                db.SaveChanges();
                return new HttpResponseMessage { Content = new StringContent("[" + cache.data + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
            }
        }
        public HttpResponseMessage GetStoreList(int Id,string md5)
        {
            string cacheName = "StoreList?TypeId="+Id;
            StoreType storeType = db.StoreType.Find(Id);
            Cache cache = db.Cache.Where(c => c.name.CompareTo(cacheName) == 0 && c.expired == true).FirstOrDefault();
            if (cache != null)
            {
                if (md5 != null && cache.md5.CompareTo(md5) == 0)
                    return new HttpResponseMessage { Content = new StringContent("cache", Encoding.GetEncoding("UTF-8"), "application/json") };
                else
                    return new HttpResponseMessage { Content = new StringContent("[" + cache.data + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else
            {
               List<RecordJSON> recordJSONs = new List<RecordJSON>();
                string str = "";
                if (storeType != null && storeType.store.Count > 0)
                {
                    foreach (store i in storeType.store)
                    {
                        str += string.Format(@",{{""id"":""{0}"",""title"":""{1}"",""data"":""{2}"",""subtext"":""{3}"",""photo"":""{4}"",""url"":""""}}",
                            i.storeId, i.storeName, i.sotrePhone, i.sotreAddr, ((i.potos != null && i.potos.Length > 0) ? i.potos.Split(',')[0] : "about:block"));

                    }

                }
                string josn =  (str.Length > 0 ? str.Substring(1) : "");
                md5= Utility.CreateMD5(josn);
                cache = db.Cache.Where(c => c.name.CompareTo(cacheName) == 0).FirstOrDefault();
                if (cache == null)
                {
                    cache = new Cache();
                    db.Cache.Add(cache);
                }
                else
                {
                    db.Entry(cache).State = EntityState.Modified;
                }
                cache.name = cacheName;
                cache.data = josn.Length>0? josn + ",{\"md5\":\""+md5+"\"}": "{\"md5\":\"" + md5 + "\"}";
                cache.expired = true;
                cache.expiredDateTime = DateTime.Now;
                cache.md5 = md5;
                db.SaveChanges();

                return new HttpResponseMessage { Content = new StringContent("[" + cache.data + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
            }
        }
        public HttpResponseMessage GetStoreDetail(int Id,string LastEditDateTime)
        {
            StoreDetail ds = null;
            store s = db.store.Find(Id);
            if (LastEditDateTime==null||s.LastEditDateTime != DateTime.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(LastEditDateTime)))) { 
                string str = "";
                if (s != null)
                {
                    ds = new StoreDetail(s.html, s.sotreWeb ?? "", s.sotreWeb2 ?? "", s.sotreAddr != null ? "https://www.google.com/maps/dir/?api=1&destination=" + HttpUtility.UrlEncode(s.sotreAddr) : "", s.products ?? "", s.potos, s.LastEditDateTime.Value);
                }
                
                return new HttpResponseMessage { Content = new StringContent("[" + JsonConvert.SerializeObject(ds) + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else
                return new HttpResponseMessage { Content = new StringContent("cache", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
    }
}
