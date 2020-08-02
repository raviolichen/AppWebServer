using AppWebServer.Models;
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
        public HttpResponseMessage GetStoreType()
        {

            var storeType = db.StoreType.ToList();
            string str = "";
            if (storeType.Count() > 0)
            {
                foreach(StoreType _storeType in storeType)
                {
                    str+=string.Format(",{{" + @"""TId"":""{0}"",""TName"":""{1}"",""Images"":[""{2}""]" + "}}",_storeType.stId, _storeType.stName, _storeType.stpotos.Replace(",",@""","""));
                }
            }
            return new HttpResponseMessage { Content = new StringContent("[" + str.Substring(1) + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        public HttpResponseMessage GetStoreList(int Id)
        {
            StoreType storeType = db.StoreType.Find(Id);
            string str = "";
            if(storeType!=null && storeType.store.Count > 0)
            {
                foreach(store i in storeType.store)
                {
                    str += string.Format(@",{{""id"":""{0}"",""title"":""{1}"",""data"":""{2}"",""subtext"":""{3}"",""photo"":""{4}"",""url"":""""}}",
                        i.storeId,i.storeName,i.sotrePhone,i.sotreAddr,((i.potos!=null&&i.potos.Length>0)?i.potos.Split(',')[0]:"about:block"));
                }

            }
            return new HttpResponseMessage { Content = new StringContent("[" +(str.Length>0?str.Substring(1):"") + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        public HttpResponseMessage GetStoreDetail(int Id)
        {
            store s = db.store.Find(Id);
            string str = "";
            if (s != null)
            {
                str += string.Format(@"{{""storeHtml"":""{0}"",""StoreWeb"":""{1}"",""storeWebF"":""{2}"",""stroeMapLocation"":""{3}"",""storeProductList"":{4},""images"":[""{5}""]}}",
                       s.html!=null?s.html.Replace("\"", @"\""") : "", s.sotreWeb ?? "", s.sotreWeb2 ?? "", s.sotreAddr!= null? "https://www.google.com/maps/dir/?api=1&destination="+HttpUtility.UrlEncode(s.sotreAddr):"", s.products ?? "", s.potos!=null? s.potos.Replace(",",@""","""):"");
                

            }
            return new HttpResponseMessage { Content = new StringContent("[" + str+"]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
    }
}
